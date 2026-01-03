using EhjozProject.Application.Interfaces;
using EhjozProject.Domain.Models.Booking;
using EhjozProject.Domain.Models.Identity;
using EhjozProject.Domain.Models.Payment;
using EhjozProject.Web.ViewModels.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EhjozProject.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IStadiumService _stadiumService;
        private readonly ITimeSlotService _timeSlotService;
        private readonly IPaymentService _paymentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(
            IBookingService bookingService,
            IStadiumService stadiumService,
            ITimeSlotService timeSlotService,
            IPaymentService paymentService,
            UserManager<ApplicationUser> userManager)
        {
            _bookingService = bookingService;
            _stadiumService = stadiumService;
            _timeSlotService = timeSlotService;
            _paymentService = paymentService;
            _userManager = userManager;
        }

        // GET: Booking/Create?stadiumId=1&date=2024-01-01
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create(int stadiumId, DateTime? date)
        {
            var stadium = await _stadiumService.GetStadiumByIdAsync(stadiumId);
            if (stadium == null || !stadium.IsActive)
            {
                return NotFound();
            }

            var selectedDate = date ?? DateTime.Now;
            var timeSlots = await _timeSlotService.GetTimeSlotsByStadiumIdAndDateAsync(
                stadiumId,
                DateOnly.FromDateTime(selectedDate));

            ViewBag.Stadium = stadium;
            ViewBag.TimeSlots = timeSlots.ToList();
            ViewBag.SelectedDate = selectedDate;

            return View(new BookingViewModel { StadiumId = stadiumId });
        }

        // POST: Booking/Confirm
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int StadiumId, int TimeSlotId, DateTime BookingDate, string? Notes)
        {
            var stadium = await _stadiumService.GetStadiumByIdAsync(StadiumId);
            var timeSlot = await _timeSlotService.GetTimeSlotByIdAsync(TimeSlotId);

            if (stadium == null || timeSlot == null)
            {
                return NotFound();
            }

            var model = new BookingViewModel
            {
                StadiumId = StadiumId,
                TimeSlotId = TimeSlotId,
                StadiumName = stadium.Name,
                BookingDate = BookingDate,
                StartTime = timeSlot.StartTime.ToString("hh:mm tt"),
                EndTime = timeSlot.EndTime.ToString("hh:mm tt"),
                TotalPrice = stadium.PricePerHour,
                Notes = Notes
            };

            return View(model);
        }

        // POST: Booking/ProcessBooking
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessBooking(BookingViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var timeSlot = await _timeSlotService.GetTimeSlotByIdAsync(model.TimeSlotId);
            if (timeSlot == null || !timeSlot.IsAvailable)
            {
                TempData["Error"] = "This time slot is no longer available.";
                return RedirectToAction("Create", new { stadiumId = model.StadiumId });
            }

            var booking = new Booking
            {
                StadiumId = model.StadiumId,
                TimeSlotId = model.TimeSlotId,
                UserId = user.Id,
                BookingDate = model.BookingDate,
                TotalPrice = model.TotalPrice,
                Notes = model.Notes,
                Status = "Pending"
            };

            var createdBooking = await _bookingService.CreateBookingAsync(booking);

            // Create payment record
            var payment = new Payment
            {
                BookingId = createdBooking.Id,
                Amount = model.TotalPrice,
                PaymentDate = DateTime.Now,
                IsPaid = false
            };

            await _paymentService.CreatePaymentAsync(payment);

            return RedirectToAction("Checkout", "Payment", new { bookingId = createdBooking.Id });
        }

        // GET: Booking/MyBookings
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var bookings = await _bookingService.GetBookingsByUserIdAsync(user.Id);
            return View(bookings);
        }

        // GET: Booking/Details/5
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null || booking.UserId != user.Id)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Cancel/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null || booking.UserId != user.Id)
            {
                return NotFound();
            }

            if (booking.Status != "Pending")
            {
                TempData["Error"] = "Only pending bookings can be cancelled.";
                return RedirectToAction(nameof(MyBookings));
            }

            await _bookingService.CancelBookingAsync(id);
            TempData["Success"] = "Booking cancelled successfully.";

            return RedirectToAction(nameof(MyBookings));
        }
    }
}