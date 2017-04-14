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

            var item = theCohort.SurveysAvailables.Where(x => x.SurveyID == model.SurveyId && x.IsDeleted == false).OrderByDescending(X => X.ID).FirstOrDefault();


            if (item == null)
            {
                if(model.SurveyTypeId != 1)
                {
                    throw new Exception("Unable to create survey");
                }
            }
            else
            {

                var nextResult = unitOfWork.Surveys.GetNextAvailableSurveyTypeForSurveyInCohort(model.SurveyId, model.CohortId);

                if (nextResult == null || nextResult.ID != model.SurveyTypeId )
                {
                    throw new Exception("Unable to create survey");

                }
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

            var cohortUsers = theCohort.CohortUsers;

            foreach(var theUser in cohortUsers)
            {
                if(theUser == null)
                {
                    continue;
                }

                var newPendingSurvey = new PendingSurvey
                {
                    DateSent = DateTime.UtcNow,
                    StatusId = 1,
                    IsDeleted = false,
                    SurveyAvailToMeID = Convert.ToInt32(SurveyRoleEnum.SELF),
                    UserSurveyForId = theUser.UserID,
                    UserTakenById = theUser.UserID
                };

                theSurveyAvailable.PendingSurveys.Add(newPendingSurvey);
            }

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

        public ICollection<UserSurveyRole> GetUserSurveyRoles()
        {
            return this.dbcontext.UserSurveyRoles.Where(x => x.IsDeleted == false).ToList();
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

                if (entity == null)
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

        public ICollection<Survey> GetAllSurveys(string currentUserID)
        {
            return this.dbcontext.Surveys.ToList();
        }

        public ICollection<PendingSurvey> GetAllSurveysForUser(string userId)
        {
            return this.dbcontext.PendingSurveys.Where(x => x.UserTakenById == userId && x.IsDeleted == false).ToList();
        }

        public ICollection<SurveyType> GetAllSurveyTypes(string currentUserID)
        {
            return this.dbcontext.SurveyTypes.ToList();
        }

        public ICollection<PendingSurvey> GetPendingSurveysForUser(string userId)
        {
            return this.dbcontext.PendingSurveys.Where(x => x.UserTakenById == userId && x.IsDeleted == false && (x.SurveyInstance == null || x.SurveyInstance.DateFinished == null))
                .Include(x => x.SurveysAvailable)
                .Include(x => x.SurveysAvailable.Survey)
                .ToList();
        }

        public ICollection<PendingSurvey> GetFinishedSurveysForUser(string userId)
        {
            return this.dbcontext.PendingSurveys.Where(x => x.UserTakenById == userId && x.IsDeleted == false && x.SurveyInstance != null && x.SurveyInstance.DateFinished != null)
                                .Include(x => x.SurveysAvailable)
                                .Include(x => x.SurveysAvailable.Survey)
                                .ToList();
        }

        public bool IsQuestionRequired(int questionId)
        {
            return this.dbcontext.Questions.Any(x => x.ID == questionId && x.IsRequired == true);
        }

        public bool FinishSurvey(int surveyInstanceId, Guid? statusGuid = default(Guid?))
        {
            var pendingSurvey = this.dbcontext.SurveyInstances.FirstOrDefault(x => x.ID == surveyInstanceId);

            var surveyId = pendingSurvey.SurveyID;

            var anyMissing = this.dbcontext.Questions.Where(x => x.Category.SurveyID == surveyId && x.IsRequired == true && x.IsDeleted == false && x.Category.IsDeleted == false).GroupJoin(this.dbcontext.AnswerInstances.Where(x => x.SurveyInstanceId == surveyInstanceId),x => x.ID, x => x.QuestionID, (x, y) => new { Count = y.Count()} ).Any(x => x.Count == 0);


            if (anyMissing)
            {
                return false;
            }

            pendingSurvey.DateFinished = DateTime.UtcNow;
            return true;
        }

        public SurveyType GetNextAvailableSurveyTypeForSurveyInCohort(int surveyId, int cohortId)
        {
            var lastSurvey = this.dbcontext.SurveysAvailables.Where(x => x.SurveyID == surveyId && x.CohortID == cohortId && x.IsDeleted == false).OrderByDescending(x => x.ID).FirstOrDefault();


            if(lastSurvey.IsCompleted == false)
            {
                return null;
            }


            var theType = this.dbcontext.SurveyTypes.Where(x => x.ID > lastSurvey.SurveyTypeId).OrderBy(x => x.ID).FirstOrDefault();

            if(theType == null)
            {
                theType = this.dbcontext.SurveyTypes.OrderBy(x => x.ID).FirstOrDefault();
            }

            return theType;
        }

        public bool CheckHaveAllSurveysBeenCompleted(int cohortId, int surveyAvailableToId)
        {
            var surveyAvailable = this.dbcontext.SurveysAvailables.Include(x => x.SurveysAvailableToes).FirstOrDefault(x => x.ID == surveyAvailableToId && x.CohortID == cohortId && x.IsDeleted == false);

            if (surveyAvailable.IsCompleted)
            {
                return true;
            }


            var requiredTypes = surveyAvailable.SurveysAvailableToes;

            var usersToTakeSurveys = this.dbcontext.CohortUsers.Where(x => x.CohortID == cohortId);

            var takenSurveys = this.dbcontext.PendingSurveys.Where(x => x.SurveyAvailToMeID == surveyAvailableToId && x.IsDeleted == false && x.SurveyInstance != null && x.SurveyInstance.DateFinished != null);

            var surveysForUsers = takenSurveys.GroupBy(x => x.UserSurveyForId);
            var surveysForTypes = surveysForUsers.Select(y => new { Key = y.Key, Counts = y.GroupBy(z => z.UserSurveyRoleID).Select(z => new { Key = z.Key, Count = z.Count() }) }).ToList();


            foreach(var user in usersToTakeSurveys)
            {
                var userID = user.UserID;

                var correspondingUser = surveysForTypes?.FirstOrDefault(x => x.Key == userID);

                if(correspondingUser == null)
                {
                    return false;
                }

                foreach(var type in requiredTypes)
                {
                    var id = type.UserSurveyRoleId;

                    var correspondingType = correspondingUser?.Counts?.FirstOrDefault(x => x.Key == id);

                    if(correspondingType == null)
                    {
                        return false;
                    }

                    if(correspondingType.Count != type.Quantity)
                    {
                        return false;
                    }
                }
            }

            surveyAvailable.IsCompleted = true;
            surveyAvailable.DateCompleted = DateTime.UtcNow;

            return true;
        }

        public ICollection<PendingSurvey> GetPendingSurveysOfRatersForUser(string userId, Guid pendingSurveyId)
        {
            var originalPendingSurvey = this.GetPendingSurvey(userId, pendingSurveyId);

            var theId = originalPendingSurvey.SurveyAvailToMeID;

            return this.dbcontext.PendingSurveys.Where(x => x.SurveyAvailToMeID == theId && x.IsDeleted == false && x.UserSurveyForId == userId).Include(x => x.SurveyInstance).Include(x => x.SurveysAvailable).Include(x => x.UserSurveyRole).ToList();
        }

        public ICollection<PendingSurvey> GetPendingSurveysOfRatersForUser(string userId, int SurveysAvailableToId)
        {
            return this.dbcontext.PendingSurveys.Where(x => x.SurveyAvailToMeID == SurveysAvailableToId && x.UserSurveyForId == userId && x.IsDeleted == false).Include(x => x.SurveyInstance).Include(x => x.SurveysAvailable).Include(x => x.UserSurveyRole).ToList();
        }

        public void TryRemovePendingSurveysSYSTEM(ICollection<PendingSurvey> surveys)
        {
            foreach(var survey in surveys)
            {
                this.dbcontext.PendingSurveys.Remove(survey);
            }
        }

        public void TryToAddPendingSurveysSYSTEM(ICollection<PendingSurvey> surveys)
        {
            foreach (var survey in surveys)
            {
                this.dbcontext.PendingSurveys.Add(survey);
            }
        }

        public ICollection<SurveysAvailable> GetAllOfferedSurveysForCohort(string currentUserID, int cohortId)
        {
            return this.dbcontext.SurveysAvailables.Where(x => x.CohortID == cohortId && x.IsDeleted == false).ToList();
        }

        public int CancelAllOldSurveyLocks()
        {
            return this.dbcontext.CancelAllOldSurveyLocks();
        }

        public SurveysAvailable GetPreviousSurveyForCohort(int cohortId, int currentAvailableId)
        {
            throw new NotImplementedException();
        }
    }
}
