using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Models;
using Codex.Services;

namespace Codex.Controllers
{
    [Authorize]
    public class SubmissionController : Controller
    {
        private readonly SubmissionService _submissionService;

        public SubmissionController()
        {
            _submissionService = new SubmissionService();
        }

        // GET: Submission
        public ActionResult Index(int? id) {
            if (id.HasValue && _submissionService.VerifyUser(User.Identity.Name, id.Value)) {
                var model = _submissionService.GetSubmissionById(id.Value);

                ViewBag.UserName = User.Identity.Name;
                return View(model);
            }

            if (User.IsInRole("Teacher")) {
                return RedirectToAction("Index", "Teacher");
            }
            else {
                return RedirectToAction("Index", "Student");
            }
        }
    }
}