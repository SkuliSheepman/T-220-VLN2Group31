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
        private readonly FileService _fileService;

        public TeacherController() {
            _userService = new UserService();
            _teacherService = new TeacherService();
            _fileService = new FileService();
        }

        // GET: Teacher
        public ActionResult Index(int? year, string semester, int? courseInstanceId) {
            var teacherId = _userService.GetUserIdByName(User.Identity.Name);
            var teacherActiveSemesters = _teacherService.GetTeacherActiveSemestersById(teacherId);

            var courseSelected = new TeacherCourseViewModel {
                OpenAssignments = new List<TeacherAssignmentViewModel>(),
                ClosedAssignments = new List<TeacherAssignmentViewModel>(),
                RequiresGradingAssignments = new List<TeacherAssignmentViewModel>(),
                UpcomingAssignments = new List<TeacherAssignmentViewModel>(),
                ProblemList = new List<TeacherProblemUpdateViewModel>()
            };


            if (year.HasValue && !String.IsNullOrEmpty(semester)) {
                var selected = teacherActiveSemesters.Find(find => find.Year == year.Value && find.Semester == semester);

                teacherActiveSemesters.RemoveAt(teacherActiveSemesters.IndexOf(selected));
                teacherActiveSemesters.Insert(0, selected);
            }
            else {
                var selected = _teacherService.GetClosestSemester(teacherActiveSemesters);

                teacherActiveSemesters.RemoveAt(teacherActiveSemesters.IndexOf(selected));
                teacherActiveSemesters.Insert(0, selected);
            }

            var teacherCourses = _teacherService.GetTeacherCoursesByDate(
                teacherId,
                teacherActiveSemesters.First().Year,
                teacherActiveSemesters.First().Semester
                );

            if (!courseInstanceId.HasValue) {
                courseInstanceId = teacherCourses.First().Id;
            }
            courseSelected = teacherCourses.SingleOrDefault(x => x.Id == courseInstanceId);

            if (courseSelected != null) {
                // Populate assignment problems
                var assignments = _teacherService.GetAssignmentsInCourseInstanceById(courseInstanceId.Value);
                foreach (var assignment in assignments) {
                    assignment.Problems = _teacherService.GetProblemsInAssignmentById(assignment.Id);
                    assignment.TimeRemaining = _teacherService.GetAssignmentTimeRemaining(assignment);
                    assignment.NumberOfProblems = assignment.Problems.Count + (assignment.Problems.Count == 1 ? " problems" : " problem");

                    // Get groups
                    foreach (var problem in assignment.Problems) {
                        problem.Groups = _teacherService.GetAssignmentGroups(assignment.Id);
                    }
                }
                courseSelected.OpenAssignments = _teacherService.GetOpenAssignmentsFromList(assignments);
                courseSelected.ClosedAssignments = _teacherService.GetClosedAssignmentsFromList(assignments);
                courseSelected.RequiresGradingAssignments = _teacherService.GetRequiresGradingAssignmentsFromList(assignments);
                courseSelected.UpcomingAssignments = _teacherService.GetUpcomingAssignmentsFromList(assignments);

                // Populate course problem list
                courseSelected.ProblemList = _teacherService.GetProblemsInCourseById(courseSelected.Id);
            }

            var model = new TeacherViewModel {
                ActiveSemesters = teacherActiveSemesters,
                TeacherCourses = teacherCourses,
                CourseSelected = courseSelected
            };

            ViewBag.UserName = User.Identity.Name;
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

        public ActionResult NewAssignment(TeacherAssignmentViewModel assignment)
        {
            return Json(assignment);
        }

        /* Creating a new problem requires 3 methods due to AJAX and model binding being weird when uploading a file, with data and a list of data */

        public ActionResult NewProblem(TeacherNewProblemViewModel problem) {
            return Json(_teacherService.CreateNewProblem(problem));
        }

        public ActionResult UpdateTestCases(List<TeacherTestCaseViewModel> testCases, int? problemId)
        {
            if (problemId.HasValue) {
                return Json(_teacherService.SetTestCasesForProblemByProblemId(problemId.Value, testCases));
            }
            else {
                return Json(false);
            } 
        }

        public ActionResult UpdateAttachment(HttpPostedFileBase attachment, int? problemId)
        {
            if (problemId.HasValue) {
                if (_teacherService.SetAttachmentToProblemInDatabaseByProblemId(problemId.Value, attachment.FileName)) {
                    if (_fileService.UploadAttachmentToServer(attachment, problemId.Value)) {
                        return Json("success");
                    }
                    else {
                        return Json("write");
                    }
                }
                else {
                    return Json("database");
                }
            }
            
            return Json(false);
        }
    }
}