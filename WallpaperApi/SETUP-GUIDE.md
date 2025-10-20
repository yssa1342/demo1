# 开发环境设置指南

本指南将帮助您设置和运行壁纸网站 API。

## 前提条件

在开始之前，请确保您的系统已安装以下软件：

1. **.NET 8.0 SDK**
   - 下载地址：https://dotnet.microsoft.com/download/dotnet/8.0
   - 验证安装：`dotnet --version`

2. **SQL Server** (选择其一)
   - SQL Server LocalDB (Windows 开发推荐)
   - SQL Server Express
   - SQL Server Developer Edition
   - Docker 中的 SQL Server

3. **代码编辑器** (推荐)
   - Visual Studio 2022
   - Visual Studio Code + C# 扩展
   - JetBrains Rider

4. **API 测试工具** (可选)
   - Postman
   - Thunder Client (VS Code 扩展)
   - REST Client (VS Code 扩展)

## 快速开始

### 1. 克隆项目

```bash
git clone <repository-url>
cd demo1/WallpaperApi
```

### 2. 配置数据库连接

编辑 `appsettings.json` 或 `appsettings.Development.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WallpaperDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**不同环境的连接字符串示例：**

- **LocalDB (Windows)**:
  ```
  Server=(localdb)\\mssqllocaldb;Database=WallpaperDb;Trusted_Connection=true;MultipleActiveResultSets=true
  ```

- **SQL Server Express**:
  ```
  Server=localhost\\SQLEXPRESS;Database=WallpaperDb;Trusted_Connection=true;MultipleActiveResultSets=true
  ```

- **SQL Server with Username/Password**:
  ```
  Server=localhost;Database=WallpaperDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true;MultipleActiveResultSets=true
  ```

- **Docker SQL Server**:
  ```
  Server=localhost,1433;Database=WallpaperDb;User Id=sa;Password=YourStrong@Password;TrustServerCertificate=true;MultipleActiveResultSets=true
  ```

### 3. 安装依赖

```bash
dotnet restore
```

### 4. 安装 EF Core 工具（如果尚未安装）

```bash
dotnet tool install --global dotnet-ef
```

### 5. 创建数据库

```bash
dotnet ef database update
```

此命令将：
- 创建数据库（如果不存在）
- 运行所有迁移
- 创建所有表和关系

### 6. 运行应用

```bash
dotnet run
```

或者使用 watch 模式（自动重新加载）：

```bash
dotnet watch run
```

应用将在以下地址启动：
- HTTPS: https://localhost:7001
- HTTP: http://localhost:5001

### 7. 测试 API

访问 Swagger UI：
```
https://localhost:7001/swagger
```

或使用提供的测试工具：

**使用 Postman**:
1. 导入 `Wallpaper-API.postman_collection.json`
2. 更新环境变量 `baseUrl` 为您的 API 地址
3. 开始测试

**使用 Bash 脚本**:
```bash
chmod +x test-api.sh
./test-api.sh
```

**使用演示 HTML 页面**:
1. 确保 API 正在运行
2. 在浏览器中打开 `demo.html`
3. 更新页面中的 `API_BASE` 变量为您的 API 地址

## 使用 Docker 运行

### 选项 1: 仅运行 SQL Server

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Password123" \
   -p 1433:1433 --name sql-server \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

然后更新连接字符串并运行 API：
```bash
dotnet ef database update
dotnet run
```

### 选项 2: 使用 Docker Compose 运行完整堆栈

```bash
docker-compose up -d
```

这将启动：
- API 服务在 http://localhost:5000
- SQL Server 在 localhost:1433

## 创建管理员用户

1. 首先通过 API 注册一个用户
2. 连接到数据库
3. 执行以下 SQL 命令：

```sql
UPDATE Users 
SET IsAdmin = 1 
WHERE Username = 'your-username';
```

## 常见问题

### 数据库连接失败

**问题**: `A network-related or instance-specific error occurred`

**解决方案**:
1. 确认 SQL Server 正在运行
2. 检查连接字符串是否正确
3. 确保 SQL Server 允许 TCP/IP 连接
4. 检查防火墙设置

### EF Core 迁移错误

**问题**: `Unable to create an object of type 'WallpaperDbContext'`

**解决方案**:
```bash
# 删除现有迁移
rm -rf Migrations/

