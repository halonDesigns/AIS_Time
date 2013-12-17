using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AIS_Time.classes;
using Vici.CoolStorage;

namespace AIS_Time.admin
{
    public partial class departments : Page
    {

        private TimeDepartments _currentDepartment;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RefreshEntries();
            }
        }

        private void RefreshEntries()
        {

            CSList<TimeDepartments> departmentList = TimeDepartments.List();

            if (departmentList.Count > 0)
            {
                rptDepartments.DataSource = departmentList;
                rptDepartments.DataBind();
            }
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "") { return; }
            _currentDepartment = (TimeDepartments)Session["CurrentDepartment"];
            if (_currentDepartment == null)
            {
                //new department object
                TimeDepartments department = TimeDepartments.New();

                //fill object with data
                department.DepartmentName = txtName.Text;
                department.Description = txtDescription.Text;
                department.Status = 1;
                department.Type = 1;

                //save the new department
                department.Save();
            }
            else
            {
                _currentDepartment.DepartmentName = txtName.Text;
                _currentDepartment.Description = txtDescription.Text;
                _currentDepartment.Status = 1;
                _currentDepartment.Type = 1;

                //save the new department
                _currentDepartment.Save();
            }

            txtName.Text = "";
            txtDescription.Text = "";
            _currentDepartment = null;
            Session["CurrentDepartment"] = _currentDepartment;
            RefreshEntries();
            updEntries.Update();
        }

        protected void rptCustomers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentDepartment = TimeDepartments.Read(pkID);
                Session["CurrentDepartment"] = _currentDepartment;
                txtName.Text = _currentDepartment.DepartmentName;
                txtDescription.Text = _currentDepartment.Description;
            }

            if (e.CommandName == "Delete")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentDepartment = TimeDepartments.Read(pkID);
                _currentDepartment.Delete();
                _currentDepartment = null;
                Session["CurrentDepartment"] = _currentDepartment;
                RefreshEntries();
                updEntries.Update();
            }
        }
    }
}