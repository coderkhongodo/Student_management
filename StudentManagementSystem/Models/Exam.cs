using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class Exam
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedBy { get; set; } = null!;

        [Required]
        public int SubjectId { get; set; } // Môn học

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;

        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty; // Path to uploaded exam file

        [StringLength(100)]
        public string FileName { get; set; } = string.Empty; // Original file name

        [StringLength(50)]
        public string FileType { get; set; } = string.Empty; // doc, docx, jpg, png, etc.

        public long FileSize { get; set; } // File size in bytes

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();
        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
