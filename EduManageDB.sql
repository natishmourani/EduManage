/* ============================================================
       EduManageDB - School Management System Database

       This database is designed to manage the academic and
       administrative operations of a school. It includes
       tables for students, teachers, administrators,
       subjects, assignments, attendance, examinations,
       results, announcements, events, study materials,
       messaging, leave requests, remarks, and profile
       update requests.

       The database uses primary keys, foreign keys, views,
       and triggers to ensure data integrity and efficient
       data management.
   ============================================================ */

USE [master];
GO

-- 1. Create the Database if it does not exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'EduManageDB')
BEGIN
    CREATE DATABASE [EduManageDB];
    PRINT 'Database [EduManageDB] created successfully.';
END
ELSE
BEGIN
    PRINT 'Database [EduManageDB] already exists.';
END
GO

USE [EduManageDB];
GO

-- 2. Drop existing tables if they exist to start fresh
-- (Ordered to avoid foreign key dependency conflicts if any exist)
IF OBJECT_ID('dbo.UpdateRequests', 'U') IS NOT NULL DROP TABLE dbo.UpdateRequests;
IF OBJECT_ID('dbo.StudyMaterials', 'U') IS NOT NULL DROP TABLE dbo.StudyMaterials;
IF OBJECT_ID('dbo.Results', 'U') IS NOT NULL DROP TABLE dbo.Results;
IF OBJECT_ID('dbo.Remarks', 'U') IS NOT NULL DROP TABLE dbo.Remarks;
IF OBJECT_ID('dbo.Messages', 'U') IS NOT NULL DROP TABLE dbo.Messages;
IF OBJECT_ID('dbo.LeaveRequests', 'U') IS NOT NULL DROP TABLE dbo.LeaveRequests;
IF OBJECT_ID('dbo.Exams', 'U') IS NOT NULL DROP TABLE dbo.Exams;
IF OBJECT_ID('dbo.Events', 'U') IS NOT NULL DROP TABLE dbo.Events;
IF OBJECT_ID('dbo.Attendances', 'U') IS NOT NULL DROP TABLE dbo.Attendances;
IF OBJECT_ID('dbo.AssignmentSubmissions', 'U') IS NOT NULL DROP TABLE dbo.AssignmentSubmissions;
IF OBJECT_ID('dbo.Assignments', 'U') IS NOT NULL DROP TABLE dbo.Assignments;
IF OBJECT_ID('dbo.Announcements', 'U') IS NOT NULL DROP TABLE dbo.Announcements;
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL DROP TABLE dbo.Students;
IF OBJECT_ID('dbo.Teachers', 'U') IS NOT NULL DROP TABLE dbo.Teachers;
IF OBJECT_ID('dbo.Admins', 'U') IS NOT NULL DROP TABLE dbo.Admins;
IF OBJECT_ID('dbo.Subjects', 'U') IS NOT NULL DROP TABLE dbo.Subjects;
IF OBJECT_ID('dbo.__EFMigrationsHistory', 'U') IS NOT NULL DROP TABLE dbo.__EFMigrationsHistory;
GO

-- Drop existing views if they exist
IF OBJECT_ID('dbo.v_StudentResultCard', 'V') IS NOT NULL DROP VIEW dbo.v_StudentResultCard;
IF OBJECT_ID('dbo.v_AssignmentProgress', 'V') IS NOT NULL DROP VIEW dbo.v_AssignmentProgress;
IF OBJECT_ID('dbo.v_StudentDashboardOverview', 'V') IS NOT NULL DROP VIEW dbo.v_StudentDashboardOverview;
GO

-- =========================================================================
-- TABLE CREATION AND OVERVIEW
-- =========================================================================
-- Admins                : System administrators
-- Teachers              : Teacher information and credentials
-- Students              : Student records and academic details
-- Subjects              : Subjects offered by the school
-- Assignments           : Assignments created by teachers
-- AssignmentSubmissions : Student assignment submissions
-- Attendances           : Student attendance records
-- Exams                 : Examination details
-- Results               : Student examination results
-- Announcements         : School notices and announcements
-- Events                : Academic and extracurricular events
-- Messages              : Internal communication system
-- LeaveRequests         : Student and teacher leave requests
-- Remarks               : Teacher remarks and feedback
-- StudyMaterials        : Learning resources and documents
-- UpdateRequests        : Requests for profile information updates
-- ============================================================

