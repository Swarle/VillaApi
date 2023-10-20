using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repository
{
    public class UserRepository : Repository<Users>
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public override async Task CreateAsync(Users entity)
        {
            entity.CreatedDate = DateTime.Now;
            
            await _context.AddAsync(entity);
        }
    }
}
