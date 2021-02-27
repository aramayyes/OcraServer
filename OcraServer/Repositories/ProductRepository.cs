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
    public class ProductRepository : BaseRepository<Product>, IRepository<Product>
    {
        // Constructors
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.Products;
        }

        // Get by rest ID
        public List<ProductViewModel> GetViewAll(APIDataLanguage lang, int categoryID)
        {
            var products = GetIQueryable(lang, p => p.IsActive && p.CategoryID == categoryID).ToList();
            return products;
        }
        public Task<List<ProductViewModel>> GetViewAllAsync(APIDataLanguage lang, int categoryID)
        {
            var products = GetIQueryable(lang, p => p.IsActive && p.CategoryID == categoryID).ToListAsync();
            return products;
        }

        // Get Count by restID
        public int GetCount(int categoryID) => Table.Count(p => p.IsActive && p.CategoryID == categoryID);
        public Task<int> GetCountAsync(int categoryID) => Table.CountAsync(p => p.IsActive && p.CategoryID == categoryID);


        // Get Range by restID
        public List<ProductViewModel> GetRange(APIDataLanguage lang, int categoryID, int startIndex, int count)
        {
            var products = GetIQueryable(lang, p => p.IsActive && p.CategoryID == categoryID).Skip(startIndex - 1).Take(count).ToList();
            return products;
        }

        public Task<List<ProductViewModel>> GetRangeAsync(APIDataLanguage lang, int categoryID, int startIndex, int count)
        {
            var products = GetIQueryable(lang, p => p.IsActive && p.CategoryID == categoryID).Skip(startIndex - 1).Take(count).ToListAsync();
            return products;
        }

        // Get count by category restaurant
        public Task<int> GetCountByCategoryAsync(int categoryID) => Table.CountAsync(p => p.IsActive && p.CategoryID == categoryID);
        public int GetCountByCategory(int categoryID) => Table.Count(p => p.IsActive && p.CategoryID == categoryID);

        // Get range by category and restaurant 
        public List<ProductViewModel> GetRangeByCategory(APIDataLanguage lang, int restaurantID, int categoryID, int startIndex, int count)
        {
            var products = GetIQueryable(lang, p => p.IsActive && p.CategoryID == categoryID && p.RestaurantID == restaurantID).Skip(startIndex - 1).Take(count).ToList();
            return products;
        }

        public Task<List<ProductViewModel>> GetRangeByCategoryAsync(APIDataLanguage lang, int restaurantID, int categoryID, int startIndex, int count)
        {
            var products = GetIQueryable(lang, p => p.IsActive && p.CategoryID == categoryID && p.RestaurantID == restaurantID).Skip(startIndex - 1).Take(count).ToListAsync();
            return products;
        }

        // Get One
        public ProductViewModel GetViewOne(APIDataLanguage lang, int id)
        {
            var product = GetIQueryable(lang, p => p.IsActive && p.ID == id).FirstOrDefault();
            return product;
        }

        public Task<ProductViewModel> GetViewOneAsync(APIDataLanguage lang, int id)
        {
            var product = GetIQueryable(lang, p => p.IsActive && p.ID == id).FirstOrDefaultAsync();
            return product;
        }

        // Get by IDs
        public List<ProductForReservation> GetViewByIDs(int[] IDs, int restaurantID)
        {
            var products = GetIQueryableForReserve(p => p.IsActive && p.RestaurantID == restaurantID && IDs.Contains(p.ID)).ToList();
            return products;
        }
        public Task<List<ProductForReservation>> GetViewByIDsAsync(int[] IDs, int restaurantID)
        {
            var products = GetIQueryableForReserve(p => p.IsActive && p.RestaurantID == restaurantID && IDs.Contains(p.ID)).ToListAsync();
			return products;
        }


        //// Search
        //public List<Product> Search(string searchTerm)
        //{
        //    return Table.Where(
        //        p => p.IsActive &&
        //        (p.NameEng.Contains(searchTerm)
        //          || p.NameRus.Contains(searchTerm)
        //          || p.NameArm.Contains(searchTerm))
        //        ).OrderByDescending(p => p.ID).ToList();
        //}

        //public Task<List<Product>> SearchAsync(string searchTerm)
        //{
        //    return Table.Where(
        //        p => p.IsActive &&
        //        (p.NameEng.Contains(searchTerm)
        //          || p.NameRus.Contains(searchTerm)
        //          || p.NameArm.Contains(searchTerm))
        //        ).OrderByDescending(p => p.ID).ToListAsync();
        //}

        //public IQueryable<NameForSuggestionModel> GetNames()
        //{
        //    return Table.Where(p => p.IsActive).Select(p => new NameForSuggestionModel { Name_Arm = p.NameArm, Name_Rus = p.Name_Rus, Name_Eng = p.Name_Eng });
        //}


        // Delete By Id
        public int Delete(int id)
        {
            Context.Entry(new Product()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new Product()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        // Private helpers
        private IQueryable<ProductViewModel> GetIQueryable(APIDataLanguage lang, Expression<Func<Product, bool>> predicate)
        {
            switch (lang)
            {
                case APIDataLanguage.EN:
                    return (Table as IQueryable<Product>).Where(predicate).OrderByDescending(p => p.ID).Select(product => new ProductViewModel
                    {
                        ID = product.ID,
                        RestaurantID = product.RestaurantID,
                        Description = product.DescriptionEng,
                        CategoryID = product.CategoryID,
                        ImgLink = product.ImgLink,
                        Name = product.NameEng,
                        Price = product.Price
                    });
                case APIDataLanguage.RU:
                    return (Table as IQueryable<Product>).Where(predicate).OrderByDescending(p => p.ID).Select(product => new ProductViewModel
                    {
                        ID = product.ID,
                        RestaurantID = product.RestaurantID,
                        Description = product.DescriptionRus,
                        CategoryID = product.CategoryID,
                        ImgLink = product.ImgLink,
                        Name = product.NameRus,
                        Price = product.Price
                    });
                case APIDataLanguage.AM:
                default:
                    return (Table as IQueryable<Product>).Where(predicate).OrderByDescending(p => p.ID).Select(product => new ProductViewModel
                    {
                        ID = product.ID,
                        RestaurantID = product.RestaurantID,
                        Description = product.DescriptionArm,
                        CategoryID = product.CategoryID,
                        ImgLink = product.ImgLink,
                        Name = product.NameArm,
                        Price = product.Price
                    });
            }
        }

        private IQueryable<ProductForReservation> GetIQueryableForReserve(Expression<Func<Product, bool>> predicate)
        {
            return (Table as IQueryable<Product>).Where(predicate).OrderByDescending(p => p.ID).Select(product => new ProductForReservation
            {
                ID = product.ID,
                Price = product.Price
            });

        }
    }
}

