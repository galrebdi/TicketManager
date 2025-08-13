using System.Security.Cryptography;
using System.Text;

namespace TicketManager.Models
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }

}