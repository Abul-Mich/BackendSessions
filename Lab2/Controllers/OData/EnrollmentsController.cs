using DbApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace DbApi.Controllers.OData
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EnrollmentsController : ODataController
    {
        private readonly UniDbContext _context;

        public EnrollmentsController(UniDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Enrollments.Include(e => e.Student).Include(e => e.Course));
        }
    }
}
