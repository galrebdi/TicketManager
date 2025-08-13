using TicketManager.Models;

namespace TicketManager.Data
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public List<TicketDto> Tickets { get; set; }
    }
}
