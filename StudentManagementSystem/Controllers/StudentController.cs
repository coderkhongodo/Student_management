using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Constants;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.ViewModels;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = Roles.Student)]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly GradeCalculationService _gradeCalculationService;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, GradeCalculationService gradeCalculationService)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _gradeCalculationService = gradeCalculationService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Load student grades with subjects for GPA calculation
            var studentWithGrades = await _context.Users
                .Include(u => u.StudentGrades)
                    .ThenInclude(sg => sg.Subject)
                .FirstOrDefaultAsync(u => u.Id == currentUser!.Id);

            var now = DateTime.Now;
            var stats = new StudentDashboardViewModel
            {
                AvailableExams = await _context.ExamSchedules
                    .Where(es => now >= es.StartTime && now <= es.EndTime && es.IsActive)
                    .CountAsync(),
                CompletedExams = await _context.Submissions
                    .Where(s => s.StudentUserId == currentUser!.Id)
                    .CountAsync(),
                PendingResults = await _context.Submissions
                    .Where(s => s.StudentUserId == currentUser!.Id && s.Grade == null)
                    .CountAsync(),
                AverageScore = await _context.Grades
                    .Where(g => g.StudentUserId == currentUser!.Id)
                    .AverageAsync(g => (double?)g.Score) ?? 0,
                GPA = studentWithGrades?.CalculateGPA() ?? 0,
                FailedSubjectsCount = studentWithGrades?.GetFailedSubjectsCount() ?? 0,
                HasAcademicWarning = studentWithGrades?.HasAcademicWarning() ?? false
            };

            // Hiển thị cảnh báo nếu sinh viên có 2 môn rớt trở lên
            if (stats.HasAcademicWarning)
            {
                TempData["AcademicWarning"] = $"Cảnh báo: Bạn đã rớt {stats.FailedSubjectsCount} môn học. Vui lòng liên hệ với phòng đào tạo để được tư vấn.";
            }

            // Lấy hoạt động gần đây
            stats.RecentActivities = await GetRecentActivitiesAsync(currentUser.Id);

            // Lấy deadline sắp tới
            stats.UpcomingDeadlines = await GetUpcomingDeadlinesAsync(currentUser.Id);

            return View(stats);
        }

        // Xem các bài kiểm tra có sẵn
        public async Task<IActionResult> AvailableExams()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var availableExams = await _context.ExamSchedules
                .Include(es => es.Exam)
                .Include(es => es.Submissions.Where(s => s.StudentUserId == currentUser!.Id))
                .Where(es => es.IsActive)
                .OrderBy(es => es.StartTime)
                .ToListAsync();

            return View(availableExams);
        }

        // Xem chi tiết bài kiểm tra và làm bài
        [HttpGet]
        public async Task<IActionResult> TakeExam(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var examSchedule = await _context.ExamSchedules
                .Include(es => es.Exam)
                .Include(es => es.Submissions.Where(s => s.StudentUserId == currentUser!.Id))
                .FirstOrDefaultAsync(es => es.Id == id);

            if (examSchedule == null)
            {
                return NotFound();
            }

            // Kiểm tra xem học sinh đã nộp bài chưa
            var existingSubmission = examSchedule.Submissions.FirstOrDefault();
            if (existingSubmission != null)
            {
                TempData["Warning"] = "Bạn đã nộp bài cho kỳ thi này rồi!";
                return RedirectToAction(nameof(AvailableExams));
            }

            // Kiểm tra thời gian thi
            var now = DateTime.Now;
            if (!(now >= examSchedule.StartTime && now <= examSchedule.EndTime && examSchedule.IsActive))
            {
                if (now < examSchedule.StartTime)
                {
                    TempData["Warning"] = "Kỳ thi chưa bắt đầu!";
                }
                else if (now > examSchedule.EndTime)
                {
                    TempData["Warning"] = "Kỳ thi đã kết thúc!";
                }
                else if (!examSchedule.IsActive)
                {
                    TempData["Warning"] = "Kỳ thi đã bị hủy!";
                }
                return RedirectToAction(nameof(AvailableExams));
            }

            var model = new TakeExamViewModel
            {
                ExamScheduleId = examSchedule.Id,
                ExamId = examSchedule.Exam.Id,
                ExamTitle = examSchedule.Exam.Title,
                ExamDescription = examSchedule.Exam.Description,
                Instructions = examSchedule.Instructions,
                StartTime = examSchedule.StartTime,
                EndTime = examSchedule.EndTime,
                DurationMinutes = examSchedule.DurationMinutes,
                ExamFilePath = examSchedule.Exam.FilePath ?? string.Empty,
                ExamFileName = !string.IsNullOrEmpty(examSchedule.Exam.FileName) ? examSchedule.Exam.FileName : "Đề thi"
            };

            return View(model);
        }

        // Nộp bài kiểm tra
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitExam(TakeExamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var examSchedule = await _context.ExamSchedules
                    .Include(es => es.Submissions.Where(s => s.StudentUserId == currentUser!.Id))
                    .FirstOrDefaultAsync(es => es.Id == model.ExamScheduleId);

                if (examSchedule == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem đã nộp bài chưa
                if (examSchedule.Submissions.Any())
                {
                    TempData["Error"] = "Bạn đã nộp bài cho kỳ thi này rồi!";
                    return RedirectToAction(nameof(AvailableExams));
                }

                // Kiểm tra thời gian
                var isLateSubmission = DateTime.Now > examSchedule.EndTime;

                var submission = new Submission
                {
                    ExamScheduleId = model.ExamScheduleId,
                    StudentUserId = currentUser!.Id,
                    AnswerText = model.AnswerText,
                    IsLateSubmission = isLateSubmission
                };

                // Handle file upload
                if (model.AnswerFile != null && model.AnswerFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "submissions");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AnswerFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.AnswerFile.CopyToAsync(fileStream);
                    }

                    submission.AnswerFilePath = Path.Combine("uploads", "submissions", uniqueFileName);
                    submission.AnswerFileName = model.AnswerFile.FileName;
                    submission.AnswerFileType = Path.GetExtension(model.AnswerFile.FileName);
                    submission.AnswerFileSize = model.AnswerFile.Length;
                }

                _context.Submissions.Add(submission);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Bài thi đã được nộp thành công!";
                return RedirectToAction(nameof(MySubmissions));
            }

            return View("TakeExam", model);
        }

        // Xem các bài đã nộp
        public async Task<IActionResult> MySubmissions()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var submissions = await _context.Submissions
                .Include(s => s.ExamSchedule)
                    .ThenInclude(es => es.Exam)
                .Include(s => s.Grade)
                .Where(s => s.StudentUserId == currentUser!.Id)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }

        // Xem điểm số theo hệ thống 1:3:6
        public async Task<IActionResult> MyGrades()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var (semester, year) = _gradeCalculationService.GetCurrentSemesterAndYear();

            // Get all subjects the student is enrolled in
            var enrolledSubjects = await _context.ClassStudents
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.ClassSubjects)
                        .ThenInclude(cs => cs.Subject)
                .Where(cs => cs.StudentUserId == currentUser!.Id && cs.IsActive)
                .SelectMany(cs => cs.Class.ClassSubjects.Select(csub => csub.Subject))
                .Distinct()
                .ToListAsync();

            var gradeViewModels = new List<StudentGradeViewModel>();

            foreach (var subject in enrolledSubjects)
            {
                // Update/calculate grades for this subject
                var studentGrade = await _gradeCalculationService.UpdateStudentGradeAsync(
                    currentUser.Id, subject.Id, semester, year);

                var gradeViewModel = new StudentGradeViewModel
                {
                    SubjectId = subject.Id,
                    SubjectCode = subject.SubjectCode,
                    SubjectName = subject.SubjectName,
                    Credits = subject.Credits,
                    AttendanceScore = studentGrade.AttendanceScore,
                    MidtermScore = studentGrade.MidtermScore,
                    FinalScore = studentGrade.FinalScore,
                    TotalScore = studentGrade.TotalScore,
                    LetterGrade = studentGrade.GetLetterGrade(),
                    GradePoint = studentGrade.GetGradePoint(),
                    GradeDescription = studentGrade.GetGradeDescription(),
                    HasFinalScore = studentGrade.FinalScore > 0,
                    Comments = studentGrade.Comments,
                    Semester = semester,
                    Year = year
                };

                gradeViewModels.Add(gradeViewModel);
            }

            // Calculate overall GPA (only for subjects with final scores)
            var subjectsWithFinalScores = gradeViewModels.Where(g => g.HasFinalScore).ToList();
            var overallGPA = 0.0m;

            if (subjectsWithFinalScores.Any())
            {
                var totalPoints = subjectsWithFinalScores.Sum(g => g.GradePoint * g.Credits);
                var totalCredits = subjectsWithFinalScores.Sum(g => g.Credits);
                overallGPA = totalCredits > 0 ? totalPoints / totalCredits : 0;
            }

            var model = new StudentGradesOverviewViewModel
            {
                StudentName = currentUser.FullName,
                StudentId = currentUser.StudentId ?? "",
                Semester = semester,
                Year = year,
                Grades = gradeViewModels,
                OverallGPA = overallGPA,
                TotalCredits = subjectsWithFinalScores.Sum(g => g.Credits),
                CompletedSubjects = subjectsWithFinalScores.Count,
                TotalSubjects = gradeViewModels.Count
            };

            return View(model);
        }

        // Xem chi tiết điểm
        public async Task<IActionResult> GradeDetail(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var grade = await _context.Grades
                .Include(g => g.Submission)
                    .ThenInclude(s => s.ExamSchedule)
                        .ThenInclude(es => es.Exam)
                .Include(g => g.GradedBy)
                .FirstOrDefaultAsync(g => g.Id == id && g.StudentUserId == currentUser!.Id);

            if (grade == null)
            {
                return NotFound();
            }

            return View(grade);
        }

        // Download my submission file
        public async Task<IActionResult> DownloadMySubmission(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var submission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.Id == id && s.StudentUserId == currentUser!.Id);

            if (submission == null || string.IsNullOrEmpty(submission.AnswerFilePath))
            {
                return NotFound();
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, submission.AnswerFilePath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", submission.AnswerFileName);
        }

        // My Schedule - Xem lịch học
        public async Task<IActionResult> MySchedule()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Lấy tất cả các lớp mà student đang tham gia
            var studentClasses = await _context.ClassStudents
                .Include(cs => cs.Class)
                .Where(cs => cs.StudentUserId == currentUser!.Id && cs.IsActive)
                .Select(cs => cs.ClassId)
                .ToListAsync();

            // Lấy tất cả attendance sessions của các lớp mà student tham gia
            var attendanceSessions = await _context.AttendanceSessions
                .Include(ats => ats.Class)
                    .ThenInclude(c => c.ClassSubjects)
                        .ThenInclude(cs => cs.Subject)
                .Include(ats => ats.CreatedBy)
                .Include(ats => ats.Attendances.Where(a => a.StudentUserId == currentUser.Id))
                .Where(ats => studentClasses.Contains(ats.ClassId))
                .OrderByDescending(ats => ats.SessionDate)
                .ThenBy(ats => ats.StartTime)
                .ToListAsync();

            var scheduleViewModels = attendanceSessions.Select(ats => {
                var attendance = ats.Attendances.FirstOrDefault();
                var hasAttended = attendance != null;
                var attendanceStatus = GetAttendanceStatusText(attendance);

                return new StudentScheduleViewModel
                {
                    AttendanceSessionId = ats.Id,
                    SessionTitle = ats.SessionTitle,
                    SessionDescription = ats.Description,
                    SubjectName = ats.Class.ClassSubjects.FirstOrDefault()?.Subject?.SubjectName ?? "N/A",
                    ClassName = ats.Class.ClassName,
                    TeacherName = ats.CreatedBy.FullName,
                    SessionDate = ats.SessionDate,
                    StartTime = ats.StartTime,
                    EndTime = ats.EndTime,
                    Location = ats.Location,
                    IsCompleted = ats.IsCompleted,
                    HasAttended = hasAttended,
                    AttendanceStatus = attendanceStatus,
                    Status = GetSessionStatus(ats, hasAttended),
                    StatusClass = GetSessionStatusClass(ats, hasAttended)
                };
            }).ToList();

            // Phân loại schedules
            var today = DateTime.Today;
            var todaySchedules = scheduleViewModels.Where(s => s.SessionDate.Date == today).ToList();
            var upcomingSchedules = scheduleViewModels.Where(s => s.SessionDate.Date > today).ToList();
            var completedSchedules = scheduleViewModels.Where(s => s.SessionDate.Date < today).ToList();

            var attendedCount = scheduleViewModels.Count(s => s.HasAttended);
            var attendanceRate = scheduleViewModels.Count > 0 ? (double)attendedCount / scheduleViewModels.Count * 100 : 0;

            var model = new StudentScheduleOverviewViewModel
            {
                StudentName = currentUser!.FullName,
                StudentId = currentUser.StudentId ?? "",
                TodaySchedules = todaySchedules,
                UpcomingSchedules = upcomingSchedules,
                CompletedSchedules = completedSchedules,
                TotalSchedules = scheduleViewModels.Count(),
                CompletedCount = completedSchedules.Count(),
                TodayCount = todaySchedules.Count(),
                AttendedCount = attendedCount,
                AttendanceRate = attendanceRate
            };

            return View(model);
        }

        // Helper methods for attendance session status
        private string GetAttendanceStatusText(Attendance? attendance)
        {
            if (attendance == null)
                return "Chưa điểm danh";

            if (attendance.IsPresent)
                return attendance.IsLate ? "Đi muộn" : "Có mặt";
            else
                return attendance.IsExcused ? "Vắng có phép" : "Vắng mặt";
        }

        private string GetSessionStatus(AttendanceSession session, bool hasAttended)
        {
            var now = DateTime.Now;
            var sessionDateTime = session.SessionDate.Date.Add(session.StartTime);
            var endDateTime = session.SessionDate.Date.Add(session.EndTime);

            if (now < sessionDateTime)
                return "Sắp diễn ra";
            else if (now >= sessionDateTime && now <= endDateTime)
                return "Đang diễn ra";
            else if (session.IsCompleted)
                return hasAttended ? "Đã tham gia" : "Đã kết thúc";
            else
                return "Chưa hoàn thành";
        }

        private string GetSessionStatusClass(AttendanceSession session, bool hasAttended)
        {
            var now = DateTime.Now;
            var sessionDateTime = session.SessionDate.Date.Add(session.StartTime);
            var endDateTime = session.SessionDate.Date.Add(session.EndTime);

            if (now < sessionDateTime)
                return "primary";
            else if (now >= sessionDateTime && now <= endDateTime)
                return "warning";
            else if (session.IsCompleted)
                return hasAttended ? "success" : "danger";
            else
                return "secondary";
        }

        // Export Schedule to Excel
        public async Task<IActionResult> ExportSchedule()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Lấy tất cả các lớp mà student đang tham gia
            var studentClasses = await _context.ClassStudents
                .Include(cs => cs.Class)
                .Where(cs => cs.StudentUserId == currentUser!.Id && cs.IsActive)
                .Select(cs => cs.ClassId)
                .ToListAsync();

            // Lấy tất cả attendance sessions
            var attendanceSessions = await _context.AttendanceSessions
                .Include(ats => ats.Class)
                    .ThenInclude(c => c.ClassSubjects)
                        .ThenInclude(cs => cs.Subject)
                .Include(ats => ats.CreatedBy)
                .Include(ats => ats.Attendances.Where(a => a.StudentUserId == currentUser.Id))
                .Where(ats => studentClasses.Contains(ats.ClassId))
                .OrderBy(ats => ats.SessionDate)
                .ThenBy(ats => ats.StartTime)
                .ToListAsync();

            // Simple CSV export
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Ngày,Giờ bắt đầu,Giờ kết thúc,Buổi học,Môn học,Lớp,Giảng viên,Địa điểm,Trạng thái điểm danh");

            foreach (var session in attendanceSessions)
            {
                var attendance = session.Attendances.FirstOrDefault();
                var attendanceStatus = GetAttendanceStatusText(attendance);

                csv.AppendLine($"{session.SessionDate:dd/MM/yyyy}," +
                              $"{session.StartTime:hh\\:mm}," +
                              $"{session.EndTime:hh\\:mm}," +
                              $"{session.SessionTitle}," +
                              $"{session.Class.ClassSubjects.FirstOrDefault()?.Subject?.SubjectName ?? "N/A"}," +
                              $"{session.Class.ClassName}," +
                              $"{session.CreatedBy.FullName}," +
                              $"{session.Location}," +
                              $"{attendanceStatus}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"LichHoc_{currentUser.StudentId}_{DateTime.Now:yyyyMMdd}.csv");
        }

        // Helper methods for dashboard data
        private async Task<List<RecentActivityViewModel>> GetRecentActivitiesAsync(string studentUserId)
        {
            var activities = new List<RecentActivityViewModel>();

            // Recent submissions
            var recentSubmissions = await _context.Submissions
                .Include(s => s.ExamSchedule)
                    .ThenInclude(es => es.Exam)
                        .ThenInclude(e => e.Subject)
                .Where(s => s.StudentUserId == studentUserId)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(3)
                .ToListAsync();

            foreach (var submission in recentSubmissions)
            {
                activities.Add(new RecentActivityViewModel
                {
                    Type = "submission",
                    Title = "Nộp bài thành công",
                    Description = $"{submission.ExamSchedule.Exam.Title} - {submission.ExamSchedule.Exam.Subject.SubjectName}",
                    Time = submission.SubmittedAt,
                    Icon = "fas fa-paper-plane",
                    Color = "success"
                });
            }

            // Recent grades
            var recentGrades = await _context.Grades
                .Include(g => g.Submission)
                    .ThenInclude(s => s.ExamSchedule)
                        .ThenInclude(es => es.Exam)
                            .ThenInclude(e => e.Subject)
                .Where(g => g.StudentUserId == studentUserId)
                .OrderByDescending(g => g.GradedAt)
                .Take(2)
                .ToListAsync();

            foreach (var grade in recentGrades)
            {
                activities.Add(new RecentActivityViewModel
                {
                    Type = "grade",
                    Title = "Điểm số được cập nhật",
                    Description = $"{grade.Submission.ExamSchedule.Exam.Title} - Điểm: {grade.Score:F1}/10",
                    Time = grade.GradedAt,
                    Icon = "fas fa-star",
                    Color = grade.Score >= 8 ? "success" : grade.Score >= 6.5m ? "warning" : "danger"
                });
            }

            // Recent exam schedules
            var recentExams = await _context.ExamSchedules
                .Include(es => es.Exam)
                    .ThenInclude(e => e.Subject)
                .Where(es => es.CreatedAt >= DateTime.Now.AddDays(-7) && es.IsActive)
                .OrderByDescending(es => es.CreatedAt)
                .Take(2)
                .ToListAsync();

            foreach (var exam in recentExams)
            {
                activities.Add(new RecentActivityViewModel
                {
                    Type = "exam",
                    Title = "Có bài thi mới",
                    Description = $"{exam.Exam.Title} - {exam.Exam.Subject.SubjectName}",
                    Time = exam.CreatedAt,
                    Icon = "fas fa-file-alt",
                    Color = "info"
                });
            }

            return activities.OrderByDescending(a => a.Time).Take(5).ToList();
        }

        private async Task<List<UpcomingDeadlineViewModel>> GetUpcomingDeadlinesAsync(string studentUserId)
        {
            var deadlines = new List<UpcomingDeadlineViewModel>();

            // Get student's classes
            var studentClasses = await _context.ClassStudents
                .Where(cs => cs.StudentUserId == studentUserId && cs.IsActive)
                .Select(cs => cs.ClassId)
                .ToListAsync();

            // Upcoming exam schedules
            var upcomingExams = await _context.ExamSchedules
                .Include(es => es.Exam)
                    .ThenInclude(e => e.Subject)
                .Where(es => studentClasses.Contains(es.ClassId ?? 0) &&
                           es.StartTime > DateTime.Now &&
                           es.IsActive)
                .OrderBy(es => es.StartTime)
                .Take(5)
                .ToListAsync();

            foreach (var exam in upcomingExams)
            {
                var timeUntil = exam.StartTime - DateTime.Now;
                var urgency = timeUntil.TotalHours < 24 ? "danger" :
                             timeUntil.TotalDays < 3 ? "warning" : "primary";

                deadlines.Add(new UpcomingDeadlineViewModel
                {
                    Title = exam.Exam.Title,
                    Subject = exam.Exam.Subject.SubjectName,
                    DueDate = exam.StartTime,
                    TimeRemaining = FormatTimeRemaining(timeUntil),
                    Urgency = urgency,
                    Type = "exam"
                });
            }

            return deadlines;
        }

        private string FormatTimeRemaining(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} ngày";
            else if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours} giờ";
            else
                return $"{(int)timeSpan.TotalMinutes} phút";
        }
    }
}
