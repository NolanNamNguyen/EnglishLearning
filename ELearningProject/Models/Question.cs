using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELearningProject.Models
{
    public class Question
    {
        public int id { get; set; }
        public string Name { get; set; }
        public float Level { get; set; }

        public QType Type { get; set; }
        public QContent Content { get; set; }
        public Answer Answer { get; set; }
        public Teacher Creator { get; set; }

        public ICollection<TestQuestionDeploy> TestQuestionDeploys { get; set; }
    }

    public interface QuestionViewModel { }

    public class PuzzelQuestionViewModel : QuestionViewModel
    {
        public int id { get; set; }
        public string Content { get; set; }
        public string Answer { get; set; }

        public PuzzelQuestionViewModel()
        { }

        public PuzzelQuestionViewModel(Question question)
        {
            this.id = question.id;
            this.Content = question.Content.Content;
            this.Answer = question.Answer.Content;
        }
    }

    public class PuzzelTestViewModel
    {
        public int StudentId { get; set; }
        public int TestId { get; set; }
        public List<PuzzelQuestionViewModel> Questions { get; set; }
    }

    public class MultipieChoiceViewModel : QuestionViewModel
    {
        public int id { get; set; }
        [Required]
        public string Content { get; set; }
        public QuizMultichoice Quiz { get; set; }
        public string data { get; set; }
        public int QuestionId { get; set; }
        public Answer_input EAnswer { get; set; }

        public MultipieChoiceViewModel() { }
    }
    public enum Answer_input
    {
        A,
        B,
        C,
        D
    }
    public class QuizMultichoice
    {
        [Required]
        public string Quiz1 { get; set; }
        [Required]
        public string Quiz2 { get; set; }
        [Required]
        public string Quiz3 { get; set; }
        [Required]
        public string Quiz4 { get; set; }
        public int Answer { get; set; }
        public QuizMultichoice()
        {

        }
    }

    public class TranslateQuestViewModel : QuestionViewModel
    {
        public int id { get; set; }
        public string Content { get; set; }
        public List<string> Answer { get; set; }

        public TranslateQuestViewModel() { }
    }
}