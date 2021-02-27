using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Repositories
{
    public class RestaurantPanoramaRepository : BaseRepository<RestaurantPanorama>, IRepository<RestaurantPanorama>
    {
        // Constructors
        public RestaurantPanoramaRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.RestaurantsPanoramas;
        }

        // Get by Restaurant ID
        public Task<List<RestaurantPanorama>> GetByRestID(int restID) => Table.Where(rp => rp.RestaurantID == restID).ToListAsync();

        // Delete By Id    
        public int Delete(int id)
        {
            Context.Entry(new RestaurantPanorama()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new RestaurantPanorama()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }
    }
}
