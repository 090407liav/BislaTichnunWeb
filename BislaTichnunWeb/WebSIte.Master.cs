using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BislaTichnunWeb
{
    public partial class WebSIte : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected string IsActive(string page)
        {
            string current = Path.GetFileName(Request.FilePath) ?? "";
            return current.Equals(page, StringComparison.OrdinalIgnoreCase) ? "active" : "";
        }
    }
}