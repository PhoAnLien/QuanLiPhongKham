using DoAnLTW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTW.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorWeeklySchedule> DoctorWeeklySchedules { get; set; }
        public DbSet<DoctorLeave> DoctorLeaves { get; set; }
        
        // public DbSet<Specialization> Specializations { get; set; } // Bỏ đi

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Appointment>(entity =>
            {
                // Cấu hình mối quan hệ
                entity.HasOne(a => a.Patient)
                    .WithMany(u => u.AppointmentsAsPatient)
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                    .WithMany()
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Cấu hình lưu trạng thái Enum dưới dạng string
                entity.Property(e => e.Status)
                    .HasConversion<string>();
            });
        }
    }
}