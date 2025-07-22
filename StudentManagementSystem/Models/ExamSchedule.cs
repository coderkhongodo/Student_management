using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class ExamSchedule
    {
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; } = null!;

        public int? ClassId { get; set; } // Optional reference to Class

        [ForeignKey("ClassId")]
        public virtual Class? Class { get; set; }

        [Required]
        [StringLength(100)]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public int DurationMinutes { get; set; }

        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [StringLength(500)]
        public string Instructions { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool AutoGradingProcessed { get; set; } = false; // Đánh dấu đã xử lý auto-grading

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedBy { get; set; } = null!;

        // Navigation properties
        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

        // Helper properties
        public bool IsAvailable => DateTime.Now >= StartTime && DateTime.Now <= EndTime && IsActive;
        public bool HasStarted => DateTime.Now >= StartTime;
        public bool HasEnded => DateTime.Now > EndTime;
    }
}
