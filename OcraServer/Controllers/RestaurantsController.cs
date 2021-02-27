using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OcraServer.Enums;
using OcraServer.Extra;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Models.ViewModels;
using OcraServer.Repositories;

namespace OcraServer.Controllers
{
    [Route("api/Data/[controller]")]
    [Route("api/Data/{lang}/[controller]")]
    public class RestaurantsController : Controller
    {
        private readonly RestaurantRepository _repo;
        private readonly RestaurantPanoramaRepository _panoramaRepo;
        private readonly RestaurantMenuCategoryRepository _menuCategoryRepo;
        private readonly UserFavoriteRestaurantRepository _favRestRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantsController(RestaurantRepository repository, RestaurantPanoramaRepository panoramaRepo, RestaurantMenuCategoryRepository menuCategoryRepo, UserFavoriteRestaurantRepository favRestRepo, UserManager<ApplicationUser> userManager)
        {
            _repo = repository;
            _panoramaRepo = panoramaRepo;
            _menuCategoryRepo = menuCategoryRepo;
            _favRestRepo = favRestRepo;
            _userManager = userManager;
        }

        // GET: api/Data/Restaurants/GetAll
        /// <summary>
        /// Gets all restaurants.
        /// </summary>
        /// <remarks>
        /// Restaurant categories:
        ///
        ///     Categories
        ///     {
        ///        Restaurant = 0,
        ///        Tavern = 1,
        ///        FastFood = 2,
        ///        Cafe = 3,
        ///        Pub = 4    
        ///     }
        ///
        /// </remarks>
        /// <returns>All restaurants.</returns>
        [HttpGet("GetAll")]
        [Produces(typeof(List<RestaurantViewModel>))]
        public async Task<IActionResult> GetAllRestaurants(APIDataLanguage lang)
        {
            var restaurants = await _repo.GetViewAllAsync(lang);
            return Ok(restaurants);
        }

        // GET: api/Data/Restaurants/GetCount
        /// <summary>
        /// Gets count of all restaurants.
        /// </summary>
        /// <returns>Count of all restaurants.</returns>
        [HttpGet("GetCount")]
        [Produces(typeof(CountViewModel))]
        public async Task<IActionResult> GetRestaurantsCount()
        {
            int restaurantsCount = await _repo.GetCountAsync();
            return Ok(new CountViewModel { Count = restaurantsCount });
        }

        // GET: api/Data/Restaurants/GetRange?startPosition=1&count=10
        /// <summary>
        /// Gets restaurants range.
        /// </summary>
        /// <returns>Restaurants range</returns>
        /// <response code="400">If request parameters are invalid</response>
        /// <param name="startPosition">The index of the first restaurant in range(1 based). (Default value = 1).</param>
        /// <param name="count">Desired restaurants count. (Default value = 10)</param>
        [HttpGet("GetRange")]
        [Produces(typeof(List<RestaurantViewModel>))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetRestaurants(APIDataLanguage lang, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
            if (startPosition < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(startPosition)}'" });
            }
            if (count < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(count)}'" });
            }

            var restaurants = await _repo.GetViewRangeAsync(lang, startPosition, count);
            return Ok(restaurants);
        }

        // GET: api/Data/Restaurants/GetRangeByCategory/4
        /// <summary>
        /// Gets count of all restaurants in the given category.
        /// </summary>
        /// <returns>Count of all restaurants in the given category.</returns>
        /// <param name="category">Desired category in string or in int.</param>
        [HttpGet("GetCountByCategory/{category}")]
        [Produces(typeof(CountViewModel))]
        public async Task<IActionResult> GetRestaurantsCountByCategory(RestaurantCategory category)
        {
            int restCount = await _repo.GetCountByCategoryAsync(category);
            return Ok(new CountViewModel { Count = restCount });
        }

