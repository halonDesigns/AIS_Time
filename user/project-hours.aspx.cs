using System;
using System.Globalization;
using System.Web.UI.WebControls;
using AIS_Time.classes;
using Vici.CoolStorage;

namespace AIS_Time.user
{
    public partial class project_hours : System.Web.UI.Page
    {
        private TimeProjectHours _currentProjectHours;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RefreshEntries();
                LoadResources();
                LoadDepartments();
                LoadProjects();
                //set the default date
                txtDate.Text = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture); 
            }
        }

        private void RefreshEntries()
        {
            CSList<TimeProjectHours> projectList = TimeProjectHours.List("TimeEmployeeID = @TimeEmployeeID",
                "@TimeEmployeeID", (int)Session["TimeEmployeeID"]);

            if (projectList.Count > 0)
            {
                rptProjectHours.DataSource = projectList;
                rptProjectHours.DataBind();
            }
        }

        private void LoadResources()
        {
            CSList<TimeResources> employees = TimeResources.OrderedList("ResourceName");

            ddlResoures.DataSource = employees;
            ddlResoures.DataValueField = "TimeResourceID";
            ddlResoures.DataTextField = "ResourceName";
            ddlResoures.DataBind();
        }

        private void LoadProjects()
        {
            CSList<TimeProjects> projects = TimeProjects.OrderedList("ProjectName");

            ddlProject.DataSource = projects;
            ddlProject.DataValueField = "TimeProjectID";
            ddlProject.DataTextField = "ProjectName";
            ddlProject.DataBind();
        }

        private void LoadDepartments()
        {
            CSList<TimeDepartments> departments = TimeDepartments.OrderedList("DepartmentName");

            ddlDepartment.DataSource = departments;
            ddlDepartment.DataValueField = "TimeDepartmentID";
            ddlDepartment.DataTextField = "DepartmentName";
            ddlDepartment.DataBind();
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            if (txtDate.Text == "" || txtHours.Text == "" || txtHours.Text == "0" || ddlDepartment.SelectedIndex == -1
                || ddlProject.SelectedIndex == -1)
            {
                lblError.Text = "Please fill in all fields"; 
                return;
            }

            _currentProjectHours = (TimeProjectHours)Session["CurrentProjectHours"];
            if (_currentProjectHours == null)
            {
                //new projectHours object
                TimeProjectHours projectHours = TimeProjectHours.New();

                //fill object with data
                projectHours.DateOfWork = DateTime.ParseExact(txtDate.Text, "M/d/yyyy", CultureInfo.InvariantCulture);
                projectHours.HoursOfWork = Convert.ToInt32(txtHours.Text);
                projectHours.TimeDepartmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
                projectHours.TimeResourceID = Convert.ToInt32(ddlResoures.SelectedValue);
                projectHours.TimeEmployeeID = (int)Session["TimeEmployeeID"];
                projectHours.TimeProjectID = Convert.ToInt32(ddlProject.SelectedValue);
                projectHours.Description = txtDescription.Text;
                projectHours.Status = 1;
                projectHours.Type = 1;

                //save the new projectHours
                projectHours.Save();
            }
            else
            {
                _currentProjectHours.DateOfWork = DateTime.ParseExact(txtDate.Text, "M/d/yyyy", CultureInfo.InvariantCulture);
                _currentProjectHours.HoursOfWork = Convert.ToInt32(txtHours.Text);
                _currentProjectHours.TimeDepartmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
                _currentProjectHours.TimeResourceID = Convert.ToInt32(ddlResoures.SelectedValue);
                _currentProjectHours.TimeEmployeeID = (int)Session["TimeEmployeeID"];
                _currentProjectHours.TimeProjectID = Convert.ToInt32(ddlProject.SelectedValue);
                _currentProjectHours.Description = txtDescription.Text;
                _currentProjectHours.Status = 1;
                _currentProjectHours.Type = 1;

                //save the new projectHours
                _currentProjectHours.Save();
            }

            txtDate.Text = "";
            txtHours.Text = "0";
            ddlDepartment.SelectedIndex = -1;
            ddlResoures.SelectedIndex = -1;
            ddlProject.SelectedIndex = -1;
            txtDescription.Text = "";
            lblError.Text = "";
            _currentProjectHours = null;
            Session["CurrentProjectHours"] = _currentProjectHours;
            RefreshEntries();
            updEntries.Update();
        }

        protected void rptCustomers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentProjectHours = TimeProjectHours.Read(pkID);
                Session["CurrentProjectHours"] = _currentProjectHours;
                txtDate.Text = _currentProjectHours.DateOfWork.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
                txtHours.Text = _currentProjectHours.HoursOfWork.ToString();
                ddlDepartment.SelectedValue = ddlDepartment.Items.FindByValue(_currentProjectHours.TimeDepartmentID.ToString()).Value;
                ddlResoures.SelectedValue = ddlResoures.Items.FindByValue(_currentProjectHours.TimeResourceID.ToString()).Value;
                ddlProject.SelectedValue = ddlProject.Items.FindByValue(_currentProjectHours.TimeProjectID.ToString()).Value;
                txtDescription.Text = _currentProjectHours.Description;
            }

            if (e.CommandName == "Delete")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentProjectHours = TimeProjectHours.Read(pkID);
                _currentProjectHours.Delete();
                _currentProjectHours = null;
                Session["CurrentProjectHours"] = _currentProjectHours;
                RefreshEntries();
                updEntries.Update();
            }
        }

        protected void cmdNew_Click(object sender, EventArgs e)
        {
            txtDate.Text = "";
            txtHours.Text = "0";
            ddlDepartment.SelectedIndex = -1;
            ddlResoures.SelectedIndex = -1;
            ddlProject.SelectedIndex = -1;
            txtDescription.Text = "";
            lblError.Text = "";
            _currentProjectHours = null;
            Session["CurrentProjectHours"] = _currentProjectHours;
            RefreshEntries();
            updEntries.Update();
        }
    }
}