using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DoAnLTW.Models;
using DoAnLTW.Services;

namespace DoAnLTW.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentController(
            IAppointmentService appointmentService,
            UserManager<ApplicationUser> userManager)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var appointments = await _appointmentService.GetAppointmentsByUserAsync(user.Id);
            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var doctors = await _appointmentService.GetAvailableDoctorsAsync();
            ViewBag.Doctors = doctors;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            var date = Request.Form["date"].ToString();
            var time = Request.Form["time"].ToString();
            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time))
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ngày và giờ khám.";
                return View(appointment);
            }
            if (!DateTime.TryParse($"{date} {time}", out DateTime appointmentDate))
            {
                TempData["ErrorMessage"] = "Ngày giờ không hợp lệ.";
                return View(appointment);
            }
            appointment.AppointmentDate = appointmentDate;
            var user = await _userManager.GetUserAsync(User);
            appointment.PatientId = user.Id;
            appointment.DoctorId = null;
            appointment.Status = AppointmentStatus.Pending;
            ModelState.Remove("DoctorId");
            ModelState.Remove("Doctor");
            ModelState.Remove("PatientId");
            ModelState.Remove("Patient");
            if (ModelState.IsValid)
            {
                await _appointmentService.CreateAppointmentAsync(appointment);
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin.";
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _appointmentService.UpdateAppointmentAsync(appointment);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Prescribe(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();
            return View(appointment);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Prescribe(int id, string Prescription, string Diagnosis, string DoctorNote)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();
            appointment.Prescription = Prescription;
            appointment.Diagnosis = Diagnosis;
            appointment.DoctorNote = DoctorNote;
            appointment.Status = AppointmentStatus.Completed;
            await _appointmentService.UpdateAppointmentAsync(appointment);
            TempData["SuccessMessage"] = "Kê toa thành công.";
            return RedirectToAction("Details", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();
            return View(appointment);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();
            return View(appointment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DateTime AppointmentDate)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();
            appointment.AppointmentDate = AppointmentDate;
            await _appointmentService.UpdateAppointmentAsync(appointment);
            TempData["SuccessMessage"] = "Cập nhật lịch hẹn thành công.";
            return RedirectToAction("Index", "Home");
        }
    }
}