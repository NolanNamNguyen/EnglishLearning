using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearningProject.Models
{
    public class StudentTestResult
    {
        public int id { get; set; }
        public Test Test { get; set; }
        public Student Student { get; set; }
        public float Score { get; set; }

        public StudentTestResult()
        {

        }
    }

    public class StudentScore
    {
        public int TestId { get; set; }
        public int StudentId { get; set; }
        public float Score { get; set; }
    }

    public class StudentScoreViewModel
    {
        public Student student { get; set; }
        public Test test { get; set; }
        public float Score { get; set; }
    }
}