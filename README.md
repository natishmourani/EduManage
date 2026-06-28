```
███████╗██████╗ ██╗   ██╗███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗
██╔════╝██╔══██╗██║   ██║████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝
█████╗  ██║  ██║██║   ██║██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  
██╔══╝  ██║  ██║██║   ██║██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  
███████╗██████╔╝╚██████╔╝██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗
╚══════╝╚═════╝  ╚═════╝ ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝
                          School Management System
```

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://docs.microsoft.com/aspnet/core/mvc/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Entity Framework](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://docs.microsoft.com/ef/core/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-Academic-blue?style=for-the-badge)](https://github.com/natishmourani/EduManage)

> 🎓 A full-featured, role-based school management portal — built for admins, teachers, and students.

</div>

---

## ✨ Features

### 🔐 Role-Based Access
Three fully separated portals, one unified system.

| Feature | 👑 Admin | 👨‍🏫 Teacher | 🎒 Student |
|---|:---:|:---:|:---:|
| Dashboard with live stats | ✅ | ❌ | ❌ |
| Manage students & teachers | ✅ | ❌ | ❌ |
| Auto-generate credentials | ✅ | ❌ | ❌ |
| Mark attendance | ✅ | ✅ | ❌ |
| View attendance history | ✅ | ✅ | ✅ |
| Create assignments | ❌ | ✅ | ❌ |
| Submit assignments | ❌ | ❌ | ✅ |
| Grade submissions | ❌ | ✅ | ❌ |
| Post announcements | ✅ | ✅ | ❌ |
| Create school events | ✅ | ❌ | ❌ |
| View grades & feedback | ❌ | ❌ | ✅ |

---

## 🛠️ Tech Stack

| Layer | 🔧 Technology | 📦 Version |
|---|---|---|
| 🖥️ Framework | ASP.NET Core MVC | .NET 8.0 |
| 🗄️ ORM | Entity Framework Core | 8.0.0 |
| 💾 Database | Microsoft SQL Server | 2022 |
| 🎨 Frontend | Razor Views, HTML, CSS | — |
| 🔑 Auth | Session-based (HttpContext) | — |
| 📁 File Storage | Local (`wwwroot/uploads/`) | — |

---

## 📁 Project Structure

```
EduManage/
│
├── 🎮 Controllers/
│   ├── AdminController.cs          ← Student & teacher management
│   ├── AnnouncementController.cs   ← Role-filtered announcements
│   ├── AssignmentController.cs     ← Create, submit & grade
│   ├── AttendanceController.cs     ← Mark, history & reports
│   ├── EventController.cs          ← School events
│   └── HomeController.cs           ← Login & landing page
│
├── 🧠 Models/                      ← Student, Teacher, Assignment, etc.
├── 🗃️ Data/                        ← ApplicationDbContext (EF Core)
├── 🛠️ Helpers/                     ← Shared utilities (StudentPortalHelper)
├── 🖼️ Views/                       ← Razor .cshtml templates
├── 🔄 Migrations/                  ← EF Core migration history
├── 🌐 wwwroot/                     ← Static files & uploaded content
│
├── 📜 EduManageDB.sql              ← Full DB schema script
├── 🌱 Insert_queries.sql           ← Seed data for testing
├── ⚙️ Program.cs                   ← App startup & DI configuration
└── 🔧 appsettings.json             ← Connection strings & settings
```

---

## 🚀 Getting Started

### Prerequisites

Make sure you have the following installed:

- [![.NET 8](https://img.shields.io/badge/.NET_8_SDK-Download-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8)
- [![SQL Server](https://img.shields.io/badge/SQL_Server-Download-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server/sql-server-downloads)
- [![Visual Studio](https://img.shields.io/badge/Visual_Studio_2022-Download-5C2D91?style=flat-square&logo=visualstudio)](https://visualstudio.microsoft.com/)

---

### ⚡ Quick Setup

**Step 1 — Clone the repo**
```bash
git clone https://github.com/natishmourani/EduManage.git
cd EduManage
```

**Step 2 — Set up the database**

Open SQL Server Management Studio and run:
```sql
-- 1. Create all tables, views, and constraints
-- Run: EduManageDB.sql

-- 2. (Optional) Load sample data
-- Run: Insert_queries.sql
```

**Step 3 — Configure your connection string**

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=EduManageDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Step 4 — Run it!**
```bash
dotnet restore
dotnet run
```

> 🌐 App starts at `https://localhost:5001`

---

## 🔑 Default Credentials

| Role | 👤 Username | 🔒 Password |
|---|---|---|
| 👑 Admin | *(set in DB seed)* | *(set in DB seed)* |
| 👨‍🏫 Teacher | *(auto-generated on creation)* | *(shown to admin on creation)* |
| 🎒 Student | *(auto-generated on creation)* | `pwd@student{roll_number}` |

> 💡 Full credentials for seeded test data are inside `Insert_queries.sql`.

---

## ⚙️ How It Works

```
Browser Request
      │
      ▼
 ASP.NET Router  ──►  Controller Action
                              │
                    ┌─────────┴──────────┐
                    │                    │
             Session Check         ApplicationDbContext
           (Role / UserId)          (EF Core → SQL Server)
                    │                    │
                    └─────────┬──────────┘
                              │
                         Razor View
                              │
                              ▼
                       HTML Response
```

EduManage uses **session-based role authentication** — after login, the user's `Role`, `StudentId`, or `TeacherId` is stored in the session. Every controller action checks this before doing anything, filtering data so that teachers only see their assigned class, students only see their own records, and admins see everything.

Database access goes through **Entity Framework Core** via `ApplicationDbContext`. SQL Server **views** are used for complex aggregated metrics on the admin dashboard (assignment progress, per-class student overviews).

---

## 🗄️ Database Schema

| 📋 Table | 📝 Description |
|---|---|
| `Students` | Profiles, roll numbers, class, section, attendance % |
| `Teachers` | Profiles, assigned class/section, subject |
| `Admins` | Admin credentials |
| `Assignments` | Details, file path, due date, class target |
| `AssignmentSubmissions` | Student files, grades, feedback, status |
| `Attendances` | Per-student, per-subject, per-date records |
| `Announcements` | Posts with target class and expiry date |
| `Events` | School events with start and end dates |
| `LeaveRequests` | Student leave requests with approval status |

---

## 📊 Language Breakdown

![HTML](https://img.shields.io/badge/HTML-56.3%25-E34F26?style=flat-square&logo=html5&logoColor=white)
![C#](https://img.shields.io/badge/C%23-35.5%25-239120?style=flat-square&logo=csharp&logoColor=white)
![TSQL](https://img.shields.io/badge/T--SQL-4.7%25-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white)
![CSS](https://img.shields.io/badge/CSS-3.5%25-1572B6?style=flat-square&logo=css3&logoColor=white)

---

## 📄 License

This project was developed as an academic project at **Mohammad Ali Jinnah University (MAJU), Karachi**. All rights reserved.

---

## 👨‍💻 Author

<div align="center">

**Natish Mourani**
BS Computer Science · Mohammad Ali Jinnah University, Karachi

[![GitHub](https://img.shields.io/badge/GitHub-natishmourani-181717?style=for-the-badge&logo=github)](https://github.com/natishmourani)

</div>
