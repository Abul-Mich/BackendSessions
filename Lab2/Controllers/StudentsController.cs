using DbApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly UniDbContext _context;

        public StudentsController(UniDbContext context)
        {
            _context = context;
        }

        // Filter by course, sort by date
        [HttpGet("course/{Id}")]
        public async Task<IActionResult> GetStudentsByCourse(
            [FromRoute] int Id,
            [FromQuery] string order = "asc"
        )
        {
            // Validate course ID
            if (Id <= 0)
                return BadRequest("Course ID must be greater than 0");

            // Validate order parameter
            if (order != "asc" && order != "desc")
                return BadRequest("Order must be 'asc' or 'desc'");

            // Select only needed fields
            var query = _context
                .Enrollments.Where(e => e.Courseid == Id)
                .Select(e => new
                {
                    StudentName = e.Student!.Name,
                    StudentEmail = e.Student.Email,
                    CourseName = e.Course!.Title,
                    EnrollmentDate = e.Enrollmentdate,
                });

            // Apply ordering
            var result =
                order == "desc"
                    ? await query.OrderByDescending(e => e.EnrollmentDate).ToListAsync()
                    : await query.OrderBy(e => e.EnrollmentDate).ToListAsync();

            // Check if any results
            if (!result.Any())
                return NotFound($"No students found for course {Id}");

            return Ok(result);
        }

        // Group by birth year
        [HttpGet("grouped-by-year")]
        public async Task<IActionResult> GetStudentsGroupedByYear()
        {
            var result = await _context
                .Students.GroupBy(s => s.Birthyear)
                .Select(g => new
                {
                    Birthyear = g.Key,
                    Count = g.Count(),
                    Students = g.Select(s => s.Name).ToList(),
                })
                .ToListAsync();
            if (!result.Any())
                return NotFound("No students found");

            return Ok(result);
        }

        // Group by year+country
        [HttpGet("grouped-by-year-country")]
        public async Task<IActionResult> GetStudentsGroupedByYearAndCountry()
        {
            var result = await _context
                .Students.GroupBy(s => new { s.Birthyear, s.Country })
                .Select(g => new
                {
                    Birthyear = g.Key.Birthyear,
                    Country = g.Key.Country,
                    Count = g.Count(),
                    Students = g.Select(s => s.Name).ToList(),
                })
                .ToListAsync();
            if (!result.Any())
                return NotFound("No students found");
            return Ok(result);
        }

        // Count all students
        [HttpGet("count")]
        public async Task<IActionResult> GetTotalStudents()
        {
            var count = await _context.Students.CountAsync();
            return Ok(new { TotalStudents = count });
        }

        // Paginate enrollments
        [HttpGet("enrollments")]
        public async Task<IActionResult> GetEnrollments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 2
        )
        {
            // Validate pagination params
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("pageNumber and pageSize must be greater than 0");

            // Get total for pagination meta
            var totalRecords = await _context.Enrollments.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Fetch paginated data
            var result = await _context
                .Enrollments.Select(e => new
                {
                    StudentName = e.Student!.Name,
                    CourseName = e.Course!.Title,
                    EnrollmentDate = e.Enrollmentdate,
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(
                new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = result,
                }
            );
        }
    }
}
