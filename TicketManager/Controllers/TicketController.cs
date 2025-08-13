using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TicketManager.Data;
using TicketManager.Enums;
using TicketManager.Models;

namespace TicketManager.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TicketController : Controller
    {

        private readonly TicketManagerDbContext _context;

        public TicketController(TicketManagerDbContext context)
        {
            _context = context;
        }


        [HttpPost]
      
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            var client = await _context.Clients.FindAsync(dto.ClientId);
            if (client == null)
                return NotFound("Client not found");

            var userRole = User.FindFirst("role")?.Value;

            if (userRole != "client")
                return BadRequest("Only external clients can create tickets.");

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                ClientId = dto.ClientId,
                ProductName = dto.ProductName,
                ProblemDescription = dto.ProblemDescription,
                Attachment = dto.Attachment,
                CreationTime = DateTime.UtcNow,
                Status = TicketStatus.New
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(new { ticket.Id, Message = "Ticket created successfully" });
        }


        [HttpGet]
        public async Task<IActionResult> GetTicketsByClient(Guid clientId)
        {
            var tickets = await _context.Tickets
                .Where(t => t.ClientId == clientId)
                .Include(t => t.Employee)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    ProductName = t.ProductName,
                    ProblemDescription = t.ProblemDescription,
                    Status = t.Status,
                    CreationTime = t.CreationTime,
                    AssignedEmployeeName = t.Employee != null ? t.Employee.FullName : null
                })
                .ToListAsync();

            return Ok(tickets);
        }


        [HttpPut("assign")]
        public async Task<IActionResult> AssignTicket(Guid ticketId, Guid employeeId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return NotFound("Ticket not found");

            if (ticket.EmployeeId != null && ticket.Status != TicketStatus.New)
                return BadRequest("Ticket already assigned");

            ticket.EmployeeId = employeeId;


            await _context.SaveChangesAsync();
            return Ok("Ticket assigned");
        }


        [HttpGet("manager/all")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Employee)
                .Include(t => t.Client)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    ProductName = t.ProductName,
                    ProblemDescription = t.ProblemDescription,
                    Status = t.Status,
                    CreationTime = t.CreationTime,
                    AssignedEmployeeName = t.Employee != null ? t.Employee.FullName : null
                })
                .ToListAsync();

            return Ok(tickets);
        }


        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetAssignedTickets(Guid employeeId)
        {
            var tickets = await _context.Tickets
                .Where(t => t.EmployeeId == employeeId)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    ProductName = t.ProductName,
                    ProblemDescription = t.ProblemDescription,
                    Status = t.Status,
                    CreationTime = t.CreationTime
                })
                .ToListAsync();

            return Ok(tickets);
        }


        [HttpPut("status/{ticketId}")]
        public async Task<IActionResult> ChangeStatusAsync(Guid ticketId, TicketStatus ticketStatus)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return NotFound("Ticket not found");

            if (ticketStatus == TicketStatus.Closed && ticket.Status != TicketStatus.Fixed)
                return BadRequest("You cannot close a ticket unless its fixed");

            ticket.Status = ticketStatus;
            await _context.SaveChangesAsync();

            return Ok("Ticket closed");
        }


        [HttpPut("edit/{ticketId}")]
        public async Task<IActionResult> EditTicket(Guid ticketId, TicketDto ticketDto)
        {
            var userRole = User.FindFirst("role")?.Value;

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound("Ticket not found");
            }

            if (ticketDto != null && ticketDto.Status == TicketStatus.New && userRole == "client")
            {
                ticket.ProductName = ticketDto.ProductName;
                ticket.ProblemDescription = ticketDto.ProblemDescription;
                ticket.Status = TicketStatus.New;

                await _context.SaveChangesAsync();

                return Ok("Ticket updated successfully.");
            }
            else
            {
                return BadRequest("Only clients can edit tickets, and only if the status is 'New'.");
            }
        }

        [HttpGet("manager/filter")]
        public async Task<IActionResult> GetTicketsWithFilters(TicketStatus? status, Guid? employeeId, Guid? clientId)
        {
            var query = _context.Tickets
                .Include(t => t.Employee)
                .Include(t => t.Client)
                .AsQueryable();


            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (employeeId.HasValue)
                query = query.Where(t => t.EmployeeId == employeeId.Value);

            if (clientId.HasValue)
                query = query.Where(t => t.ClientId == clientId.Value);

            var tickets = await query.Select(t => new TicketDto
            {
                Id = t.Id,
                ProductName = t.ProductName,
                ProblemDescription = t.ProblemDescription,
                Status = t.Status,
                CreationTime = t.CreationTime,
                AssignedEmployeeName = t.Employee != null ? t.Employee.FullName : null,
                ClientName = t.Client != null ? t.Client.Name : null,


            }).ToListAsync();

            return Ok(tickets);
        }



        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketDetails(Guid ticketId)
        {
            var userRole = User.FindFirst("role")?.Value;
            if (userRole != "manager")
            {
                return Forbid("Only support managers can view ticket details.");
            }

            var ticket = await _context.Tickets
                .Include(t => t.Employee)
                .Include(t => t.Client)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                return NotFound("Ticket not found.");

            var ticketDto = new TicketDto
            {
                Id = ticket.Id,
                ProductName = ticket.ProductName,
                ProblemDescription = ticket.ProblemDescription,
                Status = ticket.Status,
                CreationTime = ticket.CreationTime,
                AssignedEmployeeName = ticket.Employee?.FullName,
                ClientName = ticket.Client?.Name
            };

            return Ok(ticketDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(Guid clientId)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client == null)
                return NotFound("Client not found");

            var userRole = User.FindFirst("role")?.Value;

            if (userRole != "client")
                return BadRequest("Only external clients can access their tickets.");

            var tickets = await _context.Tickets
           .Where(t => t.ClientId == clientId)
           .ToListAsync();

            return Ok(tickets);
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeTicket(Guid EmployeeId)
        {
            var Employee = await _context.Employees.FindAsync(EmployeeId);
            if (Employee == null)
                return NotFound("Employee not found");

            var userRole = User.FindFirst("role")?.Value;

            if (userRole != "employee")
                return BadRequest("Only external Employees can access their tickets.");

            var tickets = await _context.Tickets
           .Where(t => t.EmployeeId == EmployeeId)
           .ToListAsync();

            return Ok(tickets);
        }
    }
}
