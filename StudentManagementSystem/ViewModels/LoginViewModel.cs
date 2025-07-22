using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalClasses { get; set; }
        public int TotalExams { get; set; }
        public int TotalSubmissions { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Mã nhân viên")]
        public string EmployeeId { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Mã sinh viên")]
        public string StudentId { get; set; } = string.Empty;

        [Display(Name = "Lương")]
        public decimal Salary { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Lớp học")]
        public int? ClassId { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Mã nhân viên")]
        public string EmployeeId { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Mã sinh viên")]
        public string StudentId { get; set; } = string.Empty;

        [Display(Name = "Lương")]
        public decimal Salary { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }

    public class TeacherDashboardViewModel
    {
        public int TotalExams { get; set; }
        public int TotalSchedules { get; set; }
        public int TotalSubmissions { get; set; }
        public int PendingGrades { get; set; }
    }

    public class CreateExamViewModel
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Tiêu đề bài kiểm tra")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn môn học")]
        [Display(Name = "Môn học")]
        public int SubjectId { get; set; }

        [Display(Name = "File bài kiểm tra")]
        public IFormFile? ExamFile { get; set; }
    }

    public class CreateScheduleViewModel
    {
        [Required]
        [Display(Name = "Bài kiểm tra")]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn lớp học")]
        [Display(Name = "Lớp học")]
        public int ClassId { get; set; }

        [Required]
        [Display(Name = "Thời gian bắt đầu")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "Thời gian kết thúc")]
        public DateTime EndTime { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa điểm")]
        public string Location { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Hướng dẫn")]
        public string Instructions { get; set; } = string.Empty;
    }

    public class GradeSubmissionViewModel
    {
        public int SubmissionId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ExamTitle { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string? AnswerText { get; set; }
        public string? AnswerFilePath { get; set; }
        public string? AnswerFileName { get; set; }

        [Required]
        [Range(0, 10)]
        [Display(Name = "Điểm số")]
        public decimal Score { get; set; }

        [StringLength(1000)]
        [Display(Name = "Nhận xét")]
        public string Comments { get; set; } = string.Empty;
    }

    public class StudentDashboardViewModel
    {
        public int AvailableExams { get; set; }
        public int CompletedExams { get; set; }
        public int PendingResults { get; set; }
        public double AverageScore { get; set; }
        public decimal GPA { get; set; }
        public int FailedSubjectsCount { get; set; }
        public bool HasAcademicWarning { get; set; }
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new List<RecentActivityViewModel>();
        public List<UpcomingDeadlineViewModel> UpcomingDeadlines { get; set; } = new List<UpcomingDeadlineViewModel>();
    }

    public class RecentActivityViewModel
    {
        public string Type { get; set; } = string.Empty; // submission, grade, exam
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty; // success, warning, danger, info
        public string TimeAgo => GetTimeAgo(Time);

        private string GetTimeAgo(DateTime time)
        {
            var timeSpan = DateTime.Now - time;
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} ngày trước";
            else if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            else if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            else
                return "Vừa xong";
        }
    }

    public class UpcomingDeadlineViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string TimeRemaining { get; set; } = string.Empty;
        public string Urgency { get; set; } = string.Empty; // danger, warning, primary
        public string Type { get; set; } = string.Empty; // exam, assignment
    }

    public class StudentScheduleViewModel
    {
        public int AttendanceSessionId { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public string SessionDescription { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public bool HasAttended { get; set; }
        public string AttendanceStatus { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusClass { get; set; } = string.Empty;
    }

    public class StudentScheduleOverviewViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public List<StudentScheduleViewModel> TodaySchedules { get; set; } = new List<StudentScheduleViewModel>();
        public List<StudentScheduleViewModel> UpcomingSchedules { get; set; } = new List<StudentScheduleViewModel>();
        public List<StudentScheduleViewModel> CompletedSchedules { get; set; } = new List<StudentScheduleViewModel>();
        public int TotalSchedules { get; set; }
        public int CompletedCount { get; set; }
        public int TodayCount { get; set; }
        public int AttendedCount { get; set; }
        public double AttendanceRate { get; set; }
    }

    public class TakeExamViewModel
    {
        public int ExamScheduleId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public string ExamDescription { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public string ExamFilePath { get; set; } = string.Empty;
        public string ExamFileName { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Câu trả lời")]
        public string AnswerText { get; set; } = string.Empty;

        [Display(Name = "File câu trả lời")]
        public IFormFile? AnswerFile { get; set; }
    }

    public class CreateStudentGradeViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn học sinh")]
        [Display(Name = "Học sinh")]
        public string StudentUserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn môn học")]
        [Display(Name = "Môn học")]
        public int SubjectId { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm giữa kỳ phải từ 0 đến 10")]
        [Display(Name = "Điểm giữa kỳ")]
        public decimal? MidtermScore { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm cuối kỳ phải từ 0 đến 10")]
        [Display(Name = "Điểm cuối kỳ")]
        public decimal? FinalScore { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm tổng kết phải từ 0 đến 10")]
        [Display(Name = "Điểm tổng kết")]
        public decimal? TotalScore { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn học kỳ")]
        [Display(Name = "Học kỳ")]
        public string Semester { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập năm học")]
        [Range(2020, 2030, ErrorMessage = "Năm học phải từ 2020 đến 2030")]
        [Display(Name = "Năm học")]
        public int Year { get; set; } = DateTime.Now.Year;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string Comments { get; set; } = string.Empty;
    }

    public class EditStudentGradeViewModel
    {
        public int Id { get; set; }
        public string StudentUserId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Môn học")]
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Học kỳ")]
        public string Semester { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Năm học")]
        public int Year { get; set; }

        [Range(0, 10)]
        [Display(Name = "Điểm danh (10%)")]
        public decimal AttendanceScore { get; set; }

        [Range(0, 10)]
        [Display(Name = "Điểm giữa kỳ (30%)")]
        public decimal MidtermScore { get; set; }

        [Range(0, 10)]
        [Display(Name = "Điểm cuối kỳ (60%)")]
        public decimal FinalScore { get; set; }

        [StringLength(1000)]
        [Display(Name = "Nhận xét")]
        public string Comments { get; set; } = string.Empty;
    }

    public class CreateSubjectViewModel
    {
        [Required]
        [StringLength(20)]
        [Display(Name = "Mã môn học")]
        public string SubjectCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Tên môn học")]
        public string SubjectName { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        [Display(Name = "Số tín chỉ")]
        public int Credits { get; set; } = 3;

        [Required]
        [Display(Name = "Giảng viên phụ trách")]
        public string TeacherUserId { get; set; } = string.Empty;
    }

    public class EditSubjectViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã môn học")]
        public string SubjectCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Tên môn học")]
        public string SubjectName { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        [Display(Name = "Số tín chỉ")]
        public int Credits { get; set; }

        [Required]
        [Display(Name = "Giảng viên phụ trách")]
        public string TeacherUserId { get; set; } = string.Empty;

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;
    }

    public class CreateStudentViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        [Display(Name = "Tên")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự")]
        [Display(Name = "Họ")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mã sinh viên")]
        [StringLength(20, ErrorMessage = "Mã sinh viên không được vượt quá 20 ký tự")]
        [Display(Name = "Mã sinh viên")]
        public string StudentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Lớp học")]
        public int? ClassId { get; set; }
    }

    public class StudentGradeViewModel
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credits { get; set; }

        // Component scores
        public decimal AttendanceScore { get; set; } // 10%
        public decimal MidtermScore { get; set; } // 30%
        public decimal FinalScore { get; set; } // 60%

        // Calculated scores
        public decimal TotalScore { get; set; }
        public string LetterGrade { get; set; } = string.Empty;
        public decimal GradePoint { get; set; }
        public string GradeDescription { get; set; } = string.Empty;

        public bool HasFinalScore { get; set; }
        public string Comments { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int Year { get; set; }
    }

    public class StudentGradesOverviewViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int Year { get; set; }

        public List<StudentGradeViewModel> Grades { get; set; } = new List<StudentGradeViewModel>();

        public decimal OverallGPA { get; set; }
        public int TotalCredits { get; set; }
        public int CompletedSubjects { get; set; }
        public int TotalSubjects { get; set; }

        // Statistics
        public int ExcellentCount => Grades.Count(g => g.GradePoint >= 3.7m);
        public int GoodCount => Grades.Count(g => g.GradePoint >= 3.0m && g.GradePoint < 3.7m);
        public int AverageCount => Grades.Count(g => g.GradePoint >= 2.0m && g.GradePoint < 3.0m);
        public int FailedCount => Grades.Count(g => g.GradePoint < 1.0m);
    }

}
