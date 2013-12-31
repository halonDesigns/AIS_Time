using System;
using System.Collections.Generic;
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
                txtDate.Text = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
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

            ddlMonthlyProjects.DataSource = projects;
            ddlMonthlyProjects.DataValueField = "TimeProjectID";
            ddlMonthlyProjects.DataTextField = "ProjectName";
            ddlMonthlyProjects.DataBind();
        }

        private void LoadEmployees()
        {
            CSList<TimeEmployees> employees = TimeEmployees.OrderedList("LastName");

            ddlEmployee.DataSource = employees;
            ddlEmployee.DataValueField = "TimeEmployeeID";
            ddlEmployee.DataTextField = "LastName";
            ddlEmployee.DataBind();

            ddlEmployeeMonthly.DataSource = employees;
            ddlEmployeeMonthly.DataValueField = "TimeEmployeeID";
            ddlEmployeeMonthly.DataTextField = "LastName";
            ddlEmployeeMonthly.DataBind();

            ddlEmployeeWeekly.DataSource = employees;
            ddlEmployeeWeekly.DataValueField = "TimeEmployeeID";
            ddlEmployeeWeekly.DataTextField = "LastName";
            ddlEmployeeWeekly.DataBind();
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

        protected void btnWeeklyProjectReport_Click(object sender, EventArgs e)
        {
            try
            {
                //grab the details on the project
                TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", ddlProjects.SelectedValue);

                //date wanted
                DateTime dt = DateTime.ParseExact(txtWeeklyDateStart.Text, "M/d/yyyy", CultureInfo.InvariantCulture);

                //grab the project hours drecords for the specified user
                //on the specified date
                CSList<TimeProjectHours> projects = TimeProjectHours.List("TimeProjectID = @TimeProjectID AND DateOfWork >= @DateOfWorkStart AND DateOfWork <= @DateOfWorkEnd",
                        "@TimeProjectID", ddlProjects.SelectedValue, "@DateOfWorkStart", dt, "@DateOfWorkEnd", dt.AddDays(7));

                string sql = "SELECT TimeProjectHours.DateOfWork, TimeProjectHours.HoursOfWork, TimeProjectHours.TimeEmployeeID ";
                sql += "  , TimeProjectHours.TimeProjectID, TimeResources.HourlyRate, TimeCEAClassCodes.CEAClassCode, TimeProjectHours.TimeDepartmentID ";
                sql += " FROM dbo.TimeProjectHours ";
                sql += " INNER JOIN dbo.TimeResources ON TimeProjectHours.TimeResourceID = TimeResources.TimeResourceID ";
                sql += " INNER JOIN dbo.TimeCEAClassCodes ON TimeResources.TimeAISCodeID = TimeCEAClassCodes.TimeCEAClassCodeID ";
                sql += " INNER JOIN dbo.TimeDepartments ON TimeProjectHours.TimeDepartmentID = TimeDepartments.TimeDepartmentID ";
                sql += " WHERE TimeProjectHours.TimeProjectID = @TimeProjectID";
                sql += " AND TimeProjectHours.DateOfWork >= @DateOfWorkStart AND TimeProjectHours.DateOfWork <= @DateOfWorkEnd";

                //add in the parameters
                var collection = new CSParameterCollection
                {
                    {"@TimeProjectID", ddlProjects.SelectedValue},
                    {"@DateOfWorkStart", dt},
                    {"@DateOfWorkEnd", dt.AddDays(7)}
                };

                WeeklyReportResult[] results = CSDatabase.RunQuery<WeeklyReportResult>(sql, collection);

                if (projects.Count > 0)
                {
                    CreateProjectWeeklyPDFReport(projects, dt, project, results);
                }
                else
                {
                    lblErrorWeeklyReport.Text = "No time cards to print";
                }
            }
            catch (Exception ex)
            {
                lblErrorWeeklyReport.Text = "Error: " + ex.Message;
            }
        }

        protected void btnMonthlyProjectsReport_Click(object sender, EventArgs e)
        {
            try
            {
                //grab the details on the project
                TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", ddlMonthlyProjects.SelectedValue);
                
                //date wanted
                DateTime dt = DateTime.ParseExact(txtProjectMonthlyDateStart.Text, "M/d/yyyy", CultureInfo.InvariantCulture);

                //find out how many days are in the month
                int days = DateTime.DaysInMonth(dt.Year, dt.Month);

                //grab the project hours drecords for the specified user
                //on the specified date
                CSList<TimeProjectHours> projects = TimeProjectHours.List("TimeProjectID = @TimeProjectID AND DateOfWork >= @DateOfWorkStart AND DateOfWork <= @DateOfWorkEnd",
                        "@TimeProjectID", ddlMonthlyProjects.SelectedValue, "@DateOfWorkStart", dt, "@DateOfWorkEnd", dt.AddDays(days));

                string sql = "SELECT TimeProjectHours.DateOfWork, TimeProjectHours.HoursOfWork, TimeProjectHours.TimeEmployeeID ";
                sql += "  , TimeProjectHours.TimeProjectID, TimeResources.HourlyRate, TimeCEAClassCodes.CEAClassCode, TimeProjectHours.TimeDepartmentID ";
                sql += " FROM dbo.TimeProjectHours ";
                sql += " INNER JOIN dbo.TimeResources ON TimeProjectHours.TimeResourceID = TimeResources.TimeResourceID ";
                sql += " INNER JOIN dbo.TimeCEAClassCodes ON TimeResources.TimeAISCodeID = TimeCEAClassCodes.TimeCEAClassCodeID ";
                sql += " INNER JOIN dbo.TimeDepartments ON TimeProjectHours.TimeDepartmentID = TimeDepartments.TimeDepartmentID ";
                sql += " WHERE TimeProjectHours.TimeProjectID = @TimeProjectID";
                sql += " AND TimeProjectHours.DateOfWork >= @DateOfWorkStart AND TimeProjectHours.DateOfWork <= @DateOfWorkEnd";

                //add in the parameters
                var collection = new CSParameterCollection
                {
                    {"@TimeProjectID", ddlMonthlyProjects.SelectedValue},
                    {"@DateOfWorkStart", dt},
                    {"@DateOfWorkEnd", dt.AddDays(days)}
                };

                WeeklyReportResult[] results = CSDatabase.RunQuery<WeeklyReportResult>(sql, collection);

                if (projects.Count > 0)
                {
                    CreateProjectMonthlyPDFReport(projects, dt, project, results);
                }
                else
                {
                    lblErrorProjectMonthlyReport.Text = "No time cards to print";
                }
            }
            catch (Exception ex)
            {
                lblErrorProjectMonthlyReport.Text = "Error: " + ex.Message;
            }
        }

        protected void btnMonthlyEmployeeReport_Click(object sender, EventArgs e)
        {
            try
            {
                //grab the details on the employee
                TimeEmployees employee = TimeEmployees.ReadFirst("TimeEmployeeID = @TimeEmployeeID", "@TimeEmployeeID", ddlEmployeeMonthly.SelectedValue);

                //date wanted
                DateTime dt = DateTime.ParseExact(txtMonth.Text, "M/d/yyyy", CultureInfo.InvariantCulture);

                //find out how many days are in the month
                int days = DateTime.DaysInMonth(dt.Year, dt.Month);

                //grab the project hours drecords for the specified user
                //on the specified date BETWEEN '19/12/2012' AND '1/17/2013'
                //must use the $ for the sql specific BETWEEN keyword
                CSList<TimeProjectHours> projects = TimeProjectHours.List(
                    "TimeEmployeeID = @TimeEmployeeID AND DateOfWork >= @DateOfWorkStart AND DateOfWork <= @DateOfWorkEnd",
                    "@TimeEmployeeID", ddlEmployeeMonthly.SelectedValue, "@DateOfWorkStart", dt, "@DateOfWorkEnd",
                    dt.AddDays(days)).OrderedBy("DateOfWork");

                if (projects.Count > 0)
                {
                    CreateMonthlyEmployeePDFReport(projects, employee, dt);
                }
                else
                {
                    lblErrorMonthly.Text = "No time cards to print";
                }
            }
            catch (Exception ex)
            {
                lblErrorMonthly.Text = "Error: " + ex.Message;
            }
        }

        protected void btnWeeklyEmployeeReport_Click(object sender, EventArgs e)
        {
            try
            {
                //grab the details on the employee
                TimeEmployees employee = TimeEmployees.ReadFirst("TimeEmployeeID = @TimeEmployeeID", "@TimeEmployeeID", ddlEmployeeWeekly.SelectedValue);

                //date wanted
                DateTime dt = DateTime.ParseExact(txtEmployeeWeekly.Text, "M/d/yyyy", CultureInfo.InvariantCulture);

                //grab the project hours drecords for the specified user
                //on the specified date BETWEEN '19/12/2012' AND '1/17/2013'
                //must use the $ for the sql specific BETWEEN keyword
                CSList<TimeProjectHours> projects = TimeProjectHours.List(
                    "TimeEmployeeID = @TimeEmployeeID AND DateOfWork >= @DateOfWorkStart AND DateOfWork <= @DateOfWorkEnd",
                    "@TimeEmployeeID", ddlEmployeeWeekly.SelectedValue, "@DateOfWorkStart", dt, "@DateOfWorkEnd",
                    dt.AddDays(7)).OrderedBy("DateOfWork");

                if (projects.Count > 0)
                {
                    CreateEmployeeWeeklyPDFReport(projects, employee, dt);
                }
                else
                {
                    lblErrorEmployeeWeekly.Text = "No time cards to print";
                }
            }
            catch (Exception ex)
            {
                lblErrorEmployeeWeekly.Text = "Error: " + ex.Message;
            }
        }

        protected void btnDailyEmployeeReport_Click(object sender, EventArgs e)
        {
            try
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

                    CreateDailyEmployeePDFReport(projects, employee, dt);
                }
                else
                {
                    lblError.Text = "No time cards to print";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void CreateDailyEmployeePDFReport(CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
        {
            try
            {
                string templatePdfPath = Server.MapPath("PDFimages");
                string oldFile = templatePdfPath + "\\DailyTemplate.pdf";

                byte[] b = WriteToPdfForEmployeeDaily(oldFile, projects, employee, dt);
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

        private void CreateEmployeeWeeklyPDFReport(CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
        {
            try
            {
                string templatePdfPath = Server.MapPath("PDFimages");
                string oldFile = templatePdfPath + "\\EmployeeWeeklyTemplate.pdf";

                byte[] b = WriteToPdfForEmployeeWeekly(oldFile, projects, employee, dt);
                if (b == null) return;
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ContentType = "application/pdf";
                response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}-{1}.pdf", "EmployeeWeeklyTimeCard-", employee.FirstName + "_" + employee.LastName));
                response.BinaryWrite(b);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                lblErrorEmployeeWeekly.Text = "Error: " + ex.Message;
            }
        }

        private void CreateMonthlyEmployeePDFReport(CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
        {
            try
            {
                string templatePdfPath = Server.MapPath("PDFimages");
                string oldFile = templatePdfPath + "\\MonthlyEmployeeTemplate.pdf";

                byte[] b = WriteToPdfForMonthly(oldFile, projects, employee, dt);
                if (b == null) return;
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ContentType = "application/pdf";
                response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}-{1}.pdf", "MonthlyTimeCard-", employee.FirstName + "_" + employee.LastName));
                response.BinaryWrite(b);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                lblErrorMonthly.Text = "Error: " + ex.Message;
            }
        }

        private void CreateProjectWeeklyPDFReport(CSList<TimeProjectHours> projects, DateTime dt, TimeProjects projectDetails, WeeklyReportResult[] results)
        {
            try
            {
                string templatePdfPath = Server.MapPath("PDFimages");
                string oldFile = templatePdfPath + "\\WeeklyProjectTemplate.pdf";

                byte[] b = WriteToPdfForProjectWeekly(oldFile, projects, dt, projectDetails, results);
                if (b == null) return;
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ContentType = "application/pdf";
                response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.pdf", "WeeklyProjectTimeCard"));
                response.BinaryWrite(b);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                lblErrorWeeklyReport.Text = "Error: " + ex.Message;
            }
        }

        private void CreateProjectMonthlyPDFReport(CSList<TimeProjectHours> projects, DateTime dt, TimeProjects projectDetails, WeeklyReportResult[] results)
        {
            try
            {
                string templatePdfPath = Server.MapPath("PDFimages");
                string oldFile = templatePdfPath + "\\MonthlyProjectTemplate.pdf";

                byte[] b = WriteToPdfForProjectMonthly(oldFile, projects, dt, projectDetails, results);
                if (b == null) return;
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ContentType = "application/pdf";
                response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.pdf", "MonthlyProjectTimeCard"));
                response.BinaryWrite(b);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                lblErrorProjectMonthlyReport.Text = "Error: " + ex.Message;
            }
        }

        public byte[] WriteToPdfForEmployeeDaily(string sourceFile, CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
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
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, employee.FirstName + " " + employee.LastName + " (AIS-0" + employee.TimeEmployeeID + ")", 155, (pageSize.Height - 168), 0);

                //Date of report
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.ToShortDateString(), 155, (pageSize.Height - 188), 0);

                int yPos = 241;
                int totalHours = 0;
                foreach (var timeProjectHourse in projects)
                {
                    TimeResources resource = TimeResources.ReadFirst("TimeResourceID = @TimeResourceID", "@TimeResourceID", timeProjectHourse.TimeResourceID);
                    TimeCEAClassCodes classCode = TimeCEAClassCodes.ReadFirst("TimeCEAClassCodeID = @TimeCEAClassCodeID", "@TimeCEAClassCodeID", resource.TimeCEAClassCodeID);

                    TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", timeProjectHourse.TimeProjectID);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectNumber, 55, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, classCode.CEAClassCode, 125, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectName, 185, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.HoursOfWork.ToString(), 500, (pageSize.Height - yPos), 0);

                    //increment the total hours
                    totalHours += timeProjectHourse.HoursOfWork;

                    yPos += 20;
                }

                //Total
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalHours.ToString(), 500, (pageSize.Height - 685), 0);

                pdfPageContents.EndText(); // Done working with text
                pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                return memoryStream.ToArray();
            }

        }

        public byte[] WriteToPdfForEmployeeWeekly(string sourceFile, CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
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
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, employee.FirstName + " " + employee.LastName + " (AIS-0" + employee.TimeEmployeeID + ")", 200, (pageSize.Height - 150), 0);

                //Date of report
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.ToShortDateString(), 155, (pageSize.Height - 188), 0);
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMM d}", dt) + " to ", 200, (pageSize.Height - 172), 0);
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMM d}", dt.AddDays(7)), 270, (pageSize.Height - 172), 0);
                
                int yPos = 221;
                int totalHours = 0;
                foreach (var timeProjectHourse in projects)
                {
                    TimeResources resource = TimeResources.ReadFirst("TimeResourceID = @TimeResourceID", "@TimeResourceID", timeProjectHourse.TimeResourceID);
                    TimeCEAClassCodes classCode = TimeCEAClassCodes.ReadFirst("TimeCEAClassCodeID = @TimeCEAClassCodeID", "@TimeCEAClassCodeID", resource.TimeCEAClassCodeID);
                    TimeDepartments deptCode = TimeDepartments.ReadFirst("TimeDepartmentID = @TimeDepartmentID", "@TimeDepartmentID", timeProjectHourse.TimeDepartmentID);
                    TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", timeProjectHourse.TimeProjectID);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMM d}", timeProjectHourse.DateOfWork), 70, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectNumber, 125, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, classCode.CEAClassCode, 185, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deptCode.DepartmentCode, 225, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectName, 280, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.HoursOfWork.ToString(), 725, (pageSize.Height - yPos), 0);

                    //increment the total hours
                    totalHours += timeProjectHourse.HoursOfWork;

                    yPos += 20;
                }

                //Total
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalHours.ToString(), 725, (pageSize.Height - 525), 0);

                pdfPageContents.EndText(); // Done working with text
                pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                return memoryStream.ToArray();
            }

        }

        public byte[] WriteToPdfForMonthly(string sourceFile, CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
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
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, employee.FirstName + " " + employee.LastName + " (AIS-0" + employee.TimeEmployeeID + ")", 155, (pageSize.Height - 168), 0);

                //Date of report
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:y}", dt), 155, (pageSize.Height - 188), 0);

                int yPos = 241;
                int totalHours = 0;
                foreach (var timeProjectHourse in projects)
                {
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.DateOfWork.Day.ToString(), 65, (pageSize.Height - yPos), 0);
                    TimeResources resource = TimeResources.ReadFirst("TimeResourceID = @TimeResourceID", "@TimeResourceID", timeProjectHourse.TimeResourceID);
                    TimeCEAClassCodes classCode = TimeCEAClassCodes.ReadFirst("TimeCEAClassCodeID = @TimeCEAClassCodeID", "@TimeCEAClassCodeID", resource.TimeCEAClassCodeID);

                    TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", timeProjectHourse.TimeProjectID);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectNumber, 115, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, classCode.CEAClassCode, 185, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectName, 245, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.HoursOfWork.ToString(), 525, (pageSize.Height - yPos), 0);

                    //increment the total hours
                    totalHours += timeProjectHourse.HoursOfWork;

                    yPos += 20;
                }

                //Total
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalHours.ToString(), 525, (pageSize.Height - 725), 0);

                pdfPageContents.EndText(); // Done working with text
                pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                return memoryStream.ToArray();
            }

        }

        public byte[] WriteToPdfForProjectWeekly(string sourceFile, CSList<TimeProjectHours> projects, DateTime dt, TimeProjects projectDetails, WeeklyReportResult[] results)
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

                //customer
                TimeCustomers customer = TimeCustomers.ReadFirst("TimeCustomerID = @TimeCustomerID", "@TimeCustomerID", projectDetails.TimeCustomerID);
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, customer.CustomerName, 165, (pageSize.Height - 151), 0);

                //Project Number
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectNumber, 165, (pageSize.Height - 173), 0);

                //project name
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectName, 165, (pageSize.Height - 196), 0);

                //Date of report, shows week span
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.ToShortDateString(), 265, (pageSize.Height - 219), 0);
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.AddDays(6).ToShortDateString(), 485, (pageSize.Height - 219), 0);

                //set array of current dates
                DateTime[] dtArr = new DateTime[7];
                for (int y = 0; y < 7; y++)
                    dtArr[y] = dt.AddDays(y);

                pdfPageContents.SetFontAndSize(baseFont, 8); // 8 point font

                //show the dates along the line
                int startX = 230;
                int lowerY = 245;
                const int INCREMENTER = 53;

                //startX = 135;
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
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:MMM d}", dt.AddDays(6)), startX, (pageSize.Height - lowerY), 0);

                //pdfPageContents.SetFontAndSize(baseFont, 11); // 11 point font
                int yPos = 266; //start line y pixel for table values
                int totalHours = 0;
                double totalCharge = 0.0;
                int dateStartX = 225;
                List<string> codeList = new List<string>();
                List<double> hourlyRateForCodeList = new List<double>();
                List<int> employeeIDList = new List<int>();
                List<int> departmentIDList = new List<int>();

                bool firstRun = true;
                foreach (var timeProjectHourse in results)
                {
                    if (firstRun)
                    {
                        codeList.Add(timeProjectHourse.CEAClassCode);
                        hourlyRateForCodeList.Add((double)timeProjectHourse.HourlyRate);
                        employeeIDList.Add(timeProjectHourse.TimeEmployeeID);
                        departmentIDList.Add(timeProjectHourse.TimeDepartmentID);
                        firstRun = false;
                    }
                    else
                    {
                        if (!codeList.Contains(timeProjectHourse.CEAClassCode))
                        {
                            codeList.Add(timeProjectHourse.CEAClassCode);
                            employeeIDList.Add(timeProjectHourse.TimeEmployeeID);
                            departmentIDList.Add(timeProjectHourse.TimeDepartmentID);
                            hourlyRateForCodeList.Add((double)timeProjectHourse.HourlyRate);
                        }
                    }
                }

                //6 days by the number of codes found
                int[,] dailyTotalsForCodeArr = new int[7, codeList.Count];

                foreach (var timeProjectHourse in results)
                {
                    if (timeProjectHourse.DateOfWork == dtArr[0])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[0, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[1])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[1, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[2])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[2, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[3])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[3, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[4])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[4, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[5])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[5, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[6])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[6, index] += timeProjectHourse.HoursOfWork;
                    }
                }

                for (int finCodeArr = 0; finCodeArr < codeList.Count; finCodeArr++)
                {
                    int totalCodeHours = 0;
                    double totalCodeCharge = 0.0;
                    int printDateX;

                    //show the employeeID in the first column
                    TimeEmployees employee = TimeEmployees.ReadFirst("TimeEmployeeID = @TimeEmployeeID", "@TimeEmployeeID", employeeIDList[finCodeArr]);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "AIS-0" + employee.TimeEmployeeID, 55, (pageSize.Height - yPos), 0);

                    //show the code in the second column
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, codeList[finCodeArr], 115, (pageSize.Height - yPos), 0);

                    TimeDepartments deptCode = TimeDepartments.ReadFirst("TimeDepartmentID = @TimeDepartmentID", "@TimeDepartmentID", departmentIDList[finCodeArr]);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deptCode.DepartmentCode, 160, (pageSize.Height - yPos), 0);

                    //now list the totals for this code per day
                    for (int finArr = 0; finArr < 7; finArr++)
                    {
                        //where are we going to print
                        printDateX = dateStartX + (INCREMENTER * finArr);

                        //print the days total hours worked for this code
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dailyTotalsForCodeArr[finArr, finCodeArr].ToString(), printDateX, (pageSize.Height - yPos), 0);

                        //increment the total hours
                        totalCodeHours += dailyTotalsForCodeArr[finArr, finCodeArr];
                        totalCodeCharge += (double)(hourlyRateForCodeList[finCodeArr] * dailyTotalsForCodeArr[finArr, finCodeArr]);

                    }

                    //show the total hours for the code for the week
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalCodeHours.ToString(), 615, (pageSize.Height - yPos), 0);
                    totalHours += totalCodeHours;
                    //show the total charged for the code for the week
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "$" + totalCodeCharge, 660, (pageSize.Height - yPos), 0);
                    totalCharge += totalCodeCharge;

                    //now increment the y val line
                    yPos += 20;
                }

                //Total Hours
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalHours.ToString(), 615, (pageSize.Height - 527), 0);
                //Total Charge
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "$" + totalCharge, 660, (pageSize.Height - 527), 0);

                pdfPageContents.EndText(); // Done working with text
                pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                return memoryStream.ToArray();
            }

        }

        public byte[] WriteToPdfForProjectMonthly(string sourceFile, CSList<TimeProjectHours> projects, DateTime dt, TimeProjects projectDetails, WeeklyReportResult[] results)
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

                //customer
                TimeCustomers customer = TimeCustomers.ReadFirst("TimeCustomerID = @TimeCustomerID", "@TimeCustomerID", projectDetails.TimeCustomerID);
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, customer.CustomerName, 165, (pageSize.Height - 151), 0);

                //Project Number
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectNumber, 165, (pageSize.Height - 173), 0);

                //project name
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectName, 165, (pageSize.Height - 196), 0);

                //Date of report, shows week span
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.ToShortDateString(), 265, (pageSize.Height - 219), 0);
                //find out how many days are in the month
                int days = DateTime.DaysInMonth(dt.Year, dt.Month);
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.AddDays(days).ToShortDateString(), 485, (pageSize.Height - 219), 0);

                //set array of current dates
                DateTime[] dtArr = new DateTime[7];
                for (int y = 0; y < 7; y++)
                    dtArr[y] = dt.AddDays(y);

                pdfPageContents.SetFontAndSize(baseFont, 8); // 8 point font

                //show the dates along the line
                int startX = 230;
                int lowerY = 245;
                const int INCREMENTER = 53;

                //startX = 135;
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
                startX = startX + INCREMENTER;
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:MMM d}", dt.AddDays(6)), startX, (pageSize.Height - lowerY), 0);

                //pdfPageContents.SetFontAndSize(baseFont, 11); // 11 point font
                int yPos = 266; //start line y pixel for table values
                int totalHours = 0;
                double totalCharge = 0.0;
                int dateStartX = 225;
                List<string> codeList = new List<string>();
                List<double> hourlyRateForCodeList = new List<double>();
                List<int> employeeIDList = new List<int>();
                List<int> departmentIDList = new List<int>();

                bool firstRun = true;
                foreach (var timeProjectHourse in results)
                {
                    if (firstRun)
                    {
                        codeList.Add(timeProjectHourse.CEAClassCode);
                        hourlyRateForCodeList.Add((double)timeProjectHourse.HourlyRate);
                        employeeIDList.Add(timeProjectHourse.TimeEmployeeID);
                        departmentIDList.Add(timeProjectHourse.TimeDepartmentID);
                        firstRun = false;
                    }
                    else
                    {
                        if (!codeList.Contains(timeProjectHourse.CEAClassCode))
                        {
                            codeList.Add(timeProjectHourse.CEAClassCode);
                            employeeIDList.Add(timeProjectHourse.TimeEmployeeID);
                            departmentIDList.Add(timeProjectHourse.TimeDepartmentID);
                            hourlyRateForCodeList.Add((double)timeProjectHourse.HourlyRate);
                        }
                    }
                }

                //6 days by the number of codes found
                int[,] dailyTotalsForCodeArr = new int[7, codeList.Count];

                foreach (var timeProjectHourse in results)
                {
                    if (timeProjectHourse.DateOfWork == dtArr[0])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[0, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[1])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[1, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[2])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[2, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[3])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[3, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[4])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[4, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[5])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[5, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[6])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.CEAClassCode);
                        dailyTotalsForCodeArr[6, index] += timeProjectHourse.HoursOfWork;
                    }
                }

                for (int finCodeArr = 0; finCodeArr < codeList.Count; finCodeArr++)
                {
                    int totalCodeHours = 0;
                    double totalCodeCharge = 0.0;
                    int printDateX;

                    //show the employeeID in the first column
                    TimeEmployees employee = TimeEmployees.ReadFirst("TimeEmployeeID = @TimeEmployeeID", "@TimeEmployeeID", employeeIDList[finCodeArr]);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "AIS-0" + employee.TimeEmployeeID, 55, (pageSize.Height - yPos), 0);

                    //show the code in the second column
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, codeList[finCodeArr], 115, (pageSize.Height - yPos), 0);

                    TimeDepartments deptCode = TimeDepartments.ReadFirst("TimeDepartmentID = @TimeDepartmentID", "@TimeDepartmentID", departmentIDList[finCodeArr]);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deptCode.DepartmentCode, 160, (pageSize.Height - yPos), 0);

                    //now list the totals for this code per day
                    for (int finArr = 0; finArr < 7; finArr++)
                    {
                        //where are we going to print
                        printDateX = dateStartX + (INCREMENTER * finArr);

                        //print the days total hours worked for this code
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dailyTotalsForCodeArr[finArr, finCodeArr].ToString(), printDateX, (pageSize.Height - yPos), 0);

                        //increment the total hours
                        totalCodeHours += dailyTotalsForCodeArr[finArr, finCodeArr];
                        totalCodeCharge += (double)(hourlyRateForCodeList[finCodeArr] * dailyTotalsForCodeArr[finArr, finCodeArr]);

                    }

                    //show the total hours for the code for the week
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalCodeHours.ToString(), 615, (pageSize.Height - yPos), 0);
                    totalHours += totalCodeHours;
                    //show the total charged for the code for the week
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "$" + totalCodeCharge, 660, (pageSize.Height - yPos), 0);
                    totalCharge += totalCodeCharge;

                    //now increment the y val line
                    yPos += 20;
                }

                //Total Hours
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalHours.ToString(), 615, (pageSize.Height - 527), 0);
                //Total Charge
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "$" + totalCharge, 660, (pageSize.Height - 527), 0);

                pdfPageContents.EndText(); // Done working with text
                pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                return memoryStream.ToArray();
            }
        }
    }
}