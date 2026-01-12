using CvProject.Models;
using CvProject.View.Models.Data;
using CvProject.View.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CvProject.View.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly MyAppContext _context;
        private readonly UserManager<User> _userManager;

        public MessageController(MyAppContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. LISTA MEDDELANDEN (Inkorg & Skickat)
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var received = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.DateSent)
                .ToListAsync();     

            var sent = await _context.Messages
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId)
                .OrderByDescending(m => m.DateSent)
                .ToListAsync();

            var model = new MessageViewModel
            {
                ReceivedMessages = received,
                SentMessages = sent
            };

            return View(model);
        }

        // 2. LÄS ETT MEDDELANDE (Och markera som läst)
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null) return NotFound();

            // Säkerhetskoll: Bara mottagare eller avsändare får läsa
            if (message.ReceiverId != userId && message.SenderId != userId)
            {
                return Unauthorized(); // Eller NotFound() för att dölja
            }

            // Om jag är mottagaren och öppnar det -> Markera som läst
            if (message.ReceiverId == userId && !message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(message);
        }

        // 3. KNAPP: MARKERA SOM LÄST / OLÄST (Från inkorgen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleReadStatus(int id)
        {
            var userId = _userManager.GetUserId(User);
            var message = await _context.Messages.FindAsync(id);

            // Bara mottagaren kan ändra status
            if (message != null && message.ReceiverId == userId)
            {
                message.IsRead = !message.IsRead; // Växlar mellan läst/oläst
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // 4. SKICKA MEDDELANDE (Från CV-profilen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string receiverId, string subject, string body, string anonymousName)
        {
            var senderId = _userManager.GetUserId(User);

            // Om man inte är inloggad krävs ett namn
            if (senderId == null && string.IsNullOrWhiteSpace(anonymousName))
            {
                return BadRequest("Du måste ange ett namn.");
            }

            var message = new Message
            {
                SenderId = senderId, // Kan vara null om anonym
                ReceiverId = receiverId,
                Subject = subject,
                Body = body,
                DateSent = DateTime.Now,
                IsRead = false,
                SenderName = senderId == null ? anonymousName : null // Spara namn om anonym
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Cv"); // Eller tillbaka till profilen
        }

        // 5. TA BORT MEDDELANDE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var message = await _context.Messages.FindAsync(id);

            // Tillåt borttagning om du är mottagare ELLER avsändare
            if (message != null && (message.ReceiverId == userId || message.SenderId == userId))
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}