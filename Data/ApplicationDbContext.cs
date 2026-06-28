using DoctorConnect.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoctorConnect.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Settings> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Appointment>()
               .HasOne(a => a.Doctor)
               .WithMany(u => u.Appointments)
               .HasForeignKey(a => a.DoctorId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Doctor>()
                .HasMany(d => d.Clinics)
                .WithMany(c => c.Doctors)
                .UsingEntity(j => j.ToTable("DoctorClinics"));

            builder.Entity<DoctorAvailability>()
                .HasMany(da => da.BreakTimes)
                .WithOne(bt => bt.DoctorAvailability)
                .HasForeignKey(bt => bt.DoctorAvailabilityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
