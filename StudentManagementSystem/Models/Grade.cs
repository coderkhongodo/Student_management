using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class Grade
    {
        public int Id { get; set; }

        [Required]
        public int SubmissionId { get; set; }

        [ForeignKey("SubmissionId")]
        public virtual Submission Submission { get; set; } = null!;

        [Required]
        public string StudentUserId { get; set; } = string.Empty;

        [ForeignKey("StudentUserId")]
        public virtual ApplicationUser Student { get; set; } = null!;

        [Required]
        [Range(0, 10)]
        public decimal Score { get; set; } // Điểm từ 0-10

        [Range(0, 100)]
        public decimal MaxScore { get; set; } = 10; // Điểm tối đa

        [StringLength(1000)]
        public string Comments { get; set; } = string.Empty; // Nhận xét của giảng viên

        [StringLength(50)]
        public string LetterGrade { get; set; } = string.Empty; // A, B, C, D, F

        [Required]
        public string GradedByUserId { get; set; } = string.Empty;

        [ForeignKey("GradedByUserId")]
        public virtual ApplicationUser GradedBy { get; set; } = null!;

        public DateTime GradedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Helper properties
        public decimal Percentage => MaxScore > 0 ? (Score / MaxScore) * 100 : 0;

        public string GetLetterGrade()
        {
            var percentage = Percentage;
            return percentage switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                >= 60 => "D",
                _ => "F"
            };
        }
    }

    public class StudentGrade
    {
        public int Id { get; set; }

        [Required]
        public string StudentUserId { get; set; } = string.Empty;

        [ForeignKey("StudentUserId")]
        public virtual ApplicationUser Student { get; set; } = null!;

        [Required]
        public int SubjectId { get; set; } // Môn học

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Semester { get; set; } = string.Empty; // Học kỳ

        [Required]
        public int Year { get; set; } // Năm học

        // Điểm danh (10%)
        [Range(0, 10)]
        public decimal AttendanceScore { get; set; } = 0;

        // Điểm giữa kỳ (30%)
        [Range(0, 10)]
        public decimal MidtermScore { get; set; } = 0;

        // Điểm cuối kỳ (60%)
        [Range(0, 10)]
        public decimal FinalScore { get; set; } = 0;

        [StringLength(1000)]
        public string Comments { get; set; } = string.Empty;

        public string? CreatedByUserId { get; set; } // Nullable for system-generated grades

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Tính điểm tổng kết
        public decimal TotalScore => (AttendanceScore * 0.1m) + (MidtermScore * 0.3m) + (FinalScore * 0.6m);

        public string GetLetterGrade()
        {
            var total = TotalScore;
            return total switch
            {
                >= 8.5m => "A+",
                >= 8.0m => "A",
                >= 7.5m => "B+",
                >= 7.0m => "B",
                >= 6.5m => "C+",
                >= 6.0m => "C",
                >= 5.5m => "D+",
                >= 5.0m => "D",
                _ => "F"
            };
        }

        public decimal GetGradePoint()
        {
            var total = TotalScore;
            return total switch
            {
                >= 8.5m => 4.0m,
                >= 8.0m => 3.7m,
                >= 7.5m => 3.3m,
                >= 7.0m => 3.0m,
                >= 6.5m => 2.7m,
                >= 6.0m => 2.3m,
                >= 5.5m => 2.0m,
                >= 5.0m => 1.7m,
                >= 4.0m => 1.0m,
                _ => 0.0m
            };
        }

        public string GetGradeDescription()
        {
            var total = TotalScore;
            return total switch
            {
                >= 8.5m => "Xuất sắc",
                >= 8.0m => "Giỏi",
                >= 7.0m => "Khá",
                >= 6.0m => "Trung bình",
                >= 5.0m => "Yếu",
                >= 4.0m => "Kém",
                _ => "Rớt môn"
            };
        }

        public bool IsFailed => TotalScore < 4.0m;
    }
}
