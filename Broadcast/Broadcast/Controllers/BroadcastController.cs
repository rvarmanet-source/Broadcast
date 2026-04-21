using Broadcast.Models;
using Broadcast.Services;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.Controllers
{
    public class BroadcastController : Controller
    {
        private readonly IBroadcastService _broadcastService;
        private readonly ILogger<BroadcastController> _logger;

        public BroadcastController(IBroadcastService broadcastService, ILogger<BroadcastController> logger)
        {
            _broadcastService = broadcastService;
            _logger = logger;
        }

        // GET: Broadcast
        public async Task<IActionResult> Index()
        {
            var messages = await _broadcastService.GetAllMessagesAsync();
            return View(messages);
        }

        // GET: Broadcast/Active
        public async Task<IActionResult> Active()
        {
            var messages = await _broadcastService.GetActiveMessagesAsync();
            return View(messages);
        }

        // GET: Broadcast/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var message = await _broadcastService.GetMessageByIdAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            await _broadcastService.IncrementViewCountAsync(id);
            return View(message);
        }

        // GET: Broadcast/Create
        public IActionResult Create()
        {
            return View(new BroadcastMessageViewModel());
        }

        // POST: Broadcast/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BroadcastMessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = await _broadcastService.CreateMessageAsync(model);
                    TempData["SuccessMessage"] = $"Broadcast message '{message.Title}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating broadcast message");
                    ModelState.AddModelError("", "An error occurred while creating the message.");
                }
            }

            return View(model);
        }

        // GET: Broadcast/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var message = await _broadcastService.GetMessageByIdAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            var model = new BroadcastMessageViewModel
            {
                Id = message.Id,
                Title = message.Title,
                Content = message.Content,
                Priority = message.Priority,
                Category = message.Category,
                ScheduledFor = message.ScheduledFor,
                SendImmediately = message.Status == MessageStatus.Sent
            };

            return View(model);
        }

        // POST: Broadcast/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BroadcastMessageViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _broadcastService.UpdateMessageAsync(id, model);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Broadcast message updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    return NotFound();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating broadcast message {MessageId}", id);
                    ModelState.AddModelError("", "An error occurred while updating the message.");
                }
            }

            return View(model);
        }

        // POST: Broadcast/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _broadcastService.DeleteMessageAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Broadcast message archived successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Message not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting broadcast message {MessageId}", id);
                TempData["ErrorMessage"] = "An error occurred while archiving the message.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Broadcast/Send/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(int id)
        {
            try
            {
                var success = await _broadcastService.SendMessageAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Broadcast message sent successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Message not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending broadcast message {MessageId}", id);
                TempData["ErrorMessage"] = "An error occurred while sending the message.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}