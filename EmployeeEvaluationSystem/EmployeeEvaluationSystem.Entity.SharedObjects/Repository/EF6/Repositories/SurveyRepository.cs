using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core.Repositories;
using EmployeeEvaluationSystem.SharedObjects.Exceptions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeEvaluationSystem.Entity.SharedObjects.Model.Survey;
using EmployeeEvaluationSystem.SharedObjects.Exceptions.Validitity;
using System.Data.Entity;
using EmployeeEvaluationSystem.SharedObjects.Enums;

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

            if (survey.UserTakenById == null)
            {
                return false;
            }

            if (survey.UserTakenById != userId)
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
            return this.dbcontext.PendingSurveys.FirstOrDefault(x => x.Id == pendingSurveyId && x.IsDeleted == false);
        }


        private PendingSurvey CreatePendingSurvey(string userId, int userRoleId, int surveyAvailableId, bool isExistingUser)
        {

            var exists = this.GetUserSurveyRole(userRoleId) != null;

            if (!exists)
            {
                throw new ItemNotFoundException($"The role {userRoleId} does not exist");
            }

            var user = this.unitOfWork.Users.GetUser(userId, userId);

            if (user == null)
            {
                throw new ItemNotFoundException("Unable to find the user to create the pending survey");
            }

            var surveyAvailable = this.GetAnAvailableSurveyForCohortSYSTEM(surveyAvailableId);

            if (surveyAvailable == null)
            {
                throw new ItemNotFoundException("Unable to find the survey available to create the pending survey");
            }

            var newPendingSurvey = new PendingSurvey
            {
                SurveyAvailToMeID = surveyAvailableId,
                UserSurveyRoleID = userRoleId,
                DateSent = DateTime.UtcNow,
                UserSurveyForId = userId,
                Email = isExistingUser ? null : userId,
                UserTakenById = isExistingUser ? userId : null,
            };

            this.dbcontext.PendingSurveys.Add(newPendingSurvey);

            return newPendingSurvey;
        }

        public PendingSurvey CreatePendingSurveyForExistingUser(string userId, int userRoleId, int surveyAvailableId)
        {
            return this.CreatePendingSurvey(userId, userRoleId, surveyAvailableId, true);
        }

        public PendingSurvey CreatePendingSurveyForGuestUser(string userEmail, int userRoleId, int surveyAvailableId)
        {
            return this.CreatePendingSurvey(userEmail, userRoleId, surveyAvailableId, false);
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

                    var theRoleID = x.First().RoleId;

                    var exists = this.GetUserSurveyRole(theRoleID) != null;

                    if (!exists)
                    {
                        throw new ItemNotFoundException($"The role {theRoleID} does not exist");
                    }


                    return new SurveysAvailableTo
                    {
                        UserSurveyRoleId = theRoleID,
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

            if (surv == null)
            {
                return false;
            }

            var currentTime = DateTime.UtcNow;

            return surv.DateOpen <= currentTime && currentTime <= surv.DateClosed;
        }

        public SurveysAvailable DeleteSurveyAvailable(string userId, int surveyAvailableId)
        {
            var survey = this.GetAnAvailableSurveyForCohortSYSTEM(surveyAvailableId);

            if (survey == null)
            {
                throw new ItemNotFoundException();
            }

            survey.IsDeleted = true;

            survey.DateDeleted = DateTime.UtcNow;

            return survey;
        }

        public UserSurveyRole GetUserSurveyRole(int roleID)
        {
            return this.dbcontext.UserSurveyRoles.FirstOrDefault(x => x.ID == roleID && x.IsDeleted == false);
        }

        public AnswerInstance AddAnswerInstanceToSurveyInstance(Guid pendingSurveyId, int questionId, CreateAnswerInstanceModel model)
        {
            var theId = this.GetPendingSurveySYSTEM(pendingSurveyId);

            if (theId?.SurveyInstanceID == null)
            {
                throw new ItemNotFoundException();
            }

            return this.AddAnswerInstanceToSurveyInstance(theId.SurveyInstanceID ?? -1, questionId, model);
        }

        public AnswerInstance AddAnswerInstanceToSurveyInstance(int surveyInstanceId, int questionId, CreateAnswerInstanceModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("The model cannot be null");
            }

            var theSurveyInstance = this.GetSurveyInstanceByIdSYSTEM(surveyInstanceId);

            if (theSurveyInstance == null)
            {
                throw new ItemNotFoundException("Unable to find the survey instance on the server.");
            }

            var theSurveyId = theSurveyInstance.SurveyID;


            var isQuestionInSurvey = this.IsQuestionInSurvey(questionId, theSurveyId);

            if (!isQuestionInSurvey)
            {
                throw new InvalidModelException("The current question is not in the specified survey.");
            }

            var questionType = this.GetQuestionTypeOfQuestion(questionId);

            if (questionType.IsRating)
            {
                var isValid = questionType.RatingMin <= model.RatingResponse && model.RatingResponse <= questionType.RatingMax;

                if (!isValid)
                {
                    throw new InvalidModelException("The rating does not fall between the specified min and max value.");
                }


                var entity = this.dbcontext.AnswerInstances.FirstOrDefault(x => x.QuestionID == questionId && x.SurveyInstanceId == surveyInstanceId);

                if(entity == null)
                {
                    var answerResult = new AnswerInstance
                    {
                        QuestionID = questionId,
                        SurveyInstanceId = surveyInstanceId,
                        ResponseNum = model.RatingResponse
                    };

                    theSurveyInstance.AnswerInstances.Add(answerResult);

                    return answerResult;
                }
                else
                {
                    entity.ResponseNum = model.RatingResponse;

                    return entity;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public Category GetCategory(int categoryId)
        {
            return this.dbcontext.Categories.FirstOrDefault(x => x.ID == categoryId && x.IsDeleted == false);
        }

        public Question GetQuestion(int questionId)
        {
            return this.dbcontext.Questions.FirstOrDefault(x => x.ID == questionId && x.IsDeleted == false);
        }

        public bool IsQuestionInSurvey(int questionId, int surveyId)
        {
            return this.dbcontext.Questions.Any(x => x.ID == questionId && x.Category.SurveyID == surveyId && x.IsDeleted == false && x.Category.IsDeleted == false && x.Category.Survey.IsDeleted == false);
        }

        public QuestionType GetQuestionType(int questionTypeId)
        {
            return this.dbcontext.QuestionTypes.FirstOrDefault(x => x.ID == questionTypeId);
        }

        public QuestionType GetQuestionTypeOfQuestion(int questionId)
        {
            return this.dbcontext.Questions.Where(x => x.ID == questionId && x.IsDeleted == false).Take(1).Select(x => x.QuestionType).FirstOrDefault();
        }

        public SurveyInstance GetSurveyInstanceById(string userIdTakingSurvey, int surveyInstanceId)
        {
            return this.GetSurveyInstanceByIdSYSTEM(surveyInstanceId);
        }

        public SurveyInstance GetSurveyInstanceByIdSYSTEM(int surveyInstanceId)
        {
            return this.dbcontext.SurveyInstances.FirstOrDefault(x => x.ID == surveyInstanceId);
        }

        public LockAndGetSurvey_Result LockAndGetSurvey(Guid pendingSurveyId, Guid? statusGuid = default(Guid?))
        {
            return this.dbcontext.LockAndGetSurvey(pendingSurveyId, statusGuid).FirstOrDefault();
        }

        public int CancelSurveyLock(Guid pendingSurveyId)
        {
            return this.dbcontext.CancelSurveyLock(pendingSurveyId);
        }

        public int UpdateSurveyLockTime(Guid pendingSurveyId)
        {
            return this.dbcontext.UpdateLockedSurveyTime(pendingSurveyId);
        }

        public Category GetFirstCategory(int surveyId)
        {
            return this.dbcontext.Categories.Where(x => x.SurveyID == surveyId).OrderBy(x => x.ID).FirstOrDefault();
        }

        public Category GetNextCategory(int categoryId)
        {
            return this.dbcontext.Categories.Where(x => x.ID == categoryId).Select(x => x.Survey.Categories.Where(y => y.ID > categoryId).OrderBy(y => y.ID).FirstOrDefault()).FirstOrDefault();
        }

        public Category GetPreviousCategory(int categoryId)
        {
            return this.dbcontext.Categories.Where(x => x.ID == categoryId).Select(x => x.Survey.Categories.Where(y => y.ID < categoryId).OrderByDescending(y => y.ID).FirstOrDefault()).FirstOrDefault();
        }

        public Category GetLastCategory(int surveyId)
        {
            return this.dbcontext.Categories.Where(x => x.SurveyID == surveyId).OrderByDescending(x => x.ID).FirstOrDefault();
        }

        public ICollection<Tuple<Question, AnswerInstance>> GetQuestionsAndPreviousResponsesForCategoryInSurveyInstance(int categoryId, int surveyInstanceId)
        {
            return this.dbcontext.Questions.Where(x => x.CategoryID == categoryId)
                .GroupJoin(this.dbcontext.AnswerInstances.Where(x => x.SurveyInstanceId == surveyInstanceId), x => x.ID, x => x.QuestionID, (q, a) => new { Question = q, Answer = a.FirstOrDefault() })
                .ToList().Select(x => Tuple.Create(x.Question, x.Answer)).ToList();
        }

        public bool IsQuestionRequired(int questionId)
        {
            return this.dbcontext.Questions.Any(x => x.ID == questionId && x.IsRequired == true);
        }
    }
}
