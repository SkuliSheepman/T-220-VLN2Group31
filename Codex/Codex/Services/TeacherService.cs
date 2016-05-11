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

    }
}