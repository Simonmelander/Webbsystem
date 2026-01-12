using CvProject.Models;
using CvProject.View.Models.Data;
using CvProject.View.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


        public async Task<IActionResult> Index()
        {
            try
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
            catch (Exception ex)
            {
                return Content($"Ett fel uppstod: {ex.Message}");
            }
        }


        public async Task<IActionResult> Details(int id)
        {
            try 
            {
                var userId = _userManager.GetUserId(User);

                var message = await _context.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (message == null) return NotFound();


                if (message.ReceiverId != userId && message.SenderId != userId)
                {
                    return Unauthorized();
                }


                if (message.ReceiverId == userId && !message.IsRead)
                {
                    message.IsRead = true;
                    await _context.SaveChangesAsync();
                }

                return View(message);
            }
            catch (Exception ex)
            {
                return Content($"Ett fel uppstod: {ex.Message}");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleReadStatus(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var message = await _context.Messages.FindAsync(id);

                if (message != null && message.ReceiverId == userId)
                {
                    message.IsRead = !message.IsRead;
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content($"Ett fel uppstod: {ex.Message}");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string receiverId, string subject, string body, string anonymousName)
        {
            try
            {
                var senderId = _userManager.GetUserId(User);

                if (senderId == null && string.IsNullOrWhiteSpace(anonymousName))
                {
                    return BadRequest("Du måste ange ett namn.");
                }

                var message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Subject = subject,
                    Body = body,
                    DateSent = DateTime.Now,
                    IsRead = false,
                    SenderName = senderId == null ? anonymousName : null
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Cv");
            }
            catch (Exception ex) 
            { 
                return Content($"Ett fel uppstod: {ex.Message}");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var message = await _context.Messages.FindAsync(id);

                if (message != null && (message.ReceiverId == userId || message.SenderId == userId))
                {
                    _context.Messages.Remove(message);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content($"Ett fel uppstod: {ex.Message}");
            }
        }
    }
}