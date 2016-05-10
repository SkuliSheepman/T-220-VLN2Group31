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
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index() {
            /*
            //Temporary model for testing
            var tempEnd = DateTime.Now;
            tempEnd = tempEnd.AddHours(1);
            tempEnd = tempEnd.AddMinutes(1);

            var tempTimeLeft = new TimeSpan(tempEnd.Ticks - DateTime.Now.Ticks);
            var tempRemaining = String.Empty;

            if (0 < tempTimeLeft.Days) {
                tempRemaining = tempTimeLeft.Days.ToString();
                tempRemaining += (tempTimeLeft.Days == 1 ? " day left" : " days left");
            }
            else if (0 < tempTimeLeft.Hours) {
                tempRemaining = tempTimeLeft.Hours.ToString();
                tempRemaining += (tempTimeLeft.Hours == 1 ? " hour left" : " hours left");
            }
            else if (0 < tempTimeLeft.Minutes) {
                tempRemaining = tempTimeLeft.Minutes.ToString();
                tempRemaining += (tempTimeLeft.Minutes == 1 ? " minute left" : " minutes left");
            }
            else if (0 < tempTimeLeft.Seconds) {
                tempRemaining = tempTimeLeft.Seconds.ToString();
                tempRemaining += (tempTimeLeft.Seconds == 1 ? " second left" : " seconds left");
            }

            var tempProblem = new StudentProblemViewModel
            {
                Id = 1,
                CourseId = 1,
                Name = "Problem 1.1",
                Description = "Temp Description problem 1.1",
                Filetype = ".cpp",
                Attachment = "Attachment.zip",
                Language = "C++",
                Weight = 100
            };

            var tempProblemList = new List<StudentProblemViewModel> {tempProblem};

            var tempNumberOfProblems = tempProblemList.Count.ToString();
            tempNumberOfProblems += (tempProblemList.Count == 1 ? " problem" : " problems");

            var tempIsDone = true;
            
            /*foreach (var problem in tempProblemList) {
                var tempProblemPass = false;
                foreach (var submission in problem.Submissions) {
                    if (submission.FailedTests == 0) {
                        tempProblemPass = true;
                        break;
                    }
                }
                problem.IsAccepted = tempProblemPass;
                if (!tempProblemPass) {
                    tempIsDone = false;
                }
            }*/

            var tempAssignment = new StudentAssignmentViewModel
            {
                Id = 1,
                CourseInstanceId = 13,
                CourseName = "Gagnaskipan",
                Name = "Assignment 1",
                Description = "Temp Description",
                StartTime = DateTime.Now,
                EndTime = tempEnd,
                TimeRemaining = tempRemaining,
                NumberOfProblems = tempNumberOfProblems,
                MaxCollaborators = 3,
                IsDone = tempIsDone,
                AssignmentProblems = tempProblemList
            };

            var tempAssignmentList = new List<StudentAssignmentViewModel> {tempAssignment};

            StudentViewModel model = new StudentViewModel {
                Assignments = tempAssignmentList
            };
            //Temporary model for testing
            */

            var assService = new AssignmentService();
            var userService = new UserService();
            var problemService = new ProblemService();
            var submissionService = new SubmissionService();

            var model = new HomeStudentViewModel();

            String studentId = userService.GetUserIdByName(User.Identity.Name);
            model.Assignments = assService.GetStudentAssignmentsByStudentId(studentId);

            foreach (var assignment in model.Assignments)
            {
                assignment.AssignmentProblems = problemService.GetAllProblemsInStudentAssignment(assignment.Id);
                foreach (var problem in assignment.AssignmentProblems)
                {
                    problem.Submissions = submissionService.GetGroupSubmissionsInProblem(studentId, problem.Id, assignment.Id);
                }
            }

            ViewBag.UserName = User.Identity.Name;
            return View(model);
        }
    }
}