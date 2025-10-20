# demo1
我自己的测试项目

## 壁纸网站用户系统与互动功能

这是一个完整的壁纸网站后端API，使用ASP.NET Core 8.0开发，包含用户系统和互动功能。

### 主要功能

1. **用户管理**
   - 用户注册、登录
   - 密码找回和重置
   - 个人资料管理
   - 用户个人中心（上传记录、收藏、评论）

2. **壁纸管理**
   - 用户上传壁纸
   - 图片格式验证和安全检查
   - 自动生成缩略图
   - 后台管理员审核机制

3. **互动功能**
   - 点赞/取消点赞
   - 收藏/取消收藏
   - 评论功能（发表、编辑、删除）

4. **认证与授权**
   - JWT令牌认证
   - 基于角色的授权（用户/管理员）

### 技术栈

- ASP.NET Core 8.0 Web API
- Entity Framework Core
- SQL Server
- JWT身份验证
- BCrypt密码加密
- SixLabors.ImageSharp图片处理

### 快速开始

详细文档请参见 [WallpaperApi/README.md](./WallpaperApi/README.md)

```bash
cd WallpaperApi
dotnet restore
dotnet ef database update
dotnet run
```

### API文档

项目启动后，访问 `https://localhost:7xxx/swagger` 查看完整API文档。

