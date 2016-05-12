using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Services;
using Codex.Models;

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
        public ActionResult Index(int? year, string semester, int? courseInstanceId) {
            var teacherId = _userService.GetUserIdByName(User.Identity.Name);
            var teacherActiveSemesters = _teacherService.GetTeacherActiveSemestersById(teacherId);

            var courseSelected = new TeacherCourseViewModel {
                OpenAssignments = new List<TeacherAssignmentViewModel>(),
                ClosedAssignments = new List<TeacherAssignmentViewModel>(),
                RequiresGradingAssignments = new List<TeacherAssignmentViewModel>(),
                UpcomingAssignments = new List<TeacherAssignmentViewModel>()
            };

            
            if (year.HasValue && !String.IsNullOrEmpty(semester)) {
                var selected = teacherActiveSemesters.Find(find => find.Year == year.Value && find.Semester == semester);

                teacherActiveSemesters.RemoveAt(teacherActiveSemesters.IndexOf(selected));
                teacherActiveSemesters.Insert(0, selected);
            }

            var teacherCourses = _teacherService.GetTeacherCoursesByDate(
                teacherId,
                teacherActiveSemesters.First().Year,
                teacherActiveSemesters.First().Semester
                );

            if (courseInstanceId.HasValue) {
                courseSelected = teacherCourses.SingleOrDefault(x => x.Id == courseInstanceId);

                if (courseSelected != null) {
                    var assignments = _teacherService.GetAssignmentsInCourseInstanceById(courseInstanceId.Value);
                    foreach (var assignment in assignments) {
                        assignment.Problems = _teacherService.GetProblemsInAssignmentById(assignment.Id);
                        assignment.TimeRemaining = _teacherService.GetAssignmentTimeRemaining(assignment);
                        assignment.NumberOfProblems = assignment.Problems.Count + (assignment.Problems.Count == 1 ? " problems" : " problem");
                        
                        // Get groups
                        foreach (var problem in assignment.Problems)
                        {
                            problem.Groups = _teacherService.GetAssignmentGroups(assignment.Id);
                        }
                    }
                    courseSelected.OpenAssignments = _teacherService.GetOpenAssignmentsFromList(assignments);
                    courseSelected.ClosedAssignments = _teacherService.GetClosedAssignmentsFromList(assignments);
                    courseSelected.RequiresGradingAssignments = _teacherService.GetRequiresGradingAssignmentsFromList(assignments);
                    courseSelected.UpcomingAssignments = _teacherService.GetUpcomingAssignmentsFromList(assignments);
                }
            }

            var model = new TeacherViewModel {
                ActiveSemesters = teacherActiveSemesters,
                TeacherCourses = teacherCourses,
                CourseSelected = courseSelected
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        public ActionResult GetTeacherCoursesByDate(int year, string semester) {
            var teacherCourses = _teacherService.GetTeacherCoursesByDate(
                _userService.GetUserIdByName(User.Identity.Name),
                year,
                semester
                );

            if (Request.IsAjaxRequest()) {
                return Json(teacherCourses);
            }

            return Json(false);
        }

        public ActionResult GradeSubmission(double grade, int submissionId)
        {
            return Json(false);
        }

        public ActionResult UpdateProblem(TeacherProblemUpdateViewModel problem) {
            return Json(_teacherService.UpdateProblem(problem));
        }
    }
}