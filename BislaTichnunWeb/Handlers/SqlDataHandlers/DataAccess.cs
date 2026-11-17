using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BislaTichnunWeb
{
    public static class DataAccess //FICTIVE DATA
    {
        public static List<Course> GetCourses(bool activeOnly = false)
        {
            List<Course> lstCourses = new List<Course>();

            lstCourses.Add(new Course
            {
                Id = 1,
                CName = "קורס טכנאי מנועים",
                CourseNum = "1265",
                ProfessionNum = 101,
                IsActive = true,
                WorkManager = "רס\"ר אבי כהן",
                StartDate = new DateTime(2026, 5, 3),
                EndDate = new DateTime(2026, 9, 24),
                TraineesCount = 28
            });

            lstCourses.Add(new Course
            {
                Id = 2,
                CName = "קורס טכנאי חשמל",
                CourseNum = "1271",
                ProfessionNum = 102,
                IsActive = true,
                WorkManager = "סמ\"ר דניאל לוי",
                StartDate = new DateTime(2026, 5, 10),
                EndDate = new DateTime(2026, 10, 8),
                TraineesCount = 32
            });

            lstCourses.Add(new Course
            {
                Id = 3,
                CName = "קורס טכנאי אוויוניקה",
                CourseNum = "1280",
                ProfessionNum = 103,
                IsActive = true,
                WorkManager = "רס\"ל יואב מזרחי",
                StartDate = new DateTime(2026, 6, 7),
                EndDate = new DateTime(2026, 11, 19),
                TraineesCount = 24
            });

            lstCourses.Add(new Course
            {
                Id = 4,
                CName = "קורס טכנאי מבנה",
                CourseNum = "1284",
                ProfessionNum = 104,
                IsActive = true,
                WorkManager = "סמ\"ר רועי פרץ",
                StartDate = new DateTime(2026, 4, 19),
                EndDate = new DateTime(2026, 8, 27),
                TraineesCount = 30
            });

            lstCourses.Add(new Course
            {
                Id = 5,
                CName = "קורס טכנאי מערכות נשק",
                CourseNum = "1292",
                ProfessionNum = 105,
                IsActive = true,
                WorkManager = "רס\"ר נועם אלון",
                StartDate = new DateTime(2026, 6, 14),
                EndDate = new DateTime(2026, 12, 3),
                TraineesCount = 21
            });

            lstCourses.Add(new Course
            {
                Id = 6,
                CName = "קורס טכנאי מכ\"ם",
                CourseNum = "1301",
                ProfessionNum = 106,
                IsActive = true,
                WorkManager = "סמ\"ר עומר ברק",
                StartDate = new DateTime(2026, 7, 5),
                EndDate = new DateTime(2026, 12, 17),
                TraineesCount = 26
            });

            lstCourses.Add(new Course
            {
                Id = 7,
                CName = "קורס טכנאי תקשורת",
                CourseNum = "1308",
                ProfessionNum = 107,
                IsActive = true,
                WorkManager = "רס\"ל איתי חדד",
                StartDate = new DateTime(2026, 7, 12),
                EndDate = new DateTime(2027, 1, 7),
                TraineesCount = 19
            });

            lstCourses.Add(new Course
            {
                Id = 8,
                CName = "קורס טכנאי חימוש",
                CourseNum = "1240",
                ProfessionNum = 108,
                IsActive = false,
                WorkManager = "סמ\"ר נדב שלום",
                StartDate = new DateTime(2026, 1, 4),
                EndDate = new DateTime(2026, 5, 28),
                TraineesCount = 25
            });

            lstCourses.Add(new Course
            {
                Id = 9,
                CName = "קורס טכנאי ציוד קרקע",
                CourseNum = "1247",
                ProfessionNum = 109,
                IsActive = false,
                WorkManager = "רס\"ר אלעד דיין",
                StartDate = new DateTime(2026, 2, 8),
                EndDate = new DateTime(2026, 6, 18),
                TraineesCount = 22
            });

            lstCourses.Add(new Course
            {
                Id = 10,
                CName = "קורס טכנאי מערכות בקרה",
                CourseNum = "1315",
                ProfessionNum = 110,
                IsActive = true,
                WorkManager = "סמ\"ר גיא בן דוד",
                StartDate = new DateTime(2026, 8, 2),
                EndDate = new DateTime(2027, 1, 21),
                TraineesCount = 27
            });

            if (activeOnly)
            {
                return lstCourses
                    .Where(course => course.IsActive)
                    .OrderBy(course => course.CourseNum)
                    .ToList();
            }

            return lstCourses
                .OrderBy(course => course.CourseNum)
                .ToList();
        }
        public static List<DutyType> GetDutyTypes()//Create hard-coded list
        {
            List<DutyType> lstDutyTypes = new List<DutyType>();

            lstDutyTypes.Add(new DutyType
            {
                Id = 1,
                DutyName = "גיבור זר",
                Color = "#2563EB",
                DefaultDurationDays = 7,
                Description = "משימת גיבור זר בבסיס נעורה",
                StartDayOfWeek = DayOfWeek.Sunday
            }) ;

            lstDutyTypes.Add(new DutyType
            {
                Id = 2,
                DutyName = "מטבח",
                Color = "#16A34A",
                DefaultDurationDays = 5,
                Description = "תורנות מטבח",
                StartDayOfWeek = DayOfWeek.Sunday
            });

            lstDutyTypes.Add(new DutyType
            {
                Id = 3,
                DutyName = "רס\"ר",
                Color = "#F97316",
                DefaultDurationDays = 5,
                Description = "תורנות רס\"ר",
                StartDayOfWeek = DayOfWeek.Sunday
            });

            lstDutyTypes.Add(new DutyType
            {
                Id = 4,
                DutyName = "אבטחת חניונים",
                Color = "#DC2626",
                DefaultDurationDays = 1,
                Description = "אבטחת שאטלים וחניונים",
                StartDayOfWeek = DayOfWeek.Sunday
            });

            lstDutyTypes.Add(new DutyType
            {
                Id = 5,
                DutyName = "ממו\"ק",
                Color = "#8B5CF6",
                DefaultDurationDays = 7,
                Description = "משימת ממו\"ק",
                StartDayOfWeek = DayOfWeek.Sunday
            });

            lstDutyTypes.Add(new DutyType
            {
                Id = 6,
                DutyName = "שמירות מגדלים",
                Color = "#0F766E",
                DefaultDurationDays = 7,
                Description = "שמירות מגדלי שמירה",
                StartDayOfWeek = DayOfWeek.Sunday
            });

            lstDutyTypes.Add(new DutyType
            {
                Id = 7,
                DutyName = "ממח\"א אבטחה",
                Color = "#EAB308",
                DefaultDurationDays = 3,
                Description = "אבטחת ממח\"א",
                StartDayOfWeek = DayOfWeek.Monday
            });

            lstDutyTypes.Add(new DutyType
            {
                Id = 8,
                DutyName = "ליווי משאיות",
                Color = "#475569",
                DefaultDurationDays = 2,
                Description = "ליווי שיירות ומשאיות",
                StartDayOfWeek = DayOfWeek.Thursday
            });

            lstDutyTypes.Add(new DutyType
            {
                Id = 9,
                DutyName = "מנהלות",
                Color = "#EC4899",
                DefaultDurationDays = 5,
                Description = "משימת מנהלות",
                StartDayOfWeek = DayOfWeek.Sunday
            });

            lstDutyTypes.Add(new DutyType
            {
                Id = 10,
                DutyName = "מפעלי חימוש",
                Color = "#14B8A6",
                DefaultDurationDays = 3,
                Description = "משימת מפעלי חימוש",
                StartDayOfWeek = DayOfWeek.Sunday
            });

            return lstDutyTypes;
        }
        private static List<DutyAssignment> _assignments;

        public static List<CourseEvent> GetCourseEvents(DateTime rangeStart, DateTime rangeEnd)
        {
            
            List<CourseEvent> events = new List<CourseEvent>();

            events.Add(new CourseEvent
            {
                Id = 1001,
                CourseId = 1,
                EventType = CourseEventType.HearingTest,
                ItemName = "בדיקות שמיעה",
                Color = "#06B6D4",
                StartDate = new DateTime(2026, 7, 15),
                EndDate = new DateTime(2026, 7, 15),
                Notes = "בדיקות שמיעה לכלל החניכים",
                Draggable = true
            });

            events.Add(new CourseEvent
            {
                Id = 1002,
                CourseId = 1,
                EventType = CourseEventType.WiringSkills,
                ItemName = "כיתת מיומנות - חיווט",
                Color = "#3B82F6",
                StartDate = new DateTime(2026, 7, 20),
                EndDate = new DateTime(2026, 7, 24),
                Notes = "",
                Draggable = true
            });

            events.Add(new CourseEvent
            {
                Id = 1003,
                CourseId = 2,
                EventType = CourseEventType.MaintenanceSkills,
                ItemName = "כיתת מיומנות - אחזקה",
                Color = "#10B981",
                StartDate = new DateTime(2026, 7, 13),
                EndDate = new DateTime(2026, 7, 17),
                Notes = "",
                Draggable = true
            });

            events.Add(new CourseEvent
            {
                Id = 1004,
                CourseId = 3,
                EventType = CourseEventType.ElectronicsSkills,
                ItemName = "כיתת מיומנות - אלקטרוניקה",
                Color = "#F59E0B",
                StartDate = new DateTime(2026, 7, 27),
                EndDate = new DateTime(2026, 7, 31),
                Notes = "",
                Draggable = true
            });

            events.Add(new CourseEvent
            {
                Id = 1005,
                CourseId = 4,
                EventType = CourseEventType.PracticalWork,
                ItemName = "עבודה מעשית",
                Color = "#8B5CF6",
                StartDate = new DateTime(2026, 7, 6),
                EndDate = new DateTime(2026, 7, 17),
                Notes = "סיפוח לבסיס אחר",
                Draggable = true
            });

            events.Add(new CourseEvent
            {
                Id = 1006,
                CourseId = 5,
                EventType = CourseEventType.ProfessionalTour,
                ItemName = "סיור מקצועי",
                Color = "#EF4444",
                StartDate = new DateTime(2026, 7, 22),
                EndDate = new DateTime(2026, 7, 22),
                Notes = "ביקור במפעל",
                Draggable = false
            });

            // מחזיר רק אירועים שנמצאים בטווח המבוקש
            return events
                .Where(e =>
                    e.StartDate <= rangeEnd &&
                    e.EndDate >= rangeStart)
                .ToList();
         
        }

        //==========================================
        // Initialization
        //==========================================

        private static void EnsureData()
        {
            if (_assignments != null)
                return;

            _assignments = BuildAssignments();
        }

        private static List<DutyAssignment> BuildAssignments()
        {
            List<DutyAssignment> list =
                new List<DutyAssignment>();

            list.Add(new DutyAssignment
            {
                Id = 1,
                CourseId = 1,
                DutyTypeId = 1,
                ItemName = "גיבור זר",
                Color = "#2563EB",
                StartDate = new DateTime(2026, 7, 5),
                EndDate = new DateTime(2026, 7, 11),
                Status = AssignmentStatus.Planning,
                Notes = "מחזור א'",
                ApprovedAt = null
            });

            list.Add(new DutyAssignment
            {
                Id = 2,
                CourseId = 1,
                DutyTypeId = 2,
                ItemName = "מטבח",
                Color = "#16A34A",
                StartDate = new DateTime(2026, 7, 19),
                EndDate = new DateTime(2026, 7, 23),
                Status = AssignmentStatus.Active,
                Notes = "",
                ApprovedAt = DateTime.Now.AddDays(-7)
            });

            list.Add(new DutyAssignment
            {
                Id = 3,
                CourseId = 2,
                DutyTypeId = 6,
                ItemName = "שמירות מגדלים",
                Color = "#0F766E",
                StartDate = new DateTime(2026, 7, 5),
                EndDate = new DateTime(2026, 7, 11),
                Status = AssignmentStatus.Planning,
                Notes = "",
                ApprovedAt = null
            });

            list.Add(new DutyAssignment
            {
                Id = 4,
                CourseId = 2,
                DutyTypeId = 3,
                ItemName = "רס\"ר",
                Color = "#F97316",
                StartDate = new DateTime(2026, 7, 12),
                EndDate = new DateTime(2026, 7, 16),
                Status = AssignmentStatus.Planning,
                Notes = "",
                ApprovedAt = null
            });

            list.Add(new DutyAssignment
            {
                Id = 5,
                CourseId = 3,
                DutyTypeId = 5,
                ItemName = "ממו\"ק",
                Color = "#8B5CF6",
                StartDate = new DateTime(2026, 7, 12),
                EndDate = new DateTime(2026, 7, 18),
                Status = AssignmentStatus.Active,
                Notes = "",
                ApprovedAt = DateTime.Now.AddDays(-2)
            });

            list.Add(new DutyAssignment
            {
                Id = 6,
                CourseId = 3,
                DutyTypeId = 7,
                ItemName = "ממח\"א אבטחה",
                Color = "#EAB308",
                StartDate = new DateTime(2026, 7, 26),
                EndDate = new DateTime(2026, 7, 28),
                Status = AssignmentStatus.Planning,
                Notes = "",
                ApprovedAt = null
            });

            list.Add(new DutyAssignment
            {
                Id = 7,
                CourseId = 4,
                DutyTypeId = 4,
                ItemName = "אבטחת חניונים",
                Color = "#DC2626",
                StartDate = new DateTime(2026, 7, 6),
                EndDate = new DateTime(2026, 7, 6),
                Status = AssignmentStatus.Planning,
                Notes = "",
                ApprovedAt = null
            });

            list.Add(new DutyAssignment
            {
                Id = 8,
                CourseId = 5,
                DutyTypeId = 8,
                ItemName = "ליווי משאיות",
                Color = "#475569",
                StartDate = new DateTime(2026, 7, 20),
                EndDate = new DateTime(2026, 7, 21),
                Status = AssignmentStatus.Active,
                Notes = "",
                ApprovedAt = DateTime.Now.AddDays(-5)
            });

            list.Add(new DutyAssignment
            {
                Id = 9,
                CourseId = 6,
                DutyTypeId = 9,
                ItemName = "מנהלות",
                Color = "#EC4899",
                StartDate = new DateTime(2026, 7, 13),
                EndDate = new DateTime(2026, 7, 17),
                Status = AssignmentStatus.Planning,
                Notes = "",
                ApprovedAt = null
            });

            list.Add(new DutyAssignment
            {
                Id = 10,
                CourseId = 7,
                DutyTypeId = 10,
                ItemName = "מפעלי חימוש",
                Color = "#14B8A6",
                StartDate = new DateTime(2026, 7, 27),
                EndDate = new DateTime(2026, 7, 29),
                Status = AssignmentStatus.Planning,
                Notes = "",
                ApprovedAt = null
            });

            // מספר תורנויות באותו שבוע לאותו קורס
            list.Add(new DutyAssignment
            {
                Id = 11,
                CourseId = 1,
                DutyTypeId = 4,
                ItemName = "אבטחת חניונים",
                Color = "#DC2626",
                StartDate = new DateTime(2026, 7, 5),
                EndDate = new DateTime(2026, 7, 5),
                Status = AssignmentStatus.Planning,
                Notes = "במקביל לגיבור זר",
                ApprovedAt = null
            });

            list.Add(new DutyAssignment
            {
                Id = 12,
                CourseId = 2,
                DutyTypeId = 2,
                ItemName = "מטבח",
                Color = "#16A34A",
                StartDate = new DateTime(2026, 7, 12),
                EndDate = new DateTime(2026, 7, 16),
                Status = AssignmentStatus.Planning,
                Notes = "",
                ApprovedAt = null
            });

            return list;
        }


        //==========================================
        // Assignments
        //==========================================

        public static List<DutyAssignment> GetAssignments(
            DateTime from,
            DateTime to)
        {
            EnsureData();

            return _assignments
                .Where(a => a.EndDate >= from &&
                            a.StartDate <= to)
                .ToList();
        }

        public static DutyAssignment GetAssignment(int id)
        {
            EnsureData();

            return _assignments
                .FirstOrDefault(a => a.Id == id);
        }

        public static int CreateAssignment(DutyAssignment assignment)
        {
            EnsureData();

            assignment.Id = _assignments.Count == 0
                ? 1
                : _assignments.Max(a => a.Id) + 1;

            assignment.Status = AssignmentStatus.Planning;

            _assignments.Add(assignment);

            return assignment.Id;
        }

        public static bool UpdateAssignment(DutyAssignment assignment)
        {
            EnsureData();

            var current =
                _assignments.FirstOrDefault(a => a.Id == assignment.Id);

            if (current == null)
                return false;

            current.CourseId = assignment.CourseId;
            current.DutyTypeId = assignment.DutyTypeId;
            current.StartDate = assignment.StartDate;
            current.EndDate = assignment.EndDate;
            current.Notes = assignment.Notes;

            // Every edit returns the assignment to planning mode
            current.Status = AssignmentStatus.Planning;
            current.ApprovedAt = null;

            return true;
        }

        public static bool DeleteAssignment(int id)
        {
            EnsureData();

            var assignment =
                _assignments.FirstOrDefault(a => a.Id == id);

            if (assignment == null)
                return false;

            _assignments.Remove(assignment);

            return true;
        }

        public static bool ConfirmAssignment(int id)
        {
            EnsureData();

            var assignment =
                _assignments.FirstOrDefault(a => a.Id == id);

            if (assignment == null)
                return false;

            assignment.Status = AssignmentStatus.Active;
            assignment.ApprovedAt = DateTime.Now;

            return true;
        }

        //==========================================
        // Dashboard
        //==========================================

        public static int GetActiveCoursesCount()
        {
            return GetCourses(true).Count;
        }

        public static int GetTotalTrainees()
        {
            return GetCourses()
                .Where(c => c.IsActive)
                .Sum(c => c.TraineesCount);
        }

        public static int GetAssignmentsCount()
        {
            EnsureData();

            return _assignments.Count;
        }

        public static int GetConfirmedCount()
        {
            EnsureData();

            return _assignments.Count(a => a.Status == AssignmentStatus.Active);
        }

        public static int GetPlanningCount()
        {
            EnsureData();

            return _assignments.Count(a => a.Status == AssignmentStatus.Planning);
        }

        //==========================================
        // Development helper
        //==========================================

        public static void ResetFakeData()
        {
            _assignments = null;
        }
    }
}
    
    /*public static class DataAccess
    {
        // ── Courses ─────────────────────────────────────────────
            
        public static List<Course> GetCourses(bool activeOnly = false)
        {
            var list = new List<Course>();

            string sql = activeOnly
                ? "SELECT * FROM Courses WHERE IsActive=True ORDER BY CourseNum"
                : "SELECT * FROM Courses ORDER BY CourseNum";

            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand(sql, conn))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                    list.Add(ReadCourse(r));
            }

            return list;
        }

        public static Course GetCourse(int id)
        {
            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand("SELECT * FROM Courses WHERE Id=?", conn))
            {
                cmd.Parameters.AddWithValue("?", id);

                using (var r = cmd.ExecuteReader())
                {
                    return r.Read() ? ReadCourse(r) : null;
                }
            }
        }

        public static int CreateCourse(Course c)
        {
            string sql = @"INSERT INTO Courses
        (CName, CourseNum, ProfessionNum, IsActive, WorkManager, StartDate, EndDate, TraineesCount)
        VALUES (?,?,?,?,?,?,?,?)";

            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("?", c.CName);
                cmd.Parameters.AddWithValue("?", c.CourseNum);
                cmd.Parameters.AddWithValue("?", c.ProfessionNum);
                cmd.Parameters.AddWithValue("?", c.IsActive);
                cmd.Parameters.AddWithValue("?", ToDb(c.WorkManager));
                cmd.Parameters.AddWithValue("?", c.StartDate);
                cmd.Parameters.AddWithValue("?", c.EndDate);
                cmd.Parameters.AddWithValue("?", c.TraineesCount);

                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT @@IDENTITY";
                cmd.Parameters.Clear();

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void UpdateCourse(Course c)
        {
            string sql = @"UPDATE Courses SET
        CName=?,
        CourseNum=?,
        ProfessionNum=?,
        IsActive=?,
        WorkManager=?,
        StartDate=?,
        EndDate=?,
        TraineesCount=?
        WHERE Id=?";

            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("?", c.CName);
                cmd.Parameters.AddWithValue("?", c.CourseNum);
                cmd.Parameters.AddWithValue("?", c.ProfessionNum);
                cmd.Parameters.AddWithValue("?", c.IsActive);
                cmd.Parameters.AddWithValue("?", ToDb(c.WorkManager));
                cmd.Parameters.AddWithValue("?", c.StartDate);
                cmd.Parameters.AddWithValue("?", c.EndDate);
                cmd.Parameters.AddWithValue("?", c.TraineesCount);
                cmd.Parameters.AddWithValue("?", c.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteCourse(int id)
        {
            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand("DELETE FROM Courses WHERE Id=?", conn))
            {
                cmd.Parameters.AddWithValue("?", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ── DutyTypes ───────────────────────────────────────────

        public static List<DutyType> GetDutyTypes()
        {
            var list = new List<DutyType>();

            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand("SELECT * FROM DutyTypes ORDER BY DutyName", conn))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                    list.Add(ReadDutyType(r));
            }

            return list;
        }
        public static List<DutyType> GetDutyTypesHC()//Create hard-coded list
        {
            List<DutyType> lstDutyTypes = new List<DutyType>();
            lstDutyTypes.Add(new DutyType(
            1,
            "גיבור זר",
            "#2563EB",
            7,
            "משימת גיבור זר בבסיס נעורה",
            DayOfWeek.Sunday"));

            lstDutyTypes.Add(new DutyType(
                2,
                "מטבח",
                "#16A34A",
                5,
                "תורנות מטבח",
                DayOfWeek.Sunday"));

            lstDutyTypes.Add(new DutyType(
                3,
                "רס\"ר",
                "#F97316",
                5,
                "תורנות רס\"ר",
                "Sunday"));

            lstDutyTypes.Add(new DutyType(
                4,
                "אבטחת חניונים",
                "#DC2626",
                1,
                "אבטחת שאטלים וחניונים",
                "Sunday"));

            lstDutyTypes.Add(new DutyType(
                5,
                "ממו\"ק",
                "#8B5CF6",
                7,
                "משימת ממו\"ק",
                "Sunday"));

            lstDutyTypes.Add(new DutyType(
                6,
                "שמירות מגדלים",
                "#0F766E",
                7,
                "שמירות מגדלי שמירה",
                "Sunday"));

            lstDutyTypes.Add(new DutyType(
                7,
                "ממח\"א אבטחה",
                "#EAB308",
                3,
                "אבטחת ממח\"א",
                "Monday"));

            lstDutyTypes.Add(new DutyType(
                8,
                "ליווי משאיות",
                "#475569",
                2,
                "ליווי שיירות ומשאיות",
                "Tuesday"));

            lstDutyTypes.Add(new DutyType(
                9,
                "מנהלות",
                "#EC4899",
                5,
                "משימת מנהלות",
                "Sunday"));

            lstDutyTypes.Add(new DutyType(
                10,
                "מפעלי חימוש",
                "#14B8A6",
                3,
                "משימת מפעלי חימוש",
                "Wednesday"));
            return lstDutyTypes;
        }

        public static int CreateDutyType(DutyType dt)
        {
            string sql = @"INSERT INTO DutyTypes
                (DutyName, Color, DefaultDurationDays, [Description], StartDayOfWeek)
                VALUES (?,?,?,?,?)";

            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("?", dt.DutyName);
                cmd.Parameters.AddWithValue("?", dt.Color);
                cmd.Parameters.AddWithValue("?", dt.DefaultDurationDays);
                cmd.Parameters.AddWithValue("?", ToDb(dt.Description));
                cmd.Parameters.AddWithValue("?", ToDb(dt.StartDayOfWeek));

                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT @@IDENTITY";
                cmd.Parameters.Clear();

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void UpdateDutyType(DutyType dt)
        {
            string sql = @"UPDATE DutyTypes SET
                DutyName=?,
                Color=?,
                DefaultDurationDays=?,
                [Description]=?,
                StartDayOfWeek=?
                WHERE Id=?";

            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("?", dt.DutyName);
                cmd.Parameters.AddWithValue("?", dt.Color);
                cmd.Parameters.AddWithValue("?", dt.DefaultDurationDays);
                cmd.Parameters.AddWithValue("?", ToDb(dt.Description));
                cmd.Parameters.AddWithValue("?", ToDb(dt.StartDayOfWeek));
                cmd.Parameters.AddWithValue("?", dt.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteDutyType(int id)
        {
            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand("DELETE FROM DutyTypes WHERE Id=?", conn))
            {
                cmd.Parameters.AddWithValue("?", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ── DutyAssignments ─────────────────────────────────────

        public static List<DutyAssignment> GetAssignments(DateTime? from = null, DateTime? to = null, string mode = null)
        {
            var list = new List<DutyAssignment>();

            string sql = @"SELECT da.*,
                      dt.DutyName AS DutyTypeName,
                      dt.Color AS DutyTypeColor,
                      c.CName AS CourseName
               FROM (DutyAssignments AS da
               INNER JOIN DutyTypes AS dt ON dt.Id = da.DutyTypeId)
               INNER JOIN Courses AS c ON c.Id = da.CourseId
               WHERE 1=1";

            if (from != null)
                sql += " AND da.EndDate >= ?";

            if (to != null)
                sql += " AND da.StartDate <= ?";

            if (mode != null)
                sql += " AND da.[Mode] = ?";

            sql += " ORDER BY da.StartDate";

            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand(sql, conn))
            {
                if (from != null)
                    cmd.Parameters.AddWithValue("?", from.Value);

                if (to != null)
                    cmd.Parameters.AddWithValue("?", to.Value);

                if (mode != null)
                    cmd.Parameters.AddWithValue("?", mode);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(ReadAssignment(r));
                }
            }

            return list;
        }

        public static int CreateAssignment(DutyAssignment a)
        {
            string sql = @"INSERT INTO DutyAssignments
                        (CourseId,DutyTypeId,StartDate,EndDate,[Mode],Notes)
                        VALUES (?,?,?,?,?,?)";

            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("?", a.CourseId);
                cmd.Parameters.AddWithValue("?", a.DutyTypeId);
                cmd.Parameters.AddWithValue("?", a.StartDate);
                cmd.Parameters.AddWithValue("?", a.EndDate);
                cmd.Parameters.AddWithValue("?", a.Mode ?? "planning");
                cmd.Parameters.AddWithValue("?", ToDb(a.Notes));

                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT @@IDENTITY";
                cmd.Parameters.Clear();

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void UpdateAssignment(DutyAssignment a)
        {
            string sql = @"UPDATE DutyAssignments SET
                        CourseId=?,
                        DutyTypeId=?,
                        StartDate=?,
                        EndDate=?,
                        [Mode]=?,
                        Notes=?
                        WHERE Id=?";

            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("?", a.CourseId);
                cmd.Parameters.AddWithValue("?", a.DutyTypeId);
                cmd.Parameters.AddWithValue("?", a.StartDate);
                cmd.Parameters.AddWithValue("?", a.EndDate);
                cmd.Parameters.AddWithValue("?", a.Mode ?? "planning");
                cmd.Parameters.AddWithValue("?", ToDb(a.Notes));
                cmd.Parameters.AddWithValue("?", a.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public static void ConfirmAssignment(int id)
        {
            string sql = "UPDATE DutyAssignments SET [Mode]=?, ConfirmedAt=? WHERE Id=?";

            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("?", "active");
                cmd.Parameters.AddWithValue("?", DateTime.Now);
                cmd.Parameters.AddWithValue("?", id);

                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteAssignment(int id)
        {
            using (var conn = DbHandler.Open())
            using (var cmd = new OleDbCommand("DELETE FROM DutyAssignments WHERE Id=?", conn))
            {
                cmd.Parameters.AddWithValue("?", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ── Readers ─────────────────────────────────────────────

        private static Course ReadCourse(OleDbDataReader r)
        {
            return new Course
            {
                Id = Convert.ToInt32(r["Id"]),
                CName = r["CName"].ToString(),
                CourseNum = r["CourseNum"].ToString(),
                ProfessionNum = Convert.ToInt32(r["ProfessionNum"]),
                IsActive = Convert.ToBoolean(r["IsActive"]),
                WorkManager = r["WorkManager"] == DBNull.Value ? null : r["WorkManager"].ToString(),
                StartDate = Convert.ToDateTime(r["StartDate"]),
                EndDate = Convert.ToDateTime(r["EndDate"]),
                TraineesCount = Convert.ToInt32(r["TraineesCount"])
            };
        }

        private static DutyType ReadDutyType(OleDbDataReader r) { return null; }
       /* {
            return new DutyType
            {
                Id = Convert.ToInt32(r["Id"]),
                DutyName = r["DutyName"].ToString(),
                Color = r["Color"].ToString(),
                DefaultDurationDays = Convert.ToInt32(r["DefaultDurationDays"]),
                Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
                StartDayOfWeek = r["StartDayOfWeek"] == DBNull.Value ? null : r["StartDayOfWeek"].ToString()
            };
        }
        
    private static DutyAssignment ReadAssignment(OleDbDataReader r)
    {
        return new DutyAssignment
        {
            Id = Convert.ToInt32(r["Id"]),
            CourseId = Convert.ToInt32(r["CourseId"]),
            DutyTypeId = Convert.ToInt32(r["DutyTypeId"]),
            StartDate = Convert.ToDateTime(r["StartDate"]),
            EndDate = Convert.ToDateTime(r["EndDate"]),
            Mode = r["Mode"].ToString(),
            Notes = r["Notes"] == DBNull.Value ? null : r["Notes"].ToString(),
            ConfirmedAt = r["ConfirmedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["ConfirmedAt"]),
            DutyTypeName = HasColumn(r, "DutyTypeName") ? r["DutyTypeName"].ToString() : null,
            DutyTypeColor = HasColumn(r, "DutyTypeColor") ? r["DutyTypeColor"].ToString() : null,
            CourseName = HasColumn(r, "CourseName") ? r["CourseName"].ToString() : null
        };
    }

    private static bool HasColumn(OleDbDataReader r, string col)
    {
        for (int i = 0; i < r.FieldCount; i++)
        {
            if (r.GetName(i).Equals(col, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static object ToDb(object value)
    {
        return value == null ? DBNull.Value : value;
    }
 */ 
