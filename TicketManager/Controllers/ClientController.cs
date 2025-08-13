using AutoMapper;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketManager.Data;
using TicketManager.Enums;
using TicketManager.Helpers;
using TicketManager.Models;

namespace TicketManager.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly TicketManagerDbContext _ticketManagerDbContext;
        private readonly IConfiguration _configuration;
        public ClientController(TicketManagerDbContext ticketManagerDbContext, IConfiguration configuration)
        {
            _ticketManagerDbContext = ticketManagerDbContext;
            _configuration = configuration;
        }
        [HttpGet("with-tickets")]

        public async Task<ActionResult<List<ClientDto>>> GetAllClientsWithTickets()
        {
            var clients = await _ticketManagerDbContext.Clients
                .Include(c => c.Tickets)
                .ToListAsync();

            var clientDtos = clients.Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Tickets = c.Tickets.Select(t => new TicketDto
                {
                    Id = t.Id,
                    ProductName = t.ProductName,
                    ProblemDescription = t.ProblemDescription,
                    CreationTime = t.CreationTime,
                    AssignedEmployeeName = t.Employee.FullName,
                    Status = t.Status,
                }).ToList()
            }).ToList();
            return Ok(clientDtos);
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> RegisterAsync(CreateClientDto dto)
        {
            string key = _configuration["MasterKey"];

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            string hashedPassword = SecurityHelper.Encrypt(dto.Password, key);

            Client client = new Client
            {
                Name = dto.Name,
                Email = dto.Email,
                HashedPassword = hashedPassword,
                IsActive = true,

            };

            await _ticketManagerDbContext.Clients.AddAsync(client);
            await _ticketManagerDbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "The employee has been created successfully",
                employeeId = client.Id
            });
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateClientDto updateClientDto)
        {
            var userRole = User.FindFirst("role")?.Value;

            var Client = await _ticketManagerDbContext.Clients.FindAsync(id);
            if (Client == null)
            {
                return NotFound("Client not found");
            }

            if (updateClientDto != null && userRole == "manager")
            {
                Client.Name = updateClientDto.Name;
                Client.Email = updateClientDto.Email;

                _ticketManagerDbContext.Update(Client);
                await _ticketManagerDbContext.SaveChangesAsync();

                return Ok("Client updated successfully.");
            }
            else
            {
                return BadRequest("Could not update the Client");
            }
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetListAsync()
        {
            var Clients = await _ticketManagerDbContext.Clients.Where(x => x.IsActive == true).ToListAsync();
            return Ok(Clients);

        }
    
       [HttpPut("set-status/{id}")]
        public async Task<IActionResult> ChangeClientActivationAsync(Guid id, bool IsActive)
        {
            var userRole = User.FindFirst("role")?.Value;

            var Client = await _ticketManagerDbContext.Clients.FindAsync(id);
            if (Client == null)
            {
                return NotFound("Client not found");
            }

            if (userRole != "manager")
            {
                Client.IsActive = IsActive;


                _ticketManagerDbContext.Update(Client);
                await _ticketManagerDbContext.SaveChangesAsync();

                return Ok("lient status updated successfully.");
            }
            else
            {
                return BadRequest("An unexpected error occurred while updating the client.");
            }
        }
    }
}
    
