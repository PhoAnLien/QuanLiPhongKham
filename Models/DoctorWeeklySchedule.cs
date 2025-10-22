using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnLTW.Models
{
    public enum ShiftType
    {
        [Display(Name = "Nghỉ")]
        None = 0,
        [Display(Name = "Ca Sáng (08:00 - 12:00)")]
        Morning = 1,
        [Display(Name = "Ca Chiều (13:00 - 17:00)")]
        Afternoon = 2,
        [Display(Name = "Cả ngày (08:00 - 17:00)")]
        FullDay = 3
    }

    public class DoctorWeeklySchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public ApplicationUser Doctor { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; } // Thứ trong tuần

        public bool IsWorking { get; set; } = false; // Có làm việc không?

        public ShiftType Shift { get; set; } = ShiftType.None;

        public string? Note { get; set; } // Ghi chú (VD: Về sớm, đi trễ)
    }
} 