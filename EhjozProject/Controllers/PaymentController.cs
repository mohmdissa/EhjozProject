using EhjozProject.Application.Interfaces;
using EhjozProject.Domain.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EhjozProject.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(
            IPaymentService paymentService,
            IBookingService bookingService,
            UserManager<ApplicationUser> userManager)
        {
            _paymentService = paymentService;
            _bookingService = bookingService;
            _userManager = userManager;
        }

        // GET: Payment/Checkout?bookingId=1
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout(int bookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null || booking.UserId != user.Id)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Payment/Process
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int bookingId, string paymentMethod)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null || booking.UserId != user.Id)
            {
                return NotFound();
            }

            // Get or create payment
            var payment = await _paymentService.GetPaymentByBookingIdAsync(bookingId);
            if (payment != null)
            {
                // Mark payment as paid (simplified for testing)
                await _paymentService.MarkAsPaidAsync(payment.Id);
            }

            // Confirm the booking
            await _bookingService.ConfirmBookingAsync(bookingId);

            return RedirectToAction(nameof(Success), new { bookingId = bookingId });
        }

        // GET: Payment/Success?bookingId=1
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Success(int bookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null || booking.UserId != user.Id)
            {
                return NotFound();
            }

            ViewBag.BookingId = bookingId;
            return View();
        }
    }
}