using AutoMapper;
using DbApi.CodeFirst;
using DbApi.DTOs;
using DbApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityController : ControllerBase
    {
        private readonly UniversityContext _context;
        private readonly IMapper _mapper;

        public UniversityController(UniversityContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ─── POST endpoints ───────────────────────────────────────────

        [HttpPost("teachers")]
        public async Task<IActionResult> AddTeacher([FromBody] CreateTeacherDto dto)
        {
            var teacher = _mapper.Map<CfTeacher>(dto);
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return Ok(
                new
                {
                    teacher.Id,
                    teacher.Name,
                    teacher.Email,
                }
            );
        }

        [HttpPost("courses")]
        public async Task<IActionResult> AddCourse([FromBody] CreateCourseDto dto)
        {
            var teacher = await _context.Teachers.FindAsync(dto.TeacherId);
            if (teacher == null)
                return NotFound($"Teacher {dto.TeacherId} not found");

            var course = _mapper.Map<CfCourse>(dto);
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return Ok(
                _mapper.Map<CourseViewModel>(
                    await _context
                        .Courses.Include(c => c.Teacher)
                        .FirstAsync(c => c.Id == course.Id)
                )
            );
        }

        [HttpPost("students")]
        public async Task<IActionResult> AddStudent([FromBody] CreateStudentDto dto)
        {
            var student = _mapper.Map<CfStudent>(dto);
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<StudentViewModel>(student));
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll([FromBody] CreateEnrollmentDto dto)
        {
            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null)
                return NotFound($"Student {dto.StudentId} not found");

            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
                return NotFound($"Course {dto.CourseId} not found");

            var enrollment = new CfEnrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                EnrollmentDate = DateTime.UtcNow,
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // reload with includes for clean response
            var result = await _context
                .Enrollments.Include(e => e.Student)
                .Include(e => e.Course)
                .FirstAsync(e => e.Id == enrollment.Id);

            return Ok(_mapper.Map<EnrollmentViewModel>(result));
        }

        // ─── GET endpoints ────────────────────────────────────────────

        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers()
        {
            var teachers = await _context.Teachers.ToListAsync();
            return Ok(
                teachers.Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Email,
                })
            );
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses.Include(c => c.Teacher).ToListAsync();
            return Ok(_mapper.Map<List<CourseViewModel>>(courses));
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context
                .Students.Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .ToListAsync();
            return Ok(_mapper.Map<List<StudentViewModel>>(students));
        }

        [HttpGet("enrollments")]
        public async Task<IActionResult> GetEnrollments()
        {
            var enrollments = await _context
                .Enrollments.Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
            return Ok(_mapper.Map<List<EnrollmentViewModel>>(enrollments));
        }

        // ─── DELETE endpoints ─────────────────────────────────────────

        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> RemoveCourse([FromRoute] int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound($"Course {id} not found");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("students/{id}")]
        public async Task<IActionResult> RemoveStudent([FromRoute] int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound($"Student {id} not found");

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
