using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cw3.Services;


namespace cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentsDbService dbService;

        public EnrollmentsController(IStudentsDbService ISDS)
        {
            dbService = ISDS;
        }

        [HttpPost]
        public IActionResult post(AddStudentRequest request)
        {
            dbService.AddStudnet(request);
            if (dbService.getMsg() == -1)
                return BadRequest("Can not add");
            if (dbService.getMsg() == -2)
                return BadRequest("Studies not found");
            if (dbService.getMsg() == -3)
                return BadRequest("Stident index is not uniqe");
            if (dbService.getMsg() == -4)
                return BadRequest("Something gone wrong :C");
            return Created("",dbService.GetEnrollment());
        }

        [HttpPost("promote")]
        public IActionResult getPromote(PromoteStudents promote)
        {
            dbService.PromoteStudents(promote);
            if (dbService.getMsg() == -4)
                return BadRequest("Something gone wrong :C");
            return Created(" ", dbService.GetEnrollment());

        }
        // POST: api/Enrollments
        /*
        [HttpPost("enrollments")]
        public void Post(string INumber, string FName, string LName, string BDate, string Studies)
        {
            dbService.AddStudnet(INumber, FName, LName, BDate, Studies);
            return Created("",);
        }

             [HttpGet]
        public IActionResult GetStudent()
        {
            return Ok(dbService.GetStudents());
        }


        // GET: api/Enrollments
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Enrollments/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // PUT: api/Enrollments/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */



    }
}