        // GET: api/Data/Restaurants/GetRange/Restaurant?startPosition=1&count=10
        /// <summary>
        /// Gets restaurants range in the given category.
        /// </summary>
        /// <returns>Restaurants range in the given category</returns>
        /// <response code="400">If request parameters are invalid</response>
        /// <param name="category">Desired category in string or in int.</param>
        /// <param name="startPosition">The index of the first restaurant in range(1 based). (Default value = 1).</param>
        /// <param name="count">Desired restaurants count. (Default value = 10)</param>
        [HttpGet("GetRangeByCategory/{category}")]
        [ProducesResponseType(400)]
        [Produces(typeof(List<RestaurantViewModel>))]
        public async Task<IActionResult> GetRestaurantsByCategory(APIDataLanguage lang, RestaurantCategory category, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
            if (startPosition < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(startPosition)}'" });
            }
            if (count < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(count)}'" });
            }
            var restaurants = await _repo.GetViewRangeByCategoryAsync(lang, category, startPosition, count);
            return Ok(restaurants);
        }

        // GET: api/Data/Restaurants/GetAllForMap
        /// <summary>
        /// Gets all restaurants for showing in map.
        /// </summary>
        /// <returns>All restaurants for showing in map</returns>
        [HttpGet("GetAllForMap")]
        [Produces(typeof(List<RestaurantForMapViewModel>))]
        public async Task<IActionResult> GetRestaurantsForMap(APIDataLanguage lang)
        {
            var restaurants = await _repo.GetAllForMapAsync(lang);
            return Ok(restaurants);
        }

        // GET: api/Data/Restaurants/GetForMapByCategory?Restaurant=true&Pub=true
        /// <summary>
        /// Gets restaurants in the given categories for showing in map.
        /// </summary>
        /// <remarks>
        /// Remark
        /// </remarks>
        /// <param name="Restaurant"></param>
        /// <param name="Pub"></param>
        /// <param name="FastFood"></param>
        /// <param name="Cafe"></param>
        /// <param name="Tavern"></param>
        /// <param name="lang"></param>
        /// <returns>Restaurants in the given categories for showing in map</returns>
        [HttpGet("GetForMapByCategory")]
        [Produces(typeof(List<RestaurantForMapViewModel>))]
        public async Task<IActionResult> GetRestaurantsForMapByCategory(APIDataLanguage lang, [FromQuery] bool Restaurant, [FromQuery] bool Tavern, [FromQuery] bool FastFood, [FromQuery] bool Cafe, [FromQuery] bool Pub)
        {
            var restaurants = await _repo.GetForMapByCategoriesAsync(lang, Restaurant, Tavern, FastFood, Cafe, Pub);
            return Ok(restaurants);
        }

        // GET: api/Data/Restaurants/GetOne/4
        /// <summary>
        /// Gets certain restaurant.
        /// </summary>
        /// <returns>Certain restaurant with the given identifier.</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <response code="404">If the restaurant with the given id is not found</response>
        /// <param name="id">Restaurant identifier.</param>
        [HttpGet("GetOne/{id}")]
        [Produces(typeof(RestaurantDetailsViewModel))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetRestaurantByID(APIDataLanguage lang, int id)
        {
            if (id < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(id)}'" });
            }

            RestaurantDetailsViewModel restaurant = (await _repo.GetViewOneAsync(lang, id)).FirstOrDefault();
            if (restaurant == null)
            {
                return NotFound();
            }

            string idFromClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idFromClaim == null)
            {
                restaurant.IsInFavorites = null;
            }
            else
            {
                var favRestCount = await _favRestRepo.GetCountByRestAndUserAsync(restaurant.ID, idFromClaim);
                restaurant.IsInFavorites = (favRestCount != 0);
            }

            if (restaurant.Brand != null)
            {
                restaurant.SubRestaurants = await _repo.GetViewByBrandAsync(lang, restaurant.Brand, restaurant.ID);
            }

            return Ok(restaurant);
        }

        //// GET: api/Data/Restaurants/GetAllByBrand
        ///// <summary>
        ///// Gets all restaurants with given brand.
        ///// </summary>
        ///// <returns>All restaurants with given brand.</returns>
        //[HttpGet("GetAllByBrand")]
        //[Produces(typeof(List<RestaurantViewModel>))]
        //public async Task<IActionResult> GetAllRestaurantsByBrand(APIDataLanguage lang)
        //{
        //	var restaurants = await _repo.GetViewByBrandAsync(lang, restaurant.Brand);
        //	return Ok(restaurants);
        //}


        // GET: api/Data/Restaurants/GetByAgent/4
        /// <summary>
        /// Gets restaurant of the given agent.
        /// </summary>
        /// <remarks>
        /// NOTE: Only agent is allowed to use this query.
        /// </remarks>
        /// <returns>Restaurant of the given agent</returns>
        /// <response code="401">If user is not authorized (Only agents are allowed)</response>
        /// <response code="404">If the restaurant is not found</response>
        [Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
        [HttpGet("GetByAgent")]
        [Produces(typeof(RestaurantForAgentViewModel))]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetRestaurantByAgent()
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            var restaurant = await _repo.GetViewByAgentAsync(user.Id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return Ok(restaurant);
        }

        // GET: api/Data/Restaurants/GetPanoramas/4
        /// <summary>
        /// Gets panoramas of the given restaurant.
        /// </summary>
        /// <returns>Panoramas of the given restaurant</returns>
        [HttpGet("GetPanoramas/{restaurantID}")]
        [Produces(typeof(List<RestaurantPanorama>))]
        public async Task<IActionResult> GetPanoramasByRest(int restaurantID)
        {
            var panoramas = await _panoramaRepo.GetByRestID(restaurantID);
            return Ok(panoramas);
        }

		// GET: api/Data/Restaurants/GetMenuCategories/4
		/// <summary>
		/// Gets menu categories of the given restaurant.
		/// </summary>
		/// <returns>Menu categories of the given restaurant.</returns>
		/// <param name="restaurantID">Restaurant identifier.</param>
		[HttpGet("GetMenuCategories/{restaurantID}")]
        [Produces(typeof(List<RestaurantMenuCategoryViewModel>))]
        public async Task<IActionResult> GetMenuCategories(APIDataLanguage lang, int restaurantID)
        {
            var categories = await _menuCategoryRepo.GetByRestIDAsync(lang, restaurantID);
            return Ok(categories);
        }

		// GET: /api/Data/Restaurants/GetDataForReservation/2
		/// <summary>
		/// Gets the restaurant data for reservation.
		/// </summary>
		/// <returns>The restaurant data for reservation.</returns>
		/// <param name="restaurantID">Restaurant identifier.</param>
		/// <response code="404">If the restaurant with the given id is not found</response>
		[HttpGet("GetDataForReservation/{restaurantID}")]
        [Produces(typeof(RestaurantDataForReservation))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDataForReservation(APIDataLanguage lang, int restaurantID)
        {
            RestaurantDataForReservation data = (await _repo.GetRestTableCountNPanoramasAsync(lang, restaurantID)).FirstOrDefault();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }


        //// POST: api/Data/RateRestaurant/48/4
        //[Authorize(Roles = Constants.ApplicationRoles.CLIENT_ROLE)]
        //[HttpPost("RateRestaurant/{restID}/{mark}")]
        //public async Task<IActionResult> RateRestaurant(int restID, int mark)
        //{
        //    if (mark > 5 || mark < 1)
        //    {
        //        return BadRequest("Rating must be in [1,5]");
        //    }

        //    ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //    if (user == null || !user.IsActive)
        //    {
        //        return Forbid();
        //    }

        //    var restaurant = await _repo.GetOneAsync(restID);
        //    if (restaurant == null || !restaurant.IsActive)
        //    {
        //        return NotFound();
        //    }

        //    var ratedRest = user.RatedRestaurants.FirstOrDefault(rr => rr.RestaurantID == restID);

        //    if (ratedRest != null)
        //    {
        //        ratedRest.Mark = mark;
        //        ratedRest.RatedTime = DateTime.Now;

        //        await _ratedRestRepo.SaveAsync(ratedRest);
        //    }
        //    else
        //    {
        //        var userRatedRestaurant = new UserRatedRestaurant
        //        {
        //            Mark = mark,
        //            RestaurantID = restID,
        //            UserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
        //            RatedTime = DateTime.Now
        //        };

        //        await _ratedRestRepo.AddAsync(userRatedRestaurant);

        //    }

        //    return Ok();
        //}


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantExists(int id)
        {
            return _repo.Exists(r => r.ID == id && r.IsActive);
        }
    }
}
