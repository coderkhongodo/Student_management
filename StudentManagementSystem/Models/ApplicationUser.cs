using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        [StringLength(20)]
        public string StudentId { get; set; } = string.Empty; // For students

        [StringLength(20)]
        public string EmployeeId { get; set; } = string.Empty; // For teachers and admin

        public decimal Salary { get; set; } // For teachers and admin

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        public virtual ICollection<Exam> CreatedExams { get; set; } = new List<Exam>();
        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();
        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public virtual ICollection<Class> TeachingClasses { get; set; } = new List<Class>();
        public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();
        public virtual ICollection<Subject> TeachingSubjects { get; set; } = new List<Subject>();
        public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
        public virtual ICollection<AttendanceSession> CreatedAttendanceSessions { get; set; } = new List<AttendanceSession>();
        public virtual ICollection<Attendance> StudentAttendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Attendance> MarkedAttendances { get; set; } = new List<Attendance>();

        // Helper methods for students
        public decimal CalculateGPA()
        {
            if (!StudentGrades.Any()) return 0.0m;

            var totalPoints = StudentGrades.Sum(sg => sg.GetGradePoint() * sg.Subject.Credits);
            var totalCredits = StudentGrades.Sum(sg => sg.Subject.Credits);

            return totalCredits > 0 ? totalPoints / totalCredits : 0.0m;
        }

        public int GetFailedSubjectsCount()
        {
            return StudentGrades.Count(sg => sg.IsFailed);
        }

        public bool HasAcademicWarning()
        {
            return GetFailedSubjectsCount() >= 2;
        }
    }
}
