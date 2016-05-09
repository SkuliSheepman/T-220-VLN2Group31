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
    public class StudentController : Controller {
        // GET: Student
        public ActionResult Index() {
            //Temporary model for testing
            var tempstart = DateTime.Now;
            tempstart = tempstart.AddHours(1);
            tempstart = tempstart.AddMinutes(1);
            var tempnow = DateTime.Now;


            var tempticks = tempstart.Ticks - tempnow.Ticks;
            var tempstimespan = new TimeSpan(tempticks);
             
            var templist = new List<ProblemViewModel> { };
            var tempprob = new ProblemViewModel {
                Id = 1,
                CourseId = 1,
                Name = "Problem 1.1",
                Description = "Temp Description problem 1.1",
                Filetype = ".cpp",
                Attachment = "Attachment",
                Language = "C++"
            };
            templist.Add(tempprob);
            var tempass = new AssignmentViewModel {
                Id = 1,
                CourseInstanceId = 1,
                Name = "Assignment 1",
                Description = "Temp Description",
                StartTime = DateTime.Now,
                EndTime = tempstart,
                MaxCollaborators = 3,
                AssignmentProblems = templist
            };
            var templist2 = new List<AssignmentViewModel>();
            templist2.Add(tempass);
            StudentViewModel model = new StudentViewModel {
                Assignments = templist2
            };
            //Temporary model for testing

            return View(model);
        }
    }
}