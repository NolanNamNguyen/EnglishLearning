using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELearningProject.Models
{
    public class Student
    {
        public int id { get; set; }
        public int Skill_listening { get; set; }
        public int Skill_speaking { get; set; }
        public int Skill_reading { get; set; }
        public int Skill_writing { get; set; }
        public int Mean_skills { get; set; }

        public Web_user web_User { get; set; }
    }

    public class StudentUserModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public DateTime birthday { get; set; }
        public string creditid { get; set; }
        public bool status { get; set; }


    }


    public class StudentUserViewModel
    {

        public List<StudentUserModel> users { get; set; }

        public StudentUserViewModel()
        {

            users = new List<StudentUserModel>();
        }
    }
}