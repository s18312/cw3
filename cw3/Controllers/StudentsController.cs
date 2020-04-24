using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using cw3.DAL;
using cw3.DTOs;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentDbService _dbService;
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s18312;Integrated Security=True";

        public IConfiguration Configuration { get; set; }


        public StudentsController([FromServices] IStudentDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
        }
        

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            var list = new List<Student>();
            
            using (SqlConnection con = new SqlConnection(ConString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "select * from Student";

                    con.Open();
                    SqlDataReader dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        var st = new Student();
                        st.IndexNumber = dr["IndexNumber"].ToString();
                        st.FirstName = dr["FirstName"].ToString();
                        st.LastName = dr["LastName"].ToString();
                        list.Add(st);
                    }

                }
            }
           

            return Ok(list);
        } 

        [HttpGet("{id}")]
        public IActionResult GetStudents(int id)
        {

            using (SqlConnection con = new SqlConnection(ConString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "select E.IdEnrollment, E.Semester, E.IdStudy, E.StartDate  from Enrollment E JOIN Student S ON E.IdEnrollment = S.IdEnrollment WHERE S.IndexNumber = @id;";
                    com.Parameters.Add(new SqlParameter("id", id));

                    con.Open();
                    SqlDataReader dr = com.ExecuteReader();
                    String result = null;
                    if (dr.Read())
                    {
                        result += "IdEnrollment: " + dr["IdEnrollment"].ToString() + " ";
                        result += "Semester: " + dr["Semester"].ToString() + " ";
                        result += "IdStudy: " + dr["IdStudy"].ToString() + " ";
                        result += "StartDate: " + dr["StartDate"].ToString();
                    }
                   

                    return Ok(result);
                }
            }
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(String index, [FromBody]Student stud)
        {
            if (stud.IndexNumber == index)
            {
                stud.FirstName = "Tymoteusz";
            }
            return Ok("Aktualizacja dokończona " + stud.FirstName + " " + stud.LastName);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(String index, [FromBody]Student stud)
        {
            if (stud.IndexNumber == index)
            {
                stud = null;
            }
            return Ok("Usuwanie ukończone");
        }

        [HttpPost]
        public IActionResult Login(LoginRequestDto request)
        {
            var claims = new[]
{
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "jan123"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Haslo1234"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );


            String password = "";
            using (SqlConnection con = new SqlConnection(ConString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "select Password from Student where IndexNumber = @id";
                    com.Parameters.Add(new SqlParameter("id", request.Login));

                    con.Open();
                    SqlDataReader dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        password = dr["Password"].ToString();
                        dr.Close();
                    }

                }
            }



            if (request.Haslo.Equals(password))
            {
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = Guid.NewGuid()
                });
            }
            else
            {
                return Ok("Błędne dane logowania");
            }
            
        }

        /* [HttpGet("{id}")]
         public IActionResult ifExists(String index)
         {
             bool existence = _dbService.ifExists(index);
             if (!existence)
             {
                 return NotFound();
             }
             return Ok("Znaleziono");
         } */

    }
}