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
    public class EventsController : Controller
    {
        private readonly EventRepository _repo;
        private readonly RestaurantRepository _restRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(EventRepository repo, RestaurantRepository restRepo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _restRepo = restRepo;
            _userManager = userManager;
        }

		// GET: api/Data/Events/GetAll
		/// <summary>
		/// Gets all events.
		/// </summary>
		/// <returns>All events.</returns>
		[HttpGet("GetAll")]
        [Produces(typeof(IEnumerable<EventViewModel>))]
        public async Task<IActionResult> GetAllEvents(APIDataLanguage lang)
        {
            var events = await _repo.GetViewAllAsync(lang);
            return Ok(events);
        }

		// GET: api/Data/Events/GetCount
		/// <summary>
		/// Gets count of all events.
		/// </summary>
		/// <returns>Count of all events.</returns>
		[HttpGet("GetCount")]
		[Produces(typeof(CountViewModel))]
        public async Task<IActionResult> GetEventsCount()
        {
            int eventsCount = await _repo.GetCountAsync();
            return Ok(new CountViewModel { Count = eventsCount });
        }

		// GET: api/Data/Events/GetRange?startPosition=1&count=10
		/// <summary>
		/// Gets events range.
		/// </summary>
		/// <returns>Events range</returns>
		/// <response code="400">If request parameters are invalid</response>
		/// <param name="startPosition">The index of the first event in range(1 based). (Default value = 1).</param>
		/// <param name="count">Desired events count. (Default value = 10)</param>
		[HttpGet("GetRange")]
		[Produces(typeof(List<EventViewModel>))]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetEvents(APIDataLanguage lang, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
			if (startPosition < 1)
			{
				return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(startPosition)}'" });
			}
			if (count < 1)
			{
				return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(count)}'" });
			}

			var events = await _repo.GetViewRangeAsync(lang, startPosition, count);
			return Ok(events);
        }

		// GET: api/Data/Events/GetAllByRest/4
		/// <summary>
		/// Gets all events of the given restaurant.
		/// </summary>
		/// <returns>All events of the given restaurant.</returns>
		/// <response code="400">If request parameter is invalid</response>
		/// <param name="restaurantID">Restaurant identifier.</param>
		[HttpGet("GetAllByRest/{restaurantID}")]
        [Produces(typeof(List<EventViewModel>))]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetAllRestEvents(APIDataLanguage lang, int restaurantID)
        {
			if (restaurantID < 1)
			{
				return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(restaurantID)}'" });
			}

			var events = await _repo.GetViewByRestAsync(lang, restaurantID);
			return Ok(events);
        }

		// GET: api/Data/Events/GetCountByRest/24
		/// <summary>
		/// Gets count of the given restaurant's events.
		/// </summary>
		/// <returns>Count of the given restaurant's events.</returns>
		/// <response code="400">If request parameter is invalid</response>
		/// <param name="restaurantID">Restaurant identifier.</param>
		[HttpGet("GetCountByRest/{restaurantID}")]
		[Produces(typeof(CountViewModel))]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetRestEventsCount(int restaurantID)
        {
			if (restaurantID < 1)
			{
				return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(restaurantID)}'" });
			}

			int eventsCount = await _repo.GetCountByRestIDAsync(restaurantID);
			return Ok(new CountViewModel { Count = eventsCount });
        }

		// GET: api/Data/Events/GetRangeByRest/24?startPosition=5&count=45
		/// <summary>
		/// Gets events range of the given restaurant.
		/// </summary>
		/// <returns>Events range of the given restaurant.</returns>
		/// <response code="400">If request parameters are invalid</response>
		/// <param name="restaurantID">Restaurant identifier.</param>
		/// <param name="startPosition">The index of the first event in range(1 based). (Default value = 1).</param>
		/// <param name="count">Desired event count. (Default value = 10).</param>
		[HttpGet("GetRangeByRest/{restaurantID}")]
        [Produces(typeof(List<EventViewModel>))]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetRestEvents(APIDataLanguage lang, int restaurantID = 0, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
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

			var events = await _repo.GetViewRangeByRestAsync(lang, restaurantID, startPosition, count);
			return Ok(events);
        }

		// GET: api/Data/Events/GetOne/4
		/// <summary>
		/// Gets certain event.
		/// </summary>
		/// <returns>Certain event with the given identifier.</returns>
		/// <response code="400">If request parameter is invalid</response>
		/// <response code="404">If the event with the given id is not found</response>
		/// <param name="id">event identifier.</param>
		[HttpGet("GetOne/{id}")]
        [Produces(typeof(EventViewModel))]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
		[ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetEvent(APIDataLanguage lang, int id)
        {
			if (id < 1)
			{
				return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(id)}'" });
			}

            EventViewModel @event = await _repo.GetViewOneAsync(lang, id);
			if (@event == null)
			{
				return NotFound(new ErrorResponse { Message = $"Event with the given id={id} was not found" });
			}

			return Ok(@event);
        }


		// GET: api/Data/Events/GetOneForEdit/4
		/// <summary>
		/// Gets certain event with all information.
		/// </summary>
		/// <returns>Certain event with the given identifier.</returns>
		/// <response code="400">If request parameter is invalid</response>
		/// <response code="401">If user is not authorized (Only agents are allowed)</response>
		/// <response code="403">If agent tries to edit another restaurant's event</response>
		/// <response code="404">If the event with the given id is not found</response>
		/// <param name="id">Event identifier.</param>
		[Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
		[HttpGet("GetOneForEdit/{id}")]
		[Produces(typeof(Event))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(typeof(ErrorResponse), 404)]
		public async Task<IActionResult> GetEventForEdit(int id)
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
				return NotFound(new ErrorResponse { Message = $"Event with the given id={id} was not found" });
			}

			if (entity.RestaurantID != user.RestaurantID)
			{
				return Forbid();
			}

			return Ok(entity);
		}


		// PUT: api/Data/Events/EditOne/5
		/// <summary>
		/// Edits the event with given id.
		/// </summary>
		/// <remarks>
		/// NOTE: Only agent is allowed to edit an event.
		/// </remarks>
		/// <returns>Nothing</returns>
		/// <response code="400">If request parameters (or body) is invalid</response>
		/// <response code="401">If user is not authorized (Only agents are allowed)</response>
		/// <response code="403">If agent tries to edit other restaurant's event</response>
		/// <response code="404">If the event with the given id is not found</response>
		/// <response code="409">If an error occurs while editing the event</response>
		/// <param name="id">Event identifier.</param>
		/// <param name="event">Event model with new data.</param>
		[Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
		[HttpPut("EditOne/{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(typeof(ErrorResponse), 404)]
		[ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> PutEvent(int id, [FromBody] EventEditBindingModel @event)
        {
			if (id < 1 || !ModelState.IsValid || @event == null || @event.ID != id)
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
				return NotFound(new ErrorResponse { Message = $"Event with the given id={id} was not found" });
			}

			if (entity.RestaurantID != user.RestaurantID)
			{
				return Forbid();
			}


            entity.NameEng = @event.NameEng;
            entity.NameRus = @event.NameRus;
            entity.NameArm = @event.NameArm;
		
            entity.DescriptionEng = @event.DescriptionEng;
            entity.DescriptionArm = @event.DescriptionArm;
            entity.DescriptionRus = @event.DescriptionRus;
		
            entity.EventDateTime = @event.EventDateTime;
            entity.AdditionalPrice = @event.AdditionalPrice;
            entity.ImgLink = @event.ImgLink;


			try
			{
				await _repo.SaveAsync(entity);
				return NoContent();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!EventExists(id))
				{
					return NotFound(new ErrorResponse { Message = $"Event with the given id={id} was not found" });
				}
				else
				{
					return StatusCode(409, new ErrorResponse { Message = "An error occurred while editing the event." });
				}
			}
        }

		// POST: api/Data/Events/AddOne
		/// <summary>
		/// Adds a event.
		/// </summary>
		/// <remarks>
		/// NOTE: Only agent is allowed to add an event.
		/// </remarks>
		/// <returns>The added event.</returns>
		/// <response code="400">If request body is invalid</response>
		/// <response code="401">If user is not authorized (Only agents are allowed)</response>
		/// <response code="409">If an error occurs while editing the event</response>
		/// <param name="event">The event which has to be added.</param>
		[Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
		[HttpPost("AddOne")]
		[Produces(typeof(Event))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> PostEvent([FromBody] EventAddBindingModel @event)
        {
			if (!ModelState.IsValid || @event == null)
			{
				return BadRequest(ModelState);
			}

			ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			if (user == null || !user.IsActive)
			{
				return Unauthorized();
			}

			var entity = new Event
            {
                NameEng = @event.NameEng,
                NameRus = @event.NameRus,
                NameArm = @event.NameArm,

                DescriptionEng = @event.DescriptionEng,
                DescriptionRus = @event.DescriptionRus,
                DescriptionArm = @event.DescriptionArm,

                EventDateTime = @event.EventDateTime,
                AdditionalPrice = @event.AdditionalPrice,
                ImgLink = @event.ImgLink,

                RestaurantID = user.RestaurantID.Value
            };

			try
			{
				await _repo.AddAsync(entity);
				return Created(new Uri(Request.Host.ToString() + Request.PathBase.ToString(), UriKind.RelativeOrAbsolute), entity);
			}
			catch (DbUpdateConcurrencyException)
			{
				return StatusCode(409, new ErrorResponse { Message = "An error occurred while adding the event." });
			}
        }

		// DELETE: api/Data/Events/DeleteOne/5
		/// <summary>
		/// Deletes the event with the given identifiers.
		/// </summary>
		/// <remarks>
		/// NOTE: Only agent is allowed to delete an event.
		/// </remarks>
		/// <returns>Nothing</returns>
		/// <response code="400">If request parameter is invalid</response>
		/// <response code="401">If user is not authorized (Only agents are allowed)</response>
		/// <response code="403">If agent tries to delete other restaurant's event</response>
		/// <response code="404">If the event with the given id is not found</response>
		/// <response code="409">If an error occurs while deleting the event</response>
		/// <param name="id">Event identifier.</param>
		[Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
		[HttpDelete("DeleteOne/{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(typeof(ErrorResponse), 404)]
		[ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> DeleteEvent(int id)
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

			Event @event = await _repo.GetOneAsync(id);
			if (@event == null || !@event.IsActive)
			{
				return NotFound(new ErrorResponse { Message = $"Event with the given id={id} was not found" });
			}

			if (@event.RestaurantID != user.RestaurantID)
			{
				return Forbid();
			}

			@event.IsActive = false;

			try
			{
				await _repo.SaveAsync(@event);
				return NoContent();
			}
			catch (DbUpdateConcurrencyException)
			{
                if (!EventExists(id))
				{
					return NotFound(new ErrorResponse { Message = $"Event with the given id={id} was not found" });
				}
				else
				{
					return StatusCode(409, new ErrorResponse { Message = "An error occurred while deleting the event." });
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

        private bool EventExists(int id)
        {
            return _repo.Exists(e => e.ID == id && e.IsActive);
        }
    }
}
