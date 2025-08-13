using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TicketManager.Data;
using TicketManager.Models;


namespace TicketManager.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly TicketManagerDbContext _context;

        public CommentController(TicketManagerDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> PostAsync(CommentDto commentdto)
        {

            if (string.IsNullOrWhiteSpace(commentdto.Content))
            {
                return BadRequest("Comment text cannot be empty.");
            }
            Comment comment = new Comment
            {
              TicketId = commentdto.TicketId,
                Content = commentdto.Content,
                Author = commentdto.Author,
              CreationTime = DateTime.Now,
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }
  


}
}
    