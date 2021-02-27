using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Enums;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Models.ViewModels;

namespace OcraServer.Repositories
{
    public class ReservationRepository : BaseRepository<Reservation>, IRepository<Reservation>
    {
        // Constructors
        public ReservationRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.Reservations;
        }

        // Get By UserID
        public List<ReservationViewModel> GetByUser(APIDataLanguage lang, string userID)
        {
            return GetIQueryable(lang, userID).ToList();
        }

        public Task<List<ReservationViewModel>> GetByUserAsync(APIDataLanguage lang, string userID)
        {
            return GetIQueryable(lang, userID).ToListAsync();
        }


        // Count By user
        public int CountByUser(string userID)
        {
            return Table.Count(r => r.IsActive && r.UserID == userID);
        }

        public Task<int> CountByUserAsync(string userID)
        {
            return Table.CountAsync(r => r.IsActive && r.UserID == userID);
        }

        // Get range by User
        public List<ReservationViewModel> GetRangeByUser(APIDataLanguage lang, string userID, int startIndex, int count)
        {
            return GetIQueryable(lang, userID).Skip(startIndex - 1).Take(count).ToList();
        }

        public Task<List<ReservationViewModel>> GetRangeByUserAsync(APIDataLanguage lang, string userID, int startIndex, int count)
        {
            return GetIQueryable(lang, userID).Skip(startIndex - 1).Take(count).ToListAsync();
        }

        // Get one by id
        public new Reservation GetOne(int? id)
        {
            return Table.Include(r => r.Restaurant).Include(r => r.User).FirstOrDefault(r => r.ID == id);
        }
        public new async Task<Reservation> GetOneAsync(int? id)
        {
            return await Table.Include(r => r.Restaurant).Include(r => r.User).FirstOrDefaultAsync(r => r.ID == id);
        }

        // Get one by id
        public Reservation GetOneWithAgent(int? id)
        {
            return Table.Include(r => r.Restaurant).ThenInclude(res => res.Agent).FirstOrDefault(r => r.ID == id);
        }
        public async Task<Reservation> GetOneWithAgentAsync(int? id)
        {
            return await Table.Include(r => r.Restaurant).ThenInclude(res => res.Agent).FirstOrDefaultAsync(r => r.ID == id);
        }


        // Get View By ID For Agent
        public RestaurantReservationDetailViewModel GetViewOneForAgent(int restID, int id)
        {
            return GetIQueryableWithDetailsForAgent(restID, id).ToList().FirstOrDefault();
        }

        public Task<List<RestaurantReservationDetailViewModel>> GetViewOneForAgentAsync(int restID, int id)
        {
            return GetIQueryableWithDetailsForAgent(restID, id).ToListAsync();
        }

        // Get By ID For User
        public ReservationDetailViewModel GetViewOne(APIDataLanguage lang, string userID, int id)
        {
            return GetIQueryableWithDetails(lang, userID, id).ToList().FirstOrDefault();
        }

        public Task<List<ReservationDetailViewModel>> GetViewOneAsync(APIDataLanguage lang, string userID, int id)
        {
            return GetIQueryableWithDetails(lang, userID, id).ToListAsync();
        }

        // Get By RestID
        public List<RestaurantReservationViewModel> GetByRest(int restID, ReservationStatus status)
        {
            if (status != ReservationStatus.Rejected)
            {
                return GetIQueryableForAgent(r => r.RestaurantID == restID && r.Status == status).ToList();
            }
            else
            {
                return GetIQueryableForAgent(r => r.RestaurantID == restID && r.Status == ReservationStatus.Rejected || r.Status == ReservationStatus.CancelledByAgentAfterAcceptance).ToList();
            }
        }

        public Task<List<RestaurantReservationViewModel>> GetByRestAsync(int restID, ReservationStatus status)
        {
            if (status != ReservationStatus.Rejected)
            {
                return GetIQueryableForAgent(r => r.RestaurantID == restID && r.Status == status).ToListAsync();
            }
            else
            {
                return GetIQueryableForAgent(r => r.RestaurantID == restID && r.Status == ReservationStatus.Rejected || r.Status == ReservationStatus.CancelledByAgentAfterAcceptance).ToListAsync();
            }

        }


