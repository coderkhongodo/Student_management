using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        public int AttendanceSessionId { get; set; }

        [ForeignKey("AttendanceSessionId")]
        public virtual AttendanceSession AttendanceSession { get; set; } = null!;

        [Required]
        public string StudentUserId { get; set; } = string.Empty;

        [ForeignKey("StudentUserId")]
        public virtual ApplicationUser Student { get; set; } = null!;

        public bool IsPresent { get; set; } = false; // Có mặt hay không

        [StringLength(500)]
        public string Note { get; set; } = string.Empty; // Ghi chú (lý do vắng mặt, đi muộn, v.v.)

        public DateTime? CheckInTime { get; set; } // Thời gian điểm danh (nếu có mặt)

        public bool IsLate { get; set; } = false; // Đi muộn

        public bool IsExcused { get; set; } = false; // Vắng có phép

        [Required]
        public string MarkedByUserId { get; set; } = string.Empty; // Người điểm danh

        [ForeignKey("MarkedByUserId")]
        public virtual ApplicationUser MarkedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Helper methods
        public string GetAttendanceStatus()
        {
            if (IsPresent)
            {
                return IsLate ? "Đi muộn" : "Có mặt";
            }
            else
            {
                return IsExcused ? "Vắng có phép" : "Vắng không phép";
            }
        }

        public string GetAttendanceStatusClass()
        {
            if (IsPresent)
            {
                return IsLate ? "warning" : "success";
            }
            else
            {
                return IsExcused ? "info" : "danger";
            }
        }
    }
}
