using lab12.Models;
using lab12.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace lab12.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly InvoiceContext _context;

        public StudentsController(InvoiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Student> GetAll()
        {
            return _context.Students.Where(s => s.Active).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Student> GetById(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentID == id && s.Active);
            if (student == null)
            {
                return NotFound();
            }
            return student;
        }

        [HttpPost]
        public ActionResult<Student> Create(Student student)
        {
            if (student == null)
            {
                return BadRequest("Estudiante inválido");
            }

            _context.Students.Add(student);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = student.StudentID }, student);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Student student)
        {
            if (id != student.StudentID)
            {
                return BadRequest("El ID del estudiante no coincide");
            }

            var existingStudent = _context.Students.FirstOrDefault(s => s.StudentID == id && s.Active);
            if (existingStudent == null)
            {
                return NotFound("Estudiante no encontrado");
            }

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.Phone = student.Phone;
            existingStudent.Email = student.Email;
            existingStudent.GradeId = student.GradeId;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentID == id && s.Active);
            if (student == null)
            {
                return NotFound("Estudiante no encontrado");
            }

            student.Active = false;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateContactInfoV4(int id, [FromBody] StudentRequestV4 studentRequest)
        {
            if (studentRequest == null)
            {
                return BadRequest("La solicitud de actualización de contacto es inválida.");
            }

            var student = _context.Students.FirstOrDefault(s => s.StudentID == id && s.Active);
            if (student == null)
            {
                return NotFound("Estudiante no encontrado.");
            }

            student.Phone = studentRequest.Phone;
            student.Email = studentRequest.Email;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePersonalInfoV5(int id, [FromBody] StudentRequestV5 studentRequest)
        {
            if (studentRequest == null)
            {
                return BadRequest("La solicitud de actualización de datos personales es inválida.");
            }

            var student = _context.Students.FirstOrDefault(s => s.StudentID == id && s.Active);
            if (student == null)
            {
                return NotFound("Estudiante no encontrado.");
            }

            student.FirstName = studentRequest.FirstName;
            student.LastName = studentRequest.LastName;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public IActionResult InsertStudentsByGradeV6([FromBody] StudentInsertRequestV6 request)
        {
            if (request == null || request.Students == null || request.Students.Count == 0)
            {
                return BadRequest("La solicitud de inserción es inválida.");
            }

            var grade = _context.Grades.FirstOrDefault(g => g.GradeID == request.IdGrade && g.Active);
            if (grade == null)
            {
                return NotFound("Grado no encontrado.");
            }

            var studentsToAdd = request.Students.Select(studentRequest => new Student
            {
                FirstName = studentRequest.FirstName,
                LastName = studentRequest.LastName,
                Phone = studentRequest.Phone,
                Email = studentRequest.Email,
                GradeId = request.IdGrade,
                Active = true
            }).ToList();

            _context.Students.AddRange(studentsToAdd);
            _context.SaveChanges();

            return Ok(new { Message = "Estudiantes insertados correctamente." });
        }
    }
}