        // Count By Rest
        public int CountByRest(int restID, ReservationStatus status)
        {
            if (status != ReservationStatus.Rejected)
            {
                return Table.Count(r => r.RestaurantID == restID && r.Status == status);
            }
            else
            {
                return Table.Count(r => r.RestaurantID == restID && r.Status == ReservationStatus.Rejected || r.Status == ReservationStatus.CancelledByAgentAfterAcceptance);
            }
        }

        public Task<int> CountByRestAsync(int restID, ReservationStatus status)
        {
            if (status != ReservationStatus.Rejected)
            {
                return Table.CountAsync(r => r.RestaurantID == restID && r.Status == status);
            }
            else
            {
                return Table.CountAsync(r => r.RestaurantID == restID && r.Status == ReservationStatus.Rejected || r.Status == ReservationStatus.CancelledByAgentAfterAcceptance);
            }
        }

        // Range By Rest
        public List<RestaurantReservationViewModel> GetRangeByRestAllCategories(int restID, int startIndex, int count)
        {
            return GetIQueryableForAgent(r => r.RestaurantID == restID && r.Status != ReservationStatus.CancelledByUser).Skip(startIndex - 1).Take(count).ToList();
        }

        public Task<List<RestaurantReservationViewModel>> GetRangeByRestAllCategoriesAsync(int restID, int startIndex, int count)
        {
            return GetIQueryableForAgent(r => r.RestaurantID == restID && r.Status != ReservationStatus.CancelledByUser).Skip(startIndex - 1).Take(count).ToListAsync();
        }

        public List<RestaurantReservationViewModel> GetRangeByRest(int restID, ReservationStatus status, int startIndex, int count)
        {
            if (status != ReservationStatus.Rejected)
            {
                return GetIQueryableForAgent(r => r.RestaurantID == restID && r.Status == status).Skip(startIndex - 1).Take(count).ToList();
            }
            else
            {
                return GetIQueryableForAgent(r => r.RestaurantID == restID && r.Status == ReservationStatus.Rejected || r.Status == ReservationStatus.CancelledByAgentAfterAcceptance).Skip(startIndex - 1).Take(count).ToList();
            }
        }

        public Task<List<RestaurantReservationViewModel>> GetRangeByRestAsync(int restID, ReservationStatus status, int startIndex, int count)
        {
            if (status != ReservationStatus.Rejected)
            {
                return GetIQueryableForAgent(r => r.RestaurantID == restID && r.Status == status).Skip(startIndex - 1).Take(count).ToListAsync();
            }
            else
            {
                return GetIQueryableForAgent(r => r.RestaurantID == restID && r.Status == ReservationStatus.Rejected || r.Status == ReservationStatus.CancelledByAgentAfterAcceptance).Skip(startIndex - 1).Take(count).ToListAsync();
            }
        }

