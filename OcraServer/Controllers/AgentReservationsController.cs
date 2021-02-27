using System;
using System.Collections.Generic;
using System.Linq;
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
using OcraServer.Services;
using OcraServer.Models.NotificationModels;

namespace OcraServer.Controllers
{
    [Authorize(Roles = Constants.ApplicationRoles.AGENT_ROLE)]
    [Route("api/Data/[controller]")]
    public class AgentReservationsController : Controller
    {
        private readonly RestaurantRepository _restRepo;
        private readonly ReservationRepository _reservRepo;
        private readonly ReservedTableRepository _reservTableRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationSender _notificationSender;

        public AgentReservationsController(RestaurantRepository restRepo, ReservationRepository reservRepo,
            ReservedTableRepository reservTableRepo, UserManager<ApplicationUser> userManager, INotificationSender notificationSender)
        {
            _restRepo = restRepo;
            _reservRepo = reservRepo;
            _reservTableRepo = reservTableRepo;
            _userManager = userManager;
            _notificationSender = notificationSender;
        }



        // GET: api/Agent/RestReservations
        /// <summary>
        /// Gets the restaurant all reservations.
        /// </summary>
        /// <remarks>
        /// Reservation status states:
        ///
        ///     Status
        ///     {
        ///        WaitingForAcceptance = 0,
        ///        CancelledByUser = 1,
        ///        Accepted = 2,
        ///        Rejected = 3,
        ///        CancelledByUserAfterAcceptance = 4,
        ///        CancelledByAgentAfterAcceptance = 5,
        ///        Done = 6
        ///     }
        ///
        /// Categories:
        ///
        ///     Status
        ///     {
        ///        WaitingForAcceptance = 0,
        ///        Accepted = 1,
        ///        Done = 2,
        ///        Rejected = 3,
        ///        CancelledByUser = 4,
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <returns>Restaurant all reservations.</returns>
        /// <param name="category">Category.</param>
        [HttpGet("GetAll")]
        [Produces(typeof(List<RestaurantReservationViewModel>))]
        public async Task<IActionResult> GetRestAllReservations([FromQuery] ReservationStatusForAgent category)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            ReservationStatus status = ManageCategory(category);

            var reservations = await _reservRepo.GetByRestAsync(user.RestaurantID.Value, status);
            return Ok(reservations);
        }

