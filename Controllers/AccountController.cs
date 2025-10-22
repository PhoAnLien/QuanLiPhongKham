using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DoAnLTW.Models;
using DoAnLTW.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DoAnLTW.Services;

namespace DoAnLTW.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, EmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    IsApproved = true,
                    Role = "Patient",
                    Allergies = "",
                    MedicalHistory = "",
                    BloodType = "",
                    ProfilePicture = "",
                    Specialization = "",
                    LicenseNumber = "",
                    Education = "",
                    Experience = "",
                    WorkingHours = "",
                    MaxAppointmentsPerDay = 20,
                    IsVerified = false,
                    VerifiedAt = null,
                    VerifiedBy = ""
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Patient");
                    TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đợi admin duyệt tài khoản.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập email và mật khẩu");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại trong hệ thống");
                return View();
            }

            if (!user.IsApproved)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn chưa được duyệt. Vui lòng đợi admin duyệt tài khoản.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu không đúng");
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Redirect("/");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập email");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại trong hệ thống");
                return View();
            }

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Đặt lại mật khẩu", $"Nhấn vào <a href='{resetLink}'>đây</a> để đặt lại mật khẩu của bạn.");

                TempData["SuccessMessage"] = "Vui lòng kiểm tra email để đặt lại mật khẩu";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi khi gửi email: " + ex.Message);
                return View();
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string token, string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ thông tin");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại trong hệ thống");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
    }
}