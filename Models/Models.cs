using System;

namespace EduManage.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Contact { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? FatherName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Class { get; set; }
        public string? Section { get; set; }
        public string? RollNumber { get; set; }
        public string? Contact { get; set; }
        public string? Address { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        // Academic
        public int Attendance { get; set; }
        public string? OverallGrade { get; set; }
    }

    public class Teacher
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Subject { get; set; }
        public string? AssignedClass { get; set; }
        public string? AssignedSection { get; set; }
        public string? Contact { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class Remark
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? TeacherName { get; set; }
        public string? RemarkType { get; set; }
        public string? RemarkText { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateRequest
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? FieldName { get; set; }
        public string? CurrentValue { get; set; }
        public string? NewValue { get; set; }
        public string? Reason { get; set; }
        public DateTime RequestedAt { get; set; }
    }

    public class LoginViewModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class TeacherViewModel
    {
        public Student SelectedStudent { get; set; }
        public System.Collections.Generic.List<Student> ClassStudents { get; set; }
    }

    public class Attendance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Present"; // Present, Absent, Leave
        public string? Remarks { get; set; }
        public string? Subject { get; set; }
    }

    public class Assignment
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public int TeacherId { get; set; }
        public string? Class { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AssignmentSubmission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string? FilePath { get; set; }
        public string? Grade { get; set; }
        public string? Feedback { get; set; }
        public string Status { get; set; } = "Submitted"; // Submitted, Graded
    }

    public class StudyMaterial
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        public string? Class { get; set; }
        public int TeacherId { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class Announcement
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? TargetClass { get; set; } // "All" or "Class 10", etc.
        public string? AuthorName { get; set; }
        public string? AuthorRole { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class SubjectMarkViewModel
    {
        public string Subject { get; set; } = "";
        public int Marks { get; set; }
        public bool HasMarks { get; set; }
    }

    public class Exam
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime Date { get; set; }
        public string? Class { get; set; }
        public string? Subject { get; set; }
        public int TotalMarks { get; set; } = 100;
    }

    public class Result
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public int ObtainedMarks { get; set; }
        public string? Grade { get; set; }
        public string? Remarks { get; set; }
        public bool IsPublished { get; set; }
    }

    public class LeaveRequest
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public string? RequesterRole { get; set; } // Student, Teacher
        public string? RequesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime RequestedAt { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string? SenderRole { get; set; } // Student, Teacher
        public string? SenderName { get; set; }
        public int ReceiverId { get; set; }
        public string? ReceiverRole { get; set; } // Student, Teacher
        public string? ReceiverName { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; } = false;
    }

    public class Event
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Type { get; set; } // Event, Holiday, Exam Schedule
    }

    public class DashboardStats
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int PendingLeaves { get; set; }
        public int TotalEvents { get; set; }
        public List<ClassStat> ClassStats { get; set; } = new();
    }

    public class ClassStat
    {
        public string? ClassName { get; set; }
        public int StudentCount { get; set; }
        public int AttendanceRate { get; set; }
    }

    public class SubjectMarksRedesignViewModel
    {
        public string SubjectName { get; set; } = "";
        public List<AssessmentRecordViewModel> Assessments { get; set; } = new();
        public int TotalObtainedMarks { get; set; }
        public int TotalPossibleMarks { get; set; }
        public double Percentage { get; set; }
        public string Grade { get; set; } = "N/A";
    }

    public class AssessmentRecordViewModel
    {
        public string AssessmentName { get; set; } = ""; // e.g. "Quiz 1" or "Midterm"
        public string AssessmentType { get; set; } = ""; // We can map this
        public int ObtainedMarks { get; set; }
        public int TotalMarks { get; set; }
    }

    public class SubjectAttendanceViewModel
    {
        public string SubjectName { get; set; } = "";
        public int TotalClassesConducted { get; set; }
        public int ClassesAttended { get; set; }
        public int ClassesMissed { get; set; }
        public double AttendancePercentage { get; set; }
        public List<AttendanceRecordViewModel> History { get; set; } = new();
    }

    public class AttendanceRecordViewModel
    {
        public DateTime Date { get; set; }
        public string SubjectName { get; set; } = "";
        public string Status { get; set; } = ""; // Present, Absent, Leave
    }

    public class StudentDashboardOverview
    {
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? RollNumber { get; set; }
        public string? StudentClass { get; set; }
        public string? StudentSection { get; set; }
        public int CumulativeAttendancePct { get; set; }
        public int TotalAssignmentsSubmitted { get; set; }
        public int PendingLeavesCount { get; set; }
        public double? AverageExamScorePct { get; set; }
    }

    public class AssignmentProgress
    {
        public int AssignmentId { get; set; }
        public string? AssignmentTitle { get; set; }
        public string? TargetClass { get; set; }
        public DateTime DueDate { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? TeacherSubject { get; set; }
        public int TotalSubmissions { get; set; }
        public int GradedSubmissions { get; set; }
    }

    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
