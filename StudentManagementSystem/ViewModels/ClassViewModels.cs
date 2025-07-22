using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.ViewModels
{
    public class CreateClassViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên lớp học")]
        [StringLength(100, ErrorMessage = "Tên lớp học không được vượt quá 100 ký tự")]
        [Display(Name = "Tên lớp học")]
        public string ClassName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Môn học")]
        public List<int> SubjectIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "Vui lòng chọn giảng viên")]
        [Display(Name = "Giảng viên")]
        public string TeacherUserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập học kỳ")]
        [StringLength(50, ErrorMessage = "Học kỳ không được vượt quá 50 ký tự")]
        [Display(Name = "Học kỳ")]
        public string Semester { get; set; } = "1";

        [Required(ErrorMessage = "Vui lòng nhập năm học")]
        [Range(2020, 2030, ErrorMessage = "Năm học phải từ 2020 đến 2030")]
        [Display(Name = "Năm học")]
        public int Year { get; set; } = DateTime.Now.Year;

        [Required(ErrorMessage = "Vui lòng nhập số sinh viên tối đa")]
        [Range(1, 100, ErrorMessage = "Số sinh viên tối đa phải từ 1 đến 100")]
        [Display(Name = "Số sinh viên tối đa")]
        public int MaxStudents { get; set; } = 30;
    }

    public class EditClassViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên lớp học")]
        [StringLength(100, ErrorMessage = "Tên lớp học không được vượt quá 100 ký tự")]
        [Display(Name = "Tên lớp học")]
        public string ClassName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Môn học")]
        public List<int> SubjectIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "Vui lòng chọn giảng viên")]
        [Display(Name = "Giảng viên")]
        public string TeacherUserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập học kỳ")]
        [StringLength(50, ErrorMessage = "Học kỳ không được vượt quá 50 ký tự")]
        [Display(Name = "Học kỳ")]
        public string Semester { get; set; } = "1";

        [Required(ErrorMessage = "Vui lòng nhập năm học")]
        [Range(2020, 2030, ErrorMessage = "Năm học phải từ 2020 đến 2030")]
        [Display(Name = "Năm học")]
        public int Year { get; set; } = DateTime.Now.Year;

        [Required(ErrorMessage = "Vui lòng nhập số sinh viên tối đa")]
        [Range(1, 100, ErrorMessage = "Số sinh viên tối đa phải từ 1 đến 100")]
        [Display(Name = "Số sinh viên tối đa")]
        public int MaxStudents { get; set; } = 30;

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;
    }

    public class AssignStudentsViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public List<Models.ApplicationUser> AvailableStudents { get; set; } = new List<Models.ApplicationUser>();
        public List<Models.ClassStudent> CurrentStudents { get; set; } = new List<Models.ClassStudent>();
    }

    public class ClassDetailsViewModel
    {
        public Models.Class Class { get; set; } = null!;
        public List<Models.ClassStudent> Students { get; set; } = new List<Models.ClassStudent>();
        public List<Models.AttendanceSession> AttendanceSessions { get; set; } = new List<Models.AttendanceSession>();
        public int TotalStudents { get; set; }
        public int TotalSessions { get; set; }
        public decimal OverallAttendanceRate { get; set; }
    }
}
