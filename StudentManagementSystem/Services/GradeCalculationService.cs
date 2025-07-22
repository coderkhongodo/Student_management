using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public class GradeCalculationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GradeCalculationService> _logger;

        public GradeCalculationService(ApplicationDbContext context, ILogger<GradeCalculationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Calculate attendance score for a student in a subject (10%)
        public async Task<decimal> CalculateAttendanceScoreAsync(string studentUserId, int subjectId, string semester, int year)
        {
            try
            {
                // Get all classes for this subject that the student is enrolled in
                var studentClasses = await _context.ClassStudents
                    .Include(cs => cs.Class)
                        .ThenInclude(c => c.ClassSubjects)
                    .Where(cs => cs.StudentUserId == studentUserId && cs.IsActive)
                    .Where(cs => cs.Class.ClassSubjects.Any(csub => csub.SubjectId == subjectId))
                    .Select(cs => cs.ClassId)
                    .ToListAsync();

                if (!studentClasses.Any())
                    return 0;

                // Get all attendance sessions for these classes
                var totalSessions = await _context.AttendanceSessions
                    .Where(ats => studentClasses.Contains(ats.ClassId))
                    .Where(ats => ats.SessionDate.Year == year)
                    .CountAsync();

                if (totalSessions == 0)
                    return 10; // Full score if no sessions yet

                // Get attended sessions
                var attendedSessions = await _context.Attendances
                    .Include(a => a.AttendanceSession)
                    .Where(a => a.StudentUserId == studentUserId)
                    .Where(a => studentClasses.Contains(a.AttendanceSession.ClassId))
                    .Where(a => a.AttendanceSession.SessionDate.Year == year)
                    .Where(a => a.IsPresent) // Present or late counts as attended
                    .CountAsync();

                var attendanceRate = (decimal)attendedSessions / totalSessions;
                return Math.Round(attendanceRate * 10, 2); // Convert to 0-10 scale
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating attendance score for student {StudentId}, subject {SubjectId}", studentUserId, subjectId);
                return 0;
            }
        }

        // Calculate midterm score for a student in a subject (30%)
        public async Task<decimal> CalculateMidtermScoreAsync(string studentUserId, int subjectId, string semester, int year)
        {
            try
            {
                // Get all midterm exam grades for this student and subject
                var midtermGrades = await _context.Grades
                    .Include(g => g.Submission)
                        .ThenInclude(s => s.ExamSchedule)
                            .ThenInclude(es => es.Exam)
                    .Where(g => g.StudentUserId == studentUserId)
                    .Where(g => g.Submission.ExamSchedule.Exam.SubjectId == subjectId)
                    .Where(g => g.Submission.ExamSchedule.Exam.Title.ToLower().Contains("giữa kì") || 
                               g.Submission.ExamSchedule.Exam.Title.ToLower().Contains("midterm"))
                    .ToListAsync();

                if (!midtermGrades.Any())
                    return 0; // No midterm exams yet

                var averageScore = midtermGrades.Average(g => g.Score);
                return Math.Round(averageScore, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating midterm score for student {StudentId}, subject {SubjectId}", studentUserId, subjectId);
                return 0;
            }
        }

        // Calculate final score for a student in a subject (60%)
        public async Task<decimal?> CalculateFinalScoreAsync(string studentUserId, int subjectId, string semester, int year)
        {
            try
            {
                // Get final exam grade for this student and subject
                var finalGrade = await _context.Grades
                    .Include(g => g.Submission)
                        .ThenInclude(s => s.ExamSchedule)
                            .ThenInclude(es => es.Exam)
                    .Where(g => g.StudentUserId == studentUserId)
                    .Where(g => g.Submission.ExamSchedule.Exam.SubjectId == subjectId)
                    .Where(g => g.Submission.ExamSchedule.Exam.Title.ToLower().Contains("cuối kì") || 
                               g.Submission.ExamSchedule.Exam.Title.ToLower().Contains("final"))
                    .FirstOrDefaultAsync();

                return finalGrade?.Score;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating final score for student {StudentId}, subject {SubjectId}", studentUserId, subjectId);
                return null;
            }
        }

        // Update or create StudentGrade record
        public async Task<StudentGrade> UpdateStudentGradeAsync(string studentUserId, int subjectId, string semester, int year)
        {
            try
            {
                // Find existing StudentGrade or create new one
                var studentGrade = await _context.StudentGrades
                    .FirstOrDefaultAsync(sg => sg.StudentUserId == studentUserId && 
                                              sg.SubjectId == subjectId && 
                                              sg.Semester == semester && 
                                              sg.Year == year);

                if (studentGrade == null)
                {
                    studentGrade = new StudentGrade
                    {
                        StudentUserId = studentUserId,
                        SubjectId = subjectId,
                        Semester = semester,
                        Year = year,
                        CreatedByUserId = null, // System-generated
                        CreatedAt = DateTime.Now
                    };
                    _context.StudentGrades.Add(studentGrade);
                }

                // Calculate scores
                studentGrade.AttendanceScore = await CalculateAttendanceScoreAsync(studentUserId, subjectId, semester, year);
                studentGrade.MidtermScore = await CalculateMidtermScoreAsync(studentUserId, subjectId, semester, year);
                
                var finalScore = await CalculateFinalScoreAsync(studentUserId, subjectId, semester, year);
                if (finalScore.HasValue)
                {
                    studentGrade.FinalScore = finalScore.Value;
                }

                studentGrade.UpdatedAt = DateTime.Now;
                studentGrade.Comments = $"Tự động tính toán - Cập nhật lúc {DateTime.Now:dd/MM/yyyy HH:mm}";

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated grades for student {StudentId}, subject {SubjectId}: Attendance={Attendance}, Midterm={Midterm}, Final={Final}", 
                    studentUserId, subjectId, studentGrade.AttendanceScore, studentGrade.MidtermScore, studentGrade.FinalScore);

                return studentGrade;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student grade for student {StudentId}, subject {SubjectId}", studentUserId, subjectId);
                throw;
            }
        }

        // Calculate grades for all students in a subject
        public async Task UpdateAllStudentGradesForSubjectAsync(int subjectId, string semester, int year)
        {
            try
            {
                // Get all students enrolled in classes for this subject
                var students = await _context.ClassStudents
                    .Include(cs => cs.Class)
                        .ThenInclude(c => c.ClassSubjects)
                    .Where(cs => cs.IsActive)
                    .Where(cs => cs.Class.ClassSubjects.Any(csub => csub.SubjectId == subjectId))
                    .Select(cs => cs.StudentUserId)
                    .Distinct()
                    .ToListAsync();

                foreach (var studentId in students)
                {
                    await UpdateStudentGradeAsync(studentId, subjectId, semester, year);
                }

                _logger.LogInformation("Updated grades for {Count} students in subject {SubjectId}", students.Count, subjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating all student grades for subject {SubjectId}", subjectId);
                throw;
            }
        }

        // Get current semester and year
        public (string semester, int year) GetCurrentSemesterAndYear()
        {
            var now = DateTime.Now;
            var year = now.Year;
            
            // Simple semester logic: Jan-May = Spring, Jun-Aug = Summer, Sep-Dec = Fall
            var semester = now.Month switch
            {
                >= 1 and <= 5 => "Spring",
                >= 6 and <= 8 => "Summer",
                >= 9 and <= 12 => "Fall",
                _ => "Spring"
            };

            return (semester, year);
        }
    }
}
