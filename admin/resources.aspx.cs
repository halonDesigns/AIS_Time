﻿using System;
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
            }
        }

        private void RefreshEntries()
        {

            CSList<TimeResources> resourceList = TimeResources.List();

            if (resourceList.Count > 0)
            {
                rptResources.DataSource = resourceList;
                rptResources.DataBind();
            }
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtHourlyRate.Text == "" || txtHourlyRate.Text == "0.0") { return; }
            _currentResource = (TimeResources)Session["CurrentResource"];
            if (_currentResource == null)
            {
                //new resource object
                TimeResources resource = TimeResources.New();

                //fill object with data
                resource.ResourceName = txtName.Text;
                resource.Description = txtDescription.Text;
                resource.AISCode = ddlAISCode.SelectedValue;
                resource.CEAClassCode = ddlCEAClassCode.SelectedValue;
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
                _currentResource.AISCode = ddlAISCode.SelectedValue;
                _currentResource.CEAClassCode = ddlCEAClassCode.SelectedValue;
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
            ddlCEAClassCode.SelectedIndex = -1;
            txtHourlyRate.Text = "0.0";
            _currentResource = null;
            Session["CurrentResource"] = _currentResource;
            RefreshEntries();
            updEntries.Update();
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
                ddlAISCode.SelectedValue = ddlAISCode.Items.FindByText(_currentResource.AISCode).Value;
                ddlCEAClassCode.SelectedValue = ddlCEAClassCode.Items.FindByText(_currentResource.CEAClassCode).Value;
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
            }
        }
    }
}