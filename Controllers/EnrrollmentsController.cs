using lab12.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab12.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly InvoiceContext _context;

        public EnrollmentsController(InvoiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Enrollment> GetAll()
        {
            return _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Enrollment> GetById(int id)
        {
            var enrollment = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefault(e => e.EnrollmentID == id);

            if (enrollment == null)
            {
                return NotFound("Inscripción no encontrada.");
            }

            return enrollment;
        }

        [HttpPost]
        public ActionResult<Enrollment> Create(Enrollment enrollment)
        {
            if (enrollment == null)
            {
                return BadRequest("Inscripción inválida.");
            }

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = enrollment.EnrollmentID }, enrollment);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return BadRequest("El ID de la inscripción no coincide.");
            }

            var existingEnrollment = _context.Enrollments.FirstOrDefault(e => e.EnrollmentID == id);
            if (existingEnrollment == null)
            {
                return NotFound("Inscripción no encontrada.");
            }

            existingEnrollment.Date = enrollment.Date;
            existingEnrollment.CourseID = enrollment.CourseID;
            existingEnrollment.StudentID = enrollment.StudentID;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var enrollment = _context.Enrollments.FirstOrDefault(e => e.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound("Inscripción no encontrada.");
            }

            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public IActionResult EnrollStudent([FromBody] EnrollmentRequest request)
        {
            if (request.CourseIds == null || !request.CourseIds.Any())
            {
                return BadRequest("La solicitud es inválida.");
            }

            var student = _context.Students.FirstOrDefault(s => s.StudentID == request.StudentId && s.Active);
            if (student == null)
            {
                return NotFound("Estudiante no encontrado.");
            }

            var courses = _context.Courses.Where(c => request.CourseIds.Contains(c.CourseID) && c.Active).ToList();
            if (courses.Count != request.CourseIds.Count)
            {
                return NotFound("Uno o más cursos no encontrados o no están activos.");
            }

            var enrollments = courses.Select(course => new Enrollment
            {
                StudentID = request.StudentId,
                CourseID = course.CourseID,
                Date = DateTime.Now
            }).ToList();

            _context.Enrollments.AddRange(enrollments);
            _context.SaveChanges();

            return Ok(new { Message = "Estudiante matriculado en los cursos correctamente." });
        }
    }
}
