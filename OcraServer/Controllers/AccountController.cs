using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using OcraServer.Enums;
using OcraServer.Extra;
using OcraServer.Models.BindingModels;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace OcraServer.Controllers
{
    [Authorize]
    [Route("api/Data/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHostingEnvironment _appEnvironment;

        private readonly IReadOnlyCollection<string> _allowedFormats = new List<string>
        {
            "image/jpeg",
            "image/jpg",
            "image/png"
        };

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IHostingEnvironment appEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appEnvironment = appEnvironment;
        }

        //[AllowAnonymous]
        //[HttpDelete("DeleteOne")]
        //public async Task<IActionResult> DeleteUser()
        //{
        //    ApplicationUser user = await _userManager.FindByNameAsync("+37477687852");
        //    var result  = await _userManager.DeleteAsync(user);

        //    return Ok();
        //}

        // Get api/Data/Account/GetProfile
        /// <summary>
        /// Gets the user profile.
        /// </summary>
        /// <returns>The user profile.</returns>
        [HttpGet("GetProfile")]
        [Produces(typeof(ApplicationUserViewModel))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetUserProfile()
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }
            Mapper.Initialize(cfg => cfg.CreateMap<ApplicationUser, ApplicationUserViewModel>());
            return Ok(Mapper.Map<ApplicationUserViewModel>(user));
        }

        // POST api/Account/ChangePassword
        /// <summary>
        /// Changes the user password.
        /// </summary>
        /// <remarks>
        /// NOTE: Only agent is allowed to change his/her password.
        /// </remarks>
        /// <returns>Nothing</returns>
        /// <response code="400">If request parameters (or body) is invalid</response>
        /// <param name="model">New password</param>
        [Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
        [HttpPost("ChangePassword")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return NoContent();
        }


        // PUT api/Account/ChangeProfile
        /// <summary>
        /// Changes the user profile data.
        /// </summary>
        /// <remarks>
        /// NOTE: Only client is allowed to change his/her profile data.
        /// </remarks>
        /// <returns>Nothing</returns>
        /// <response code="400">If request parameters (or body) is invalid</response>
        /// <param name="model">User new profile data</param>
        [Authorize(Roles = Constants.ApplicationRoles.CLIENT_ROLE)]
        [HttpPut("ChangeProfile")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ChangeUserProfile([FromBody] ApplicationUserEditBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            if (!user.HasFilled)
            {
                user.HasFilled = true;
                user.Points += Constants.UserPoints.PROFILE_DATA;
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.City = model.City;
            user.Sex = model.Sex;
            user.DateOfBirth = model.DateOfBirth;

            await _userManager.UpdateAsync(user);
            return NoContent();
        }

        // PUT api/Account/ChangeImage
        /// <summary>
        /// Changes the user avatar image.
        /// </summary>
        /// <remarks>
        /// NOTE: Only client is allowed to change his/her profile data.
        /// </remarks>
        /// <returns>New image url</returns>
        /// <response code="400">If request parameters (or body) is invalid or file is too large</response>
        [Authorize(Roles = Constants.ApplicationRoles.CLIENT_ROLE)]
        [HttpPut("ChangeImage")]
        [Produces(typeof(ProfileImageViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ChangeUserImage(IFormFile newImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            // Make sure uploaded file is an image
            if (newImage == null || !_allowedFormats.Contains(newImage.ContentType))
            {
                return BadRequest(new ErrorResponse { Message = "Invalid image or image format. Only '.jpeg', '.jpg', '.png' are allowed." });
            }

            /* If user already has an image, just overwrite that file
             * Otherwise, create a new file (image)
             */

            try
            {
                string imgUrl = user.ImgUrl;
                string photoID = imgUrl.Substring(imgUrl.LastIndexOf('/'));
                string path = Constants.StaticFiles.USER_IMAGES_PATH + photoID;

                if (photoID == Constants.StaticFiles.DEFAULT_IMAGE)
                {
                    path = Constants.StaticFiles.USER_IMAGES_PATH + "/" + Guid.NewGuid().ToString() + newImage.FileName.Substring(newImage.FileName.LastIndexOf('.'));
                }

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await newImage.CopyToAsync(fileStream);
                }

                user.ImgUrl = path;
                await _userManager.UpdateAsync(user);

                return Ok(new ProfileImageViewModel { ImgUrl = user.ImgUrl });
            }
            catch
            {
                return BadRequest(new ErrorResponse { Message = "Invalid image" });
            }
        }

        // PUT api/Account/DeleteImage
        /// <summary>
        /// Deletes the user avatar image.
        /// </summary>
        /// <remarks>
        /// NOTE: Only client is allowed to delete his/her profile data.
        /// </remarks>
        /// <returns>Nothing</returns>
        /// <response code="400">If there is no image to delete</response>
        [Authorize(Roles = Constants.ApplicationRoles.CLIENT_ROLE)]
        [HttpPut("DeleteImage")]
        [Produces(typeof(ProfileImageViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DeleteUserImage()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            string imgUrl = user.ImgUrl;
            string photoID = imgUrl.Substring(imgUrl.LastIndexOf('/'));
            
            if (photoID == Constants.StaticFiles.DEFAULT_IMAGE)
            {
                return BadRequest(new ErrorResponse { Message = "There are no images to delete" });
            }

            string path = Constants.StaticFiles.USER_IMAGES_PATH + "/" + photoID;
            FileInfo file = new FileInfo(_appEnvironment.WebRootPath + path);

            if (file.Exists)
            {
                file.Delete();
            }

            user.ImgUrl = Constants.StaticFiles.USER_IMAGES_PATH + Constants.StaticFiles.DEFAULT_IMAGE;
            await _userManager.UpdateAsync(user);

            return Ok(new ProfileImageViewModel { ImgUrl = user.ImgUrl });
        }

        // PUT api/Account/ChangeNotificationToken
        /// <summary>
        /// Changes the user registration token for notification.
        /// </summary>
        /// <remarks>
        /// Platforms:
        ///
        ///     Platforms
        ///     {
        ///          Web = 0,
        ///          Android = 1,
        ///          iOS = 2
        ///     }
        ///
        /// </remarks>
        /// <returns>Nothing</returns>
        /// <response code="400">If request parameters (or body) is invalid</response>
        /// <param name="model">User new registration token</param>
        [Authorize(Roles = Constants.ApplicationRoles.CLIENT_ROLE)]
        [HttpPut("ChangeNotificationToken")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ChangeUserNotificationToken([FromBody] ApplicationUserRegistrationTokenBindingModel model)
        {
            if (!ModelState.IsValid || model.DeviceID == string.Empty)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            switch (model.DevicePlatform)
            {
                case ClientPlatform.Android:
                    user.FirebaseNotificationTokenAndroid = model.DeviceID;
                    break;
                case ClientPlatform.iOS:
                    user.FirebaseNotificationTokeniOS = model.DeviceID;
                    break;
                case ClientPlatform.Web:
                default:
                    user.FirebaseNotificationTokenWeb = model.DeviceID;
                    break;
            }

            await _userManager.UpdateAsync(user);
            return NoContent();
        }

        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

    }
    
}