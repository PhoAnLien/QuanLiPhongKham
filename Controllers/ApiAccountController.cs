using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DoAnLTW.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace DoAnLTW.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountApiController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState.Values.SelectMany(v => v.Errors) });

            // Kiểm tra email đã tồn tại chưa
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email đã được sử dụng" });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                IsApproved = true,
                Address = model.Address ?? "",
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender ?? "Không xác định",
                ProfilePicture = "",
                Specialization = "",
                LicenseNumber = "",
                Education = "",
                Experience = "",
                WorkingHours = "",
                MaxAppointmentsPerDay = 20,
                MedicalHistory = "",
                BloodType = "",
                Allergies = "",
                IsVerified = false,
                VerifiedAt = null,
                VerifiedBy = "",
                Role = "Patient"
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Patient");
                return Ok(new
                {
                    message = "Đăng ký thành công. Vui lòng đợi admin duyệt tài khoản.",
                    user = new { user.Id, user.Email, user.FullName }
                });
            }

            return BadRequest(new
            {
                message = "Đăng ký thất bại",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { message = "Email không tồn tại trong hệ thống" });

            if (!user.IsApproved)
                return BadRequest(new { message = "Tài khoản của bạn chưa được duyệt. Vui lòng đợi admin duyệt tài khoản." });

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new
                {
                    message = "Đăng nhập thành công",
                    user = new
                    {
                        user.Id,
                        user.Email,
                        user.FullName,
                        user.PhoneNumber,
                        user.IsApproved,
                        Roles = roles
                    }
                });
            }
            else if (result.IsLockedOut)
            {
                return BadRequest(new { message = "Tài khoản của bạn đã bị khóa do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau." });
            }

            return BadRequest(new { message = "Mật khẩu không đúng" });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Đăng xuất thành công" });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng" });

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new
            {
                user.Id,
                user.Email,
                user.FullName,
                user.PhoneNumber,
                user.IsApproved,
                Roles = roles
            });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng" });

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { message = "Đổi mật khẩu thành công" });
            }

            return BadRequest(new
            {
                message = "Đổi mật khẩu thất bại",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { message = "Email không tồn tại trong hệ thống" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // TODO: Gửi email chứa token để reset password
            // Trong môi trường thực tế, bạn cần implement phần gửi email

            return Ok(new { message = "Vui lòng kiểm tra email để đặt lại mật khẩu" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { message = "Email không tồn tại trong hệ thống" });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { message = "Đặt lại mật khẩu thành công" });
            }

            return BadRequest(new
            {
                message = "Đặt lại mật khẩu thất bại",
                errors = result.Errors.Select(e => e.Description)
            });
        }
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ hoa, một chữ thường và một số")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        public string Gender { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ hoa, một chữ thường và một số")]
        public string NewPassword { get; set; }
    }

    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token là bắt buộc")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ hoa, một chữ thường và một số")]
        public string NewPassword { get; set; }
    }
}