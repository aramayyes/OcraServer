using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OcraServer.EntityFramework;
using OcraServer.Extra;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Repositories;
using OcraServer.Services;

namespace OcraServer.Infrastructure
{
    public class JwtTokenProvider
    {
        #region Private Members

        private readonly RequestDelegate _next;

        // JWT-related members
        private readonly TimeSpan _bTokenExpiration;
        private readonly TimeSpan _cTokenExpiration;
        private readonly SigningCredentials _signingCredentials;

        // Refresh-token members
        private readonly TimeSpan _refreshTokenExpiration;

        // EF and Identity members, available through DI
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationUserRepository _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly RefreshTokenRepository _refreshTokenRepo;
        private readonly IRefreshTokenGenerator _tokenGenerator;


        #endregion Private Members

        #region Static Members
        private static readonly string PrivateKey = "private_key_1234567890";
        public static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(PrivateKey));
        public static readonly string Issuer = "BamiaAM";
        public static readonly string Audience = "BamiaAMAud";
        public static string BTokenEndpoint = "/BToken"; // Business token endpoint
        public static string CTokenEndpoint = "/CToken"; // Customer token endpoint
        #endregion Static Members

        #region Private Members
        private const string API_KEY = "AIzaSyBsgDv0CTD6v5vybNCppbiDMJovjVVk4HQ";
        #endregion Private Members

        #region Constructors
        public JwtTokenProvider(RequestDelegate next, ApplicationDbContext dbContext,
                                ApplicationUserRepository userRepo,
        UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, RefreshTokenRepository refreshTokenRepo, IRefreshTokenGenerator tokenGenerator)
        {
            _next = next;

            // Instantiate JWT-related members
            _bTokenExpiration = TimeSpan.FromDays(3);
            _cTokenExpiration = TimeSpan.FromHours(2);
            _signingCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

            // Instantiate Refresh-Token relatet members
            _refreshTokenExpiration = TimeSpan.FromDays(180);
            _refreshTokenRepo = refreshTokenRepo;

            // Instantiate through Dependency Injection
            _dbContext = dbContext;
            _userRepo = userRepo;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

            _tokenGenerator = tokenGenerator;
        }
        #endregion Constructors

        #region Public Methods
        public Task Invoke(HttpContext httpContext)
        {
            // Check which token is requested. (B or C)
            if (httpContext.Request.Path.Equals(BTokenEndpoint, StringComparison.Ordinal))
            {
                return CheckBToken(httpContext);
            }
            else if (httpContext.Request.Path.Equals(CTokenEndpoint, StringComparison.Ordinal))
            {
                return CheckCToken(httpContext);
            }
            else
            {
                return _next(httpContext);
            }
        }


        public async Task CheckBToken(HttpContext httpContext)
        {
            // Check if the current request is a valid POST with the appropriate content type(application/ x - www - form - urlencoded)
            if (httpContext.Request.Method.Equals("POST") && httpContext.Request.HasFormContentType)
            {
                try
                {
                    // Retrieve the relevant FORM data
                    string username = httpContext.Request.Form["username"];
                    string password = httpContext.Request.Form["password"];

                    // Check if there's an user with the given username
                    var user = await _userManager.FindByNameAsync(username);

                    // May be user have changed password
                    if (user != null)
                    {
                        await _dbContext.Entry(user).ReloadAsync();
                    }

                    if (user != null && user.IsActive && await _userManager.CheckPasswordAsync(user, password))
                    {
                        string token = CreateBToken(user);

                        // return token
                        httpContext.Response.ContentType = "application/json";
                        await httpContext.Response.WriteAsync(token);
                        return;
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 400;
                        await httpContext.Response.WriteAsync("Invalid username or password.");
                    }
                }
                catch
                {
                    httpContext.Response.StatusCode = 400;
                    await httpContext.Response.WriteAsync("Invalid username or password.");
                    // TODO handle errors
                }

            }
            else
            {
                // Not OK: output a 400 - Bad request HTTP error.
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Bad request.");
                return;
            }
        }

