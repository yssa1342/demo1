# Wallpaper Website API

A comprehensive ASP.NET Core Web API for a wallpaper website with user system and interactive features.

## Features

### User Management
- User registration and login with JWT authentication
- Password recovery and reset functionality
- User profile management
- User personal center with uploaded wallpapers, favorites, and comments

### Wallpaper Management
- Upload wallpapers with automatic thumbnail generation
- Image validation and security checks
- Admin approval system for user-uploaded wallpapers
- Browse wallpapers by category
- View wallpaper details with statistics

### Interactive Features
- Like/unlike wallpapers
- Favorite/unfavorite wallpapers
- Comment on wallpapers
- Edit and delete own comments
- Admin can delete any comment

### Authorization
- JWT-based authentication
- Role-based authorization (User/Admin)
- Protected endpoints for authenticated users only

## Technology Stack

- **Framework**: ASP.NET Core 8.0 Web API
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Password Hashing**: BCrypt
- **Image Processing**: SixLabors.ImageSharp
- **ORM**: Entity Framework Core

## Database Schema

### Tables
- **Users**: User accounts and profiles
- **Wallpapers**: Wallpaper information and metadata
- **Comments**: User comments on wallpapers
- **Likes**: User likes on wallpapers
- **Favorites**: User favorites/bookmarks

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full SQL Server)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd WallpaperApi
   ```

2. **Update connection string**
   Edit `appsettings.json` and update the connection string if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WallpaperDb;Trusted_Connection=true;MultipleActiveResultSets=true"
   }
   ```

3. **Update JWT Secret**
   In production, change the JWT secret in `appsettings.json`:
   ```json
   "Jwt": {
     "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm"
   }
   ```

4. **Install dependencies**
   ```bash
   dotnet restore
   ```

5. **Create database and run migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

6. **Run the application**
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:7xxx` and `http://localhost:5xxx` (ports may vary).

## API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password with token
- `GET /api/auth/me` - Get current user info (requires authentication)

### User (`/api/user`)
All endpoints require authentication unless specified.

- `GET /api/user/{userId}` - Get user profile
- `PUT /api/user/profile` - Update own profile
- `POST /api/user/change-password` - Change password
- `GET /api/user/{userId}/wallpapers` - Get user's uploaded wallpapers
- `GET /api/user/{userId}/favorites` - Get user's favorite wallpapers
- `GET /api/user/{userId}/comments` - Get user's comments

### Wallpaper (`/api/wallpaper`)
- `GET /api/wallpaper` - Get all approved wallpapers (public)
  - Query params: `page`, `pageSize`, `category`
- `GET /api/wallpaper/{wallpaperId}` - Get wallpaper details (public)
- `POST /api/wallpaper` - Upload wallpaper (requires authentication)
- `PUT /api/wallpaper/{wallpaperId}` - Update wallpaper (requires authentication, owner only)
- `DELETE /api/wallpaper/{wallpaperId}` - Delete wallpaper (requires authentication, owner or admin)
- `GET /api/wallpaper/pending` - Get pending approval wallpapers (admin only)
- `POST /api/wallpaper/{wallpaperId}/approve` - Approve/reject wallpaper (admin only)

### Interaction (`/api/interaction`)
All endpoints require authentication unless specified.

- `POST /api/interaction/wallpapers/{wallpaperId}/like` - Like wallpaper
- `DELETE /api/interaction/wallpapers/{wallpaperId}/like` - Unlike wallpaper
- `GET /api/interaction/wallpapers/{wallpaperId}/like` - Check if wallpaper is liked
- `POST /api/interaction/wallpapers/{wallpaperId}/favorite` - Favorite wallpaper
- `DELETE /api/interaction/wallpapers/{wallpaperId}/favorite` - Unfavorite wallpaper
- `GET /api/interaction/wallpapers/{wallpaperId}/favorite` - Check if wallpaper is favorited
- `GET /api/interaction/wallpapers/{wallpaperId}/comments` - Get wallpaper comments (public)
- `POST /api/interaction/wallpapers/{wallpaperId}/comments` - Create comment
- `PUT /api/interaction/comments/{commentId}` - Update comment (owner only)
- `DELETE /api/interaction/comments/{commentId}` - Delete comment (owner or admin)

## Request/Response Examples

### Register User
```bash
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "Password123!",
  "displayName": "John Doe"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "displayName": "John Doe",
    "isAdmin": false,
    "createdAt": "2025-10-20T07:00:00Z"
  }
}
```

### Upload Wallpaper
```bash
POST /api/wallpaper
Authorization: Bearer <your-jwt-token>
Content-Type: multipart/form-data

title: "Beautiful Sunset"
description: "A stunning sunset over the ocean"
category: "Nature"
tags: "sunset,ocean,nature"
image: <file>
```

### Like Wallpaper
```bash
POST /api/interaction/wallpapers/1/like
Authorization: Bearer <your-jwt-token>
```

## Security Features

- Password hashing with BCrypt
- JWT token-based authentication
- Role-based authorization
- File upload validation (size, format)
- SQL injection protection via EF Core
- CORS configuration

## File Upload

- **Supported formats**: JPG, JPEG, PNG, WEBP
- **Maximum file size**: 10MB
- **Automatic thumbnail generation**: 400x400px max
- **Upload directory**: `wwwroot/uploads/wallpapers/`
- **Thumbnail directory**: `wwwroot/uploads/thumbnails/`

## Admin Features

To create an admin user, you need to manually update the `IsAdmin` field in the database for a user:

```sql
UPDATE Users SET IsAdmin = 1 WHERE Username = 'admin';
```

Admin users can:
- Approve or reject uploaded wallpapers
- Delete any wallpaper
- Delete any comment
- Access pending wallpapers list

## Development

### Creating Migrations
```bash
dotnet ef migrations add MigrationName
```

### Updating Database
```bash
dotnet ef database update
```

### Running in Development
```bash
dotnet watch run
```

## Production Considerations

1. **Change JWT Secret**: Use a strong, unique secret key
2. **Use HTTPS**: Configure SSL/TLS certificates
3. **Update CORS**: Restrict origins to your frontend domain
4. **Database Connection**: Use production SQL Server with proper credentials
5. **File Storage**: Consider using cloud storage (Azure Blob, AWS S3) instead of local files
6. **Email Service**: Implement email service for password reset
7. **Logging**: Configure proper logging and monitoring
8. **Rate Limiting**: Add rate limiting to prevent abuse
9. **Image Optimization**: Optimize images before storing

## Future Enhancements

- User avatar upload
- Wallpaper categories management
- Search functionality
- Leaderboards and rankings
- Membership/subscription system
- Wallpaper collections
- Social features (following users)
- Wallpaper downloads tracking
- Advanced filtering and sorting

## License

This project is for educational purposes.
