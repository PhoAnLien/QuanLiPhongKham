using DoAnLTW.Models;
using System.Collections.Generic;

namespace DoAnLTW.ViewModels
{
    public class PatientLookupViewModel
    {
        public string CurrentPatientSearch { get; set; }
        public List<PatientSearchResult> Results { get; set; }
    }

    public class PatientSearchResult
    {
        public string PatientId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
} 