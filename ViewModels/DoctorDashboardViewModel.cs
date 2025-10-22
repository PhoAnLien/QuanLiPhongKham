using DoAnLTW.Models;
using System.Collections.Generic;

namespace DoAnLTW.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public ApplicationUser DoctorInfo { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
} 