using DoAnLTW.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnLTW.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<UserViewModel> AllUsers { get; set; }
        public List<Appointment> Appointments { get; set; }
        public IList<ApplicationUser> Doctors { get; set; }
        public List<Appointment> PendingAppointments { get; set; }
        public List<Appointment> ConfirmedAppointments { get; set; }
        public List<DoctorWeeklySchedule> WeeklySchedules { get; set; }
        public List<DoctorLeave> Leaves { get; set; }
        public ILookup<string, DoctorWeeklySchedule> SchedulesByDoctor { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public bool IsApproved { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string Specialization { get; set; } // Thêm vào để dùng ở bảng bác sĩ
        public string WorkingHours { get; set; } // Thêm vào để dùng ở bảng bác sĩ
    }
}