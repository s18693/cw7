using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;

namespace cw3.Controllers
{
    [Route("api")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentsDbService dbService;
        public IConfiguration Configuration { get; set; }

        public EnrollmentsController(IStudentsDbService ISDS, IConfiguration configuration)
        {
            dbService = ISDS;
            Configuration = configuration;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
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
            return Created("", dbService.GetEnrollment());
        }

        [HttpPost("promote")]
        [Authorize(Roles = "employee")]
        public IActionResult getPromote(PromoteStudents promote)
        {
            dbService.PromoteStudents(promote);
            if (dbService.getMsg() == -4)
                return BadRequest("Something gone wrong :C");
            return Created(" ", dbService.GetEnrollment());

        }
        private bool checkPassword(string p)
        {
            if (p.Equals("a"))
                return true;
            return false;
        }

        //Sprawdzam czy role dzialaja
        [HttpPost("s")]
        [Authorize(Roles = "student")]
        //[Authorize(Users = "LOL")]
        public IActionResult postForSutdent()
        {

            return Unauthorized("You are only student bro :P");
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult login(LoginRequest login)
        {

            if (checkPassword(login.password))
            {
                var claims = new[]
                {
                //new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "LOL"),
                //new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student"),
                //new Claim(ClaimTypes.Role, "employee")
            };



                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                issuer: "student",
                audience: "student",
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: creds
                );
                //Sprawdzałem czemu nie dziala i po prostu secretKey byl za krotki :P
                //IdentityModelEventSource.ShowPII = true;

                return Ok(new
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = Guid.NewGuid()
                });

            }
            else
                return BadRequest("Password is wrong");
        }
    }
}
