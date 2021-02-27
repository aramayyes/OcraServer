using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Repositories
{
    public class ReservedTableRepository : BaseRepository<ExternalReservation>, IRepository<ExternalReservation>
    {
        // Constructors
        public ReservedTableRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.ReservedTables;
        }

        // Get by Rest

        public List<ExternalReservation> GetByRest(int restID)
        {
            return Table.Where(rt => rt.RestaurantID == restID).ToList();
        }

        public Task<List<ExternalReservation>> GetByRestAsync(int restID)
        {
            return Table.Where(rt => rt.RestaurantID == restID).OrderBy(rt => rt.ReservationDateTime).ToListAsync();
        }

        // Delete By Id
        public int Delete(int id)
        {
            Context.Entry(new ExternalReservation()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new ExternalReservation()
            {
                ID = id
            }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }
    }
}
