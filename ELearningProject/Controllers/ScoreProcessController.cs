using ELearningProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELearningProject.Controllers
{
    public class ScoreProcessController : Controller
    {
        // GET: ScoreProcess
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult StudentResult(StudentTestResult result)
        {

            return Json("Ok");
        }
    }
}