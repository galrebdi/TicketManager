using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketManager.Data;
using TicketManager.Helpers;
using TicketManager.Migrations;
using TicketManager.Models;

namespace TicketManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly TicketManagerDbContext _context;

        public LoginController(IConfiguration configuration, TicketManagerDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }


        [HttpPost("login")]

        public async Task<IActionResult> LoginAsync(LoginDto input)
        {
            string key = _configuration["MasterKey"];

            Employee? employee = await _context.Employees.FirstOrDefaultAsync(x => x.Email == input.Email);

            if (employee != null && VerifyPassword(input.Password, employee.HashedPassword, key))
            {
               
                var token = GenerateJwtToken(employee.Type == Enums.UserType.Employee ? "employee" : "manager");
                return Ok(new { token });
            }
                
            Client? client = await _context.Clients.FirstOrDefaultAsync(x => x.Email == input.Email);

            if(client != null && VerifyPassword(input.Password, client.HashedPassword, key))
            {
                var token = GenerateJwtToken("client");
                return Ok(new { token });
            }
           
            return Unauthorized("Invalid credentials");
        }



        private string GenerateJwtToken(string role)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim(ClaimTypes.Role, role) };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword, string key)
        {
            string decryptedPassword = SecurityHelper.Decrypt(hashedPassword, key);

            return decryptedPassword == inputPassword;
        }
    }
}
