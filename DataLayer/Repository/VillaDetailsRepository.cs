using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Models;

namespace DataLayer.Repository
{
    //public class VillaDetailsRepository : Repository<VillaDetails>
    //{
    //    private readonly ApplicationContext _context;
    //    public VillaDetailsRepository(ApplicationContext context) : base(context)
    //    {
    //        _context = context;
    //    }

    //    public override async Task CreateAsync(VillaDetails entity)
    //    {
    //        entity.CreatedDate = DateTime.Now;
    //        entity.UpdatedDate = entity.CreatedDate;
    //        await _context.VillaDetails.AddAsync(entity);
    //    }

    //    public override void Update(VillaDetails entity)
    //    {
    //        entity.UpdatedDate = DateTime.Now;
    //        _context.VillaDetails.Update(entity);
    //    }
    //}
}
