using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BislaTichnunWeb
{
    public partial class DutyTypes : System.Web.UI.Page
    {
        protected List<DutyType> lstDutyTypes = new List<DutyType>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lstDutyTypes = DataAccess.GetDutyTypes(); //Replace later with stored proceedure
            }
        }
        protected string EscJs(string s)
        {
            return (s ?? "")
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", "")
                .Replace("\n", "");
        }
    }
}