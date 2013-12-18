using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AIS_Time.classes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Vici.CoolStorage;

namespace AIS_Time.admin
{
    public partial class reports : Page
    {
        private TimeProjectHours _currentProjectHours;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //RefreshEntries();
                LoadEmployees();
                LoadProjects();
            }
        }

        //private void RefreshEntries()
        //{
        //    CSList<TimeProjectHours> projectList = TimeProjectHours.List();

        //    if (projectList.Count > 0)
        //    {
        //        rptProjectHours.DataSource = projectList;
        //        rptProjectHours.DataBind();
        //    }
        //}

        private void LoadProjects()
        {
            CSList<TimeProjects> projects = TimeProjects.OrderedList("ProjectName");

            ddlProjects.DataSource = projects;
            ddlProjects.DataValueField = "TimeProjectID";
            ddlProjects.DataTextField = "ProjectName";
            ddlProjects.DataBind();
        }

        private void LoadEmployees()
        {
            CSList<TimeEmployees> employees = TimeEmployees.OrderedList("LastName");

            ddlEmployee.DataSource = employees;
            ddlEmployee.DataValueField = "TimeEmployeeID";
            ddlEmployee.DataTextField = "LastName";
            ddlEmployee.DataBind();
        }

        protected void rptCustomers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                string allKeys = Convert.ToString(e.CommandArgument);
                int pkID = Convert.ToInt32(allKeys);
                _currentProjectHours = TimeProjectHours.Read(pkID);
                _currentProjectHours.Delete();
                _currentProjectHours = null;
                Session["CurrentProjectHours"] = _currentProjectHours;
                //RefreshEntries();
                //updEntries.Update();
            }
        }

        protected void btnWeeklyReport_Click(object sender, EventArgs e)
        {
            ////grab the details on the project
            TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", ddlProjects.SelectedValue);

            //date wanted
            DateTime dt = DateTime.ParseExact(txtWeeklyDateStart.Text, "M/d/yyyy", CultureInfo.InvariantCulture);

            //grab the project hours drecords for the specified user
            //on the specified date
            CSList<TimeProjectHours> projects = TimeProjectHours.List("TimeProjectID = @TimeProjectID AND DateOfWork > @DateOfWork",
                    "@TimeProjectID", ddlProjects.SelectedValue, "@DateOfWork", dt);


            string sql = "Select TimeProjectHours.DateOfWork, TimeProjectHours.HoursOfWork, TimeProjectHours.TimeEmployeeID, TimeProjectHours.TimeProjectID";
            sql += " , (SELECT CEAClassCode FROM TimeCEAClassCodes WHERE TimeCEAClassCodeID = (SELECT TimeCEAClassCodeID FROM TimeResources WHERE TimeResources.TimeResourceID = ";
            sql += " (SELECT TimeResourceID FROM TimeEmployees WHERE TimeEmployees.TimeEmployeeId = TimeProjectHours.TimeEmployeeID))) as ClassCode,";
            sql += " (SELECT HourlyRate FROM TimeResources WHERE TimeResources.TimeResourceID = ";
            sql += " (SELECT TimeResourceID FROM TimeEmployees WHERE TimeEmployees.TimeEmployeeId = TimeProjectHours.TimeEmployeeID)) as HourlyRate";
            sql += " from TimeProjectHours where TimeProjectHours.DateOfWork > '" + "12/12/2013'";

            WeeklyReportResult[] results = CSDatabase.RunQuery<WeeklyReportResult>(sql);

            if (projects.Count > 0)
            {

                CreateWeeklyPDFReport(projects, dt, project, results);
            }
            else
            {
                lblError.Text = "No time cards to print";
            }
        }

        protected void btnDailyReport_Click(object sender, EventArgs e)
        {

            //grab the details on the employee
            TimeEmployees employee = TimeEmployees.ReadFirst("TimeEmployeeID = @TimeEmployeeID", "@TimeEmployeeID", ddlEmployee.SelectedValue);

            //date wanted
            DateTime dt = DateTime.ParseExact(txtDate.Text, "M/d/yyyy", CultureInfo.InvariantCulture);

            //grab the project hours drecords for the specified user
            //on the specified date
            CSList<TimeProjectHours> projects = TimeProjectHours.List("TimeEmployeeID = @TimeEmployeeID AND DateOfWork = @DateOfWork",
                    "@TimeEmployeeID", ddlEmployee.SelectedValue, "@DateOfWork", dt);

            if (projects.Count > 0)
            {

                CreateDailyPDFReport(projects, employee, dt);
            }
            else
            {
                lblError.Text = "No time cards to print";
            }
        }

        private void CreateDailyPDFReport(CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
        {
            try
            {
                string templatePdfPath = Server.MapPath("PDFimages");
                string oldFile = templatePdfPath + "\\DailyTemplate.pdf";

                byte[] b = WriteToPdfForDaily(oldFile, projects, employee, dt);
                if (b == null) return;
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ContentType = "application/pdf";
                response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}-{1}.pdf", "DailyTimeCard-", employee.FirstName + "_" + employee.LastName));
                response.BinaryWrite(b);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void CreateWeeklyPDFReport(CSList<TimeProjectHours> projects, DateTime dt, TimeProjects projectDetails, WeeklyReportResult[] results)
        {
            try
            {
                string templatePdfPath = Server.MapPath("PDFimages");
                string oldFile = templatePdfPath + "\\WeeklyTemplate.pdf";

                byte[] b = WriteToPdfForWeekly(oldFile, projects, dt, projectDetails,results);
                if (b == null) return;
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ContentType = "application/pdf";
                response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.pdf", "WeeklyTimeCard"));
                response.BinaryWrite(b);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }

        public byte[] WriteToPdfForDaily(string sourceFile, CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
        {
            PdfReader reader = new PdfReader(sourceFile);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // PDFStamper is the class we use from iTextSharp to alter an existing PDF.
                PdfStamper pdfStamper = new PdfStamper(reader, memoryStream);

                Rectangle pageSize = reader.GetPageSizeWithRotation(1);

                PdfContentByte pdfPageContents = pdfStamper.GetOverContent(1);
                pdfPageContents.BeginText(); // Start working with text.

                BaseFont baseFont = BaseFont.CreateFont(BaseFont.COURIER, Encoding.ASCII.EncodingName, false);
                pdfPageContents.SetFontAndSize(baseFont, 11); // 11 point font
                pdfPageContents.SetRGBColorFill(0, 0, 0);

                // Note: The x,y of the Pdf Matrix is from bottom left corner. 
                // This command tells iTextSharp to write the text at a certain location with a certain angle.
                // Again, this will angle the text from bottom left corner to top right corner and it will 
                // place the text in the middle of the page. 
                //
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, playerName, pageSize.Width / 2, (pageSize.Height / 2) + 115, 0);
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, teamName, pageSize.Width / 2, (pageSize.Height / 2) + 80, 0);

                //user Name
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, employee.FirstName + " " + employee.LastName, 155, (pageSize.Height - 168), 0);

                //user CEA Class
                TimeResources resource = TimeResources.ReadFirst("TimeResourceID = @TimeResourceID", "@TimeResourceID", employee.TimeResourceID);
                TimeCEAClassCodes classCode = TimeCEAClassCodes.ReadFirst("TimeCEAClassCodeID = @TimeCEAClassCodeID", "@TimeCEAClassCodeID", resource.TimeCEAClassCodeID);

                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, classCode.CEAClassCode, 155, (pageSize.Height - 190), 0);

                //Date of report
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.ToShortDateString(), 155, (pageSize.Height - 213), 0);

                int yPos = 263;
                int totalHours = 0;
                foreach (var timeProjectHourse in projects)
                {
                    //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse., 175, (pageSize.Height - 165), 0);

                    TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", timeProjectHourse.TimeProjectID);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectNumber, 75, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectName, 150, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.HoursOfWork.ToString(), 500, (pageSize.Height - yPos), 0);

                    //increment the total hours
                    totalHours += timeProjectHourse.HoursOfWork;

                    yPos += 20;
                }


                //Total
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalHours.ToString(), 500, (pageSize.Height - 465), 0);

                pdfPageContents.EndText(); // Done working with text
                pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                return memoryStream.ToArray();
            }

        }

        public byte[] WriteToPdfForWeekly(string sourceFile, CSList<TimeProjectHours> projects, DateTime dt, TimeProjects projectDetails, WeeklyReportResult[] results)
        {
            PdfReader reader = new PdfReader(sourceFile);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // PDFStamper is the class we use from iTextSharp to alter an existing PDF.
                PdfStamper pdfStamper = new PdfStamper(reader, memoryStream);

                Rectangle pageSize = reader.GetPageSizeWithRotation(1);

                PdfContentByte pdfPageContents = pdfStamper.GetOverContent(1);
                pdfPageContents.BeginText(); // Start working with text.

                BaseFont baseFont = BaseFont.CreateFont(BaseFont.COURIER, Encoding.ASCII.EncodingName, false);
                pdfPageContents.SetFontAndSize(baseFont, 11); // 11 point font
                pdfPageContents.SetRGBColorFill(0, 0, 0);

                // Note: The x,y of the Pdf Matrix is from bottom left corner. 
                // This command tells iTextSharp to write the text at a certain location with a certain angle.
                // Again, this will angle the text from bottom left corner to top right corner and it will 
                // place the text in the middle of the page. 
                //
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, playerName, pageSize.Width / 2, (pageSize.Height / 2) + 115, 0);
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, teamName, pageSize.Width / 2, (pageSize.Height / 2) + 80, 0);

                //user Name
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectNumber, 155, (pageSize.Height - 168), 0);

                //user CEA Class
                // TimeResources resource = TimeResources.ReadFirst("TimeResourceID = @TimeResourceID", "@TimeResourceID", employee.TimeResourceID);
                // TimeCEAClassCodes classCode = TimeCEAClassCodes.ReadFirst("TimeCEAClassCodeID = @TimeCEAClassCodeID", "@TimeCEAClassCodeID", resource.TimeCEAClassCodeID);

                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectName, 155, (pageSize.Height - 190), 0);

                //Date of report
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.ToShortDateString(), 155, (pageSize.Height - 213), 0);

                pdfPageContents.SetFontAndSize(baseFont, 8); // 8 point font

                //show the dates along the line
                int startX = 135;
                int upperY = 237;
                int lowerY = 244;
                const int INCREMENTER = 55;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt), startX, (pageSize.Height - upperY), 0);
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt.AddDays(1)), startX, (pageSize.Height - upperY), 0);
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt.AddDays(2)), startX, (pageSize.Height - upperY), 0);
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt.AddDays(3)), startX, (pageSize.Height - upperY), 0);
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt.AddDays(4)), startX, (pageSize.Height - upperY), 0);
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt.AddDays(5)), startX, (pageSize.Height - upperY), 0);

                startX = 135;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:MMM d}", dt), startX, (pageSize.Height - lowerY), 0);
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:MMM d}", dt.AddDays(1)), startX, (pageSize.Height - lowerY), 0);
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:MMM d}", dt.AddDays(2)), startX, (pageSize.Height - lowerY), 0);
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:MMM d}", dt.AddDays(3)), startX, (pageSize.Height - lowerY), 0);
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:MMM d}", dt.AddDays(4)), startX, (pageSize.Height - lowerY), 0);
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:MMM d}", dt.AddDays(5)), startX, (pageSize.Height - lowerY), 0);

                pdfPageContents.SetFontAndSize(baseFont, 11); // 11 point font
                int yPos = 263;
                int totalHours = 0;
                double totalCharge = 0.0;
                foreach (var timeProjectHourse in results)
                {
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.ClassCode, 60, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.HoursOfWork.ToString(), 125, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "$" + ((double)(timeProjectHourse.HourlyRate * timeProjectHourse.HoursOfWork)), 500, (pageSize.Height - yPos), 0);

                    //increment the total hours
                    totalHours += timeProjectHourse.HoursOfWork;
                    totalCharge += (double) (timeProjectHourse.HourlyRate*timeProjectHourse.HoursOfWork);

                    yPos += 20;
                }


                //Total
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalHours.ToString(), 500, (pageSize.Height - 663), 0);

                pdfPageContents.EndText(); // Done working with text
                pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                return memoryStream.ToArray();
            }

        }
    }
}