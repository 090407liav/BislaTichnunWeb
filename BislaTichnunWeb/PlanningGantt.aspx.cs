using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;

namespace BislaTichnunWeb
{
    public partial class PlanningGantt : Page
    {
        protected List<Course> Courses = new List<Course>();
        protected List<DutyType> DutyTypes = new List<DutyType>();
        protected List<WeekInfo> Weeks = new List<WeekInfo>();
        protected List<DutyAssignment> Assignments = new List<DutyAssignment>();
        protected List<CourseEvent> CourseEvents = new List<CourseEvent>();

        /*
         * הרשימה המשותפת שמכילה גם תורנויות וגם אירועי קורס.
         */
        protected List<GanttItem> GanttItems = 
            new List<GanttItem>();

        protected int CurrentYear;
        protected int CurrentMonth; // 0–11

        protected ViewScale CurrentViewScale =
            ViewScale.Month;

        /*
         * [CourseId][WeekStart] -> Gantt items
         */
        protected Dictionary<
            int,
            Dictionary<string, List<GanttItem>>
        > GanttItemMap =
            new Dictionary<
                int,
                Dictionary<string, List<GanttItem>>
            >();

        /*
         * שם חודש + מספר השבועות ששייכים אליו.
         * משמש לכותרת תצוגת הרבעון.
         */
        protected List<Tuple<string, int>> MonthGroups =
            new List<Tuple<string, int>>();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            SetCurrentPeriodFromRequest();

            LoadData();

            bool hasDutyTypes =
                DutyTypes != null &&
                DutyTypes.Count > 0;

            pnlNoDutyTypes.Visible =
                !hasDutyTypes;

            pnlPlanner.Visible =
                hasDutyTypes;
        }

        private void SetCurrentPeriodFromRequest()
        {
            CurrentYear =
                DateTime.Today.Year;

            CurrentMonth =
                DateTime.Today.Month - 1;

            int queryYear;

            if (int.TryParse(
                    Request.QueryString["y"],
                    out queryYear))
            {
                CurrentYear = queryYear;
            }

            int queryMonth;

            if (int.TryParse(
                    Request.QueryString["m"],
                    out queryMonth) &&
                queryMonth >= 0 &&
                queryMonth <= 11)
            {
                CurrentMonth = queryMonth;
            }

            CurrentViewScale =
                ParseViewScale(
                    Request.QueryString["scale"]);
        }

        private void LoadData()
        {
            DutyTypes =
                DataAccess.GetDutyTypes();

            Courses =
                DataAccess.GetCourses(
                    activeOnly: true);

            DateTime rangeStart;
            DateTime rangeEnd;

            CalculateVisibleRange(
                out rangeStart,
                out rangeEnd);

            BuildWeeks(
                rangeStart,
                rangeEnd);

            BuildMonthGroups();

            LoadGanttItems(
                rangeStart,
                rangeEnd);

            BuildGanttItemMap();
        }

        private void CalculateVisibleRange(
            out DateTime rangeStart,
            out DateTime rangeEnd)
        {
            DateTime selectedMonth =
                new DateTime(
                    CurrentYear,
                    CurrentMonth + 1,
                    1);

            if (CurrentViewScale ==
                ViewScale.Quarter)
            {
                /*
                 * חודש קודם + חודש נוכחי + חודש הבא.
                 */
                rangeStart =
                    StartOfWeek(
                        selectedMonth.AddMonths(-1));

                DateTime lastDay =
                    selectedMonth
                        .AddMonths(2)
                        .AddDays(-1);

                rangeEnd =
                    EndOfWeek(lastDay);
            }
            else
            {
                rangeStart =
                    StartOfWeek(selectedMonth);

                DateTime lastDay =
                    new DateTime(
                        CurrentYear,
                        CurrentMonth + 1,
                        DateTime.DaysInMonth(
                            CurrentYear,
                            CurrentMonth + 1));

                rangeEnd =
                    EndOfWeek(lastDay);
            }
        }

        private void BuildWeeks(
            DateTime rangeStart,
            DateTime rangeEnd)
        {
            Weeks =
                new List<WeekInfo>();

            DateTime weekStart =
                rangeStart;

            while (weekStart <= rangeEnd)
            {
                DateTime calculatedWeekEnd =
                    weekStart.AddDays(6);

                DateTime weekEnd =
                    calculatedWeekEnd < rangeEnd
                        ? calculatedWeekEnd
                        : rangeEnd;

                Weeks.Add(
                    new WeekInfo
                    {
                        WeekStart = weekStart,
                        WeekEnd = weekEnd,

                        Label =
                            weekStart.ToString("dd/MM") +
                            " - " +
                            calculatedWeekEnd.ToString("dd/MM")
                    });

                weekStart =
                    weekStart.AddDays(7);
            }
        }

