namespace TicketManager.Models
{
    public class UpdateTicketDto
    {
        public Guid ClientId { get; set; }
        public string ProductName { get; set; }
        public string ProblemDescription { get; set; }
        public byte[] Attachment { get; set; }
    }
}
