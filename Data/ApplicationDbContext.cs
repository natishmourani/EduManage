using Microsoft.EntityFrameworkCore;
using EduManage.Models;

namespace EduManage.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Remark> Remarks { get; set; }
        public DbSet<UpdateRequest> UpdateRequests { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<StudyMaterial> StudyMaterials { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        public DbSet<StudentDashboardOverview> StudentDashboardOverviews { get; set; }
        public DbSet<AssignmentProgress> AssignmentProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentDashboardOverview>()
                .ToView("v_StudentDashboardOverview")
                .HasKey(x => x.StudentId);

            modelBuilder.Entity<AssignmentProgress>()
                .ToView("v_AssignmentProgress")
                .HasKey(x => x.AssignmentId);
        }
    }
}