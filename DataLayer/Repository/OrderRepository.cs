using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Models;
using DataLayer.Repository.Interfaces;

namespace DataLayer.Repository
{
    public class OrderRepository : Repository<Orders>
    {
        private readonly ApplicationContext _context;
        public OrderRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public override async Task CreateAsync(Orders entity)
        {
            entity.CreatedDate = DateTime.Now;
            entity.UpdatedDate = entity.CreatedDate;
            await _context.Orders.AddAsync(entity);
        }

        public override void Update(Orders entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Update(entity);
        }
    }
}
