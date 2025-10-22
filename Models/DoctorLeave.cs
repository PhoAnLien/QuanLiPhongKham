using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnLTW.Models
{
    public class DoctorLeave
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public ApplicationUser Doctor { get; set; }

        [Required]
        public DateTime LeaveDate { get; set; } // Ngày nghỉ

        public string? Reason { get; set; } // Lý do nghỉ
    }
} 