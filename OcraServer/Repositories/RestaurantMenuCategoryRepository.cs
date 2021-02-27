﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Enums;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Models.ViewModels;

namespace OcraServer.Repositories
{
    public class RestaurantMenuCategoryRepository : BaseRepository<RestaurantMenuCategory>, IRepository<RestaurantMenuCategory>
    {
		// Constructors
		public RestaurantMenuCategoryRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.RestaurantsCategories;
		}

		// Get by Restaurant ID
		public List<RestaurantMenuCategoryViewModel> GetByRestID(APIDataLanguage lang, int restID)
		{
			switch (lang)
			{
				case APIDataLanguage.EN:
					return Table.Where(rp => rp.RestaurantID == restID).Select(rp => new RestaurantMenuCategoryViewModel
					{
						ID = rp.ID,
						Name = rp.NameEng,
						RestaurantID = rp.RestaurantID
					}).ToList();
				case APIDataLanguage.RU:
					return Table.Where(rp => rp.RestaurantID == restID).Select(rp => new RestaurantMenuCategoryViewModel
					{
						ID = rp.ID,
						Name = rp.NameRus,
						RestaurantID = rp.RestaurantID
					}).ToList();
				case APIDataLanguage.AM:
				default:
					return Table.Where(rp => rp.RestaurantID == restID).Select(rp => new RestaurantMenuCategoryViewModel
					{
						ID = rp.ID,
						Name = rp.NameArm,
						RestaurantID = rp.RestaurantID
					}).ToList();
			}
		}

		public Task<List<RestaurantMenuCategoryViewModel>> GetByRestIDAsync(APIDataLanguage lang, int restID)
        {
            switch (lang)
            {
                case APIDataLanguage.EN:
                    return Table.Where(rp => rp.RestaurantID == restID).Select(rp => new RestaurantMenuCategoryViewModel
                    {
                        ID = rp.ID,
                        Name = rp.NameEng,
                        RestaurantID = rp.RestaurantID
                    }) .ToListAsync();
                case APIDataLanguage.RU:
					return Table.Where(rp => rp.RestaurantID == restID).Select(rp => new RestaurantMenuCategoryViewModel
					{
						ID = rp.ID,
						Name = rp.NameRus,
						RestaurantID = rp.RestaurantID
					}).ToListAsync();
                case APIDataLanguage.AM:
                    default:
					return Table.Where(rp => rp.RestaurantID == restID).Select(rp => new RestaurantMenuCategoryViewModel
					{
						ID = rp.ID,
						Name = rp.NameArm,
						RestaurantID = rp.RestaurantID
					}).ToListAsync();
            }
        }


		// Delete By Id    
		public int Delete(int id)
		{
			Context.Entry(new RestaurantMenuCategory()
			{
				ID = id
			}).State = EntityState.Deleted;
			return SaveChanges();
		}

		public Task<int> DeleteAsync(int id)
		{
			Context.Entry(new RestaurantMenuCategory()
			{
				ID = id
			}).State = EntityState.Deleted;
			return SaveChangesAsync();
		}
    }
}
