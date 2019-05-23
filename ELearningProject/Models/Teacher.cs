using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearningProject.Models
{
    public class Teacher
    {
        public int id { get; set; }
        public Web_user User { get; set; }
        public float MeanRating { get; set; }
    }

    public class TeacherIndexViewModel
    {
        public StudentTestViewModel stvm { get; set; }
        public List<int> OwnTests { get; set; }
    }

    public class CreateTestViewModel
    {
        public int TeacherId { get; set; }
        public List<QType> Types { get; set; }
        public List<PuzzelQuestionViewModel> PuzzQuestions { get; set; }
        public List<MultipieChoiceViewModel> MultQuestions { get; set; }

        public CreateTestViewModel()
        {
        }

        public CreateTestViewModel(List<PuzzelQuestionViewModel> puzzelQuestions, 
            List<MultipieChoiceViewModel> multipieChoices, List<QType> Types, int TeacherId)
        {
            this.PuzzQuestions = puzzelQuestions;
            this.MultQuestions = multipieChoices;
            this.Types = Types;
            this.TeacherId = TeacherId;
        }
    }

    public class SingleQuestionAddViewModel
    {
        public int TeacherId { get; set; }
        public int QuestionId { get; set; }
        public bool Submit { get; set; }
        public string TestName { get; set; }
        public int Order { get; set; }
        public int TestType { get; set; }
        public string Image { get; set; }
        public string Tags { get; set; }
    }

    public class TeacherUserModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public DateTime birthday { get; set; }
        public string creditid { get; set; }
        public bool status { get; set; }


    }

    public class TeacherUserViewModel
    {

        public List<TeacherUserModel> users { get; set; }

        public TeacherUserViewModel()
        {

            users = new List<TeacherUserModel>();
        }
    }
}