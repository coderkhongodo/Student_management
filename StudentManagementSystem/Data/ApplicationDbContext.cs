using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamSchedule> ExamSchedules { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<StudentGrade> StudentGrades { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassStudent> ClassStudents { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ClassSubject> ClassSubjects { get; set; }
        public DbSet<AttendanceSession> AttendanceSessions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Exam>()
                .HasOne(e => e.CreatedBy)
                .WithMany(u => u.CreatedExams)
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamSchedule>()
                .HasOne(es => es.Exam)
                .WithMany(e => e.ExamSchedules)
                .HasForeignKey(es => es.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExamSchedule>()
                .HasOne(es => es.CreatedBy)
                .WithMany(u => u.ExamSchedules)
                .HasForeignKey(es => es.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Submission>()
                .HasOne(s => s.ExamSchedule)
                .WithMany(es => es.Submissions)
                .HasForeignKey(s => s.ExamScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Grade>()
                .HasOne(g => g.Submission)
                .WithOne(s => s.Grade)
                .HasForeignKey<Grade>(g => g.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(u => u.Grades)
                .HasForeignKey(g => g.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Grade>()
                .HasOne(g => g.GradedBy)
                .WithMany()
                .HasForeignKey(g => g.GradedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision
            builder.Entity<ApplicationUser>()
                .Property(u => u.Salary)
                .HasPrecision(18, 2);

            builder.Entity<Grade>()
                .Property(g => g.Score)
                .HasPrecision(5, 2);

            builder.Entity<Grade>()
                .Property(g => g.MaxScore)
                .HasPrecision(5, 2);

            // Configure StudentGrade relationships
            builder.Entity<StudentGrade>()
                .HasOne(sg => sg.Student)
                .WithMany()
                .HasForeignKey(sg => sg.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentGrade>()
                .HasOne(sg => sg.CreatedBy)
                .WithMany()
                .HasForeignKey(sg => sg.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // Allow null for system-generated grades

            // Configure decimal precision for StudentGrade
            builder.Entity<StudentGrade>()
                .Property(sg => sg.AttendanceScore)
                .HasPrecision(5, 2);

            builder.Entity<StudentGrade>()
                .Property(sg => sg.MidtermScore)
                .HasPrecision(5, 2);

            builder.Entity<StudentGrade>()
                .Property(sg => sg.FinalScore)
                .HasPrecision(5, 2);

            // Configure Submission file fields as nullable
            builder.Entity<Submission>()
                .Property(s => s.AnswerFilePath)
                .IsRequired(false);

            builder.Entity<Submission>()
                .Property(s => s.AnswerFileName)
                .IsRequired(false);

            builder.Entity<Submission>()
                .Property(s => s.AnswerFileType)
                .IsRequired(false);

            // Add unique constraints
            builder.Entity<Submission>()
                .HasIndex(s => new { s.ExamScheduleId, s.StudentUserId })
                .IsUnique();

            builder.Entity<StudentGrade>()
                .HasIndex(sg => new { sg.StudentUserId, sg.SubjectId, sg.Semester, sg.Year })
                .IsUnique();

            // Configure Class relationships
            builder.Entity<Class>()
                .HasOne(c => c.TeacherUser)
                .WithMany(u => u.TeachingClasses)
                .HasForeignKey(c => c.TeacherUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Class>()
                .HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ClassStudent relationships
            builder.Entity<ClassStudent>()
                .HasOne(cs => cs.Class)
                .WithMany(c => c.ClassStudents)
                .HasForeignKey(cs => cs.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ClassStudent>()
                .HasOne(cs => cs.Student)
                .WithMany(u => u.ClassStudents)
                .HasForeignKey(cs => cs.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ClassStudent>()
                .HasOne(cs => cs.CreatedBy)
                .WithMany()
                .HasForeignKey(cs => cs.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add unique constraint for ClassStudent
            builder.Entity<ClassStudent>()
                .HasIndex(cs => new { cs.ClassId, cs.StudentUserId })
                .IsUnique();

            // Configure Subject relationships
            builder.Entity<Subject>()
                .HasOne(s => s.Teacher)
                .WithMany(u => u.TeachingSubjects)
                .HasForeignKey(s => s.TeacherUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Subject>()
                .HasOne(s => s.CreatedBy)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Remove old Class-Subject direct relationship since we use ClassSubject now

            // Configure ClassSubject relationships (additional subjects)
            builder.Entity<ClassSubject>()
                .HasOne(cs => cs.Class)
                .WithMany(c => c.ClassSubjects)
                .HasForeignKey(cs => cs.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ClassSubject>()
                .HasOne(cs => cs.Subject)
                .WithMany(s => s.ClassSubjects)
                .HasForeignKey(cs => cs.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ClassSubject>()
                .HasOne(cs => cs.CreatedByUser)
                .WithMany()
                .HasForeignKey(cs => cs.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add unique constraint for ClassSubject (one record per class per subject)
            builder.Entity<ClassSubject>()
                .HasIndex(cs => new { cs.ClassId, cs.SubjectId })
                .IsUnique();

            // Configure StudentGrade-Subject relationship
            builder.Entity<StudentGrade>()
                .HasOne(sg => sg.Subject)
                .WithMany(s => s.StudentGrades)
                .HasForeignKey(sg => sg.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Exam-Subject relationship
            builder.Entity<Exam>()
                .HasOne(e => e.Subject)
                .WithMany(s => s.Exams)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add unique constraint for Subject code
            builder.Entity<Subject>()
                .HasIndex(s => s.SubjectCode)
                .IsUnique();

            // Configure AttendanceSession relationships
            builder.Entity<AttendanceSession>()
                .HasOne(ats => ats.Class)
                .WithMany(c => c.AttendanceSessions)
                .HasForeignKey(ats => ats.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AttendanceSession>()
                .HasOne(ats => ats.CreatedBy)
                .WithMany(u => u.CreatedAttendanceSessions)
                .HasForeignKey(ats => ats.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Attendance relationships
            builder.Entity<Attendance>()
                .HasOne(a => a.AttendanceSession)
                .WithMany(ats => ats.Attendances)
                .HasForeignKey(a => a.AttendanceSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(u => u.StudentAttendances)
                .HasForeignKey(a => a.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Attendance>()
                .HasOne(a => a.MarkedBy)
                .WithMany(u => u.MarkedAttendances)
                .HasForeignKey(a => a.MarkedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add unique constraint for Attendance (one record per student per session)
            builder.Entity<Attendance>()
                .HasIndex(a => new { a.AttendanceSessionId, a.StudentUserId })
                .IsUnique();
        }
    }
}
