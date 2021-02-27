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
    public class EventRepository : BaseRepository<Event>, IRepository<Event>
    {
        // Constructors
        public EventRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.Events;
        }

		// Get All
		public List<EventViewModel> GetViewAll(APIDataLanguage lang)
		{
			var events = GetIQueryable(lang, e => e.IsActive).ToList();
			return events;
		}

		public Task<List<EventViewModel>> GetViewAllAsync(APIDataLanguage lang)
		{
			var events = GetIQueryable(lang, e => e.IsActive).ToListAsync();
			return events;
		}

		// Get Count
		public override int GetCount() => Table.Count(e => e.IsActive);
		public override Task<int> GetCountAsync() => Table.CountAsync(e => e.IsActive);

		// Get Range
        public List<EventViewModel> GetViewRange(APIDataLanguage lang, int startIndex, int count)
		{
			var events = GetIQueryable(lang, e => e.IsActive).Skip(startIndex - 1).Take(count).ToList();
			return events;
		}

		public Task<List<EventViewModel>> GetViewRangeAsync(APIDataLanguage lang, int startIndex, int count)
		{
			var events = GetIQueryable(lang, e => e.IsActive).Skip(startIndex - 1).Take(count).ToListAsync();
			return events;
		}

		// Get By RestID
		public List<EventViewModel> GetViewByRest(APIDataLanguage lang, int restID)
		{
			var events = GetIQueryable(lang, e => e.IsActive && e.RestaurantID == restID).ToList();
			return events;
		}

		public Task<List<EventViewModel>> GetViewByRestAsync(APIDataLanguage lang, int restID)
		{
			var events = GetIQueryable(lang, e => e.IsActive && e.RestaurantID == restID).ToListAsync();
			return events;
		}

		// Get Count By Rest
		public int GetCountByRestID(int restID) => Table.Count(e => e.IsActive && e.RestaurantID == restID);
		public Task<int> GetCountByRestIDAsync(int restID) => Table.CountAsync(e => e.IsActive && e.RestaurantID == restID);

		// Get Range By RestID
		public List<EventViewModel> GetViewRangeByRest(APIDataLanguage lang, int restID, int startIndex, int count)
		{
			var events = GetIQueryable(lang, e => e.IsActive && e.RestaurantID == restID).Skip(startIndex - 1).Take(count).ToList();
			return events;
		}

		public Task<List<EventViewModel>> GetViewRangeByRestAsync(APIDataLanguage lang, int restID, int startIndex, int count)
		{
			var events = GetIQueryable(lang, e => e.IsActive && e.RestaurantID == restID).Skip(startIndex - 1).Take(count).ToListAsync();
			return events;
		}

        // Get One
        public EventViewModel GetViewOne(APIDataLanguage lang, int? id)
		{
			EventViewModel @event = GetIQueryable(lang, e => e.IsActive && e.ID == id).FirstOrDefault();
			return @event;
		}

		public Task<EventViewModel> GetViewOneAsync(APIDataLanguage lang, int? id)
		{
			Task<EventViewModel> @event = GetIQueryable(lang, e => e.IsActive && e.ID == id).FirstOrDefaultAsync();
			return @event;
		}

 
		// Private helpers
        private IQueryable<EventViewModel> GetIQueryable(APIDataLanguage lang, Expression<Func<Event, bool>> predicate)
		{
			switch (lang)
			{
				case APIDataLanguage.EN:
                    return (Table as IQueryable<Event>).Where(predicate).OrderByDescending(e => e.ID).Select(@event => new EventViewModel
					{
						ID = @event.ID,
                        AdditionalPrice = @event.AdditionalPrice,
                        Description = @event.DescriptionEng ?? @event.DescriptionArm,
                        EventDateTime = @event.EventDateTime,
                        ImgLink = @event.ImgLink,
                        Name = @event.NameEng ?? @event.NameArm,
                        RestaurantID = @event.RestaurantID,
                        RestoLogoLink = @event.Restaurant.LogoLink,
                        RestoName = @event.Restaurant.NameEng                                        
					});
				case APIDataLanguage.RU:
					return (Table as IQueryable<Event>).Where(predicate).OrderByDescending(e => e.ID).Select(@event => new EventViewModel
					{
						ID = @event.ID,
						AdditionalPrice = @event.AdditionalPrice,
						Description = @event.DescriptionRus ?? @event.DescriptionArm,
						EventDateTime = @event.EventDateTime,
						ImgLink = @event.ImgLink,
						Name = @event.NameRus ?? @event.NameArm,
						RestaurantID = @event.RestaurantID,
						RestoLogoLink = @event.Restaurant.LogoLink,
						RestoName = @event.Restaurant.NameRus ?? @event.Restaurant.NameEng
					});
				case APIDataLanguage.AM:
				default:
					return (Table as IQueryable<Event>).Where(predicate).OrderByDescending(e => e.ID).Select(@event => new EventViewModel
					{
						ID = @event.ID,
						AdditionalPrice = @event.AdditionalPrice,
						Description = @event.DescriptionArm,
						EventDateTime = @event.EventDateTime,
						ImgLink = @event.ImgLink,
						Name = @event.NameArm,
						RestaurantID = @event.RestaurantID,
						RestoLogoLink = @event.Restaurant.LogoLink,
                        RestoName = @event.Restaurant.NameArm ?? @event.Restaurant.NameEng
					});
			}
		}



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
    }
}
