using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Codex.DAL;
using Codex.Models.SharedModels.SharedViewModels;

namespace Codex.Services
{
    public class SubmissionService
    {
        private readonly Database _db;
        private AssignmentService _assignmentService;

        public SubmissionService() {
            _db = new Database();
        }

        public List<SubmissionViewModel> GetGroupSubmissionsInProblem(string studentId, int problemId, int assignmentId) {
            var collaborators = _assignmentService.GetCollaborators(assignmentId, studentId);
            var groupSubmissions = new List<SubmissionViewModel>();
            foreach (var student in collaborators) {
                var submissions = _db.Submissions.Where(x => x.AssignmentId == assignmentId && x.ProblemId == problemId && x.StudentId == student.Id);
                foreach (var submission in submissions) {
                    groupSubmissions.Add(new SubmissionViewModel() {
                        Id = submission.Id,
                        SubmissionTime = submission.Time,
                        FailedTests = submission.FailedTests,
                        Owner = student.Id
                    });
                }
            }
            return groupSubmissions;
        }

        public List<SubmissionViewModel> GetAllGroupSubmissionsInProblem(string groupId, int assignmentId) {
            return new List<SubmissionViewModel>();
        }

        
    }
}