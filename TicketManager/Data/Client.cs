using TicketManager.Enums;

namespace TicketManager.Data
{
    public class Client
    {
        public Guid Id { get; set; }
        public ClientType Type { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Ticket> Tickets { get; set; }

       
    }
}
