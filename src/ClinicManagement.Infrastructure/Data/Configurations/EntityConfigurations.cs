using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patient");
        builder.HasKey(p => p.PatientId);
        builder.Property(p => p.PatientId).HasColumnName("PatientID").ValueGeneratedOnAdd();
        builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Email).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Password).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Phone).HasMaxLength(20);
        builder.Property(p => p.Address).HasMaxLength(200);
        builder.Property(p => p.Gender).HasMaxLength(10);
        builder.Property(p => p.IsActive).HasDefaultValue(true);
        builder.Property(p => p.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
    }
}

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctor");
        builder.HasKey(d => d.DoctorId);
        builder.Property(d => d.DoctorId).HasColumnName("DoctorID").ValueGeneratedOnAdd();
        builder.Property(d => d.Name).HasMaxLength(50).IsRequired();
        builder.Property(d => d.Email).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Password).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Phone).HasMaxLength(20);
        builder.Property(d => d.Address).HasMaxLength(200);
        builder.Property(d => d.Gender).HasMaxLength(10);
        builder.Property(d => d.Specialization).HasMaxLength(100);
        builder.Property(d => d.Qualification).HasMaxLength(200);
        builder.Property(d => d.ChargesPerVisit).HasColumnType("decimal(10,2)");
        builder.Property(d => d.Salary).HasColumnType("decimal(10,2)");
        builder.Property(d => d.IsActive).HasDefaultValue(true);
        builder.Property(d => d.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(d => d.Department)
            .WithMany(dept => dept.Doctors)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");
        builder.HasKey(d => d.DepartmentId);
        builder.Property(d => d.DepartmentId).HasColumnName("DeptNo").ValueGeneratedOnAdd();
        builder.Property(d => d.DeptName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(500);
        builder.Property(d => d.IsActive).HasDefaultValue(true);
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointment");
        builder.HasKey(a => a.AppointmentId);
        builder.Property(a => a.AppointmentId).HasColumnName("AppointmentID").ValueGeneratedOnAdd();
        builder.Property(a => a.Status).HasMaxLength(20).HasDefaultValue("Pending");
        builder.Property(a => a.Disease).HasMaxLength(100);
        builder.Property(a => a.Progress).HasMaxLength(200);
        builder.Property(a => a.Prescription).HasMaxLength(500);
        builder.Property(a => a.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.TimeSlot)
            .WithMany(t => t.Appointments)
            .HasForeignKey(a => a.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("TimeSlot");
        builder.HasKey(t => t.TimeSlotId);
        builder.Property(t => t.TimeSlotId).HasColumnName("SlotID").ValueGeneratedOnAdd();
        builder.Property(t => t.Timings).HasMaxLength(50).IsRequired();
        builder.Property(t => t.IsAvailable).HasDefaultValue(true);

        builder.HasOne(t => t.Doctor)
            .WithMany(d => d.TimeSlots)
            .HasForeignKey(t => t.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.ToTable("Bill");
        builder.HasKey(b => b.BillId);
        builder.Property(b => b.BillId).HasColumnName("BillID").ValueGeneratedOnAdd();
        builder.Property(b => b.Amount).HasColumnType("decimal(10,2)");
        builder.Property(b => b.BillDate).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(b => b.Appointment)
            .WithOne(a => a.Bill)
            .HasForeignKey<Bill>(b => b.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Patient)
            .WithMany(p => p.Bills)
            .HasForeignKey(b => b.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("OtherStaff");
        builder.HasKey(s => s.StaffId);
        builder.Property(s => s.StaffId).HasColumnName("StaffID").ValueGeneratedOnAdd();
        builder.Property(s => s.Name).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.Address).HasMaxLength(200);
        builder.Property(s => s.Gender).HasMaxLength(10);
        builder.Property(s => s.Designation).HasMaxLength(100);
        builder.Property(s => s.Qualification).HasMaxLength(200);
        builder.Property(s => s.Salary).HasColumnType("decimal(10,2)");
        builder.Property(s => s.IsActive).HasDefaultValue(true);
        builder.Property(s => s.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
    }
}
