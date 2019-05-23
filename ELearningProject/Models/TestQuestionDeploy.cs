using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ELearningProject.Models
{
    public class TestQuestionDeploy
    {
        public int id { get; set; }
        public Test Test { get; set; }
        public Question Question { get; set; }
        public int Order { get; set; }
    }
}