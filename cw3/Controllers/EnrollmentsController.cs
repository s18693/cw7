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
using System.Data.SqlClient;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace cw3.Controllers
{
    [Route("api")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        string SqlConnection = "Data Source=db-mssql;Initial Catalog=s18693;Integrated Security=True";

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



        private bool checkPassword(string index, string p, string salt)
        {
            using (var client = new SqlConnection(SqlConnection))
            using (var command = new SqlCommand())
            {
                command.Connection = client;
                client.Open();

                command.CommandText = "select StudentN.Password from StudentN where StudentN.IndexNumber like '" + index + "'";
                var read = command.ExecuteReader();
                if (read.Read())
                {
                    //Console.WriteLine("Read " + read[0].ToString());
                    if (p.Equals(create(read[0].ToString(), salt)))
                        return true;
                    else
                        return false;
                }
                if (!read.Read())
                    return false;
            }
            return false;
        }

        //Sprawdzam czy role dzialaja
        [HttpPost("s")]
        [Authorize(Roles = "student")]
        //[Authorize(Users = "Buka")]
        public IActionResult postForSutdent()
        {
            return Unauthorized("You are only student bro :P");
        }

        string create(string value, string sal)
        {
            Console.WriteLine(sal);
            var v = KeyDerivation.Pbkdf2(password: value, salt: Encoding.UTF8.GetBytes(sal), prf: KeyDerivationPrf.HMACSHA512, iterationCount: 10_000, numBytesRequested: 256 / 8);
            return Convert.ToBase64String(v);
        }

        string createSalt()
        {
            byte[] random = new byte[128 / 8];
            using (var g = RandomNumberGenerator.Create())
            {
                g.GetBytes(random);
                return Convert.ToBase64String(random);
            }

        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult login(LoginRequest login)
        {
            string sal = createSalt();
            string pas = create(login.password, sal);
            if (checkPassword(login.login, pas, sal))
            {
                //właściowści tokenu
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Robert"),
                //new Claim(ClaimTypes.Role, "student"),
                //new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "employee")
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                //Nazywamy tak samo jak w startup !!!!! bo inaczej nie bedzie dzialac
                var token = new JwtSecurityToken
                (
                issuer: "buka",
                audience: "studenta",
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: creds
                );

                //Sprawdzałem czemu key nie dziala i po prostu secretKey byl za krotki :P
                //IdentityModelEventSource.ShowPII = true;

                //Wyśli token
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
