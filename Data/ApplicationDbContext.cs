using DoctorConnect.Models;
using Microsoft.AspNetCore.Identity;
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
        public DbSet<Relative> Relatives { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<DoctorTask> DoctorTasks { get; set; }
        public DbSet<TaskBullet> TaskBullets { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<PermissionAuditLog> PermissionAuditLogs { get; set; }

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

            builder.Entity<Relative>()
                .HasOne(r => r.Patient)
                .WithMany(p => p.Relatives)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Doctor>()
                .HasMany(d => d.Clinics)
                .WithMany(c => c.Doctors)
                .UsingEntity(j => j.ToTable("DoctorClinics"));

            builder.Entity<DoctorAvailability>()
                .HasMany(da => da.BreakTimes)
                .WithOne(bt => bt.DoctorAvailability)
                .HasForeignKey(bt => bt.DoctorAvailabilityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Service>()
                .HasOne(s => s.Doctor)
                .WithMany(d => d.Services)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Service>()
                .HasOne(s => s.Clinic)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DoctorTask>()
                .HasOne(t => t.Doctor)
                .WithMany(d => d.Tasks)
                .HasForeignKey(t => t.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DoctorTask>()
                .HasMany(t => t.Bullets)
                .WithOne(b => b.DoctorTask)
                .HasForeignKey(b => b.DoctorTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            builder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.CreatedByUser)
                .WithMany()
                .HasForeignKey(rp => rp.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<PermissionAuditLog>()
                .HasOne(p => p.ChangedByUser)
                .WithMany(u => u.PermissionAuditLogs)
                .HasForeignKey(p => p.ChangedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<PermissionAuditLog>()
                .HasIndex(p => p.ChangedAt);
        }
    }
}
