using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BislaTichnunWeb
{
    public static class DataAccess
    {
        // ── Courses ────────────────────────────────────────────────────────────

        public static List<Course> GetCourses(bool activeOnly = false)
        {
            var list = new List<Course>();
            string sql = activeOnly
                ? "SELECT * FROM Courses WHERE IsActive=1 ORDER BY CourseNumber"
                : "SELECT * FROM Courses ORDER BY CourseNumber";
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand(sql, conn))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(ReadCourse(r));
            return list;
        }

        public static Course GetCourse(int id)
        {
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand("SELECT * FROM Courses WHERE Id=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? ReadCourse(r) : null;
            }
        }

        public static int CreateCourse(Course c)
        {
            string sql = @"INSERT INTO Courses (Name,CourseNumber,StartDate,EndDate,TraineesCount,Specialty,IsActive,Notes)
                           VALUES (@n,@cn,@sd,@ed,@tc,@sp,@ia,@no); SELECT SCOPE_IDENTITY();";
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@n", c.Name);
                cmd.Parameters.AddWithValue("@cn", c.CourseNumber);
                cmd.Parameters.AddWithValue("@sd", c.StartDate);
                cmd.Parameters.AddWithValue("@ed", c.EndDate);
                cmd.Parameters.AddWithValue("@tc", c.TraineesCount);
                cmd.Parameters.AddWithValue("@sp", (object)c.Specialty ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ia", c.IsActive);
                cmd.Parameters.AddWithValue("@no", (object)c.Notes ?? DBNull.Value);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void UpdateCourse(Course c)
        {
            string sql = @"UPDATE Courses SET Name=@n,CourseNumber=@cn,StartDate=@sd,EndDate=@ed,
                           TraineesCount=@tc,Specialty=@sp,IsActive=@ia,Notes=@no WHERE Id=@id";
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", c.Id);
                cmd.Parameters.AddWithValue("@n", c.Name);
                cmd.Parameters.AddWithValue("@cn", c.CourseNumber);
                cmd.Parameters.AddWithValue("@sd", c.StartDate);
                cmd.Parameters.AddWithValue("@ed", c.EndDate);
                cmd.Parameters.AddWithValue("@tc", c.TraineesCount);
                cmd.Parameters.AddWithValue("@sp", (object)c.Specialty ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ia", c.IsActive);
                cmd.Parameters.AddWithValue("@no", (object)c.Notes ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteCourse(int id)
        {
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand("DELETE FROM Courses WHERE Id=@id", conn))
            { cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery(); }
        }

        // ── DutyTypes ──────────────────────────────────────────────────────────

        public static List<DutyType> GetDutyTypes()
        {
            var list = new List<DutyType>();
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand("SELECT * FROM DutyTypes ORDER BY Name", conn))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(ReadDutyType(r));
            return list;
        }

        public static int CreateDutyType(DutyType dt)
        {
            string sql = @"INSERT INTO DutyTypes (Name,Color,DefaultDurationDays,Description,StartDayOfWeek)
                           VALUES (@n,@c,@d,@desc,@sdow); SELECT SCOPE_IDENTITY();";
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@n", dt.Name);
                cmd.Parameters.AddWithValue("@c", dt.Color);
                cmd.Parameters.AddWithValue("@d", dt.DefaultDurationDays);
                cmd.Parameters.AddWithValue("@desc", (object)dt.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@sdow", (object)dt.StartDayOfWeek ?? DBNull.Value);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void UpdateDutyType(DutyType dt)
        {
            string sql = @"UPDATE DutyTypes SET Name=@n,Color=@c,DefaultDurationDays=@d,
                           Description=@desc,StartDayOfWeek=@sdow WHERE Id=@id";
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", dt.Id);
                cmd.Parameters.AddWithValue("@n", dt.Name);
                cmd.Parameters.AddWithValue("@c", dt.Color);
                cmd.Parameters.AddWithValue("@d", dt.DefaultDurationDays);
                cmd.Parameters.AddWithValue("@desc", (object)dt.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@sdow", (object)dt.StartDayOfWeek ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteDutyType(int id)
        {
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand("DELETE FROM DutyTypes WHERE Id=@id", conn))
            { cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery(); }
        }

        // ── DutyAssignments ────────────────────────────────────────────────────

        public static List<DutyAssignment> GetAssignments(DateTime? from = null, DateTime? to = null, string mode = null)
        {
            var list = new List<DutyAssignment>();
            string sql = @"SELECT da.*,dt.Name AS DutyTypeName,dt.Color AS DutyTypeColor,c.Name AS CourseName
                           FROM DutyAssignments da
                           INNER JOIN DutyTypes dt ON dt.Id=da.DutyTypeId
                           INNER JOIN Courses c ON c.Id=da.CourseId
                           WHERE 1=1";
            if (from != null) sql += " AND da.EndDate >= @from";
            if (to != null) sql += " AND da.StartDate <= @to";
            if (mode != null) sql += " AND da.Mode = @mode";
            sql += " ORDER BY da.StartDate";

            using (var conn = DB.Open())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (from != null) cmd.Parameters.AddWithValue("@from", from.Value);
                if (to != null) cmd.Parameters.AddWithValue("@to", to.Value);
                if (mode != null) cmd.Parameters.AddWithValue("@mode", mode);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(ReadAssignment(r));
            }
            return list;
        }

        public static int CreateAssignment(DutyAssignment a)
        {
            string sql = @"INSERT INTO DutyAssignments (CourseId,DutyTypeId,StartDate,EndDate,Mode,Notes)
                           VALUES (@ci,@dti,@sd,@ed,@m,@no); SELECT SCOPE_IDENTITY();";
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ci", a.CourseId);
                cmd.Parameters.AddWithValue("@dti", a.DutyTypeId);
                cmd.Parameters.AddWithValue("@sd", a.StartDate);
                cmd.Parameters.AddWithValue("@ed", a.EndDate);
                cmd.Parameters.AddWithValue("@m", a.Mode ?? "planning");
                cmd.Parameters.AddWithValue("@no", (object)a.Notes ?? DBNull.Value);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void UpdateAssignment(DutyAssignment a)
        {
            string sql = @"UPDATE DutyAssignments SET CourseId=@ci,DutyTypeId=@dti,StartDate=@sd,
                           EndDate=@ed,Mode=@m,Notes=@no WHERE Id=@id";
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", a.Id);
                cmd.Parameters.AddWithValue("@ci", a.CourseId);
                cmd.Parameters.AddWithValue("@dti", a.DutyTypeId);
                cmd.Parameters.AddWithValue("@sd", a.StartDate);
                cmd.Parameters.AddWithValue("@ed", a.EndDate);
                cmd.Parameters.AddWithValue("@m", a.Mode ?? "planning");
                cmd.Parameters.AddWithValue("@no", (object)a.Notes ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ConfirmAssignment(int id)
        {
            string sql = "UPDATE DutyAssignments SET Mode='active', ConfirmedAt=GETDATE() WHERE Id=@id";
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand(sql, conn))
            { cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery(); }
        }

        public static void DeleteAssignment(int id)
        {
            using (var conn = DB.Open())
            using (var cmd = new SqlCommand("DELETE FROM DutyAssignments WHERE Id=@id", conn))
            { cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery(); }
        }

        // ── Private readers ────────────────────────────────────────────────────

        private static Course ReadCourse(SqlDataReader r)
        {
            return new Course
            {
                Id = (int)r["Id"],
                Name = r["Name"].ToString(),
                CourseNumber = r["CourseNumber"].ToString(),
                StartDate = (DateTime)r["StartDate"],
                EndDate = (DateTime)r["EndDate"],
                TraineesCount = (int)r["TraineesCount"],
                Specialty = r["Specialty"] == DBNull.Value ? null : r["Specialty"].ToString(),
                IsActive = (bool)r["IsActive"],
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"].ToString(),
            };
        }

        private static DutyType ReadDutyType(SqlDataReader r)
        {
            return new DutyType
            {
                Id = (int)r["Id"],
                Name = r["Name"].ToString(),
                Color = r["Color"].ToString(),
                DefaultDurationDays = (int)r["DefaultDurationDays"],
                Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
                StartDayOfWeek = r["StartDayOfWeek"] == DBNull.Value ? null : r["StartDayOfWeek"].ToString(),
            };
        }

        private static DutyAssignment ReadAssignment(SqlDataReader r)
        {
            return new DutyAssignment
            {
                Id = (int)r["Id"],
                CourseId = (int)r["CourseId"],
                DutyTypeId = (int)r["DutyTypeId"],
                StartDate = (DateTime)r["StartDate"],
                EndDate = (DateTime)r["EndDate"],
                Mode = r["Mode"].ToString(),
                Notes = r["Notes"] == DBNull.Value ? null : r["Notes"].ToString(),
                ConfirmedAt = r["ConfirmedAt"] == DBNull.Value ? (DateTime?)null : (DateTime)r["ConfirmedAt"],
                DutyTypeName = r.GetSchemaTable() != null && HasColumn(r, "DutyTypeName") ? r["DutyTypeName"].ToString() : null,
                DutyTypeColor = r.GetSchemaTable() != null && HasColumn(r, "DutyTypeColor") ? r["DutyTypeColor"].ToString() : null,
                CourseName = r.GetSchemaTable() != null && HasColumn(r, "CourseName") ? r["CourseName"].ToString() : null,
            };
        }

        private static bool HasColumn(SqlDataReader r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (r.GetName(i).Equals(col, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}