using TicketManager.Enums;

namespace TicketManager.Data
{
    public class Ticket
    {
        public Guid Id { get;set;} 
        public Guid ClientId {get;set;}
        public Client Client {get;set;}
        public string? ProductName {get; set;}
        public string? ProblemDescription {get; set;}
        public byte[]? Attachment {get; set;}
        public Guid? EmployeeId {get; set;}
        public Employee Employee {get; set;}
        public TicketStatus Status {get;set;}
        public DateTime CreationTime { get; set;} 
        public ICollection<Comment> Comments { get; set;}

        
    }
}
