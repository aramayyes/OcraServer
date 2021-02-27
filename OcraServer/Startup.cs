using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using OcraServer.EntityFramework;
using OcraServer.Infrastructure;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Repositories;
using OcraServer.Services;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using OcraServer.Models.ViewModels;

namespace OcraServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add a reference to the Configuration object for DI
            services.AddSingleton<IConfiguration>(c => Configuration);

            /* TODO */
            // Add Cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowAnyOrigin()
                                      .AllowCredentials()
                    );
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigins"));
            });

            // Setup maximum size for uploading user images
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 4 * 1024 * 1024;
            });
            
            
            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
                });

            // Add EntityFramework's Identity support.
            services.AddEntityFramework();

            // Add Identity Services & Stores
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 5;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireDigit = false;
                config.Password.RequireUppercase = true;
                config.Password.RequireLowercase = true;
                config.Cookies.ApplicationCookie.AutomaticChallenge = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Add ApplicationDbContext.
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            // Add DbSeeder
            services.AddSingleton<DbSeeder>();

            // Add respositories
            services.AddScoped<RestaurantRepository>();
            services.AddScoped<DiscountRepository>();
            services.AddScoped<EventRepository>();
            services.AddScoped<UserFavoriteRestaurantRepository>();
            services.AddScoped<FeedbackRepository>();
            services.AddScoped<ProductRepository>();
            services.AddScoped<ReservationRepository>();
            services.AddScoped<ReservedTableRepository>();
            services.AddScoped<RestaurantPanoramaRepository>();
            services.AddScoped<RestaurantMenuCategoryRepository>();
            services.AddScoped<RefreshTokenRepository>();
            services.AddTransient<ApplicationUserRepository>();


            // Add services
            services.AddTransient<IRefreshTokenGenerator, CryptRefreshTokenGenerator>();
            services.AddTransient<INotificationSender, FirebaseCloudMessaging>();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "360Booking Web API",
                    Version = "v1"
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "OcraServer.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DbSeeder dbSeeder)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Critical);
            //loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Debug);

            // Exception handler middlewear
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null && ex.Error is InvalidDataException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.Response.ContentType = "application/json";

                        var err = JsonConvert.SerializeObject(new ErrorResponse
                        {
                            Message = "File is too large."
                        });

                        await context.Response.Body.WriteAsync(Encoding.ASCII.GetBytes(err), 0, err.Length).ConfigureAwait(false);
                    }
                });
            });

            /*TODO*/
            // Setup Cors Support
            app.UseCors("AllowAllOrigins");
            /*TODO*/

            // Configure a rewrite rule to auto-lookup for standard default files such as index.html.app.UseDefaultFiles();
            // Serve static files (html, css, js, images & more). 
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    // Disable caching for all static files.
                    context.Context.Response.Headers["Cache-Control"] = Configuration["StaticFiles:Headers:Cache-Control"];
                    context.Context.Response.Headers["Pragma"] = Configuration["StaticFiles:Headers:Pragma"];
                    context.Context.Response.Headers["Expires"] = Configuration["StaticFiles:Headers:Expires"];
                }
            });


            // Add a custom Jwt Provider to generate Tokens
            app.UseJwtProvider();
            // Add the Jwt Bearer Header Authentication to validate Tokens
            app.UseJwtBearerAuthentication(new JwtBearerOptions()
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                RequireHttpsMetadata = false,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = JwtTokenProvider.SecurityKey,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = JwtTokenProvider.Issuer,
                    ValidateIssuer = true,

                    ValidAudience = JwtTokenProvider.Audience,
                    ValidateAudience = true,

                    ValidateLifetime = true,
                }
            });

            // Add MVC to the pipeline
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "360Booking Web API");
            });

            // Seed the Database (if needed)
            try
            {
                dbSeeder.SeedAsync().Wait();
            }
            catch (AggregateException e)
            {
                throw new Exception(e.ToString());
            }
        }
    }
}
