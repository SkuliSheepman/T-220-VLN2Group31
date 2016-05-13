using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Codex.Services;
using Codex.Models;

/*using Codex.Models.StudentModels.HelperModels;
using Codex.Models.StudentModels.ViewModels;*/

namespace Codex.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {

        private readonly UserService _userService;
        private readonly FileService _fileService;
        private readonly StudentService _studentService;

        public StudentController() {
            _userService = new UserService();
            _fileService = new FileService();
            _studentService = new StudentService();
        }

        /// <summary>
        /// Gets all assignments/problems and submissions related to the student
        /// </summary>
        public ActionResult Index() {
            var studentId = _userService.GetUserIdByName(User.Identity.Name);

            // Populate assignments
            var userAssignments = _studentService.GetStudentAssignmentsByStudentId(studentId);

            foreach (var assignment in userAssignments) {
                assignment.Problems = _studentService.GetStudentProblemsByAssignmentId(assignment.Id);

                foreach (var problem in assignment.Problems) {
                    problem.Submissions = _studentService.GetSubmissionsByAssignmentGroup(studentId, problem.Id, assignment.Id);
                    problem.IsAccepted = _studentService.IsProblemDone(problem);
                    problem.BestSubmission = _studentService.GetBestSubmission(problem.Submissions);
                }

                assignment.TimeRemaining = _studentService.GetAssignmentTimeRemaining(assignment);
                assignment.IsDone = _studentService.IsAssignmentDone(assignment);
                assignment.NumberOfProblems = assignment.Problems.Count + " " + (assignment.Problems.Count == 1 ? "problem" : "problems");
            }

            StudentViewModel model = new StudentViewModel {
                studentId = studentId,
                Assignments = userAssignments
            };

            ViewBag.UserName = User.Identity.Name;
            ViewBag.UserId = _userService.GetUserIdByName(User.Identity.Name);
            return View(model);
        }

        /// <summary>
        /// Gets a specific assignment as a single view from a query ID
        /// </summary>
        [Authorize]
        public ActionResult Assignment(int? id)
        {

            if (id.HasValue)
            {

                var studentId = _userService.GetUserIdByName(User.Identity.Name);
                var assignment = _studentService.GetStudentAssignmentById(id.Value, studentId);

                assignment.Problems = _studentService.GetStudentProblemsByAssignmentId(assignment.Id);

                foreach (var problem in assignment.Problems)
                {
                    problem.Submissions = _studentService.GetSubmissionsByAssignmentGroup(studentId, problem.Id, assignment.Id);
                    problem.IsAccepted = _studentService.IsProblemDone(problem);
                    problem.BestSubmission = _studentService.GetBestSubmission(problem.Submissions);
                }

                assignment.TimeRemaining = _studentService.GetAssignmentTimeRemaining(assignment);
                assignment.IsDone = _studentService.IsAssignmentDone(assignment);
                assignment.NumberOfProblems = assignment.Problems.Count + " " + (assignment.Problems.Count == 1 ? "problem" : "problems");

                ViewBag.UserName = User.Identity.Name;
                ViewBag.UserId = _userService.GetUserIdByName(User.Identity.Name);
                ViewBag.Loners = _studentService.GetLonelyCollaboratorsInCourseInstance(assignment.Id);
                return View(assignment);

            }

            return RedirectToAction("Index", "Student");

        }

        /// <summary>
        /// Submits a user soloution to a problem in an uploaded file
        /// </summary>
        public ActionResult Submit(HttpPostedFileBase file, int assignmentId, int problemId) {
            var studentId = _userService.GetUserIdByName(User.Identity.Name);
            var submissionAllowed = _studentService.IsSubmissionAllowed(studentId, assignmentId, problemId);

            if (!submissionAllowed) {
                return Json("max");
            }

            if (file != null && 0 < file.ContentLength && assignmentId != 0 && problemId != 0) {
                var userId = _userService.GetUserIdByName(User.Identity.Name);

                var submissionId = _studentService.InsertSubmissionToDatabase(file, assignmentId, problemId, userId);

                if (submissionId != 0) {
                    if (_fileService.UploadSubmissionToServer(file, assignmentId, problemId, submissionId)) {
                        if (_fileService.CompileCPlusPlusBySubmissionId(submissionId)) {
                            var failed = _fileService.RunTestCasesBySubmissionId(submissionId);
                            if (failed != -1) {
                                var obj = new { SubmissionId = submissionId, Message = failed };
                                return Json(obj);
                            }
                            else {
                                return Json("case");
                            }
                        }
                        else {
                            var obj = new { SubmissionId = submissionId, Message = "compile" };
                            return Json(obj);
                        }
                    }
                    else {
                        return Json("write");
                    }
                }
                else {
                    return Json("db");
                }
            }

            return Json(false);
        }

        /// <summary>
        /// Downloads an attachment for a problem
        /// </summary>
        public void DownloadAttachmentFile(int? problemId, int? assignmentId)
        {
            var name = User.Identity.Name;
            if (problemId.HasValue && assignmentId.HasValue)
            {
                _fileService.DownloadAttachment(_userService.GetUserIdByName(User.Identity.Name), problemId.Value, assignmentId.Value);
            }
        }

        /// <summary>
        /// Makes the current session user leave the assignment id group
        /// </summary>
        public ActionResult LeaveAssignmentGroup(int? assignmentId)
        {
            if (assignmentId.HasValue)
            {
                return Json(_studentService.AssignUserToGroup(_userService.GetUserIdByName(User.Identity.Name), assignmentId.Value));
            }
            else
            {
                return Json(true);
            }
        }

        /// <summary>
        /// Assigns a user to a group as long as it's provided with an assignment id and groupid
        /// </summary>
        public ActionResult AssignUserToGroup(string userId, int? assignmentId, int? groupId)
        {
            if (userId != null && assignmentId.HasValue && groupId.HasValue)
            {
                return Json(_studentService.AssignUserToGroup(userId, assignmentId.Value, groupId.Value));
            } else
            {
                return Json(true);
            }
        }
    }
}