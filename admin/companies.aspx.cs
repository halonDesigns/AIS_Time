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
    public partial class companies : System.Web.UI.Page
    {

        private TimeCompanies _currentCompany;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RefreshEntries();
            }
        }

        private void RefreshEntries()
        {

            CSList<TimeCompanies> companyList = TimeCompanies.List();

            if (companyList.Count > 0)
            {
                rptCompanies.DataSource = companyList;
                rptCompanies.DataBind();
            }
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "") { return; }
            _currentCompany = (TimeCompanies)Session["CurrentCompany"];
            if (_currentCompany == null)
            {
                //new company object
                TimeCompanies company = TimeCompanies.New();

                //fill object with data
                company.CompanyName = txtName.Text;
                company.Description = txtDescription.Text;
                company.Status = 1;
                company.Type = 1;

                //save the new company
                company.Save();
            }
            else
            {
                _currentCompany.CompanyName = txtName.Text;
                _currentCompany.Description = txtDescription.Text;
                _currentCompany.Status = 1;
                _currentCompany.Type = 1;

                //save the new company
                _currentCompany.Save();
            }

            txtName.Text = "";
            txtDescription.Text = "";
            _currentCompany = null;
            Session["CurrentCompany"] = _currentCompany;
            RefreshEntries();
            updEntries.Update();
        }

        //protected void btnDelete_Click(object sender, CommandEventArgs e)
        //{
        //    TimeCompanies bpe = TimeCompanies.ReadSafe(Convert.ToInt32(e.CommandArgument));
        //    if (bpe != null)
        //    {
        //        bpe.Delete();
        //        RefreshEntries();
        //        updEntries.Update();
        //    }
        //}

        protected void rptCompanies_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentCompany = TimeCompanies.Read(pkID);
                Session["CurrentCompany"] = _currentCompany;
                txtName.Text = _currentCompany.CompanyName;
                txtDescription.Text = _currentCompany.Description;
            }

            if (e.CommandName == "Delete")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentCompany = TimeCompanies.Read(pkID);
                _currentCompany.Delete();
                _currentCompany = null;
                Session["CurrentCompany"] = _currentCompany;
                RefreshEntries();
                updEntries.Update();
            }
        }
    }
}