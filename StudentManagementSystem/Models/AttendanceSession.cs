using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class AttendanceSession
    {
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string SessionTitle { get; set; } = string.Empty; // Tiêu đề buổi học

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty; // Mô tả nội dung buổi học

        [Required]
        public DateTime SessionDate { get; set; } // Ngày học

        [Required]
        public TimeSpan StartTime { get; set; } // Giờ bắt đầu

        [Required]
        public TimeSpan EndTime { get; set; } // Giờ kết thúc

        [StringLength(100)]
        public string Location { get; set; } = string.Empty; // Phòng học

        public bool IsCompleted { get; set; } = false; // Đã hoàn thành điểm danh chưa

        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        // Helper methods
        public int GetTotalStudents()
        {
            return Class?.ClassStudents?.Count(cs => cs.IsActive) ?? 0;
        }

        public int GetPresentStudents()
        {
            return Attendances.Count(a => a.IsPresent);
        }

        public int GetAbsentStudents()
        {
            return Attendances.Count(a => !a.IsPresent);
        }

        public decimal GetAttendanceRate()
        {
            var total = GetTotalStudents();
            if (total == 0) return 0;
            return (decimal)GetPresentStudents() / total * 100;
        }
    }
}
