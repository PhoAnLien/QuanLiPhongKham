using DoAnLTW.Models;
using System.Collections.Generic;

namespace DoAnLTW.ViewModels
{
    public class ManageWeeklyScheduleViewModel
    {
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public List<DoctorWeeklySchedule> Schedules { get; set; }

        public ManageWeeklyScheduleViewModel()
        {
            Schedules = new List<DoctorWeeklySchedule>();
        }
    }
} 