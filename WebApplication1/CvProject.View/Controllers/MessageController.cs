using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CvProject.View.Models.Data;
using CvProject.Models; // Innehåller din User-klass och Message-klass
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CvProject.View.Controllers
{
    public class MessageController : Controller
    {
        private readonly MyAppContext _context;

        // FIX: Här skriver vi ut hela sökvägen (CvProject.Models.User) för att undvika krocken
        private readonly UserManager<CvProject.Models.User> _userManager;

        public MessageController(MyAppContext context, UserManager<CvProject.Models.User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Inkorgen
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // "User" här refererar till den inloggade användaren (egenskapen på controllern)
            var userId = _userManager.GetUserId(User);

            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.DateSent)
                .ToListAsync();

            return View(messages);
        }

        // Skicka meddelande
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

            // Om SenderId är null är man inte inloggad -> spara det anonyma namnet
            if (msg.SenderId == null)
            {
                msg.AnonymousName = anonymousName ?? "Anonym";
            }

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // Skicka tillbaka användaren
            string referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer)) return RedirectToAction("Index", "Home");
            return Redirect(referer);
        }

        // Markera som läst
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

        // Ta bort
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