-- __EFMigrationsHistory: Used by Entity Framework Core to manage schema changes
CREATE TABLE [dbo].[__EFMigrationsHistory](
    [MigrationId] NVARCHAR(150) NOT NULL,
    [ProductVersion] NVARCHAR(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
);
GO

-- Subjects: Stores subjects taught in school
CREATE TABLE [dbo].[Subjects](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Admins: Stores system administrator credentials and profiles
CREATE TABLE [dbo].[Admins](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FullName] NVARCHAR(MAX) NULL,
    [Contact] NVARCHAR(MAX) NULL,
    [Username] NVARCHAR(MAX) NULL,
    [Password] NVARCHAR(MAX) NULL,
    [Role] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Admins] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Trigger to prevent deleting admins
CREATE TRIGGER [dbo].[trg_PreventAdminDelete]
ON [dbo].[Admins]
INSTEAD OF DELETE
AS
BEGIN
    RAISERROR('Deletions are not allowed on the Admins table.', 16, 1);
END;
GO

-- Teachers: Stores credentials and details of teaching staff
CREATE TABLE [dbo].[Teachers](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FullName] NVARCHAR(MAX) NULL,
    [Subject] NVARCHAR(MAX) NULL,
    [AssignedClass] NVARCHAR(MAX) NULL,
    [AssignedSection] NVARCHAR(MAX) NULL,
    [Contact] NVARCHAR(MAX) NULL,
    [Username] NVARCHAR(MAX) NULL,
    [Password] NVARCHAR(MAX) NULL,
    [Role] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Teachers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Students: Stores student profiles, personal info, and overall marks
CREATE TABLE [dbo].[Students](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FullName] NVARCHAR(MAX) NULL,
    [FatherName] NVARCHAR(MAX) NULL,
    [DateOfBirth] DATETIME2(7) NULL,
    [Gender] NVARCHAR(MAX) NULL,
    [Class] NVARCHAR(MAX) NULL,
    [Section] NVARCHAR(MAX) NULL,
    [RollNumber] NVARCHAR(MAX) NULL,
    [Contact] NVARCHAR(MAX) NULL,
    [Address] NVARCHAR(MAX) NULL,
    [Username] NVARCHAR(MAX) NULL,
    [Password] NVARCHAR(MAX) NULL,
    [Attendance] INT NOT NULL,
    [OverallGrade] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Announcements: School-wide or class-specific public notifications
CREATE TABLE [dbo].[Announcements](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(MAX) NULL,
    [Content] NVARCHAR(MAX) NULL,
    [TargetClass] NVARCHAR(MAX) NULL,
    [AuthorName] NVARCHAR(MAX) NULL,
    [AuthorRole] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    [ExpiryDate] DATETIME2(7) NULL,
    CONSTRAINT [PK_Announcements] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Assignments: Uploaded by teachers for specific classes
CREATE TABLE [dbo].[Assignments](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(MAX) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [DueDate] DATETIME2(7) NOT NULL,
    [TeacherId] INT NOT NULL,
    [Class] NVARCHAR(MAX) NULL,
    [FilePath] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    CONSTRAINT [PK_Assignments] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- AssignmentSubmissions: Student submissions for given assignments
CREATE TABLE [dbo].[AssignmentSubmissions](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [AssignmentId] INT NOT NULL,
    [StudentId] INT NOT NULL,
    [SubmissionDate] DATETIME2(7) NOT NULL,
    [FilePath] NVARCHAR(MAX) NULL,
    [Grade] NVARCHAR(MAX) NULL,
    [Feedback] NVARCHAR(MAX) NULL,
    [Status] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_AssignmentSubmissions] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Attendances: Daily attendance logging for students
CREATE TABLE [dbo].[Attendances](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [StudentId] INT NOT NULL,
    [Date] DATETIME2(7) NOT NULL,
    [Status] NVARCHAR(MAX) NOT NULL,
    [Remarks] NVARCHAR(MAX) NULL,
    [Subject] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Attendances] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Events: Stores calendar events, holidays, and schedules
CREATE TABLE [dbo].[Events](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(MAX) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [StartDate] DATETIME2(7) NOT NULL,
    [EndDate] DATETIME2(7) NOT NULL,
    [Type] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Exams: Exam profiles containing subject, target class, and total marks
CREATE TABLE [dbo].[Exams](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(MAX) NULL,
    [Date] DATETIME2(7) NOT NULL,
    [Class] NVARCHAR(MAX) NULL,
    [Subject] NVARCHAR(MAX) NULL,
    [TotalMarks] INT NOT NULL,
    CONSTRAINT [PK_Exams] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- LeaveRequests: Requests submitted by teachers or students
CREATE TABLE [dbo].[LeaveRequests](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [RequesterId] INT NOT NULL,
    [RequesterRole] NVARCHAR(MAX) NULL,
    [RequesterName] NVARCHAR(MAX) NULL,
    [StartDate] DATETIME2(7) NOT NULL,
    [EndDate] DATETIME2(7) NOT NULL,
    [Reason] NVARCHAR(MAX) NULL,
    [Status] NVARCHAR(MAX) NOT NULL,
    [RequestedAt] DATETIME2(7) NOT NULL,
    CONSTRAINT [PK_LeaveRequests] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Messages: Internal messaging system records
CREATE TABLE [dbo].[Messages](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [SenderId] INT NOT NULL,
    [SenderRole] NVARCHAR(MAX) NULL,
    [SenderName] NVARCHAR(MAX) NULL,
    [ReceiverId] INT NOT NULL,
    [ReceiverRole] NVARCHAR(MAX) NULL,
    [ReceiverName] NVARCHAR(MAX) NULL,
    [Subject] NVARCHAR(MAX) NULL,
    [Body] NVARCHAR(MAX) NULL,
    [SentAt] DATETIME2(7) NOT NULL,
    [IsRead] BIT NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Remarks: Qualitative feedback left for students by teachers
CREATE TABLE [dbo].[Remarks](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [StudentId] INT NOT NULL,
    [TeacherName] NVARCHAR(MAX) NULL,
    [RemarkType] NVARCHAR(MAX) NULL,
    [RemarkText] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    CONSTRAINT [PK_Remarks] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Results: Academic scores achieved by students in exams
CREATE TABLE [dbo].[Results](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ExamId] INT NOT NULL,
    [StudentId] INT NOT NULL,
    [ObtainedMarks] INT NOT NULL,
    [Grade] NVARCHAR(MAX) NULL,
    [Remarks] NVARCHAR(MAX) NULL,
    [IsPublished] BIT NOT NULL,
    CONSTRAINT [PK_Results] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- StudyMaterials: Files and references shared by teachers for classes
CREATE TABLE [dbo].[StudyMaterials](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(MAX) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [FilePath] NVARCHAR(MAX) NULL,
    [Class] NVARCHAR(MAX) NULL,
    [TeacherId] INT NOT NULL,
    [UploadedAt] DATETIME2(7) NOT NULL,
    CONSTRAINT [PK_StudyMaterials] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- UpdateRequests: Student requests to update profile fields
CREATE TABLE [dbo].[UpdateRequests](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [StudentId] INT NOT NULL,
    [FieldName] NVARCHAR(MAX) NULL,
    [CurrentValue] NVARCHAR(MAX) NULL,
    [NewValue] NVARCHAR(MAX) NULL,
    [Reason] NVARCHAR(MAX) NULL,
    [RequestedAt] DATETIME2(7) NOT NULL,
    CONSTRAINT [PK_UpdateRequests] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- =========================================================================
-- VIEWS CREATION (JOINS AND SUBQUERIES)
-- =========================================================================

-- v_StudentResultCard: Joins Results, Students, and Exams.
-- Uses a correlated subquery to fetch the average score of all students for that exam.
PRINT 'Creating View v_StudentResultCard...';
GO
CREATE VIEW [dbo].[v_StudentResultCard] AS
SELECT 
    r.Id AS ResultId,
    s.Id AS StudentId,
    s.FullName AS StudentName,
    s.RollNumber,
    s.Class AS StudentClass,
    e.Id AS ExamId,
    e.Name AS ExamName,
    e.Subject,
    r.ObtainedMarks,
    e.TotalMarks,
    r.Grade AS ExamGrade,
    r.Remarks AS ExamRemarks,
    r.IsPublished,
    (SELECT AVG(CAST(r2.ObtainedMarks AS FLOAT)) 
     FROM [dbo].[Results] r2 
     WHERE r2.ExamId = e.Id) AS ClassAverageMarks
FROM [dbo].[Results] r
INNER JOIN [dbo].[Students] s ON r.StudentId = s.Id
INNER JOIN [dbo].[Exams] e ON r.ExamId = e.Id;
GO

-- v_AssignmentProgress: Joins Assignments and Teachers.
-- Uses subqueries to count total submissions and graded submissions.
PRINT 'Creating View v_AssignmentProgress...';
GO
CREATE VIEW [dbo].[v_AssignmentProgress] AS
SELECT 
    a.Id AS AssignmentId,
    a.Title AS AssignmentTitle,
    a.Class AS TargetClass,
    a.DueDate,
    t.Id AS TeacherId,
    t.FullName AS TeacherName,
    t.Subject AS TeacherSubject,
    (SELECT COUNT(*) 
     FROM [dbo].[AssignmentSubmissions] sub 
     WHERE sub.AssignmentId = a.Id) AS TotalSubmissions,
    (SELECT COUNT(*) 
     FROM [dbo].[AssignmentSubmissions] sub 
     WHERE sub.AssignmentId = a.Id AND sub.Status = 'Graded') AS GradedSubmissions
FROM [dbo].[Assignments] a
INNER JOIN [dbo].[Teachers] t ON a.TeacherId = t.Id;
GO

-- v_StudentDashboardOverview: Provides aggregated metrics for each student.
-- Uses multiple subqueries to pull total submitted assignments, pending leave requests, and average exam grade percentages.
PRINT 'Creating View v_StudentDashboardOverview...';
GO
CREATE VIEW [dbo].[v_StudentDashboardOverview] AS
SELECT 
    s.Id AS StudentId,
    s.FullName AS StudentName,
    s.RollNumber,
    s.Class AS StudentClass,
    s.Section AS StudentSection,
    s.Attendance AS CumulativeAttendancePct,
    (SELECT COUNT(*) 
     FROM [dbo].[AssignmentSubmissions] sub 
     WHERE sub.StudentId = s.Id) AS TotalAssignmentsSubmitted,
    (SELECT COUNT(*) 
     FROM [dbo].[LeaveRequests] lr 
     WHERE lr.RequesterId = s.Id AND lr.RequesterRole = 'Student' AND lr.Status = 'Pending') AS PendingLeavesCount,
    (SELECT AVG(CAST(r.ObtainedMarks AS FLOAT) / CAST(e.TotalMarks AS FLOAT) * 100.0)
     FROM [dbo].[Results] r
     INNER JOIN [dbo].[Exams] e ON r.ExamId = e.Id
     WHERE r.StudentId = s.Id) AS AverageExamScorePct
FROM [dbo].[Students] s;
GO

-- =========================================================================
-- SEED DATA INSERTION
-- =========================================================================

PRINT 'Seeding data...';

-- 1. Sync EF Migrations History
-- This tells Entity Framework Core that existing migrations are already applied.
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES
('20260607124829_InitialCreate', '8.0.0'),
('20260607153734_MakeColumnsNullable', '8.0.0'),
('20260615155915_AddNewFeatures', '8.0.0'),
('20260615165432_AddTeacherAssignedSection', '8.0.0'),
('20260617102039_AddAnnouncementExpiryDate', '8.0.0'),
('20260617121945_AddSubjectToAttendance', '8.0.0');

-- 2. Seed Teachers
INSERT INTO [dbo].[Teachers] ([FullName], [Subject], [AssignedClass], [AssignedSection], [Contact], [Username], [Password], [Role]) VALUES
('Sarah Connor', 'Mathematics', 'Class 10', 'A', '555-0199', 'sarahc', 'Pwd@teacher123', 'Teacher'),
('Bruce Wayne', 'Physics', 'Class 10', 'A', '555-0124', 'brucew', 'Pwd@teacher123', 'Teacher'),
('Clark Kent', 'English', 'Class 11', 'B', '555-0185', 'clarkk', 'Pwd@teacher123', 'Teacher');

-- 2b. Seed Admins
INSERT INTO [dbo].[Admins] ([FullName], [Contact], [Username], [Password], [Role]) VALUES
('Super Admin', '555-0100', 'admin', 'admin123', 'Super Admin');

-- 2c. Seed Subjects
INSERT INTO [dbo].[Subjects] ([Name]) VALUES
('Mathematics'),
('English'),
('Physics'),
('Chemistry'),
('Biology'),
('Computer Science'),
('Urdu'),
('Pakistan Studies'),
('Islamiat');

-- 3. Seed Students
INSERT INTO [dbo].[Students] ([FullName], [FatherName], [DateOfBirth], [Gender], [Class], [Section], [RollNumber], [Contact], [Address], [Username], [Password], [Attendance], [OverallGrade]) VALUES
('John Doe', 'Robert Doe', '2008-05-15 00:00:00', 'Male', 'Class 10', 'A', 'STU001', '555-1234', '123 Elm St, Metropolis', 'johndoe', 'pwd@student001', 92, 'A'),
('Jane Smith', 'Will Smith', '2008-09-20 00:00:00', 'Female', 'Class 10', 'A', 'STU002', '555-5678', '456 Oak Rd, Gotham', 'janesmith', 'pwd@student002', 96, 'A+'),
('Alex Johnson', 'Mark Johnson', '2009-02-10 00:00:00', 'Male', 'Class 10', 'B', 'STU003', '555-9876', '789 Pine Ave, Star City', 'alexj', 'pwd@student003', 80, 'B');

-- 4. Seed Announcements
INSERT INTO [dbo].[Announcements] ([Title], [Content], [TargetClass], [AuthorName], [AuthorRole], [CreatedAt], [ExpiryDate]) VALUES
('Welcome to the New Term', 'Welcome back everyone! Let''s work hard and aim for excellence this term.', 'All', 'Principal', 'Admin', GETDATE(), DATEADD(month, 2, GETDATE())),
('Math Midterm Quiz Announcement', 'A preparatory quiz covering Algebra will be held next Tuesday during standard class hours.', 'Class 10', 'Sarah Connor', 'Teacher', GETDATE(), DATEADD(day, 7, GETDATE()));

-- 5. Seed Assignments
INSERT INTO [dbo].[Assignments] ([Title], [Description], [DueDate], [TeacherId], [Class], [FilePath], [CreatedAt]) VALUES
('Algebra Practice Set 1', 'Solve problems 1-15 on Page 120 of the course book. Scan and upload the PDF file.', DATEADD(day, 5, GETDATE()), 1, 'Class 10', NULL, GETDATE()),
('Mechanics Assignment', 'Write a short summary on Newton''s Laws of Motion with real-world applications.', DATEADD(day, 8, GETDATE()), 2, 'Class 10', NULL, GETDATE());

-- 6. Seed AssignmentSubmissions
INSERT INTO [dbo].[AssignmentSubmissions] ([AssignmentId], [StudentId], [SubmissionDate], [FilePath], [Grade], [Feedback], [Status]) VALUES
(1, 1, DATEADD(day, -1, GETDATE()), 'submissions/stu001_alg1.pdf', 'A', 'Excellent logical breakdown of algebra problems. Keep it up!', 'Graded'),
(1, 2, GETDATE(), 'submissions/stu002_alg1.pdf', NULL, NULL, 'Submitted');

-- 7. Seed Attendances
INSERT INTO [dbo].[Attendances] ([StudentId], [Date], [Status], [Remarks], [Subject]) VALUES
(1, DATEADD(day, -2, GETDATE()), 'Present', 'On time', 'Mathematics'),
(1, DATEADD(day, -1, GETDATE()), 'Present', 'On time', 'Physics'),
(1, GETDATE(), 'Present', 'On time', 'Mathematics'),
(2, DATEADD(day, -2, GETDATE()), 'Present', 'On time', 'Mathematics'),
(2, DATEADD(day, -1, GETDATE()), 'Present', 'On time', 'Physics'),
(2, GETDATE(), 'Present', 'On time', 'Mathematics'),
(3, DATEADD(day, -2, GETDATE()), 'Absent', 'Excused medical absence', 'Mathematics'),
(3, DATEADD(day, -1, GETDATE()), 'Present', 'On time', 'Physics'),
(3, GETDATE(), 'Present', 'On time', 'Mathematics');

-- 8. Seed Events
INSERT INTO [dbo].[Events] ([Title], [Description], [StartDate], [EndDate], [Type]) VALUES
('Annual Science Fair', 'Showcasing innovative student projects in biology, physics, and chemistry.', DATEADD(day, 12, GETDATE()), DATEADD(day, 13, GETDATE()), 'Event'),
('Parent-Teacher Association Meeting', 'Meeting to discuss academic progress and general school updates.', DATEADD(day, 18, GETDATE()), DATEADD(day, 18, GETDATE()), 'Event'),
('Summer Break Starts', 'School will remain closed for the annual summer vacation.', DATEADD(month, 3, GETDATE()), DATEADD(month, 4, GETDATE()), 'Holiday');

-- 9. Seed Exams
INSERT INTO [dbo].[Exams] ([Name], [Date], [Class], [Subject], [TotalMarks]) VALUES
('Algebra Quiz 1', DATEADD(day, -5, GETDATE()), 'Class 10', 'Mathematics', 50),
('Physics Dynamics Exam', DATEADD(day, -10, GETDATE()), 'Class 10', 'Physics', 100);

-- 10. Seed Results
INSERT INTO [dbo].[Results] ([ExamId], [StudentId], [ObtainedMarks], [Grade], [Remarks], [IsPublished]) VALUES
(1, 1, 42, 'A', 'Great conceptual understanding.', 1),
(1, 2, 48, 'A+', 'Outstanding score! Near flawless.', 1),
(1, 3, 31, 'C', 'Needs to pay more attention to algebraic proofs.', 1);

-- 11. Seed LeaveRequests
INSERT INTO [dbo].[LeaveRequests] ([RequesterId], [RequesterRole], [RequesterName], [StartDate], [EndDate], [Reason], [Status], [RequestedAt]) VALUES
(3, 'Student', 'Alex Johnson', DATEADD(day, 3, GETDATE()), DATEADD(day, 4, GETDATE()), 'Family reunion and travel.', 'Pending', GETDATE());

-- 12. Seed Messages
INSERT INTO [dbo].[Messages] ([SenderId], [SenderRole], [SenderName], [ReceiverId], [ReceiverRole], [ReceiverName], [Subject], [Body], [SentAt], [IsRead]) VALUES
(1, 'Student', 'John Doe', 1, 'Teacher', 'Sarah Connor', 'Question on Algebra assignment', 'Hello Mrs. Connor, I wanted to ask if we can solve question 7 using graphical methods as well. Thanks!', DATEADD(day, -1, GETDATE()), 0),
(1, 'Teacher', 'Sarah Connor', 1, 'Student', 'John Doe', 'RE: Question on Algebra assignment', 'Hi John, yes, graphical solution is perfectly acceptable. Just make sure to label the coordinates properly.', GETDATE(), 1);

-- 13. Seed Remarks
INSERT INTO [dbo].[Remarks] ([StudentId], [TeacherName], [RemarkType], [RemarkText], [CreatedAt]) VALUES
(1, 'Sarah Connor', 'Academic', 'John has shown fantastic progress in calculus and algebra.', DATEADD(day, -10, GETDATE())),
(3, 'Bruce Wayne', 'Behavioral', 'Alex needs to stay focused during laboratory experiments.', DATEADD(day, -5, GETDATE()));

-- 14. Seed UpdateRequests
INSERT INTO [dbo].[UpdateRequests] ([StudentId], [FieldName], [CurrentValue], [NewValue], [Reason], [RequestedAt]) VALUES
(2, 'Contact', '555-5678', '555-9000', 'Parents upgraded their phone plans and contact details.', GETDATE());

PRINT 'Seeding completed successfully.';
GO
