using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;
using OcraServer.Models.EntityFrameworkModels;

namespace OcraServer.Repositories
{
    public class ApplicationUserRepository : IDisposable
    {
        protected ApplicationDbContext Context { get; set; }


        public ApplicationUserRepository(ApplicationDbContext context)
        {
            Context = context;
        }


        public async Task<ApplicationUser> GetWithRefreshTokens(string username)
        {
            var resultList = await Context.Users.GroupJoin(Context.RefreshTokens, u => u.Id, rt => rt.UserID, (u, rts) => new { user = u, rts })
                                          .Where(x =>x.user.UserName == username).ToListAsync();
            var result = resultList.FirstOrDefault();
            if (result == null)
            {
                return null;
            }

            ApplicationUser user = result.user;
            user.RefreshTokens = result.rts.ToList();

            return user;
        }

        // Implementing IDisposable
        bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Context.Dispose();
                // Free any managed objects here. 
                //
            }

            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }
    }
}