        private void BuildMonthGroups()
        {
            MonthGroups =
                new List<Tuple<string, int>>();

            if (CurrentViewScale !=
                ViewScale.Quarter)
            {
                return;
            }

            CultureInfo hebrewCulture =
                CultureInfo.GetCultureInfo(
                    "he-IL");

            string currentLabel = null;
            int currentCount = 0;

            foreach (WeekInfo week in Weeks)
            {
                string label =
                    week.WeekStart.ToString(
                        "MMMM yyyy",
                        hebrewCulture);

                if (label == currentLabel)
                {
                    currentCount++;
                    continue;
                }

                if (currentLabel != null)
                {
                    MonthGroups.Add(
                        Tuple.Create(
                            currentLabel,
                            currentCount));
                }

                currentLabel = label;
                currentCount = 1;
            }

            if (currentLabel != null)
            {
                MonthGroups.Add(
                    Tuple.Create(
                        currentLabel,
                        currentCount));
            }
        }

        private void LoadGanttItems(
            DateTime rangeStart,
            DateTime rangeEnd)
        {
            Assignments =
                DataAccess.GetAssignments(
                    rangeStart,
                    rangeEnd);

            CourseEvents =
                DataAccess.GetCourseEvents(
                    rangeStart,
                    rangeEnd);

            /*
             * מוסיפים אוטומטית אירועי פתיחה וסיום
             * לפי הנתונים של הקורסים.
             */
            List<CourseEvent> courseBoundaryEvents =
                BuildCourseBoundaryEvents(
                    rangeStart,
                    rangeEnd);

            CourseEvents.AddRange(
                courseBoundaryEvents);

            GanttItems =
                new List<GanttItem>();

            GanttItems.AddRange(
                Assignments);

            GanttItems.AddRange(
                CourseEvents);
        }

        private List<CourseEvent>
            BuildCourseBoundaryEvents(
                DateTime rangeStart,
                DateTime rangeEnd)
        {
            List<CourseEvent> events =
                new List<CourseEvent>();

            /*
             * מזהים שליליים כדי שלא יתנגשו
             * עם IDs של אירועים מהמסד.
             */
            int generatedId = -1;

            foreach (Course course in Courses)
            {
                if (course.StartDate.Date >=
                        rangeStart.Date &&
                    course.StartDate.Date <=
                        rangeEnd.Date)
                {
                    events.Add(
                        new CourseEvent
                        {
                            Id = generatedId--,
                            CourseId = course.Id,

                            EventType =
                                CourseEventType.CourseOpen,

                            ItemName =
                                "פתיחת קורס",

                            Color =
                                "#22C55E",

                            StartDate =
                                course.StartDate.Date,

                            EndDate =
                                course.StartDate.Date,

                            Notes =
                                string.Empty,

                            Draggable =
                                false
                        });
                }

                if (course.EndDate.Date >=
                        rangeStart.Date &&
                    course.EndDate.Date <=
                        rangeEnd.Date)
                {
                    events.Add(
                        new CourseEvent
                        {
                            Id = generatedId--,
                            CourseId = course.Id,

                            EventType =
                                CourseEventType.CourseClose,

                            ItemName =
                                "סיום קורס",

                            Color =
                                "#EF4444",

                            StartDate =
                                course.EndDate.Date,

                            EndDate =
                                course.EndDate.Date,

                            Notes =
                                string.Empty,

                            Draggable =
                                false
                        });
                }
            }

            return events;
        }

        private void BuildGanttItemMap()
        {
            GanttItemMap =
                new Dictionary<
                    int,
                    Dictionary<
                        string,
                        List<GanttItem>
                    >
                >();

            foreach (Course course in Courses)
            {
                GanttItemMap[course.Id] =
                    new Dictionary<
                        string,
                        List<GanttItem>
                    >();
            }

            foreach (GanttItem item in GanttItems)
            {
                /*
                 * אירועים של קורסים שאינם מופיעים כרגע
                 * בגאנט לא ייכנסו למפה.
                 */
                if (!GanttItemMap.ContainsKey(
                        item.CourseId))
                {
                    continue;
                }

                foreach (WeekInfo week in Weeks)
                {
                    bool overlapsWeek =
                        item.StartDate <=
                            week.WeekEnd &&
                        item.EndDate >=
                            week.WeekStart;

                    if (!overlapsWeek)
                    {
                        continue;
                    }

                    string weekKey =
                        week.WeekStart.ToString(
                            "yyyy-MM-dd");

                    Dictionary<
                        string,
                        List<GanttItem>
                    > courseWeeks =
                        GanttItemMap[
                            item.CourseId];

                    if (!courseWeeks.ContainsKey(
                            weekKey))
                    {
                        courseWeeks[weekKey] =
                            new List<GanttItem>();
                    }

                    /*
                     * ItemType נכלל בבדיקה משום ש-ID
                     * של Assignment ושל CourseEvent
                     * עלול להיות זהה.
                     */
                    bool alreadyAdded =
                        courseWeeks[weekKey]
                            .Any(existingItem =>
                                existingItem.Id ==
                                    item.Id &&
                                existingItem.ItemType ==
                                    item.ItemType);

                    if (!alreadyAdded)
                    {
                        courseWeeks[weekKey]
                            .Add(item);
                    }
                }
            }
        }

