using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OcraServer.EntityFramework;

namespace OcraServer.Repositories
{
    public class BaseRepository<T> : IDisposable where T : class
    {
        protected ApplicationDbContext Context { get; set; }
        protected DbSet<T> Table;

        public BaseRepository(ApplicationDbContext context)
        {
            Context = context;
        }

        // Get Count
        public virtual int GetCount() => Table.Count();
        public virtual Task<int> GetCountAsync() => Table.CountAsync();

        // Get One
        public virtual T GetOne(int? id) => Table.Find(id);
        public virtual async Task<T> GetOneAsync(int? id) => await Table.FindAsync(id);

        // Get All
        public virtual List<T> GetAll() => Table.ToList();
        public virtual Task<List<T>> GetAllAsync() => Table.ToListAsync();

        // Add One
        public int Add(T entity)
        {
            Table.Add(entity);
            return SaveChanges();
        }

        public async Task<int> AddAsync(T entity)
        {
            Table.Add(entity);
            return await SaveChangesAsync();
        }

        // Add Range
        public int AddRange(IList<T> entities)
        {
            Table.AddRange(entities);
            return SaveChanges();
        }
        public Task<int> AddRangeAsync(IList<T> entities)
        {
            Table.AddRange(entities);
            return SaveChangesAsync();
        }

        // Modify One
        public int Save(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return SaveChanges();
        }

        public async Task<int> SaveAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return await SaveChangesAsync();
        }

        // Delete One
        public int Delete(T entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            return SaveChanges();
        }

        public async Task<int> DeleteAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            return await SaveChangesAsync();
        }

        // Exists
        public bool Exists(Func<T, bool> predicate)
        {
            return Table.Count(predicate) > 0;
        }

        // Execute SQL code
        public List<T> ExecuteQuery(string sql) => Table.FromSql(sql).ToList();

        public Task<List<T>> ExecuteQueryAsync(string sql)
        {
            return Table.FromSql(sql).ToListAsync();
        }

        public List<T> ExecuteQuery(string sql, object[] sqlParametersObjects)
        {
            return Table.FromSql(sql, sqlParametersObjects).ToList();
        }

        public Task<List<T>> ExecuteQueryAsync(string sql, object[] sqlParametersObjects)
        {
            return Table.FromSql(sql).ToListAsync();
        }

        // Save Changes
        internal int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //var telemetry = new TelemetryClient();
                //telemetry.TrackException(ex);

                //Thrown when there is a concurrency error
                //If Entries propery is null, no records were modified
                //entities in Entries threw error due to timestamp/conncurrency
                //for now, just rethrow the exception
                throw;
            }
            catch (DbUpdateException ex)
            {
                //var telemetry = new TelemetryClient();
                //telemetry.TrackException(ex);

                //Thrown when database update fails
                //Examine the inner exception(s) for additional 
                //details and affected objects
                //for now, just rethrow the exception
                throw;
            }
            catch (Exception ex)
            {
                //var telemetry = new TelemetryClient();
                //telemetry.TrackException(ex);

                //some other exception happened and should be handled
                throw;
            }
        }

        internal async Task<int> SaveChangesAsync()
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //var telemetry = new TelemetryClient();
                //telemetry.TrackException(ex);

                //Thrown when there is a concurrency error
                //for now, just rethrow the exception
                throw;
            }
            catch (DbUpdateException ex)
            {
                //var telemetry = new TelemetryClient();
                //telemetry.TrackException(ex);


                //Thrown when database update fails
                //Examine the inner exception(s) for additional 
                //details and affected objects
                //for now, just rethrow the exception
                throw;
            }
            catch (Exception ex)
            {
                //var telemetry = new TelemetryClient();
                //telemetry.TrackException(ex);

                //some other exception happened and should be handled
                throw;
            }
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
