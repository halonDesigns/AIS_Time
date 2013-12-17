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
                LoadResources();
                LoadRoles();
            }
        }

        private void RefreshEntries()
        {

            CSList<TimeEmployees> employeeList = TimeEmployees.List();

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

        private void LoadResources()
        {
            CSList<TimeResources> resources = TimeResources.OrderedList("ResourceName");

            ddlResouces.DataSource = resources;
            ddlResouces.DataValueField = "TimeResourceID";
            ddlResouces.DataTextField = "ResourceName";
            ddlResouces.DataBind();
        }

         private void LoadRoles()
         {
            string[] roles = Roles.GetAllRoles();

            ddlRoles.DataSource = roles;
            ddlRoles.DataBind();
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            if (txtFname.Text == "" || txtLname.Text == "" || ddlCompany.SelectedIndex == -1) { return; }

            _currentEmployee = (TimeEmployees)Session["CurrentEmployee"];
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
                        employee.TimeResourceID = Convert.ToInt32(ddlResouces.SelectedValue);
                        employee.Description = txtDescription.Text;
                        employee.Status = 1;
                        employee.Type = 1;
                        employee.UserID = guid;
                        employee.Phone = txtPhone.Text;
                        employee.Email = txtEmail.Text;
                        //save the new employee
                        employee.Save();
                    }
                }
            }
            else
            {
                //we are editing an existing employee
                _currentEmployee.FirstName = txtFname.Text;
                _currentEmployee.LastName = txtLname.Text;
                _currentEmployee.CompanyID = Convert.ToInt32(ddlCompany.SelectedValue);
                _currentEmployee.TimeResourceID = Convert.ToInt32(ddlResouces.SelectedValue);
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
            }

            txtFname.Text = "";
            txtLname.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            Password.Text = "";
            UserName.Text = "";
            ddlCompany.SelectedIndex = -1;
            ddlResouces.SelectedIndex = -1;
            txtDescription.Text = "";
            _currentEmployee = null;
            Session["CurrentEmployee"] = _currentEmployee;
            RefreshEntries();
            updEntries.Update();
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
                ddlResouces.SelectedValue = ddlResouces.Items.FindByValue(_currentEmployee.TimeResourceID.ToString()).Value;
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
                _currentEmployee.Delete();
                _currentEmployee = null;
                Session["CurrentEmployee"] = _currentEmployee;
                RefreshEntries();
                updEntries.Update();
            }
        }
    }
}