using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DoAnLTW.Data;
using DoAnLTW.Models;

namespace DoAnLTW.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByUserAsync(string userId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == userId)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorAsync(string doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<ApplicationUser>> GetAvailableDoctorsAsync()
        {
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            return doctors.Where(d => d.IsApproved).ToList();
        }

        public async Task CreateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }
    }
}