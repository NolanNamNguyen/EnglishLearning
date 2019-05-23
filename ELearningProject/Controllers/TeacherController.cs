using ELearningProject.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELearningProject.Controllers
{
    public class TeacherController : Controller
    {
        private static List<List<int>> AddingQuestions = new List<List<int>>();
        private static List<int> AddingTeachers = new List<int>();
        public ActionResult Index()
        {
            var tivm = new TeacherIndexViewModel();
            tivm.OwnTests = new List<int>();
            using (var db = new ApplicationDbContext())
            {
                int teacherid = 0;
                try
                {
                    teacherid = int.Parse(Request.Cookies["TeacherId"].Value);
                } catch (NullReferenceException)
                {
                    return RedirectToAction("Index", "Home");
                }
                tivm.OwnTests = (from t in db.Tests
                                 where t.Creator.id == teacherid
                                 select t.id).ToList<int>();
            }
            tivm.stvm = GetStvm();
            return View(tivm);
        }

        public JsonResult SubmitTest(SingleQuestionAddViewModel model)
        {
            if (model.Submit)
            {
                //Submit the test to the database

                //Add the chosen questions to the quests list
                //List<Question> quests = new List<Question>();
                //foreach (var quest in AddingQuestions[AddingTeachers.IndexOf(model.TeacherId)])
                //{
                //    using (var db = new ApplicationDbContext())
                //    {
                //        var temp = (from q in db.Questions
                //                    join qc in db.QContents on q.Content.id equals qc.id
                //                    join a in db.Answers on q.Answer.id equals a.id
                //                    select q).ToList<Question>();
                //        quests.AddRange(temp);
                //    }
                //}

                using (var db = new ApplicationDbContext())
                {
                    //First we have to create a new test
                    Test t = new Test()
                    {
                        Rating = 0.0F,
                        Desc = model.TestName,
                        //Creator = (from tea in db.Teachers where tea.id == model.TeacherId select tea).First()
                        Image = model.Image,
                        Type = (from tt in db.TTypes
                                join qt in db.QTypes on tt.name.ToLower() equals qt.Name.ToLower()
                                where qt.id == model.TestType
                                select tt).FirstOrDefault<TType>(),
                        Tags = model.Tags,
                    };
                    db.Tests.Add(t);

                    //Shift throught all questions to be added and add them to TestQuestionDeploy
                    int count = 0;
                    foreach (var quest in AddingQuestions[AddingTeachers.IndexOf(model.TeacherId)])
                    {
                        Question question = (from q in db.Questions
                                             where q.id == quest
                                             select q).FirstOrDefault();
                        TestQuestionDeploy tqd = new TestQuestionDeploy()
                        {
                            Question = question,
                            Test = t,
                            Order = count++,
                        };
                        db.TestQuestionDeploys.Add(tqd);
                    }

                    //And save the changes
                    db.SaveChanges();

                    //And remove informations of this text
                    AddingQuestions.Remove(AddingQuestions[AddingTeachers.IndexOf(model.TeacherId)]);
                    AddingTeachers.Remove(model.TeacherId);
                }

                return Json("SucSub");
            }
            else
            {
                //Add the selected question to the "AddingQuestion" list and devided by teacher id, since this is a server side
                using (var db = new ApplicationDbContext())
                {
                    if (AddingTeachers.IndexOf(model.TeacherId) == -1)
                    {
                        AddingTeachers.Add(model.TeacherId);
                        int idx = AddingTeachers.IndexOf(model.TeacherId);
                        try
                        {
                            AddingQuestions[idx] = new List<int>();
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            AddingQuestions.Add(new List<int>());
                        }
                    }

                    List<int> qts = AddingQuestions[AddingTeachers.IndexOf(model.TeacherId)];
                    qts.Add(model.QuestionId);
                    AddingQuestions[AddingTeachers.IndexOf(model.TeacherId)] = qts;
                }
                return Json("SucAdd");
            }
        }
        //Create Test Section

        public ActionResult Test1()
        {
            return RedirectToAction("TestAjaxForm");
        }
        public JsonResult TestAjaxForm()
        {
            string Myname = "Nguyen Nam";

            return Json(Myname, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RealCreateTest()
        {
            return View();
        }

        //đây là action đc gọi khi người dùng click create button trong form tạo câu hỏi kiểu Quiz
        [HttpPost]
        public PartialViewResult CreateQuizQues(MultipieChoiceViewModel TheQuiz)
        {

            int TeacherID = int.Parse(Request.Cookies["TeacherID"].Value);
            ApplicationDbContext db = new ApplicationDbContext();
            string myAnswerContent;
            if (TheQuiz.Content != null && TheQuiz.Quiz.Quiz1 != null && TheQuiz.Quiz.Quiz2 != null && TheQuiz.Quiz.Quiz3 != null && TheQuiz.Quiz.Quiz4 != null)
            {
                try
                {
                    try
                    {
                        QContent qcontent = new QContent();
                        Answer qAnswer = new Answer();
                        qcontent.Content = TheQuiz.Content;
                        switch (TheQuiz.EAnswer)
                        {
                            case Answer_input.A:
                                TheQuiz.Quiz.Answer = 1;
                                break;
                            case Answer_input.B:
                                TheQuiz.Quiz.Answer = 2;
                                break;
                            case Answer_input.C:
                                TheQuiz.Quiz.Answer = 3;
                                break;
                            case Answer_input.D:
                                TheQuiz.Quiz.Answer = 4;
                                break;
                        }
                        qAnswer.Content = JsonConvert.SerializeObject(TheQuiz.Quiz);
                        myAnswerContent = JsonConvert.SerializeObject(TheQuiz.Quiz);
                        db.Answers.Add(qAnswer);
                        db.QContents.Add(qcontent);
                        db.SaveChanges();
                    }
                    catch
                    {
                        return PartialView("_CreateQuesFailed");
                    }
                    Question QuizQues = new Question
                    {
                        Level = 2,
                        Type = (from t in db.QTypes where t.id == 2 select t).First(),
                        Content = (from ct in db.QContents where ct.Content == TheQuiz.Content select ct).First(),
                        Answer = (from a in db.Answers where a.Content == myAnswerContent select a).First(),
                        Creator = (from tea in db.Teachers where tea.id == TeacherID select tea).First()
                    };
                    db.Questions.Add(QuizQues);
                    db.SaveChanges();
                    //đưa object về partial view để JS lấy đc id của object, từ đó gọi ajax về 1 action khác
                    return PartialView("_QuizCreateSuccess", QuizQues);
                    //đưa object về partial view để JS lấy đc id của object, từ đó gọi ajax về 1 action khác

                }
                catch
                {
                    return PartialView("_CreateQuesFailed");
                }
            }
            else
            {
                return PartialView("_CreateQuesFailed");
            }

        }
        //đây là action đc gọi khi người dùng click create button trong form tạo câu hỏi kiểu Quiz


        //Action sẽ chạy khi người dùng click vào tạo kiểu quiz question, nó sẽ trả về 1 cái form với object rỗng để người dùng nhập thông tin
        [HttpPost]
        public PartialViewResult SendFormQuizQues()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MultipieChoiceViewModel QuizQues = new MultipieChoiceViewModel();

            return PartialView("_QuizTypeCreate", QuizQues);
        }
        //Action sẽ chạy khi người dùng click vào tạo kiểu quiz question, nó sẽ trả về 1 cái form với object rỗng để người dùng nhập thông tin


        //đây là action xác định loại câu hỏi tương ứng để trả về phần view test create của người dùng
        //mỗi loại câu hỏi sẽ render ra 1 partialview khác nhau
        [HttpGet]
        public ActionResult RedirectQues(string QuesId)
        {
            int myQuesId = int.Parse(QuesId);
            ApplicationDbContext db = new ApplicationDbContext();
            Question Ques = (from q in db.Questions where q.id == myQuesId select q).First();
            QType QuesT = (from q in db.Questions
                           join qt in db.QTypes on q.Type.id equals qt.id
                           where q.id == myQuesId
                           select qt).First();
            switch (QuesT.id)
            {
                case 1:
                    return RedirectToAction("ReturnPuzzle", "Teacher", Ques);
                case 2:
                    return ReturnQuiz(Ques);
                case 3:
                    return RedirectToAction("ReturnTranslate", "Teacher", Ques);
            }
            return RedirectToAction("RedirectFailed", "Teacher");
        }
        //mỗi loại câu hỏi sẽ render ra 1 partialview khác nhau
        //đây là action xác định loại câu hỏi tương ứng để trả về phần view test create của người dùng

        public PartialViewResult RedirectFailed()
        {

            return PartialView("_RedirectFailed");
        }

        //đây là action trả về khi loại câu hỏi là Quiz (TypeID =2) 
        public PartialViewResult ReturnQuiz(Question Ques)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MultipieChoiceViewModel MulQues = (from q in db.Questions
                                               join qc in db.QContents on q.Content.id equals qc.id
                                               join qt in db.QTypes on q.Type.id equals qt.id
                                               join qa in db.Answers on q.Answer.id equals qa.id
                                               where q.id == Ques.id
                                               select new MultipieChoiceViewModel()
                                               {
                                                   id = q.id,
                                                   Content = qc.Content,
                                                   data = qa.Content,
                                               }).First();
            MulQues.Quiz = JsonConvert.DeserializeObject<QuizMultichoice>(MulQues.data);
            return PartialView("_QuizType", MulQues);
        }
        //đây là action trả về khi loại câu hỏi là Quiz (TypeID =2) 

        //[HttpPost]
        //public PartialViewResult CreateTestF(string )
        //{

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Testing(string someValue)
        {
            string Mystring = "Hello";
            return Json(someValue);
        }

        //Create Test Section

        public JsonResult FilterQuestions(int? TestType)
        {
            List<QuestionViewModel> questions = new List<QuestionViewModel>();
            if (TestType != null)
            {
                questions = GetQuestionsByType(TestType);
            }
            return Json(questions);
        }
        public List<QuestionViewModel> GetQuestionsByType(int? TypeId)
        {
            List<QuestionViewModel> questions = new List<QuestionViewModel>();
            using (var db = new ApplicationDbContext())
            {
                switch (TypeId)
                {
                    case 1:
                        {
                            questions.AddRange((from q in db.Questions
                                                join qc in db.QContents on q.Content.id equals qc.id
                                                join a in db.Answers on q.Answer.id equals a.id
                                                join qt in db.QTypes on q.Type.id equals qt.id
                                                where qt.id == TypeId
                                                select new PuzzelQuestionViewModel()
                                                {
                                                    id = q.id,
                                                    Content = qc.Content,
                                                    Answer = a.Content
                                                })
                            .ToList<PuzzelQuestionViewModel>());
                            break;
                        }
                    case 2:
                        {
                            questions.AddRange((from q in db.Questions
                                                join qc in db.QContents on q.Content.id equals qc.id
                                                join a in db.Answers on q.Answer.id equals a.id
                                                join qt in db.QTypes on q.Type.id equals qt.id
                                                where qt.id == TypeId
                                                select new MultipieChoiceViewModel()
                                                {
                                                    id = q.id,
                                                    Content = qc.Content,
                                                    data = a.Content
                                                })
                             .ToList<MultipieChoiceViewModel>());
                            break;
                        }
                    case 3:
                        {
                            questions.AddRange((from q in db.Questions
                                                join qc in db.QContents on q.Content.id equals qc.id
                                                join a in db.Answers on q.Answer.id equals a.id
                                                join qt in db.QTypes on q.Type.id equals qt.id
                                                where qt.id == TypeId
                                                select new TranslateQuestViewModel()
                                                {
                                                    id = q.id,
                                                    Content = qc.Content,
                                                    Answer = JsonConvert.DeserializeObject<List<string>>(a.Content)
                                                })
                             .ToList<TranslateQuestViewModel>());
                            break;
                        }
                }
            }
            return questions;
        }

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

        public ActionResult IndexTeacher()
        {
            return View(GetTuvm());
        }

        private TeacherUserViewModel GetTuvm()
        {
            //Get a list of tests and add it to viewbag since we use many object here

            List<TeacherUserModel> teachers = new List<TeacherUserModel>();
            using (var db = new ApplicationDbContext())
            {
                teachers = (from st in db.Teachers
                            join u in db.Web_Users on st.User.id equals u.id
                            select new TeacherUserModel() { id = st.id, name = u.Name, birthday = u.Birthday, creditid = u.CreditId/*, status = u.status*/ }).ToList<TeacherUserModel>();
            }
            var suvm = new TeacherUserViewModel();
            foreach (var t in teachers)
            {
                suvm.users.Add(t);
            }



            return suvm;

        }

        public JsonResult GetTestResult(int testid)
        {
            var testlist = new List<StudentTestResultViewModel>();
            using (var db = new ApplicationDbContext())
            {
                StudentTestResultViewModel test_result = new StudentTestResultViewModel();
                try
                {
                    test_result = (from test in db.Tests
                                   join str in db.StudentTestResults on test.id equals str.Test.id
                                   join student in db.Students on str.Student.id equals student.id
                                   join wu in db.Web_Users on student.web_User.id equals wu.id
                                   where test.id == testid
                                   select new StudentTestResultViewModel()
                                   {
                                       Student = wu,
                                       Score = str.Score
                                   }).First();

                    testlist.Add(test_result);
                } catch (InvalidOperationException) { }
            }
            return Json(testlist);
        }
        //đây là action trả về khi loại câu hỏi là Quiz (TypeID =2) 

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public PartialViewResult CreateMixTest(CreateMixTestViewModel model, ICollection<string> ListQuesId)
        {
            try
            {
                ApplicationDbContext db = new ApplicationDbContext();
                int TeacherId = int.Parse(Request.Cookies["TeacherID"].Value);
                var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", "jpeg"
                };
                try
                {
                    Test myTest = new Test();
                    myTest.Title = model.Test_title;
                    myTest.Desc = model.Test_desc;
                    myTest.Creator = (from t in db.Teachers where t.id == TeacherId select t).First();
                    myTest.Type = (from tt in db.TTypes where tt.id == 1 select tt).First();
                    if (model.File != null)
                    {
                        var filename = Path.GetFileName(model.File.FileName);
                        var extension = Path.GetExtension(model.File.FileName);
                        if (allowedExtensions.Contains(extension))
                        {
                            string name = Path.GetFileNameWithoutExtension(filename);
                            string myTestImage = name + "_" + TeacherId + extension;
                            var savePath = Path.Combine(Server.MapPath("~/Content/TestImage"), myTestImage);
                            var imagePath = Path.Combine("/Content/TestImage/", myTestImage);
                            myTest.Image = imagePath;
                            model.File.SaveAs(savePath);
                            db.Tests.Add(myTest);
                            db.SaveChanges();
                        }
                        else
                        {
                            return PartialView("_WrongFileType");
                        }
                    }
                    else
                    {
                        db.Tests.Add(myTest);
                        db.SaveChanges();
                    }
                    foreach (string id in ListQuesId)
                    {
                        int qid = int.Parse(id);
                        TestQuestionDeploy TQdeploy = new TestQuestionDeploy();
                        TQdeploy.Test = (from t in db.Tests where t.id == myTest.id select t).First();
                        TQdeploy.Question = (from q in db.Questions where q.id == qid select q).First();
                        db.TestQuestionDeploys.Add(TQdeploy);
                    }
                    db.SaveChanges();
                    return PartialView("_CreateTestSuccessful");
                }
                catch
                {
                    return PartialView("_CreateTestFailed");
                }
            }
            catch
            {
                return PartialView("_CreateTesttooFailed");

            }
        }
    }
}