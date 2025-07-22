# 🎓 Student Management System

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-purple.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-9.0-green.svg)](https://docs.microsoft.com/en-us/ef/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red.svg)](https://www.microsoft.com/en-us/sql-server/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple.svg)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](http://makeapullrequest.com)
[![GitHub stars](https://img.shields.io/github/stars/coderkhongodo/Student_management.svg)](https://github.com/coderkhongodo/Student_management/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/coderkhongodo/Student_management.svg)](https://github.com/coderkhongodo/Student_management/network)

> **Version 1.0.0** - Hệ thống quản lý sinh viên hiện đại và đầy đủ tính năng

Hệ thống quản lý sinh viên hiện đại được xây dựng bằng ASP.NET Core 9.0, cung cấp giải pháp toàn diện cho việc quản lý giáo dục trực tuyến.

## 🌟 Tính năng chính

### 👨‍🎓 Dành cho Sinh viên
- **Dashboard cá nhân** với thống kê học tập trực quan
- **Xem điểm số** và theo dõi tiến độ học tập
- **Thi trực tuyến** với giao diện thân thiện
- **Lịch học** và lịch thi cá nhân
- **Nộp bài tập** trực tuyến (hỗ trợ file đính kèm)
- **Theo dõi điểm danh** và tỷ lệ tham gia

### 👨‍🏫 Dành cho Giảng viên
- **Quản lý lớp học** và danh sách sinh viên
- **Tạo và quản lý bài kiểm tra** trực tuyến
- **Chấm điểm tự động** và thủ công
- **Quản lý lịch thi** và thời gian biểu
- **Theo dõi tiến độ** của từng sinh viên
- **Xuất báo cáo** điểm số và thống kê

### 👨‍💼 Dành cho Quản trị viên
- **Quản lý người dùng** (sinh viên, giảng viên)
- **Quản lý môn học** và chương trình đào tạo
- **Phân quyền hệ thống** chi tiết
- **Báo cáo tổng quan** và thống kê hệ thống
- **Sao lưu và khôi phục** dữ liệu

## 🛠️ Công nghệ sử dụng

### Backend
- **ASP.NET Core 9.0** - Framework web hiện đại
- **Entity Framework Core 9.0** - ORM cho .NET
- **ASP.NET Core Identity** - Quản lý xác thực và phân quyền
- **SQL Server** - Cơ sở dữ liệu quan hệ

### Frontend
- **Razor Pages** - Server-side rendering
- **Bootstrap 5.3** - CSS framework responsive
- **jQuery** - JavaScript library
- **Font Awesome** - Icon library
- **Animate.css** - CSS animations

### Thư viện bổ sung
- **EPPlus** - Xuất file Excel
- **AutoMapper** - Object mapping
- **FluentValidation** - Validation framework

## 📋 Yêu cầu hệ thống

- **.NET 9.0 SDK** hoặc mới hơn
- **SQL Server 2019** hoặc mới hơn (hoặc SQL Server Express)
- **Visual Studio 2022** hoặc **VS Code** (khuyến nghị)
- **IIS Express** (đã bao gồm trong Visual Studio)

## 🚀 Hướng dẫn cài đặt

### 1. Clone repository
```bash
git clone https://github.com/coderkhongodo/Student_management.git
cd Student_management
```

### 2. Cấu hình cơ sở dữ liệu
Mở file `appsettings.json` và cập nhật connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Cài đặt dependencies
```bash
cd StudentManagementSystem
dotnet restore
```

### 4. Tạo và cập nhật database
```bash
dotnet ef database update
```

### 5. Chạy ứng dụng
```bash
dotnet run
```

Truy cập: `https://localhost:5001` hoặc `http://localhost:5000`

## 👥 Tài khoản mặc định

Hệ thống sẽ tự động tạo các tài khoản mặc định khi khởi chạy lần đầu:

### Quản trị viên
- **Email:** admin@sms.com
- **Mật khẩu:** Admin@123

### Giảng viên
- **Email:** teacher@sms.com  
- **Mật khẩu:** Teacher@123

### Sinh viên
- **Email:** student@sms.com
- **Mật khẩu:** Student@123

## 📁 Cấu trúc dự án

```
StudentManagementSystem/
├── Controllers/           # API Controllers
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── StudentController.cs
│   └── TeacherController.cs
├── Models/               # Data Models
│   ├── ApplicationUser.cs
│   ├── Class.cs
│   ├── Exam.cs
│   ├── Grade.cs
│   └── ...
├── ViewModels/           # View Models
├── Views/               # Razor Views
│   ├── Admin/
│   ├── Student/
│   ├── Teacher/
│   └── Shared/
├── Services/            # Business Logic
│   ├── AutoGradingService.cs
│   ├── GradeCalculationService.cs
│   └── DbInitializer.cs
├── Data/               # Database Context
├── wwwroot/           # Static Files
│   ├── css/
│   ├── js/
│   └── uploads/
└── Migrations/        # EF Migrations
```

## 🎨 Giao diện người dùng

### Dashboard Sinh viên
- 📊 Thống kê học tập với biểu đồ trực quan
- 🎯 Cards hiển thị điểm số, bài thi, và tiến độ
- 📱 Giao diện responsive, thân thiện với mobile
- 🎨 Theme màu xanh nhạt (#a8e6cf, #7fcdcd, #81c784)
- ✨ Animations mượt mà với Animate.css

### Dashboard Giảng viên
- 👥 Quản lý lớp học và sinh viên
- 📝 Tạo bài kiểm tra với nhiều loại câu hỏi
- ⚡ Chấm điểm tự động và thống kê chi tiết
- 📈 Biểu đồ tiến độ học tập của sinh viên

### Panel Quản trị
- 🔧 Quản lý toàn bộ hệ thống
- 📊 Báo cáo và thống kê tổng quan
- ⚙️ Cấu hình hệ thống linh hoạt
- 👤 Quản lý người dùng và phân quyền

## 📸 Screenshots

### Student Dashboard
```
┌─────────────────────────────────────────────────────────────┐
│  🎓 Chào mừng Sinh viên! 👨‍🎓                    📅 22/07/2025 │
│  Hôm nay là ngày tuyệt vời để học tập và phát triển bản thân  │
├─────────────────────────────────────────────────────────────┤
│  [0]      [0]        [1]        [0]        [0.00]          │
│ MÔN RỚT  BÀI THI   ĐÃ HOÀN    CHỜ KẾT    GPA (THANG       │
│         CÓ SẴN     THÀNH       QUẢ        ĐIỂM 4)          │
└─────────────────────────────────────────────────────────────┘
```

### Teacher Dashboard
```
┌─────────────────────────────────────────────────────────────┐
│  👨‍🏫 Chào mừng Giảng viên! 📚                  📅 22/07/2025 │
│  Hôm nay là ngày tuyệt vời để truyền đạt kiến thức          │
├─────────────────────────────────────────────────────────────┤
│  [5]      [120]      [15]       [8]        [3]            │
│ LỚP HỌC  SINH VIÊN  BUỔI HỌC  BÀI KIỂM   LỊCH THI         │
│                                  TRA                        │
└─────────────────────────────────────────────────────────────┘
```

## 🎥 Demo Video

> **Lưu ý:** Để xem demo đầy đủ, vui lòng clone repository và chạy ứng dụng locally.

### Tính năng chính được demo:
1. **Đăng nhập/Đăng ký** với các vai trò khác nhau
2. **Student Dashboard** - Xem điểm, lịch thi, nộp bài
3. **Teacher Dashboard** - Tạo bài kiểm tra, chấm điểm
4. **Admin Panel** - Quản lý người dùng, báo cáo
5. **Responsive Design** - Hoạt động tốt trên mobile

## 🔧 Tính năng nâng cao

### Chấm điểm tự động
- Hỗ trợ nhiều loại câu hỏi (trắc nghiệm, tự luận)
- Chấm điểm ngay lập tức cho câu hỏi trắc nghiệm
- Lưu trữ lịch sử chấm điểm
- Tính toán GPA tự động theo thang điểm 4.0

### Quản lý file
- Upload và download file bài tập
- Hỗ trợ nhiều định dạng file (PDF, DOC, DOCX, TXT)
- Kiểm tra kích thước và bảo mật
- Lưu trữ file an toàn trên server

### Báo cáo và thống kê
- Xuất báo cáo Excel với EPPlus
- Thống kê theo thời gian thực
- Biểu đồ và dashboard trực quan
- Phân tích xu hướng học tập

### Bảo mật
- Xác thực đa lớp với ASP.NET Core Identity
- Phân quyền chi tiết theo vai trò
- Mã hóa mật khẩu và bảo vệ session
- Validation dữ liệu đầu vào

## 🗄️ Database Schema

### Bảng chính
- **AspNetUsers** - Thông tin người dùng (kế thừa IdentityUser)
- **Classes** - Lớp học
- **Subjects** - Môn học
- **Exams** - Bài kiểm tra
- **ExamSchedules** - Lịch thi
- **Submissions** - Bài nộp của sinh viên
- **Grades** - Điểm số
- **Attendance** - Điểm danh
- **AttendanceSessions** - Buổi học

### Quan hệ dữ liệu
- **One-to-Many**: User → Classes, Class → Students
- **Many-to-Many**: Students ↔ Classes, Classes ↔ Subjects
- **One-to-One**: Submission → Grade

## 🔌 API Endpoints

### Authentication
```
POST /Account/Login          # Đăng nhập
POST /Account/Register       # Đăng ký
POST /Account/Logout         # Đăng xuất
```

### Student APIs
```
GET  /Student/Dashboard      # Dashboard sinh viên
GET  /Student/MyGrades       # Xem điểm
GET  /Student/MySchedule     # Lịch học/thi
POST /Student/SubmitExam     # Nộp bài thi
```

### Teacher APIs
```
GET  /Teacher/Dashboard      # Dashboard giảng viên
GET  /Teacher/MyClasses      # Lớp học của tôi
POST /Teacher/CreateExam     # Tạo bài kiểm tra
POST /Teacher/GradeSubmission # Chấm điểm
```

### Admin APIs
```
GET  /Admin/Dashboard        # Dashboard admin
GET  /Admin/Users            # Quản lý người dùng
POST /Admin/CreateUser       # Tạo người dùng mới
GET  /Admin/Reports          # Báo cáo hệ thống
```

## 🔧 Troubleshooting

### Lỗi thường gặp

#### 1. Lỗi kết nối Database
```bash
# Kiểm tra SQL Server đang chạy
services.msc → SQL Server (MSSQLSERVER)

# Hoặc sử dụng LocalDB
dotnet ef database update --connection "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDB;Trusted_Connection=true"
```

#### 2. Lỗi Migration
```bash
# Xóa database và tạo lại
dotnet ef database drop
dotnet ef database update
```

#### 3. Lỗi Dependencies
```bash
# Làm sạch và restore
dotnet clean
dotnet restore
dotnet build
```

#### 4. Lỗi Port đã được sử dụng
```bash
# Thay đổi port trong launchSettings.json
"applicationUrl": "https://localhost:5001;http://localhost:5000"
```

### FAQ

**Q: Làm sao để thêm sinh viên mới?**
A: Đăng nhập với tài khoản Admin → Users → Create New User → Chọn role "Student"

**Q: Làm sao để tạo bài kiểm tra?**
A: Đăng nhập với tài khoản Teacher → Dashboard → Tạo bài kiểm tra mới

**Q: Có thể thay đổi theme màu sắc không?**
A: Có, chỉnh sửa file `wwwroot/css/student-ui.css` và `wwwroot/css/site.css`

**Q: Hệ thống có hỗ trợ tiếng Việt không?**
A: Có, giao diện đã được localize hoàn toàn sang tiếng Việt

**Q: Có thể deploy lên cloud không?**
A: Có, hỗ trợ deploy lên Azure, AWS, hoặc bất kỳ hosting nào hỗ trợ .NET Core

## 🚀 Deployment

### Deploy lên Azure
1. Tạo Azure App Service
2. Cấu hình Connection String trong Azure Portal
3. Deploy từ Visual Studio hoặc GitHub Actions

### Deploy lên IIS
1. Publish project: `dotnet publish -c Release`
2. Copy files đến IIS folder
3. Cấu hình Application Pool (.NET Core)
4. Cập nhật connection string trong `appsettings.json`

### Docker (Optional)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["StudentManagementSystem.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentManagementSystem.dll"]
```

## 🤝 Đóng góp

Chúng tôi hoan nghênh mọi đóng góp! Vui lòng:

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

### Coding Standards
- Sử dụng C# naming conventions
- Viết unit tests cho các tính năng mới
- Cập nhật documentation khi cần thiết
- Tuân thủ SOLID principles

## 🗺️ Roadmap

### Version 1.1.0 (Planned)
- [ ] 📱 Mobile App (React Native/Flutter)
- [ ] 🔔 Real-time notifications với SignalR
- [ ] 📊 Advanced Analytics Dashboard
- [ ] 🌐 Multi-language support (English, Vietnamese)
- [ ] 📧 Email notifications system

### Version 1.2.0 (Future)
- [ ] 🎥 Video conferencing integration
- [ ] 📚 Digital library management
- [ ] 🤖 AI-powered grading suggestions
- [ ] 📱 Progressive Web App (PWA)
- [ ] 🔐 Two-factor authentication (2FA)

### Version 2.0.0 (Long-term)
- [ ] 🏗️ Microservices architecture
- [ ] ☁️ Cloud-native deployment
- [ ] 📊 Machine Learning analytics
- [ ] 🌍 Multi-tenant support
- [ ] 🔄 Real-time collaboration tools

## 🏆 Features Highlights

### ✅ Đã hoàn thành
- ✅ User Authentication & Authorization
- ✅ Student Dashboard với statistics
- ✅ Teacher Dashboard với class management
- ✅ Admin Panel với user management
- ✅ Online Exam System
- ✅ Automatic Grading
- ✅ File Upload/Download
- ✅ Responsive UI Design
- ✅ Grade Calculation (GPA)
- ✅ Attendance Tracking

### 🚧 Đang phát triển
- 🚧 Advanced Reporting System
- 🚧 Email Notification System
- 🚧 Mobile Responsive Improvements
- 🚧 Performance Optimizations

## 📈 Statistics

- **Lines of Code:** ~15,000+
- **Controllers:** 5
- **Models:** 10+
- **Views:** 30+
- **Database Tables:** 12
- **Supported Languages:** Vietnamese, English (partial)

## 📝 License

Dự án này được phân phối dưới giấy phép MIT. Xem file `LICENSE` để biết thêm chi tiết.

## 📞 Liên hệ

- **GitHub:** [@coderkhongodo](https://github.com/coderkhongodo)
- **Email:** coderkhongodo@gmail.com
- **LinkedIn:** [Huỳnh Lý Tân Khoa](https://linkedin.com/in/coderkhongodo)

## 🙏 Lời cảm ơn

Cảm ơn tất cả những người đã đóng góp vào dự án này và cộng đồng open source .NET!

### Công nghệ và thư viện được sử dụng:
- Microsoft .NET Team cho ASP.NET Core
- Entity Framework Team
- Bootstrap Team
- Font Awesome
- Animate.css
- EPPlus Library

---

⭐ **Nếu dự án này hữu ích, hãy cho chúng tôi một star!** ⭐

**Made with ❤️ by [coderkhongodo](https://github.com/coderkhongodo)**
