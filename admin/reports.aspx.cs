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


            //string sql = "Select TimeProjectHours.DateOfWork, TimeProjectHours.HoursOfWork, TimeProjectHours.TimeEmployeeID, TimeProjectHours.TimeProjectID";
            //sql += " , (SELECT CEAClassCode FROM TimeCEAClassCodes WHERE TimeCEAClassCodeID = (SELECT TimeCEAClassCodeID FROM TimeResources WHERE TimeResources.TimeResourceID = ";
            //sql += " (SELECT TimeResourceID FROM TimeEmployees WHERE TimeEmployees.TimeEmployeeId = TimeProjectHours.TimeEmployeeID))) as ClassCode,";
            //sql += " (SELECT HourlyRate FROM TimeResources WHERE TimeResources.TimeResourceID = ";
            //sql += " (SELECT TimeResourceID FROM TimeEmployees WHERE TimeEmployees.TimeEmployeeId = TimeProjectHours.TimeEmployeeID)) as HourlyRate";
            //sql += " from TimeProjectHours where TimeProjectHours.DateOfWork > '" + "12/12/2013'";

            string sql = "Select TimeProjectHours.DateOfWork, TimeProjectHours.HoursOfWork, TimeProjectHours.TimeEmployeeID, TimeProjectHours.TimeProjectID ";
            sql += " , (SELECT CEAClassCode FROM TimeCEAClassCodes WHERE TimeCEAClassCodeID = (SELECT TimeCEAClassCodeID FROM TimeResources WHERE TimeResources.TimeResourceID = TimeProjectHours.TimeResourceID)) as ClassCode, ";
            sql += " (SELECT HourlyRate FROM TimeResources WHERE TimeResources.TimeResourceID = TimeProjectHours.TimeResourceID) as HourlyRate ";
            sql += " from TimeProjectHours where TimeProjectHours.DateOfWork > ' 12/12/2013'";

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

                byte[] b = WriteToPdfForWeekly(oldFile, projects, dt, projectDetails, results);
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

                //customer
                TimeCustomers customer = TimeCustomers.ReadFirst("TimeCustomerID = @TimeCustomerID", "@TimeCustomerID", projectDetails.TimeCustomerID);
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, customer.CustomerName, 155, (pageSize.Height - 168), 0);

                //Project Number
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectNumber, 155, (pageSize.Height - 190), 0);

                //project name
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectName, 155, (pageSize.Height - 213), 0);

                //Date of report, shows week span
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.ToShortDateString(), 225, (pageSize.Height - 236), 0);
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.AddDays(5).ToShortDateString(), 385, (pageSize.Height - 236), 0);

                //set array of current dates
                DateTime[] dtArr = new DateTime[7];
                for (int y = 0; y < 7; y++)
                    dtArr[y] = dt.AddDays(y);

                pdfPageContents.SetFontAndSize(baseFont, 8); // 8 point font

                //show the dates along the line
                int startX = 163;
                //int upperY = 237;
                int lowerY = 265;
                const int INCREMENTER = 44;
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt), startX, (pageSize.Height - upperY), 0);
                //startX = startX + INCREMENTER;
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt.AddDays(1)), startX, (pageSize.Height - upperY), 0);
                //startX = startX + INCREMENTER;
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt.AddDays(2)), startX, (pageSize.Height - upperY), 0);
                //startX = startX + INCREMENTER;
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt.AddDays(3)), startX, (pageSize.Height - upperY), 0);
                //startX = startX + INCREMENTER;
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt.AddDays(4)), startX, (pageSize.Height - upperY), 0);
                //startX = startX + INCREMENTER;
                //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, String.Format("{0:dddd}", dt.AddDays(5)), startX, (pageSize.Height - upperY), 0);

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
                int yPos = 283; //start line y pixel for table values
                int totalHours = 0;
                double totalCharge = 0.0;
                int dateStartX = 163;
                List<string> codeList = new List<string>();
                List<double> hourlyRateForCodeList = new List<double>();
                List<int> employeeIDList = new List<int>();

                bool firstRun = true;
                foreach (var timeProjectHourse in results)
                {
                    if (firstRun)
                    {
                        codeList.Add(timeProjectHourse.ClassCode);
                        hourlyRateForCodeList.Add((double)timeProjectHourse.HourlyRate);
                        employeeIDList.Add(timeProjectHourse.TimeEmployeeID);
                        firstRun = false;
                    }
                    else
                    {
                        if (!codeList.Contains(timeProjectHourse.ClassCode))
                        {
                            codeList.Add(timeProjectHourse.ClassCode);
                            hourlyRateForCodeList.Add((double)timeProjectHourse.HourlyRate);
                            employeeIDList.Add(timeProjectHourse.TimeEmployeeID);
                        }
                    }
                }

                //6 days by the number of codes found
                int[,] dailyTotalsForCodeArr = new int[7, codeList.Count];

                foreach (var timeProjectHourse in results)
                {
                    if (timeProjectHourse.DateOfWork == dtArr[0])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.ClassCode);
                        dailyTotalsForCodeArr[0, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[1])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.ClassCode);
                        dailyTotalsForCodeArr[1, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[2])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.ClassCode);
                        dailyTotalsForCodeArr[2, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[3])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.ClassCode);
                        dailyTotalsForCodeArr[3, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[4])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.ClassCode);
                        dailyTotalsForCodeArr[4, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[5])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.ClassCode);
                        dailyTotalsForCodeArr[5, index] += timeProjectHourse.HoursOfWork;
                    }
                    else if (timeProjectHourse.DateOfWork == dtArr[6])
                    {
                        int index = codeList.IndexOf(timeProjectHourse.ClassCode);
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
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalCodeHours.ToString(), 465, (pageSize.Height - yPos), 0);
                    totalHours += totalCodeHours;
                    //show the total charged for the code for the week
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "$" + totalCodeCharge, 508, (pageSize.Height - yPos), 0);
                    totalCharge += totalCodeCharge;

                    //now increment the y val line
                    yPos += 20;
                }

                //Total Hours
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalHours.ToString(), 465, (pageSize.Height - 727), 0);
                //Total Charge
                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "$" + totalCharge, 508, (pageSize.Height - 727), 0);

                pdfPageContents.EndText(); // Done working with text
                pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                return memoryStream.ToArray();
            }

        }



    }
}