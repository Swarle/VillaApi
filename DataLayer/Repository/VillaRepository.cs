using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Models;

namespace DataLayer.Repository
{
    public class VillaRepository : Repository<Villa>
    {
        private readonly ApplicationContext _context;
        public VillaRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public override async Task CreateAsync(Villa entity)
        {
            entity.VillaDetails.CreatedDate = DateTime.Now;
            entity.VillaDetails.UpdatedDate = entity.VillaDetails.CreatedDate;

            await _context.AddAsync(entity);
        }

        public override void Update(Villa entity)
        {
            entity.VillaDetails.UpdatedDate = DateTime.Now;

            _context.Update(entity);
        }
    }
}
