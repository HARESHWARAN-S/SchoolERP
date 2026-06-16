using Microsoft.EntityFrameworkCore;
using SchoolERP.Models.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;        
using Microsoft.EntityFrameworkCore.Storage.ValueConversion; 
using SchoolERP.Models;

namespace SchoolERP.Contexts
{
    public class SchoolERPDbContext : DbContext
    {
        public SchoolERPDbContext(DbContextOptions<SchoolERPDbContext> options) : base(options) { }

        public DbSet<Login> Logins { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<TeacherAttendance> TeacherAttendances { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<StudentAttendance> StudentAttendances { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<AdminAttendance> AdminAttendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d));

            var dateOnlyComparer = new ValueComparer<DateOnly>(
                (x, y) => x == y,
                d => d.GetHashCode(),
                d => d);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateOnly))
                    {
                        property.SetValueConverter(dateOnlyConverter);
                        property.SetValueComparer(dateOnlyComparer);
                    }
                    if (property.ClrType == typeof(DateOnly?))
                    {
                        var nullableDateOnlyConverter = new ValueConverter<DateOnly?, DateTime?>(
                            d => d == null ? null : d.Value.ToDateTime(TimeOnly.MinValue),
                            d => d == null ? null : DateOnly.FromDateTime(d.Value));
                        property.SetValueConverter(nullableDateOnlyConverter);
                    }
                }
            }

            modelBuilder.Entity<StudentClass>()
                .HasKey(sc => new { sc.Class, sc.Sec });
            modelBuilder.Entity<TeacherAttendance>()
                .HasKey(ta => new { ta.TeacherId, ta.Date });
            modelBuilder.Entity<Subject>()
                .HasKey(s => new { s.Class, s.Sec, s.SubjectName });
            modelBuilder.Entity<StudentAttendance>()
                .HasKey(sa => new { sa.AdmnNo, sa.Date });
            modelBuilder.Entity<AdminAttendance>()
                .HasKey(aa => new { aa.AdminId, aa.Date });

            modelBuilder.Entity<Admin>()
                .Property(a => a.AttendancePercentage).HasPrecision(5, 2);
            modelBuilder.Entity<Teacher>()
                .Property(t => t.AttendancePercentage).HasPrecision(5, 2);
            modelBuilder.Entity<Student>()
                .Property(s => s.AttendancePercentage).HasPrecision(5, 2);
            modelBuilder.Entity<Fee>()
                .Property(f => f.Amount).HasPrecision(10, 2);
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount).HasPrecision(10, 2);
            modelBuilder.Entity<Mark>()
                .Property(m => m.MarksObtained).HasPrecision(6, 2);
            modelBuilder.Entity<Mark>()
                .Property(m => m.TotalMarks).HasPrecision(6, 2);

            modelBuilder.Entity<Login>()
                .Property(l => l.Role).HasConversion<string>();
            modelBuilder.Entity<Login>()
                .Property(l => l.Status).HasConversion<string>();
            modelBuilder.Entity<Admin>()
                .Property(a => a.Gender).HasConversion<string>();
            modelBuilder.Entity<Admin>()
                .Property(a => a.BloodGrp).HasConversion<string>();
            modelBuilder.Entity<Teacher>()
                .Property(t => t.Gender).HasConversion<string>();
            modelBuilder.Entity<Teacher>()
                .Property(t => t.BloodGrp).HasConversion<string>();
            modelBuilder.Entity<Student>()
                .Property(s => s.Gender).HasConversion<string>();
            modelBuilder.Entity<Student>()
                .Property(s => s.BloodGrp).HasConversion<string>();
            modelBuilder.Entity<TeacherAttendance>()
                .Property(ta => ta.Status).HasConversion<string>();
            modelBuilder.Entity<Fee>()
                .Property(f => f.Status).HasConversion<string>();
            modelBuilder.Entity<StudentAttendance>()
                .Property(sa => sa.Status).HasConversion<string>();
            modelBuilder.Entity<AdminAttendance>()
                .Property(aa => aa.Status).HasConversion<string>();

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.Login)
                .WithOne(l => l.Admin)
                .HasForeignKey<Admin>(a => a.AdminId);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Login)
                .WithOne(l => l.Teacher)
                .HasForeignKey<Teacher>(t => t.TeacherId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Login)
                .WithOne(l => l.Student)
                .HasForeignKey<Student>(s => s.AdmnNo);

            modelBuilder.Entity<StudentClass>()
                .HasOne(sc => sc.ClassTeacher)
                .WithOne()
                .HasForeignKey<StudentClass>(sc => sc.ClassTeacherId);
            
            modelBuilder.Entity<Fee>()
                .HasOne(f => f.Student)
                .WithMany()
                .HasForeignKey(f => f.AdmnNo);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Fee)
                .WithMany()
                .HasForeignKey(p => p.FeeId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Student)
                .WithMany()
                .HasForeignKey(p => p.AdmnNo);

            modelBuilder.Entity<StudentClass>()
                .Property(sc => sc.ClassTeacherId)
                .IsRequired();

            modelBuilder.Entity<TeacherAttendance>()
                .HasOne(ta => ta.Teacher)
                .WithMany()
                .HasForeignKey(ta => ta.TeacherId);

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.StudentClass)
                .WithMany()
                .HasForeignKey(s => new { s.Class, s.Sec });

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Teacher)
                .WithMany()
                .HasForeignKey(s => s.TeacherId);

            modelBuilder.Entity<Homework>()
                .HasOne(h => h.StudentClass)
                .WithMany()
                .HasForeignKey(h => new { h.Class, h.Sec });

            modelBuilder.Entity<StudentAttendance>()
                .HasOne(sa => sa.Student)
                .WithMany()
                .HasForeignKey(sa => sa.AdmnNo);

            modelBuilder.Entity<Mark>()
                .HasOne(m => m.Student)
                .WithMany()
                .HasForeignKey(m => m.AdmnNo);

            modelBuilder.Entity<AdminAttendance>()
                .HasOne(aa => aa.Admin)
                .WithMany()
                .HasForeignKey(aa => aa.AdminId);
        }
    }
}