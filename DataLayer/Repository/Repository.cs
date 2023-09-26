﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Repository.Interfaces;
using DataLayer.Specification;
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
        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
        public  void Delete(TEntity entity)
        {
             _dbSet.Remove(entity);
        }
        public async Task<IEnumerable<TEntity>> Find(ISpecification<TEntity> specification)
        {
            var query = SpecificationEvaluator<TEntity>.GetQuery(_dbSet.AsQueryable(), specification);
            return await query.ToListAsync();
        }

    }
}