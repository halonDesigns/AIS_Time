using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AIS_Time.classes;

namespace AIS_Time
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string uName = HttpContext.Current.User.Identity.Name;

                if (Roles.IsUserInRole(uName, "Administrator"))
                {
                    //store name
                    Session["LoggedInUserName"] = uName;

                    //get the GUID of the newly created user
                    MembershipUser user = Membership.GetUser(uName);
                    var guid = (Guid)user.ProviderUserKey;
                    Session["UserID"] = guid;

                    TimeEmployees employee = TimeEmployees.ReadFirst("UserID = @UserID", "@UserID", guid);
                    Session["TimeEmployeeID"] = employee.TimeEmployeeID;

                    Response.Redirect("~/admin/index.aspx");
                }
                else if (Roles.IsUserInRole(uName, "Consultant"))
                {
                    //store name
                    Session["LoggedInUserName"] = uName;

                    //get the GUID of the newly created user
                    MembershipUser user = Membership.GetUser(uName);
                    var guid = (Guid)user.ProviderUserKey;
                    Session["UserID"] = guid;

                    TimeEmployees employee = TimeEmployees.ReadFirst("UserID = @UserID", "@UserID", guid);
                    Session["TimeEmployeeID"] = employee.TimeEmployeeID;

                    Response.Redirect("~/user/index.aspx");
                }

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