using System.ComponentModel.DataAnnotations;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.ViewModels
{


    public class CreateAttendanceSessionViewModel
    {
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Tiêu đề buổi học là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        [Display(Name = "Tiêu đề buổi học")]
        public string SessionTitle { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        [Display(Name = "Mô tả nội dung")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày học là bắt buộc")]
        [Display(Name = "Ngày học")]
        public DateTime SessionDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Giờ bắt đầu là bắt buộc")]
        [Display(Name = "Giờ bắt đầu")]
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0);

        [Required(ErrorMessage = "Giờ kết thúc là bắt buộc")]
        [Display(Name = "Giờ kết thúc")]
        public TimeSpan EndTime { get; set; } = new TimeSpan(10, 0, 0);

        [StringLength(100, ErrorMessage = "Phòng học không được vượt quá 100 ký tự")]
        [Display(Name = "Phòng học")]
        public string Location { get; set; } = string.Empty;
    }

    public class TakeAttendanceViewModel
    {
        public AttendanceSession Session { get; set; } = null!;
        public Class Class { get; set; } = null!;
        public List<StudentAttendanceItem> StudentAttendances { get; set; } = new List<StudentAttendanceItem>();
    }

    public class StudentAttendanceItem
    {
        public string StudentUserId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public bool IsPresent { get; set; } = false;
        public bool IsLate { get; set; } = false;
        public bool IsExcused { get; set; } = false;
        public string Note { get; set; } = string.Empty;
        public DateTime? CheckInTime { get; set; }
    }

    public class AttendanceReportViewModel
    {
        public Class Class { get; set; } = null!;
        public List<AttendanceSession> Sessions { get; set; } = new List<AttendanceSession>();
        public List<StudentAttendanceReport> StudentReports { get; set; } = new List<StudentAttendanceReport>();
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class StudentAttendanceReport
    {
        public ApplicationUser Student { get; set; } = null!;
        public int TotalSessions { get; set; }
        public int PresentSessions { get; set; }
        public int LateSessions { get; set; }
        public int ExcusedSessions { get; set; }
        public int AbsentSessions { get; set; }
        public decimal AttendanceRate { get; set; }
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
    }


}
