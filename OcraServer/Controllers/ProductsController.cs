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
    public class ProductsController : Controller
    {
        private readonly ProductRepository _repo;
        private readonly RestaurantRepository _restRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ProductRepository repo, RestaurantRepository restRepo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _restRepo = restRepo;
            _userManager = userManager;
        }

		// GET: api/Data/Products/GetAll
		/// <summary>
		/// Gets all products.
		/// </summary>
		/// <returns>All products.</returns>
	    /// <param name="categoryID">Category identifier</param>
		[HttpGet("GetAll/{categoryID}")]
		[Produces(typeof(List<ProductViewModel>))]
        public async Task<IActionResult> GetAllProducts(APIDataLanguage lang, int categoryID)
        {
            return Ok(await _repo.GetViewAllAsync(lang, categoryID));
        }

		// GET: api/Data/Products/GetCount
		/// <summary>
		/// Gets count of all products.
		/// </summary>
		/// <returns>Count of all products.</returns>
		/// <param name="categoryID">Category identifier</param>
		[HttpGet("GetCount/{categoryID}")]
		[Produces(typeof(CountViewModel))]
        public async Task<IActionResult> GetAllProductsCount(int categoryID)
        {
            int productsCount = await _repo.GetCountAsync(categoryID);
            return Ok(new CountViewModel { Count = productsCount });
        }

		// GET: api/Data/Products/GetRange?startPosition=1&count=10
		/// <summary>
		/// Gets products range.
		/// </summary>
		/// <returns>Products range</returns>
		/// <response code="400">If request parameters are invalid</response>
		/// <param name="startPosition">The index of the first product in range(1 based). (Default value = 1).</param>
		/// <param name="count">Desired products count. (Default value = 10)</param>
        /// <param name="categoryID">Category identifier</param>
        [HttpGet("GetRange/{categoryID}")]
        [Produces(typeof(List<ProductViewModel>))]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetProducts(APIDataLanguage lang, int categoryID, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
			if (startPosition < 1)
			{
				return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(startPosition)}'" });
			}
			if (count < 1)
			{
				return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(count)}'" });
			}

            return Ok(await _repo.GetRangeAsync(lang, categoryID, startPosition, count));
        }

        /*

		// GET: api/Data/Restaurants/GetCountByCategory/4
		/// <summary>
		/// Gets count of all products in the given category.
		/// </summary>
		/// <returns>Count of all products in the given category.</returns>
		/// <param name="category">Desired category in string or in int.</param>
        [HttpGet("GetCountByCategory/{categoryID}/{category}")]
        public async Task<IActionResult> GetProductsByCategoryCount(int category, int categoryID)
        {
            int productsCount = await _repo.GetCountByCategoryAsync(category, categoryID);
            return Ok(new CountViewModel { Count = productsCount });
        }


		// GET: api/Data/Products/GetRange/Restaurant?startPosition=1&count=10
		/// <summary>
		/// Gets restaurants range in the given category.
		/// </summary>
		/// <returns>Restaurants range in the given category</returns>
		/// <response code="400">If request parameters are invalid</response>
		/// <param name="category">Desired category in string or in int.</param>
		/// <param name="startPosition">The index of the first restaurant in range(1 based). (Default value = 1).</param>
		/// <param name="count">Desired restaurants count. (Default value = 10)</param>
        /// <param name="restaurantID">Restaurant identifier</param>
        [HttpGet("GetRangeByCategory/{restaurantID}/{category}")]
        [Produces(typeof(List<ProductViewModel>))]
        public async Task<IActionResult> GetProductsByCategory(APIDataLanguage lang, int restaurantID, int category, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
			if (startPosition < 1)
			{
				return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(startPosition)}'" });
			}
			if (count < 1)
			{
				return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(count)}'" });
			}
            return Ok(await _repo.GetRangeByCategoryAsync(lang, restaurantID, category, startPosition, count));
        }

        */

		// GET: api/Data/Restaurants/GetOne/4
		/// <summary>
		/// Gets certain restaurant.
		/// </summary>
		/// <returns>Certain restaurant with the given identifier.</returns>
		/// <response code="400">If request parameter is invalid</response>
		/// <response code="404">If the restaurant with the given id is not found</response>
		/// <param name="id">Restaurant identifier.</param>
		[HttpGet("GetOne/{id}")]
        [Produces(typeof(ProductViewModel))]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
		[ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetProduct(APIDataLanguage lang, int id)
        {
			if (id < 1)
			{
				return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(id)}'" });
			}

            ProductViewModel product = await _repo.GetViewOneAsync(lang, id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }


		// GET: api/Data/Products/GetOneForEdit/4
		/// <summary>
		/// Gets certain product with all information.
		/// </summary>
		/// <returns>Certain product with the given identifier.</returns>
		/// <response code="400">If request parameter is invalid</response>
		/// <response code="401">If user is not authorized (Only agents are allowed)</response>
		/// <response code="403">If agent tries to edit another restaurant's product</response>
		/// <response code="404">If the product with the given id is not found</response>
		/// <param name="id">Product identifier.</param>
		[Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
		[HttpGet("GetOneForEdit/{id}")]
        [Produces(typeof(Product))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetProductForEdit(int id)
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
				return NotFound(new ErrorResponse { Message = $"Product with the given id={id} was not found" });
			}

			if (entity.RestaurantID != user.RestaurantID)
			{
				return Forbid();
			}

			return Ok(entity);
        }


		// PUT: api/Data/EditOne/5
		/// <summary>
		/// Edits the product with given id.
		/// </summary>
		/// <remarks>
		/// NOTE: Only agent is allowed to edit a product.
		/// </remarks>
		/// <returns>Nothing</returns>
		/// <response code="400">If request parameters (or body) is invalid</response>
		/// <response code="401">If user is not authorized (Only agents are allowed)</response>
		/// <response code="403">If agent tries to edit other restaurant's product</response>
		/// <response code="404">If the product with the given id is not found</response>
		/// <response code="409">If an error occurs while editing the product</response>
		/// <param name="id">Product identifier.</param>
		/// <param name="product">Product model with new data.</param>
		[Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
		[HttpPut("EditOne/{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(typeof(ErrorResponse), 404)]
		[ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> PutProduct(int id, [FromBody] ProductEditBindingModel product)
        {
            if (id < 1 || !ModelState.IsValid || product == null || product.ID != id)
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
				return NotFound(new ErrorResponse { Message = $"Product with the given id={id} was not found" });
			}

			if (entity.RestaurantID != user.RestaurantID)
			{
				return Forbid();
			}

            entity.NameArm = product.NameArm;
            entity.NameEng = product.NameEng;
            entity.NameRus = product.NameRus;
            entity.DescriptionArm = product.DescriptionArm;
            entity.DescriptionEng = product.DescriptionEng;
            entity.DescriptionRus = product.DescriptionRus;
            entity.ImgLink = product.ImgLink;
            entity.Price = product.Price;


			try
			{
				await _repo.SaveAsync(entity);
				return NoContent();
			}
			catch (DbUpdateConcurrencyException)
			{
                if (!ProductExists(id))
				{
					return NotFound(new ErrorResponse { Message = $"Product with the given id={id} was not found" });
				}
				else
				{
					return StatusCode(409, new ErrorResponse { Message = "An error occurred while editing the product." });
				}
			}
        }

		// POST: api/Data/AddOne
		/// <summary>
		/// Adds a product.
		/// </summary>
		/// /// <remarks>
		/// NOTE: Only agent is allowed to add a product.
		/// </remarks>
		/// <returns>The added product.</returns>
		/// <response code="400">If request body is invalid</response>
		/// <response code="401">If user is not authorized (Only agents are allowed)</response>
		/// <response code="403"></response>
		/// <response code="409">If an error occurs while editing the product</response>
		/// <param name="product">The product which has to be added.</param>
		[Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
		[HttpPost("AddOne")]
        [Produces(typeof(Product))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
        [ProducesResponseType(403)]
		[ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> PostProduct([FromBody] ProductAddBindingModel product)
		{
			if (!ModelState.IsValid || product == null)
			{
				return BadRequest(ModelState);
			}

			ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			if (user == null || !user.IsActive)
			{
				return Unauthorized();
			}

            var entity = new Product
            {
                NameArm = product.NameArm,
                NameEng = product.NameEng,
                NameRus = product.NameRus,

                DescriptionArm = product.DescriptionArm,
                DescriptionEng = product.DescriptionEng,
                DescriptionRus = product.DescriptionRus,

				CategoryID = product.CategoryID,
                ImgLink =  product.ImgLink,
                Price = product.Price,

                RestaurantID = user.RestaurantID.Value
            };

            bool validateCategory = (await _restRepo.ValidateCategoryAsync(user.RestaurantID.Value, product.CategoryID)) == 1;

            if (!validateCategory)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter CategoryID" });
            }

			try
			{
				await _repo.AddAsync(entity);
				return Created(new Uri(Request.Host.ToString() + Request.PathBase.ToString(), UriKind.RelativeOrAbsolute), entity);
			}
			catch (DbUpdateConcurrencyException)
			{
				return StatusCode(409, new ErrorResponse { Message = "An error occurred while adding the product." });
			}
            catch(DbUpdateException)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter CategoryID" });
            }
        }

		// DELETE: api/Data/DeleteOne/5
		/// <summary>
		/// Deletes the product with the given identifiers.
		/// </summary>
		/// <remarks>
		/// NOTE: Only agent is allowed to delete a product.
		/// </remarks>
		/// <returns>Nothing</returns>
		/// <response code="400">If request parameter is invalid</response>
		/// <response code="401">If user is not authorized (Only agents are allowed)</response>
		/// <response code="403">If agent tries to delete other restaurant's product</response>
		/// <response code="404">If the product with the given id is not found</response>
		/// <response code="409">If an error occurs while deleting the product</response>
		/// <param name="id">Product identifier.</param>
		[Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
		[HttpDelete("DeleteOne/{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(typeof(ErrorResponse), 404)]
		[ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> DeleteProduct(int id)
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

			Product product = await _repo.GetOneAsync(id);
			if (product == null || !product.IsActive)
			{
				return NotFound(new ErrorResponse { Message = $"Product with the given id={id} was not found" });
			}

			if (product.RestaurantID != user.RestaurantID)
			{
				return Forbid();
			}

			product.IsActive = false;


			try
			{
                await _repo.SaveAsync(product);
				return NoContent();
			}
			catch (DbUpdateConcurrencyException)
			{
                if (!ProductExists(id))
				{
					return NotFound(new ErrorResponse { Message = $"Product with the given id={id} was not found" });
				}
				else
				{
					return StatusCode(409, new ErrorResponse { Message = "An error occurred while deleting the product." });
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

        private bool ProductExists(int id)
        {
            return _repo.Exists(p => p.ID == id && p.IsActive);
        }
    }
}
