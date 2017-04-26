using System.Linq;
using System.Web.Mvc;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.MVC.Models.Report;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    public class ReportGenerationController : Controller
    {


        private IUnitOfWorkCreator creator;

        public IUnitOfWorkCreator Creator
        {
            get { return creator ?? HttpContext.GetOwinContext().Get<IUnitOfWorkCreator>(); }
            private set { creator = value; }
        }

        public ReportGenerationController()
        {
        }

        public ReportGenerationController(IUnitOfWorkCreator creator)
        {
            this.creator = creator;
        }

        // GET: ReportGeneration
        public ActionResult Index()
        { 
            return View();
        }
        
        public ActionResult ReportPage(string userId, int survAvailId)
        {
            using(var unitOfWork = this.Creator.Create())
            {
                if(userId != null)
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
                    var title2 = $"Evaluation {type}({stage})- generated on " + dateCreated.Date;
                    var model = new ReportDetailsViewModel
                    {
                        ResponseItems = reportDetails,
                        Categories = reportDetails.SelectMany(x => x.Questions).GroupBy(x => x.CategoryId).Select(x => new ReportCategory { Id = x.Key, Name = x.FirstOrDefault()?.CategoryName, Questions = x.GroupBy(y => y.QuestionId).Select(y => new ReportQuestion { Id = y.Key, Text = y.FirstOrDefault()?.QuestionText }).ToList() }).ToList(),
                        Header = title,
                        Header2 = title2

                    };

                    return View(model);
                }
                else
                {
                    var reportDetails = unitOfWork.Reports.GetDetailsForReport(survAvailId);
                    var sa = unitOfWork.Surveys.GetAnAvailableSurveyForCohortSYSTEM(survAvailId);

                    var type = sa.Survey.Name;
                    var stage = sa.SurveyType.Name;
                    var dateCreated = sa.SurveyType.DateCreated;

                    var title = "Cohort Evaluation Report";
                    var title2 = $"Evaluation {type}({stage})- generated on " + dateCreated.Date;
                    var model = new ReportDetailsViewModel
                    {
                        ResponseItems = reportDetails,
                        Categories = reportDetails.SelectMany(x => x.Questions).GroupBy(x => x.CategoryId).Select(x => new ReportCategory { Id = x.Key, Name = x.FirstOrDefault()?.CategoryName, Questions = x.GroupBy(y => y.QuestionId).Select(y => new ReportQuestion { Id = y.Key, Text = y.FirstOrDefault()?.QuestionText }).ToList() }).ToList(),
                        Header = title,
                        Header2 = title2

                    };

                    return View(model);
                }
            }
        }


    }
}