using DoAnLTW.Data;
using DoAnLTW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTW.Controllers
{
    [ApiController]
    [Route("api/appointment")]
    public class AppointmentApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AppointmentApiController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Đặt lịch hẹn mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointment = new Appointment
            {
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                AppointmentDate = model.AppointmentDate,
                Reason = model.Reason,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đặt lịch thành công", appointment });
        }

        // Lấy chi tiết lịch hẹn
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.Id == id)
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
                .FirstOrDefaultAsync();

            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        // Xem danh sách lịch hẹn của user (Patient hoặc Doctor)
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == userId || a.DoctorId == userId)
                .OrderByDescending(a => a.AppointmentDate)
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

        // Cập nhật lịch hẹn
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AppointmentUpdateRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != model.Id) return BadRequest();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.AppointmentDate = model.AppointmentDate;
            appointment.Reason = model.Reason;
            appointment.Status = model.Status;
            appointment.UpdatedAt = DateTime.UtcNow;

            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật lịch hẹn thành công", appointment });
        }

        // Hủy lịch hẹn
        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();
            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã hủy lịch hẹn" });
        }
    }

    public class AppointmentRequest
    {
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }
    }

    public class AppointmentUpdateRequest
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}