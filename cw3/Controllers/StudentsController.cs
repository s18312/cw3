using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {

        

        [HttpGet]
        public string GetStudents(string orderBy)
        {
            return "Kowalski, Malewski, Andrzejewski sortowanie = {orderBy}";
        } 

        [HttpGet("{id}")]
        public IActionResult GetStudents(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }
            else if (id == 2)
            {
                return Ok("Malewski");
            }
            return NotFound("Nie znaleziono studenta");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, [FromBody]Student stud)
        {
            if(stud.IdStudent == id)
            {
                stud.FirstName = "Tymoteusz";
            }
            return Ok("Aktualizacja dokończona " + stud.FirstName + " " + stud.LastName);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id, [FromBody]Student stud)
        {
            if (stud.IdStudent == id)
            {
                stud = null;
            }
            return Ok("Usuwanie ukończone");
        }

    }
}