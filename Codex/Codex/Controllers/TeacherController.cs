using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Services;
using Codex.Models;
using Codex.Models.TeacherViewModels;
namespace Codex.Controllers
{

    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {

        private readonly UserService _userService;
        private readonly SubmissionService _submissionService;
        private readonly AssignmentService _assignmentService;
        private readonly CourseService _courseService;
        private readonly TeacherService _teacherService;

        public TeacherController() {
            _userService = new UserService();
            _submissionService = new SubmissionService();
            _assignmentService = new AssignmentService();
            _courseService = new CourseService();
            _teacherService = new TeacherService();
        }

        // GET: Teacher
        public ActionResult Index(
            int year = 0,
            string semester = null,
            int courseInstanceId = 0
            ) {

            var teacherId = _userService.GetUserIdByName(User.Identity.Name);
            var teacherActiveSemesters = _teacherService.GetTeacherActiveSemestersById(teacherId);

            if (year != 0 && semester != null)
            {

                var selected = teacherActiveSemesters.Find(delegate (ActiveSemesterViewModel find)
                {
                    return find.Year == year && find.Semester == semester;
                });

                teacherActiveSemesters.RemoveAt(teacherActiveSemesters.IndexOf(selected));
                teacherActiveSemesters.Insert(0, selected);

            }

            var model = new TeacherViewModel
            {
                YearSelected = year,
                SemesterSelected = semester,
                ActiveSemesters = teacherActiveSemesters,
                TeacherCourses = _teacherService.GetTeacherCoursesByDate(
                    teacherId,
                    teacherActiveSemesters.First().Year,
                    teacherActiveSemesters.First().Semester
                    )
            };

            return View(model);

        }

        /// <summary>
        /// 
        /// </summary>
        public ActionResult GetTeacherCoursesByDate(int year, string semester)
        {

            var teacherCourses = _teacherService.GetTeacherCoursesByDate(
                _userService.GetUserIdByName(User.Identity.Name),
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