        protected string NavUrl(
            int yearOffset,
            int monthOffset,
            string scale = null)
        {
            int year =
                CurrentYear;

            int month =
                CurrentMonth +
                monthOffset +
                yearOffset * 12;

            while (month < 0)
            {
                month += 12;
                year--;
            }

            while (month > 11)
            {
                month -= 12;
                year++;
            }

            ViewScale targetScale =
                scale == null
                    ? CurrentViewScale
                    : ParseViewScale(scale);

            return string.Format(
                "PlanningGantt.aspx?y={0}&m={1}&scale={2}",
                year,
                month,
                ToQueryString(targetScale));
        }

        protected string MonthTitle()
        {
            string[] months =
            {
                "ינואר",
                "פברואר",
                "מרץ",
                "אפריל",
                "מאי",
                "יוני",
                "יולי",
                "אוגוסט",
                "ספטמבר",
                "אוקטובר",
                "נובמבר",
                "דצמבר"
            };

            if (CurrentViewScale ==
                ViewScale.Quarter)
            {
                DateTime selectedMonth =
                    new DateTime(
                        CurrentYear,
                        CurrentMonth + 1,
                        1);

                DateTime firstMonth =
                    selectedMonth.AddMonths(-1);

                DateTime lastMonth =
                    selectedMonth.AddMonths(1);

                return
                    months[firstMonth.Month - 1] +
                    " " +
                    firstMonth.Year +
                    " — " +
                    months[lastMonth.Month - 1] +
                    " " +
                    lastMonth.Year;
            }

            return
                months[CurrentMonth] +
                " " +
                CurrentYear;
        }

        protected bool IsQuarterView
        {
            get
            {
                return CurrentViewScale ==
                       ViewScale.Quarter;
            }
        }

        protected string CurrentViewScaleValue
        {
            get
            {
                return ToQueryString(
                    CurrentViewScale);
            }
        }

        private static ViewScale ParseViewScale(
            string value)
        {
            if (string.Equals(
                    value,
                    "quarter",
                    StringComparison.OrdinalIgnoreCase))
            {
                return ViewScale.Quarter;
            }

            /*
             * גם "week" וגם "month" מחזירים Month,
             * כדי שה-ASPX הקיים ימשיך לעבוד.
             */
            return ViewScale.Month;
        }

        private static string ToQueryString(
            ViewScale scale)
        {
            return scale ==
                   ViewScale.Quarter
                ? "quarter"
                : "week";
        }

        private static DateTime StartOfWeek(
            DateTime date)
        {
            return date.Date.AddDays(
                -(int)date.DayOfWeek);
        }

        private static DateTime EndOfWeek(
            DateTime date)
        {
            return StartOfWeek(date)
                .AddDays(6);
        }

        protected string HexToRgb(string hex)
        {
            const string defaultRgb = "26,111,168";

            if (string.IsNullOrWhiteSpace(hex))
            {
                return defaultRgb;
            }

            hex = hex.TrimStart('#');

            if (hex.Length != 6)
            {
                return defaultRgb;
            }

            int red;
            int green;
            int blue;

            bool redIsValid = int.TryParse(
                hex.Substring(0, 2),
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out red);

            bool greenIsValid = int.TryParse(
                hex.Substring(2, 2),
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out green);

            bool blueIsValid = int.TryParse(
                hex.Substring(4, 2),
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out blue);

            if (!redIsValid || !greenIsValid || !blueIsValid)
            {
                return defaultRgb;
            }

            return string.Format(
                "{0},{1},{2}",
                red,
                green,
                blue);
        }

        protected string RenderColumnFilter(string key, string label)
        {
            string html = "";

            html += "<div class='excel-filter-wrap'>";
            html += "<span>" + Server.HtmlEncode(label) + "</span>";

            html += "<button type='button' id='" + key +
                "FilterButton' class='excel-filter-button' " +
                "title='סינון לפי " + Server.HtmlEncode(label) + "'>▼</button>";

            html += "<div id='" + key +
                "FilterMenu' class='excel-filter-menu' style='display:none;'>";

            html += "<input type='text' id='" + key +
                "FilterSearch' class='excel-filter-search' " +
                "placeholder='חיפוש " + Server.HtmlEncode(label) +
                "...' autocomplete='off' />";

            html += "<label class='excel-filter-select-all'>";
            html += "<input type='checkbox' id='" + key +
                "SelectAll' checked='checked' /> בחר הכול";
            html += "</label>";

            html += "<div id='" + key +
                "FilterOptions' class='excel-filter-options'></div>";

            html += "<div class='excel-filter-actions'>";

            html += "<button type='button' id='" + key +
                "FilterApply' class='excel-button'>אישור</button>";

            html += "<button type='button' id='" + key +
                "FilterCancel' class='excel-button'>ביטול</button>";

            html += "</div></div></div>";

            return html;
        }
    }
}