using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

        /// <summary>
        /// Inserts an entry for a submission in the database by assignment Id, problem Id and user Id
        /// </summary>
        public int InsertSubmissionToDatabase(HttpPostedFileBase file, int assignmentId, int problemId, string userId) {
            Submission newSubmission = new Submission {
                StudentId = userId,
                AssignmentId = assignmentId,
                ProblemId = problemId,
                OriginalFileName = file.FileName,
                Time = DateTime.Now
            };

            _db.Submissions.Add(newSubmission);

            try {
                _db.SaveChanges();
                return newSubmission.Id;
            }
            catch (Exception e) {
                return 0;
            }
        }

        /// <summary>
        /// Uploads a submission to the server by assignment Id, problem Id and user Id
        /// </summary>
        public bool UploadSubmissionToServer(HttpPostedFileBase file, int assignmentId, int problemId, int submissionId) {
            string uploadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["SubmissionsFolder"]);
            uploadPath += assignmentId + "\\" + problemId + "\\" + submissionId + "\\";

            if (!Directory.Exists(uploadPath)) {
                Directory.CreateDirectory(uploadPath);
            }

            file.SaveAs(uploadPath + submissionId + Path.GetExtension(file.FileName));

            if (File.Exists(uploadPath + submissionId)) {
                return true;
            }

            return false;
        }
    }
}