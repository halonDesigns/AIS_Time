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
    public partial class configurations : System.Web.UI.Page
    {
        private TimeAISCodes _currentAISCode;
        private TimeCEAClassCodes _currentCEAClassCode;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RefreshCEAClassCodeEntries();
                RefreshAISCodeEntries();
            }
        }

        private void RefreshCEAClassCodeEntries()
        {

            CSList<TimeCEAClassCodes> ceaClassCodeList = TimeCEAClassCodes.List().OrderedBy("CEAClassCode");

            if (ceaClassCodeList.Count > 0)
            {
                rptCEAClassCodes.DataSource = ceaClassCodeList;
                rptCEAClassCodes.DataBind();
            }
        }
        private void RefreshAISCodeEntries()
        {

            CSList<TimeAISCodes> aisCodeList = TimeAISCodes.List().OrderedBy("AISCode");

            if (aisCodeList.Count > 0)
            {
                rptAISCodes.DataSource = aisCodeList;
                rptAISCodes.DataBind();
            }
        }

        protected void cmdSubmitCEAClassCode_Click(object sender, EventArgs e)
        {
            if (txtCEAClassCodeName.Text == "")
            {
                lblError.Text = "Please enter a CEAClassCode name.";
                return;
            }
            lblError.Text = "";

            _currentCEAClassCode = (TimeCEAClassCodes)Session["CurrentCEAClassCode"];
            if (_currentCEAClassCode == null)
            {
                //new department object
                TimeCEAClassCodes ceaClassCode = TimeCEAClassCodes.New();

                //fill object with data
                ceaClassCode.CEAClassCode = txtCEAClassCodeName.Text;
                ceaClassCode.Description = txtCEAClassCodeDescription.Text;
                ceaClassCode.Status = 1;
                ceaClassCode.Type = 1;

                //save the new department
                ceaClassCode.Save();
            }
            else
            {
                _currentCEAClassCode.CEAClassCode = txtCEAClassCodeName.Text;
                _currentCEAClassCode.Description = txtCEAClassCodeDescription.Text;
                _currentCEAClassCode.Status = 1;
                _currentCEAClassCode.Type = 1;

                //save the new department
                _currentCEAClassCode.Save();
            }

            txtCEAClassCodeName.Text = "";
            txtCEAClassCodeDescription.Text = "";
            _currentCEAClassCode = null;
            Session["CurrentCEAClassCode"] = _currentCEAClassCode;
            RefreshCEAClassCodeEntries();
            updEntries.Update();
        }

        protected void cmdSubmitAISCode_Click(object sender, EventArgs e)
        {
           if (txtAISCodeName.Text == "")
            {
                lblErrorAIS.Text = "Please enter a AISCode name.";
                return;
            }
           lblErrorAIS.Text = "";

            _currentAISCode = (TimeAISCodes)Session["CurrentAISCode"];
            if (_currentAISCode == null)
            {
                //new department object
                TimeAISCodes aisCode = TimeAISCodes.New();

                //fill object with data
                aisCode.AISCode = txtAISCodeName.Text;
                aisCode.Description = txtAISCodeDescription.Text;
                aisCode.Status = 1;
                aisCode.Type = 1;

                //save the new department
                aisCode.Save();
            }
            else
            {
                _currentAISCode.AISCode = txtAISCodeName.Text;
                _currentAISCode.Description = txtAISCodeDescription.Text;
                _currentAISCode.Status = 1;
                _currentAISCode.Type = 1;

                //save the new department
                _currentAISCode.Save();
            }

            txtAISCodeName.Text = "";
            txtAISCodeDescription.Text = "";
            _currentAISCode = null;
            Session["CurrentAISCode"] = _currentAISCode;
            RefreshAISCodeEntries();
            updEntries.Update();
        }

        protected void rptAISCodes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentAISCode = TimeAISCodes.Read(pkID);
                Session["CurrentAISCode"] = _currentAISCode;
                txtAISCodeName.Text = _currentAISCode.AISCode;
                txtAISCodeDescription.Text = _currentAISCode.Description;
            }

            if (e.CommandName == "Delete")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentAISCode = TimeAISCodes.Read(pkID);
                _currentAISCode.Delete();
                _currentAISCode = null;
                Session["CurrentAISCode"] = _currentAISCode;
                RefreshAISCodeEntries();
                updEntries.Update();
            }
        }
        protected void rptCEAClassCodes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentCEAClassCode = TimeCEAClassCodes.Read(pkID);
                Session["CurrentCEAClassCode"] = _currentCEAClassCode;
                txtCEAClassCodeName.Text = _currentCEAClassCode.CEAClassCode;
                txtCEAClassCodeDescription.Text = _currentCEAClassCode.Description;
            }

            if (e.CommandName == "Delete")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentCEAClassCode = TimeCEAClassCodes.Read(pkID);
                _currentCEAClassCode.Delete();
                _currentCEAClassCode = null;
                Session["CurrentCEAClassCode"] = _currentCEAClassCode;
                RefreshCEAClassCodeEntries();   
                updEntries.Update();
            }
        }
    }
}