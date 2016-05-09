using Codex.Models.SharedModels.SharedViewModels;
using Codex.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Codex.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() {
            var assService = new AssignmentService();
            var userService = new UserService();
            var model = new HomeStudentViewModel();
            String studentId = userService.GetUserIdByName(User.Identity.Name);
            model.Assignments = assService.GetStudentAssignmentsByStudentId(studentId);
            ViewBag.UserName = User.Identity.Name;
            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";
            
            return View();
        }
    }
}