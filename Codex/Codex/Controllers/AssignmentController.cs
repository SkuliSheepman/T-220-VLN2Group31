using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Services;
using Codex.Models.SharedModels.SharedViewModels;

namespace Codex.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly AssignmentService _assignmentService;
        private readonly ProblemService _problemService;

        public AssignmentController() {
            _assignmentService = new AssignmentService();
            _problemService = new ProblemService();
        }

        // GET: Assignment
        public ActionResult Index(int Id) {
            var model = _assignmentService.GetAssignment(Id);

            return View(model);
        }


        public ActionResult GetAssignmentById(int assignmentId) {
            var result = _assignmentService.GetAssignment(assignmentId);
            return Json(result);
        }

        public ActionResult GetCollaborators(int assignmentId, string studentId) {
            var result = _assignmentService.GetCollaborators(assignmentId, studentId);
            return Json(result);
        }

        public ActionResult GetProblemsInAssignmentById(int assignmentId) {
            var result = _problemService.GetAllProblemsInAssignment(assignmentId);
            return Json(result);
        }

        public ActionResult RemoveCollaboratorsFromAssignment(int assignmentId, string studentId) {
            var result = _assignmentService.RemoveCollboratorsFromAssignment(assignmentId, studentId);
            return Json(result);
        }

        public ActionResult AddCollaboratorsToAssignment(int assignmentId, string studentId) {
            var result = _assignmentService.AddCollaborator(assignmentId, studentId);
            return Json(result);
        }
    }
}