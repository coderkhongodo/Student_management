# 🎓 Student Management System

> **Modern, Responsive, and Accessible Student Management System built with ASP.NET Core MVC**

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple.svg)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![WCAG](https://img.shields.io/badge/WCAG-2.1%20AA-brightgreen.svg)](https://www.w3.org/WAI/WCAG21/quickref/)
[![GitHub stars](https://img.shields.io/github/stars/coderkhongodo/Student_management.svg)](https://github.com/coderkhongodo/Student_management/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/coderkhongodo/Student_management.svg)](https://github.com/coderkhongodo/Student_management/network)

A comprehensive student management system featuring **modern UI/UX design**, **full responsiveness**, **accessibility compliance**, and **robust performance optimization**.

## ✨ **Key Features**

### 🎯 **Modern Design System**
- **Unified Color Palette**: Consistent #a8e6cf, #7fcdcd, #81c784 theme
- **Responsive Design**: Mobile-first approach with breakpoints for all devices
- **Accessibility**: WCAG 2.1 AA compliant with keyboard navigation and screen reader support
- **Micro-interactions**: Smooth animations and hover effects for enhanced UX

### 👨‍🎓 **Student Portal**
- **Interactive Dashboard**: Real-time updates with modern card layouts
- **Exam Taking System**: Intuitive interface with auto-save and timer
- **Grade Analytics**: Visual charts and performance tracking
- **Mobile-Optimized**: Touch-friendly interface for mobile devices

### 👨‍🏫 **Teacher Dashboard**
- **Class Management**: Streamlined interface for managing multiple classes
- **Exam Creation**: Modern form design with comprehensive options
- **Grading System**: Bulk operations and automated grading features
- **Analytics**: Comprehensive reporting and data visualization

### 👨‍💼 **Admin Panel**
- **User Management**: Advanced CRUD operations with search and filtering
- **System Analytics**: Real-time dashboard with key performance indicators
- **Bulk Operations**: Efficient management of large datasets
- **Security Controls**: Role-based access with audit trails

## 🚀 **Technology Stack**

### **Backend**
- **ASP.NET Core 8.0 MVC** - Modern web framework
- **Entity Framework Core** - ORM with optimized queries
- **SQL Server** - Robust database with performance tuning
- **ASP.NET Core Identity** - Secure authentication system

### **Frontend**
- **Bootstrap 5.3** - Responsive CSS framework
- **Custom Design System** - Unified component library
- **Vanilla JavaScript** - Performance-optimized interactions
- **Font Awesome 6** - Modern icon library

### **Performance & Optimization**
- **Service Worker** - Intelligent caching strategy
- **CSS/JS Minification** - Optimized asset delivery
- **Database Optimization** - Query caching and indexing
- **CDN Integration** - Fast global content delivery

### **Accessibility & UX**
- **WCAG 2.1 AA Compliance** - Full accessibility support
- **Keyboard Navigation** - Complete keyboard accessibility
- **Screen Reader Support** - ARIA labels and semantic HTML
- **Cross-browser Compatibility** - Chrome, Firefox, Safari, Edge

## 📱 **Responsive Design**

| Device | Breakpoint | Optimization |
|--------|------------|--------------|
| Mobile | < 576px | Touch-friendly, single column |
| Tablet | 576px - 991px | Two-column layouts, expanded menus |
| Desktop | 992px+ | Multi-column, hover interactions |

## 🎨 **Design System**

### **Color Palette**
```css
--color-primary: #7fcdcd;      /* Primary brand color */
--color-primary-light: #a8e6cf; /* Light accent */
--color-primary-dark: #81c784;  /* Dark accent */
--color-success: #28a745;       /* Success states */
--color-danger: #dc3545;        /* Error states */
--color-warning: #ffc107;       /* Warning states */
```

### **Typography**
- **Font Family**: Inter, system fonts
- **Scale**: 8px base unit with consistent spacing
- **Weights**: 300, 400, 500, 600, 700

### **Components**
- **Cards**: Modern shadows with hover effects
- **Buttons**: Gradient backgrounds with ripple effects
- **Forms**: Enhanced validation with real-time feedback
- **Tables**: Responsive with sorting and filtering

## 🔧 **Installation & Setup**

### **Prerequisites**
- .NET 8.0 SDK
- SQL Server 2019+ or LocalDB
- Visual Studio 2022 or VS Code

### **Quick Start**
```bash
# Clone the repository
git clone https://github.com/coderkhongodo/Student_management.git
cd Student_management/StudentManagementSystem

# Restore dependencies
dotnet restore

# Update database
dotnet ef database update

# Run the application
dotnet run
```

### **Environment Configuration**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDB;Trusted_Connection=true"
  }
}
```

## 👥 **Default Accounts**

| Role | Email | Password | Features |
|------|-------|----------|----------|
| **Admin** | admin@sms.com | Admin@123 | Full system access |
| **Teacher** | teacher@sms.com | Teacher@123 | Class & exam management |
| **Student** | student@sms.com | Student@123 | Exam taking & grades |

## 📊 **Performance Metrics**

### **Core Web Vitals**
- **First Contentful Paint**: < 1.5s
- **Largest Contentful Paint**: < 2.5s
- **Cumulative Layout Shift**: < 0.1
- **First Input Delay**: < 100ms

### **Accessibility Score**
- **WCAG 2.1 AA**: 100% compliant
- **Keyboard Navigation**: Full support
- **Screen Reader**: Complete compatibility
- **Color Contrast**: 4.5:1 minimum ratio

## 🔒 **Security Features**

### **Authentication & Authorization**
- **JWT Tokens** with secure secret rotation
- **Role-based Access Control** (RBAC)
- **Password Policies** with complexity requirements
- **Session Management** with timeout controls

### **Data Protection**
- **HTTPS Enforcement** with HSTS headers
- **Input Validation** with sanitization
- **SQL Injection Prevention** via parameterized queries
- **CSRF Protection** with anti-forgery tokens

### **Security Headers**
```
Strict-Transport-Security: max-age=31536000
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Content-Security-Policy: default-src 'self'
```

## 📈 **Database Schema**

### **Core Entities**
```mermaid
erDiagram
    User ||--o{ Class : teaches
    User ||--o{ Submission : submits
    Class ||--o{ ExamSchedule : has
    ExamSchedule ||--o{ Submission : receives
    Submission ||--|| Grade : graded
```

### **Key Tables**
- **AspNetUsers**: User accounts with role management
- **Classes**: Course classes with teacher assignments
- **Exams**: Exam templates with questions
- **ExamSchedules**: Scheduled exam instances
- **Submissions**: Student exam submissions
- **Grades**: Grading results with analytics

## 🧪 **Testing Strategy**

### **Unit Tests**
```bash
dotnet test --filter Category=Unit
```

### **Integration Tests**
```bash
dotnet test --filter Category=Integration
```

### **Performance Tests**
```bash
dotnet test --filter Category=Performance
```

### **Accessibility Tests**
- **axe-core** integration for automated testing
- **Manual testing** with screen readers
- **Keyboard navigation** verification

## 📚 **Documentation**

### **Design System Documentation**
- **Color Palette**: Complete color specifications
- **Typography**: Font scales and usage guidelines
- **Components**: Interactive component library
- **Responsive Design**: Breakpoint specifications

### **API Documentation**
- **REST Endpoints**: Complete API reference
- **Authentication**: JWT implementation guide
- **Error Handling**: Standardized error responses
- **Rate Limiting**: API usage guidelines

### **Deployment Guide**
- **Production Setup**: Complete deployment checklist
- **Security Configuration**: Security hardening guide
- **Performance Optimization**: Caching and CDN setup
- **Monitoring**: Logging and alerting configuration

## 🚀 **Deployment**

### **Production Checklist**
- [ ] SSL certificate configured
- [ ] Environment variables set
- [ ] Database migrations applied
- [ ] Security headers configured
- [ ] Monitoring setup complete
- [ ] Performance testing passed

### **Docker Support**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY . /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "StudentManagementSystem.dll"]
```

## 🤝 **Contributing**

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### **Development Workflow**
1. Fork the repository
2. Create a feature branch
3. Follow coding standards
4. Add tests for new features
5. Submit a pull request

### **Code Standards**
- **C# Conventions**: Microsoft coding standards
- **CSS/JS**: Consistent formatting with Prettier
- **Accessibility**: WCAG 2.1 AA compliance required
- **Performance**: Core Web Vitals targets must be met

## 📄 **License**

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 **Acknowledgments**

- **ASP.NET Core Team** for the excellent framework
- **Bootstrap Team** for the responsive CSS framework
- **Accessibility Community** for WCAG guidelines
- **Open Source Contributors** for various libraries used

## 📞 **Support**

- **Issues**: [GitHub Issues](https://github.com/coderkhongodo/Student_management/issues)
- **Discussions**: [GitHub Discussions](https://github.com/coderkhongodo/Student_management/discussions)
- **Email**: coderkhongodo@gmail.com

---

<div align="center">

**Built with ❤️ for education**

[Demo](http://localhost:5161) • [Documentation](StudentManagementSystem/DOCUMENTATION.md) • [Support](mailto:coderkhongodo@gmail.com)

</div>
