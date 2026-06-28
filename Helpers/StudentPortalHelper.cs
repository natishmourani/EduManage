using System;
using System.Collections.Generic;
using System.Linq;
using EduManage.Data;
using EduManage.Models;

namespace EduManage.Helpers
{
    public static class StudentPortalHelper
    {
        public static IQueryable<Announcement> ActiveAnnouncementsForStudent(
            IQueryable<Announcement> announcements, Student student)
        {
            var today = DateTime.Today;
            return announcements.Where(a =>
                (a.TargetClass == "All" || a.TargetClass == student.Class) &&
                (a.ExpiryDate == null || a.ExpiryDate.Value.Date >= today));
        }

        public static List<string> GetAssignedSubjects(ApplicationDbContext context, Student student)
        {
            return context.Teachers
                .Where(t => t.AssignedClass == student.Class &&
                    (string.IsNullOrEmpty(student.Section) || t.AssignedSection == student.Section))
                .Select(t => t.Subject!)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .OrderBy(s => s)
                .ToList();
        }

        public static List<SubjectMarkViewModel> GetSubjectMarks(ApplicationDbContext context, Student student)
        {
            var assignedSubjects = GetAssignedSubjects(context, student);
            var subjectMarks = new List<SubjectMarkViewModel>();

            foreach (var subject in assignedSubjects)
            {
                int marks = 0;
                bool hasMarks = false;

                var examResult = (from r in context.Results
                                  join e in context.Exams on r.ExamId equals e.Id
                                  where r.StudentId == student.Id &&
                                        r.IsPublished &&
                                        e.Subject == subject
                                  orderby e.Date descending
                                  select new { r.ObtainedMarks, e.TotalMarks })
                    .FirstOrDefault();

                if (examResult != null && examResult.TotalMarks > 0)
                {
                    marks = (int)Math.Round((double)examResult.ObtainedMarks / examResult.TotalMarks * 100);
                    hasMarks = true;
                }

                subjectMarks.Add(new SubjectMarkViewModel
                {
                    Subject = subject,
                    Marks = marks,
                    HasMarks = hasMarks
                });
            }

            return subjectMarks;
        }
    }
}
