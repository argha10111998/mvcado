using EcommerceMVCAdo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;

namespace EcommerceMVCAdo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel model)
        {
            // Basic model validation check
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            string query = @"INSERT INTO AppUser (Username, Email, Password, CreatedAt) 
                         VALUES (@Username, @Email, @Password, GETUTCDATE())";

            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(query, connection);

                // Always use parameters to prevent SQL Injection
                command.Parameters.AddWithValue("@Username", model.Name);
                command.Parameters.AddWithValue("@Email", model.Email);

                // Note: In a real app, hash the password using BCrypt or ASP.NET Core Identity's PasswordHasher
                string hashedPassword = HashPassword(model.Password);
                command.Parameters.AddWithValue("@Password", hashedPassword);

                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return Ok(new { success = true ,message = "User registered successfully!" });
                }

                return BadRequest(new { success = false ,message = "Registration failed." });
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // SQL Server error numbers for unique constraint / duplicate key violations
                return Conflict(new { message = "Username or Email already exists." });
            }
        }

        private string HashPassword(string password)
        {
            // Placeholder for real hashing logic (e.g., BCrypt.Net)
            var passwordhasher = new PasswordHasher<UserModel>();
            return passwordhasher.HashPassword(new UserModel(), password);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            if (String.IsNullOrEmpty(model.Password) || String.IsNullOrEmpty(model.Email)) 
            { 
                return BadRequest("Empty email or password"); 
            }
            UserModel user = null;
            using (SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
            {
                con.Open();
                string query = @"select* from AppUser where Email=@Email;";
                using (SqlCommand cmd = new SqlCommand(query,con))
                {
                    cmd.Parameters.Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        25
                    ).Value = model.Email;
                    cmd.Parameters.Add(
                        "@Password",
                        SqlDbType.NVarChar,
                        100
                    ).Value = model.Password;

                    using (SqlDataReader reader = cmd.ExecuteReader()) 
                    {
                        if (reader.Read())
                        {
                            user = new UserModel
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                UserName = (reader["UserId"]).ToString(),
                                Email = (reader["Email"]).ToString(),
                                Password = (reader["Password"]).ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };
                        }
                    }

                    if (user==null)
                    {
                        return Unauthorized("Wrong email or password");
                    }

                    var passwordhasher = new PasswordHasher<UserModel>();
                    var result = passwordhasher.VerifyHashedPassword(
                        user,
                        user.Password,
                        model.Password
                    );
                    if (result == PasswordVerificationResult.Failed) 
                    {
                        return Unauthorized("Wrong email or password");
                    }

                    var claims = new List<Claim> {
                        new Claim("UserId",user.UserId.ToString()),
                        new Claim("UserEmail",user.Email),
                        new Claim("UserName" , user.UserName)
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        issuer: _configuration["jwt:issuer"],
                        audience: _configuration["jwt:audience"],
                        claims: claims,
                        expires:DateTime.Now.AddHours(1),
                        signingCredentials:credentials
                    );
                    string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                    Response.Cookies.Append(
                        "loginToken",
                        jwtToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            IsEssential = true
                        }
                    );
                    return StatusCode(200,new { message="user logged in", token=jwtToken });

                }
            }
        }
    }
}
