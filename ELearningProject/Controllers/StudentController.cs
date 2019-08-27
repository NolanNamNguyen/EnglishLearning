using ELearningProject.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdentityAuthentication.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            return View(GetStvm());
        }

        public ActionResult NewsIndex()
        {
            return View(GetNews());
        }

      

        [HttpGet]
        public ActionResult TestRouting(int TestId)
        {
            int route = 0;
            using (var db = new ApplicationDbContext())
            {
                route = (from t in db.Tests
                         join tt in db.TTypes on t.Type.id equals tt.id
                         where t.id == TestId
                         select tt.id).First();
            }

            switch (route)
            {
                case 1:
                    {
                        return PuzzelEnglish(TestId);
                    }
                case 2:
                    {
                        return MultipieChoice(TestId);
                    }
                case 3:
                    {
                        return Translating(TestId);
                    }
                default:
                    {
                        return RedirectToAction("TestRouting");    
                    }
            }
        }

        [HttpGet]
        public ActionResult PuzzelEnglish(int? TestId)
        {
            //If the student intent to see all the tests, not a particular one
            if (TestId == null)
            {
                return View(GetTTVM("Puzzel Game"));
            }

            //Create pquests list which contains all the question that is in the test, since the test is divided by type
            //so we dont have to check the QType right now
            List<PuzzelQuestionViewModel> pquests = new List<PuzzelQuestionViewModel>();
            var ptvm = new PuzzelTestViewModel();
            ptvm.StudentId = int.Parse(Request.Cookies["StudentID"].Value);
            ptvm.TestId = TestId.GetValueOrDefault();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                pquests = (from q in db.Questions
                           join qt in db.TestQuestionDeploys on q.id equals qt.Question.id
                           join t in db.Tests on qt.Test.id equals t.id
                           join qc in db.QContents on q.Content.id equals qc.id
                           join a in db.Answers on q.Answer.id equals a.id
                           where t.id == TestId
                           select new PuzzelQuestionViewModel() { id = q.id, Content = qc.Content, Answer = a.Content })
                              .ToList<PuzzelQuestionViewModel>();
            }
            ptvm.Questions = pquests;
            //And return them to the list
            return View("NewPuzzleEnglishTest", ptvm);
        }

        [HttpGet]
        public ActionResult MultipieChoice(int? TestId)
        {
            if (TestId == null)
            {
                return View(GetTTVM("Multipie Choice"));
            }
            else
            {
                //Create the mcquests list that contains all the questions in this test
                List<MultipieChoiceViewModel> mcquests = new List<MultipieChoiceViewModel>();
                using (var db = new ApplicationDbContext())
                {
                    mcquests = (from q in db.Questions
                                join qt in db.TestQuestionDeploys on q.id equals qt.Question.id
                                join t in db.Tests on qt.Test.id equals t.id
                                join qc in db.QContents on q.Content.id equals qc.id
                                join a in db.Answers on q.Answer.id equals a.id
                                where t.id == TestId
                                select new MultipieChoiceViewModel()
                                {
                                    id = t.id,
                                    Content = qc.Content,
                                    Quiz = new QuizMultichoice(),
                                    data = a.Content,
                                    QuestionId = q.id
                                }).ToList<MultipieChoiceViewModel>();
                    foreach (var t in mcquests)
                    {
                        t.Quiz = JsonConvert.DeserializeObject<QuizMultichoice>(t.data);
                    }
                }
                //And return them to the controller as usual
                return View("MultipieChoiceTest", mcquests);
            }
        }

        [HttpGet]
        public ActionResult Translating(int? TestId)
        {
            if (TestId == null)
            {
                return View(GetTTVM("Translating"));
            }
            else
            {
                //Create a trsQuest list, you should have known what i ment by thi
                List<TranslateQuestViewModel> trsQuests = new List<TranslateQuestViewModel>();
                using (var db = new ApplicationDbContext())
                {
                    trsQuests = (from q in db.Questions
                                 join qt in db.TestQuestionDeploys on q.id equals qt.Question.id
                                 join t in db.Tests on qt.Test.id equals t.id
                                 join qc in db.QContents on q.Content.id equals qc.id
                                 join a in db.Answers on q.Answer.id equals a.id
                                 where t.id == TestId
                                 select new TranslateQuestViewModel()
                                 {
                                     id = t.id,
                                     Content = qc.Content,
                                     Answer = JsonConvert.DeserializeObject<List<string>>(a.Content)
                                 }).ToList<TranslateQuestViewModel>();
                }
                return View("TranslatingQuestTest", trsQuests);
            }
        }

        //Action for adding the score
        public JsonResult SubmitScore(StudentScore result)
        {
            using (var db = new ApplicationDbContext())
            {
                //First get the test we are taking
                var test = (from t in db.Tests
                            where t.id == result.TestId
                            select t).FirstOrDefault();
                //Then get the student taking the test
                var student = (from s in db.Students
                               where s.id == result.StudentId
                               select s).FirstOrDefault();
                //Get the question count of this particular test
                var count = (from t in db.Tests
                             join tqd in db.TestQuestionDeploys on t.id equals tqd.Test.id
                             where t.id == result.TestId
                             select tqd).Count();
                //Then create the StudentTestResult object with these value, and save it
                var TestResult = new StudentTestResult()
                {
                    Test = test,
                    Student = student,
                    Score = result.Score
                };

                db.StudentTestResults.Add(TestResult);
                db.SaveChanges();

                //Then we will calculate the mean score for this particular test, to see how the students doing
                List<float> scores = (from str in db.StudentTestResults
                                      where str.Test.id == result.TestId
                                      select str.Score).ToList<float>();
                float avg = scores.Average();
                test.Rating = avg;
                db.SaveChanges();
            }
            return Json("Score submitted");
        }

        //Get the Student Test View contains all the test with types
        private StudentTestViewModel GetStvm()
        {
            //Get a list of tests and add it to viewbag since we use many object here
            List<TestTypeView> ttests = new List<TestTypeView>();
            using (var db = new ApplicationDbContext())
            {
                ttests = (from t in db.Tests
                          join tt in db.TTypes on t.Type.id equals tt.id
                          select new TestTypeView() { test = t, type = tt }).ToList<TestTypeView>();
            }

            //Create new list with our tests
            List<Test> tests = new List<Test>();
            foreach (var ttest in ttests)
            {
                var test = new Test()
                {
                    id = ttest.test.id,
                    Desc = ttest.test.Desc,
                    Image = ttest.test.Image,
                    Rating = ttest.test.Rating,
                    Tags = ttest.test.Tags,
                    Type = ttest.type
                };
                tests.Add(test);
            }

            //Shift through the list and cast them to TestViewModels, then add to the array divided by type
            var stvm = new StudentTestViewModel();
            foreach (var test in tests)
            {
                //Create a list of tags for each test
                List<string> tags = JsonConvert.DeserializeObject<List<string>>(test.Tags);
                var t = new TestViewModel()
                {
                    id = test.id,
                    Desc = test.Desc,
                    Image = test.Image,
                    Rating = test.Rating,
                    Tags = tags
                };

                //Check if this type is in the list or not, and add it to
                if (stvm.TestTypes.IndexOf(test.Type.name) == -1)
                {
                    stvm.TestTypes.Add(test.Type.name);
                    stvm.Tests.Add(new List<TestViewModel>());
                }

                stvm.Tests[stvm.TestTypes.IndexOf(test.Type.name)].Add(t);
            }
            return stvm;
        }

        private TestTagViewModel GetTTVM(string name)
        {
            //Create an instance of TestTagViewModel and stvm
            var ttvm = new TestTagViewModel();
            var stvm = GetStvm();

            //Get desired tests and add them to the ttvm (this will be edited later for better performance)
            var tests = stvm.Tests[stvm.TestTypes.IndexOf(name)];
            foreach (var test in tests)
            {
                ttvm.Tests.Add(test);
                //Get the tags and add to the tags list, and add the testid to the testids list
                List<string> tags = test.Tags;
                foreach (var tag in tags)
                {
                    //Check if the tag is already saved or not
                    if (ttvm.Tags.IndexOf(tag) == -1)
                    {
                        ttvm.Tags.Add(tag);
                        ttvm.TestIds.Add(new List<int>());
                    }

                    //Add the testid to the testids list, the reason i dont add the whole text because thay will duplicate,
                    //since one test have a lot of tags,
                    //which draw a lot of unnecessary memory
                    ttvm.TestIds[ttvm.Tags.IndexOf(tag)].Add(test.id);
                }
            }
            return ttvm;
        }

        [HttpPost]
        public JsonResult ShowStuTest(int StuId)
        {
            List<StudentScoreViewModel> ResultList = new List<StudentScoreViewModel>();
            using (var db = new ApplicationDbContext())
            {
                ResultList = (from s in db.Students
                              join tr in db.StudentTestResults on s.id equals tr.Student.id
                              join t in db.Tests on tr.Test.id equals t.id
                              where s.id == StuId
                              select new StudentScoreViewModel()
                              {
                                  student = s,
                                  test = t,
                                  Score = tr.Score
                              }).ToList<StudentScoreViewModel>();
            }
            return Json(ResultList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNews()
        {
            News news = new News();
            using (var db = new ApplicationDbContext())
            {
                news = (from n in db.News
                        where n.Id == 1
                        select n).FirstOrDefault();
            }
            return View("NewsIndex", news);
        }

        public ActionResult IndexStudent()
        {
            return View(GetSuvm());
        }

        private StudentUserViewModel GetSuvm()
        {
            //Get a list of tests and add it to viewbag since we use many object here

            List<StudentUserModel> students = new List<StudentUserModel>();
            using (var db = new ApplicationDbContext())
            {
                students = (from st in db.Students
                            join u in db.Web_Users on st.web_User.id equals u.id
                            select new StudentUserModel() { id = st.id, name = u.Name, birthday = u.Birthday, creditid = u.CreditId/*, status = u.status*/ }).ToList<StudentUserModel>();
            }
            var suvm = new StudentUserViewModel();
            foreach (var t in students)
            {
                suvm.users.Add(t);
            }



            return suvm;
        }

        [HttpGet]
        public JsonResult SendObjectPuzzleTest(string TestId)
        {
            int T_Id = int.Parse(TestId);
            List<PuzzelQuestionViewModel> pquests = new List<PuzzelQuestionViewModel>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                pquests = (from q in db.Questions
                           join qt in db.TestQuestionDeploys on q.id equals qt.Question.id
                           join t in db.Tests on qt.Test.id equals t.id
                           join qc in db.QContents on q.Content.id equals qc.id
                           join a in db.Answers on q.Answer.id equals a.id
                           where t.id == T_Id
                           select new PuzzelQuestionViewModel() { id = q.id, Content = qc.Content, Answer = a.Content })
                              .ToList<PuzzelQuestionViewModel>();
            }
            return Json(pquests, JsonRequestBehavior.AllowGet);
        }
    }
}