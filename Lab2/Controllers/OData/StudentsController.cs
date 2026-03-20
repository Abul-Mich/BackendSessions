using DbApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace DbApi.Controllers.OData
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StudentsController : ODataController
    {
        private readonly UniDbContext _context;

        public StudentsController(UniDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Students.Include(s => s.Enrollments).ThenInclude(e => e.Course));
        }
    }
}
