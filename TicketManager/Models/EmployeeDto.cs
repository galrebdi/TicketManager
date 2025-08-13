using TicketManager.Data;
using TicketManager.Enums;

namespace TicketManager.Models
{
    public class EmployeeDto
    {
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public byte[]? Image { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string HashedPassword { get; set; }
        public string? Address { get; set; }

    }
}
