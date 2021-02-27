using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Repositories
{
    public class RestaurantImgRepository : BaseRepository<RestaurantImg>, IRepository<RestaurantImg>
    {
        // Constructors
        public RestaurantImgRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.RestaurantsImgs;
        }

        // Delete By Id
        public int Delete(int id)
        {
            Context.Entry(new RestaurantImg()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new RestaurantImg()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }
    }
}