        // Delete By Id
        public int Delete(int id)
        {
            Context.Entry(new Reservation()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new Reservation()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        // Private helpers
        private IQueryable<ReservationViewModel> GetIQueryable(APIDataLanguage lang, string userID)
        {
            switch (lang)
            {
                case APIDataLanguage.EN:
                    return (Table
                            .Join(Context.Restaurants, rv => rv.RestaurantID, r => r.ID, (reservation, restaurant) => new ReservationViewModel
                            {
                                ID = reservation.ID,
                                IsActive = reservation.IsActive,
                                ReservationDateTime = reservation.ReservationDateTime,
                                RestaurantID = reservation.RestaurantID,
                                RestoName = restaurant.NameEng,
                                RestoImgLink = restaurant.MainImgLink,
                                Status = reservation.Status,
                                SumPrice = reservation.SumPrice,
                                TableNumber = reservation.TableNumber,
                                UserID = reservation.UserID
                            })
                            .OrderBy(rvm => rvm.ReservationDateTime)
                            .Where(rvm => rvm.IsActive && rvm.UserID == userID));
                case APIDataLanguage.RU:
                    return (Table
                            .Join(Context.Restaurants, rv => rv.RestaurantID, r => r.ID, (reservation, restaurant) => new ReservationViewModel
                            {
                                ID = reservation.ID,
                                IsActive = reservation.IsActive,
                                ReservationDateTime = reservation.ReservationDateTime,
                                RestaurantID = reservation.RestaurantID,
                                RestoName = restaurant.NameRus,
                                RestoImgLink = restaurant.MainImgLink,
                                Status = reservation.Status,
                                SumPrice = reservation.SumPrice,
                                TableNumber = reservation.TableNumber,
                                UserID = reservation.UserID
                            })
                            .OrderBy(rvm => rvm.ReservationDateTime)
                            .Where(rvm => rvm.IsActive && rvm.UserID == userID));
                case APIDataLanguage.AM:
                default:
                    return (Table
                            .Join(Context.Restaurants, rv => rv.RestaurantID, r => r.ID, (reservation, restaurant) => new ReservationViewModel
                            {
                                ID = reservation.ID,
                                IsActive = reservation.IsActive,
                                ReservationDateTime = reservation.ReservationDateTime,
                                RestaurantID = reservation.RestaurantID,
                                RestoName = restaurant.NameArm,
                                RestoImgLink = restaurant.MainImgLink,
                                Status = reservation.Status,
                                SumPrice = reservation.SumPrice,
                                TableNumber = reservation.TableNumber,
                                UserID = reservation.UserID
                            })
                            .OrderBy(rvm => rvm.ReservationDateTime)
                            .Where(rvm => rvm.IsActive && rvm.UserID == userID));
            }
        }


        private IQueryable<RestaurantReservationViewModel> GetIQueryableForAgent(Expression<Func<Reservation, bool>> predicate)
        {
            return Table
                    .AsQueryable()
                    .Include(r => r.User)
                    .Where(predicate)
                    .OrderBy(r => r.Created)
                    .Select(r => new RestaurantReservationViewModel
                    {
                        ID = r.ID,
                        Status = r.Status,
                        ReservationDateTime = r.ReservationDateTime,
                        SumPrice = r.SumPrice,
                        TableNumber = r.TableNumber,
                        Note = r.Note,
                        PeopleCount = r.PeopleCount,
                        UserFirstName = r.User.FirstName,
                        UserLastName = r.User.LastName,
                        UserPhoneNumber = r.User.PhoneNumber ?? r.User.UserName
                    });
        }


        private IQueryable<ReservationDetailViewModel> GetIQueryableWithDetails(APIDataLanguage lang, string userID, int id)
        {
            switch (lang)
            {
                case APIDataLanguage.EN:

                    return (Table
                            .Join(Context.Restaurants, rv => rv.RestaurantID, r => r.ID, (reservation, restaurant) => new { reservation, restaurant })
                            .GroupJoin(Context.ReservationProducts, rv => rv.reservation.ID, rp => rp.ReservationID, (r, rp) => new ReservationDetailViewModel
                            {
                                ID = r.reservation.ID,
                                IsActive = r.reservation.IsActive,
                                ReservationDateTime = r.reservation.ReservationDateTime,
                                RestaurantID = r.reservation.RestaurantID,
                                Note = r.reservation.Note,
                                PeopleCount = r.reservation.PeopleCount,
                                RestoName = r.restaurant.NameEng,
                                RestoAddress = r.restaurant.AddressEng,
                                RestoImgLink = r.restaurant.MainImgLink,
                                Rest = r.restaurant,  //TODO fix 
                                Status = r.reservation.Status,
                                SumPrice = r.reservation.SumPrice,
                                TableNumber = r.reservation.TableNumber,
                                UserID = r.reservation.UserID,
                                OrderedProducts = rp.Select(op => new ReservationProductViewModel
                                {
                                    Count = op.Count,
                                    Price = op.Price,
                                    ReservationID = op.ReservationID,
                                    Product = new ProductInReservationViewModel()
                                    {
                                        Description = op.Product.DescriptionEng,
                                        ID = op.Product.ID,
                                        ImgLink = op.Product.ImgLink,
                                        Name = op.Product.NameEng
                                    }
                                })
                            }).Where(rvm => rvm.ID == id && rvm.IsActive && rvm.UserID == userID));
                case APIDataLanguage.RU:
                    return (Table
                            .Join(Context.Restaurants, rv => rv.RestaurantID, r => r.ID, (reservation, restaurant) => new { reservation, restaurant })
                            .GroupJoin(Context.ReservationProducts, rv => rv.reservation.ID, rp => rp.ReservationID, (r, rp) => new ReservationDetailViewModel
                            {
                                ID = r.reservation.ID,
                                IsActive = r.reservation.IsActive,
                                ReservationDateTime = r.reservation.ReservationDateTime,
                                RestaurantID = r.reservation.RestaurantID,
                                Note = r.reservation.Note,
                                PeopleCount = r.reservation.PeopleCount,
                                RestoName = r.restaurant.NameRus,
                                RestoAddress = r.restaurant.AddressRus,
                                RestoImgLink = r.restaurant.MainImgLink,
                                Rest = r.restaurant,  //TODO fix 
                                Status = r.reservation.Status,
                                SumPrice = r.reservation.SumPrice,
                                TableNumber = r.reservation.TableNumber,
                                UserID = r.reservation.UserID,
                                OrderedProducts = rp.Select(op => new ReservationProductViewModel
                                {
                                    Count = op.Count,
                                    Price = op.Price,
                                    ReservationID = op.ReservationID,
                                    Product = new ProductInReservationViewModel()
                                    {
                                        Description = op.Product.DescriptionRus,
                                        ID = op.Product.ID,
                                        ImgLink = op.Product.ImgLink,
                                        Name = op.Product.NameRus
                                    }
                                })
                            }).Where(rvm => rvm.ID == id && rvm.IsActive && rvm.UserID == userID));
                case APIDataLanguage.AM:
                default:
                    return (Table
                            .Join(Context.Restaurants, rv => rv.RestaurantID, r => r.ID, (reservation, restaurant) => new { reservation, restaurant })
                            .GroupJoin(Context.ReservationProducts, rv => rv.reservation.ID, rp => rp.ReservationID, (r, rp) => new ReservationDetailViewModel
                            {
                                ID = r.reservation.ID,
                                IsActive = r.reservation.IsActive,
                                ReservationDateTime = r.reservation.ReservationDateTime,
                                RestaurantID = r.reservation.RestaurantID,
                                Note = r.reservation.Note,
                                PeopleCount = r.reservation.PeopleCount,
                                RestoName = r.restaurant.NameArm,
                                RestoAddress = r.restaurant.AddressArm,
                                RestoImgLink = r.restaurant.MainImgLink,
                                Rest = r.restaurant,  //TODO fix 
                                Status = r.reservation.Status,
                                SumPrice = r.reservation.SumPrice,
                                TableNumber = r.reservation.TableNumber,
                                UserID = r.reservation.UserID,
                                OrderedProducts = rp.Select(op => new ReservationProductViewModel
                                {
                                    Count = op.Count,
                                    Price = op.Price,
                                    ReservationID = op.ReservationID,
                                    Product = new ProductInReservationViewModel()
                                    {
                                        Description = op.Product.DescriptionArm,
                                        ID = op.Product.ID,
                                        ImgLink = op.Product.ImgLink,
                                        Name = op.Product.NameArm
                                    }
                                })
                            }).Where(rvm => rvm.ID == id && rvm.IsActive && rvm.UserID == userID));
            }
        }

        private IQueryable<RestaurantReservationDetailViewModel> GetIQueryableWithDetailsForAgent(int restID, int id)
        {
            return (Table
                    .GroupJoin(Context.ReservationProducts, rv => rv.ID, rp => rp.ReservationID, (r, rp) => new RestaurantReservationDetailViewModel
                    {
                        ID = r.ID,
                        ReservationDateTime = r.ReservationDateTime,
                        RestaurantID = r.RestaurantID,
                        Note = r.Note,
                        PeopleCount = r.PeopleCount,
                        Status = r.Status,
                        SumPrice = r.SumPrice,
                        TableNumber = r.TableNumber,
                        //OrdererdProds = rp.Select(op => op)
                        OrderedProducts = rp.Select(op => new ReservationProductViewModel
                        {
                            Count = op.Count,
                            Price = op.Price,
                            ReservationID = op.ReservationID,
                            Product = new ProductInReservationViewModel()
                            {
                                Description = op.Product.DescriptionArm,
                                ID = op.Product.ID,
                                ImgLink = op.Product.ImgLink,
                                Name = op.Product.NameArm
                            }
                        })
                    })
                    .Where(rvm => rvm.ID == id && rvm.RestaurantID == restID));

        }
    }
}
