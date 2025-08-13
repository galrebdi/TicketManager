using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TicketManager.Data;
using TicketManager.Enums;
using TicketManager.Helpers;
using TicketManager.Migrations;
using TicketManager.Models;
using static System.Net.Mime.MediaTypeNames;

namespace TicketManager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly TicketManagerDbContext _context;
        private readonly IConfiguration _configuration;

        public EmployeeController(TicketManagerDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateEmployeeDto dto)
        {
            string key = _configuration["MasterKey"];

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            string hashedPassword = SecurityHelper.Encrypt(dto.Password, key);

            Employee employee = new Employee
            {
                FullName = dto.FullName,
                Email = dto.Email,
                HashedPassword = hashedPassword,
                Type = Enums.UserType.Employee,
                IsActive = true,
                MobileNumber = dto.MobileNumber,
                Image = dto.Image,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address,

            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "The employee has been created successfully",
                employeeId = employee.Id
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetListAsync()
        {
            var employees = await _context.Employees.Where(x => x.Type == Enums.UserType.Employee && x.IsActive == true).ToListAsync();
            return Ok(employees);

        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditEmployee(Guid id, EmployeeDto employeeDto)

        {
            var userRole = User.FindFirst("role")?.Value;

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("employee not found");
            }

            if (employeeDto != null  && userRole == "manager")
            {
                employee.FullName = employeeDto.FullName;
                employee.MobileNumber = employeeDto.MobileNumber;
                employee.Email = employeeDto.Email;
                employee.DateOfBirth = employeeDto.DateOfBirth;
                employee.Address = employeeDto.Address;

                _context.Update(employee);
                await _context.SaveChangesAsync();

                return Ok("employee updated successfully.");
            }
            else
            {
                return BadRequest("Could not update the employee");
            }
        }
        [HttpPut("set-status/{id}")]
        public async Task<IActionResult> SetActiveStatusAsync(Guid id , bool IsActive)
        {
            var userRole = User.FindFirst("role")?.Value;

            var Employee = await _context.Employees.FindAsync(id);
            if (Employee == null)
            {
                return NotFound("employee not found");
            }

            if ( userRole == "manager")
            {
                Employee.IsActive = IsActive;


                _context.Update(Employee);
                await _context.SaveChangesAsync();

                return Ok("Employee status updated successfully");
            }
            else
            {
                return BadRequest("You are not authorized to update employee status.");
            }
        }
    }
}
    
