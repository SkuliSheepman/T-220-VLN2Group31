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

    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {

        private readonly UserService _userService;
        private readonly SubmissionService _submissionService;
        private readonly AssignmentService _assignmentService;
        private readonly CourseService _courseService;

        public TeacherController() {
            _userService = new UserService();
            _submissionService = new SubmissionService();
            _assignmentService = new AssignmentService();
            _courseService = new CourseService();
        }

        // GET: Teacher
        public ActionResult Index() {

            var teacherId = _userService.GetUserIdByName(User.Identity.Name);


            return View();

        }
    }
}