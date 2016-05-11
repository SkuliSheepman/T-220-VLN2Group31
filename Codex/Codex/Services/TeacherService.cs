using Codex.DAL;
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
            foreach(var course in teacherCoursesQuery)
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
            var assignments = _db.Assignments
                              .Where(x => x.CourseInstanceId == courseInstanceId)
                              .Select(_assignment => new AssignmentViewModel
                              {
                                  Id = _assignment.Id,
                                  Name = _assignment.Name,
                                  StartTime = _assignment.StartTime,
                                  EndTime = _assignment.EndTime,
                                  MaxCollaborators = _assignment.MaxCollaborators
                              }).ToList();

            foreach (var _assignment in assignments)
            {
                var problems = (from problemRelation in _db.AssignmentProblems
                                join problem in _db.Problems on problemRelation.ProblemId equals problem.Id
                                where problemRelation.AssignmentId == _assignment.Id
                                select new { problem, problemRelation }).Select(_problemPair => new ProblemViewModel
                                {
                                    Id = _problemPair.problem.Id,
                                    Name = _problemPair.problem.Name,
                                    Weight = _problemPair.problemRelation.Weight
                                }).ToList();
                _assignment.Problems = problems;
            }

            return assignments;
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
            /*var requiresGrading = assignments
                                  .Where(x => x.)*/

            return new List<AssignmentViewModel>();

        }

        public List<AssignmentViewModel> GetClosedAssignmentsFromList(List<AssignmentViewModel> assignments)
        {

            /*(from _assignment in assignments
                                join _dbAssignment in _db.Assignments on _assignment.Id equals _dbAssignment.Id
                                join _submission in _db.Submissions on _dbAssignment.Id equals _submission.AssignmentId
                                join _group in _db.AssignmentGroups on _submission.AspNetUser equals _group.AspNetUser
                                where*/


            return new List<AssignmentViewModel>();

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
                    MaxCollaborators = assignment.MaxCollaborators,
                    AssignmentState = assignmentState
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
                    catch (Exception e)
                    {

                    }
                }
            }
        }
    }
}