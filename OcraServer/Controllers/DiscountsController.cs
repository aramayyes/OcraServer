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
using OcraServer.Models.BindingModels;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Models.ViewModels;
using OcraServer.Repositories;

namespace OcraServer.Controllers
{
    [Route("api/Data/[controller]")]
    [Route("api/Data/{lang}/[controller]")]
    public class DiscountsController : Controller
    {
        private readonly DiscountRepository _repo;
        private readonly RestaurantRepository _restRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiscountsController(DiscountRepository repo, RestaurantRepository restRepo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _restRepo = restRepo;
            _userManager = userManager;
        }

        // GET: api/Data/Discounts/GetAll
        /// <summary>
        /// Gets all discounts.
        /// </summary>
        /// <returns>All discounts.</returns>
        [HttpGet("GetAll")]
        [Produces(typeof(List<DiscountViewModel>))]
        public async Task<IActionResult> GetAllDiscounts(APIDataLanguage lang)
        {
            var discounts = await _repo.GetViewAllAsync(lang);
            return Ok(discounts);
        }

        // GET: api/Data/Discounts/GetCount
        /// <summary>
        /// Gets count of all discounts.
        /// </summary>
        /// <returns>Count of all discounts.</returns>
        [HttpGet("GetCount")]
        [Produces(typeof(CountViewModel))]
        public async Task<IActionResult> GetDiscountsCount()
        {
            int discountsCount = await _repo.GetCountAsync();
            return Ok(new CountViewModel { Count = discountsCount });
        }

        // GET: api/Data/Discounts/GetRange?startPosition=1&count=10
        /// <summary>
        /// Gets discounts range.
        /// </summary>
        /// <returns>Discounts range</returns>
        /// <response code="400">If request parameters are invalid</response>
        /// <param name="startPosition">The index of the first discount in range(1 based). (Default value = 1).</param>
        /// <param name="count">Desired discounts count. (Default value = 10)</param>
        [HttpGet("GetRange")]
        [Produces(typeof(List<DiscountViewModel>))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetDiscounts(APIDataLanguage lang, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
            if (startPosition < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(startPosition)}'" });
            }
            if (count < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(count)}'" });
            }

            var discounts = await _repo.GetViewRangeAsync(lang, startPosition, count);
            return Ok(discounts);
        }

        // GET: api/Data/Discounts/GetAllByRest/4
        /// <summary>
        /// Gets all discounts of the given restaurant.
        /// </summary>
        /// <returns>All discounts of the given restaurant.</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <param name="restaurantID">Restaurant identifier.</param>
        [HttpGet("GetAllByRest/{restaurantID}")]
        [Produces(typeof(List<DiscountViewModel>))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetAllRestDiscounts(APIDataLanguage lang, int restaurantID)
        {
            if (restaurantID < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(restaurantID)}'" });
            }

            var discounts = await _repo.GetViewByRestAsync(lang, restaurantID);
            return Ok(discounts);
        }

        // GET: api/Data/Discounts/GetCountByRest/24
        /// <summary>
        /// Gets count of the given restaurant's discounts.
        /// </summary>
        /// <returns>Count of the given restaurant's discounts.</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <param name="restaurantID">Restaurant identifier.</param>
        [HttpGet("GetCountByRest/{restaurantID}")]
        [Produces(typeof(CountViewModel))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetRestDiscountsCount(int restaurantID)
        {
            if (restaurantID < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(restaurantID)}'" });
            }

            int discountsCount = await _repo.GetCountByRestIDAsync(restaurantID);
            return Ok(new CountViewModel { Count = discountsCount });
        }

