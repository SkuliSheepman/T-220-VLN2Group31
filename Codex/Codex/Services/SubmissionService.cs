using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Codex.DAL;
using Codex.Models;
using Microsoft.Ajax.Utilities;

namespace Codex.Services
{
    public class SubmissionService
    {
        private readonly Database _db;

        public SubmissionService() {
            _db = new Database();
        }

        /// <summary>
        /// Get a submission by its ID
        /// </summary>
        public SubmissionViewModel GetSubmissionById(int submissionId) {
            var submission = _db.Submissions.SingleOrDefault(x => x.Id == submissionId);

            if (submission != null) {
                var submissionNumber = _db.Submissions.Where(x => x.AspNetUser.Id == submission.AspNetUser.Id).ToList().FindIndex(x => x.Id == submission.Id) + 1;

                var model = new SubmissionViewModel {
                    Id = submission.Id,
                    FileName = submission.OriginalFileName,
                    SubmissionNumber = submissionNumber,
                    Time = submission.Time,
                    Assignment = new SubmissionAssignmentViewModel {
                        Id = submission.AssignmentId,
                        Course = submission.Assignment.CourseInstance.Course.Name,
                        Name = submission.Assignment.Name
                    },
                    Problem = new SubmissionProblemViewModel {
                        Id = submission.ProblemId,
                        Name = submission.Problem.Name
                    },
                    FailedTestCases = new List<SubmissionTestCaseViewModel>(),
                    PassedTestCases = new List<SubmissionTestCaseViewModel>()
                };

                // Get weight, incase of null
                var problem = _db.AssignmentProblems.SingleOrDefault(x => x.AssignmentId == submission.AssignmentId && x.ProblemId == submission.ProblemId);
                if (problem != null) {
                    model.Problem.Weight = problem.Weight;
                }

                // Add passed test cases
                var count = 1;
                foreach (var result in _db.TestResults.Where(x => x.SubmissionId == submission.Id && x.Passed.Value)) {
                    if (!result.Passed.HasValue) continue;
                    var testCase = new SubmissionTestCaseViewModel {
                        TestCaseNumber = count,
                        ExpectedOutput = result.TestCase.ExpectedOutput,
                        Input = result.TestCase.Input,
                        SubmissionOutput = result.ProgramOutput
                    };

                    model.PassedTestCases.Add(testCase);

                    count++;
                }

                // Add failed test cases
                count = 1;
                foreach (var result in _db.TestResults.Where(x => x.SubmissionId == submission.Id && !x.Passed.Value)) {
                    if (!result.Passed.HasValue) continue;
                    var testCase = new SubmissionTestCaseViewModel {
                        TestCaseNumber = count,
                        ExpectedOutput = result.TestCase.ExpectedOutput,
                        Input = result.TestCase.Input,
                        SubmissionOutput = result.ProgramOutput
                    };

                    model.FailedTestCases.Add(testCase);

                    count++;
                }

                return model;
            }

            return null;
        }

        /// <summary>
        /// Verify that the user requesting the submission has access to it by checking the submissions user group
        /// and checking if the user is in the group
        /// </summary>
        public bool VerifyUser(string username, int submissionId) {
            var submission = _db.Submissions.SingleOrDefault(x => x.Id == submissionId);

            if (submission != null) {
                var user = _db.AspNetUsers.SingleOrDefault(x => x.UserName == username);

                if (user != null) {
                    var submitter = _db.AspNetUsers.SingleOrDefault(x => x.Id == submission.StudentId);

                    if (submitter != null) {
                        var userGroup = _db.AssignmentGroups.SingleOrDefault(x => x.UserId == user.Id && x.AssignmentId == submission.AssignmentId);
                        var submitterGroup = _db.AssignmentGroups.SingleOrDefault(x => x.UserId == submitter.Id && x.AssignmentId == submission.AssignmentId);

                        return (userGroup == submitterGroup);
                    }
                }
            }

            return false;
        }
    }
}