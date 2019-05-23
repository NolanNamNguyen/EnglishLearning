using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ELearningProject.Models
{
    public class Test
    {
        public int id { get; set; }
        public string Title { get; set; }
        public Teacher Creator { get; set; }
        public float Rating { get; set; }
        public string Desc { get; set; }
        public string Tags { get; set; }
        public string Image { get; set; }
        public TType Type { get; set; }

        public ICollection<TestQuestionDeploy> TestQuestionDeploys { get; set; }
    }

    public class TestViewModel
    {
        public int id { get; set; }
        public float Rating { get; set; }
        public string Desc { get; set; }
        public List<String> Tags { get; set; }
        public string Image { get; set; }
    }

    public class TestTypeView
    {
        public Test test { get; set; }
        public TType type { get; set; }
    }

    public class TestTagViewModel
    {
        public List<string> Tags { get; set; }
        public List<List<int>> TestIds { get; set; }
        public List<TestViewModel> Tests { get; set; }

        public TestTagViewModel()
        {
            Tags = new List<string>();
            TestIds = new List<List<int>>();
            Tests = new List<TestViewModel>();
        }
    }

    public class TeacherTestViewModel
    {

    }

    public class StudentTestViewModel
    {
        public List<string> TestTypes { get; set; }
        public List<List<TestViewModel>> Tests { get; set; }

        public StudentTestViewModel()
        {
            TestTypes = new List<string>();
            Tests = new List<List<TestViewModel>>();
        }
    }

    public class StudentTestResultViewModel
    {
        public Test Test  { get; set; }
        public float Score { get; set; }
        public Web_user Student { get; set; }
        public StudentTestResultViewModel()
        {

        }
    }

    public class CreateMixTestViewModel
    {
        public HttpPostedFileBase File { get; set; }
        public string Test_title { get; set; }
        public string Test_desc { get; set; }
        public CreateMixTestViewModel()
        {

        }
    }
}