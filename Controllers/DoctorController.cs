using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DoAnLTW.Models;
using DoAnLTW.Services;
using DoAnLTW.ViewModels;

namespace DoAnLTW.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppointmentService _appointmentService;

        public DoctorController(
            UserManager<ApplicationUser> userManager,
            IAppointmentService appointmentService)
        {
            _userManager = userManager;
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var doctor = await _userManager.GetUserAsync(User);
            var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctor.Id);

            var viewModel = new DoctorDashboardViewModel
            {
                DoctorInfo = doctor,
                Appointments = appointments
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorAppointmentsJson()
        {
            var doctor = await _userManager.GetUserAsync(User);
            var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctor.Id);

            var events = appointments.Select(a => new
            {
                title = $"Bệnh nhân: {a.Patient?.FullName}",
                start = a.AppointmentDate.ToString("yyyy-MM-dd"),
                url = Url.Action("Details", "Appointment", new { id = a.Id }),
                backgroundColor = a.Status == AppointmentStatus.Completed ? "#198754" : "#0d6efd",
                borderColor = a.Status == AppointmentStatus.Completed ? "#198754" : "#0d6efd"
            }).ToList();

            return Json(events);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAppointment(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Confirmed;
                await _appointmentService.UpdateAppointmentAsync(appointment);
            }
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _appointmentService.UpdateAppointmentAsync(appointment);
            }
            return RedirectToAction(nameof(Dashboard));
        }
    }
}