using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Models.EntityFrameworkModels;
using OcraServer.Models.ViewModels;

namespace OcraServer.Repositories
{
    public class FeedbackRepository : BaseRepository<Feedback>, IRepository<Feedback>
    {
        //Constructors
        public FeedbackRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.Feedbacks;
        }

        // Get By Rest
        public List<FeedbackViewModel> GetViewByRest(int restID)
        {
            var feedbacks = GetIQueryable(f => f.RestaurantID == restID).ToList();
            return feedbacks;
        }
        public Task<List<FeedbackViewModel>> GetViewByRestAsync(int restID)
        {
			var feedbacks = GetIQueryable(f => f.RestaurantID == restID).ToListAsync();
			return feedbacks;
        }

        // Get Count By Rest
        public int GetCountByRestID(int restID) => Table.Count(f => f.RestaurantID == restID);
        public Task<int> GetCountByRestIDAsync(int restID) => Table.CountAsync(f => f.RestaurantID == restID);

        // Get Range By Rest
        public List<FeedbackViewModel> GetRangeByRest(int restID, int startIndex, int count)
        {
			var feedbacks = GetIQueryable(f => f.RestaurantID == restID).Skip(startIndex - 1).Take(count).ToList();
			return feedbacks;
        }

        public Task<List<FeedbackViewModel>> GetRangeByRestAsync(int restID, int startIndex, int count)
        {
			var feedbacks = GetIQueryable(f => f.RestaurantID == restID).Skip(startIndex - 1).Take(count).ToListAsync();
			return feedbacks;
        }

		// Get By Rest and user
		public List<FeedbackViewModel> GetViewByRestNUser(int restID, string userID)
		{
            var feedbacks = GetIQueryable(f => f.RestaurantID == restID && f.UserID == userID).ToList();
			return feedbacks;
		}
        public Task<List<FeedbackViewModel>> GetViewByRestNUserAsync(int restID, string userID)
		{
			var feedbacks = GetIQueryable(f => f.RestaurantID == restID && f.UserID == userID).ToListAsync();
			return feedbacks;
		}

        // Delete By Id
        public int Delete(int id)
        {
            Context.Entry(new Feedback()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new Feedback()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        // Private helpers
        private IQueryable<FeedbackViewModel> GetIQueryable(Expression<Func<Feedback, bool>> predicate)
        {
            return (Table as IQueryable<Feedback>).Where(predicate).OrderByDescending(f => f.Created).Select(feedback => new FeedbackViewModel
            {
                ID = feedback.ID,
                Created = feedback.Created,
                HasBeenEdited = feedback.HasBeenEdited,
                Text = feedback.Text,
                Mark = feedback.Mark,
                UserID = feedback.UserID,
                UserImgUrl = feedback.User.ImgUrl,
                UserFullName = feedback.User.FirstName + " " + feedback.User.LastName
            });
        }
    }
}
