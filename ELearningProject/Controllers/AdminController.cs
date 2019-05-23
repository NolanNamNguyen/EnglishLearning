using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELearningProject.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        public ActionResult AdminIndex()
        {
            ViewBag.Online = "";
            try
            {
                ViewBag.Online = HttpContext.Application["Online"].ToString(); //Số người online
            }
            catch (NullReferenceException)
            {
                ViewBag.Online = "1";
            }

            ViewBag.PageView = "";
            try
            {
                ViewBag.PageView = HttpContext.Application["PageView"].ToString(); //Số lượng người truy cập
            }
            catch (NullReferenceException)
            {
                ViewBag.PageView = "1";
            }
            return View();
        }
    }
}