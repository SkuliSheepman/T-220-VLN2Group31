using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Services;
using Codex.Models;
using Codex.Models.SharedModels.SharedViewModels;

namespace Codex.Controllers
{
    public class TeacherController : Controller
    {
        private UserService _userService;
        private SubmissionService _submissionService;
        private AssignmentService _assignmentService;

        public TeacherController() {
            _userService = new UserService();
            _submissionService = new SubmissionService();
            _assignmentService = new AssignmentService();
        }

        // GET: Teacher
        public ActionResult Index() {
            return View();
        }
    }
}