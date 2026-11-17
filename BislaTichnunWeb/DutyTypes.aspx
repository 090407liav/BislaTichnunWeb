<%@ Page Title="סוגי תורנויות" Language="C#" MasterPageFile="~/WebSite.Master"
         AutoEventWireup="true" CodeFile="DutyTypes.aspx.cs" Inherits="BislaTichnunWeb.DutyTypes" %>

<asp:Content ID="C1" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
  <div>
    <div class="page-title">סוגי תורנויות</div>
    <div class="page-subtitle">הגדרת סוגים, צבעים ומשך ברירת מחדל</div>
  </div>
  <button class="btn btn-primary" onclick="openDtModal(null)">+ סוג חדש</button>
</div>

<div class="page-body">
  <div class="card">
    <div class="card-body">
      <table class="data-table">
        <thead>
          <tr>
            <th>צבע</th><th>שם</th><th>משך ברירת מחדל</th><th>תיאור</th><th>יום התחלה</th><th>פעולות</th>
          </tr>
        </thead>
        <tbody>
          <% foreach (var dt in lstDutyTypes) { %>
          <tr>
            <td><span class="color-swatch" style="background:<%= dt.Color %>;"></span></td>
            <td style="font-weight:bold;color:<%= dt.Color %>;"><%= Server.HtmlEncode(dt.DutyName) %></td>
            <td><%= dt.DefaultDurationDays %> ימים</td>
            <td><%= Server.HtmlEncode(dt.Description ?? "-") %></td>
            <td><%= Server.HtmlEncode(dt.StartDayOfWeek.HasValue ? dt.StartDayOfWeek.ToString() : "-") %></td>              <td>
              <button class="btn btn-secondary btn-sm"
                onclick="openDtModal(<%= dt.Id %>,'<%= EscJs(dt.DutyName) %>','<%= EscJs(dt.Color) %>',<%= dt.DefaultDurationDays %>,'<%= EscJs(dt.Description??"") %>'">
                עריכה
              </button>
              <button class="btn btn-danger btn-sm" onclick="deleteDt(<%= dt.Id %>)">מחק</button>
            </td>
          </tr>
          <% } %>
          <% if (lstDutyTypes.Count == 0) { %>
          <tr><td colspan="6" style="text-align:center;color:#3a5a78;padding:20px;">אין סוגי תורנויות</td></tr>
          <% } %>
        </tbody>
      </table>
    </div>
  </div>
</div>

<!-- Modal -->
<div class="modal-overlay" id="dtModal">
  <div class="modal">
    <div class="modal-header">
      <span class="modal-title" id="dt-modal-title">סוג תורנות</span>
      <button class="modal-close" onclick="closeModal('dtModal')">&times;</button>
    </div>
    <div class="modal-body">
      <input type="hidden" id="dt-id" />
      <div class="form-group">
        <label class="form-label">שם *</label>
        <input type="text" id="dt-name" class="form-control" />
      </div>
      <div class="form-row">
        <div class="form-group">
          <label class="form-label">צבע</label>
          <input type="color" id="dt-color" class="form-control" style="height:36px;padding:2px;" />
        </div>
        <div class="form-group">
          <label class="form-label">משך ברירת מחדל (ימים)</label>
          <input type="number" id="dt-duration" class="form-control" min="1" max="30" value="7" />
        </div>
      </div>
      <div class="form-group">
        <label class="form-label">תיאור</label>
        <input type="text" id="dt-desc" class="form-control" />
      </div>
      <div class="form-group">
        <label class="form-label">יום התחלה</label>
        <select id="dt-startday" class="form-control">
          <option value="">— ללא —</option>
          <option value="sunday">ראשון</option>
          <option value="monday">שני</option>
          <option value="tuesday">שלישי</option>
          <option value="wednesday">רביעי</option>
          <option value="thursday">חמישי</option>
          <option value="friday">שישי</option>
        </select>
      </div>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal('dtModal')">ביטול</button>
      <button class="btn btn-primary" onclick="saveDt()">שמור</button>
    </div>
  </div>
</div>
</asp:Content>

<asp:Content ID="C2" ContentPlaceHolderID="ScriptContent" runat="server">
<script>
function openDtModal(id, name, color, duration, desc, startday) {
  document.getElementById('dt-modal-title').textContent = id ? 'עריכת סוג תורנות' : 'סוג חדש';
  document.getElementById('dt-id').value       = id || '';
  document.getElementById('dt-name').value     = name || '';
  document.getElementById('dt-color').value    = color || '#3b82f6';
  document.getElementById('dt-duration').value = duration || 7;
  document.getElementById('dt-desc').value     = desc || '';
  document.getElementById('dt-startday').value = startday || '';
  openModal('dtModal');
}
function saveDt() {
  var id = document.getElementById('dt-id').value;
  var data = {
    id:                 id ? parseInt(id,10) : 0,
    name:               document.getElementById('dt-name').value,
    color:              document.getElementById('dt-color').value,
    defaultDurationDays:parseInt(document.getElementById('dt-duration').value,10)||7,
    description:        document.getElementById('dt-desc').value,
    startDayOfWeek:     document.getElementById('dt-startday').value
  };
  if (!data.name) { alert('שם הוא שדה חובה'); return; }
  var action = id ? 'update' : 'create';
  ajax('Handlers/DutyTypeHandler.ashx?action='+action,'POST',data,function(ok){
    if(ok) location.reload(); else alert('שגיאה');
  });
}
function deleteDt(id) {
  if (!confirm('למחוק סוג תורנות זה?')) return;
  ajax('Handlers/DutyTypeHandler.ashx?action=delete','POST',{id:id},function(ok){
    if(ok) location.reload(); else alert('שגיאה');
  });
}
</script>
</asp:Content>
