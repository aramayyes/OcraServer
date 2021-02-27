using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OcraServer.Enums;
using OcraServer.Models.ViewModels;
using OcraServer.Repositories;

namespace OcraServer.Controllers
{
    [Route("api/Data/[controller]")]
	[Route("api/Data/{lang}/[controller]")]
    public class SearchController : Controller
    {
        private readonly RestaurantRepository _restRepo;
        private readonly ProductRepository _prodRepo;

        public SearchController(RestaurantRepository restRepo, ProductRepository prodRepo)
        {
            _restRepo = restRepo;
            _prodRepo = prodRepo;
        }

		// GET: api/Data/Search/Restaurant?searchTerm=hi
		/// <summary>
        /// Searches for restaurants (Name).
		/// </summary>
		/// <returns>Restaurants which names contain searchTerm.</returns>
		/// <param name="searchTerm">Search text</param>
		[HttpGet("Restaurant")]
        [Produces(typeof(List<RestaurantSearchResultViewModel>))]
        public async Task<IActionResult> GetByString(APIDataLanguage lang, [FromQuery] string searchTerm)
        {
            if(searchTerm == null || searchTerm == string.Empty)
            {
               return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(searchTerm)}'" });
            }

            var restaurants = await  _restRepo.SearchAsync(lang, searchTerm);
            return Ok(restaurants);
        }


        // GET: api/Data/Search/RestaurantSuggestions
        /// <summary>
        /// Gets the suggestions.
        /// </summary>
        /// <returns>The suggestions.</returns>
        [HttpGet("RestaurantSuggestions")]
        [Produces(typeof(List<SearchSuggestionViewModel>))]
        public IActionResult GetSuggestions()
        {
            var restNames = _restRepo.GetNames();

            List<SearchSuggestionViewModel> suggestions = new List<SearchSuggestionViewModel>();

            foreach (var rest in restNames)
            {
                suggestions.Add(new SearchSuggestionViewModel { Name = rest.Name_Eng });
                if (rest.Name_Rus != rest.Name_Eng)
                {
                    suggestions.Add(new SearchSuggestionViewModel { Name = rest.Name_Rus });
                }
                if (rest.Name_Arm != rest.Name_Eng && rest.Name_Arm != rest.Name_Rus)
                {
                    suggestions.Add(new SearchSuggestionViewModel { Name = rest.Name_Arm });
                }
            }
            return Ok(suggestions);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _restRepo.Dispose();
                _prodRepo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
