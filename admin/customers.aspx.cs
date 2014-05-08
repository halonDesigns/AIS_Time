using System;
using System.Web.UI.WebControls;
using AIS_Time.classes;
using Vici.CoolStorage;

namespace AIS_Time.admin
{
    public partial class customers : System.Web.UI.Page
    {

        private TimeCustomers _currentCustomer;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RefreshEntries();
            }
        }

        private void RefreshEntries()
        {

            CSList<TimeCustomers> customerList = TimeCustomers.List().OrderedBy("CustomerName");

            if (customerList.Count > 0)
            {
                rptCustomers.DataSource = customerList;
                rptCustomers.DataBind();
            }
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                lblError.Text = "Please enter a customer name.";
                return;
            }
            
            lblError.Text = "";
            _currentCustomer = (TimeCustomers) Session["CurrentCustomer"];
            if (_currentCustomer == null)
            {
                //new company object
                TimeCustomers company = TimeCustomers.New();

                //fill object with data
                company.CustomerName = txtName.Text;
                company.Description = txtDescription.Text;
                company.Status = 1;
                company.Type = 1;

                //save the new company
                company.Save();
            }
            else
            {
                _currentCustomer.CustomerName = txtName.Text;
                _currentCustomer.Description = txtDescription.Text;
                _currentCustomer.Status = 1;
                _currentCustomer.Type = 1;

                //save the new company
                _currentCustomer.Save();
            }

            txtName.Text = "";
            txtDescription.Text = "";
            _currentCustomer = null;
            Session["CurrentCustomer"] = _currentCustomer;
            RefreshEntries();
            updEntries.Update();

            lblSuccessMessage.Text = "Successfully submitted data!";
            mpSuccess.Show();
        }

        //protected void btnDelete_Click(object sender, CommandEventArgs e)
        //{
        //    TimeCustomers bpe = TimeCustomers.ReadSafe(Convert.ToInt32(e.CommandArgument));
        //    if (bpe != null)
        //    {
        //        bpe.Delete();
        //        RefreshEntries();
        //        updEntries.Update();
        //    }
        //}

        protected void rptCustomers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
           if (e.CommandName == "Edit")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentCustomer = TimeCustomers.Read(pkID);
                Session["CurrentCustomer"] = _currentCustomer;
                txtName.Text = _currentCustomer.CustomerName;
               txtDescription.Text = _currentCustomer.Description;
            }

            if (e.CommandName == "Delete")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentCustomer = TimeCustomers.Read(pkID);
                _currentCustomer.Delete();
                _currentCustomer = null;
                Session["CurrentCustomer"] = _currentCustomer;
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