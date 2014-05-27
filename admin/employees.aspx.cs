using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using AIS_Time.classes;
using Vici.CoolStorage;


namespace AIS_Time.admin
{
    public partial class employees : System.Web.UI.Page
    {
        private TimeEmployees _currentEmployee;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RefreshEntries();
                LoadCompanies();
                LoadRoles();
            }
        }

        private void RefreshEntries()
        {

            CSList<TimeEmployees> employeeList = TimeEmployees.List().OrderedBy("LastName");

            if (employeeList.Count > 0)
            {
                rptEmployee.DataSource = employeeList;
                rptEmployee.DataBind();
            }
        }

        private void LoadCompanies()
        {
            CSList<TimeCompanies> companies = TimeCompanies.OrderedList("CompanyName");

            ddlCompany.DataSource = companies;
            ddlCompany.DataValueField = "TimeCompanyID";
            ddlCompany.DataTextField = "CompanyName";
            ddlCompany.DataBind();
        }

         private void LoadRoles()
         {
            string[] roles = Roles.GetAllRoles();

            ddlRoles.DataSource = roles;
            ddlRoles.DataBind();
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            _currentEmployee = (TimeEmployees)Session["CurrentEmployee"];

            if (txtFname.Text == "")
            {
                lblError.Text = "Please enter a first name.";
                return;
            }
            if (txtLname.Text == "")
            {
                lblError.Text = "Please enter a last name.";
                return;
            }
            if (ddlCompany.SelectedIndex == -1)
            {
                lblError.Text = "Please choose a company.";
                return;
            }
            if (txtEmail.Text == "")
            {
                lblError.Text = "Please enter an email address.";
                return;
            }
            if (UserName.Text == "" && _currentEmployee == null)
            {
                lblError.Text = "Please enter a username.";
                return;
            }
            if (Password.Text == "" && _currentEmployee == null)
            {
                lblError.Text = "Please enter a password.";
                return;
            }

            if (ddlRoles.SelectedIndex == -1 && _currentEmployee == null)
            {
                lblError.Text = "Please choose a role for this user.";
                return;
            }
            lblError.Text = "";

            
            if (_currentEmployee == null)
            {

                //create a user login
                MembershipCreateStatus status;
                Membership.CreateUser(UserName.Text, Password.Text, txtEmail.Text, null, null, true, out status);

                if (status == MembershipCreateStatus.Success)
                {
                    //get the GUID of the newly created user
                    var user = Membership.GetUser(UserName.Text);
                    if (user != null)
                    {
                        var guid = (Guid) user.ProviderUserKey;

                        //add user to the coach role
                        Roles.AddUserToRole(UserName.Text, ddlRoles.SelectedItem.ToString());

                        //new employee object
                        TimeEmployees employee = TimeEmployees.New();

                        //fill object with data
                        employee.FirstName = txtFname.Text;
                        employee.LastName = txtLname.Text;
                        employee.CompanyID = Convert.ToInt32(ddlCompany.SelectedValue);
                        employee.Description = txtDescription.Text;
                        employee.Status = 1;
                        employee.Type = 1;
                        employee.UserID = guid;
                        employee.Phone = txtPhone.Text;
                        employee.Email = txtEmail.Text;
                        //save the new employee
                        employee.Save();

                        txtFname.Text = "";
                        txtLname.Text = "";
                        txtEmail.Text = "";
                        txtPhone.Text = "";
                        Password.Text = "";
                        UserName.Text = "";
                        ddlCompany.SelectedIndex = -1;
                        txtDescription.Text = "";
                        _currentEmployee = null;
                        Session["CurrentEmployee"] = _currentEmployee;
                        RefreshEntries();
                        updEntries.Update();

                        lblSuccessMessage.Text = "Successfully submitted data!";
                        mpSuccess.Show();
                    }
                }
                else
                {
                    lblSuccessMessage.Text = GetErrorMessage(status);
                    mpSuccess.Show();
                }
            }
            else
            {
                //we are editing an existing employee
                _currentEmployee.FirstName = txtFname.Text;
                _currentEmployee.LastName = txtLname.Text;
                _currentEmployee.CompanyID = Convert.ToInt32(ddlCompany.SelectedValue);
                _currentEmployee.Description = txtDescription.Text;
                _currentEmployee.Email = txtEmail.Text;
                _currentEmployee.Phone = txtPhone.Text;
                _currentEmployee.Status = 1;
                _currentEmployee.Type = 1;

                //save the new employee
                _currentEmployee.Save();

                //re-enable the hidden controls
                UserName.Visible = true;
                Password.Visible = true;
                ddlRoles.Visible = true;

                txtFname.Text = "";
                txtLname.Text = "";
                txtEmail.Text = "";
                txtPhone.Text = "";
                Password.Text = "";
                UserName.Text = "";
                ddlCompany.SelectedIndex = -1;
                txtDescription.Text = "";
                _currentEmployee = null;
                Session["CurrentEmployee"] = _currentEmployee;
                RefreshEntries();
                updEntries.Update();

                lblSuccessMessage.Text = "Successfully submitted data!";
                mpSuccess.Show();
            }
        }

        public string GetErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value of at least 6 characters.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        protected void rptCustomers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentEmployee = TimeEmployees.Read(pkID);
                Session["CurrentEmployee"] = _currentEmployee;
                txtFname.Text = _currentEmployee.FirstName;
                txtLname.Text = _currentEmployee.LastName;
                txtEmail.Text = _currentEmployee.Email;
                txtPhone.Text = _currentEmployee.Phone;
                ddlCompany.SelectedValue = ddlCompany.Items.FindByValue(_currentEmployee.CompanyID.ToString()).Value;
                txtDescription.Text = _currentEmployee.Description;

                UserName.Visible = false;
                Password.Visible = false;
                ddlRoles.Visible = false;
            }

            if (e.CommandName == "Delete")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentEmployee = TimeEmployees.Read(pkID);

                var user = Membership.GetUser(_currentEmployee.UserID);
                if (user == null)
                {
                    lblError.Text = "User could not be deleted";
                }
                else
                {
                    //delete the user from the membership tables
                    Membership.DeleteUser(user.UserName);

                    _currentEmployee.Delete();
                    _currentEmployee = null;
                    Session["CurrentEmployee"] = _currentEmployee;
                    RefreshEntries();
                    updEntries.Update();
                    lblSuccessMessage.Text = "Successfully deleted data!";
                    mpSuccess.Show();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs args)
        {
            //all good
            mpSuccess.Hide();
        }
    }
}