using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class ReportGenerationController : Controller
    {
        private EmployeeDatabaseEntities entities;

        // GET: ReportGeneration
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string ReportHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader stReader = new StringReader(ReportHtml);
                Document pdfReport = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfReport, stream);
                pdfReport.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfReport, stReader);
                pdfReport.Close();
                return File(stream.ToArray(), "application/pdf", "EvaluationReport.pdf");
            }
        }




    }
}