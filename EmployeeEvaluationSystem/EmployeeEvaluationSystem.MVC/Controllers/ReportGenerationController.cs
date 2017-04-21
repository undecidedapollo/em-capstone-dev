using System.Linq;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using System.Data;
using EmployeeEvaluationSystem.MVC.Models.Report;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class ReportGenerationController : Controller
    {

        // GET: ReportGeneration
        public ActionResult Index()
        { 
            return View();
        }
        
        public ActionResult ReportPage(string userId, int survAvailId)
        {
            using(var unitOfWork = new UnitOfWork())
            {
                var reportDetails = unitOfWork.Reports.GetDetailsForReport(userId, survAvailId);
                var sa = unitOfWork.Surveys.GetAnAvailableSurveyForCohortSYSTEM(survAvailId);
                var user = unitOfWork.Users.GetUser(userId, userId);

                var type = sa.Survey.Name;
                var stage = sa.SurveyType.Name;
                var dateCreated = sa.SurveyType.DateCreated;

                var firstName = user.FirstName;
                var lastName = user.LastName;

                var title = "Employee " + firstName + " " + lastName + " " + " Evaluation Report";
                var title2 = "Evaluation " + stage + " " + type + " " + "- generated on " + dateCreated.Date;
                var model = new ReportDetailsViewModel
                {
                    ResponseItems = reportDetails,
                    Categories = reportDetails.SelectMany(x => x.Questions).GroupBy(x => x.CategoryId).Select(x =>  new ReportCategory { Id = x.Key, Name = x.FirstOrDefault()?.CategoryName, Questions = x.GroupBy(y => y.QuestionId).Select(y => new ReportQuestion { Id = y.Key, Text = y.FirstOrDefault()?.QuestionText }).ToList() }).ToList(),
                    Header = title,
                    Header2 = title2
                    
                };

                return View(model);
            }
        }


    }
}