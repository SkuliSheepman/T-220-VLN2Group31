﻿using Codex.DAL;
using Codex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Services
{
    public class StudentService
    {
        private Database _db;
        private CourseService _courseService;

        public StudentService() {
            _db = new Database();
            _courseService = new CourseService();
        }

        /// <summary>
        /// Find a students collaborators in a specific assignment. Returns a list af CollaboratorViewModel including the given student.
        /// </summary>
        public List<CollaboratorViewModel> GetCollaborators(int assignmentId, string studentId) {
            var collaboratorList = new List<CollaboratorViewModel>();

            var groupNumberQuery = _db.AssignmentGroups.SingleOrDefault(x => x.AssignmentId == assignmentId && x.AspNetUser.Id == studentId).GroupNumber;
            var collaboratorQuery = _db.AssignmentGroups.Where(x => x.AssignmentId == assignmentId && x.GroupNumber == groupNumberQuery);
            foreach (var collaborator in collaboratorQuery) {
                collaboratorList.Add(new CollaboratorViewModel {
                    Id = collaborator.AspNetUser.Id,
                    Name = collaborator.AspNetUser.FullName,
                    GroupNumber = collaborator.GroupNumber,
                });
            }
            return collaboratorList;
        }

        /// <summary>
        /// Finds all assignments that a student is connected too based on his Id. Returns a list of StudentAssignmentViewModel
        /// </summary>
        public List<StudentAssignmentViewModel> GetStudentAssignmentsByStudentId(string studentId) {
            var assignmentList = new List<StudentAssignmentViewModel>();

            var assignmentQuery = _db.AssignmentGroups.Where(x => x.UserId == studentId);

            foreach (var group in assignmentQuery) {
                assignmentList.Add(new StudentAssignmentViewModel {
                    Id = group.Assignment.Id,
                    Course = group.Assignment.CourseInstance.Course.Name,
                    Name = group.Assignment.Name,
                    Description = group.Assignment.Description,
                    StartTime = group.Assignment.StartTime,
                    EndTime = group.Assignment.EndTime,
                    MaxCollaborators = group.Assignment.MaxCollaborators,
                    AssignmentGrade = group.AssignmentGrade
                });
            }
            foreach (var assignment in assignmentList) {
                assignment.Collaborators = GetCollaborators(assignment.Id, studentId);
            }
            return assignmentList;
        }

        /// <summary>
        /// Gets all problems in assignment based on assignment Id, Returnst a list of StudentProblemViewModel
        /// </summary>
        public List<StudentProblemViewModel> GetStudentProblemsByAssignmentId(int assignmentId) {
            var problemQuery = _db.AssignmentProblems.Where(x => x.AssignmentId == assignmentId);
            var problemList = new List<StudentProblemViewModel>();
            foreach (var relation in problemQuery) {
                problemList.Add(new StudentProblemViewModel() {
                    Id = relation.Problem.Id,
                    CourseId = relation.Problem.CourseId,
                    Name = relation.Problem.Name,
                    Description = relation.Problem.Description,
                    Filetype = relation.Problem.Filetype,
                    Attachment = relation.Problem.Attachment,
                    Weight = relation.Weight,
                    MaxSubmissions = relation.MaxSubmissions,
                    IsAccepted = false
                });
            }
            return problemList;
        }

        /// <summary>
        /// Gets submission to problem from all collaborators in assignment. Returns list of StudentSubmissionViewModel
        /// </summary>
        public List<StudentSubmissionViewModel> GetSubmissionsByAssignmentGroup(string studentId, int problemId, int assignmentId) {
            var groupSubmissions = new List<StudentSubmissionViewModel>();

            var collaboratorQuery = GetCollaborators(assignmentId, studentId);

            foreach (var student in collaboratorQuery) {
                var submissionQuery = _db.Submissions.Where(x => x.AssignmentId == assignmentId && x.ProblemId == problemId && x.StudentId == student.Id).OrderByDescending(x => x.Id);
                foreach (var submission in submissionQuery) {
                    groupSubmissions.Add(new StudentSubmissionViewModel() {
                        Id = submission.Id,
                        SubmissionTime = submission.Time,
                        FailedTests = submission.FailedTests,
                        Owner = student.Id,
                        OriginalFilename = submission.OriginalFileName
                    });
                }
            }
            return groupSubmissions;
        }

        /// <summary>
        /// Check if a problem has any accepted submissions via StudentProblemViewModel
        /// </summary>
        public bool IsProblemDone(StudentProblemViewModel problem) {
            var done = false;
            foreach (var submission in problem.Submissions) {
                if (submission.FailedTests == 0) {
                    done = true;
                    break;
                }
            }

            return done;
        }

        /// <summary>
        /// Check if an assignment's problems are accepted
        /// </summary>
        public bool IsAssignmentDone(StudentAssignmentViewModel assignment) {
            return assignment.Problems.All(x => x.IsAccepted);
        }

        /// <summary>
        /// Get how much time is left of an assignment in terms of days, hours, minutes or second, 
        /// depending on how much time is left
        /// </summary>
        public string GetAssignmentTimeRemaining(StudentAssignmentViewModel assignment) {
            var timeRemaining = String.Empty;

            if (assignment.StartTime.HasValue && assignment.EndTime.HasValue) {
                if (0 < assignment.EndTime.Value.CompareTo(DateTime.Now)) {
                    var remainingTimeSpan = new TimeSpan(assignment.EndTime.Value.Ticks - DateTime.Now.Ticks);
                    if (0 < remainingTimeSpan.Days) {
                        timeRemaining = remainingTimeSpan.Days.ToString();
                        timeRemaining += (remainingTimeSpan.Days == 1 ? " day left" : " days left");
                    }
                    else if (0 < remainingTimeSpan.Hours) {
                        timeRemaining = remainingTimeSpan.Hours.ToString();
                        timeRemaining += (remainingTimeSpan.Hours == 1 ? " hour left" : " hours left");
                    }
                    else if (0 < remainingTimeSpan.Minutes) {
                        timeRemaining = remainingTimeSpan.Minutes.ToString();
                        timeRemaining += (remainingTimeSpan.Minutes == 1 ? " minute left" : " minutes left");
                    }
                    else if (0 < remainingTimeSpan.Seconds) {
                        timeRemaining = remainingTimeSpan.Seconds.ToString();
                        timeRemaining += (remainingTimeSpan.Seconds == 1 ? " second left" : " seconds left");
                    }
                }
            }

            return timeRemaining;
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
        /// Get the most recent submisssion with the fewest failed cases, or null if there are no submissions
        /// </summary>
        public StudentSubmissionViewModel GetBestSubmission(List<StudentSubmissionViewModel> groupSubmissions) {
            var bestSubmission = new StudentSubmissionViewModel();
            if (groupSubmissions.Count != 0) {
                //List is sorted by FailedTests (Null last) then by Submission time, fewest failed cases first, in order of submissison time
                groupSubmissions = groupSubmissions.OrderByDescending(x => x.FailedTests.HasValue)
                                                   .ThenBy(x => x.FailedTests)
                                                   .ThenByDescending(x => x.SubmissionTime)
                                                   .ToList();

                //The first submission in the list will be the most recent submission with the fewest failed cases
                bestSubmission = groupSubmissions.First();

                return bestSubmission;
            }
            //if there are no submissions, return null
            else return null;
        }

        /// <summary>
        /// Get an assignment by it's id
        /// </summary>
        public StudentAssignmentViewModel GetStudentAssignmentById(int assignmentId, string studentId) {

            var group = _db.AssignmentGroups.Where(x => x.UserId == studentId && x.AssignmentId == assignmentId).SingleOrDefault();
            var assignment = new StudentAssignmentViewModel
            {
                Id = group.Assignment.Id,
                Course = group.Assignment.CourseInstance.Course.Name,
                Name = group.Assignment.Name,
                Description = group.Assignment.Description,
                StartTime = group.Assignment.StartTime,
                EndTime = group.Assignment.EndTime,
                MaxCollaborators = group.Assignment.MaxCollaborators,
                AssignmentGrade = group.AssignmentGrade,
                Collaborators = GetCollaborators(assignmentId, studentId)
            };

            return assignment;

        }

        /// <summary>
        /// Assign user to an existing group or a new one 
        /// </summary>
        public bool AssignUserToGroup(string userId, int assignmentId, int groupNumber = -1)
        {

            var user = _db.AspNetUsers.FirstOrDefault(x => x.Id == userId);
            var assignment = _db.Assignments.FirstOrDefault(x => x.Id == assignmentId);
            var currentAssignmentGroupRelation = _db.AssignmentGroups.FirstOrDefault(x => x.UserId == userId && x.AssignmentId == assignmentId);

            if (user != null && assignment != null)
            {
                if (groupNumber != -1)
                {

                    currentAssignmentGroupRelation.GroupNumber = groupNumber;

                } else //new group
                {
                    
                    var highestAssignmentProblemGroupNumber = _db.AssignmentGroups.Where(x => x.AssignmentId == assignmentId).OrderByDescending(y => y.GroupNumber).First().GroupNumber;
                    currentAssignmentGroupRelation.GroupNumber = highestAssignmentProblemGroupNumber    ;

                }

                try
                {
                    _db.SaveChanges();
                    return true;
                } catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a user from a specific group
        /// </summary>
        public bool RemoveUserFromGroupById(string userId, int assignmentId)
        {

            var assignmentGroupRelation = _db.AssignmentGroups
                                  .FirstOrDefault(x => x.UserId == userId
                                                  && x.AssignmentId == assignmentId);

            if (assignmentGroupRelation != null)
            {
                _db.AssignmentGroups.Remove(assignmentGroupRelation);
            }

            try
            {
                _db.SaveChanges();
                return true;
            } catch
            {
                return false;
            }

        }

        /// <summary>
        /// Gets all students whom are alone in assignment groups
        /// </summary>
        public List<CollaboratorViewModel> GetLonelyCollaboratorsInCourseInstance(int assignmentid)
        {

            var loners = new List<CollaboratorViewModel>();

            var assignment = _db.Assignments.FirstOrDefault(x => x.Id == assignmentid);

            var usersInCourseInstance = _db.CourseInstances.FirstOrDefault(x => x.Id == assignment.CourseInstanceId);

            foreach (var user in usersInCourseInstance.AspNetUsers)
            {
                var collaborators = GetCollaborators(assignment.Id, user.Id);
                if (collaborators.Count() == 1)
                {
                    loners.Add(new CollaboratorViewModel
                    {
                        Id = user.Id,
                        Name = user.FullName
                    });
                }
            }

            return loners;
        }
    }
}
