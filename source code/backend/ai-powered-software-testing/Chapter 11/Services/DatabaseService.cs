

using Chapter_11.Data;

namespace Chapter_11.Services
{
    public interface IDatabaseService
    {
        Task<T> GetByIdAsync<T>(string id) where T : class;
        Task<T> SaveAsync<T>(T entity) where T : class;
        Task UpdateAsync<T>(T entity) where T : class;
        Task DeleteAsync<T>(string id) where T : class;
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly SpectrumDbContext _dbContext;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(SpectrumDbContext dbContext, ILogger<DatabaseService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<T> GetByIdAsync<T>(string id) where T : class
        {
            try
            {
                return await _dbContext.Set<T>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get entity of type {Type} with id {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public async Task<T> SaveAsync<T>(T entity) where T : class
        {
            try
            {
                await _dbContext.Set<T>().AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save entity of type {Type}", typeof(T).Name);
                throw;
            }
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            try
            {
                _dbContext.Set<T>().Update(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update entity of type {Type}", typeof(T).Name);
                throw;
            }
        }

        public async Task DeleteAsync<T>(string id) where T : class
        {
            try
            {
                var entity = await GetByIdAsync<T>(id);
                if (entity != null)
                {
                    _dbContext.Set<T>().Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete entity of type {Type} with id {Id}", typeof(T).Name, id);
                throw;
            }
        }
    }
}
