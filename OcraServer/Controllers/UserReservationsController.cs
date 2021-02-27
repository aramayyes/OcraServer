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
    [Authorize(Roles = Constants.ApplicationRoles.CLIENT_ROLE)]
    [Route("api/Data/[controller]")]
    [Route("api/Data/{lang}/[controller]")]
    public class UserReservationsController : Controller
    {
        private readonly ReservationRepository _repo;
        private readonly RestaurantRepository _restRepo;
        private readonly ProductRepository _productRepo;
        private readonly ReservedTableRepository _reservTableRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationSender _notificationSender;


        private const int RESERVATION_DEADLINE_IN_DAYS = 7;

        public UserReservationsController(ReservationRepository repo,
                                          RestaurantRepository restRepo,
                                          ProductRepository productRepo,
                                          ReservedTableRepository reservTableRepo,
                                          UserManager<ApplicationUser> userManager,
                                         INotificationSender notificationSender)
        {
            _repo = repo;
            _restRepo = restRepo;
            _productRepo = productRepo;
            _reservTableRepo = reservTableRepo;
            _userManager = userManager;
            _notificationSender = notificationSender;
        }


        // GET: api/Data/GetAll
        /// <summary>
        /// Gets the user all reservations.
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
        /// </remarks>
        /// <returns>The user all reservations.</returns>
        [HttpGet("GetAll")]
        [Produces(typeof(List<ReservationViewModel>))]
        public async Task<IActionResult> GetUserAllReservations(APIDataLanguage lang)
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userID == null)
            {
                return Unauthorized();
            }

            var reservations = await _repo.GetByUserAsync(lang, userID);
            return Ok(reservations);
        }

        // GET: api/Data/GetCount
        /// <summary>
        /// Gets the user reservations count.
        /// </summary>
        /// <returns>The user reservations count.</returns>
        [HttpGet("GetCount")]
        [Produces(typeof(CountViewModel))]
        public async Task<IActionResult> GetUserReservationsCount()
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userID == null)
            {
                return Unauthorized();
            }

            int reservationsCount = await _repo.CountByUserAsync(userID);
            return Ok(new CountViewModel { Count = reservationsCount });
        }

        // GET: api/Data/GetRange?startPosition=2&count=12
        /// <summary>
        /// Gets the user reservations range.
        /// </summary>
        /// <returns>The user reservations range.</returns>
        /// <param name="startPosition">The index of the first reservation in range(1 based). (Default value = 1).</param>
        /// <param name="count">Desired reservations count. (Default value = 10)</param>
        [HttpGet("GetRange")]
        [Produces(typeof(List<ReservationViewModel>))]
        public async Task<IActionResult> GetUserReservations(APIDataLanguage lang, [FromQuery] int startPosition = 1, [FromQuery] int count = 10)
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userID == null)
            {
                return Unauthorized();
            }

            IEnumerable<ReservationViewModel> reservations = await _repo.GetRangeByUserAsync(lang, userID, startPosition, count);
            return Ok(reservations);
        }

        // GET: api/Data/GetOne/4
        /// <summary>
        /// Gets the user reservation.
        /// </summary>
        /// <returns>The user reservation.</returns>
        /// <param name="id">Reservation identifier.</param>
        /// <response code="404">If the reservation with the given id is not found</response>
        [HttpGet("GetOne/{id}")]
        [Produces(typeof(ReservationDetailViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserReservation(APIDataLanguage lang, int id)
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userID == null)
            {
                return Unauthorized();
            }

            ReservationDetailViewModel reservation = (await _repo.GetViewOneAsync(lang, userID, id)).FirstOrDefault();
            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        // POST: api/Data/UserReservations/Reserve
        /// <summary>
        /// Add a reservation.
        /// </summary>
        /// <remarks>
        /// User will receive a notification when reservation status changes.
        ///
        ///     Notification json:
        ///     {
        ///        "reservationID": 0,
        ///        "restaurantNameArm": "string",
        ///        "restaurantNameArm": "string",
        ///        "restaurantNameArm": "string",
        ///        "reservationDateTime": "2017-09-09T07:42:51.720Z",
        ///        "status": 0
        ///     }
        ///
        /// </remarks>
        /// <returns>Created(added) reservation.</returns>
        /// <param name="reservation">Reservation</param>
        [HttpPost("Reserve")]
        [Produces(typeof(ReservationResultViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostUserReservation([FromBody] ReservationAddBindingModel reservation)
        {
            if (!ModelState.IsValid || reservation == null)
            {
                return BadRequest(ModelState);
            }

            var userID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Restaurant res = await _restRepo.GetOneAsync(reservation.RestaurantID);
            if (res == null || !res.IsActive)
            {
                return BadRequest(new ErrorResponse { Message = "Invalid parameter 'restaurantID'" });
            }

            reservation.ReservationDateTime = reservation.ReservationDateTime.ToUniversalTime();

            if (!(ValidateReservationTime(res, reservation.ReservationDateTime)))
            {
                return BadRequest(new ErrorResponse { Message = "Invalid parameter 'reservationDateTime'" });
            }

            if (reservation.TableNumber != null && (reservation.TableNumber > res.TableCount || reservation.TableNumber <= 0))
            {
                return BadRequest(new ErrorResponse { Message = "Invalid parameter 'tableNumber'" });
            }

            List<ReservationProduct> orderedProducts = new List<ReservationProduct>();
            int? sumPrice = null;
            if (reservation.OrderedProducts != null)
            {
                var productsInDictionary = reservation.OrderedProducts.Select(p => new { p.ProductID, p.Count }).ToDictionary(p => p.ProductID, p => p.Count);

                int[] IDs = productsInDictionary.Keys.ToArray();


                if (IDs != null && IDs.Any())
                {
                    sumPrice = 0;
                    var productsToAdd = await _productRepo.GetViewByIDsAsync(IDs, res.ID);
                    if (productsToAdd != null && productsToAdd.Any())
                    {
                        sumPrice = 0;
                        foreach (var product in productsToAdd)
                        {
                            int count = productsInDictionary[product.ID];
                            orderedProducts.Add(new ReservationProduct
                            {
                                ProductID = product.ID,
                                Count = count,
                                Price = product.Price

                            });
                            sumPrice += (count * product.Price);
                        }
                    }

                }
            }


            var entity = new Reservation
            {
                Created = DateTime.UtcNow,
                Note = reservation.Note,
                PeopleCount = reservation.PeopleCount,
                ReservationDateTime = reservation.ReservationDateTime,
                RestaurantID = res.ID,
                Status = ReservationStatus.WaitingForAcceptance,
                SumPrice = sumPrice,
                TableNumber = reservation.TableNumber,
                UserID = userID,
                OrderedProducts = orderedProducts
            };
            try
            {
                await _repo.AddAsync(entity);
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }

            await SendNotificationAsync(entity, res.Agent);

            var reservationToReturn = new ReservationResultViewModel
            {
                ID = entity.ID,
                ReservationDateTime = entity.ReservationDateTime,
                Status = entity.Status,
                SumPrice = entity.SumPrice,
                PeopleCount = entity.PeopleCount,
                Note = entity.Note,
                TableNumber = entity.TableNumber
            };

            return Created(new Uri(Request.Host.ToString() + Request.PathBase.ToString(), UriKind.RelativeOrAbsolute), reservationToReturn);
        }


        // PUT: api/Data/UserReservations/CancelOne/4
        /// <summary>
        /// Cancells the user reservation.
        /// </summary>
        /// <returns>Nothing</returns>
        /// <param name="id">Reservation identifier.</param>
        /// <response code="400">If request parameters (or body) is invalid</response>
        /// <response code="403">If user tries to cancel other user's reservation</response>
        /// <response code="404">If the reservation with the given id is not found</response>
        /// <response code="409">If an error occurs while cancelling the reservation</response>
        [HttpPut("CancelOne/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> PutCancelUserReservation(int id)
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

            var entity = await _repo.GetOneWithAgentAsync(id);
            if (entity == null || !entity.IsActive)
            {
                return NotFound();
            }

            if (entity.UserID != user.Id || !entity.IsActive)
            {
                return Forbid();
            }

            switch (entity.Status)
            {
                case ReservationStatus.WaitingForAcceptance:
                    entity.Status = ReservationStatus.CancelledByUser;
                    break;
                case ReservationStatus.Accepted:
                    entity.Status = ReservationStatus.CancelledByUserAfterAcceptance;
                    user.Points += Constants.UserPoints.CANCEL_RESERVATION;
                    break;
                default:
                    return Forbid();
            }

            try
            {
                await _repo.SaveAsync(entity);
                if (entity.Status == ReservationStatus.CancelledByUserAfterAcceptance)
                {
                    await _userManager.UpdateAsync(user);
                    await SendNotificationAsync(entity, entity.Restaurant.Agent);
                }

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound(new ErrorResponse { Message = $"Discount with the given id={id} was not found" });
                }
                else
                {
                    return StatusCode(409, new ErrorResponse { Message = "An error occurred while editing the discount." });
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
        }


        // PUT: api/Data/UserReservations/EditOne/3
        /// <summary>
        /// Edits the user reservation.
        /// </summary>
        /// <returns>Nothing</returns>
        /// <param name="id">Reservation identifier.</param>
        /// <param name="reservation">Reservation with new data</param>
        /// <response code="400">If request parameters (or body) is invalid</response>
        /// <response code="403">If user tries to edit other user's reservation</response>
        /// <response code="404">If the reservation with the given id is not found</response>
        /// <response code="409">If an error occurs while cancelling the reservation</response>
        [HttpPut("EditOne/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> PutUserReservation(int id, [FromBody] ReservationEditBindingModel reservation)
        {
            if (reservation == null || id < 1 || id != reservation.ID)
            {
                return BadRequest(ModelState);
            }

            var userID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var entity = _repo.GetOneWithAgent(id);
            if (entity == null || !entity.IsActive)
            {
                return NotFound();
            }

            if (entity.Status != ReservationStatus.WaitingForAcceptance)
            {
                return BadRequest();
            }

            if (entity.UserID != userID || !entity.IsActive)
            {
                return Forbid();
            }

            Restaurant res = entity.Restaurant;
            if (res == null || !res.IsActive)
            {
                return BadRequest(new ErrorResponse { Message = "Invalid parameter 'restaurantID'" });
            }

            reservation.ReservationDateTime = reservation.ReservationDateTime.ToUniversalTime();

            if (!(ValidateReservationTime(res, reservation.ReservationDateTime)))
            {
                return BadRequest(new ErrorResponse { Message = "Invalid parameter 'reservationDateTime'" });
            }

            entity.ReservationDateTime = reservation.ReservationDateTime;
            entity.PeopleCount = reservation.PeopleCount;
            entity.Note = reservation.Note;


            var productsInDictionary = reservation.OrderedProducts.Select(p => new { p.ProductID, p.Count }).ToDictionary(p => p.ProductID, p => p.Count);

            int[] IDs = productsInDictionary.Keys.ToArray();

            List<ReservationProduct> orderedProducts = new List<ReservationProduct>();
            int? sumPrice = null;
            if (IDs != null && IDs.Any())
            {
                sumPrice = 0;
                var productsToAdd = await _productRepo.GetViewByIDsAsync(IDs, res.ID);
                foreach (var product in productsToAdd)
                {
                    int count = productsInDictionary[product.ID];
                    orderedProducts.Add(new ReservationProduct
                    {
                        ProductID = product.ID,
                        Count = count,
                        Price = product.Price

                    });
                    sumPrice += (count * product.Price);
                }
            }

            entity.OrderedProducts = orderedProducts;

            try
            {
                await _repo.SaveAsync(entity);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound(new ErrorResponse { Message = $"Discount with the given id={id} was not found" });
                }
                else
                {
                    return StatusCode(409, new ErrorResponse { Message = "An error occurred while editing the discount." });
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }

            await SendNotificationAsync(entity, res.Agent);
            return NoContent();
        }


        // PUT: api/Data/UserReservations/DeleteOne/3
        /// <summary>
        /// Deletes the reservation.
        /// </summary>
        /// <returns>Nothing</returns>
        /// <param name="id">Reservation Identifier</param>
        /// <response code="400">If request parameters (or body) is invalid</response>
        /// <response code="404">If the reservation with the given id is not found</response>
        /// <response code="409">If an error occurs while deleting the reservation</response>
        [HttpPut("DeleteOne/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            if (id < 1)
            {
                return BadRequest(ModelState);
            }

            var userID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var entity = _repo.GetOne(id);
            if (entity == null || !entity.IsActive)
            {
                return NotFound();
            }

            if (entity.UserID != userID)
            {
                return Forbid();
            }

            entity.IsActive = false;

            try
            {
                await _repo.SaveAsync(entity);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound(new ErrorResponse { Message = $"Discount with the given id={id} was not found" });
                }
                else
                {
                    return StatusCode(409, new ErrorResponse { Message = "An error occurred while editing the discount." });
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
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

        private async Task SendNotificationAsync(Reservation reservation, ApplicationUser agent)
        {
            if (agent == null || !agent.IsActive)
            {
                return;
            }

            List<string> ids = GetUserIDs(agent);
            if (ids == null)
            {
                return;
            }

            else
            {
                var now = DateTime.UtcNow;
                var reservationDateTime = reservation.ReservationDateTime;
                var timeToLive = Convert.ToInt64((reservationDateTime - now).TotalSeconds);

                var data = new UserAddedReservationNotificationModel
                {
                    ReservationID = reservation.ID,
                    ReservationDateTime = reservation.ReservationDateTime,
                    SumPrice = reservation.SumPrice,
                    TableNumber = reservation.TableNumber,
                    PeopleCount = reservation.PeopleCount,
                    Note = reservation.Note,
                    Status = reservation.Status
                };

                await _notificationSender.SendNotificationAsync(ids, data, timeToLive);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
                _restRepo.Dispose();
                _productRepo.Dispose();
                _reservTableRepo.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReservationExists(int id)
        {
            return _repo.Exists(r => r.ID == id);
        }

        private bool ValidateReservationTime(Restaurant res, DateTime reservationDateTime)
        {
            var now = DateTime.UtcNow;
            if ((reservationDateTime - now).TotalSeconds < 3600)
            {
                return false;
            }

            var dayOfWeek = (int)reservationDateTime.DayOfWeek;
            if ((now >= reservationDateTime) || (now.AddDays(RESERVATION_DEADLINE_IN_DAYS) < reservationDateTime))
            {
                return false;
            }

            if (res.IsOpen24)
            {
                return true;
            }

            bool isWeekend = (dayOfWeek == 6 || dayOfWeek == 7);
            var openingTime = isWeekend ? res.AdditionalOpeningTime : res.OpeningTime;
            var closingTime = isWeekend ? res.AdditionalClosingTime : res.ClosingTime;


            return (reservationDateTime.TimeOfDay > openingTime && reservationDateTime.TimeOfDay < closingTime);
        }
    }
}
