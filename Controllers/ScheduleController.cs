using DoAnLTW.Data;
using DoAnLTW.Models;
using DoAnLTW.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnLTW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ScheduleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Trang quản lý lịch cố định hàng tuần
        [HttpGet]
        public async Task<IActionResult> ManageWeeklySchedule(string id) // id là doctorId
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var doctor = await _userManager.FindByIdAsync(id);
            if (doctor == null) return NotFound();

            var schedule = await _context.DoctorWeeklySchedules
                                    .Where(s => s.DoctorId == id)
                                    .ToListAsync();
            
            var viewModel = new ManageWeeklyScheduleViewModel
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.FullName
            };

            // Nếu bác sĩ chưa có lịch, tạo lịch mặc định (tất cả đều không làm việc)
            if (!schedule.Any())
            {
                for (int i = 0; i < 7; i++)
                {
                    viewModel.Schedules.Add(new DoctorWeeklySchedule
                    {
                        DoctorId = id,
                        DayOfWeek = (DayOfWeek)i,
                        IsWorking = false
                    });
                }
            }
            else
            {
                viewModel.Schedules = schedule.OrderBy(s => s.DayOfWeek).ToList();
            }
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageWeeklySchedule(ManageWeeklyScheduleViewModel viewModel)
        {
            // Xóa lỗi xác thực không cần thiết liên quan đến navigation property "Doctor"
            foreach (var key in ModelState.Keys.Where(k => k.EndsWith(".Doctor")).ToList())
            {
                ModelState.Remove(key);
            }

            if (ModelState.IsValid)
            {
                foreach (var scheduleItem in viewModel.Schedules)
                {
                    // Nếu ngày đó không làm việc, reset ca và ghi chú
                    if (!scheduleItem.IsWorking)
                    {
                        scheduleItem.Shift = ShiftType.None;
                        scheduleItem.Note = null;
                    }

                    var existingSchedule = await _context.DoctorWeeklySchedules
                        .FirstOrDefaultAsync(s => s.Id == scheduleItem.Id);

                    if (existingSchedule != null)
                    {
                        existingSchedule.IsWorking = scheduleItem.IsWorking;
                        existingSchedule.Shift = scheduleItem.Shift;
                        existingSchedule.Note = scheduleItem.Note;
                        _context.Update(existingSchedule);
                    }
                    else
                    {
                         // Đảm bảo DoctorId được gán trước khi thêm mới
                         scheduleItem.DoctorId = viewModel.DoctorId;
                         _context.Add(scheduleItem);
                    }
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật lịch làm việc thành công!";
                return RedirectToAction("Dashboard", "Admin");
            }

            // Nếu model không hợp lệ, trả về view với dữ liệu cũ để người dùng sửa
            var doctor = await _userManager.FindByIdAsync(viewModel.DoctorId);
            viewModel.DoctorName = doctor?.FullName;
            return View(viewModel);
        }

        // Trang quản lý ngày nghỉ phép
        [HttpGet]
        public async Task<IActionResult> ManageLeave(string id) // id là doctorId
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var doctor = await _userManager.FindByIdAsync(id);
            if (doctor == null) return NotFound();

            var leaves = await _context.DoctorLeaves
                                .Where(l => l.DoctorId == id && l.LeaveDate >= DateTime.Today)
                                .OrderBy(l => l.LeaveDate)
                                .ToListAsync();
            
            ViewBag.DoctorId = id;
            ViewBag.DoctorName = doctor.FullName;
            return View(leaves);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLeave(string doctorId, DateTime leaveDate, string reason)
        {
            if(!string.IsNullOrEmpty(doctorId) && leaveDate != default)
            {
                var newLeave = new DoctorLeave
                {
                    DoctorId = doctorId,
                    LeaveDate = leaveDate,
                    Reason = reason
                };
                _context.DoctorLeaves.Add(newLeave);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "Thêm ngày nghỉ thành công!";
            }
            return RedirectToAction(nameof(ManageLeave), new { id = doctorId });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLeave(int id)
        {
            var leave = await _context.DoctorLeaves.FindAsync(id);
            if (leave == null) return NotFound();
            
            var doctorId = leave.DoctorId;
            _context.DoctorLeaves.Remove(leave);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa ngày nghỉ thành công!";

            return RedirectToAction(nameof(ManageLeave), new { id = doctorId });
        }
    }
} 