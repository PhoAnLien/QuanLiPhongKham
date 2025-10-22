using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DoAnLTW.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public string? Gender { get; set; }

        public string? ProfilePicture { get; set; }

        // For doctors
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public bool IsApproved { get; set; }
        public string? WorkingHours { get; set; }
        public int MaxAppointmentsPerDay { get; set; } = 20;

        // For patients
        public string? MedicalHistory { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? VerifiedBy { get; set; }

        public string? Description { get; set; } // Thêm trường mô tả bản thân

        public string? Role { get; set; }

        // Navigation properties
        public ICollection<Appointment>? AppointmentsAsPatient { get; set; }
    }
}