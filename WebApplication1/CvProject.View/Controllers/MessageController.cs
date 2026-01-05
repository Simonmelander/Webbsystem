using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CvProject.View.Models.Data;
using CvProject.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CvProject.View.Controllers
{
    public class MessageController : Controller
    {
        private readonly MyAppContext _context;
        private readonly UserManager<CvProject.Models.User> _userManager;

        public MessageController(MyAppContext context, UserManager<CvProject.Models.User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.DateSent)
                .ToListAsync();

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Send(string receiverId, string subject, string body, string anonymousName)
        {
            if (string.IsNullOrEmpty(receiverId) || string.IsNullOrEmpty(body))
            {
                return RedirectToAction("Index", "Home");
            }

            var msg = new Message
            {
                ReceiverId = receiverId,
                Subject = subject ?? "Inget ämne",
                Body = body,
                DateSent = DateTime.Now,
                IsRead = false,
                SenderId = _userManager.GetUserId(User)
            };

            if (msg.SenderId == null)
            {
                msg.AnonymousName = anonymousName ?? "Anonym";
            }

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // Sätter bekräftelsemeddelandet
            TempData["Success"] = "Ditt meddelande har skickats!";

            string referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer)) return RedirectToAction("Index", "Home");
            return Redirect(referer);
        }

        [Authorize]
        public async Task<IActionResult> Read(int id)
        {
            var userId = _userManager.GetUserId(User);
            var msg = await _context.Messages.FindAsync(id);

            if (msg != null && msg.ReceiverId == userId)
            {
                msg.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var msg = await _context.Messages.FindAsync(id);

            if (msg != null && msg.ReceiverId == userId)
            {
                _context.Messages.Remove(msg);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}