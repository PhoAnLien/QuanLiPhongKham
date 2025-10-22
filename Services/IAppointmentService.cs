using DoAnLTW.Models;

namespace DoAnLTW.Services
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<List<Appointment>> GetAppointmentsByUserAsync(string userId);
        Task<List<Appointment>> GetAppointmentsByDoctorAsync(string doctorId);
        Task<Appointment> GetAppointmentByIdAsync(int id);
        Task<List<ApplicationUser>> GetAvailableDoctorsAsync();
        Task CreateAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
    }
}