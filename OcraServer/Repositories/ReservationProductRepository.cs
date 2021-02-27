using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Repositories
{
    public class ReservationProductRepository : BaseRepository<ReservationProduct>, IRepository<ReservationProduct>
    {
        // Constructors
        public ReservationProductRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.ReservationProducts;
        }

        // Delete By Id
        public int Delete(int reservationId, int productId)
        {
            Context.Entry(new ReservationProduct()
            {
                ReservationID = reservationId,
                ProductID = productId
            }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int reservationId, int productId)
        {
            Context.Entry(new ReservationProduct()
            {
                ReservationID = reservationId,
                ProductID = productId
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
    }
}
