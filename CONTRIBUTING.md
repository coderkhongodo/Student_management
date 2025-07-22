# 🤝 Contributing to Student Management System

Cảm ơn bạn đã quan tâm đến việc đóng góp cho dự án Student Management System! 

## 📋 Quy trình đóng góp

### 1. Fork Repository
- Fork repository này về GitHub account của bạn
- Clone fork về máy local của bạn

```bash
git clone https://github.com/YOUR_USERNAME/Student_management.git
cd Student_management
```

### 2. Tạo Branch mới
```bash
git checkout -b feature/your-feature-name
# hoặc
git checkout -b bugfix/your-bugfix-name
```

### 3. Thực hiện thay đổi
- Viết code theo coding standards
- Thêm tests nếu cần thiết
- Cập nhật documentation

### 4. Commit và Push
```bash
git add .
git commit -m "Add: your descriptive commit message"
git push origin feature/your-feature-name
```

### 5. Tạo Pull Request
- Tạo Pull Request từ branch của bạn về main branch
- Mô tả chi tiết những thay đổi bạn đã thực hiện
- Liên kết với issue nếu có

## 📝 Coding Standards

### C# Code Style
- Sử dụng PascalCase cho class, method, property names
- Sử dụng camelCase cho local variables
- Sử dụng meaningful names
- Thêm XML documentation cho public methods

```csharp
/// <summary>
/// Calculates the GPA for a student
/// </summary>
/// <param name="studentId">The student's ID</param>
/// <returns>The calculated GPA</returns>
public decimal CalculateGPA(string studentId)
{
    // Implementation here
}
```

### Frontend Standards
- Sử dụng Bootstrap classes khi có thể
- Viết CSS responsive
- Sử dụng semantic HTML
- Optimize cho accessibility

### Database
- Sử dụng Entity Framework migrations
- Đặt tên table và column rõ ràng
- Thêm indexes khi cần thiết

## 🐛 Báo cáo Bug

### Trước khi báo cáo bug
- Kiểm tra xem bug đã được báo cáo chưa
- Đảm bảo bạn đang sử dụng version mới nhất

### Thông tin cần thiết
- **Mô tả bug:** Mô tả ngắn gọn về vấn đề
- **Steps to reproduce:** Các bước để tái hiện bug
- **Expected behavior:** Hành vi mong đợi
- **Actual behavior:** Hành vi thực tế
- **Environment:** OS, Browser, .NET version
- **Screenshots:** Nếu có thể

## 💡 Đề xuất tính năng

### Template cho Feature Request
```markdown
## Feature Description
Mô tả chi tiết tính năng bạn muốn đề xuất

## Use Case
Tại sao tính năng này hữu ích?

## Proposed Solution
Đề xuất cách implement

## Alternatives
Các giải pháp thay thế khác
```

## 🧪 Testing

### Unit Tests
- Viết unit tests cho business logic
- Sử dụng xUnit framework
- Aim for >80% code coverage

### Integration Tests
- Test các API endpoints
- Test database operations
- Test authentication/authorization

### Manual Testing
- Test trên nhiều browsers
- Test responsive design
- Test với different user roles

## 📚 Documentation

### Code Documentation
- Thêm XML comments cho public APIs
- Viết README cho modules phức tạp
- Document configuration settings

### User Documentation
- Cập nhật README.md khi cần
- Thêm screenshots cho features mới
- Viết user guides nếu cần

## 🔍 Code Review Process

### Reviewer Guidelines
- Kiểm tra code style và standards
- Verify functionality works as expected
- Check for security vulnerabilities
- Ensure tests are adequate

### Author Guidelines
- Respond to feedback constructively
- Make requested changes promptly
- Keep PRs focused and small

## 🚀 Release Process

### Version Numbering
- Sử dụng Semantic Versioning (MAJOR.MINOR.PATCH)
- MAJOR: Breaking changes
- MINOR: New features (backward compatible)
- PATCH: Bug fixes

### Release Checklist
- [ ] All tests passing
- [ ] Documentation updated
- [ ] Version number bumped
- [ ] Release notes written
- [ ] Database migrations tested

## 📞 Liên hệ

Nếu bạn có câu hỏi về việc đóng góp:
- Tạo issue trên GitHub
- Email: coderkhongodo@gmail.com
- Hoặc comment trực tiếp trong PR/Issue

## 🙏 Cảm ơn

Mọi đóng góp đều được đánh giá cao, từ việc báo cáo bug đến việc implement features mới!

---

**Happy Coding! 🎉**
