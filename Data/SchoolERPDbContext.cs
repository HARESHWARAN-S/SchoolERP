using Microsoft.EntityFrameworkCore;
using SchoolERP.Models.Entities;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Composite Primary Key
            modelBuilder.Entity<StudentClass>()
                .HasKey(sc => new { sc.Class, sc.Sec });
            modelBuilder.Entity<TeacherAttendance>()
                .HasKey(ta => new { ta.TeacherId, ta.Date });
            modelBuilder.Entity<Subject>()
                .HasKey(s => new { s.Class, s.Sec, s.SubjectName });

            // Decimal Precision
            modelBuilder.Entity<Admin>()
                .Property(a => a.AttendancePercentage).HasPrecision(5, 2);
            modelBuilder.Entity<Teacher>()
                .Property(t => t.AttendancePercentage).HasPrecision(5, 2);
            modelBuilder.Entity<Student>()
                .Property(s => s.AttendancePercentage).HasPrecision(5, 2);

            // Enums as strings
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

            // Relationships
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

            // Enum conversions if needed
            modelBuilder.Entity<StudentClass>()
                .Property(sc => sc.ClassTeacherId)
                .IsRequired();

            modelBuilder.Entity<TeacherAttendance>()
                .HasOne(ta => ta.Teacher)
                .WithMany()
                .HasForeignKey(ta => ta.TeacherId);
            
            // Subject -> StudentClass
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.StudentClass)
                .WithMany()
                .HasForeignKey(s => new { s.Class, s.Sec });

            // Subject -> Teacher
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Teacher)
                .WithMany()
                .HasForeignKey(s => s.TeacherId);

            modelBuilder.Entity<Homework>()
            .HasOne(h => h.StudentClass)
            .WithMany()
            .HasForeignKey(h => new { h.Class, h.Sec });
        }
    }
}