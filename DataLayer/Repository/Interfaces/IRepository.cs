using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Specification;

namespace DataLayer.Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public Task CreateAsync(TEntity entity);
        public Task<TEntity?> GetByIdAsync(Guid id);
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public void Update(TEntity entity);
        public void Delete(TEntity entity);
        public Task<IEnumerable<TEntity>> Find(ISpecification<TEntity> specification);
    }
}
