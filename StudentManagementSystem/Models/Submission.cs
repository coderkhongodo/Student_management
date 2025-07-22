using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class Submission
    {
        public int Id { get; set; }

        [Required]
        public int ExamScheduleId { get; set; }

        [ForeignKey("ExamScheduleId")]
        public virtual ExamSchedule ExamSchedule { get; set; } = null!;

        [Required]
        public string StudentUserId { get; set; } = string.Empty;

        [ForeignKey("StudentUserId")]
        public virtual ApplicationUser Student { get; set; } = null!;

        [StringLength(2000)]
        public string AnswerText { get; set; } = string.Empty; // Text answers

        [StringLength(500)]
        public string? AnswerFilePath { get; set; } // Path to uploaded answer file

        [StringLength(100)]
        public string? AnswerFileName { get; set; } // Original answer file name

        [StringLength(50)]
        public string? AnswerFileType { get; set; } // File type

        public long AnswerFileSize { get; set; } // File size in bytes

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public bool IsLateSubmission { get; set; } = false;

        public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;

        // Navigation properties
        public virtual Grade? Grade { get; set; }
    }

    public enum SubmissionStatus
    {
        Submitted = 1,
        Graded = 2,
        Returned = 3
    }
}