# 重新创建迁移
dotnet ef migrations add InitialCreate

# 更新数据库
dotnet ef database update
```

### HTTPS 证书问题

**问题**: `The SSL connection could not be established`

**解决方案**:
```bash
# 信任开发证书
dotnet dev-certs https --trust
```

### 文件上传失败

**问题**: 上传图片时出现错误

**解决方案**:
1. 确保 `wwwroot/uploads/wallpapers` 和 `wwwroot/uploads/thumbnails` 目录存在
2. 检查目录权限
3. 检查文件大小限制（默认 10MB）

## 开发工作流

### 1. 添加新功能

1. 创建分支：`git checkout -b feature/new-feature`
2. 进行更改
3. 测试功能
4. 提交更改：`git commit -m "Add new feature"`
5. 推送分支：`git push origin feature/new-feature`

### 2. 数据库更改

当修改模型后：

```bash
# 创建新迁移
dotnet ef migrations add YourMigrationName

# 查看将要执行的 SQL
dotnet ef migrations script

# 应用迁移
dotnet ef database update
```

### 3. 回滚迁移

```bash
# 回滚到特定迁移
dotnet ef database update PreviousMigrationName

# 移除最后一个迁移
dotnet ef migrations remove
```

## 生产部署

### 准备清单

- [ ] 更改 JWT Secret 为强密码
- [ ] 使用生产数据库连接字符串
- [ ] 配置 HTTPS
- [ ] 设置 CORS 策略
- [ ] 配置日志记录
- [ ] 设置环境变量
- [ ] 配置反向代理（Nginx/IIS）
- [ ] 实施速率限制
- [ ] 启用应用程序监控
- [ ] 配置自动备份

### 部署到 Azure

```bash
# 发布应用
dotnet publish -c Release -o ./publish

# 使用 Azure CLI 部署
az webapp up --name your-app-name --resource-group your-resource-group
```

### 部署到 Linux 服务器

1. 发布应用：
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. 复制到服务器：
   ```bash
   scp -r ./publish user@server:/var/www/wallpaper-api
   ```

3. 配置 systemd 服务（参见生产文档）

## 性能优化

### 数据库索引

已在模型中配置：
- Users: Username, Email (唯一索引)
- Likes: UserId + WallpaperId (唯一索引)
- Favorites: UserId + WallpaperId (唯一索引)

### 缓存

考虑添加：
- Redis 用于会话管理
- 响应缓存中间件
- 内存缓存用于频繁访问的数据

### 图片优化

- 使用 CDN 存储和提供图片
- 实施图片压缩
- 使用 WebP 格式
- 实施懒加载

## 安全最佳实践

1. **永远不要提交敏感信息**
   - JWT Secret
   - 数据库密码
   - API 密钥

2. **使用环境变量**
   ```bash
   export Jwt__Secret="your-secret-key"
   export ConnectionStrings__DefaultConnection="your-connection-string"
   ```

3. **定期更新依赖**
   ```bash
   dotnet list package --outdated
   dotnet add package PackageName --version x.x.x
   ```

4. **实施速率限制**
   - 考虑使用 AspNetCoreRateLimit 包

5. **启用 HTTPS**
   - 在生产环境中始终使用 HTTPS
   - 配置 HSTS

## 监控和日志

### 日志配置

在 `appsettings.json` 中：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

### 推荐的监控工具

- Application Insights (Azure)
- Serilog
- ELK Stack
- Prometheus + Grafana

## 获取帮助

- 查看 [README.md](README.md) 了解 API 文档
- 查看 Swagger UI 了解端点详细信息
- 检查日志文件以排查问题
- 在 GitHub 上提交 issue

## 贡献指南

1. Fork 项目
2. 创建功能分支
3. 提交更改
4. 推送到分支
5. 创建 Pull Request

## 许可证

本项目用于教育目的。
