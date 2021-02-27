using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Models.EntityFrameworkModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OcraServer.Models.ViewModels;
using OcraServer.Enums;
using System.Linq.Expressions;

namespace OcraServer.Repositories
{
    public class UserFavoriteRestaurantRepository : BaseRepository<UserFavoriteRestaurant>, IRepository<UserFavoriteRestaurant>
    {
        // Constructors
        public UserFavoriteRestaurantRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.UsersFavoriteRestaurants;
        }


        // Get By UserID

        protected List<RestaurantViewModel> GetViewByUser(APIDataLanguage lang, string userID)
        {
            var restaurants = GetIQueryable(lang, ufr => ufr.UserID == userID).ToList();
            return restaurants;
        }

        public Task<List<RestaurantViewModel>> GetViewByUserAsync(APIDataLanguage lang, string userID)
        {
			var restaurants = GetIQueryable(lang, ufr => ufr.UserID == userID).ToListAsync();
			return restaurants;
        }

        // Get count

        public int CountByUser(string userID)
        {
            return Table.Where(fv => fv.UserID == userID).Count();
        }

        public Task<int> CountByUserAsync(string userID)
        {
            return Table.Where(fv => fv.UserID == userID).CountAsync();
        }

        // Get range by user
        public List<RestaurantViewModel> GetRangeByUser(APIDataLanguage lang,  string userID, int startIndex, int count)
        {
			var restaurants = GetIQueryable(lang, ufr => ufr.UserID == userID).Skip(startIndex - 1).Take(count).ToList();
			return restaurants;
        }

        public Task<List<RestaurantViewModel>> GetRangeByUserAsync(APIDataLanguage lang,  string userID, int startIndex, int count)
        {
			var restaurants = GetIQueryable(lang, ufr => ufr.UserID == userID).Skip(startIndex - 1).Take(count).ToListAsync();
			return restaurants;
        }

        // Get By RestID and User

        public int GetCountByRestAndUser(int restID, string userID)
        => Table.Count(fv => fv.RestaurantID == restID && fv.UserID == userID);

        public Task<int> GetCountByRestAndUserAsync(int restID, string userID)
        => Table.CountAsync(fv => fv.RestaurantID == restID && fv.UserID == userID);

        // Delete By Id
        public int Delete(string userID, int restaurantID)
        {
            Context.Entry(new UserFavoriteRestaurant()
            {
                UserID = userID,
                RestaurantID = restaurantID
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(string userID, int restaurantID)
        {
            Context.Entry(new UserFavoriteRestaurant()
            {
                UserID = userID,
                RestaurantID = restaurantID
            }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

		// Private helpers
        private IQueryable<RestaurantViewModel> GetIQueryable(APIDataLanguage lang, Expression<Func<UserFavoriteRestaurant, bool>> predicate)
		{
			switch (lang)
			{
				case APIDataLanguage.EN:
					return (Table as IQueryable<UserFavoriteRestaurant>).Where(predicate).OrderByDescending(r => r.AddedDate).Select(ufr => new RestaurantViewModel
					{
						ID = ufr.Restaurant.ID,
						Address = ufr.Restaurant.AddressEng,
						City = ufr.Restaurant.CityEng,
						Cost = ufr.Restaurant.Cost,
						LogoLink = ufr.Restaurant.LogoLink,
						MainImgLink = ufr.Restaurant.MainImgLink,
						Name = ufr.Restaurant.NameEng,
						RatedCount = ufr.Restaurant.RatedCount,
						Rating = ufr.Restaurant.Rating
					});
				case APIDataLanguage.RU:
					return (Table as IQueryable<UserFavoriteRestaurant>).Where(predicate).OrderByDescending(r => r.AddedDate).Select(ufr => new RestaurantViewModel
					{
						ID = ufr.Restaurant.ID,
						Address = ufr.Restaurant.AddressRus,
						City = ufr.Restaurant.CityRus,
						Cost = ufr.Restaurant.Cost,
						LogoLink = ufr.Restaurant.LogoLink,
						MainImgLink = ufr.Restaurant.MainImgLink,
						Name = ufr.Restaurant.NameRus,
						RatedCount = ufr.Restaurant.RatedCount,
						Rating = ufr.Restaurant.Rating
					});
				case APIDataLanguage.AM:
				default:
					return (Table as IQueryable<UserFavoriteRestaurant>).Where(predicate).OrderByDescending(r => r.AddedDate).Select(ufr => new RestaurantViewModel
					{
						ID = ufr.Restaurant.ID,
						Address = ufr.Restaurant.AddressArm,
						City = ufr.Restaurant.CityArm,
						Cost = ufr.Restaurant.Cost,
						LogoLink = ufr.Restaurant.LogoLink,
						MainImgLink = ufr.Restaurant.MainImgLink,
						Name = ufr.Restaurant.NameArm,
						RatedCount = ufr.Restaurant.RatedCount,
						Rating = ufr.Restaurant.Rating
					});
			}
		}
    }
}
