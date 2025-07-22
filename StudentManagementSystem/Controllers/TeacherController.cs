using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Constants;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = Roles.Teacher)]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeacherController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Thống kê cho giảng viên
            var myClasses = await _context.Classes
                .Where(c => c.TeacherUserId == currentUser!.Id && c.IsActive)
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.ClassStudents)
                .Include(c => c.AttendanceSessions)
                .ToListAsync();

            var totalStudents = myClasses.Sum(c => c.ClassStudents.Count(cs => cs.IsActive));
            var totalSessions = myClasses.Sum(c => c.AttendanceSessions.Count);

            var stats = new TeacherDashboardViewModel
            {
                TotalExams = await _context.Exams.CountAsync(e => e.CreatedByUserId == currentUser!.Id),
                TotalSchedules = await _context.ExamSchedules.CountAsync(es => es.CreatedByUserId == currentUser!.Id),
                TotalSubmissions = await _context.Submissions
                    .Include(s => s.ExamSchedule)
                    .CountAsync(s => s.ExamSchedule.CreatedByUserId == currentUser!.Id),
                PendingGrades = await _context.Submissions
                    .Include(s => s.ExamSchedule)
                    .CountAsync(s => s.ExamSchedule.CreatedByUserId == currentUser!.Id && s.Grade == null)
            };

            ViewBag.TotalClasses = myClasses.Count;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalSessions = totalSessions;
            ViewBag.MyClasses = myClasses;

            return View(stats);
        }

        // Quản lý bài kiểm tra
        public async Task<IActionResult> Exams()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var exams = await _context.Exams
                .Where(e => e.CreatedByUserId == currentUser!.Id)
                .Include(e => e.ExamSchedules)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return View(exams);
        }

        [HttpGet]
        public async Task<IActionResult> CreateExam()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var subjects = await _context.Subjects
                .Where(s => s.TeacherUserId == currentUser!.Id && s.IsActive)
                .ToListAsync();

            ViewBag.Subjects = subjects;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(CreateExamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                
                var exam = new Exam
                {
                    Title = model.Title,
                    Description = model.Description,
                    SubjectId = model.SubjectId,
                    CreatedByUserId = currentUser!.Id
                };

                // Handle file upload
                if (model.ExamFile != null && model.ExamFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "exams");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ExamFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ExamFile.CopyToAsync(fileStream);
                    }

                    exam.FilePath = Path.Combine("uploads", "exams", uniqueFileName);
                    exam.FileName = model.ExamFile.FileName;
                    exam.FileType = Path.GetExtension(model.ExamFile.FileName);
                    exam.FileSize = model.ExamFile.Length;
                }

                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Bài kiểm tra đã được tạo thành công!";
                return RedirectToAction(nameof(Exams));
            }

            // Reload subjects for dropdown
            var currentUserForReload = await _userManager.GetUserAsync(User);
            var subjects = await _context.Subjects
                .Where(s => s.TeacherUserId == currentUserForReload!.Id && s.IsActive)
                .ToListAsync();

            ViewBag.Subjects = subjects;
            return View(model);
        }

        // Quản lý lịch thi
        public async Task<IActionResult> Schedules()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var schedules = await _context.ExamSchedules
                .Where(es => es.CreatedByUserId == currentUser!.Id)
                .Include(es => es.Exam)
                .Include(es => es.Submissions)
                .Include(es => es.Class)
                    .ThenInclude(c => c.ClassStudents)
                .OrderByDescending(es => es.StartTime)
                .ToListAsync();

            return View(schedules);
        }

        [HttpGet]
        public async Task<IActionResult> CreateSchedule()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var exams = await _context.Exams
                .Where(e => e.CreatedByUserId == currentUser!.Id && e.IsActive)
                .ToListAsync();

            var classes = await _context.Classes
                .Include(c => c.ClassStudents)
                .Where(c => c.TeacherUserId == currentUser!.Id && c.IsActive)
                .ToListAsync();

            ViewBag.Exams = exams;
            ViewBag.Classes = classes;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchedule(CreateScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                // Get class name from ClassId
                var selectedClass = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == model.ClassId);

                if (selectedClass == null)
                {
                    ModelState.AddModelError("ClassId", "Lớp học không tồn tại.");
                }
                else
                {
                    var schedule = new ExamSchedule
                    {
                        ExamId = model.ExamId,
                        ClassId = model.ClassId,
                        ClassName = selectedClass.ClassName,
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                        DurationMinutes = (int)(model.EndTime - model.StartTime).TotalMinutes,
                        Location = model.Location,
                        Instructions = model.Instructions,
                        CreatedByUserId = currentUser!.Id
                    };

                    _context.ExamSchedules.Add(schedule);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Lịch thi đã được tạo thành công!";
                    return RedirectToAction(nameof(Schedules));
                }
            }

            // Reload data for dropdowns
            var currentUserForReload = await _userManager.GetUserAsync(User);
            var exams = await _context.Exams
                .Where(e => e.CreatedByUserId == currentUserForReload!.Id && e.IsActive)
                .ToListAsync();

            var classes = await _context.Classes
                .Include(c => c.ClassStudents)
                .Where(c => c.TeacherUserId == currentUserForReload!.Id && c.IsActive)
                .ToListAsync();

            ViewBag.Exams = exams;
            ViewBag.Classes = classes;
            return View(model);
        }

        // Chấm điểm
        public async Task<IActionResult> Grading()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var submissions = await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.ExamSchedule)
                    .ThenInclude(es => es.Exam)
                .Include(s => s.Grade)
                .Where(s => s.ExamSchedule.CreatedByUserId == currentUser!.Id)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }

        [HttpGet]
        public async Task<IActionResult> GradeSubmission(int id)
        {
            var submission = await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.ExamSchedule)
                    .ThenInclude(es => es.Exam)
                .Include(s => s.Grade)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (submission.ExamSchedule.CreatedByUserId != currentUser!.Id)
            {
                return Forbid();
            }

            var model = new GradeSubmissionViewModel
            {
                SubmissionId = submission.Id,
                StudentName = submission.Student.FullName,
                ExamTitle = submission.ExamSchedule.Exam.Title,
                SubmittedAt = submission.SubmittedAt,
                AnswerText = submission.AnswerText,
                AnswerFilePath = submission.AnswerFilePath,
                AnswerFileName = submission.AnswerFileName,
                Score = submission.Grade?.Score ?? 0,
                Comments = submission.Grade?.Comments ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GradeSubmission(GradeSubmissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var submission = await _context.Submissions
                    .Include(s => s.ExamSchedule)
                    .Include(s => s.Grade)
                    .FirstOrDefaultAsync(s => s.Id == model.SubmissionId);

                if (submission == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (submission.ExamSchedule.CreatedByUserId != currentUser!.Id)
                {
                    return Forbid();
                }

                if (submission.Grade == null)
                {
                    submission.Grade = new Grade
                    {
                        SubmissionId = submission.Id,
                        StudentUserId = submission.StudentUserId,
                        Score = model.Score,
                        Comments = model.Comments,
                        GradedByUserId = currentUser.Id
                    };
                    _context.Grades.Add(submission.Grade);
                }
                else
                {
                    submission.Grade.Score = model.Score;
                    submission.Grade.Comments = model.Comments;
                    submission.Grade.UpdatedAt = DateTime.Now;
                }

                submission.Status = SubmissionStatus.Graded;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Điểm đã được cập nhật thành công!";
                return RedirectToAction(nameof(Grading));
            }

            return View(model);
        }

        // Quản lý lớp học và điểm danh
        public async Task<IActionResult> MyClasses()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var classes = await _context.Classes
                .Where(c => c.TeacherUserId == currentUser!.Id)
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.ClassStudents)
                    .ThenInclude(cs => cs.Student)
                .Include(c => c.AttendanceSessions)
                .OrderByDescending(c => c.Year)
                .ThenBy(c => c.Semester)
                .ThenBy(c => c.ClassName)
                .ToListAsync();

            return View(classes);
        }

        public async Task<IActionResult> ClassDetails(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var classEntity = await _context.Classes
                .Where(c => c.Id == id && c.TeacherUserId == currentUser!.Id)
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.TeacherUser)
                .Include(c => c.ClassStudents)
                    .ThenInclude(cs => cs.Student)
                .Include(c => c.AttendanceSessions)
                    .ThenInclude(ats => ats.Attendances)
                .FirstOrDefaultAsync();

            if (classEntity == null)
            {
                return NotFound();
            }

            var model = new ClassDetailsViewModel
            {
                Class = classEntity,
                Students = classEntity.ClassStudents.Where(cs => cs.IsActive).ToList(),
                AttendanceSessions = classEntity.AttendanceSessions.OrderByDescending(ats => ats.SessionDate).ToList(),
                TotalStudents = classEntity.ClassStudents.Count(cs => cs.IsActive),
                TotalSessions = classEntity.AttendanceSessions.Count
            };

            // Tính tỷ lệ điểm danh tổng thể
            if (model.TotalSessions > 0 && model.TotalStudents > 0)
            {
                var totalPossibleAttendances = model.TotalSessions * model.TotalStudents;
                var totalPresentAttendances = classEntity.AttendanceSessions
                    .SelectMany(ats => ats.Attendances)
                    .Count(a => a.IsPresent);

                model.OverallAttendanceRate = totalPossibleAttendances > 0
                    ? (decimal)totalPresentAttendances / totalPossibleAttendances * 100
                    : 0;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAttendanceSession(int classId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var classEntity = await _context.Classes
                .Where(c => c.Id == classId && c.TeacherUserId == currentUser!.Id)
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .FirstOrDefaultAsync();

            if (classEntity == null)
            {
                return NotFound();
            }

            var model = new CreateAttendanceSessionViewModel
            {
                ClassId = classId,
                SessionDate = DateTime.Today,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(10, 0, 0)
            };

            ViewBag.Class = classEntity;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAttendanceSession(CreateAttendanceSessionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var classEntity = await _context.Classes
                    .Where(c => c.Id == model.ClassId && c.TeacherUserId == currentUser!.Id)
                    .FirstOrDefaultAsync();

                if (classEntity == null)
                {
                    return NotFound();
                }

                var session = new AttendanceSession
                {
                    ClassId = model.ClassId,
                    SessionTitle = model.SessionTitle,
                    Description = model.Description,
                    SessionDate = model.SessionDate,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Location = model.Location,
                    CreatedByUserId = currentUser!.Id
                };

                _context.AttendanceSessions.Add(session);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Buổi học đã được tạo thành công!";
                return RedirectToAction(nameof(ClassDetails), new { id = model.ClassId });
            }

            var classForView = await _context.Classes
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .FirstOrDefaultAsync(c => c.Id == model.ClassId);
            ViewBag.Class = classForView;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TakeAttendance(int sessionId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var session = await _context.AttendanceSessions
                .Include(ats => ats.Class)
                    .ThenInclude(c => c.ClassSubjects)
                        .ThenInclude(cs => cs.Subject)
                .Include(ats => ats.Class)
                    .ThenInclude(c => c.ClassStudents)
                        .ThenInclude(cs => cs.Student)
                .Include(ats => ats.Attendances)
                .Where(ats => ats.Id == sessionId && ats.Class.TeacherUserId == currentUser!.Id)
                .FirstOrDefaultAsync();

            if (session == null)
            {
                return NotFound();
            }

            var students = session.Class.ClassStudents.Where(cs => cs.IsActive).ToList();
            var studentAttendances = new List<StudentAttendanceItem>();

            foreach (var student in students)
            {
                var attendance = session.Attendances.FirstOrDefault(a => a.StudentUserId == student.StudentUserId);
                studentAttendances.Add(new StudentAttendanceItem
                {
                    StudentUserId = student.StudentUserId,
                    StudentName = student.Student.FullName,
                    StudentId = student.Student.StudentId,
                    IsPresent = attendance?.IsPresent ?? false,
                    IsLate = attendance?.IsLate ?? false,
                    IsExcused = attendance?.IsExcused ?? false,
                    Note = attendance?.Note ?? string.Empty,
                    CheckInTime = attendance?.CheckInTime
                });
            }

            var model = new TakeAttendanceViewModel
            {
                Session = session,
                Class = session.Class,
                StudentAttendances = studentAttendances
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeAttendance(int sessionId, List<StudentAttendanceItem> studentAttendances)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var session = await _context.AttendanceSessions
                .Include(ats => ats.Class)
                .Include(ats => ats.Attendances)
                .Where(ats => ats.Id == sessionId && ats.Class.TeacherUserId == currentUser!.Id)
                .FirstOrDefaultAsync();

            if (session == null)
            {
                return NotFound();
            }

            foreach (var studentAttendance in studentAttendances)
            {
                var existingAttendance = session.Attendances
                    .FirstOrDefault(a => a.StudentUserId == studentAttendance.StudentUserId);

                if (existingAttendance != null)
                {
                    // Cập nhật điểm danh hiện có
                    existingAttendance.IsPresent = studentAttendance.IsPresent;
                    existingAttendance.IsLate = studentAttendance.IsLate;
                    existingAttendance.IsExcused = studentAttendance.IsExcused;
                    existingAttendance.Note = studentAttendance.Note ?? string.Empty;
                    existingAttendance.CheckInTime = studentAttendance.IsPresent ? DateTime.Now : null;
                    existingAttendance.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // Tạo điểm danh mới
                    var newAttendance = new Attendance
                    {
                        AttendanceSessionId = sessionId,
                        StudentUserId = studentAttendance.StudentUserId,
                        IsPresent = studentAttendance.IsPresent,
                        IsLate = studentAttendance.IsLate,
                        IsExcused = studentAttendance.IsExcused,
                        Note = studentAttendance.Note ?? string.Empty,
                        CheckInTime = studentAttendance.IsPresent ? DateTime.Now : null,
                        MarkedByUserId = currentUser!.Id
                    };

                    _context.Attendances.Add(newAttendance);
                }
            }

            session.IsCompleted = true;
            session.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Điểm danh đã được lưu thành công!";
            return RedirectToAction(nameof(ClassDetails), new { id = session.ClassId });
        }

        [HttpGet]
        public async Task<IActionResult> ExportAttendanceReport(int classId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var classEntity = await _context.Classes
                .Where(c => c.Id == classId && c.TeacherUserId == currentUser!.Id)
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.TeacherUser)
                .Include(c => c.ClassStudents)
                    .ThenInclude(cs => cs.Student)
                .Include(c => c.AttendanceSessions)
                    .ThenInclude(ats => ats.Attendances)
                .FirstOrDefaultAsync();

            if (classEntity == null)
            {
                return NotFound();
            }

            // Filter sessions by date range if provided
            var sessions = classEntity.AttendanceSessions.AsQueryable();
            if (fromDate.HasValue)
            {
                sessions = sessions.Where(s => s.SessionDate >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                sessions = sessions.Where(s => s.SessionDate <= toDate.Value);
            }

            var filteredSessions = sessions.OrderBy(s => s.SessionDate).ToList();
            var students = classEntity.ClassStudents.Where(cs => cs.IsActive).OrderBy(cs => cs.Student.StudentId).ToList();

            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Báo cáo điểm danh");

            // Header information
            worksheet.Cells[1, 1].Value = "BÁO CÁO ĐIỂM DANH";
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, 10].Merge = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 1].Value = $"Lớp: {classEntity.ClassName}";

            // Display all subjects
            var allSubjects = classEntity.ClassSubjects.Select(cs => $"{cs.Subject.SubjectName} ({cs.Subject.SubjectCode})").ToList();
            worksheet.Cells[3, 1].Value = $"Môn học: {string.Join(", ", allSubjects)}";

            worksheet.Cells[4, 1].Value = $"Giảng viên: {classEntity.TeacherUser.FirstName} {classEntity.TeacherUser.LastName}";
            worksheet.Cells[5, 1].Value = $"Học kỳ: {classEntity.Semester} - {classEntity.Year}";
            worksheet.Cells[6, 1].Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";

            if (fromDate.HasValue || toDate.HasValue)
            {
                var dateRange = "";
                if (fromDate.HasValue && toDate.HasValue)
                    dateRange = $"Từ {fromDate.Value:dd/MM/yyyy} đến {toDate.Value:dd/MM/yyyy}";
                else if (fromDate.HasValue)
                    dateRange = $"Từ {fromDate.Value:dd/MM/yyyy}";
                else if (toDate.HasValue)
                    dateRange = $"Đến {toDate.Value:dd/MM/yyyy}";

                worksheet.Cells[7, 1].Value = $"Khoảng thời gian: {dateRange}";
            }

            // Generate file
            var fileName = $"DiemDanh_{classEntity.ClassName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // Download submission file
        public async Task<IActionResult> DownloadSubmission(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var submission = await _context.Submissions
                .Include(s => s.ExamSchedule)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null || string.IsNullOrEmpty(submission.AnswerFilePath))
            {
                return NotFound();
            }

            // Check if teacher owns this exam
            if (submission.ExamSchedule.CreatedByUserId != currentUser!.Id)
            {
                return Forbid();
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, submission.AnswerFilePath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", submission.AnswerFileName);
        }

        // Auto-grade expired exams
        [HttpPost]
        public async Task<IActionResult> ProcessExpiredExam(int examScheduleId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var examSchedule = await _context.ExamSchedules
                .Include(es => es.Class)
                    .ThenInclude(c => c.ClassStudents)
                        .ThenInclude(cs => cs.Student)
                .Include(es => es.Submissions)
                    .ThenInclude(s => s.Grade)
                .FirstOrDefaultAsync(es => es.Id == examScheduleId);

            if (examSchedule == null || examSchedule.CreatedByUserId != currentUser!.Id)
            {
                return NotFound();
            }

            if (examSchedule.EndTime > DateTime.Now)
            {
                TempData["Error"] = "Kỳ thi chưa kết thúc, không thể xử lý tự động chấm điểm.";
                return RedirectToAction(nameof(Grading));
            }

            if (examSchedule.AutoGradingProcessed)
            {
                TempData["Warning"] = "Kỳ thi này đã được xử lý tự động chấm điểm trước đó.";
                return RedirectToAction(nameof(Grading));
            }

            try
            {
                // Lấy danh sách tất cả học sinh trong lớp
                var studentsInClass = examSchedule.Class.ClassStudents
                    .Select(cs => cs.StudentUserId)
                    .ToList();

                // Lấy danh sách học sinh đã nộp bài
                var submittedStudents = examSchedule.Submissions
                    .Select(s => s.StudentUserId)
                    .ToList();

                // Tìm học sinh chưa nộp bài
                var missingStudents = studentsInClass
                    .Except(submittedStudents)
                    .ToList();

                int processedCount = 0;
                foreach (var studentId in missingStudents)
                {
                    // Tạo submission trống với điểm 0
                    var submission = new Submission
                    {
                        ExamScheduleId = examSchedule.Id,
                        StudentUserId = studentId,
                        AnswerText = "Không tham gia kiểm tra",
                        AnswerFilePath = null,
                        AnswerFileName = null,
                        AnswerFileType = null,
                        AnswerFileSize = 0,
                        SubmittedAt = examSchedule.EndTime.AddMinutes(1),
                        IsLateSubmission = true,
                        Status = SubmissionStatus.Submitted
                    };

                    _context.Submissions.Add(submission);
                    await _context.SaveChangesAsync();

                    // Tạo grade với điểm 0
                    var grade = new Grade
                    {
                        SubmissionId = submission.Id,
                        StudentUserId = studentId,
                        Score = 0,
                        MaxScore = 10,
                        Comments = "Tự động chấm điểm 0 do không tham gia kiểm tra trong thời gian quy định.",
                        LetterGrade = "F",
                        GradedByUserId = currentUser.Id,
                        GradedAt = DateTime.Now
                    };

                    _context.Grades.Add(grade);
                    processedCount++;
                }

                // Đánh dấu đã xử lý
                examSchedule.AutoGradingProcessed = true;
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đã tự động chấm điểm 0 cho {processedCount} học sinh không tham gia kiểm tra.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xử lý tự động chấm điểm: " + ex.Message;
            }

            return RedirectToAction(nameof(Grading));
        }

        // Student Grades Management
        public async Task<IActionResult> StudentGrades(int? classId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (classId.HasValue)
            {
                // Show grades for specific class
                var classItem = await _context.Classes
                    .Include(c => c.ClassStudents)
                        .ThenInclude(cs => cs.Student)
                    .Include(c => c.ClassSubjects)
                        .ThenInclude(cs => cs.Subject)
                    .FirstOrDefaultAsync(c => c.Id == classId.Value && c.TeacherUserId == currentUser!.Id);

                if (classItem == null)
                {
                    return NotFound();
                }

                var grades = await _context.StudentGrades
                    .Include(sg => sg.Student)
                    .Include(sg => sg.Subject)
                    .Where(sg => classItem.ClassSubjects.Select(cs => cs.SubjectId).Contains(sg.SubjectId) &&
                                classItem.ClassStudents.Select(cs => cs.StudentUserId).Contains(sg.StudentUserId))
                    .ToListAsync();

                ViewBag.Class = classItem;
                return View("ClassStudentGrades", grades);
            }
            else
            {
                // Show all grades for teacher's subjects
                var subjects = await _context.Subjects
                    .Where(s => s.TeacherUserId == currentUser!.Id)
                    .ToListAsync();

                var grades = await _context.StudentGrades
                    .Include(sg => sg.Student)
                    .Include(sg => sg.Subject)
                    .Where(sg => subjects.Select(s => s.Id).Contains(sg.SubjectId))
                    .OrderBy(sg => sg.Subject.SubjectName)
                    .ThenBy(sg => sg.Student.LastName)
                    .ToListAsync();

                ViewBag.Subjects = subjects;
                return View("AllStudentGrades", grades);
            }
        }
    }
}
