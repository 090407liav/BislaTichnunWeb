using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BislaTichnunWeb
{
    public class Course
    {
        public int Id { get; set; }
        public string CName { get; set; }
        public string Gaf { get; set; }
        public string CourseNum { get; set; }
        public int ProfessionNum { get; set; }
        public bool IsActive { get; set; }
        public string WorkManager { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TraineesCount { get; set; }
    }
    public class DutyType
    {
        public int Id { get; set; }
        public string DutyName { get; set; }
        public string Color { get; set; }
        public int DefaultDurationDays { get; set; }
        public string Description { get; set; }
        public DayOfWeek? StartDayOfWeek { get; set; }
    }
    public class WeekInfo
    {
        public DateTime WeekStart { get; set; }

        public DateTime WeekEnd { get; set; }

        public string Label { get; set; }
    }

}