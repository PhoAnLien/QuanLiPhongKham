using System.ComponentModel.DataAnnotations;

namespace DoAnLTW.ViewModels
{
    public class CreateDoctorViewModel
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

        [Required(ErrorMessage = "Chuyên khoa là bắt buộc")]
        public string Specialization { get; set; }

        [Required(ErrorMessage = "Học vấn là bắt buộc")]
        public string Education { get; set; }

        [Required(ErrorMessage = "Kinh nghiệm là bắt buộc")]
        public string Experience { get; set; }

        [Required(ErrorMessage = "Giờ làm việc là bắt buộc")]
        public string WorkingHours { get; set; }
    }
}