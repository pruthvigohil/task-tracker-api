# TaskTracker API

.NET 8 Web API backend for the Task Management System assessment.

## Tech Stack
- .NET 8 Web API
- Entity Framework Core 8
- MySQL 8 (Pomelo provider)
- JWT Authentication
- Google OAuth via Google.Apis.Auth

## Prerequisites
- .NET 8 SDK
- MySQL Server 8
- Visual Studio 2022 or VS Code

## Setup Instructions

### 1. Clone the repository
```bash
git clone https://github.com/YOUR_USERNAME/task-tracker-api.git
cd task-tracker-api
```

### 2. Configure appsettings.json
Copy the template and fill in your values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=tasktracker;User=root;Password=YOUR_PASSWORD;"
  },
  "Jwt": {
    "Key": "YourSecretKeyAtLeast32CharactersLong!",
    "Issuer": "TaskTrackerAPI",
    "Audience": "TaskTrackerClient"
  },
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID"
  }
}
```

### 3. Create and seed the database
```bash
dotnet ef database update
```

Then seed default users by running the app and calling: