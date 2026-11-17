<%@ Page Title="" Language="C#" MasterPageFile="~/WebSIte.Master" AutoEventWireup="true" CodeBehind="PlanningGantt.aspx.cs" Inherits="BislaTichnunWeb.PlanningGantt" %>

<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="BislaTichnunWeb" %>

<asp:Content ID="C1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="<%= ResolveUrl("~/StyleSheets/PlanningGantt.css") %>" rel="stylesheet" type="text/css" />

    <asp:Panel ID="pnlNoDutyTypes" runat="server" Visible="false">
        <div class="no-duty-types">
            <h2>לא הוגדרו סוגי תורנויות</h2>
            <p>יש להוסיף לפחות סוג תורנות אחד כדי להשתמש בלוח התכנון.</p>
            <button type="button" onclick="location.href='DutyTypes.aspx';">הוסף סוג תורנות</button>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlPlanner" runat="server">
        <div class="gantt-page" dir="rtl">

            <div class="page-header">
                <div>
                    <div class="page-title">לוח תכנון קורסים</div>
                    <div class="page-subtitle">תכנון תורנויות ואירועים לפי קורס ושבוע</div>
                </div>
                <div class="header-actions">
                    <div class="mode-group">
                        <button type="button" class="mode-btn<%= CurrentViewScale == ViewScale.Month ? " active-planning" : "" %>" onclick="location.href='<%= NavUrl(0,0,"month") %>'">שבועי</button>
                        <button type="button" class="mode-btn<%= CurrentViewScale == ViewScale.Quarter ? " active-planning" : "" %>" onclick="location.href='<%= NavUrl(0,0,"quarter") %>'">תמונה גדולה</button>
                    </div>
                    <div class="mode-group">
                        <button type="button" class="mode-btn active-planning" data-mode="planning" onclick="BISLAT.setViewMode('planning'); return false;">✎ רובד תכנוני</button>
                        <button type="button" class="mode-btn" data-mode="active" onclick="BISLAT.setViewMode('active'); return false;">🔒 רובד פעיל</button>
                    </div>
                </div>
            </div>

            <div class="toolbar">
                <div class="month-nav">
                    <button type="button" onclick="location.href='<%= NavUrl(0,-1) %>'" title="תקופה קודמת">&#8250;</button>
                    <span class="month-label"><%= MonthTitle() %></span>
                    <button type="button" onclick="location.href='<%= NavUrl(0,1) %>'" title="תקופה הבאה">&#8249;</button>
                </div>
                <div class="legend">
                    <% foreach (DutyType dt in DutyTypes)
                        { %>
                    <span class="legend-item">
                        <span class="legend-dot" style="background: <%= dt.Color %>"></span>
                        <%= Server.HtmlEncode(dt.DutyName) %>
                    </span>
                    <% } %>
                    <span class="legend-item approved-legend">✓ מאושר</span>
                </div>
            </div>

            <div class="excel-tools">
                <div class="excel-search">
                    <label for="ganttGlobalSearch">חיפוש בכל הקורסים</label>
                    <input type="text"
                        id="ganttGlobalSearch"
                        placeholder="שם קורס, מספר, גף, מקצוע..."
                        autocomplete="off" />
                </div>

                <div class="excel-tool-actions">
                    <button type="button" class="excel-button" id="selectVisibleRows">
                        בחירת כל השורות המוצגות
                    </button>

                    <button type="button" class="excel-button" id="clearGanttFilters">
                        נקה סינון
                    </button>

                    <div class="columns-menu" id="columnsMenu">
                        <button type="button" class="excel-button" id="columnsMenuButton">
                            ⚙ עמודות
                        </button>

                        <div class="columns-menu-content" id="columnsMenuContent">
                            <div class="columns-menu-title">הצג או הסתר עמודות</div>
                            <div id="columnsMenuOptions"></div>
                        </div>
                    </div>
                </div>

                <div class="selection-status">
                    נבחרו <strong id="selectedRowsCount">0</strong> קורסים
        <span id="visibleRowsCount"></span>
                </div>
            </div>

            <div class="toolbar-hint">
                לחץ על תא ריק להוספה | גרור תורנות לשינוי מיקום
            </div>

            <div class="gantt-wrap">
                <!-- בנק התורנויות וטבלת ה-Gantt נשארים כאן ללא שינוי -->
            </div>

            <div class="gantt-wrap">

                <div class="duty-bank" id="duty-bank">
                    <div class="bank-header">
                        בנק תורנויות
                <div class="bank-sub">גרור לתא בגריד</div>
                    </div>
                    <div class="bank-items">
                        <% foreach (DutyType dt in DutyTypes)
                            {
                                string rgb = HexToRgb(dt.Color); %>
                        <div class="bank-chip" draggable="true"
                            data-dt-id="<%= dt.Id %>"
                            data-duration="<%= dt.DefaultDurationDays %>"
                            style="background: rgba(<%= rgb %>,.15); border-color: rgba(<%= rgb %>,.35); color: <%= dt.Color %>;">
                            <span class="bank-chip-dot" style="background: <%= dt.Color %>"></span>
                            <span><%= Server.HtmlEncode(dt.DutyName) %></span>
                            <span class="bank-chip-days"><%= dt.DefaultDurationDays %>י'</span>
                        </div>
                        <% } %>
                    </div>
                    <div class="bank-footer">ניתן גם לגרור תורנות קיימת</div>
                </div>

                <div class="gantt-scroll" id="ganttScroll">
                    <table class="gantt-table excel-gantt-table" id="ganttTable">
                        <thead>

                            <% if (CurrentViewScale == ViewScale.Quarter && MonthGroups.Count > 0)
                                { %>
                            <tr class="month-header">
                                <th class="month-metadata-header" colspan="8">פרטי קורס</th>
                                <% foreach (Tuple<string, int> mg in MonthGroups)
                                    { %>
                                <th class="month-group-header" colspan="<%= mg.Item2 %>"><%= mg.Item1 %></th>
                                <% } %>
                            </tr>
                            <% } %>

                            <tr class="week-header column-header-row">
                                <th class="select-column" data-col="select">
                                    <input type="checkbox" id="selectAllRows" title="בחר את כל השורות המוצגות" />
                                </th>

                                <th class="gaf-column" data-col="gaf">
                                        <%= RenderColumnFilter("gaf", "גף") %>
                                    <span class="column-resize-handle"></span>

                                </th>

                                <th data-col="courseNum">
                                        <%= RenderColumnFilter("courseNum", "מספר קורס") %>
                                        <span class="column-resize-handle"></span>

                                </th>

                                <th data-col="profession">
                                        <%= RenderColumnFilter("profession", "מקצוע") %>
                                        <span class="column-resize-handle"></span>

                                </th>

                                <th data-col="courseName">
                                        <%= RenderColumnFilter("courseName", "שם הקורס") %>
                                        <span class="column-resize-handle"></span>

                                </th>

                                <th data-col="trainees">
                                        <%= RenderColumnFilter("trainees", "חניכים") %>
                                        <span class="column-resize-handle"></span>

                                </th>

                                <th data-col="startDate">
                                        <%= RenderColumnFilter("startDate", "תאריך פתיחה") %>
                                        <span class="column-resize-handle"></span>

                                </th>

                                <th data-col="endDate">
                                        <%= RenderColumnFilter("endDate", "תאריך סיום") %>
                                        <span class="column-resize-handle"></span>

                                </th>

                                <% foreach (WeekInfo wk in Weeks)
                                    { %>
                                <th class="week-column" data-col="week_<%= wk.WeekStart.ToString("yyyyMMdd") %>"
                                    style="<%= CurrentViewScale == ViewScale.Quarter ? "min-width:85px;": "min-width:115px;" %>">
                                    <%= wk.Label %>
                                </th>
                                <% } %>
                            </tr>
                        </thead>

                        <tbody>
                            <% foreach (Course course in Courses)
                                {
                                    string startDateText = course.StartDate.ToString("dd/MM/yy");
                                    string endDateText = course.EndDate.ToString("dd/MM/yy");
                            %>
                            <tr class="course-row"
                                data-course-id="<%= course.Id %>"
                                data-gaf="<%= Server.HtmlEncode(course.Gaf ?? "") %>"
                                data-course-num="<%= Server.HtmlEncode(course.CourseNum ?? "") %>"
                                data-profession="<%= course.ProfessionNum %>"
                                data-course-name="<%= Server.HtmlEncode(course.CName ?? "") %>"
                                data-trainees="<%= course.TraineesCount %>"
                                data-start-date="<%= startDateText %>"
                                data-end-date="<%= endDateText %>">

                                <td class="select-column select-row-cell" data-col="select">
                                    <input type="checkbox" class="row-select" aria-label="בחר קורס" />
                                </td>
                                <td class="course-data-cell gaf-column" data-col="gaf"><%= Server.HtmlEncode(course.Gaf ?? "") %></td>
                                <td class="course-data-cell course-num-column" data-col="courseNum"><%= Server.HtmlEncode(course.CourseNum ?? "") %></td>
                                <td class="course-data-cell profession-column" data-col="profession"><%= course.ProfessionNum %></td>
                                <td class="course-data-cell course-name-column course-cell" data-col="courseName">
                                    <div class="course-name"><%= Server.HtmlEncode(course.CName ?? "") %></div>
                                </td>
                                <td class="course-data-cell trainees-column" data-col="trainees"><%= course.TraineesCount %></td>
                                <td class="course-data-cell date-column" data-col="startDate"><%= startDateText %></td>
                                <td class="course-data-cell date-column" data-col="endDate"><%= endDateText %></td>

                                <% foreach (WeekInfo wk in Weeks)
                                    {
                                        string wkKey = wk.WeekStart.ToString("yyyy-MM-dd");
                                        List<GanttItem> cellItems = new List<GanttItem>();
                                        if (GanttItemMap.ContainsKey(course.Id) && GanttItemMap[course.Id].ContainsKey(wkKey))
                                        {
                                            cellItems = GanttItemMap[course.Id][wkKey];
                                        }
                                %>
                                <td class="gantt-cell"
                                    data-col="week_<%= wk.WeekStart.ToString("yyyyMMdd") %>"
                                    data-course-id="<%= course.Id %>"
                                    data-course-name="<%= Server.HtmlEncode(course.CName ?? "") %>"
                                    data-week-start="<%= wkKey %>"
                                    data-week-end="<%= wk.WeekEnd.ToString("yyyy-MM-dd") %>">
                                    <div class="cell-inner">
                                        <% foreach (GanttItem item in cellItems)
                                            {
                                                string rgb = HexToRgb(item.Color ?? "#888");
                                        %>
                                        <div class="chip<%= item.IsApproved ? " confirmed" : "" %>"
                                            draggable="<%= item.IsDraggable ? "true" : "false" %>"
                                            data-id="<%= item.Id %>"
                                            data-item-type="<%= item.ItemType %>"
                                            data-course-id="<%= item.CourseId %>"
                                            data-start="<%= item.StartDate.ToString("yyyy-MM-dd") %>"
                                            data-end="<%= item.EndDate.ToString("yyyy-MM-dd") %>"
                                            data-notes="<%= Server.HtmlEncode(item.Notes ?? "") %>"
                                            style="background: rgba(<%= rgb %>,.18); border-right-color: <%= item.Color %>; color: <%= item.Color %>;">
                                            <span><%= Server.HtmlEncode(item.ItemName ?? "") %></span>
                                            <span class="chip-date"><%= item.StartDate.ToString("dd/MM") %> — <%= item.EndDate.ToString("dd/MM") %></span>
                                        </div>
                                        <% } %>
                                        <% if (cellItems.Count == 0)
       { %><div class="add-hint">+</div>
                                        <% } %>
                                    </div>
                                </td>
                                <% } %>
                            </tr>
                            <% } %>
                        </tbody>
                    </table>
                </div>
            </div>

            <div class="modal-overlay" id="assignmentModal">
                <div class="modal">
                    <div class="modal-header">
                        <span class="modal-title" id="modal-title">תורנות</span>
                        <span class="modal-subtitle" id="modal-course-name" style="font-size: 11px; color: #5a7fa0; margin-right: 8px;"></span>
                        <button type="button" class="modal-close" onclick="closeModal('assignmentModal')">&times;</button>
                    </div>
                    <div class="modal-body">
                        <input type="hidden" id="am-id" />
                        <input type="hidden" id="am-course-id" />
                        <div class="form-group">
                            <label class="form-label">סוג תורנות</label>
                            <% foreach (DutyType dt in DutyTypes)
                                { %>
                            <label class="duty-type-option">
                                <input type="radio" name="am-dt" value="<%= dt.Id %>" />
                                <span class="duty-type-dot" style="background: <%= dt.Color %>"></span>
                                <span><%= Server.HtmlEncode(dt.DutyName) %></span>
                                <span style="margin-right: auto; font-size: 10px; color: #4a6a88;"><%= dt.DefaultDurationDays %> י'</span>
                            </label>
                            <% } %>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label" for="am-start">מתאריך</label>
                                <input type="date" id="am-start" class="form-control" />
                            </div>
                            <div class="form-group">
                                <label class="form-label" for="am-end">עד תאריך</label>
                                <input type="date" id="am-end" class="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="form-label" for="am-notes">הערות</label>
                            <input type="text" id="am-notes" class="form-control" placeholder="הערות אופציונליות..." />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger btn-sm" id="btn-delete" onclick="deleteAssignment()">מחק</button>
                        <button type="button" class="btn btn-success btn-sm" id="btn-confirm" onclick="confirmAssignment()">✓ אשר</button>
                        <span style="flex: 1"></span>
                        <button type="button" class="btn btn-secondary btn-sm" onclick="closeModal('assignmentModal')">ביטול</button>
                        <button type="button" class="btn btn-primary btn-sm" onclick="saveAssignment()">שמור</button>
                    </div>
                </div>
            </div>

        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="C2" ContentPlaceHolderID="ScriptContent" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/PlanningGanttScript.js") %>"></script>
</asp:Content>

