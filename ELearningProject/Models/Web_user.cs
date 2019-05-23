using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearningProject.Models
{
    public class Web_user
    {
        public int id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string CreditId { get; set; }
        public string UserID { get; set; }
        public string UserImage { get; set; }
        public bool Locked { get; set; }
    }
}