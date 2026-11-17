using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace BislaTichnunWeb
{
    public partial class MainDashboard : Page
    {
        protected int ActiveCourses;
        protected int TotalTrainees;
        protected int TotalAssignments;
        protected int ConfirmedThisMonth;
        protected int PendingThisWeek;

        protected List<DutyAssignment> WeekAssignments =
            new List<DutyAssignment>();

        protected Dictionary<int, Course> CourseLookup =
            new Dictionary<int, Course>();

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            List<Course> courses =
                DataAccess.GetCourses(activeOnly: true);

            ActiveCourses = courses.Count;

            TotalTrainees =
                courses.Sum(course => course.TraineesCount);

            CourseLookup =
                courses.ToDictionary(course => course.Id);

            /*
             * אם ב-DataAccess שלך המתודה נקראת אחרת,
             * החלף את השורה בהתאם.
             */
            TotalAssignments =
                DataAccess.GetAssignmentsCount();

            DateTime today = DateTime.Today;

            DateTime weekStart =
                StartOfWeek(today);

            DateTime weekEnd =
                weekStart.AddDays(6);

            WeekAssignments =
                DataAccess.GetAssignments(
                    weekStart,
                    weekEnd);

            PendingThisWeek =
                WeekAssignments.Count(assignment =>
                    assignment.Status ==
                    AssignmentStatus.Planning);

            DateTime monthStart =
                new DateTime(
                    today.Year,
                    today.Month,
                    1);

            DateTime monthEnd =
                monthStart
                    .AddMonths(1)
                    .AddDays(-1);

            List<DutyAssignment> monthAssignments =
                DataAccess.GetAssignments(
                    monthStart,
                    monthEnd);

            ConfirmedThisMonth =
                monthAssignments.Count(assignment =>
                    assignment.Status ==
                        AssignmentStatus.Active &&
                    assignment.ApprovedAt.HasValue &&
                    assignment.ApprovedAt.Value.Date >=
                        monthStart.Date &&
                    assignment.ApprovedAt.Value.Date <=
                        monthEnd.Date);
        }

        private static DateTime StartOfWeek(
            DateTime date)
        {
            int daysFromSunday =
                (int)date.DayOfWeek;

            return date.Date.AddDays(
                -daysFromSunday);
        }

        protected string GetCourseName(
            int courseId)
        {
            Course course;

            if (CourseLookup.TryGetValue(
                courseId,
                out course))
            {
                return course.CName;
            }

            return "קורס לא נמצא";
        }

        protected string GetStatusCssClass(
            DutyAssignment assignment)
        {
            return assignment.Status ==
                   AssignmentStatus.Active
                ? "badge-active"
                : "badge-inactive";
        }

        protected string GetStatusText(
            DutyAssignment assignment)
        {
            return assignment.Status ==
                   AssignmentStatus.Active
                ? "מאושר"
                : "בתכנון";
        }
    }
}