using DoAnLTW.Data;
using DoAnLTW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DoAnLTW.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    public class ApiDoctorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApiDoctorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Lấy danh sách bác sĩ
        [HttpGet]
        public IActionResult GetAll()
        {
            var doctors = _context.Users
                .Where(u => u.Role == "Doctor")
                .Select(u => new {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.PhoneNumber,
                    u.Specialization,
                    u.IsApproved,
                    u.Education,
                    u.Experience,
                    u.WorkingHours
                }).ToList();
            return Ok(doctors);
        }

        // Lấy chi tiết bác sĩ
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var doctor = _context.Users
                .Where(u => u.Role == "Doctor" && u.Id == id)
                .Select(u => new {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.PhoneNumber,
                    u.Specialization,
                    u.Education,
                    u.Experience,
                    u.IsApproved,
                    u.WorkingHours,
                    u.MaxAppointmentsPerDay
                }).FirstOrDefault();
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }

        // Tạo bác sĩ mới (Chỉ admin mới có quyền)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorCreateRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Specialization = model.Specialization,
                Education = model.Education,
                Experience = model.Experience,
                WorkingHours = model.WorkingHours,
                IsApproved = true, // Tự động duyệt khi tạo bởi admin
                Role = "Doctor"
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Doctor");
                return StatusCode(201, new { message = "Tạo bác sĩ thành công", doctor = user });
            }
            return BadRequest(result.Errors);
        }

        // Cập nhật thông tin bác sĩ
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromBody] DoctorUpdateRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != model.Id) return BadRequest();

            var doctor = await _userManager.FindByIdAsync(id);
            if (doctor == null || doctor.Role != "Doctor") return NotFound("Bác sĩ không tồn tại.");

            doctor.FullName = model.FullName;
            doctor.PhoneNumber = model.PhoneNumber;
            doctor.Specialization = model.Specialization;
            doctor.Education = model.Education;
            doctor.Experience = model.Experience;
            doctor.WorkingHours = model.WorkingHours;
            doctor.IsApproved = model.IsApproved; // Có thể cập nhật trạng thái duyệt bởi admin

            var result = await _userManager.UpdateAsync(doctor);
            if (result.Succeeded)
            {
                return Ok(new { message = "Cập nhật thông tin bác sĩ thành công" });
            }
            return BadRequest(result.Errors);
        }

        // Xóa bác sĩ
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var doctor = await _userManager.FindByIdAsync(id);
            if (doctor == null || doctor.Role != "Doctor") return NotFound("Bác sĩ không tồn tại.");

            var result = await _userManager.DeleteAsync(doctor);
            if (result.Succeeded)
            {
                return Ok(new { message = "Xóa bác sĩ thành công" });
            }
            return BadRequest(result.Errors);
        }
    }

    public class DoctorCreateRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public string Education { get; set; }
        public string Experience { get; set; }
        public string WorkingHours { get; set; }
    }

    public class DoctorUpdateRequest
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public string Education { get; set; }
        public string Experience { get; set; }
        public string WorkingHours { get; set; }
        public bool IsApproved { get; set; }
    }
}