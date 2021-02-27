using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OcraServer.Extra;
using OcraServer.Models.BindingModels;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Models.ViewModels;
using OcraServer.Repositories;

namespace OcraServer.Controllers
{
    [Route("api/Data/[controller]")]
    public class FeedbacksController : Controller
    {
        private readonly FeedbackRepository _repo;
        private readonly RestaurantRepository _restRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbacksController(FeedbackRepository repo, RestaurantRepository restRepo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _restRepo = restRepo;
            _userManager = userManager;
        }

        // GET: api/Data/Feedbacks/GetAllByRest/3
        /// <summary>
        /// Gets all feedbacks of the given restaurant.
        /// </summary>
        /// <returns>The all feedbacks by rest.</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <param name="restaurantID">Restaurant identifier.</param>
        [HttpGet("GetAllByRest/{restaurantID}")]
        [Produces(typeof(List<FeedbackViewModel>))]
        public async Task<IActionResult> GetAllFeedbacksByRest(int restaurantID)
        {
            var feedbacks = await _repo.GetViewByRestAsync(restaurantID);
            return Ok(feedbacks);
        }

        // GET: api/Data/Feedbacks/GetCountByRest/24
        /// <summary>
        /// Gets count of the given restaurant's discounts.
        /// </summary>
        /// <returns>Count of the given restaurant's discounts.</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <param name="restaurantID">Restaurant identifier.</param>
        [HttpGet("GetCountByRest/{restaurantID}")]
        [Produces(typeof(CountViewModel))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetFeedbacksByRestCount(int restaurantID)
        {
            if (restaurantID < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(restaurantID)}'" });
            }

            int feedbacksCount = await _repo.GetCountByRestIDAsync(restaurantID);
            return Ok(new CountViewModel { Count = feedbacksCount });
        }

