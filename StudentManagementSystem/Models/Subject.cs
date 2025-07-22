using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string SubjectCode { get; set; } = string.Empty; // Mã môn học (VD: CS101)

        [Required]
        [StringLength(200)]
        public string SubjectName { get; set; } = string.Empty; // Tên môn học

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty; // Mô tả môn học

        [Required]
        public int Credits { get; set; } = 3; // Số tín chỉ

        [Required]
        public string TeacherUserId { get; set; } = string.Empty; // Giảng viên phụ trách

        [ForeignKey("TeacherUserId")]
        public virtual ApplicationUser Teacher { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedBy { get; set; } = null!;

        // Navigation properties
        public virtual ICollection<Class> Classes { get; set; } = new List<Class>(); // Main subject classes
        public virtual ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>(); // Additional subject classes
        public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
        public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

        // Helper property to get all classes (main + additional)
        public virtual IEnumerable<Class> AllClasses => Classes.Concat(ClassSubjects.Select(cs => cs.Class));
    }
}
