using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Repositories
{
    public class FeedbackImgRepository : BaseRepository<FeedbackImg>, IRepository<FeedbackImg>
    {
        // Constructors
        public FeedbackImgRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.FeedbacksImgs;
        }

        // Delete by Id
        public int Delete(int id)
        {
            Context.Entry(new FeedbackImg()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new FeedbackImg()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }
    }
}