        // GET: api/Data/Feedbacks/GetRangeByRest/24?startPosition=5&count=45
        /// <summary>
        /// Gets discounts range of the given restaurant.
        /// </summary>
        /// <returns>Discounts range of the given restaurant.</returns>
        /// <response code="400">If request parameters are invalid</response>
        /// <param name="restaurantID">Restaurant identifier.</param>
        /// <param name="startPosition">The index of the first feedback in range(1 based). (Default value = 1).</param>
        /// <param name="count">Desired discounts count. (Default value = 10).</param>
        [HttpGet("GetRangeByRest/{restaurantID}")]
        [Produces(typeof(List<FeedbackViewModel>))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetFeedbacksByRest(int restaurantID = 0, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
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

            string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (startPosition == 1 && userID != null)
            {
                var userFeedbacks = await _repo.GetViewByRestNUserAsync(restaurantID, userID);
                var feedbacks = await _repo.GetRangeByRestAsync(restaurantID, startPosition, count);

                if (userFeedbacks.Any())
                {
                    userFeedbacks.AddRange(feedbacks);
                    return Ok(userFeedbacks);
                }
                else
                {
                    return Ok(feedbacks);
                }
            }
            else
            {
                var feedbacks = await _repo.GetRangeByRestAsync(restaurantID, startPosition, count);
                return Ok(feedbacks);
            }
        }

        // PUT: api/Data/Feedbacks/EditOne/5
        /// <summary>
        /// Edits the feedback with given id.
        /// </summary>
        /// <remarks>
        /// NOTE: Only client is allowed to edit a feedback.
        /// </remarks>
        /// <returns>Nothing</returns>
        /// <response code="400">If request parameters (or body) is invalid</response>
        /// <response code="401">If user is not authorized (Only clients are allowed)</response>
        /// <response code="403">If cleint tries to edit other client's feedback</response>
        /// <response code="404">If the feedback with the given id is not found</response>
        /// <response code="409">If an error occurs while editing the feedback</response>
        /// <param name="id">Feedback identifier.</param>
        /// <param name="feedback">Feedback model with new data.</param>
        [Authorize(Roles = Constants.ApplicationRoles.CLIENT_ROLE)]
        [HttpPut("EditOne/{id}")]
        [Produces(typeof(FeedbackViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> PutFeedback(int id, [FromBody] FeedbackEditBindingModel feedback)
        {
            if (id < 1 || !ModelState.IsValid || feedback == null || feedback.ID != id)
            {
                return BadRequest(ModelState);
            }

            var entity = await _repo.GetOneAsync(id);
            if (feedback == null)
            {
                return NotFound(new ErrorResponse { Message = $"Feedback with the given id={id} was not found" });
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            if (entity.UserID != user.Id)
            {
                return Forbid();
            }

            entity.Text = feedback.Text;
            entity.Mark = feedback.Mark;
            entity.Created = DateTime.UtcNow;
            entity.HasBeenEdited = true;

            try
            {
                await _repo.SaveAsync(entity);
                return Ok(new FeedbackViewModel
                {
                    ID = entity.ID,
                    Created = entity.Created,
                    HasBeenEdited = entity.HasBeenEdited,
                    Text = entity.Text,
                    Mark = entity.Mark,
                    UserFullName = user.FirstName + " " + user.LastName,
                    UserID = entity.UserID,
                    UserImgUrl = user.ImgUrl
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
                {
                    return NotFound(new ErrorResponse { Message = $"Feedback with the given id={id} was not found" });
                }
                else
                {
                    return StatusCode(409, new ErrorResponse { Message = "An error occurred while editing the feedback." });
                }
            }
        }

        // POST: api/Data/Feedbacks/AddOne
        /// <summary>
        /// Adds new feedback.
        /// </summary>
        /// <returns>The created feedback.</returns>
        /// <response code="400">If request parameters (or body) is invalid</response>
        /// <response code="401">If user is not authorized (Only clients are allowed)</response>
        /// <response code="409">If an error occurs while editing the feedback</response>
        /// <param name="feedback">Feedback.</param>
        [Authorize(Roles = Constants.ApplicationRoles.CLIENT_ROLE)]
        [HttpPost("AddOne")]
        [Produces(typeof(FeedbackResultViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> PostFeedback([FromBody] FeedbackAddBindingModel feedback)
        {
            if (!ModelState.IsValid || feedback == null)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }


            Restaurant rest = await _restRepo.GetOneAsync(feedback.RestaurantID);
            if (rest == null || !rest.IsActive)
            {
                return Forbid();
            }

            var entity = new Feedback
            {
                UserID = user.Id,
                RestaurantID = feedback.RestaurantID,
                Created = DateTime.UtcNow,
                Text = feedback.Text,
            };


            try
            {
                await _repo.AddAsync(entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409, new ErrorResponse { Message = "An error occurred while adding the feedback." });
            }


            var response = new FeedbackResultViewModel
            {
                ID = entity.ID,
                Created = entity.Created,
                HasBeenEdited = entity.HasBeenEdited,
                Mark = entity.Mark,
                Text = entity.Text,
                UserID = user.Id
            };

            return Created(new Uri(Request.Host.ToString() + Request.PathBase.ToString(), UriKind.RelativeOrAbsolute), response);
        }

        // DELETE: api/Data/DeleteOne/5
        /// <summary>
        /// Deletes the feedback with the given identifiers.
        /// </summary>
        /// <remarks>
        /// NOTE: Only client is allowed to delete a feedback.
        /// </remarks>
        /// <returns>Nothing</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <response code="401">If user is not authorized (Only clients are allowed)</response>
        /// <response code="403">If client tries to delete other cleint's feedback</response>
        /// <response code="404">If the feedback with the given id is not found</response>
        /// <response code="409">If an error occurs while deleting the feedback</response>
        /// <param name="id">Feedback identifier.</param>
        [Authorize(Roles = Constants.ApplicationRoles.CLIENT_ROLE)]
        [HttpDelete("DeleteOne/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> DeleteFeedback(int id)
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

            Feedback feedback = await _repo.GetOneAsync(id);
            if (feedback == null)
            {
                return NotFound(new ErrorResponse { Message = $"Feedback with the given id={id} was not found" });
            }

            if (feedback.UserID != user.Id)
            {
                return Forbid();
            }

            try
            {
                await _repo.DeleteAsync(feedback);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
                {
                    return NotFound(new ErrorResponse { Message = $"Feedback with the given id={id} was not found" });
                }
                else
                {
                    return StatusCode(409, new ErrorResponse { Message = "An error occurred while deleting the feedback." });
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

        private bool FeedbackExists(int id)
        {
            return _repo.Exists(f => f.ID == id);
        }
    }
}
