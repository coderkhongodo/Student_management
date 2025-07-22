using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public class AutoGradingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutoGradingService> _logger;

        public AutoGradingService(IServiceProvider serviceProvider, ILogger<AutoGradingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiredExams();
                    
                    // Chạy mỗi 5 phút
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing expired exams");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task ProcessExpiredExams()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Tìm các kỳ thi đã hết hạn nhưng chưa được xử lý auto-grading
            var expiredExams = await context.ExamSchedules
                .Include(es => es.Class)
                    .ThenInclude(c => c.ClassStudents)
                        .ThenInclude(cs => cs.Student)
                .Include(es => es.Submissions)
                    .ThenInclude(s => s.Grade)
                .Where(es => es.EndTime < DateTime.Now && es.IsActive && !es.AutoGradingProcessed)
                .ToListAsync();

            foreach (var examSchedule in expiredExams)
            {
                await ProcessExpiredExam(context, examSchedule);
            }

            if (expiredExams.Any())
            {
                await context.SaveChangesAsync();
                _logger.LogInformation($"Processed {expiredExams.Count} expired exams for auto-grading");
            }
        }

        private async Task ProcessExpiredExam(ApplicationDbContext context, ExamSchedule examSchedule)
        {
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
                        SubmittedAt = examSchedule.EndTime.AddMinutes(1), // Đánh dấu là nộp muộn
                        IsLateSubmission = true,
                        Status = SubmissionStatus.Submitted
                    };

                    context.Submissions.Add(submission);
                    await context.SaveChangesAsync(); // Save để có submission.Id

                    // Tạo grade với điểm 0
                    var grade = new Grade
                    {
                        SubmissionId = submission.Id,
                        StudentUserId = studentId,
                        Score = 0,
                        MaxScore = 10,
                        Comments = "Tự động chấm điểm 0 do không tham gia kiểm tra trong thời gian quy định.",
                        LetterGrade = "F",
                        GradedByUserId = examSchedule.CreatedByUserId, // Teacher tạo kỳ thi
                        GradedAt = DateTime.Now
                    };

                    context.Grades.Add(grade);

                    _logger.LogInformation($"Auto-graded student {studentId} with score 0 for exam {examSchedule.Id}");
                }

                // Đánh dấu kỳ thi đã được xử lý auto-grading
                examSchedule.AutoGradingProcessed = true;

                _logger.LogInformation($"Processed exam {examSchedule.Id}: {missingStudents.Count} students auto-graded with score 0");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing expired exam {examSchedule.Id}");
            }
        }
    }
}
