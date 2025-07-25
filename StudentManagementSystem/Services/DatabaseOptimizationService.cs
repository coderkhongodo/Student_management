using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace StudentManagementSystem.Services
{
    public class DatabaseOptimizationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DatabaseOptimizationService> _logger;

        public DatabaseOptimizationService(
            ApplicationDbContext context,
            IMemoryCache cache,
            ILogger<DatabaseOptimizationService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        // Optimized query for teacher dashboard
        public async Task<IEnumerable<Class>> GetTeacherClassesOptimizedAsync(string teacherId)
        {
            var cacheKey = $"teacher_classes_{teacherId}";
            
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Class>? cachedClasses))
            {
                return cachedClasses!;
            }

            var classes = await _context.Classes
                .AsNoTracking() // Read-only for better performance
                .Where(c => c.TeacherUserId == teacherId && c.IsActive)
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.ClassStudents)
                .Include(c => c.AttendanceSessions)
                .AsSplitQuery() // Avoid cartesian explosion
                .ToListAsync();

            // Cache for 5 minutes
            _cache.Set(cacheKey, classes, TimeSpan.FromMinutes(5));
            
            return classes;
        }

        // Optimized query for student submissions
        public async Task<IEnumerable<Submission>> GetStudentSubmissionsOptimizedAsync(string studentId)
        {
            var cacheKey = $"student_submissions_{studentId}";
            
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Submission>? cachedSubmissions))
            {
                return cachedSubmissions!;
            }

            var submissions = await _context.Submissions
                .AsNoTracking()
                .Where(s => s.StudentUserId == studentId)
                .Include(s => s.ExamSchedule)
                    .ThenInclude(es => es.Exam)
                .Include(s => s.Grade)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            // Cache for 2 minutes
            _cache.Set(cacheKey, submissions, TimeSpan.FromMinutes(2));
            
            return submissions;
        }

        // Optimized query for available exams
        public async Task<IEnumerable<ExamSchedule>> GetAvailableExamsOptimizedAsync(string studentId)
        {
            var cacheKey = $"available_exams_{studentId}";
            
            if (_cache.TryGetValue(cacheKey, out IEnumerable<ExamSchedule>? cachedExams))
            {
                return cachedExams!;
            }

            var exams = await _context.ExamSchedules
                .AsNoTracking()
                .Where(es => es.IsActive)
                .Include(es => es.Exam)
                .Include(es => es.Submissions.Where(s => s.StudentUserId == studentId))
                .OrderBy(es => es.StartTime)
                .ToListAsync();

            // Cache for 1 minute (more dynamic data)
            _cache.Set(cacheKey, exams, TimeSpan.FromMinutes(1));
            
            return exams;
        }

        // Optimized query for admin dashboard stats
        public async Task<AdminDashboardStats> GetAdminStatsOptimizedAsync()
        {
            var cacheKey = "admin_dashboard_stats";
            
            if (_cache.TryGetValue(cacheKey, out AdminDashboardStats? cachedStats))
            {
                return cachedStats!;
            }

            // Use parallel queries for independent data
            var tasks = new[]
            {
                _context.Users.AsNoTracking().CountAsync(u => u.StudentId != null),
                _context.Users.AsNoTracking().CountAsync(u => u.EmployeeId != null),
                _context.Subjects.AsNoTracking().CountAsync(),
                _context.Classes.AsNoTracking().CountAsync(),
                _context.Exams.AsNoTracking().CountAsync(),
                _context.Submissions.AsNoTracking().CountAsync()
            };

            var results = await Task.WhenAll(tasks);

            var stats = new AdminDashboardStats
            {
                TotalStudents = results[0],
                TotalTeachers = results[1],
                TotalSubjects = results[2],
                TotalClasses = results[3],
                TotalExams = results[4],
                TotalSubmissions = results[5]
            };

            // Cache for 10 minutes
            _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(10));
            
            return stats;
        }

        // Optimized query for class details with pagination
        public async Task<ClassDetailsOptimized> GetClassDetailsOptimizedAsync(int classId, string teacherId, int page = 1, int pageSize = 20)
        {
            var cacheKey = $"class_details_{classId}_{teacherId}_{page}_{pageSize}";
            
            if (_cache.TryGetValue(cacheKey, out ClassDetailsOptimized? cachedDetails))
            {
                return cachedDetails!;
            }

            var classEntity = await _context.Classes
                .AsNoTracking()
                .Where(c => c.Id == classId && c.TeacherUserId == teacherId)
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.TeacherUser)
                .FirstOrDefaultAsync();

            if (classEntity == null)
            {
                throw new ArgumentException("Class not found");
            }

            // Paginated students
            var students = await _context.ClassStudents
                .AsNoTracking()
                .Where(cs => cs.ClassId == classId)
                .Include(cs => cs.Student)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Recent attendance sessions (last 10)
            var recentSessions = await _context.AttendanceSessions
                .AsNoTracking()
                .Where(ats => ats.ClassId == classId)
                .Include(ats => ats.Attendances)
                .OrderByDescending(ats => ats.SessionDate)
                .Take(10)
                .ToListAsync();

            var result = new ClassDetailsOptimized
            {
                Class = classEntity,
                Students = students,
                RecentSessions = recentSessions,
                TotalStudents = await _context.ClassStudents.CountAsync(cs => cs.ClassId == classId)
            };

            // Cache for 3 minutes
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(3));
            
            return result;
        }

        // Batch operations for better performance
        public async Task<bool> BulkUpdateGradesAsync(IEnumerable<Grade> grades)
        {
            try
            {
                _context.UpdateRange(grades);
                await _context.SaveChangesAsync();
                
                // Clear related caches
                ClearGradeRelatedCaches();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk grade update");
                return false;
            }
        }

        // Cache invalidation methods
        public void ClearTeacherCache(string teacherId)
        {
            _cache.Remove($"teacher_classes_{teacherId}");
        }

        public void ClearStudentCache(string studentId)
        {
            _cache.Remove($"student_submissions_{studentId}");
            _cache.Remove($"available_exams_{studentId}");
        }

        public void ClearAdminCache()
        {
            _cache.Remove("admin_dashboard_stats");
        }

        private void ClearGradeRelatedCaches()
        {
            // This would need to be more sophisticated in a real application
            // For now, we'll clear all caches
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Clear();
            }
        }

        // Database health check
        public async Task<DatabaseHealthStatus> CheckDatabaseHealthAsync()
        {
            try
            {
                var startTime = DateTime.UtcNow;
                
                // Simple connectivity test
                await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                
                var responseTime = DateTime.UtcNow - startTime;
                
                return new DatabaseHealthStatus
                {
                    IsHealthy = true,
                    ResponseTime = responseTime,
                    Message = "Database is healthy"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                
                return new DatabaseHealthStatus
                {
                    IsHealthy = false,
                    ResponseTime = TimeSpan.Zero,
                    Message = $"Database error: {ex.Message}"
                };
            }
        }
    }

    // Supporting classes
    public class AdminDashboardStats
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalClasses { get; set; }
        public int TotalExams { get; set; }
        public int TotalSubmissions { get; set; }
    }

    public class ClassDetailsOptimized
    {
        public Class Class { get; set; } = null!;
        public IEnumerable<ClassStudent> Students { get; set; } = new List<ClassStudent>();
        public IEnumerable<AttendanceSession> RecentSessions { get; set; } = new List<AttendanceSession>();
        public int TotalStudents { get; set; }
    }

    public class DatabaseHealthStatus
    {
        public bool IsHealthy { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
