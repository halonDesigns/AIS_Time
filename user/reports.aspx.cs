﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AIS_Time.classes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Vici.CoolStorage;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace AIS_Time.user
{
    public partial class reports : Page
    {
        private TimeProjectHours _currentProjectHours;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
             
            }
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

        protected void btnMonthlyEmployeeReport_Click(object sender, EventArgs e)
        {
            try
            {
                //grab the details on the employee
                TimeEmployees employee = TimeEmployees.ReadFirst("TimeEmployeeID = @TimeEmployeeID", "@TimeEmployeeID", (int)Session["TimeEmployeeID"]);

                //date wanted
                DateTime dt = DateTime.ParseExact(txtMonth.Text, "M/d/yyyy", CultureInfo.InvariantCulture);

                //find out how many days are in the month
                int days = DateTime.DaysInMonth(dt.Year, dt.Month);

                //grab the project hours drecords for the specified user
                //on the specified date BETWEEN '19/12/2012' AND '1/17/2013'
                //must use the $ for the sql specific BETWEEN keyword
                CSList<TimeProjectHours> projects = TimeProjectHours.List(
                    "TimeEmployeeID = @TimeEmployeeID AND DateOfWork >= @DateOfWorkStart AND DateOfWork <= @DateOfWorkEnd",
                    "@TimeEmployeeID", (int)Session["TimeEmployeeID"], "@DateOfWorkStart", dt, "@DateOfWorkEnd",
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
                TimeEmployees employee = TimeEmployees.ReadFirst("TimeEmployeeID = @TimeEmployeeID", "@TimeEmployeeID", (int)Session["TimeEmployeeID"]);

                //date wanted
                DateTime dt = DateTime.ParseExact(txtEmployeeWeekly.Text, "M/d/yyyy", CultureInfo.InvariantCulture);

                //grab the project hours drecords for the specified user
                //on the specified date BETWEEN '19/12/2012' AND '1/17/2013'
                //must use the $ for the sql specific BETWEEN keyword
                CSList<TimeProjectHours> projects = TimeProjectHours.List(
                    "TimeEmployeeID = @TimeEmployeeID AND DateOfWork >= @DateOfWorkStart AND DateOfWork <= @DateOfWorkEnd",
                    "@TimeEmployeeID", (int)Session["TimeEmployeeID"], "@DateOfWorkStart", dt, "@DateOfWorkEnd",
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

        private void CreateEmployeeWeeklyPDFReport(CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
        {
            try
            {
                string templatePdfPath = Server.MapPath("PDFimages");
                string oldFile = templatePdfPath + "\\EmployeeWeeklyTemplate.pdf";

                List<byte[]> pages = WriteToPdfForEmployeeWeekly(oldFile, projects, employee, dt);

                //if (b == null) return;
                //HttpResponse response = HttpContext.Current.Response;
                //response.Clear();
                //response.ContentType = "application/pdf";
                //response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}-{1}.pdf", "EmployeeWeeklyTimeCard-", employee.FirstName + "_" + employee.LastName));
                //response.BinaryWrite(b);
                //response.Flush();
                //response.End();

                if (pages == null) return;
                using (var output = new MemoryStream())
                {
                    var document = new Document();
                    var writer = new PdfCopy(document, output);
                    document.Open();
                    for (int index = 0; index < pages.Count; index++)
                    {
                        var file = pages[index];
                        var reader = new PdfReader(file);
                        int n = reader.NumberOfPages;
                        PdfImportedPage page;
                        for (int p = 1; p <= n; p++)
                        {
                            page = writer.GetImportedPage(reader, p);
                            writer.AddPage(page);
                        }
                    }
                    document.Close();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}-{1}.pdf", "EmployeeWeeklyTimeCard-", employee.FirstName + "_" + employee.LastName));
                    Response.BinaryWrite(output.ToArray());
                    Response.Flush();
                    Response.End();
                }

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

                List<byte[]> pages = WriteToPdfForEmployeeMonthly(oldFile, projects, employee, dt);

                //if (b == null) return;
                //HttpResponse response = HttpContext.Current.Response;
                //response.Clear();
                //response.ContentType = "application/pdf";
                //response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}-{1}.pdf", "MonthlyTimeCard-", employee.FirstName + "_" + employee.LastName));
                //response.BinaryWrite(b);
                //response.Flush();
                //response.End();

                if (pages == null) return;
                using (var output = new MemoryStream())
                {
                    var document = new Document();
                    var writer = new PdfCopy(document, output);
                    document.Open();
                    for (int index = 0; index < pages.Count; index++)
                    {
                        var file = pages[index];
                        var reader = new PdfReader(file);
                        int n = reader.NumberOfPages;
                        PdfImportedPage page;
                        for (int p = 1; p <= n; p++)
                        {
                            page = writer.GetImportedPage(reader, p);
                            writer.AddPage(page);
                        }
                    }
                    document.Close();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}-{1}.pdf", "MonthlyTimeCard-", employee.FirstName + "_" + employee.LastName));
                    Response.BinaryWrite(output.ToArray());
                    Response.Flush();
                    Response.End();
                }


            }
            catch (Exception ex)
            {
                lblErrorMonthly.Text = "Error: " + ex.Message;
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
                    TimeAISCodes classCode = TimeAISCodes.ReadFirst("TimeAISCodeID = @TimeAISCodeID", "@TimeAISCodeID", resource.TimeAISCodeID);

                    TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", timeProjectHourse.TimeProjectID);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectNumber, 55, (pageSize.Height - yPos), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, classCode.AISCode, 125, (pageSize.Height - yPos), 0);
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

        public List<byte[]> WriteToPdfForEmployeeWeekly(string sourceFile, CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
        {
            List<byte[]> pages = new List<byte[]>();

            int totalItemCount = 0;
            int currentLoopCount = 0;
            int totalHours = 0;

            while (totalItemCount < projects.Count)
            {
                PdfReader reader = new PdfReader(sourceFile);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // PDFStamper is the class we use from iTextSharp to alter an existing PDF.
                    PdfStamper pdfStamper = new PdfStamper(reader, memoryStream);

                    Rectangle pageSize = reader.GetPageSizeWithRotation(1);

                    PdfContentByte pdfPageContents = pdfStamper.GetOverContent(1);
                    pdfPageContents.BeginText(); // Start working with text.

                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, Encoding.ASCII.EncodingName, false);
                    pdfPageContents.SetFontAndSize(baseFont, 10); // 10 point font
                    pdfPageContents.SetRGBColorFill(0, 0, 0);

                    //Name
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, employee.FirstName + " " + employee.LastName + " (AIS-0" + employee.TimeEmployeeID + ")", 200, (pageSize.Height - 150), 0);

                    //Date of report
                    //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.ToShortDateString(), 155, (pageSize.Height - 188), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMMM dd, yyyy}", dt) + " to " + String.Format("{0:MMMM dd, yyyy}", dt.AddDays(7)), 200, (pageSize.Height - 172), 0);

                    int yPos = 221;
                    int localLoopCount = 0;
                    foreach (var timeProjectHourse in projects)
                    {
                        if (localLoopCount > currentLoopCount || currentLoopCount == 0)
                        {
                            TimeResources resource = TimeResources.ReadFirst("TimeResourceID = @TimeResourceID", "@TimeResourceID", timeProjectHourse.TimeResourceID);
                            TimeAISCodes classCode = TimeAISCodes.ReadFirst("TimeAISCodeID = @TimeAISCodeID", "@TimeAISCodeID", resource.TimeAISCodeID);
                            TimeDepartments deptCode = TimeDepartments.ReadFirst("TimeDepartmentID = @TimeDepartmentID", "@TimeDepartmentID", timeProjectHourse.TimeDepartmentID);
                            TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", timeProjectHourse.TimeProjectID);

                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMM d}", timeProjectHourse.DateOfWork), 70, (pageSize.Height - yPos), 0);
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectNumber, 125, (pageSize.Height - yPos), 0);
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, classCode.AISCode, 185, (pageSize.Height - yPos), 0);

                            //show the function
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deptCode.DepartmentName, 225, (pageSize.Height - yPos), 0);

                            //show the hours worked
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.HoursOfWork.ToString(), 725, (pageSize.Height - yPos), 0);

                            const int NUM_CHARS_ALLOWED = 85;
                            int numLines = 0;
                            if (timeProjectHourse.Description.Length <= NUM_CHARS_ALLOWED)
                                numLines = 1;
                            else if (timeProjectHourse.Description.Length > NUM_CHARS_ALLOWED && timeProjectHourse.Description.Length <= (NUM_CHARS_ALLOWED * 2))
                                numLines = 2;
                            else if (timeProjectHourse.Description.Length > (NUM_CHARS_ALLOWED * 2) && timeProjectHourse.Description.Length <= (NUM_CHARS_ALLOWED * 3))
                                numLines = 3;

                            int partCount = numLines;
                            string input = timeProjectHourse.Description;
                            var results = new string[partCount];
                            int rem = timeProjectHourse.Description.Length % NUM_CHARS_ALLOWED;
                            for (var i = 0; i < partCount; i++)
                            {
                                if (i == partCount - 1)
                                    results[i] = input.Substring(NUM_CHARS_ALLOWED * i, rem);
                                else
                                    results[i] = input.Substring(NUM_CHARS_ALLOWED * i, NUM_CHARS_ALLOWED);
                            }

                            for (var l = 0; l < numLines; l++)
                            {
                                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, results[l], 350, (pageSize.Height - yPos), 0);
                                yPos += 10;
                            }

                            //increment the total hours
                            totalHours += timeProjectHourse.HoursOfWork;

                            var botPos = (int)(pageSize.Height - yPos);
                            pdfPageContents.SetLineWidth((float).5);
                            pdfPageContents.MoveTo(65, botPos);
                            pdfPageContents.LineTo(pageSize.Width - 35, botPos);
                            pdfPageContents.Stroke();

                            yPos += 10;

                            totalItemCount++;

                            //check to see if we are at the bottom of the page
                            if (yPos > 540)
                            {
                                break;
                            }
                        }
                        localLoopCount++;
                    }

                    currentLoopCount = localLoopCount;

                    if (totalItemCount == projects.Count)
                    {
                        //Total
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Total Hours " + totalHours.ToString(), 730, (pageSize.Height - yPos), 0);
                    }

                    pdfPageContents.EndText(); // Done working with text
                    pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                    pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                    pages.Add(memoryStream.ToArray());
                }
            }

            return pages;
        }

        static string WordCut(string text, int cutOffLength, char[] separators)
        {
            cutOffLength = cutOffLength > text.Length ? text.Length : cutOffLength;
            int separatorIndex = text.Substring(0, cutOffLength).LastIndexOfAny(separators);
            if (separatorIndex > 0)
                return text.Substring(0, separatorIndex);
            return text.Substring(0, cutOffLength);
        }

        public List<byte[]> WriteToPdfForEmployeeMonthly(string sourceFile, CSList<TimeProjectHours> projects, TimeEmployees employee, DateTime dt)
        {
            List<byte[]> pages = new List<byte[]>();

            int totalItemCount = 0;
            int currentLoopCount = 0;
            int totalHours = 0;

            while (totalItemCount < projects.Count)
            {
                PdfReader reader = new PdfReader(sourceFile);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // PDFStamper is the class we use from iTextSharp to alter an existing PDF.
                    PdfStamper pdfStamper = new PdfStamper(reader, memoryStream);

                    Rectangle pageSize = reader.GetPageSizeWithRotation(1);

                    PdfContentByte pdfPageContents = pdfStamper.GetOverContent(1);
                    pdfPageContents.BeginText(); // Start working with text.

                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, Encoding.ASCII.EncodingName, false);
                    pdfPageContents.SetFontAndSize(baseFont, 10); // 10 point font
                    pdfPageContents.SetRGBColorFill(0, 0, 0);

                    //Name
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, employee.FirstName + " " + employee.LastName, 210, (pageSize.Height - 150), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "AIS-0" + employee.TimeEmployeeID, 525, (pageSize.Height - 150), 0);

                    //find out how many days are in the month
                    int days = DateTime.DaysInMonth(dt.Year, dt.Month);

                    //Date of report
                    //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, dt.ToShortDateString(), 155, (pageSize.Height - 188), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMMM dd, yyyy}", dt), 210, (pageSize.Height - 172), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMMM dd, yyyy}", dt.AddDays(days - 1)), 525, (pageSize.Height - 172), 0);

                    int yPos = 221;
                    int localLoopCount = 0;
                    foreach (var timeProjectHourse in projects)
                    {
                        if (localLoopCount > currentLoopCount || currentLoopCount == 0)
                        {
                            TimeResources resource = TimeResources.ReadFirst("TimeResourceID = @TimeResourceID", "@TimeResourceID", timeProjectHourse.TimeResourceID);
                            TimeAISCodes classCode = TimeAISCodes.ReadFirst("TimeAISCodeID = @TimeAISCodeID", "@TimeAISCodeID", resource.TimeAISCodeID);
                            TimeDepartments deptCode = TimeDepartments.ReadFirst("TimeDepartmentID = @TimeDepartmentID", "@TimeDepartmentID", timeProjectHourse.TimeDepartmentID);
                            TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", timeProjectHourse.TimeProjectID);

                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMM d}", timeProjectHourse.DateOfWork), 70, (pageSize.Height - yPos), 0);
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, project.ProjectNumber, 115, (pageSize.Height - yPos), 0);
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, classCode.AISCode, 175, (pageSize.Height - yPos), 0);

                            //show the function
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deptCode.DepartmentName, 220, (pageSize.Height - yPos), 0);

                            //show the hours worked
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.HoursOfWork.ToString(), 730, (pageSize.Height - yPos), 0);

                            const int NUM_CHARS_ALLOWED = 85;
                            int numLines = 0;
                            if (timeProjectHourse.Description.Length <= NUM_CHARS_ALLOWED)
                                numLines = 1;
                            else if (timeProjectHourse.Description.Length > NUM_CHARS_ALLOWED && timeProjectHourse.Description.Length <= (NUM_CHARS_ALLOWED * 2))
                                numLines = 2;
                            else if (timeProjectHourse.Description.Length > (NUM_CHARS_ALLOWED * 2) && timeProjectHourse.Description.Length <= (NUM_CHARS_ALLOWED * 3))
                                numLines = 3;

                            int partCount = numLines;
                            string input = timeProjectHourse.Description;
                            var results = new string[partCount];
                            int rem = timeProjectHourse.Description.Length % NUM_CHARS_ALLOWED;
                            for (var i = 0; i < partCount; i++)
                            {
                                if (i == partCount - 1)
                                    results[i] = input.Substring(NUM_CHARS_ALLOWED * i, rem);
                                else
                                    results[i] = input.Substring(NUM_CHARS_ALLOWED * i, NUM_CHARS_ALLOWED);
                            }

                            for (var l = 0; l < numLines; l++)
                            {
                                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, results[l], 345, (pageSize.Height - yPos), 0);
                                yPos += 10;
                            }

                            //increment the total hours
                            totalHours += timeProjectHourse.HoursOfWork;

                            var botPos = (int)(pageSize.Height - yPos);
                            pdfPageContents.SetLineWidth((float).5);
                            pdfPageContents.MoveTo(65, botPos);
                            pdfPageContents.LineTo(pageSize.Width - 35, botPos);
                            pdfPageContents.Stroke();

                            yPos += 10;

                            totalItemCount++;

                            //check to see if we are at the bottom of the page
                            if (yPos > 540)
                            {
                                break;
                            }
                        }
                        localLoopCount++;
                    }

                    currentLoopCount = localLoopCount;

                    if (totalItemCount == projects.Count)
                    {
                        //Total
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Total Hours " + totalHours.ToString(), 730, (pageSize.Height - yPos), 0);
                    }

                    pdfPageContents.EndText(); // Done working with text
                    pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                    pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                    pages.Add(memoryStream.ToArray());
                }
            }

            return pages;
        }

        public List<byte[]> WriteToPdfForProjectWeekly(string sourceFile, CSList<TimeProjectHours> projects, DateTime dt, TimeProjects projectDetails, WeeklyReportResult[] results)
        {
            List<byte[]> pages = new List<byte[]>();

            int totalItemCount = 0;
            int currentLoopCount = 0;
            int totalHours = 0;
            decimal totalCharge = 0;

            int pageCount = 0;
            while (totalItemCount < projects.Count)
            {
                pageCount += 1;
                PdfReader reader = new PdfReader(sourceFile);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // PDFStamper is the class we use from iTextSharp to alter an existing PDF.
                    PdfStamper pdfStamper = new PdfStamper(reader, memoryStream);

                    Rectangle pageSize = reader.GetPageSizeWithRotation(1);

                    PdfContentByte pdfPageContents = pdfStamper.GetOverContent(1);
                    pdfPageContents.BeginText(); // Start working with text.

                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, Encoding.ASCII.EncodingName, false);
                    pdfPageContents.SetFontAndSize(baseFont, 10); // 10 point font
                    pdfPageContents.SetRGBColorFill(0, 0, 0);

                    //      //customer
                    //TimeCustomers customer = TimeCustomers.ReadFirst("TimeCustomerID = @TimeCustomerID", "@TimeCustomerID", projectDetails.TimeCustomerID);
                    //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, customer.CustomerName, 210, (pageSize.Height - 150), 0);

                    //project name
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectName, 210, (pageSize.Height - 150), 0);

                    //Project Number
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectNumber, 525, (pageSize.Height - 150), 0);

                    //Date of report, shows week span
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMMM dd, yyyy}", dt), 210, (pageSize.Height - 172), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMMM dd, yyyy}", dt.AddDays(6)), 525, (pageSize.Height - 172), 0);


                    int yPos = 221;
                    int localLoopCount = 0;
                    foreach (var timeProjectHourse in projects)
                    {
                        if (localLoopCount > currentLoopCount || currentLoopCount == 0)
                        {
                            TimeResources resource = TimeResources.ReadFirst("TimeResourceID = @TimeResourceID", "@TimeResourceID", timeProjectHourse.TimeResourceID);
                            TimeAISCodes classCode = TimeAISCodes.ReadFirst("TimeAISCodeID = @TimeAISCodeID", "@TimeAISCodeID", resource.TimeAISCodeID);
                            TimeDepartments deptCode = TimeDepartments.ReadFirst("TimeDepartmentID = @TimeDepartmentID", "@TimeDepartmentID", timeProjectHourse.TimeDepartmentID);
                            TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", timeProjectHourse.TimeProjectID);
                            //TimeEmployees employee = TimeEmployees.ReadFirst("TimeEmployeeID = @TimeEmployeeID", "@TimeEmployeeID", timeProjectHourse.TimeEmployeeID);

                            //show the date worked
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMM d}", timeProjectHourse.DateOfWork), 82, (pageSize.Height - yPos), 0);
                            //show the employee id
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "AIS-0" + timeProjectHourse.TimeEmployeeID, 121, (pageSize.Height - yPos), 0);
                            //show the code
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, classCode.AISCode, 176, (pageSize.Height - yPos), 0);
                            //show the function
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deptCode.DepartmentName, 205, (pageSize.Height - yPos), 0);
                            //show the hours worked
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.HoursOfWork.ToString(), 640, (pageSize.Height - yPos), 0);
                            //show the hourly rate
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:C}", resource.HourlyRate), 662, (pageSize.Height - yPos), 0);

                            decimal totalForDay = timeProjectHourse.HoursOfWork * resource.HourlyRate;

                            //show the total dollars for that day
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:C}", totalForDay), 705, (pageSize.Height - yPos), 0);

                            const int NUM_CHARS_ALLOWED = 65;
                            int numLines = 0;
                            if (timeProjectHourse.Description.Length <= NUM_CHARS_ALLOWED)
                                numLines = 1;
                            else if (timeProjectHourse.Description.Length > NUM_CHARS_ALLOWED && timeProjectHourse.Description.Length <= (NUM_CHARS_ALLOWED * 2))
                                numLines = 2;
                            else if (timeProjectHourse.Description.Length > (NUM_CHARS_ALLOWED * 2) && timeProjectHourse.Description.Length <= (NUM_CHARS_ALLOWED * 3))
                                numLines = 3;
                            else if (timeProjectHourse.Description.Length > (NUM_CHARS_ALLOWED * 3) && timeProjectHourse.Description.Length <= (NUM_CHARS_ALLOWED * 4))
                                numLines = 4;

                            int partCount = numLines;
                            string input = timeProjectHourse.Description;
                            var descResults = new string[partCount];
                            int rem = timeProjectHourse.Description.Length % NUM_CHARS_ALLOWED;
                            for (var i = 0; i < partCount; i++)
                            {
                                if (i == partCount - 1)
                                    descResults[i] = input.Substring(NUM_CHARS_ALLOWED * i, rem);
                                else
                                    descResults[i] = input.Substring(NUM_CHARS_ALLOWED * i, NUM_CHARS_ALLOWED);
                            }

                            for (var l = 0; l < numLines; l++)
                            {
                                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, descResults[l], 325, (pageSize.Height - yPos), 0);
                                yPos += 10;
                            }

                            //increment the total hours
                            totalHours += timeProjectHourse.HoursOfWork;

                            //increment the total charge-out
                            totalCharge += totalForDay;

                            var botPos = (int)(pageSize.Height - yPos);
                            pdfPageContents.SetLineWidth((float).5);
                            pdfPageContents.MoveTo(68, botPos);
                            pdfPageContents.LineTo(pageSize.Width - 38, botPos);
                            pdfPageContents.Stroke();

                            yPos += 10;

                            totalItemCount++;

                            //check to see if we are at the bottom of the page
                            if (yPos > 540)
                            {
                                break;
                            }
                        }
                        localLoopCount++;
                    }

                    currentLoopCount = localLoopCount;

                    if (totalItemCount == projects.Count)
                    {
                        //Total
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Totals", 620, (pageSize.Height - yPos), 0);
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalHours.ToString(), 640, (pageSize.Height - yPos), 0);
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:C}", totalCharge), 705, (pageSize.Height - yPos), 0);

                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Page " + pageCount, 705, (pageSize.Height - (yPos + 20)), 0);
                    }
                    else
                    {
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Page " + pageCount, 705, (pageSize.Height - (yPos + 20)), 0);
                    }

                    pdfPageContents.EndText(); // Done working with text
                    pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                    pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                    pages.Add(memoryStream.ToArray());
                }
            }

            return pages;
        }

        public List<byte[]> WriteToPdfForProjectMonthly(string sourceFile, CSList<TimeProjectHours> projects, DateTime dt, TimeProjects projectDetails, WeeklyReportResult[] results)
        {
            List<byte[]> pages = new List<byte[]>();

            int totalItemCount = 0;
            int currentLoopCount = 0;
            int totalHours = 0;
            decimal totalCharge = 0;

            int pageCount = 0;
            while (totalItemCount < projects.Count)
            {
                pageCount += 1;
                PdfReader reader = new PdfReader(sourceFile);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // PDFStamper is the class we use from iTextSharp to alter an existing PDF.
                    PdfStamper pdfStamper = new PdfStamper(reader, memoryStream);

                    Rectangle pageSize = reader.GetPageSizeWithRotation(1);

                    PdfContentByte pdfPageContents = pdfStamper.GetOverContent(1);
                    pdfPageContents.BeginText(); // Start working with text.

                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, Encoding.ASCII.EncodingName, false);
                    pdfPageContents.SetFontAndSize(baseFont, 10); // 10 point font
                    pdfPageContents.SetRGBColorFill(0, 0, 0);

                    //      //customer
                    //TimeCustomers customer = TimeCustomers.ReadFirst("TimeCustomerID = @TimeCustomerID", "@TimeCustomerID", projectDetails.TimeCustomerID);
                    //pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, customer.CustomerName, 210, (pageSize.Height - 150), 0);

                    //project name
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectName, 210, (pageSize.Height - 150), 0);

                    //Project Number
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, projectDetails.ProjectNumber, 525, (pageSize.Height - 150), 0);

                    //find out how many days are in the month
                    int days = DateTime.DaysInMonth(dt.Year, dt.Month);

                    //Date of report, shows week span
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMMM dd, yyyy}", dt), 210, (pageSize.Height - 172), 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMMM dd, yyyy}", dt.AddDays(days - 1)), 525, (pageSize.Height - 172), 0);


                    int yPos = 221;
                    int localLoopCount = 0;
                    foreach (var timeProjectHourse in projects)
                    {
                        if (localLoopCount > currentLoopCount || currentLoopCount == 0)
                        {
                            TimeResources resource = TimeResources.ReadFirst("TimeResourceID = @TimeResourceID", "@TimeResourceID", timeProjectHourse.TimeResourceID);
                            TimeAISCodes classCode = TimeAISCodes.ReadFirst("TimeAISCodeID = @TimeAISCodeID", "@TimeAISCodeID", resource.TimeAISCodeID);
                            TimeDepartments deptCode = TimeDepartments.ReadFirst("TimeDepartmentID = @TimeDepartmentID", "@TimeDepartmentID", timeProjectHourse.TimeDepartmentID);
                            TimeProjects project = TimeProjects.ReadFirst("TimeProjectID = @TimeProjectID", "@TimeProjectID", timeProjectHourse.TimeProjectID);
                            //TimeEmployees employee = TimeEmployees.ReadFirst("TimeEmployeeID = @TimeEmployeeID", "@TimeEmployeeID", timeProjectHourse.TimeEmployeeID);

                            //show the date worked
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:MMM d}", timeProjectHourse.DateOfWork), 82, (pageSize.Height - yPos), 0);
                            //show the employee id
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "AIS-0" + timeProjectHourse.TimeEmployeeID, 121, (pageSize.Height - yPos), 0);
                            //show the code
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, classCode.AISCode, 176, (pageSize.Height - yPos), 0);
                            //show the function
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deptCode.DepartmentName, 205, (pageSize.Height - yPos), 0);
                            //show the hours worked
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, timeProjectHourse.HoursOfWork.ToString(), 640, (pageSize.Height - yPos), 0);
                            //show the hourly rate
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:C}", resource.HourlyRate), 662, (pageSize.Height - yPos), 0);

                            decimal totalForDay = timeProjectHourse.HoursOfWork * resource.HourlyRate;

                            //show the total dollars for that day
                            pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:C}", totalForDay), 705, (pageSize.Height - yPos), 0);

                            const int NUM_CHARS_ALLOWED = 65;
                            int numLines = 0;
                            if (timeProjectHourse.Description.Length <= NUM_CHARS_ALLOWED)
                                numLines = 1;
                            else if (timeProjectHourse.Description.Length > NUM_CHARS_ALLOWED && timeProjectHourse.Description.Length <= (NUM_CHARS_ALLOWED * 2))
                                numLines = 2;
                            else if (timeProjectHourse.Description.Length > (NUM_CHARS_ALLOWED * 2) && timeProjectHourse.Description.Length <= (NUM_CHARS_ALLOWED * 3))
                                numLines = 3;
                            else if (timeProjectHourse.Description.Length > (NUM_CHARS_ALLOWED * 3) && timeProjectHourse.Description.Length <= (NUM_CHARS_ALLOWED * 4))
                                numLines = 4;

                            int partCount = numLines;
                            string input = timeProjectHourse.Description;
                            var descResults = new string[partCount];
                            int rem = timeProjectHourse.Description.Length % NUM_CHARS_ALLOWED;
                            for (var i = 0; i < partCount; i++)
                            {
                                if (i == partCount - 1)
                                    descResults[i] = input.Substring(NUM_CHARS_ALLOWED * i, rem);
                                else
                                    descResults[i] = input.Substring(NUM_CHARS_ALLOWED * i, NUM_CHARS_ALLOWED);
                            }

                            for (var l = 0; l < numLines; l++)
                            {
                                pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, descResults[l], 325, (pageSize.Height - yPos), 0);
                                yPos += 10;
                            }

                            //increment the total hours
                            totalHours += timeProjectHourse.HoursOfWork;

                            //increment the total charge-out
                            totalCharge += totalForDay;

                            var botPos = (int)(pageSize.Height - yPos);
                            pdfPageContents.SetLineWidth((float).5);
                            pdfPageContents.MoveTo(68, botPos);
                            pdfPageContents.LineTo(pageSize.Width - 38, botPos);
                            pdfPageContents.Stroke();

                            yPos += 10;

                            totalItemCount++;

                            //check to see if we are at the bottom of the page
                            if (yPos > 480) //540
                            {
                                break;
                            }
                        }
                        localLoopCount++;
                    }

                    currentLoopCount = localLoopCount;

                    if (totalItemCount == projects.Count)
                    {
                        //Total
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Totals", 620, (pageSize.Height - yPos), 0);
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, totalHours.ToString(), 640, (pageSize.Height - yPos), 0);
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, String.Format("{0:C}", totalCharge), 705, (pageSize.Height - yPos), 0);

                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Page " + pageCount, 705, 60, 0);
                    }
                    else
                    {
                        pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Page " + pageCount, 705, 60, 0);
                    }

                    pdfPageContents.SetLineWidth((float).5);
                    pdfPageContents.MoveTo(68, 55);
                    pdfPageContents.LineTo(pageSize.Width - 325, 55);
                    pdfPageContents.Stroke();

                    pdfPageContents.MoveTo(68, 70);
                    pdfPageContents.LineTo(pageSize.Width - 325, 70);
                    pdfPageContents.Stroke();

                    pdfPageContents.MoveTo(68, 85);
                    pdfPageContents.LineTo(pageSize.Width - 325, 85);
                    pdfPageContents.Stroke();

                    pdfPageContents.MoveTo(68, 100);
                    pdfPageContents.LineTo(pageSize.Width - 325, 100);
                    pdfPageContents.Stroke();

                    //vertical
                    pdfPageContents.MoveTo(68, 55);
                    pdfPageContents.LineTo(68, 100);
                    pdfPageContents.Stroke();

                    pdfPageContents.MoveTo(191, 55);
                    pdfPageContents.LineTo(191, 100);
                    pdfPageContents.Stroke();

                    pdfPageContents.MoveTo(204, 55);
                    pdfPageContents.LineTo(204, 100);
                    pdfPageContents.Stroke();

                    pdfPageContents.MoveTo(304, 55);
                    pdfPageContents.LineTo(304, 100);
                    pdfPageContents.Stroke();

                    pdfPageContents.MoveTo(318, 55);
                    pdfPageContents.LineTo(318, 100);
                    pdfPageContents.Stroke();

                    pdfPageContents.MoveTo(445, 55);
                    pdfPageContents.LineTo(445, 100);
                    pdfPageContents.Stroke();

                    pdfPageContents.MoveTo(pageSize.Width - 325, 55);
                    pdfPageContents.LineTo(pageSize.Width - 325, 100);
                    pdfPageContents.Stroke();
                    //end vertical

                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, " Assistant Project Engineer   B   Software Developer       D   Advanced Specialist Engineer   F+   ", 78, 90, 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, " General Management           C   Specialist Engineer        D   Report Writing                           R   ", 78, 75, 0);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_LEFT, " Mathematician                     D   User Experience (GUI)  D   Technician                                  T4   ", 78, 60, 0);


                    pdfPageContents.EndText(); // Done working with text
                    pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                    pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 

                    pages.Add(memoryStream.ToArray());
                }
            }

            return pages;
        }

        protected void ddlEmployeeWeekly_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}