using Codex.Models.SharedModels.SharedViewModels;
using Codex.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Services;
using Codex.Models.SharedModels.SharedViewModels;

namespace Codex.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() {
            AssignmentService assService = new AssignmentService();
            UserService userService = new UserService();
            StudentViewModel model = new StudentViewModel();
            String studentId = userService.GetUserIdByName(User.Identity.Name);
            model.Assignments = assService.GetAssignmentsByStudentId(studentId);
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