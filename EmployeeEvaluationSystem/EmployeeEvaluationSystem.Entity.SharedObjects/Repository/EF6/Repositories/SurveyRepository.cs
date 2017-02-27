using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;
using EmployeeEvaluationSystem.SharedObjects.Exceptions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Survey;
using EmployeeEvaluationSystem.SharedObjects.Exceptions.Validitity;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    public class SurveyRepository : Repository, ISurveyRepository
    {
        public SurveyRepository(UnitOfWork unitOfWork, EmployeeDatabaseEntities dbcontext) : base(unitOfWork, dbcontext)
        {

        }

        public bool CanExistingUserTakeSurvey(string userId, Guid pendingSurveyId)
        {
            var survey = this.GetPendingSurveySYSTEM(pendingSurveyId);

            if (survey == null)
            {
                return false;
            }

            if (survey.UserForId == null)
            {
                return false;
            }

            if (survey.UserForId != userId)
            {
                return false;
            }

            return true;
        }

        public bool CanGuestUserTakeSurvey(string userEmail, Guid pendingSurveyId)
        {
            var survey = this.GetPendingSurveySYSTEM(pendingSurveyId);

            if (survey == null)
            {
                return false;
            }

            if (survey.Email == null)
            {
                return false;
            }

            if (survey.Email != userEmail)
            {
                return false;
            }

            return true;
        }

        private SurveyInstance CreateSurveyInstance(string userId, Guid pendingSurveyId, bool isExistingUser)
        {
            var survey = this.GetPendingSurveySYSTEM(pendingSurveyId);

            if (survey == null)
            {
                throw new ItemNotFoundException();
            }

            var surveyToTake = survey.SurveysAvailable.SurveyID;

            var instance = new SurveyInstance
            {
                UserTakenBy = isExistingUser ? userId : null,
                DateStarted = DateTime.UtcNow,
                SurveyID = surveyToTake
            };

            survey.SurveyInstance = instance;

            return instance;
        }

        public SurveyInstance CreateSurveyInstanceForExistingUser(string userIdTakingSurvey, Guid pendingSurveyId)
        {
            return this.CreateSurveyInstance(userIdTakingSurvey, pendingSurveyId, true);
        }

        public SurveyInstance CreateSurveyInstanceForGuestUser(string userIdTakingSurvey, Guid pendingSurveyId)
        {
            return this.CreateSurveyInstance(userIdTakingSurvey, pendingSurveyId, false);
        }

        public bool DoesSurveyInstanceAlreadyExistSYSTEM(Guid pendingSurveyId)
        {
            var survey = this.GetPendingSurveySYSTEM(pendingSurveyId);

            if (survey == null)
            {
                return false;
            }

            return survey.SurveyInstanceID != null;
        }

        public PendingSurvey GetPendingSurvey(string userId, Guid pendingSurveyId)
        {
            return this.GetPendingSurveySYSTEM(pendingSurveyId);
        }

        public SurveyInstance GetPendingSurveyInstance(Guid pendingSurveyId)
        {
            var survey = this.GetPendingSurveySYSTEM(pendingSurveyId);

            return survey?.SurveyInstance;
        }

        public PendingSurvey GetPendingSurveySYSTEM(Guid pendingSurveyId)
        {
            return this.dbcontext.PendingSurveys.FirstOrDefault(x => x.Id == pendingSurveyId);
        }

        public SurveysAvailable CreateAnAvailableSurveyForCohort(string currentUserID, CreateAvailableSurveyModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("Model cannot be null");
            }

            if (string.IsNullOrWhiteSpace(currentUserID))
            {
                throw new ArgumentNullException("CurrentUserId cannot be null");
            }

            //Check Permissions

            var user = this.unitOfWork.Users.GetUser(currentUserID, currentUserID);

            if (user == null)
            {
                throw new ItemNotFoundException("Unable to find the user to create the available survey");
            }

            var theCohort = unitOfWork.Cohorts.GetCohort(currentUserID, model.CohortId);

            if (theCohort == null)
            {
                throw new ItemNotFoundException("Unable to find the cohort to create this survey.");
            }

            var theSurvey = this.GetSurvey(currentUserID, model.SurveyId);

            if (theSurvey == null)
            {
                throw new ItemNotFoundException("Unable to find the base survey to create this survey.");
            }

            var theSurveyType = this.GetSurveyType(currentUserID, model.SurveyId);

            if (theSurveyType == null)
            {
                throw new ItemNotFoundException("Unable to find the survey type to create this survey.");
            }

            var validRoles = model.RolesSurveyFor
                .GroupBy(x => x.RoleId)
                .Select(x =>
                {
                    if (x.Count() != 1)
                    {
                        throw new InvalidModelException("There must only be one instance for each role Id.");
                    }

                    if (x.First().Quantity <= 0)
                    {
                        throw new InvalidModelException("The quantity for a role must be greater than 0.");
                    }

                    return new SurveysAvailableTo
                    {
                        UserSurveyRoleId = x.First().RoleId,
                        Quantity = x.First().Quantity
                    };
                }).ToList();

            var theSurveyAvailable = new SurveysAvailable
            {
                SurveyID = model.SurveyId,
                CohortID = model.CohortId,
                SurveyTypeId = model.SurveyTypeId,
                DateCreated = DateTime.UtcNow,
                DateOpen = model.DateStart.ToUniversalTime(),
                DateClosed = model.DateEnd.ToUniversalTime(),
                SurveysAvailableToes = validRoles
            };

            this.dbcontext.SurveysAvailables.Add(theSurveyAvailable);

            return theSurveyAvailable;
        }

        public SurveysAvailable GetAnAvailableSurveyForCohort(string currentUserID, int surveyAvailableId)
        {
            return this.GetAnAvailableSurveyForCohortSYSTEM(surveyAvailableId);
        }

        public Survey GetSurvey(string userId, int surveyId)
        {
            return this.GetSurveySYSTEM(surveyId);
        }

        public Survey GetSurveySYSTEM(int surveyId)
        {
            return this.dbcontext.Surveys.FirstOrDefault(x => x.ID == surveyId && x.IsDeleted == false);
        }

        public SurveyType GetSurveyType(string userId, int surveyTypeId)
        {
            return this.GetSurveyTypeSYSTEM(surveyTypeId);
        }

        public SurveyType GetSurveyTypeSYSTEM(int surveyTypeId)
        {
            return this.dbcontext.SurveyTypes.FirstOrDefault(x => x.ID == surveyTypeId && x.IsDeleted == false);
        }

        public SurveysAvailable GetAnAvailableSurveyForCohortSYSTEM(int surveyAvailableId)
        {
            return this.dbcontext.SurveysAvailables.FirstOrDefault(x => x.ID == surveyAvailableId && x.IsDeleted == false);
        }

        public bool IsSurveyAvailableStillOpen(int surveyAvailableId)
        {
            var surv = this.GetAnAvailableSurveyForCohortSYSTEM(surveyAvailableId);

            if(surv == null)
            {
                return false;
            }

            var currentTime = DateTime.UtcNow;

            return surv.DateOpen <= currentTime && currentTime <= surv.DateClosed;
        }

        public SurveysAvailable DeleteSurveyAvailable(string userId, int surveyAvailableId)
        {
            var survey = this.GetAnAvailableSurveyForCohortSYSTEM(surveyAvailableId);

            if(survey == null)
            {
                throw new ItemNotFoundException();
            }

            survey.IsDeleted = true;

            survey.DateDeleted = DateTime.UtcNow;

            return survey;
        }
    }
}
