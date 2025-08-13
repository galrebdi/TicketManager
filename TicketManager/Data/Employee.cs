using TicketManager.Enums;

namespace TicketManager.Data
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public byte[]? Image { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserType Type { get; set; }
        public string HashedPassword { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Ticket> Tickets { get; set; }


        
    }
}
        