        // GET: api/Data/AgentReservations/GetCount?category=2
        /// <summary>
        /// Gets the restaurant reservations count (by category).
        /// </summary>
        /// <returns>Reservations count.</returns>
        /// <param name="category">Category.</param>
        [HttpGet("GetCount")]
        [Produces(typeof(CountViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetRestReservationsCount([FromQuery] ReservationStatusForAgent category)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            ReservationStatus status = ManageCategory(category);

            int reservationsCount = await _reservRepo.CountByRestAsync(user.RestaurantID.Value, status);
            return Ok(new CountViewModel { Count = reservationsCount });
        }

        // GET: api/Data/AgentReservations/Range
        /// <summary>
        /// Gets the restaurant reservations range.
        /// </summary>
        /// <remarks>
        /// Reservation status states:
        ///
        ///     Status
        ///     {
        ///        WaitingForAcceptance = 0,
        ///        CancelledByUser = 1,
        ///        Accepted = 2,
        ///        Rejected = 3,
        ///        CancelledByUserAfterAcceptance = 4,
        ///        CancelledByAgentAfterAcceptance = 5,
        ///        Done = 6
        ///     }
        ///
        /// Categories:
        ///
        ///     Status
        ///     {
        ///        WaitingForAcceptance = 0,
        ///        Accepted = 1,
        ///        Done = 2,
        ///        Rejected = 3,
        ///        CancelledByUser = 4,
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <returns>The restaurant reservations range.</returns>
        /// <response code="400">If request parameter (or body) is invalid</response>
        /// <param name="category">Category.</param>
        /// <param name="startPosition">Start position.</param>
        /// <param name="count">Count.</param>
        [HttpGet("GetRange")]
        [Produces(typeof(List<RestaurantReservationViewModel>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetRestReservations([FromQuery]ReservationStatusForAgent category, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
            if (startPosition < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(startPosition)}'" });
            }
            if (count < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(count)}'" });
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            ReservationStatus status = ManageCategory(category);
            var reservations = await _reservRepo.GetRangeByRestAsync(user.RestaurantID.Value, status, startPosition, count);

            return Ok(reservations);
        }


        // GET: api/Data/AgentReservations/AllCategRange
        /// <summary>
        /// Gets the restaurant reservations range .
        /// </summary>
        /// <remarks>
        /// Reservation status states:
        ///
        ///     Status
        ///     {
        ///        WaitingForAcceptance = 0,
        ///        CancelledByUser = 1,
        ///        Accepted = 2,
        ///        Rejected = 3,
        ///        CancelledByUserAfterAcceptance = 4,
        ///        CancelledByAgentAfterAcceptance = 5,
        ///        Done = 6
        ///     }
        /// </remarks>
        /// <returns>The restaurant reservations range. Category doesn't matter.</returns>
        /// <response code="400">If request parameter (or body) is invalid</response>
        /// <param name="startPosition">Start position.</param>
        /// <param name="count">Count.</param>
        [HttpGet("AllCategRange")]
        [Produces(typeof(List<RestaurantReservationViewModel>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetRestReservations([FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
            if (startPosition < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(startPosition)}'" });
            }
            if (count < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(count)}'" });
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            var reservations = await _reservRepo.GetRangeByRestAllCategoriesAsync(user.RestaurantID.Value, startPosition, count);
            return Ok(reservations);
        }

        // GET: api/Agent/RestReservationByID/4
        /// <summary>
        /// Gets certain reservation.
        /// </summary>
        /// <remarks>
        /// Agent will receive a notification when user makes a new reservation (request).
        ///
        ///     Notification json:
        ///     {
        ///        "reservationID": 0,
        ///        "sumPrice": 0,
        ///        "tableNumber": 0,
        ///        "peopleCount": 0,
        ///        "note": "string",
        ///        "reservationDateTime": "2017-09-09T07:42:51.720Z",
        ///        "status": 0
        ///     }
        ///
        /// Reservation status states:
        ///
        ///     Status
        ///     {
        ///        WaitingForAcceptance = 0,
        ///        CancelledByUser = 1,
        ///        Accepted = 2,
        ///        Rejected = 3,
        ///        CancelledByUserAfterAcceptance = 4,
        ///        CancelledByAgentAfterAcceptance = 5,
        ///        Done = 6
        ///     }
        ///
        /// </remarks>
        /// <returns>The certain reservation.</returns>
        /// <response code="400">If request parameter (or body) is invalid</response>
        /// <response code="403">If agent tries get other restaurant's reservation</response>
        /// <param name="id">Reservation identifier.</param>
        [HttpGet("GetOne/{id}")]
        [Produces(typeof(RestaurantReservationDetailViewModel))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRestReservation(int id)
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

            RestaurantReservationDetailViewModel reservation = ( await _reservRepo.GetViewOneForAgentAsync(user.RestaurantID.Value, id)).FirstOrDefault();
            if (reservation == null)
            {
                return Forbid();
            }

            //reservation.OrderedProducts = reservation.OrdererdProds.Select(op => new ReservationProductViewModel
            //{
            //    Count = op.Count,
            //    Price = op.Price,
            //    ReservationID = op.ReservationID,
            //    Product = new ProductInReservationViewModel()
            //    {
            //        Description = op.Product.DescriptionArm,
            //        ID = op.Product.ID,
            //        ImgLink = op.Product.ImgLink,
            //        Name = op.Product.NameArm
            //    }
            //});

            return Ok(reservation);
        }

        // GET: api/Data/AgentReservations/GetExternals
        /// <summary>
        /// Gets the restaurant external reservations.
        /// </summary>
        /// <returns>The external reservations.</returns>
        [HttpGet("GetExternals")]
        [Produces(typeof(List<ExternalReservation>))]
        public async Task<IActionResult> GetExternalReservations()
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            var resTables = await _reservTableRepo.GetByRestAsync(user.RestaurantID.Value);
            return Ok(resTables);
        }

        // POST: api/Data/AgentReservations/AddExternal
        /// <summary>
        /// Adds an external reservation.
        /// </summary>
        /// <returns>The added external reservation.</returns>
        /// <response code="400">If request parameter (or body) is invalid</response>
        /// <response code="409">If an error occurs while adding the reservation</response>
        /// <param name="externalReservation">External reservation.</param>
        [HttpPost("AddExternal")]
        [Produces(typeof(ExternalReservation))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> PostExternal([FromBody] ExternalReservationBindingModel externalReservation)
        {
            if (!ModelState.IsValid || externalReservation == null)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            ExternalReservation entity = new ExternalReservation
            {
                ReservationDateTime = externalReservation.ReservationDateTime,
                TableNumber = externalReservation.TableNumber,
                RestaurantID = user.RestaurantID.Value,
                PeopleCount = externalReservation.PeopleCount
            };

            try
            {
                await _reservTableRepo.AddAsync(entity);
                return Created(new Uri(Request.Host.ToString() + Request.PathBase.ToString(), UriKind.RelativeOrAbsolute), entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409, new ErrorResponse { Message = "An error occurred while adding the reservation." });
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
        }

        // DELETE: api/Data/AgentReservations/DeleteExternal/3
        /// <summary>
        /// Deletes the reservation.
        /// </summary>
        /// <returns>Nothing</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <response code="403">If agent tries to delete other restaurant's reservation</response>
        /// <response code="404">If the reservation with the given id is not found</response>
        /// <response code="409">If an error occurs while deleting the reservation</response>
        /// <param name="id">External reservation Identifier.</param>
        [HttpDelete("DeleteExternal/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> DeleteExternal(int id)
        {
            if (id < 1)
            {
                return BadRequest(new ErrorResponse { Message = $"Invalid parameter '{nameof(id)}'" });
            }

            var externalReservation = await _reservTableRepo.GetOneAsync(id);
            if (externalReservation == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            if (externalReservation.RestaurantID != user.RestaurantID)
            {
                return Forbid();
            }

            try
            {
                await _reservTableRepo.DeleteAsync(externalReservation);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExternalReservationExists(id))
                {
                    return NotFound(new ErrorResponse { Message = $"Reservation with the given id={id} was not found" });
                }
                else
                {
                    return StatusCode(409, new ErrorResponse { Message = "An error occurred while deleting the reservation." });
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }

        }

        // PUT: api/Data/AgentReservations/AcceptOne/97
        /// <summary>
        /// Accepts the reservation with the given id.
        /// </summary>
        /// <returns>Nothing</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <response code="403">If agent tries to accept other restaurant's reservation</response>
        /// <response code="404">If the reservation with the given id is not found</response>
        /// <response code="409">If an error occurs while accepting the reservation</response>
        /// <param name="id">Reservation identifier.</param>
        /// <param name="tableNumber">Table number (if the customer didn't choose one).</param>
        [HttpPut("AcceptOne/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> PutAcceptReservation(int id, [FromQuery] int? tableNumber = null)
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

            Reservation reservation = await _reservRepo.GetOneAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.RestaurantID != user.RestaurantID)
            {
                return Forbid();
            }

            switch (reservation.Status)
            {
                case ReservationStatus.WaitingForAcceptance:
                    reservation.Status = ReservationStatus.Accepted;
                    break;
                default:
                    return BadRequest();
            }

            // If client didn't choose "tableNumber"
            if (reservation.TableNumber == null && tableNumber == null)
            {

                return BadRequest(new ErrorResponse { Message = "'tableNumber' is required" });
            }
            else if (tableNumber != null)
            {
                reservation.TableNumber = tableNumber;
            }


            try
            {
                await _reservRepo.SaveAsync(reservation);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound(new ErrorResponse { Message = $"Reservation with the given id={id} was not found" });
                }
                else
                {
                    return StatusCode(409, new ErrorResponse { Message = "An error occurred while accepting the reservation." });
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }

            await SendNotificationAsync(reservation);
            return NoContent();
        }

        // PUT api/Data/AgentReservations/RejectOne/34
        /// <summary>
        /// Rejects the reservation with the given id.
        /// </summary>
        /// <returns>Nothing.</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <response code="403">If agent tries to reject other restaurant's reservation</response>
        /// <response code="404">If the reservation with the given id is not found</response>
        /// <response code="409">If an error occurs while rejecting the reservation</response>
        /// <param name="id">Reservation identifier.</param>
        [HttpPut("RejectOne/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> PutRejectReservation(int id)
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

            Reservation reservation = await _reservRepo.GetOneAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.RestaurantID != user.RestaurantID)
            {
                return Forbid();
            }

            switch (reservation.Status)
            {
                case ReservationStatus.WaitingForAcceptance:
                    reservation.Status = ReservationStatus.Rejected;
                    break;
                default:
                    return BadRequest();
            }

            try
            {
                await _reservRepo.SaveAsync(reservation);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound(new ErrorResponse { Message = $"Reservation with the given id={id} was not found" });
                }
                else
                {
                    return StatusCode(409, new ErrorResponse { Message = "An error occurred while rejecting the reservation." });
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }

            await SendNotificationAsync(reservation);
            return NoContent();
        }

        // PUT api/Data/AgentReservations/CancelOne/34
        /// <summary>
        /// Cancels the reservation with the given id.
        /// </summary>
        /// <returns>Nothing.</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <response code="403">If agent tries to reject other restaurant's reservation</response>
        /// <response code="404">If the reservation with the given id is not found</response>
        /// <response code="409">If an error occurs while rejecting the reservation</response>
        /// <param name="id">Reservation identifier.</param>
        [HttpPut("CancelOne/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> PutCancelReservation(int id)
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

            Reservation reservation = await _reservRepo.GetOneAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.RestaurantID != user.RestaurantID)
            {
                return Forbid();
            }

            switch (reservation.Status)
            {
                case ReservationStatus.Accepted:
                    reservation.Status = ReservationStatus.CancelledByAgentAfterAcceptance;
                    break;
                default:
                    return BadRequest();
            }

            try
            {
                await _reservRepo.SaveAsync(reservation);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409, new ErrorResponse { Message = "An error occurred while cancelling the reservation." });
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }

            await SendNotificationAsync(reservation);
            await _userManager.UpdateAsync(reservation.User);
            return NoContent();
        }


        // PUT: api/Data/AgentReservations/CompleteOne/97
        /// <summary>
        /// Completes the reservation with the given id.
        /// </summary>
        /// <returns>Nothing</returns>
        /// <response code="400">If request parameter is invalid</response>
        /// <response code="403">If agent tries to copmlete other restaurant's reservation</response>
        /// <response code="404">If the reservation with the given id is not found</response>
        /// <response code="409">If an error occurs while completing the reservation</response>
        /// <param name="id">Reservation identifier.</param>
        [HttpPut("CompleteOne/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> PutCompleteReservation(int id)
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

            Reservation reservation = await _reservRepo.GetOneAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.RestaurantID != user.RestaurantID)
            {
                return Forbid();
            }

            switch (reservation.Status)
            {
                case ReservationStatus.Accepted:
                    reservation.Status = ReservationStatus.Done;
                    break;
                default:
                    return BadRequest();
            }

            try
            {
                await _reservRepo.SaveAsync(reservation);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409, new ErrorResponse { Message = "An error occurred while completing the reservation." });

            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }


            await SendNotificationAsync(reservation);
            reservation.User.Points += Constants.UserPoints.RESERVATION;
            await _userManager.UpdateAsync(reservation.User);

            return NoContent();
        }


        // Private 
        private ReservationStatus ManageCategory(ReservationStatusForAgent category)
        {
            ReservationStatus status;
            switch (category)
            {
                case ReservationStatusForAgent.Accepted:
                    status = ReservationStatus.Accepted;
                    break;
                case ReservationStatusForAgent.Done:
                    status = ReservationStatus.Done;
                    break;
                case ReservationStatusForAgent.WaitingForAcceptance:
                    status = ReservationStatus.WaitingForAcceptance;
                    break;
                case ReservationStatusForAgent.Rejected:
                    status = ReservationStatus.Rejected;
                    break;
                case ReservationStatusForAgent.CancelledByUser:
                    status = ReservationStatus.CancelledByUserAfterAcceptance;
                    break;
                default:
                    status = ReservationStatus.Accepted;
                    break;
            }

            return status;
        }

        private List<String> GetUserIDs(ApplicationUser customer)
        {
            List<string> ids = new List<string>();
            if (customer.FirebaseNotificationTokenAndroid != null)
            {
                ids.Add(customer.FirebaseNotificationTokenAndroid);
            }
            if (customer.FirebaseNotificationTokeniOS != null)
            {
                ids.Add(customer.FirebaseNotificationTokeniOS);
            }
            if (customer.FirebaseNotificationTokenWeb != null)
            {
                ids.Add(customer.FirebaseNotificationTokenWeb);
            }

            return ids.Any() ? ids : null;
        }

        private async Task SendNotificationAsync(Reservation reservation)
        {
            ApplicationUser customer = reservation.User;
            if (customer == null || !customer.IsActive)
            {
                return;
            }

            List<string> ids = GetUserIDs(customer);
            if (ids == null)
            {
                return;
            }

            else
            {
                var now = DateTime.UtcNow;
                var reservationDateTime = reservation.ReservationDateTime;
                var timeToLive = Convert.ToInt64((reservationDateTime - now).TotalSeconds);

                var data = new ReservationStatusChangedNotificationModel
                {
                    ReservationID = reservation.ID,
                    RestaurantNameArm = reservation.Restaurant.NameArm,
                    RestaurantNameEng = reservation.Restaurant.NameEng,
                    RestaurantNameRus = reservation.Restaurant.NameRus,
                    ReservationDateTime = reservation.ReservationDateTime,
                    Status = reservation.Status
                };

                await _notificationSender.SendNotificationAsync(ids, data, timeToLive);
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _reservRepo.Dispose();
                _reservTableRepo.Dispose();
                _restRepo.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReservationExists(int id)
        {
            return _reservRepo.Exists(r => r.ID == id && r.Status == ReservationStatus.WaitingForAcceptance);
        }

        private bool ExternalReservationExists(int id)
        {
            return _reservTableRepo.Exists(r => r.ID == id);
        }
    }
}
