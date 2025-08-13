using TicketManager.Data;

namespace TicketManager.Models
{
    public class CreateClientDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
    }
    public class UpdateClientDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

}
