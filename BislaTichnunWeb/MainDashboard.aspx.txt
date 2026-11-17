<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/WebSIte.Master"
    AutoEventWireup="true"
    CodeBehind="MainDashboard.aspx.cs"
    Inherits="BislaTichnunWeb.MainDashboard" %>

<%@ Import Namespace="BislaTichnunWeb" %>

    <asp:Content
    ID="C1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="page-header">
          <div>
            <div class="page-title">סקירה כללית</div>

            <div class="page-subtitle">
              מצב תורנויות —
              <%= DateTime.Today.ToString("dd/MM/yyyy") %>
            </div>
          </div>
        </div>

    <div class="page-body">

        <div class="stats-grid">

            <div class="stat-card">
              <div class="stat-value">
                <%= ActiveCourses %>
              </div>

              <div class="stat-label">
                קורסים פעילים
              </div>
            </div>

            <div class="stat-card">
              <div class="stat-value">
                <%= TotalTrainees %>
              </div>

              <div class="stat-label">
                סה"כ חניכים
              </div>
            </div>

            <div class="stat-card">
              <div class="stat-value">
                <%= TotalAssignments %>
              </div>

              <div class="stat-label">
                תורנויות מתוכננות
              </div>
            </div>

            <div class="stat-card">
              <div class="stat-value"
                   style="color:#3dbb7e;">

                <%= ConfirmedThisMonth %>
              </div>

              <div class="stat-label">
                מאושרות החודש
              </div>
            </div>

            <div class="stat-card">
              <div class="stat-value"
                   style="color:#f59e0b;">

                <%= PendingThisWeek %>
              </div>

              <div class="stat-label">
                בתכנון השבוע
              </div>
            </div>
  </div>

        <div class="card">

            <div class="card-header">
              <span class="card-title">
                תורנויות פעילות — השבוע
              </span>
            </div>

            <div class="card-body">

              <table class="data-table">

                <thead>
                  <tr>
                    <th>קורס</th>
                    <th>סוג תורנות</th>
                    <th>מתאריך</th>
                    <th>עד תאריך</th>
                    <th>סטטוס</th>
                  </tr>
                </thead>

                <tbody>

                  <% if (WeekAssignments.Count == 0) { %>

                  <tr>
                    <td colspan="5"
                        style="
                          text-align:center;
                          color:#3a5a78;
                          padding:20px;">

                      אין תורנויות השבוע
                    </td>
                  </tr>

                  <% } else { %>

                    <% foreach (var a in WeekAssignments) { %>

                    <tr>

                      <td>
                        <%= Server.HtmlEncode(
                            GetCourseName(a.CourseId)) %>
                      </td>

                      <td>
                        <span style="
                            color:<%= a.Color %>;
                            font-weight:bold;">

                          <span style="
                              display:inline-block;
                              width:10px;
                              height:10px;
                              border-radius:2px;
                              background:<%= a.Color %>;
                              margin-left:5px;">
                          </span>

                          <%= Server.HtmlEncode(a.ItemName) %>
                        </span>
                      </td>

                      <td>
                        <%= a.StartDate.ToString("dd/MM/yyyy") %>
                      </td>

                      <td>
                        <%= a.EndDate.ToString("dd/MM/yyyy") %>
                      </td>

                      <td>
                        <span class="badge
                            <%= a.Status == AssignmentStatus.Active
                                ? "badge-active"
                                : "badge-inactive" %>">

                          <%= a.Status == AssignmentStatus.Active
                              ? "מאושר"
                              : "בתכנון" %>
                        </span>
                      </td>

                    </tr>

                    <% } %>

                  <% } %>
                </tbody>
        </table>
    </div>
  </div>

</div>

</asp:Content>