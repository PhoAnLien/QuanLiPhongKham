using DoAnLTW.Data;
using DoAnLTW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnLTW.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")] // Uncomment this line if you have JWT or cookie authentication for Admin role
    public class AdminApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public AdminApiController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Lấy danh sách user chờ duyệt (bao gồm cả bác sĩ và bệnh nhân)
        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var pendingUsers = await _context.Users
                .Where(u => !u.IsApproved && (u.Role == "Doctor" || u.Role == "Patient"))
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.PhoneNumber,
                    u.Role,
                    u.IsApproved,
                    u.Specialization // Chỉ có ở doctor, sẽ null ở patient
                })
                .ToListAsync();
            return Ok(pendingUsers);
        }

        // Duyệt user (bác sĩ hoặc bệnh nhân)
        [HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User không tồn tại.");

            user.IsApproved = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "Duyệt user thành công" });
            }
            return BadRequest(result.Errors);
        }

        // Lấy tất cả lịch hẹn
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Select(a => new
                {
                    a.Id,
                    PatientName = a.Patient.FullName,
                    DoctorName = a.Doctor.FullName,
                    a.AppointmentDate,
                    a.Reason,
                    Status = a.Status.ToString(),
                    a.CreatedAt,
                    a.UpdatedAt
                })
                .ToListAsync();
            return Ok(appointments);
        }

        // Xóa user (bác sĩ hoặc bệnh nhân)
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User không tồn tại.");

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "Xóa user thành công" });
            }
            return BadRequest(result.Errors);
        }
    }
}