        // GET: api/Data/Discounts/GetRangeByRest/24?startPosition=5&count=45
        /// <summary>
        /// Gets discounts range of the given restaurant.
        /// </summary>
        /// <returns>Discounts range of the given restaurant.</returns>
        /// <response code="400">If request parameters are invalid</response>
        /// <param name="restaurantID">Restaurant identifier.</param>
        /// <param name="startPosition">The index of the first discount in range(1 based). (Default value = 1).</param>
        /// <param name="count">Desired discounts count. (Default value = 10).</param>
        [HttpGet("GetRangeByRest/{restaurantID}")]
        [Produces(typeof(List<DiscountViewModel>))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetRestDiscounts(APIDataLanguage lang, int restaurantID = 0, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
            if (startPosition < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(startPosition)}'" });
            }
            if (count < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(count)}'" });
            }
            if (restaurantID < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(restaurantID)}'" });
            }

            var discounts = await _repo.GetViewRangeByRestAsync(lang, restaurantID, startPosition, count);
            return Ok(discounts);
        }

        // GET: api/Data/Discounts/GetOne/4
        /// <summary>
        /// Gets certain discount.
        /// </summary>
        /// <returns>Certain discount with the given identifier.</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <response code="404">If the discount with the given id is not found</response>
        /// <param name="id">Discount identifier.</param>
        [HttpGet("GetOne/{id}")]
        [Produces(typeof(DiscountViewModel))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetDiscount(APIDataLanguage lang, int id)
        {
            if (id < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(id)}'" });
            }

            DiscountViewModel discount = await _repo.GetViewOneAsync(lang, id);
            if (discount == null)
            {
                return NotFound(new ErrorResponse { Message = $"Discount with the given id={id} was not found" });
            }

            return Ok(discount);
        }

        // GET: api/Data/Discounts/GetOneForEdit/4
        /// <summary>
        /// Gets certain discount with all information.
        /// </summary>
        /// <returns>Certain discount with the given identifier.</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <response code="401">If user is not authorized (Only agents are allowed)</response>
        /// <response code="403">If agent tries to edit another restaurant's discount</response>
        /// <response code="404">If the discount with the given id is not found</response>
        /// <param name="id">Discount identifier.</param>
        [Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
        [HttpGet("GetOneForEdit/{id}")]
        [Produces(typeof(Discount))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetDiscountForEdit(int id)
        {
            if (id < 1)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            var entity = await _repo.GetOneAsync(id);
            if (entity == null || !entity.IsActive)
            {
                return NotFound(new ErrorResponse { Message = $"Discount with the given id={id} was not found" });
            }

            if (entity.RestaurantID != user.RestaurantID)
            {
                return Forbid();
            }

            return Ok(entity);
        }

        // PUT: api/Data/EditOne/5
        /// <summary>
        /// Edits the discount with given id.
        /// </summary>
        /// <remarks>
        /// NOTE: Only agent is allowed to edit a discount.
        /// </remarks>
        /// <returns>Nothing</returns>
        /// <response code="400">If request parameters (or body) is invalid</response>
        /// <response code="401">If user is not authorized (Only agents are allowed)</response>
        /// <response code="403">If agent tries to edit other restaurant's discount</response>
        /// <response code="404">If the discount with the given id is not found</response>
        /// <response code="409">If an error occurs while editing the discount</response>
        /// <param name="id">Discount identifier.</param>
        /// <param name="discount">Discount model with new data.</param>
        [Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
        [HttpPut("EditOne/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> PutDiscount(int id, [FromBody] DiscountEditBindingModel discount)
        {
            if (id < 1 || !ModelState.IsValid || discount == null || discount.ID != id)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            var entity = await _repo.GetOneAsync(id);
            if (entity == null || !entity.IsActive)
            {
                return NotFound(new ErrorResponse { Message = $"Discount with the given id={id} was not found" });
            }

            if (entity.RestaurantID != user.RestaurantID)
            {
                return Forbid();
            }

            entity.NameEng = discount.NameEng;
            entity.NameRus = discount.NameRus;
            entity.NameArm = discount.NameArm;

            entity.DescriptionEng = discount.DescriptionEng;
            entity.DescriptionRus = discount.DescriptionRus;
            entity.DescriptionArm = discount.DescriptionArm;

            entity.Deadline = discount.Deadline;

            entity.DiscountSize = discount.DiscountSize;
            entity.NewPrice = discount.NewPrice;

            entity.ImgLink = discount.ImgLink;

            try
            {
                await _repo.SaveAsync(entity);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountExists(id))
                {
                    return NotFound(new ErrorResponse { Message = $"Discount with the given id={id} was not found" });
                }
                else
                {
                    return StatusCode(409, new ErrorResponse { Message = "An error occurred while editing the discount." });
                }
            }
        }

		// POST: api/Data/AddOne
		/// <summary>
		/// Adds a discount.
		/// </summary>
		/// <remarks>
		/// NOTE: Only agent is allowed to add a discount.
		/// </remarks>
		/// <returns>The added discount.</returns>
		/// <response code="400">If request body is invalid</response>
		/// <response code="401">If user is not authorized (Only agents are allowed)</response>
		/// <response code="409">If an error occurs while editing the discount</response>
		/// <param name="discount">The discount which has to be added.</param>
		[Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
        [HttpPost("AddOne")]
        [Produces(typeof(Discount))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> PostDiscount([FromBody] DiscountAddBindingModel discount)
        {
            if (!ModelState.IsValid || discount == null)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            var entity = new Discount
            {
                NameEng = discount.NameEng,
                NameRus = discount.NameRus,
                NameArm = discount.NameArm,

                DescriptionEng = discount.DescriptionEng,
                DescriptionRus = discount.DescriptionRus,
                DescriptionArm = discount.DescriptionArm,

                Deadline = discount.Deadline,
                DiscountSize = discount.DiscountSize,
                NewPrice = discount.NewPrice,

                ImgLink = discount.ImgLink,

                RestaurantID = user.RestaurantID.Value
            };

            try
            {
                await _repo.AddAsync(entity);
                return Created(new Uri(Request.Host.ToString() + Request.PathBase.ToString(), UriKind.RelativeOrAbsolute), entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409, new ErrorResponse { Message = "An error occurred while adding the discount." });
            }
        }

		// DELETE: api/Data/DeleteOne/5
		/// <summary>
		/// Deletes the discount with the given identifiers.
		/// </summary>
		/// <remarks>
		/// NOTE: Only agent is allowed to delete a discount.
		/// </remarks>
		/// <returns>Nothing</returns>
		/// <response code="400">If request parameter is invalid</response>
		/// <response code="401">If user is not authorized (Only agents are allowed)</response>
		/// <response code="403">If agent tries to delete other restaurant's discount</response>
		/// <response code="404">If the discount with the given id is not found</response>
		/// <response code="409">If an error occurs while deleting the discount</response>
		/// <param name="id">Discount identifier.</param>
		[Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
        [HttpDelete("DeleteOne/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            if (id < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(id)}'" });
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            Discount discount = await _repo.GetOneAsync(id);
            if (discount == null || !discount.IsActive)
            {
                return NotFound(new ErrorResponse { Message = $"Discount with the given id={id} was not found" });
            }

            if (discount.RestaurantID != user.RestaurantID)
            {
                return Forbid();
            }

            discount.IsActive = false;

            try
            {
                await _repo.SaveAsync(discount);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountExists(id))
                {
                    return NotFound(new ErrorResponse { Message = $"Discount with the given id={id} was not found" });
                }
                else
                {
                    return StatusCode(409, new ErrorResponse { Message = "An error occurred while deleting the discount." });
                }
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

        private bool DiscountExists(int id)
        {
            return _repo.Exists(d => d.ID == id && d.IsActive);
        }

    }
}
