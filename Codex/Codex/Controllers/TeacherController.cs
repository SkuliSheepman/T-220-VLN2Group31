using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Services;
using Codex.Models;
using Codex.Models.SharedModels.SharedViewModels;
using Codex.Models.TeacherModels.ViewModels;
using Codex.Models.TeacherModels.HelperModels;

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
        public ActionResult Index(
            int year = 0,
            int semesterId = 0,
            int courseInstanceId = 0
            ) {

            var teacherId = _userService.GetUserIdByName(User.Identity.Name);
            var userCourses = _courseService.GetCoursesByUserId(teacherId);
            var UserYearsActive = new List<int>();

            foreach (var course in userCourses)
            {
                if (!UserYearsActive.Contains(course.Year))
                    UserYearsActive.Add(course.Year);
            }

            UserYearsActive.OrderByDescending(x => x);

            var model = new TeacherViewModel
            {
                TeacherYearsActive = UserYearsActive
            };

            return View(model);

        }

        public ActionResult GetTeacherCoursesByDate()
        {
            var teacherId = _userService.GetUserIdByName(User.Identity.Name);
            var userCourses = _courseService.GetCoursesByUserId(teacherId);
            var teacherCourses = new List<CourseHelperModel>();

            return Json(false);
        }

    }
}