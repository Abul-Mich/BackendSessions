using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApi.DTOs;
using MyApi.Models;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private static List<Student> _students = new List<Student>
        {
            new Student
            {
                Id = 1,
                Name = "Alice",
                Age = 20,
            },
            new Student
            {
                Id = 2,
                Name = "Bob",
                Age = 22,
            },
            new Student
            {
                Id = 3,
                Name = "Charlie",
                Age = 21,
            },
        };

        private Student? FindStudent(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }

        [HttpGet]
        public async Task<ActionResult> GetStudents()
        {
            return Ok(_students);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetStudentById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id must be greater than 0");
            }

            var student = FindStudent(id);

            if (student == null)
            {
                return NotFound($"Student with Id {id} was not found");
            }
            return Ok(student);
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchStudents([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name query parameter is required");
            }

            var filteredStudents = _students
                .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Ok(filteredStudents);
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetLocalizedDate(
            [FromHeader(Name = "Accept-Language")] string language
        )
        {
            try
            {
                if (string.IsNullOrWhiteSpace(language))
                    return BadRequest("Accept-Language header is required");

                var culture = new System.Globalization.CultureInfo(language);
                var localizedDate = DateTime.Now.ToString("D", culture);

                return Ok(new { language = language, date = localizedDate });
            }
            catch (System.Globalization.CultureNotFoundException)
            {
                return BadRequest($"Language '{language}' is not supported");
            }
            finally
            {
                Console.WriteLine($"GetLocalizedDate called with language: {language}");
            }
        }

        [HttpPost("{id:int}")]
        public async Task<ActionResult> UpdateStudent(
            [FromRoute] int id,
            [FromBody] UpdateStudentNameDto updatedStudent
        )
        {
            if (id <= 0)
            {
                return BadRequest("Id must be greater than 0");
            }

            var existingStudent = FindStudent(id);
            if (existingStudent == null)
            {
                return NotFound($"Student with Id {id} was not found");
            }

            existingStudent.Name = updatedStudent.Name;

            return Ok(existingStudent);
        }

        [HttpPost("{id:int}/upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(
            [FromRoute] int id,
            [FromForm] ImageUploadDto uploadImageDto
        )
        {
            if (id <= 0)
            {
                return BadRequest("Id must be greater than 0");
            }

            var student = FindStudent(id);

            if (student == null)
            {
                return NotFound($"Student with id {id} was not found");
            }

            if (
                uploadImageDto == null
                || uploadImageDto.Image == null
                || uploadImageDto.Image.Length == 0
            )
            {
                return BadRequest("Image file is required");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(uploadImageDto.Image.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Only .jpg, .jpeg, .png files are allowed");
            }

            var fileName = $"student_{id}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine("wwwroot", "images", fileName);

            Directory.CreateDirectory(Path.Combine("wwwroot", "images"));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadImageDto.Image.CopyToAsync(stream);
            }

            student.ImagePath = $"/images/{fileName}";

            return Ok(student);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStudent([FromRoute] int id)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0");

            var student = FindStudent(id);

            if (student == null)
                return NotFound($"Student with id {id} was not found");

            _students.Remove(student);

            return NoContent();
        }
    }
}
