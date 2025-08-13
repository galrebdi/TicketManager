using System.ComponentModel.DataAnnotations;
using TicketManager.Data;

namespace TicketManager.Models
{
    public class CommentDto
    {
        [Required]
        public Guid TicketId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public AuthorInfo Author { get; set; }
        

    }
}
