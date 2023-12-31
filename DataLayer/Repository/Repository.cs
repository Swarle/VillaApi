﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Repository.Interfaces;
using DataLayer.Specification.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(ApplicationContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
        public virtual void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }
        public virtual async Task<IEnumerable<TEntity>> Find(ISpecification<TEntity> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        public virtual async Task<TEntity?> FindSingle(ISpecification<TEntity> specification)
        {
            return await ApplySpecification(specification).SingleOrDefaultAsync();
        }

        public virtual async Task<bool> FindAny(ISpecification<TEntity> specification)
        {
            return await ApplySpecification(specification).AnyAsync();
        }

        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
        {
            return SpecificationEvaluator<TEntity>.GetQuery(_dbSet.AsQueryable(), specification);
        }
    }
}
