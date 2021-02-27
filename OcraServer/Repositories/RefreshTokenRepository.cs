using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRepository<RefreshToken>
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
            Table = Context.RefreshTokens;
        }

        public Task<RefreshToken> GetByToken(string token)
        {
            return Table.Include(rf => rf.User).Where(rf => rf.Token == token && rf.User.IsActive).FirstOrDefaultAsync();
        }

        public int Delete(int id)
        {
			Context.Entry(new RefreshToken()
			{
				ID = id
			}).State = EntityState.Deleted;
			return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
			Context.Entry(new RefreshToken()
			{
				ID = id
			}).State = EntityState.Deleted;
			return SaveChangesAsync();
        }
    }
}
