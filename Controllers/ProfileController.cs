using DoAnLTW.Data;
using DoAnLTW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.EntityFrameworkCore; // Added for ToListAsync()
using Microsoft.AspNetCore.Mvc.Rendering; // Added for SelectList

namespace DoAnLTW.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            ApplicationUser userToView;
            var loggedInUser = await _userManager.GetUserAsync(User);

            if (string.IsNullOrEmpty(id))
            {
                // If no ID is provided, show the logged-in user's own profile.
                userToView = loggedInUser;
            }
            else
            {
                // If an ID is provided, find that user to display their profile.
                userToView = await _userManager.FindByIdAsync(id);
            }

            if (userToView == null)
            {
                return NotFound();
            }

            // Check the role of the user whose profile is being viewed
            ViewData["ProfileIsDoctor"] = await _userManager.IsInRoleAsync(userToView, "Doctor");
            ViewData["ProfileIsPatient"] = await _userManager.IsInRoleAsync(userToView, "Patient");

            // Pass the ID of the user being viewed and the logged-in user to the view
            ViewData["CurrentUserId"] = userToView.Id;
            ViewData["LoggedInUserId"] = loggedInUser?.Id;

            return View(userToView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ApplicationUser model, IFormFile avatarFile, string id)
        {
             // id này là của người dùng cần cập nhật, được gửi từ form
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Lấy người dùng đang đăng nhập để kiểm tra quyền
            var loggedInUser = await _userManager.GetUserAsync(User);

            // Cập nhật các trường chung
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;

            // Xử lý upload avatar
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var fileExt = Path.GetExtension(avatarFile.FileName);
                // Tạo tên file duy nhất để tránh trùng lặp
                var fileName = $"{Guid.NewGuid()}{fileExt}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }
                // Chỉ lưu đường dẫn tương đối, thay thế dấu \ bằng dấu /
                user.ProfilePicture = $"/uploads/avatars/{fileName}".Replace('\\', '/');
            }

            // Cập nhật các trường theo vai trò
            if (await _userManager.IsInRoleAsync(user, "Doctor"))
            {
                // Chỉ admin mới có thể cập nhật chuyên khoa
                if (await _userManager.IsInRoleAsync(loggedInUser, "Admin"))
                {
                    user.Specialization = model.Specialization;
                }
                // user.Education = model.Education; // Bỏ đi
                user.Description = model.Description; // Cho phép bác sĩ cập nhật mô tả
            }
            if (await _userManager.IsInRoleAsync(user, "Patient"))
            {
                user.BloodType = model.BloodType;
                user.Allergies = model.Allergies;
                user.MedicalHistory = model.MedicalHistory;
            }
            
            var result = await _userManager.UpdateAsync(user);

            if(result.Succeeded)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật.";
                // Thêm các lỗi vào ModelState để hiển thị nếu cần
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                ViewData["CurrentUserId"] = user.Id; // Đảm bảo ID được truyền lại view khi có lỗi
                return View(user); // Quay lại view với lỗi
            }
            
            return RedirectToAction("Index", new { id = user.Id }); // Chuyển hướng lại trang cá nhân của người dùng vừa cập nhật
        }
    }
} 