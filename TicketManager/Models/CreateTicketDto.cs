using System.Net.Mail;
using TicketManager.Data;
using TicketManager.Enums;

namespace TicketManager.Models
{
    public class CreateTicketDto
    {
 
        public Guid ClientId { get; set; }
        public ClientType Type { get; set; }
        public string ProductName { get; set; }
        public string  ProblemDescription { get; set; }
        public byte[] Attachment { get; set; }
        public DateTime CreationTime { get; set; }
        public TicketStatus Status { get; set; }

    }
}
