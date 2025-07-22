using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class Class
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên lớp học là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên lớp học không được vượt quá 100 ký tự")]
        public string ClassName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giảng viên là bắt buộc")]
        public string TeacherUserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Học kỳ là bắt buộc")]
        [StringLength(50)]
        public string Semester { get; set; } = string.Empty;

        [Required(ErrorMessage = "Năm học là bắt buộc")]
        [Range(2020, 2030, ErrorMessage = "Năm học phải từ 2020 đến 2030")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Số lượng sinh viên tối đa là bắt buộc")]
        [Range(1, 100, ErrorMessage = "Số lượng sinh viên phải từ 1 đến 100")]
        public int MaxStudents { get; set; } = 50;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("TeacherUserId")]
        public virtual ApplicationUser TeacherUser { get; set; } = null!;

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedByUser { get; set; } = null!;

        // Many-to-many relationship với Subject thông qua ClassSubject
        public virtual ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();

        // One-to-many relationships
        public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();
        public virtual ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();
        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();

        // Helper properties
        public int CurrentStudentCount => ClassStudents?.Count(cs => cs.IsActive) ?? 0;
        public bool IsFull => CurrentStudentCount >= MaxStudents;
    }

    public class ClassStudent
    {
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; } = null!;

        [Required]
        public string StudentUserId { get; set; } = string.Empty;

        [ForeignKey("StudentUserId")]
        public virtual ApplicationUser Student { get; set; } = null!;

        public DateTime EnrolledAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedBy { get; set; } = null!;
    }
}
