using lab12.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace lab12.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly InvoiceContext _context;

        public CoursesController(InvoiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Course> GetAll()
        {
            return _context.Courses.Where(c => c.Active).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Course> GetById(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseID == id && c.Active);
            if (course == null)
            {
                return NotFound("Curso no encontrado o ya eliminado.");
            }
            return course;
        }

        [HttpPost]
        public ActionResult<Course> Create(Course course)
        {
            if (course == null)
            {
                return BadRequest("Curso inválido.");
            }

            _context.Courses.Add(course);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = course.CourseID }, course);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Course course)
        {
            if (id != course.CourseID)
            {
                return BadRequest("El ID del curso no coincide.");
            }

            var existingCourse = _context.Courses.FirstOrDefault(c => c.CourseID == id && c.Active);
            if (existingCourse == null)
            {
                return NotFound("Curso no encontrado o ya eliminado.");
            }

            existingCourse.Name = course.Name;
            existingCourse.Credit = course.Credit;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseID == id && c.Active);
            if (course == null)
            {
                return NotFound("Curso no encontrado o ya eliminado.");
            }

            course.Active = false;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public IActionResult DeleteCourses([FromBody] DeleteCoursesRequest request)
        {
            if (request.CourseIds == null || !request.CourseIds.Any())
            {
                return BadRequest("Lista de cursos vacía.");
            }

            var courses = _context.Courses.Where(c => request.CourseIds.Contains(c.CourseID) && c.Active).ToList();
            if (!courses.Any())
            {
                return NotFound("No se encontraron cursos activos para eliminar.");
            }

            foreach (var course in courses)
            {
                course.Active = false;
            }

            _context.SaveChanges();

            return Ok(new { Message = "Cursos eliminados correctamente." });
        }
    }
}
