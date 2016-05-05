using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Models;

namespace Codex.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Users()
        {
            UserViewModel model = new UserViewModel();
            return View(model);
        }

        public ActionResult Courses()
        {
            CourseViewModel model = new CourseViewModel();
            return View(model);
        }

        public ActionResult CreateUser(NewUserViewModel newUser) {
            var testJson = new { cakeName = newUser.Name, cakeEmail = newUser.Email };
            return Json(testJson);
        }

        public ActionResult CreateCourse(NewCourseViewModel newCourse) {
            return Json(newCourse);
        }
    }
}