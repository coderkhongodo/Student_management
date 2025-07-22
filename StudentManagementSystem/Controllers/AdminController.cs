using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Constants;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var stats = new AdminDashboardViewModel
            {
                TotalStudents = await _userManager.GetUsersInRoleAsync(Roles.Student).ContinueWith(t => t.Result.Count),
                TotalTeachers = await _userManager.GetUsersInRoleAsync(Roles.Teacher).ContinueWith(t => t.Result.Count),
                TotalSubjects = await _context.Subjects.CountAsync(),
                TotalClasses = await _context.Classes.CountAsync(),
                TotalExams = await _context.Exams.CountAsync(),
                TotalSubmissions = await _context.Submissions.CountAsync()
            };

            return View(stats);
        }

        // Quản lý giảng viên
        public async Task<IActionResult> Teachers()
        {
            var teachers = await _userManager.GetUsersInRoleAsync(Roles.Teacher);
            return View(teachers);
        }

        [HttpGet]
        public IActionResult CreateTeacher()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeacher(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmployeeId = model.EmployeeId,
                    Salary = model.Salary,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Teacher);
                    TempData["Success"] = "Giảng viên đã được tạo thành công!";
                    return RedirectToAction(nameof(Teachers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditTeacher(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmployeeId = user.EmployeeId,
                Salary = user.Salary,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeacher(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.EmployeeId = model.EmployeeId;
                user.Salary = model.Salary;
                user.Address = model.Address;
                user.DateOfBirth = model.DateOfBirth;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Thông tin giảng viên đã được cập nhật!";
                    return RedirectToAction(nameof(Teachers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra xem giảng viên có bài kiểm tra nào không
            var hasExams = await _context.Exams.AnyAsync(e => e.CreatedByUserId == id);
            if (hasExams)
            {
                TempData["Error"] = "Không thể xóa giảng viên này vì đã có bài kiểm tra được tạo!";
                return RedirectToAction(nameof(Teachers));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Giảng viên đã được xóa thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa giảng viên!";
            }

            return RedirectToAction(nameof(Teachers));
        }

        // Quản lý học sinh
        public async Task<IActionResult> Students()
        {
            var students = await _userManager.GetUsersInRoleAsync(Roles.Student);
            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> CreateStudent()
        {
            var classes = await _context.Classes
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Where(c => c.IsActive)
                .ToListAsync();
            ViewBag.Classes = classes;

            var model = new CreateStudentViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(CreateStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    StudentId = model.StudentId,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth ?? default,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Student);

                    // Thêm sinh viên vào lớp nếu được chọn
                    if (model.ClassId.HasValue)
                    {
                        var classStudent = new ClassStudent
                        {
                            ClassId = model.ClassId.Value,
                            StudentUserId = user.Id,
                            CreatedByUserId = currentUser!.Id
                        };
                        _context.ClassStudents.Add(classStudent);
                        await _context.SaveChangesAsync();
                    }

                    TempData["Success"] = "Học sinh đã được tạo thành công!";
                    return RedirectToAction(nameof(Students));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var classes = await _context.Classes
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Where(c => c.IsActive)
                .ToListAsync();
            ViewBag.Classes = classes;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditStudent(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                StudentId = user.StudentId,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.StudentId = model.StudentId;
                user.Address = model.Address;
                user.DateOfBirth = model.DateOfBirth;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Thông tin học sinh đã được cập nhật!";
                    return RedirectToAction(nameof(Students));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra xem học sinh có bài nộp nào không
            var hasSubmissions = await _context.Submissions.AnyAsync(s => s.StudentUserId == id);
            if (hasSubmissions)
            {
                TempData["Error"] = "Không thể xóa học sinh này vì đã có bài nộp!";
                return RedirectToAction(nameof(Students));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Học sinh đã được xóa thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa học sinh!";
            }

            return RedirectToAction(nameof(Students));
        }

        // Quản lý điểm số sinh viên
        public async Task<IActionResult> StudentGrades()
        {
            var studentGrades = await _context.StudentGrades
                .Include(sg => sg.Student)
                .Include(sg => sg.Subject)
                .Include(sg => sg.CreatedBy)
                .OrderByDescending(sg => sg.Year)
                .ThenBy(sg => sg.Semester)
                .ThenBy(sg => sg.Subject.SubjectName)
                .ToListAsync();

            return View(studentGrades);
        }

        [HttpGet]
        public async Task<IActionResult> CreateStudentGrade()
        {
            var students = await _userManager.GetUsersInRoleAsync(Roles.Student);
            var subjects = await _context.Subjects.Where(s => s.IsActive).ToListAsync();
            ViewBag.Students = students;
            ViewBag.Subjects = subjects;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudentGrade(CreateStudentGradeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var studentGrade = new StudentGrade
                {
                    StudentUserId = model.StudentUserId,
                    SubjectId = model.SubjectId,
                    Semester = model.Semester,
                    Year = model.Year,
                    AttendanceScore = 0, // Không có trong form mới
                    MidtermScore = model.MidtermScore ?? 0,
                    FinalScore = model.FinalScore ?? 0,
                    Comments = model.Comments,
                    CreatedByUserId = currentUser!.Id
                };

                _context.StudentGrades.Add(studentGrade);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Điểm số đã được tạo thành công!";
                return RedirectToAction(nameof(StudentGrades));
            }

            var students = await _userManager.GetUsersInRoleAsync(Roles.Student);
            var subjects = await _context.Subjects.Where(s => s.IsActive).ToListAsync();
            ViewBag.Students = students;
            ViewBag.Subjects = subjects;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditStudentGrade(int id)
        {
            var studentGrade = await _context.StudentGrades
                .Include(sg => sg.Student)
                .Include(sg => sg.Subject)
                .FirstOrDefaultAsync(sg => sg.Id == id);

            if (studentGrade == null)
            {
                return NotFound();
            }

            var model = new EditStudentGradeViewModel
            {
                Id = studentGrade.Id,
                StudentUserId = studentGrade.StudentUserId,
                StudentName = studentGrade.Student.FullName,
                SubjectId = studentGrade.SubjectId,
                SubjectName = studentGrade.Subject.SubjectName,
                Semester = studentGrade.Semester,
                Year = studentGrade.Year,
                AttendanceScore = studentGrade.AttendanceScore,
                MidtermScore = studentGrade.MidtermScore,
                FinalScore = studentGrade.FinalScore,
                Comments = studentGrade.Comments
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudentGrade(EditStudentGradeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var studentGrade = await _context.StudentGrades.FindAsync(model.Id);
                if (studentGrade == null)
                {
                    return NotFound();
                }

                studentGrade.AttendanceScore = model.AttendanceScore;
                studentGrade.MidtermScore = model.MidtermScore;
                studentGrade.FinalScore = model.FinalScore;
                studentGrade.Comments = model.Comments;
                studentGrade.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Điểm số đã được cập nhật thành công!";
                return RedirectToAction(nameof(StudentGrades));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudentGrade(int id)
        {
            var studentGrade = await _context.StudentGrades.FindAsync(id);
            if (studentGrade == null)
            {
                return NotFound();
            }

            _context.StudentGrades.Remove(studentGrade);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Điểm số đã được xóa thành công!";
            return RedirectToAction(nameof(StudentGrades));
        }

        // Quản lý môn học
        public async Task<IActionResult> Subjects()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Teacher)
                .Include(s => s.CreatedBy)
                .OrderBy(s => s.SubjectCode)
                .ToListAsync();

            return View(subjects);
        }

        [HttpGet]
        public async Task<IActionResult> CreateSubject()
        {
            var teachers = await _userManager.GetUsersInRoleAsync(Roles.Teacher);
            ViewBag.Teachers = teachers;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubject(CreateSubjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var subject = new Subject
                {
                    SubjectCode = model.SubjectCode,
                    SubjectName = model.SubjectName,
                    Description = model.Description,
                    Credits = model.Credits,
                    TeacherUserId = model.TeacherUserId,
                    CreatedByUserId = currentUser!.Id
                };

                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Môn học đã được tạo thành công!";
                return RedirectToAction(nameof(Subjects));
            }

            var teachers = await _userManager.GetUsersInRoleAsync(Roles.Teacher);
            ViewBag.Teachers = teachers;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            var teachers = await _userManager.GetUsersInRoleAsync(Roles.Teacher);
            ViewBag.Teachers = teachers;

            var model = new EditSubjectViewModel
            {
                Id = subject.Id,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
                Credits = subject.Credits,
                TeacherUserId = subject.TeacherUserId,
                IsActive = subject.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSubject(EditSubjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subject = await _context.Subjects.FindAsync(model.Id);
                if (subject == null)
                {
                    return NotFound();
                }

                subject.SubjectCode = model.SubjectCode;
                subject.SubjectName = model.SubjectName;
                subject.Description = model.Description;
                subject.Credits = model.Credits;
                subject.TeacherUserId = model.TeacherUserId;
                subject.IsActive = model.IsActive;
                subject.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Môn học đã được cập nhật thành công!";
                return RedirectToAction(nameof(Subjects));
            }

            var teachers = await _userManager.GetUsersInRoleAsync(Roles.Teacher);
            ViewBag.Teachers = teachers;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            // Kiểm tra xem môn học có lớp học nào không
            var hasClasses = await _context.ClassSubjects.AnyAsync(cs => cs.SubjectId == id);
            if (hasClasses)
            {
                TempData["Error"] = "Không thể xóa môn học này vì đã có lớp học!";
                return RedirectToAction(nameof(Subjects));
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Môn học đã được xóa thành công!";
            return RedirectToAction(nameof(Subjects));
        }

        // Quản lý lớp học
        public async Task<IActionResult> Classes()
        {
            var classes = await _context.Classes
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.TeacherUser)
                .Include(c => c.ClassStudents)
                .OrderByDescending(c => c.Year)
                .ThenBy(c => c.Semester)
                .ThenBy(c => c.ClassName)
                .ToListAsync();

            return View(classes);
        }

        [HttpGet]
        public async Task<IActionResult> CreateClass()
        {
            var subjects = await _context.Subjects.Where(s => s.IsActive).ToListAsync();
            var teachers = await _userManager.GetUsersInRoleAsync(Roles.Teacher);

            ViewBag.Subjects = subjects;
            ViewBag.Teachers = teachers;

            // Truyền empty model để tránh null reference
            var model = new CreateClassViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClass(CreateClassViewModel model)
        {
            // 🔍 Debug logging
            Console.WriteLine($"DEBUG: Model received - ClassName: {model.ClassName}");
            Console.WriteLine($"DEBUG: Model received - TeacherUserId: {model.TeacherUserId}");
            Console.WriteLine($"DEBUG: SubjectIds received: {(model.SubjectIds == null ? "NULL" : string.Join(", ", model.SubjectIds))}");
            Console.WriteLine($"DEBUG: SubjectIds count: {model.SubjectIds?.Count ?? 0}");

            // ✅ Kiểm tra các trường bắt buộc trước khi xử lý
            if (string.IsNullOrEmpty(model.TeacherUserId))
            {
                ModelState.AddModelError("TeacherUserId", "Vui lòng chọn giảng viên.");
            }

            // ✅ Kiểm tra và làm sạch SubjectIds
            if (model.SubjectIds == null)
            {
                model.SubjectIds = new List<int>();
            }

            // Lọc bỏ các giá trị không hợp lệ (0, âm, trùng lặp)
            model.SubjectIds = model.SubjectIds.Where(id => id > 0).Distinct().ToList();

            if (!model.SubjectIds.Any())
            {
                ModelState.AddModelError("SubjectIds", "Vui lòng chọn ít nhất một môn học.");
            }

            // ✅ Kiểm tra SubjectIds có tồn tại trong database không
            if (model.SubjectIds.Any())
            {
                var existingSubjectIds = await _context.Subjects
                    .Where(s => model.SubjectIds.Contains(s.Id) && s.IsActive)
                    .Select(s => s.Id)
                    .ToListAsync();

                var invalidSubjectIds = model.SubjectIds.Except(existingSubjectIds).ToList();
                if (invalidSubjectIds.Any())
                {
                    ModelState.AddModelError("SubjectIds", $"Các môn học không tồn tại hoặc không hoạt động: {string.Join(", ", invalidSubjectIds)}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = await _userManager.GetUserAsync(User);

                    if (currentUser == null)
                    {
                        TempData["Error"] = "Không thể xác định người dùng hiện tại.";
                        return RedirectToAction(nameof(CreateClass));
                    }

                    var classEntity = new Class
                    {
                        ClassName = model.ClassName,
                        Description = model.Description,
                        TeacherUserId = model.TeacherUserId,
                        Semester = model.Semester,
                        Year = model.Year,
                        MaxStudents = model.MaxStudents,
                        CreatedByUserId = currentUser.Id,
                        CreatedAt = DateTime.Now
                    };

                    _context.Classes.Add(classEntity);
                    await _context.SaveChangesAsync(); // Save class first to get ID

                    // ✅ Add subjects - với null safety
                    if (model.SubjectIds != null && model.SubjectIds.Any())
                    {
                        foreach (var subjectId in model.SubjectIds)
                        {
                            if (subjectId > 0) // currentUser đã được kiểm tra null ở trên
                            {
                                var classSubject = new ClassSubject
                                {
                                    ClassId = classEntity.Id,
                                    SubjectId = subjectId,
                                    CreatedByUserId = currentUser.Id,
                                    CreatedAt = DateTime.Now
                                };
                                _context.ClassSubjects.Add(classSubject);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Lớp học đã được tạo thành công!";
                    return RedirectToAction(nameof(Classes));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Có lỗi xảy ra khi tạo lớp học: " + ex.Message;
                }
            }

            var subjects = await _context.Subjects.Where(s => s.IsActive).ToListAsync();
            var teachers = await _userManager.GetUsersInRoleAsync(Roles.Teacher);

            ViewBag.Subjects = subjects;
            ViewBag.Teachers = teachers;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditClass(int id)
        {
            var classEntity = await _context.Classes
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.TeacherUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classEntity == null)
            {
                return NotFound();
            }

            var subjects = await _context.Subjects.Where(s => s.IsActive).ToListAsync();
            var teachers = await _userManager.GetUsersInRoleAsync(Roles.Teacher);

            ViewBag.Subjects = subjects;
            ViewBag.Teachers = teachers;

            var model = new EditClassViewModel
            {
                Id = classEntity.Id,
                ClassName = classEntity.ClassName,
                Description = classEntity.Description,
                SubjectIds = classEntity.ClassSubjects.Select(cs => cs.SubjectId).ToList(),
                TeacherUserId = classEntity.TeacherUserId,
                Semester = classEntity.Semester,
                Year = classEntity.Year,
                MaxStudents = classEntity.MaxStudents,
                IsActive = classEntity.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClass(EditClassViewModel model)
        {
            // ✅ Kiểm tra các trường bắt buộc trước khi xử lý
            if (string.IsNullOrEmpty(model.TeacherUserId))
            {
                ModelState.AddModelError("TeacherUserId", "Vui lòng chọn giảng viên.");
            }

            // ✅ Kiểm tra và làm sạch SubjectIds
            if (model.SubjectIds == null)
            {
                model.SubjectIds = new List<int>();
            }

            // Lọc bỏ các giá trị không hợp lệ (0, âm, trùng lặp)
            model.SubjectIds = model.SubjectIds.Where(id => id > 0).Distinct().ToList();

            // ✅ Debug logging
            Console.WriteLine($"[DEBUG] EditClass POST - SubjectIds count: {model.SubjectIds.Count}");
            Console.WriteLine($"[DEBUG] EditClass POST - SubjectIds: [{string.Join(", ", model.SubjectIds)}]");

            if (!model.SubjectIds.Any())
            {
                ModelState.AddModelError("SubjectIds", "Vui lòng chọn ít nhất một môn học.");
            }

            // ✅ Kiểm tra SubjectIds có tồn tại trong database không
            if (model.SubjectIds.Any())
            {
                var existingSubjectIds = await _context.Subjects
                    .Where(s => model.SubjectIds.Contains(s.Id) && s.IsActive)
                    .Select(s => s.Id)
                    .ToListAsync();

                var invalidSubjectIds = model.SubjectIds.Except(existingSubjectIds).ToList();
                if (invalidSubjectIds.Any())
                {
                    ModelState.AddModelError("SubjectIds", $"Các môn học không tồn tại hoặc không hoạt động: {string.Join(", ", invalidSubjectIds)}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = await _userManager.GetUserAsync(User);

                    if (currentUser == null)
                    {
                        TempData["Error"] = "Không thể xác định người dùng hiện tại.";
                        return RedirectToAction(nameof(Classes));
                    }

                    // ✅ Sử dụng transaction để đảm bảo tính nhất quán
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        var classEntity = await _context.Classes
                            .Include(c => c.ClassSubjects)
                            .FirstOrDefaultAsync(c => c.Id == model.Id);

                        if (classEntity == null)
                        {
                            return NotFound();
                        }

                        // Update main class properties
                        classEntity.ClassName = model.ClassName;
                        classEntity.Description = model.Description;
                        classEntity.TeacherUserId = model.TeacherUserId;
                        classEntity.Semester = model.Semester;
                        classEntity.Year = model.Year;
                        classEntity.MaxStudents = model.MaxStudents;
                        classEntity.IsActive = model.IsActive;
                        classEntity.UpdatedAt = DateTime.Now;

                        // Remove existing ClassSubjects
                        var existingClassSubjects = await _context.ClassSubjects
                            .Where(cs => cs.ClassId == model.Id)
                            .ToListAsync();
                        _context.ClassSubjects.RemoveRange(existingClassSubjects);

                        // Save changes để commit việc xóa trước
                        await _context.SaveChangesAsync();

                        // Add new subjects
                        foreach (var subjectId in model.SubjectIds)
                        {
                            if (subjectId > 0)
                            {
                                // ✅ Kiểm tra xem ClassSubject đã tồn tại chưa (phòng trường hợp constraint)
                                var existingClassSubject = await _context.ClassSubjects
                                    .FirstOrDefaultAsync(cs => cs.ClassId == classEntity.Id && cs.SubjectId == subjectId);
                                
                                if (existingClassSubject == null)
                                {
                                    var classSubject = new ClassSubject
                                    {
                                        ClassId = classEntity.Id,
                                        SubjectId = subjectId,
                                        CreatedByUserId = currentUser.Id, // currentUser đã được kiểm tra null ở trên
                                        CreatedAt = DateTime.Now
                                    };
                                    _context.ClassSubjects.Add(classSubject);
                                }
                            }
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        TempData["Success"] = "Lớp học đã được cập nhật thành công!";
                        return RedirectToAction(nameof(Classes));
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw; // Re-throw để catch bên ngoài xử lý
                    }
                }
                catch (Exception ex)
                {
                    // Log chi tiết lỗi để debug
                    Console.WriteLine($"[ERROR] EditClass Exception: {ex}");
                    Console.WriteLine($"[ERROR] Inner Exception: {ex.InnerException?.Message}");
                    Console.WriteLine($"[ERROR] Stack Trace: {ex.StackTrace}");

                    TempData["Error"] = "Có lỗi xảy ra khi cập nhật lớp học: " + ex.Message;
                }
            }

            var subjects = await _context.Subjects.Where(s => s.IsActive).ToListAsync();
            var teachers = await _userManager.GetUsersInRoleAsync(Roles.Teacher);

            ViewBag.Subjects = subjects;
            ViewBag.Teachers = teachers;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var classEntity = await _context.Classes
                .Include(c => c.ClassStudents)
                .Include(c => c.AttendanceSessions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classEntity == null)
            {
                return NotFound();
            }

            // Kiểm tra xem lớp có sinh viên hoặc buổi học không
            if (classEntity.ClassStudents.Any() || classEntity.AttendanceSessions.Any())
            {
                TempData["Error"] = "Không thể xóa lớp học này vì đã có sinh viên hoặc buổi học!";
                return RedirectToAction(nameof(Classes));
            }

            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Lớp học đã được xóa thành công!";
            return RedirectToAction(nameof(Classes));
        }

        [HttpGet]
        public async Task<IActionResult> AssignStudents(int id)
        {
            var classEntity = await _context.Classes
                .Include(c => c.ClassSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.ClassStudents)
                    .ThenInclude(cs => cs.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classEntity == null)
            {
                return NotFound();
            }

            var allStudents = await _userManager.GetUsersInRoleAsync(Roles.Student);
            var currentStudentIds = classEntity.ClassStudents.Select(cs => cs.StudentUserId).ToList();
            var availableStudents = allStudents.Where(s => !currentStudentIds.Contains(s.Id)).ToList();

            var model = new AssignStudentsViewModel
            {
                ClassId = classEntity.Id,
                ClassName = classEntity.ClassName,
                AvailableStudents = availableStudents,
                CurrentStudents = classEntity.ClassStudents.Where(cs => cs.IsActive).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudents(int classId, List<string> selectedStudentIds)
        {
            var classEntity = await _context.Classes.FindAsync(classId);
            if (classEntity == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Thêm sinh viên mới vào lớp
            foreach (var studentId in selectedStudentIds)
            {
                // Kiểm tra xem sinh viên đã có trong lớp chưa
                var existingAssignment = await _context.ClassStudents
                    .FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.StudentUserId == studentId);

                if (existingAssignment == null)
                {
                    var classStudent = new ClassStudent
                    {
                        ClassId = classId,
                        StudentUserId = studentId,
                        CreatedByUserId = currentUser!.Id
                    };

                    _context.ClassStudents.Add(classStudent);
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã phân công {selectedStudentIds.Count} sinh viên vào lớp học!";
            return RedirectToAction(nameof(AssignStudents), new { id = classId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudentFromClass(int classId, string studentId)
        {
            var classStudent = await _context.ClassStudents
                .FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.StudentUserId == studentId);

            if (classStudent != null)
            {
                _context.ClassStudents.Remove(classStudent);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa sinh viên khỏi lớp học!";
            }

            return RedirectToAction(nameof(AssignStudents), new { id = classId });
        }

        // Xem tất cả bài kiểm tra
        public async Task<IActionResult> Exams()
        {
            var exams = await _context.Exams
                .Include(e => e.CreatedBy)
                .Include(e => e.ExamSchedules)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return View(exams);
        }

        // Xem tất cả điểm số
        public async Task<IActionResult> Grades()
        {
            var grades = await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Submission)
                    .ThenInclude(s => s.ExamSchedule)
                        .ThenInclude(es => es.Exam)
                .Include(g => g.GradedBy)
                .OrderByDescending(g => g.GradedAt)
                .ToListAsync();

            return View(grades);
        }
    }
}
