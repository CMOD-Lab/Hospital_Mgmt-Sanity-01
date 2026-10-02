using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Data;

/// <summary>Entity Framework Core database context for the Clinic Management System.</summary>
public class ClinicDbContext : DbContext
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

    public DbSet<LoginTable> LoginTable => Set<LoginTable>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<OtherStaff> OtherStaff => Set<OtherStaff>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // LoginTable
        modelBuilder.Entity<LoginTable>(e =>
        {
            e.ToTable("LoginTable");
            e.HasKey(x => x.LoginId);
            e.Property(x => x.LoginId).HasColumnName("LoginID").ValueGeneratedOnAdd();
            e.Property(x => x.Password).HasMaxLength(20).IsRequired();
            e.Property(x => x.Email).HasMaxLength(30).IsRequired();
            e.Property(x => x.Type).HasConversion<int>().IsRequired();
        });

        // Patient
        modelBuilder.Entity<Patient>(e =>
        {
            e.ToTable("Patient");
            e.HasKey(x => x.PatientId);
            e.Property(x => x.PatientId).HasColumnName("PatientID").ValueGeneratedNever();
            e.Property(x => x.Name).HasMaxLength(30).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(11).IsFixedLength();
            e.Property(x => x.Address).HasMaxLength(40);
            e.Property(x => x.BirthDate).HasColumnType("date").IsRequired();
            e.Property(x => x.Gender).HasMaxLength(1).IsFixedLength().IsRequired();

            e.HasOne(x => x.Login)
             .WithOne(x => x.Patient)
             .HasForeignKey<Patient>(x => x.PatientId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Department
        modelBuilder.Entity<Department>(e =>
        {
            e.ToTable("Department");
            e.HasKey(x => x.DeptNo);
            e.Property(x => x.DeptNo).ValueGeneratedNever();
            e.Property(x => x.DeptName).HasMaxLength(30).IsRequired();
            e.HasIndex(x => x.DeptName).IsUnique();
            e.Property(x => x.Description).HasMaxLength(1000);
        });

        // Doctor
        modelBuilder.Entity<Doctor>(e =>
        {
            e.ToTable("Doctor");
            e.HasKey(x => x.DoctorId);
            e.Property(x => x.DoctorId).HasColumnName("DoctorID").ValueGeneratedNever();
            e.Property(x => x.Name).HasMaxLength(30).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(11).IsFixedLength();
            e.Property(x => x.Address).HasMaxLength(40);
            e.Property(x => x.BirthDate).HasColumnType("date").IsRequired();
            e.Property(x => x.Gender).HasMaxLength(1).IsFixedLength().IsRequired();
            e.Property(x => x.ChargesPerVisit).HasColumnName("Charges_Per_Visit").IsRequired();
            e.Property(x => x.MonthlySalary).HasColumnName("MonthlySalary");
            e.Property(x => x.ReputeIndex).HasColumnName("ReputeIndex");
            e.Property(x => x.PatientsTreated).HasColumnName("Patients_Treated").HasDefaultValue(0).IsRequired();
            e.Property(x => x.Qualification).HasMaxLength(100).IsRequired();
            e.Property(x => x.Specialization).HasMaxLength(100);
            e.Property(x => x.WorkExperience).HasColumnName("Work_Experience");
            e.Property(x => x.Status).HasConversion<int>().IsRequired();

            e.HasOne(x => x.Department)
             .WithMany(x => x.Doctors)
             .HasForeignKey(x => x.DeptNo)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Login)
             .WithOne(x => x.Doctor)
             .HasForeignKey<Doctor>(x => x.DoctorId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // OtherStaff
        modelBuilder.Entity<OtherStaff>(e =>
        {
            e.ToTable("OtherStaff");
            e.HasKey(x => x.StaffId);
            e.Property(x => x.StaffId).HasColumnName("StaffID").ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasMaxLength(30).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(11).IsFixedLength();
            e.Property(x => x.Address).HasMaxLength(30);
            e.Property(x => x.Designation).HasMaxLength(15).IsRequired();
            e.Property(x => x.Gender).HasMaxLength(1).IsFixedLength().IsRequired();
            e.Property(x => x.BirthDate).HasColumnType("date");
            e.Property(x => x.HighestQualification).HasColumnName("Highest_Qualification").HasMaxLength(50);
            e.Property(x => x.Salary);
        });

        // Appointment
        modelBuilder.Entity<Appointment>(e =>
        {
            e.ToTable("Appointment");
            e.HasKey(x => x.AppointId);
            e.Property(x => x.AppointId).HasColumnName("AppointID").ValueGeneratedOnAdd();
            e.Property(x => x.AppointmentStatus).HasColumnName("Appointment_Status").HasConversion<int>();
            e.Property(x => x.BillAmount).HasColumnName("Bill_Amount");
            e.Property(x => x.BillStatus).HasColumnName("Bill_Status").HasConversion<string>().HasMaxLength(10);
            e.Property(x => x.DoctorNotification).HasConversion<int>();
            e.Property(x => x.PatientNotification).HasConversion<int>();
            e.Property(x => x.FeedbackStatus).HasConversion<int>();
            e.Property(x => x.Disease).HasMaxLength(100);
            e.Property(x => x.Progress).HasMaxLength(100);
            e.Property(x => x.Prescription).HasMaxLength(100);

            e.HasOne(x => x.Doctor)
             .WithMany(x => x.Appointments)
             .HasForeignKey(x => x.DoctorId)
             .OnDelete(DeleteBehavior.SetNull);

            e.HasOne(x => x.Patient)
             .WithMany(x => x.Appointments)
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
