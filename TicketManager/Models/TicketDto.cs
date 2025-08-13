using TicketManager.Enums;

namespace TicketManager.Models
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProblemDescription { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreationTime { get; set; }
        public string AssignedEmployeeName { get; set; }
        public string ClientName { get; set; }
    }
}


                   