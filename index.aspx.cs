using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AIS_Time
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.User.IsInRole("Administrator"))
                {
                    Response.Redirect("~/admin/index.aspx");
                }
                else if (HttpContext.Current.User.IsInRole("Consultant"))
                {
                    Response.Redirect("~/user/index.aspx");
                } 
            }
        }
    }
}