using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AIS_Time.classes;

namespace AIS_Time.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //RegisterHyperLink.NavigateUrl = "Register";
            //OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];

            //var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            //if (!String.IsNullOrEmpty(returnUrl))
            //{
            //    RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            //}
            TextBox tb = (TextBox)Login1.FindControl("UserName");
            tb.Focus();
            
        }

        protected void LoginUser_LoggedIn(object sender, EventArgs e)
        {
            if (Roles.IsUserInRole(Login1.UserName, "Administrator"))
            {
                //store name
                Session["LoggedInUserName"] = Login1.UserName;
               
                 //get the GUID of the newly created user
                MembershipUser user = Membership.GetUser(Login1.UserName);
                var guid = (Guid)user.ProviderUserKey;
                Session["UserID"] = guid;

                TimeEmployees employee = TimeEmployees.ReadFirst("UserID = @UserID", "@UserID", guid);
                Session["TimeEmployeeID"] = employee.TimeEmployeeID;

                Response.Redirect("~/admin/index.aspx");
            }
            else if (Roles.IsUserInRole(Login1.UserName, "Consultant"))
            {
                //store name
                Session["LoggedInUserName"] = Login1.UserName;

                //get the GUID of the newly created user
                MembershipUser user = Membership.GetUser(Login1.UserName);
                var guid = (Guid)user.ProviderUserKey;
                Session["UserID"] = guid;

                TimeEmployees employee = TimeEmployees.ReadFirst("UserID = @UserID", "@UserID", guid);
                Session["TimeEmployeeID"] = employee.TimeEmployeeID;

                Response.Redirect("~/user/index.aspx");
            }
        }

        protected void LoginUser_LoginError(object sender, EventArgs e)
        {
            LOGS_InvalidCredentialsDataSource.InsertParameters["UserName"].DefaultValue = Login1.UserName;
            LOGS_InvalidCredentialsDataSource.InsertParameters["IPAddress"].DefaultValue = Request.UserHostAddress;

            //The password is only supplied if the user enters an invalid username or invalid password - set it to Nothing, by default
            LOGS_InvalidCredentialsDataSource.InsertParameters["Password"].DefaultValue = null;

            //There was a problem logging in the user
            //See if this user exists in the database
            MembershipUser userInfo = Membership.GetUser(Login1.UserName);

            if (userInfo == null)
            {
                //The user entered an invalid username...
                LoginErrorDetails.Text = "Invalid username/password combination";

                //The password is only supplied if the user enters an invalid username or invalid password
                LOGS_InvalidCredentialsDataSource.InsertParameters["Password"].DefaultValue = Login1.Password;
            }
            else
            {
                //See if the user is locked out or not approved
                if (!userInfo.IsApproved)
                {
                    LoginErrorDetails.Text = "Your account has not yet been approved by the site's administrators. Please try again later...";
                }
                else if (userInfo.IsLockedOut)
                {
                    LoginErrorDetails.Text = "Your account has been locked out because of a maximum number of incorrect login attempts. You will NOT be able to login until you contact a site administrator and have your account unlocked.";
                }
                else
                {
                    //The password was incorrect (don't show anything, the Login control already describes the problem)
                    LoginErrorDetails.Text = string.Empty;

                    //The password is only supplied if the user enters an invalid username or invalid password
                    LOGS_InvalidCredentialsDataSource.InsertParameters["Password"].DefaultValue = Login1.Password;
                }
            }

            //Add a new record to the LOGS_InvalidCredentials table
            LOGS_InvalidCredentialsDataSource.Insert();
        }

        protected void LoginUser_LoggingIn(object sender, System.Web.UI.WebControls.LoginCancelEventArgs e)
        {
           
        }
    }
}