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
            string semester = null,
            int courseInstanceId = 0
            ) {

            var teacherId = _userService.GetUserIdByName(User.Identity.Name);
            var userCourses = _courseService.GetCoursesByUserId(teacherId);
            var TeacherActiveSemesters = new List<Tuple<int, string>>();

            foreach (var course in userCourses)
            {
                var YearAndSemester = new Tuple<int, string>(course.Year, course.SemesterName);
                if (!TeacherActiveSemesters.Contains(YearAndSemester))
                    TeacherActiveSemesters.Add(YearAndSemester);
            }

            TeacherActiveSemesters.Sort();
            TeacherActiveSemesters.Reverse();
            if (year != 0 && semester != null)
            {

                var selected = TeacherActiveSemesters.Find(delegate (Tuple<int, string> find)
                {
                    return find.Item1 == year && find.Item2 == semester;
                });

                TeacherActiveSemesters.RemoveAt(TeacherActiveSemesters.IndexOf(selected));
                TeacherActiveSemesters.Insert(0, selected);

            }

            var teacherCourses = _courseService.GetTeacherCoursesByDate(User.Identity.Name, TeacherActiveSemesters.First().Item1, TeacherActiveSemesters.First().Item2);

            var model = new TeacherViewModel
            {
                YearSelected = year,
                SemesterSelected = semester,
                TeacherActiveSemesters = TeacherActiveSemesters,
                TeacherCourses = teacherCourses
            };

            return View(model);

        }

        /// <summary>
        /// 
        /// </summary>
        public ActionResult GetTeacherCoursesByDate(int year, string semester)
        {

            var teacherCourses = _courseService.GetTeacherCoursesByDate(
                User.Identity.Name,
                year,
                semester
                );

            if (Request.IsAjaxRequest()) {

                return Json(teacherCourses);

            }
            else
            {

                return View(teacherCourses);

            }

        }

    }
}