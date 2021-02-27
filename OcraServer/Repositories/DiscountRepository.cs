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


#pragma warning disable 1591

namespace OcraServer.Repositories
{
    public class DiscountRepository : BaseRepository<Discount>, IRepository<Discount>
    {
        // Constructors
        public DiscountRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.Discounts;
        }

        // Get All
        public List<DiscountViewModel> GetViewAll(APIDataLanguage lang)
        {
            var discounts = GetIQueryable(lang, d => d.IsActive).ToList();
            return discounts;
        }

        public Task<List<DiscountViewModel>> GetViewAllAsync(APIDataLanguage lang)
        {
            var discounts = GetIQueryable(lang, d => d.IsActive).ToListAsync();
            return discounts;
        }

        // Get Count
        public override int GetCount() => Table.Count(d => d.IsActive);
        public override Task<int> GetCountAsync() => Table.CountAsync(d => d.IsActive);

        // Get Range
        public List<DiscountViewModel> GetViewRange(APIDataLanguage lang, int startIndex, int count)
        {
            var discounts = GetIQueryable(lang, d => d.IsActive).Skip(startIndex - 1).Take(count).ToList();
            return discounts;
        }

        public Task<List<DiscountViewModel>> GetViewRangeAsync(APIDataLanguage lang, int startIndex, int count)
        {
            var discounts = GetIQueryable(lang, d => d.IsActive).Skip(startIndex - 1).Take(count).ToListAsync();
            return discounts;
        }

        // Get By RestID
        public List<DiscountViewModel> GetViewByRest(APIDataLanguage lang,int restID)
        {
            var discounts = GetIQueryable(lang, d => d.IsActive && d.RestaurantID == restID).ToList();

            return discounts;
        }

        public Task<List<DiscountViewModel>> GetViewByRestAsync(APIDataLanguage lang, int restID)
        {
            var discounts = GetIQueryable(lang, d => d.IsActive && d.RestaurantID == restID).ToListAsync();
            return discounts;
        }

        // Get Count By Rest
        public int GetCountByRestID(int restID) => Table.Count(d => d.IsActive && d.RestaurantID == restID);
        public Task<int> GetCountByRestIDAsync(int restID) => Table.CountAsync(d => d.IsActive && d.RestaurantID == restID);

        // Get Range By RestID
        public List<DiscountViewModel> GetViewRangeByRest(APIDataLanguage lang, int restID, int startIndex, int count)
        {
            var discounts = GetIQueryable(lang, d => d.IsActive && d.RestaurantID == restID).Skip(startIndex - 1).Take(count).ToList();
            return discounts;
        }

        public Task<List<DiscountViewModel>> GetViewRangeByRestAsync(APIDataLanguage lang, int restID, int startIndex, int count)
        {
            var discounts = GetIQueryable(lang, d => d.IsActive && d.RestaurantID == restID).Skip(startIndex - 1).Take(count).ToListAsync();
            return discounts;
        }

        public DiscountViewModel GetViewOne(APIDataLanguage lang, int? id)
        {
            DiscountViewModel discount = GetIQueryable(lang, d => d.IsActive && d.ID == id).FirstOrDefault();
            return discount;
        }

        public Task<DiscountViewModel> GetViewOneAsync(APIDataLanguage lang,int? id)
        {
            Task<DiscountViewModel> discount = GetIQueryable(lang, d => d.IsActive && d.ID == id).FirstOrDefaultAsync();
            return discount;
        }

        // Delete By Id
        public int Delete(int id)
        {
            Context.Entry(new Discount()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new Discount()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        // Private helpers
        private IQueryable<DiscountViewModel> GetIQueryable(APIDataLanguage lang, Expression<Func<Discount, bool>> predicate)
        {
            switch (lang)
            {
                case APIDataLanguage.EN:
					return (Table as IQueryable<Discount>).Where(predicate).OrderByDescending(d => d.ID).Select(discount => new DiscountViewModel
					{
						ID = discount.ID,
						Deadline = discount.Deadline,
                        Description = discount.DescriptionEng ?? discount.DescriptionArm,
						DiscountSize = discount.DiscountSize,
						ImgLink = discount.ImgLink,
                        Name = discount.NameEng ?? discount.NameArm,
						NewPrice = discount.NewPrice,
						RestaurantID = discount.RestaurantID,
						RestoLogoLink = discount.Restaurant.LogoLink,
                        RestoName = discount.Restaurant.NameEng
					});
                case APIDataLanguage.RU:
					return (Table as IQueryable<Discount>).Where(predicate).OrderByDescending(d => d.ID).Select(discount => new DiscountViewModel
					{
						ID = discount.ID,
						Deadline = discount.Deadline,
						Description = discount.DescriptionRus ?? discount.DescriptionArm,
						DiscountSize = discount.DiscountSize,
						ImgLink = discount.ImgLink,
						Name = discount.NameRus ?? discount.NameArm,
						NewPrice = discount.NewPrice,
						RestaurantID = discount.RestaurantID,
						RestoLogoLink = discount.Restaurant.LogoLink,
                        RestoName = discount.Restaurant.NameRus ?? discount.Restaurant.NameEng
					});
                case APIDataLanguage.AM:
                default:
					return (Table as IQueryable<Discount>).Where(predicate).OrderByDescending(d => d.ID).Select(discount => new DiscountViewModel
					{
						ID = discount.ID,
						Deadline = discount.Deadline,
						Description = discount.DescriptionArm,
						DiscountSize = discount.DiscountSize,
						ImgLink = discount.ImgLink,
						Name = discount.NameArm,
						NewPrice = discount.NewPrice,
						RestaurantID = discount.RestaurantID,
						RestoLogoLink = discount.Restaurant.LogoLink,
						RestoName = discount.Restaurant.NameArm ?? discount.Restaurant.NameEng
					});
            }
        }
    }
}