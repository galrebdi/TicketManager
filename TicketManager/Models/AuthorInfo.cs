using TicketManager.Enums;

namespace TicketManager.Models
{
    public class AuthorInfo
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public AuthorType AuthorType { get; set; }

    }
}