        public async Task CheckCToken(HttpContext httpContext)
        {
            try
            {
                string tokenType = httpContext.Request.Form["grant_type"];
                switch (tokenType)
                {
                    case "access_token":
                        await ManageAccessToken(httpContext);
                        break;
                    case "refresh_token":
                        await ManageRefreshToken(httpContext);
                        break;
                    default:
                        httpContext.Response.StatusCode = 400;
                        await httpContext.Response.WriteAsync("Invalid grant type");
                        return;
                }
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Invalid grant type");
                return;

                //TODO handle errors
            }

        }
        #endregion public methods

        #region Private Methods
        private async Task ManageAccessToken(HttpContext httpContext)
        {
            // Read token from request data
            string aToken = httpContext.Request.Form["access_token"];

            if (await VerifyToken(aToken))
            {
                UserData userData = await GetUserPhoneNumber(aToken);
                if (userData == null || userData.PhoneNumber == null || userData.UID == null)
                {
                    httpContext.Response.StatusCode = 400;
                    await httpContext.Response.WriteAsync("Invalid token");
                    return;
                }

                // Check if there's an user with the given username
                var user = await _userRepo.GetWithRefreshTokens(userData.PhoneNumber); //_userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.UserName == userData.PhoneNumber); //FindByNameAsync(userData.PhoneNumber);
                bool isFirstLogin = false;
                if (user == null)
                {
                    user = await RegisterUser(userData.PhoneNumber, userData.UID);
                    isFirstLogin = true;
                }
                else if (!user.IsActive)
                {
                    httpContext.Response.StatusCode = 400;
                    await httpContext.Response.WriteAsync("Invalid token");
                    return;
                }

                // Create refresh-token
                string refreshToken = await CreateRefreshToken(user);

                // Create access token (also include refresh-token)
                var token = CreateCToken(user, refreshToken, isFirstLogin);


                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(token);
                return;
            }
            else
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Invalid token");
                return;
            }
        }

        private async Task ManageRefreshToken(HttpContext httpContext)
        {
            // Read token from request data
            string rToken = httpContext.Request.Form["refresh_token"];

            // Validate refresh-token
            var refreshToken = await _refreshTokenRepo.GetByToken(rToken);
            if (refreshToken == null || !refreshToken.IsActive || refreshToken.ExpirationDate <= DateTime.UtcNow)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Invalid token");
                return;
            }

            var user = refreshToken.User;
            if (user != null)
            {
                await _dbContext.Entry(user).ReloadAsync();
            }

            // Create new refresh-token
            string refreshTokenString = await UpdateRefreshToken(refreshToken);

            // Create access-token (also include refresh token)
            var token = CreateCToken(user, refreshTokenString, false);

            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(token);
            return;
        }

        private string CreateCToken(ApplicationUser user, string refreshToken, bool isFirstLogin = false)
        {
            DateTime now = DateTime.UtcNow;

            // Add the registered claims for JWT (RFC7519).
            // For more info, see https: tools.ietf.org/html/rfc7519#section-4.1
            List<Claim> claims = new List<Claim> {
                        new Claim(JwtRegisteredClaimNames.Iss, Issuer),
                        new Claim(JwtRegisteredClaimNames.Aud, Audience),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)};


            claims.Add(new Claim("roles", Constants.ApplicationRoles.CLIENT_ROLE));

            // Create the JWT and write it to a string
            var token = new JwtSecurityToken(claims: claims,
                notBefore: now,
                expires: now.Add(_cTokenExpiration),
                signingCredentials: _signingCredentials);

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            var jwt = new
            {
                access_token = encodedToken,
                token_type = "Bearer",
                expires_in = (int)_cTokenExpiration.TotalSeconds,
                issued = now,
                expires = now.Add(_cTokenExpiration),
                refresh_token = refreshToken,
                user = new
                {
                    isFirstLogin,
                    ID = user.Id,
                    user.UserName,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.EmailConfirmed,
                    user.ImgUrl,
                    user.Points,
                    roles = new[] { Constants.ApplicationRoles.CLIENT_ROLE },
                }
            };
            return JsonConvert.SerializeObject(jwt, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" });

        }


        private string CreateBToken(ApplicationUser user)
        {
            DateTime now = DateTime.UtcNow;

            // Add the registered claims for JWT (RFC7519).
            // For more info, see https: tools.ietf.org/html/rfc7519#section-4.1
            List<Claim> claims = new List<Claim> {
                        new Claim(JwtRegisteredClaimNames.Iss, Issuer),
                        new Claim(JwtRegisteredClaimNames.Aud, Audience),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)};

            //Add additional claims

            claims.Add(new Claim("roles", Constants.ApplicationRoles.AGENT_ROLE));


            // Create the JWT and write it to a string
            var token = new JwtSecurityToken(claims: claims,
                notBefore: now,
                expires: now.Add(_bTokenExpiration),
                signingCredentials: _signingCredentials);

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Build the json response
            var jwt = new
            {
                access_token = encodedToken,
                token_type = "Bearer",
                expires_in = (int)_bTokenExpiration.TotalSeconds,
                userName = user.UserName,
                roles = new[] { Constants.ApplicationRoles.AGENT_ROLE },
                issued = now,
                expires = now.Add(_bTokenExpiration),
				user = new
				{
					ID = user.Id,
					user.UserName,
					user.FirstName,
					user.LastName,
					user.Email,
					user.EmailConfirmed,
					user.ImgUrl,
					roles = new[] { Constants.ApplicationRoles.CLIENT_ROLE },
				}
            };
            return JsonConvert.SerializeObject(jwt, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" });
        }

        private async Task<string> CreateRefreshToken(ApplicationUser user)
        {
            var now = DateTime.UtcNow;
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            var tokens = user.RefreshTokens.ToList();
            if (tokens.Count() >= 5)
            {
                var firstToken = tokens.OrderBy(t => t.ExpirationDate).FirstOrDefault();
                firstToken.IsActive = true;
                firstToken.ExpirationDate = now.Add(_refreshTokenExpiration);
                firstToken.Token = refreshToken;

                try
                {
                    await _refreshTokenRepo.SaveAsync(firstToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    refreshToken = null;
                }
                catch (DbUpdateException)
                {
                    refreshToken = null;
                }
            }
            else
            {
                try
                {
                    await _refreshTokenRepo.AddAsync(new RefreshToken
                    {
                        IsActive = true,
                        Token = refreshToken,
                        UserID = user.Id,
                        ExpirationDate = now.Add(_refreshTokenExpiration)
                    });
                }
                catch (DbUpdateConcurrencyException)
                {
                    refreshToken = null;
                }
                catch (DbUpdateException)
                {
                    refreshToken = null;
                }
            }

            return refreshToken;
        }

        private async Task<string> UpdateRefreshToken(RefreshToken token)
        {
            var now = DateTime.UtcNow;
            var refreshToken = _tokenGenerator.GenerateRefreshToken();


            token.IsActive = true;
            token.ExpirationDate = now.Add(_refreshTokenExpiration);
            token.Token = refreshToken;

            try
            {
                await _refreshTokenRepo.SaveAsync(token);
            }
            catch (DbUpdateConcurrencyException)
            {
                refreshToken = null;
            }
            catch (DbUpdateException)
            {
                refreshToken = null;
            }

            return refreshToken;

        }

        private async Task<bool> VerifyToken(string aToken)
        {
            if (string.IsNullOrEmpty(aToken))
            {
                return false;
            }

            using (HttpClient client = new HttpClient())
            {
                var jsonResult = await client.GetStringAsync("https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com");

                //Convert JSON Result
                var x509Metadata = JObject.Parse(jsonResult)
                                    .Children()
                                    .Cast<JProperty>()
                                    .Select(i => new X509Metadata(i.Path, i.Value.ToString()));
                //Extract IssuerSigningKeys
                var issuerSigningKeys = x509Metadata.Select(s => s.X509SecurityKey);

                //Setup JwtTokenHandler 
                var handler = new JwtSecurityTokenHandler();
                SecurityToken token;

                // Validate JWT token
                try
                {
                    handler.ValidateToken(aToken, new TokenValidationParameters
                    {
                        IssuerSigningKeys = issuerSigningKeys,
                        ValidAudience = "bamia-9fe92",
                        ValidIssuer = "https://securetoken.google.com/bamia-9fe92",
                        IssuerSigningKeyResolver = (arbitrarily, declaring, these, parameters) => issuerSigningKeys
                    }, out token);
                }
                catch
                {
                    return false;
                }
                if (token == null)
                {
                    return false;
                }

                return true;
            }
        }

        private async Task<UserData> GetUserPhoneNumber(string aToken)
        {
            if (string.IsNullOrEmpty(aToken))
            {
                return null;
            }

            using (HttpClient client = new HttpClient())
            {
                var body = new
                {
                    idToken = aToken
                };

                try
                {
                    var bodyJson = JsonConvert.SerializeObject(body);
                    var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    var result = await client.PostAsync($"https://www.googleapis.com/identitytoolkit/v3/relyingparty/getAccountInfo?key={API_KEY}", content);

                    var json = JObject.Parse(await result.Content.ReadAsStringAsync());
                    var phoneNumber = json["users"][0]["phoneNumber"].ToString();
                    var uID = json["users"][0]["localId"].ToString();

                    return new UserData
                    {
                        PhoneNumber = phoneNumber,
                        UID = uID
                    };
                }
                catch
                {
                    return null; // TODO handle errors
                }
            }
        }

        private async Task<ApplicationUser> RegisterUser(string phoneNumber, string firebaseUID)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = phoneNumber,
                FirstName = "Unknown",
                LastName = "Unknown",
                FirebaseUID = firebaseUID,
                Email = phoneNumber,
                PhoneNumber = phoneNumber,
                ImgUrl = "/Files/Photos/Users/Default.png",
                Points = Constants.UserPoints.REGISTRATION
            };


            IdentityResult result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                // TODO Handle Errors
            }

            bool isInRole = await _userManager.IsInRoleAsync(user, Constants.ApplicationRoles.CLIENT_ROLE);
            if (!isInRole)
            {
                if (!(await _roleManager.RoleExistsAsync(Constants.ApplicationRoles.CLIENT_ROLE)))
                {
                    IdentityRole role = new IdentityRole(Constants.ApplicationRoles.CLIENT_ROLE);
                    await _roleManager.CreateAsync(role);
                }
                await _userManager.AddToRoleAsync(user, Constants.ApplicationRoles.CLIENT_ROLE);
            }

            return user;
        }

        #endregion Private Methods

    }

    #region Extension Methods
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class JwtProviderExtensions
    {
        public static IApplicationBuilder UseJwtProvider(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtTokenProvider>();
        }
    }
    #endregion Extension Methods
}


public class X509Metadata
{
    public string KID { get; set; }
    public string Certificate { get; set; }
    public X509SecurityKey X509SecurityKey { get; set; }

    public X509Metadata(string kid, string certificate)
    {
        KID = kid;
        Certificate = certificate;
        X509SecurityKey = BuildSecurityKey(Certificate);
    }

    private X509SecurityKey BuildSecurityKey(string certificate)
    {
        //Remove : -----BEGIN CERTIFICATE----- & -----END CERTIFICATE-----
        var lines = certificate.Split('\n');
        var selectedLines = lines.Skip(1).Take(lines.Length - 3);
        var key = string.Join(Environment.NewLine, selectedLines);

        return new X509SecurityKey(new X509Certificate2(Convert.FromBase64String(key)));
    }
}

public class UserData
{
    public string PhoneNumber { get; set; }
    public string UID { get; set; }
}
