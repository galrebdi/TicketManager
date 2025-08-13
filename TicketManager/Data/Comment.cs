using TicketManager.Models;

namespace TicketManager.Data
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public string Content { get; set; }
        public AuthorInfo Author { get; set; }
        public DateTime CreationTime { get; set; }

     
    }
}
