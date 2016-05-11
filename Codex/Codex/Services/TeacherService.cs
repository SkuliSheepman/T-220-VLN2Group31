﻿using Codex.DAL;
using Codex.Models.TeacherViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Services
{
    public class TeacherService
    {
        private Database _db;
        private AssignmentService _assignmentService;

        public TeacherService()
        {
            _db = new Database();
            _assignmentService = new AssignmentService();
        }

        public List<CourseViewModel> GetCoursesByUserId(string teacherId)
        {
            var teacherCoursesQuery = _db.Teachers.Where(x => x.UserId == teacherId);
            var courseList = new List<CourseViewModel>();
            foreach (var course in teacherCoursesQuery)
            {
                courseList.Add(new CourseViewModel
                {
                    Id = course.CourseInstance.Id,
                    Name = course.CourseInstance.Course.Name,
                    IsAssistant = course.IsAssistant,
                    Year = course.CourseInstance.Year,
                    Semester = course.CourseInstance.Semester.Name
                });
            }
            return courseList;
        }

        public List<ActiveSemesterViewModel> GetTeacherActiveSemestersById(string userId)
        {
            var teacherCourses = GetCoursesByUserId(userId);
            var teacherActiveSemesters = new List<ActiveSemesterViewModel>();
            foreach (var _course in teacherCourses)
            {
                var newActiveSemesterEntry = new ActiveSemesterViewModel
                {
                    Year = _course.Year,
                    Semester = _course.Semester
                };
                if (!teacherActiveSemesters.Contains(newActiveSemesterEntry))
                {
                    teacherActiveSemesters.Add(newActiveSemesterEntry);
                }
            }
            return teacherActiveSemesters;
        }

        public List<CourseViewModel> GetTeacherCoursesByDate(string userId, int year, string semester)
        {
            var teacherCourses = GetCoursesByUserId(userId);
            var datedTeacherCourses = teacherCourses.Where(x => x.Year == year && x.Semester == semester).ToList();
            return datedTeacherCourses;
        }

        public List<AssignmentViewModel> GetAssignmentsInCourseInstanceById(int courseInstanceId)
        {

            var course = _db.CourseInstances.SingleOrDefault(x => x.Id == courseInstanceId);
            var assignments = course.Assignments.Select(_assignment => new AssignmentViewModel
                                                        {
                                                            Id = _assignment.Id,
                                                            Name = _assignment.Name,
                                                            StartTime = _assignment.StartTime,
                                                            EndTime = _assignment.EndTime,
                                                            MaxCollaborators = _assignment.MaxCollaborators
                                                        }).ToList();

            return assignments;
        }

        public List<ProblemViewModel> GetProblemsInAssignmentById(int assignmentId)
        {
            var problemQuery = _db.AssignmentProblems.Where(x => x.AssignmentId == assignmentId);
            var problemList = new List<ProblemViewModel>();
            foreach (var problem in problemQuery)
            {
                problemList.Add(new ProblemViewModel
                {
                    Id = problem.Problem.Id,
                    Name = problem.Problem.Name,
                    Weight = problem.Weight
                });
            }
            return problemList;
        }

        public List<AssignmentGroupViewModel> GetAssignmentGroups(int assignmentId)
        {
            var groupQuery = _db.AssignmentGroups.Where(x => x.AssignmentId == assignmentId);
            var groupNumberSet = new HashSet<int>();
            var assignmentGroupList = new List<AssignmentGroupViewModel>();
            foreach (var student in groupQuery)
            {
                groupNumberSet.Add(student.GroupNumber);
            }
            foreach (var group in groupNumberSet)
            {
                var assignmentGroup = new AssignmentGroupViewModel();
                assignmentGroup.GroupNumber = group;
                var studentsGroupQuery = _db.AssignmentGroups.Where(x => x.GroupNumber == group && x.AssignmentId == assignmentId);
                foreach (var student in studentsGroupQuery)
                {
                    assignmentGroup.StudentIds.Add(student.UserId);
                }
                assignmentGroupList.Add(assignmentGroup);
            }
            return assignmentGroupList;
        }

        public List<SubmissionViewModel> GetSubmissionsFromGroupByStudentIds(List<string> studentIds, int assignmentId, int problemId)
        {
            var submissionList = new List<SubmissionViewModel>();
            foreach (var studentId in studentIds)
            {
                var submissionQuery = _db.Submissions.Where(x => x.StudentId == studentId && x.AssignmentId == assignmentId && x.ProblemId == problemId);
                foreach (var submission in submissionQuery)
                {
                    submissionList.Add(new SubmissionViewModel
                    {
                        Id = submission.Id,
                        StudentName = submission.AspNetUser.FullName,
                        SubmissionTime = submission.Time,
                        FailedTests = submission.FailedTests,
                        SubmissionGrade = submission.SubmissionGrade.Grade
                    });
                }
            }
            return submissionList;
        }

        // Gets submission with fewest FailedTests and Newest SubmissionTime
        public SubmissionViewModel GetBestSubmissionFromSubmissionList(List<SubmissionViewModel> submissions)
        {
            var bestSubmission = new SubmissionViewModel();
            foreach (var submission in submissions)
            {
                if (bestSubmission == null)
                {
                    bestSubmission = submission;
                }
                else if (submission.FailedTests < bestSubmission.FailedTests)
                {
                    bestSubmission = submission;
                }
                else if (bestSubmission.FailedTests == submission.FailedTests && bestSubmission.SubmissionTime < submission.SubmissionTime)
                {
                    bestSubmission = submission;
                }
            }
            return bestSubmission;
        }

        public List<AssignmentViewModel> GetOpenAssignmentsFromList(List<AssignmentViewModel> assignments)
        {
            var openAssignments = assignments
                .Where(
                    x => x.StartTime > DateTime.Now
                         && DateTime.Now < x.EndTime)
                .ToList();

            return openAssignments;
        }

        public List<AssignmentViewModel> GetUpcomingAssignmentsFromList(List<AssignmentViewModel> assignments)
        {
            var upcomingAssignments = assignments
                .Where(
                    x => x.StartTime > DateTime.Now)
                .ToList();

            return upcomingAssignments;
        }

        public List<AssignmentViewModel> GetRequiresGradingAssignmentsFromList(List<AssignmentViewModel> assignments)
        {
            var notGradedAssignments = assignments.Where(x => x.EndTime < DateTime.Now && x.IsGraded == false).ToList();
            return notGradedAssignments;
        }

        public List<AssignmentViewModel> GetClosedAssignmentsFromList(List<AssignmentViewModel> assignments)
        {
            var closedAssignments = assignments.Where(x => x.EndTime < DateTime.Now && x.IsGraded == true).ToList();
            return closedAssignments;
        }

        public ProblemUpdateViewModel UpdateProblem(ProblemUpdateViewModel problemViewModel)
        {
            var problemExists = _db.Problems.SingleOrDefault(x => x.Id == problemViewModel.Id);
            /*var problem = new Problem
            {
                CourseId = problemViewModel.CourseId,
                Name = problemViewModel.Name,
                Description = problemViewModel.Description,
                Filetype = problemViewModel.Filetype,
                Attachment = problemViewModel.Attachment,
                Language = problemViewModel.Language
            };*/

            var problem = new Problem
            {
                CourseId = problemViewModel.CourseId,
                Name = problemViewModel.Name,
                Description = problemViewModel.Description,
                Filetype = problemViewModel.Filetype,
                Attachment = problemViewModel.Attachment,
                Language = problemViewModel.Language
            };

            if (problemExists != null)
            {
                problem.Id = problemViewModel.Id;
                problemExists = problem;
            }
            else
            {
                _db.Problems.Add(problem);
            }

            _db.SaveChanges();
            return problemViewModel;
        }

        public List<AssignmentViewModel> GetAllAssignmentsInCourse(int courseInstanceId)
        {
            var assignmentQuery = _db.Assignments.Where(x => x.CourseInstanceId == courseInstanceId);
            var assignmentList = new List<AssignmentViewModel>();
            var assignmentState = "";

            foreach (var assignment in assignmentQuery)
            {
                if (DateTime.Now < assignment.StartTime) { assignmentState = "Upcoming"; }
                else if (assignment.StartTime < DateTime.Now && DateTime.Now < assignment.EndTime) { assignmentState = "Open"; }
                else if (assignment.EndTime < DateTime.Now) { assignmentState = "Ended"; }

                assignmentList.Add(new AssignmentViewModel
                {

                    Id = assignment.Id,
                    Name = assignment.Name,
                    StartTime = assignment.StartTime,
                    EndTime = assignment.EndTime,
                    MaxCollaborators = assignment.MaxCollaborators
                });
            }
            return assignmentList;
        }

        public void CheckUngradedAssignments(int courseInstanceId)
        {
            // Gets all closed and ungraded assignments in courseInstance
            var assignmentsQuery = _db.Assignments.Where(x => x.CourseInstanceId == courseInstanceId && x.EndTime < DateTime.Now && x.IsGraded == false);
            foreach (var assignment in assignmentsQuery)
            {
                // Assumes the assignment is graded
                bool IsGraded = true;

                // Gets all problems in assignment
                var problemQuery = _db.AssignmentProblems.Where(x => x.AssignmentId == assignment.Id);

                // Gets all students assigned to assignment and collects uniqe groupNumbers in HashSet<int>
                var groupQuery = _db.AssignmentGroups.Where(x => x.AssignmentId == assignment.Id);
                var groups = new HashSet<int>();
                foreach (var student in groupQuery)
                {
                    groups.Add(student.GroupNumber);
                }

                // Foreach problem in assignment
                foreach (var problem in problemQuery)
                {
                    // Foreach unique group assigned to assignment
                    foreach (var group in groups)
                    {
                        // Gets all students in particular unique group for assignment
                        var studentsGroupQuery = _db.AssignmentGroups.Where(x => x.GroupNumber == group && x.AssignmentId == assignment.Id);

                        // Gets the list of all submissions from group members
                        var groupSubmissionsQuery = new List<Submission>();
                        foreach (var student in studentsGroupQuery)
                        {
                            // Get all submission that have been graded
                            var studentSubmissionQuery = _db.Submissions.Where(x => x.StudentId == student.UserId && x.AssignmentId == assignment.Id && x.ProblemId == problem.ProblemId && x.SubmissionGrade != null);
                            foreach (var submission in studentSubmissionQuery)
                            {
                                groupSubmissionsQuery.Add(submission);
                            }
                        }
                        // If no submission that is graded is found we break and render the assignment still NOT Graded
                        if (groupSubmissionsQuery == null)
                        {
                            IsGraded = false;
                            break;
                        }
                    }
                    if (IsGraded == false)
                    {
                        break;
                    }
                }
                // We have iterated through the entire assignment and there is no problem with at least one group with no graded submission
                // So we update the database and set the assignment to IsGraded = true
                if (IsGraded == true)
                {
                    assignment.IsGraded = true;
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (Exception e) { }
                }
            }
        }
    }
}