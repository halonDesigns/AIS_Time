using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AIS_Time.classes;
using Vici.CoolStorage;

namespace AIS_Time.admin
{
    public partial class resources : Page
    {
        private TimeResources _currentResource;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RefreshEntries();
                LoadAISCodes();
                //LoadCEAClassCodes();
            }
        }

        private void RefreshEntries()
        {

            CSList<TimeResources> resourceList = TimeResources.List().OrderedBy("ResourceName");

            if (resourceList.Count > 0)
            {
                rptResources.DataSource = resourceList;
                rptResources.DataBind();
            }
        }

        private void LoadAISCodes()
        {
            CSList<TimeAISCodes> companies = TimeAISCodes.OrderedList("AISCode");

            ddlAISCode.DataSource = companies;
            ddlAISCode.DataValueField = "TimeAISCodeID";
            ddlAISCode.DataTextField = "AISCode";
            ddlAISCode.DataBind();
        }

        //private void LoadCEAClassCodes()
        //{
        //    CSList<TimeCEAClassCodes> companies = TimeCEAClassCodes.OrderedList("CEAClassCode");

        //    ddlCEAClassCode.DataSource = companies;
        //    ddlCEAClassCode.DataValueField = "TimeCEAClassCodeID";
        //    ddlCEAClassCode.DataTextField = "CEAClassCode";
        //    ddlCEAClassCode.DataBind();
        //}

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
           
            if (txtName.Text == "")
            {
                lblError.Text = "Please enter a Resource Name.";
                return;
            }
            if (txtHourlyRate.Text == "" || txtHourlyRate.Text == "0.0")
            {
                lblError.Text = "Please enter an hourly rate.";
                return;
            }
            lblError.Text = "";

            _currentResource = (TimeResources)Session["CurrentResource"];
            if (_currentResource == null)
            {
                //new resource object
                TimeResources resource = TimeResources.New();

                //fill object with data
                resource.ResourceName = txtName.Text;
                resource.Description = txtDescription.Text;
                resource.TimeAISCodeID = Convert.ToInt32(ddlAISCode.SelectedValue);
                //resource.TimeCEAClassCodeID = Convert.ToInt32(ddlCEAClassCode.SelectedValue);
                decimal res;
                resource.HourlyRate = decimal.TryParse(txtHourlyRate.Text, out res) ? res : new decimal(0.0);
                resource.Status = 1;
                resource.Type = 1;

                //save the new resource
                resource.Save();
            }
            else
            {
                _currentResource.ResourceName = txtName.Text;
                _currentResource.Description = txtDescription.Text;
                _currentResource.TimeAISCodeID = Convert.ToInt32(ddlAISCode.SelectedValue);
                //_currentResource.TimeCEAClassCodeID = Convert.ToInt32(ddlCEAClassCode.SelectedValue);
                decimal res;
                _currentResource.HourlyRate = decimal.TryParse(txtHourlyRate.Text, out res) ? res : new decimal(0.0);
                _currentResource.Status = 1;
                _currentResource.Type = 1;

                //save the new resource
                _currentResource.Save();
            }

            txtName.Text = "";
            txtDescription.Text = "";
            ddlAISCode.SelectedIndex = -1;
            //ddlCEAClassCode.SelectedIndex = -1;
            txtHourlyRate.Text = "0.0";
            _currentResource = null;
            Session["CurrentResource"] = _currentResource;
            RefreshEntries();
            updEntries.Update();

            lblSuccessMessage.Text = "Successfully submitted data!";
            mpSuccess.Show();
        }

        protected void rptResource_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentResource = TimeResources.Read(pkID);
                Session["CurrentResource"] = _currentResource;
                txtName.Text = _currentResource.ResourceName;
                txtDescription.Text = _currentResource.Description;
                ddlAISCode.SelectedValue = ddlAISCode.Items.FindByValue(_currentResource.TimeAISCodeID.ToString()).Value;
                //ddlCEAClassCode.SelectedValue = ddlCEAClassCode.Items.FindByValue(_currentResource.TimeCEAClassCodeID.ToString()).Value;
                txtHourlyRate.Text = _currentResource.HourlyRate.ToString();
            }

            if (e.CommandName == "Delete")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentResource = TimeResources.Read(pkID);
                _currentResource.Delete();
                _currentResource = null;
                Session["CurrentResource"] = _currentResource;
                RefreshEntries();
                updEntries.Update();
                lblSuccessMessage.Text = "Successfully deleted data!";
                mpSuccess.Show();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs args)
        {
            //all good
            mpSuccess.Hide();
        }
    }
}