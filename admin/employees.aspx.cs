using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
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

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            if (txtFname.Text == "" || txtLname.Text == "" || ddlCompany.SelectedIndex == -1) { return; }

            _currentEmployee = (TimeEmployees)Session["CurrentEmployee"];
            if (_currentEmployee == null)
            {
                //new employee object
                TimeEmployees employee = TimeEmployees.New();

                //fill object with data
                employee.FirstName = txtFname.Text;
                employee.LastName = txtLname.Text;
                employee.CompanyID = Convert.ToInt32(ddlCompany.SelectedValue);
                employee.Description = txtDescription.Text;
                employee.Status = 1;
                employee.Type = 1;
             
                //save the new employee
                employee.Save();
            }
            else
            {
                _currentEmployee.FirstName = txtFname.Text;
                _currentEmployee.LastName = txtLname.Text;
                _currentEmployee.CompanyID = Convert.ToInt32(ddlCompany.SelectedValue);
                _currentEmployee.Description = txtDescription.Text;
                _currentEmployee.Status = 1;
                _currentEmployee.Type = 1;

                //save the new employee
                _currentEmployee.Save();
            }

            txtFname.Text = "";
            txtLname.Text = "";
            ddlCompany.SelectedIndex = -1;
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
                ddlCompany.SelectedValue = ddlCompany.Items.FindByValue(_currentEmployee.CompanyID.ToString()).Value;
                txtDescription.Text = _currentEmployee.Description;
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