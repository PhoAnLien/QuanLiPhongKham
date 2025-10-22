using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnLTW.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PatientId { get; set; }

        //[Required]
        public string? DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string Reason { get; set; }

        public AppointmentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public string? Prescription { get; set; } // Toa thuốc
        public string? Diagnosis { get; set; } // Bệnh/chẩn đoán
        public string? DoctorNote { get; set; } // Lời dặn của bác sĩ

        // Navigation properties
        [ForeignKey("PatientId")]
        public ApplicationUser Patient { get; set; }

        [ForeignKey("DoctorId")]
        public ApplicationUser Doctor { get; set; }
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}