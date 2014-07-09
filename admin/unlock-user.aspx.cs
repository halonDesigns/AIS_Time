using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AIS_Time.admin
{
    public partial class unlock_user : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //This event handler is fired whenever the user clicks the IsApproved CheckBox
        protected void ToggleApproved(object sender, EventArgs e)
        {
            //Reference the checkbox
            CheckBox isApproved = (CheckBox)sender;

            //Reference the GridViewRow used by the CheckBox
            GridViewRow gvr = (GridViewRow)isApproved.Parent.Parent;

            //Get the user information
            string userName = GridView1.DataKeys[gvr.RowIndex].Value.ToString();
            MembershipUser userInfo = Membership.GetUser(userName);

            //Set the value...
            userInfo.IsApproved = isApproved.Checked;

            //Update the record
            Membership.UpdateUser(userInfo);
        }

        //This event handler is fired whenever the user clicks the IsAdministrator CheckBox
        protected void ToggleAdministrator(object sender, EventArgs e)
        {
            //Reference the checkbox
            CheckBox isAdministrator = (CheckBox)sender;

            //Reference the GridViewRow used by the CheckBox
            GridViewRow gvr = (GridViewRow)isAdministrator.Parent.Parent;

            //Get the user information
            string userName = GridView1.DataKeys[gvr.RowIndex].Value.ToString();

            //Set the value...
            const string ADMIN_ROLE_NAME = "Administrator";
            if (isAdministrator.Checked)
            {
                //Add the user to the role
                Roles.AddUserToRole(userName, ADMIN_ROLE_NAME);
            }
            else
            {
                //Remove the user from the role
                Roles.RemoveUserFromRole(userName, ADMIN_ROLE_NAME);
            }
        }

        //This event handler is fired whenever the user clicks the IsAssistantCoach CheckBox
        protected void ToggleConsultant(object sender, EventArgs e)
        {
            //Reference the checkbox
            CheckBox IsConsultant = (CheckBox)sender;

            //Reference the GridViewRow used by the CheckBox
            GridViewRow gvr = (GridViewRow)IsConsultant.Parent.Parent;

            //Get the user information
            string userName = GridView1.DataKeys[gvr.RowIndex].Value.ToString();

            //Set the value...
            const string CONSULTANT_ROLE_NAME = "Consultant";
            if (IsConsultant.Checked)
            {
                //Add the user to the role
                Roles.AddUserToRole(userName, CONSULTANT_ROLE_NAME);
            }
            else
            {
                //Remove the user from the role
                Roles.RemoveUserFromRole(userName, CONSULTANT_ROLE_NAME);
            }
        }

        protected void GridView1_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            //We need to unlock the specified user
            if (e.CommandName == "UnlockUser")
            {
                //Reference the checkbox
                LinkButton unlockLinkButton = (LinkButton)e.CommandSource;

                //Reference the GridViewRow used by the CheckBox
                GridViewRow gvr = (GridViewRow)unlockLinkButton.Parent.Parent;

                //Get the user information
                string userName = GridView1.DataKeys[gvr.RowIndex].Value.ToString();
                MembershipUser userInfo = Membership.GetUser(userName);

                //Unlock the user
                userInfo.UnlockUser();

                //Rebind the data to the GridView
                GridView1.DataBind();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;
                MembershipUser mu = Membership.GetUser(username);

                //lets unlock if needed
                mu.UnlockUser();
                Membership.UpdateUser(mu);

                //now reset the password
                mu.ChangePassword(mu.ResetPassword(), password);

                lblPasswordChange.Text = "Password changed and user unlocked if it was neccesary.";
            }
            catch (Exception ex)
            {
                lblPasswordChange.Text = "Error changing password for user : " + ex.Message;
            }

        }
    }
}