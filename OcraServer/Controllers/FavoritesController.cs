using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OcraServer.Enums;
using OcraServer.Extra;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Models.ViewModels;
using OcraServer.Repositories;

namespace OcraServer.Controllers
{
    [Authorize(Roles = Constants.ApplicationRoles.CLIENT_ROLE)]
    [Route("api/Data/[controller]")]
    [Route("api/Data/{lang}/[controller]")]
    public class FavoritesController : Controller
    {
        private readonly UserFavoriteRestaurantRepository _repo;
        private readonly RestaurantRepository _restRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoritesController(UserFavoriteRestaurantRepository repo, RestaurantRepository restRepo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _restRepo = restRepo;
            _userManager = userManager;
        }

        // GET: api/Data/Favorites/GetAll
        /// <summary>
        /// Gets user all favorite restaurants.
        /// </summary>
        /// <returns>User all favorite restaurants</returns>
        /// <response code="401">If user is not authorized (Only clients are allowed)</response>
        [HttpGet("GetAll")]
        [ProducesResponseType(401)]
        [Produces(typeof(List<RestaurantViewModel>))]
        public async Task<IActionResult> GetUserAllFavoriteRests(APIDataLanguage lang)
        {
            var restaurants = await _repo.GetViewByUserAsync(lang, User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return Ok(restaurants);
        }

        // GET: api/Data/Favorites/GetCount
        /// <summary>
        /// Gets user all favorite restaurants count.
        /// </summary>
        /// <returns>User all favorite restaurants count</returns>
        /// <response code="401">If user is not authorized (Only clients are allowed)</response>
        [HttpGet("GetCount")]
        [Produces(typeof(CountViewModel))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetUserFavoriteRestsCount()
        {
            int restaurantsCount = await _repo.CountByUserAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Ok(new CountViewModel { Count = restaurantsCount });
        }


        // GET: api/Data/Favorites/GetRange
        /// <summary>
        /// Gets user favorite restaurants range.
        /// </summary>
        /// <returns>User favorite restaurants range</returns>
        /// <response code="400">If request parameters are invalid</response>
        /// <response code="401">If user is not authorized (Only clients are allowed)</response>
        /// <param name="startPosition">The index of the first discount in range(1 based). (Default value = 1).</param>
        /// <param name="count">Desired discounts count. (Default value = 10)</param>
        [HttpGet("GetRange")]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(401)]
        [Produces(typeof(List<RestaurantViewModel>))]
        public async Task<IActionResult> GetUserFavoriteRests(APIDataLanguage lang, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
            if (startPosition < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(startPosition)}'" });
            }
            if (count < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(count)}'" });
            }

            var restaurants = await _repo.GetRangeByUserAsync(lang, User.FindFirst(ClaimTypes.NameIdentifier).Value, startPosition, count);
            return Ok(restaurants);
        }

        // POST: api/Data/AddToFavorites/45
        /// <summary>
        /// Adds the given restaurant to the user favorites.
        /// </summary>
        /// <response code="401">If user is not authorized (Only clients are allowed)</response>
        /// <param name="restaurantID">Restaurant identifier.</param>
        [HttpPost("AddToFavorites/{restaurantID}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> PostUserFavoriteRest(int restaurantID)
        {
            try
            {
                var userFavRest = new UserFavoriteRestaurant
                {
                    RestaurantID = restaurantID,
                    UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    AddedDate = DateTime.UtcNow
				};

				await _repo.AddAsync(userFavRest);

				return NoContent();
            }
            catch (DbUpdateException)
            {
                return NoContent();
            }
        }

		// DELETE: api/Data/RemoveFromFavorites/45
		/// <summary>
		/// Removes the given restaurant from the user favorites.
		/// </summary>
		/// <response code="401">If user is not authorized (Only clients are allowed)</response>
		/// <param name="restaurantID">Restaurant identifier.</param>
		[HttpDelete("RemoveFromFavorites/{restaurantID}")]
		[ProducesResponseType(204)]
		public async Task<IActionResult> DeleteUserFavoriteRest(int restaurantID)
        {
			var userFavRest = new UserFavoriteRestaurant
			{
				RestaurantID = restaurantID,
				UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value,
			};

            try
            {
				await _repo.DeleteAsync(userFavRest);
				return NoContent();
            }
            catch (Exception)
            {
                return NoContent();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
                _restRepo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
