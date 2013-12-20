using System;
using System.Web.UI.WebControls;
using AIS_Time.classes;
using Vici.CoolStorage;

namespace AIS_Time.admin
{
    public partial class projects : System.Web.UI.Page
    {
        private TimeProjects _currentProject;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RefreshEntries();
                LoadCustomers();
            }
        }

        private void RefreshEntries()
        {
            CSList<TimeProjects> projectList = TimeProjects.List();

            if (projectList.Count > 0)
            {
                rptProjects.DataSource = projectList;
                rptProjects.DataBind();
            }
        }

        private void LoadCustomers()
        {
            CSList<TimeCustomers> companies = TimeCustomers.OrderedList("CustomerName");

            ddlCustomer.DataSource = companies;
            ddlCustomer.DataValueField = "TimeCustomerID";
            ddlCustomer.DataTextField = "CustomerName";
            ddlCustomer.DataBind();
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                lblError.Text = "Please enter a Project Name.";
                return;
            }
            if (txtNumber.Text == "")
            {
                lblError.Text = "Please enter a Project Number.";
                return;
            }
            if (ddlCustomer.SelectedIndex == -1)
            {
                lblError.Text = "Please choose a customer.";
                return;
            }
            lblError.Text = "";
            
            _currentProject = (TimeProjects)Session["CurrentProject"];
            if (_currentProject == null)
            {
                //new project object
                TimeProjects project = TimeProjects.New();

                //fill object with data
                project.ProjectName = txtName.Text;
                project.ProjectNumber = txtNumber.Text;
                project.TimeCustomerID = Convert.ToInt32(ddlCustomer.SelectedValue);
                project.Description = txtDescription.Text;
                project.Status = 1;
                project.Type = 1;

                //save the new project
                project.Save();
            }
            else
            {
                _currentProject.ProjectName = txtName.Text;
                _currentProject.ProjectNumber = txtNumber.Text;
                _currentProject.TimeCustomerID = Convert.ToInt32(ddlCustomer.SelectedValue);
                _currentProject.Description = txtDescription.Text;
                _currentProject.Status = 1;
                _currentProject.Type = 1;

                //save the new project
                _currentProject.Save();
            }

            txtName.Text = "";
            txtNumber.Text = "";
            ddlCustomer.SelectedIndex = -1;
            txtDescription.Text = "";
            _currentProject = null;
            Session["CurrentProject"] = _currentProject;
            RefreshEntries();
            updEntries.Update();
        }

        protected void rptCustomers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentProject = TimeProjects.Read(pkID);
                Session["CurrentProject"] = _currentProject;
                txtName.Text = _currentProject.ProjectName;
                txtNumber.Text = _currentProject.ProjectNumber;
                ddlCustomer.SelectedValue = ddlCustomer.Items.FindByValue(_currentProject.TimeCustomerID.ToString()).Value;
                txtDescription.Text = _currentProject.Description;
            }

            if (e.CommandName == "Delete")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentProject = TimeProjects.Read(pkID);
                _currentProject.Delete();
                _currentProject = null;
                Session["CurrentProject"] = _currentProject;
                RefreshEntries();
                updEntries.Update();
            }
        }
    }
}