using DoAnLTW.Data;
using DoAnLTW.Models;
using DoAnLTW.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnLTW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Dashboard(string searchString, string roleFilter, string statusFilter)
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = roles.FirstOrDefault(),
                    IsApproved = user.IsApproved,
                    LockoutEnd = user.LockoutEnd,
                    Specialization = user.Specialization,
                    WorkingHours = user.WorkingHours
                });
            }

            // Apply search and filters
            if (!string.IsNullOrEmpty(searchString))
            {
                userViewModels = userViewModels.Where(u => 
                    u.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase) || 
                    (u.Email != null && u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrEmpty(roleFilter))
            {
                userViewModels = userViewModels.Where(u => u.Role == roleFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                switch (statusFilter)
                {
                    case "approved":
                        userViewModels = userViewModels.Where(u => u.IsApproved && (u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.Now)).ToList();
                        break;
                    case "pending":
                        userViewModels = userViewModels.Where(u => !u.IsApproved).ToList();
                        break;
                    case "banned":
                        userViewModels = userViewModels.Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.Now).ToList();
                        break;
                }
            }

            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentRole"] = roleFilter;
            ViewData["CurrentStatus"] = statusFilter;

            var viewModel = new AdminDashboardViewModel
            {
                AllUsers = userViewModels,
                Doctors = await _userManager.GetUsersInRoleAsync("Doctor"),
                PendingAppointments = await _context.Appointments
                                            .Include(a => a.Patient)
                                            .Where(a => a.Status == AppointmentStatus.Pending)
                                            .OrderBy(a => a.AppointmentDate)
                                            .ToListAsync(),
                ConfirmedAppointments = await _context.Appointments
                                            .Include(a => a.Patient)
                                            .Include(a => a.Doctor)
                                            .Where(a => a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed)
                                            .OrderByDescending(a => a.AppointmentDate)
                                            .ToListAsync(),
                WeeklySchedules = await _context.DoctorWeeklySchedules.ToListAsync(),
                Leaves = await _context.DoctorLeaves.ToListAsync(),
                SchedulesByDoctor = (await _context.DoctorWeeklySchedules.ToListAsync()).ToLookup(s => s.DoctorId)
            };

            var allAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId != null)
                .ToListAsync();

            ViewBag.DoctorAppointments = allAppointments;
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointmentsJson()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed)
                .ToListAsync();

            var events = appointments.Select(a => new
            {
                title = $"BS: {a.Doctor?.FullName} - BN: {a.Patient?.FullName}",
                start = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                url = Url.Action("Details", "Appointment", new { id = a.Id }),
            }).ToList();

            return Json(events);
        }

        [HttpGet]
        public async Task<IActionResult> PatientLookup(string patientSearch)
        {
            var viewModel = new PatientLookupViewModel
            {
                CurrentPatientSearch = patientSearch,
                Results = new List<PatientSearchResult>()
            };

            if (!string.IsNullOrEmpty(patientSearch))
            {
                var patients = await _userManager.GetUsersInRoleAsync("Patient");
                var filteredPatients = patients.Where(p => p.FullName.Contains(patientSearch, StringComparison.OrdinalIgnoreCase) ||
                                                           (p.Email != null && p.Email.Contains(patientSearch, StringComparison.OrdinalIgnoreCase)))
                                               .ToList();

                foreach (var patient in filteredPatients)
                {
                    var appointments = await _context.Appointments
                        .Include(a => a.Doctor)
                        .Where(a => a.PatientId == patient.Id)
                        .OrderByDescending(a => a.AppointmentDate)
                        .ToListAsync();

                    viewModel.Results.Add(new PatientSearchResult
                    {
                        PatientId = patient.Id,
                        FullName = patient.FullName,
                        Email = patient.Email,
                        PhoneNumber = patient.PhoneNumber,
                        Appointments = appointments
                    });
                }
            }

            return View(viewModel);
        }

        // Action để duyệt người dùng
        [HttpPost]
        public async Task<IActionResult> ApproveUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsApproved = true;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Duyệt người dùng thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Duyệt người dùng thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction(nameof(Dashboard));
        }

        // Action để cập nhật vai trò người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                user.Role = role;
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Cập nhật vai trò thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Cập nhật vai trò thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDoctorInfo(string id, string fullName, string specialization, string phoneNumber)
        {
            var doctor = await _userManager.FindByIdAsync(id);
            if (doctor == null || !await _userManager.IsInRoleAsync(doctor, "Doctor"))
            {
                TempData["ErrorMessage"] = "Không tìm thấy bác sĩ.";
                return RedirectToAction("Dashboard");
            }

            doctor.FullName = fullName;
            doctor.Specialization = specialization;
            doctor.PhoneNumber = phoneNumber;

            var result = await _userManager.UpdateAsync(doctor);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin bác sĩ thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Cập nhật thông tin thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Dashboard");
        }

        // Action để xóa người dùng/bác sĩ
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Xóa người dùng thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa người dùng thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction(nameof(Dashboard));
        }

        // Action để cập nhật trạng thái lịch hẹn
        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, AppointmentStatus status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = status;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái lịch hẹn thành công.";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now)
            {
                // User is currently locked out, so unlock them
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Tài khoản đã được mở khóa.";
            }
            else
            {
                // User is not locked out, so lock them
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Tài khoản đã bị khóa.";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDoctorWorkingHours(string doctorId, string workingHours)
        {
            var doctor = await _userManager.FindByIdAsync(doctorId);
            if (doctor == null || !await _userManager.IsInRoleAsync(doctor, "Doctor"))
            {
                TempData["ErrorMessage"] = "Không tìm thấy bác sĩ.";
                return RedirectToAction("Dashboard");
            }
            doctor.WorkingHours = workingHours;
            await _userManager.UpdateAsync(doctor);
            TempData["SuccessMessage"] = "Cập nhật lịch làm việc thành công.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDoctor(int appointmentId, string doctorId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return NotFound();

            var doctor = await _userManager.FindByIdAsync(doctorId);
            if (doctor == null) return NotFound();

            var appointmentDay = appointment.AppointmentDate.DayOfWeek;
            var isOnLeave = await _context.DoctorLeaves.AnyAsync(l => l.DoctorId == doctorId && l.LeaveDate.Date == appointment.AppointmentDate.Date);
            var hasSchedule = await _context.DoctorWeeklySchedules.AnyAsync(s => s.DoctorId == doctorId && s.DayOfWeek == appointmentDay && s.IsWorking);
            
            // KIỂM TRA TRÙNG LỊCH HẸN
            var isBusy = await _context.Appointments.AnyAsync(a => 
                a.DoctorId == doctorId && 
                a.Id != appointmentId && // Bỏ qua chính lịch hẹn này
                a.AppointmentDate == appointment.AppointmentDate &&
                (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed));

            if (isOnLeave || !hasSchedule || isBusy)
            {
                if (isBusy)
                    TempData["ErrorMessage"] = $"Không thể phân công. Bác sĩ {doctor.FullName} đã có lịch hẹn khác vào thời điểm này.";
                else
                    TempData["ErrorMessage"] = $"Không thể phân công. Bác sĩ {doctor.FullName} không có lịch làm việc vào ngày {appointment.AppointmentDate.ToString("dd/MM/yyyy")}.";
                
                return RedirectToAction(nameof(Dashboard));
            }

            appointment.DoctorId = doctorId;
            appointment.Status = AppointmentStatus.Confirmed;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã phân bác sĩ và xác nhận lịch hẹn.";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                return RedirectToAction("Index", "Home");
            }
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa lịch hẹn.";
            return RedirectToAction("Index", "Home");
        }
    }
}