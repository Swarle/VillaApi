using DataLayer.Context;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public VillaController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/<VillaController>
        [HttpGet]
        public IEnumerable<Villa> Get()
        {
            return _context.Villa.ToList();
        }

        
    }
}
