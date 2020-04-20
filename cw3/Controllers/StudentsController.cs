using System;
using Microsoft.AspNetCore.Mvc;
using cw3.Models;
using cw3.DAL;
using System.Data.SqlClient;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        [HttpGet("sql/get")]
        public IActionResult GetStudents(string index)
        {

            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18693;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = $"select Student.FirstName, Student.LastName, Enrollment.Semester from Student inner join Enrollment on Student.IdEnrollment = Enrollment.IdEnrollment where Student.IndexNumber =@index";
                com.Parameters.AddWithValue("index", index);

                client.Open();
                var dr = com.ExecuteReader();

                while (dr.Read())
                {
                    var st = new Student { IndexNumber = index, FirstName = dr["FirstName"].ToString(), LastName = dr["LastName"].ToString() };
                    return Ok($"{dr.HasRows} {st.ToString()} {dr["Semester"].ToString()} ");
                }
                return NotFound();
            }

            //return Ok(_dbService.GetStudents());

        }

        [HttpPost("sql/crt {id}")]
        public IActionResult CreateStudent(Student student, string studies)
        {
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18693;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                command.Connection = client;
                command.CommandText = "select StudiesN.IdStudy from StudentN where StudiesN.Name = @S; ";
                command.Parameters.AddWithValue("S", studies);
                client.Open();
                var dr = command.ExecuteReader();
                int eId = -1;
                while (dr.Read())
                {
                    eId = int.Parse(dr["IdStudy"].ToString());
                }
                client.Close();
                command.CommandText = "Insert into Student values (@index,@Fname,@Lname,@Date," + eId + ")";
                command.Parameters.AddWithValue("index", student.IndexNumber);
                command.Parameters.AddWithValue("Fname", student.FirstName);
                command.Parameters.AddWithValue("Lname", student.LastName);
                command.Parameters.AddWithValue("Date", student.BirthDate);
                command.Parameters.AddWithValue("E", student.IdEnrollment);

                client.Open();
                command.ExecuteNonQuery();
            }
            return StatusCode((int)System.Net.HttpStatusCode.Created);
            //return BadRequest();400
            //return Createrd("", obiekt pobrany z bazy);
            //return StatusCode((int)HttpStatuseCode.Created, objekt);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateStudetn(int id)
        {

            return Ok("Aktualizacja dokonczona");
        }
        [HttpDelete]
        public IActionResult DeleteStudent(int id)
        {

            return Ok("Usuwanie ukonczone");
        }

        /*
         [HttpGet]
        public string GetStudents(string orderBy)
        {
            return $"Adam, Jan, Kuba sortowanie={orderBy}";

        } 
         
        [HttpGet]
        //[HttpGet("getStudent")]
        public string GetStudents()
        {
            return "Jan Kowalski";
        }
        
        [HttpPost]
        public string PostStudents([FromBody]Student student)
        {
            return "Jan Kowalski";
        }
          
          
        [HttpGet("{id}")]
        public IActionResult GetStudents2([FromRoute] int id)
        {
            if (id == 1)
                return Ok("Jan Kowalski");
            if (id == 2)
                return Ok("Adam Nowak");
            return NotFound("Student not found");

        }

        [HttpGet("{id}")]
        public string GetStudents2([FromRoute] int id)
        {
            if (id == 1)
                return "Jan Kowalski ";
            if (id == 2)
                return "Adam Nowak";
            return "Student not found";

        }

        [HttpPost]
        public string PostStudents([FromQuery], [FromBody]Student student)
        {
            return "Jan Kowalski";
        }

        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet("sql")]
        public IActionResult GetStudents(string index)
        {

            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18693;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                //com.CommandText = $"select * from Authoer where name='{name}'";
                com.CommandText = $"select Student.FirstName, Student.LastName, (Studies.Name)as Studies from Student " +
                    $"inner join Enrollment on Student.IdEnrollment = Enrollment.IdEnrollment " +
                    $"inner join Studies on Enrollment.IdEnrollment = Studies.IdStudy" +
                    $"where Student.IndexNumber=@index;";
                com.Parameters.AddWithValue("index", index);

                client.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student { IndexNumber = dr["IndexNumber"].ToString(), FirstName = dr["FirstName"].ToString(), LastName = dr["LastName"].ToString() };
                    return Ok($"{dr.HasRows} {st.ToString()}");
                }
                return Ok(dr.HasRows);
            }

            //return Ok(_dbService.GetStudents());

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {

            return Ok("Usuwanie ukonczone");
        }
        */
    }
}