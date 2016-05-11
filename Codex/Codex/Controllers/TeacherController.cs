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
        private readonly TeacherService _teacherService;

        public TeacherController() {
            _userService = new UserService();
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

            var teacherCourses = _teacherService.GetTeacherCoursesByDate(
                    teacherId,
                    teacherActiveSemesters.First().Year,
                    teacherActiveSemesters.First().Semester
                    );

            var model = new TeacherViewModel
            {
                YearSelected = year,
                SemesterSelected = semester,
                ActiveSemesters = teacherActiveSemesters,
                TeacherCourses = teacherCourses
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

        public ActionResult UpdateProblem(ProblemUpdateViewModel problem)
        {
            return Json(_teacherService.UpdateProblem(problem));
        }

    }
}