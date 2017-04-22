using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    public class ReportRepository : Repository, IReportRepository
    {

        public ReportRepository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext) : base(unitOfWork, dbcontext)
        {
        }
        
        public List<ReportRole> GetDetailsForReport(string userId, int surveyAvailableId)
        {
            var results = this.dbcontext.PendingSurveys
                .Where(x => x.IsDeleted == false && x.SurveyAvailToMeID == surveyAvailableId && x.UserSurveyForId == userId)
                .GroupBy(x => x.UserSurveyRoleID)
                .Select(x => new {
                    RoleId = x.Key,
                    RoleName = x.Select(y => y.UserSurveyRole.Name).FirstOrDefault(),
                    Questions = x.Select(y => y.SurveyInstance)
                        .Where(z => z.DateFinished != null)
                        .SelectMany(y => y.AnswerInstances)
                        .GroupBy(y => y.QuestionID)
                        .Select(y => new {
                            QuestionId = y.Key,
                            QuestionName = y.Select(z => z.Question.Name).FirstOrDefault(),
                            CategoryId = y.Select(z => z.Question.Category.ID).FirstOrDefault(),
                            CategoryName = y.Select(z => z.Question.Category.Name).FirstOrDefault(),
                            Avg = y.Select(z => z.ResponseNum).Average(),
                            Count = y.Count()
                        })
                }).ToList();

            var newResults = results.Select(
                x => new ReportRole
                {
                    Id = x.RoleId,
                    Name = x.RoleName,
                    Questions = x.Questions.Select(
                        y => new ReportQuestionAverage
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            QuestionId = y.QuestionId,
                            QuestionText = y.QuestionName,
                            RatingValue = y.Avg
                        }).ToList()
                }).ToList();


            var overall = this.dbcontext.PendingSurveys
                .Where(x => x.IsDeleted == false && x.SurveyAvailToMeID == surveyAvailableId && x.UserSurveyForId == userId).Select(y => y.SurveyInstance)
                        .Where(z => z.DateFinished != null)
                        .SelectMany(y => y.AnswerInstances)
                        .GroupBy(y => y.QuestionID)
                        .Select(y => new {
                            QuestionId = y.Key,
                            QuestionName = y.Select(z => z.Question.Name).FirstOrDefault(),
                            CategoryId = y.Select(z => z.Question.Category.ID).FirstOrDefault(),
                            CategoryName = y.Select(z => z.Question.Category.Name).FirstOrDefault(),
                            Avg = y.Select(z => z.ResponseNum).Average(),
                            Count = y.Count()
                        }).ToList();

            newResults.Add(new ReportRole
            {
                Name = "Overall",
                Questions = overall.Select(
                        y => new ReportQuestionAverage
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            QuestionId = y.QuestionId,
                            QuestionText = y.QuestionName,
                            RatingValue = y.Avg
                        }).ToList()
            });

            return newResults;
        }
    }
}
