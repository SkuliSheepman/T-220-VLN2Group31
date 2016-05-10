using Codex.DAL;
using Codex.Models.StudentViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Services
{
    public class StudentService
    {
        private Database _db;
        public StudentService()
        {
            _db = new Database();
        }
        
        public List<CollaboratorViewModel> GetCollaborators(int assignmentId, string studentId)
        {
            var collaboratorList = new List<CollaboratorViewModel>();

            var groupNumberQuery = _db.AssignmentGroups.SingleOrDefault(x => x.AssignmentId == assignmentId && x.AspNetUser.Id == studentId).GroupNumber;
            var collaboratorQuery = _db.AssignmentGroups.Where(x => x.AssignmentId == assignmentId && x.GroupNumber == groupNumberQuery);
            foreach (var collaborator in collaboratorQuery)
            {
                collaboratorList.Add(new CollaboratorViewModel
                {
                    Id = collaborator.AspNetUser.Id,
                    Name = collaborator.AspNetUser.FullName,
                    GroupNumber = collaborator.GroupNumber,
                });
            }
            return collaboratorList;
        }

        public List<StudentAssignmentViewModel> GetStudentAssignmentsByStudentId(string studentId)
        {
            var assignmentList = new List<StudentAssignmentViewModel>();

            var assignmentQuery = _db.AssignmentGroups.Where(x => x.UserId == studentId);

            foreach (var group in assignmentQuery)
            {
                assignmentList.Add(new StudentAssignmentViewModel
                {
                    Id = group.Assignment.Id,
                    Course = group.Assignment.CourseInstance.Course.Name,
                    Name = group.Assignment.Name,
                    StartTime = group.Assignment.StartTime,
                    EndTime = group.Assignment.EndTime,
                    MaxCollaborators = group.Assignment.MaxCollaborators,
                    AssignmentGrade = group.AssignmentGrade,
                });
            }
            foreach (var assignment in assignmentList)
            {
                assignment.Collaborators = GetCollaborators(assignment.Id, studentId);
            }
            return assignmentList;
        }
        public List<StudentProblemViewModel> GetStudentProblemsByAssignmentId(int assignmentId)
        {
            var problemQuery = _db.AssignmentProblems.Where(x => x.AssignmentId == assignmentId);
            var problemList = new List<StudentProblemViewModel>();
            foreach (var relation in problemQuery)
            {
                problemList.Add(new StudentProblemViewModel()
                {
                    Id = relation.Problem.Id,
                    CourseId = relation.Problem.CourseId,
                    Name = relation.Problem.Name,
                    Description = relation.Problem.Description,
                    Filetype = relation.Problem.Filetype,
                    Attachment = relation.Problem.Attachment,
                    Weight = relation.Weight,
                    MaxSubmissions = relation.MaxSubmissions
                });
            }
            return problemList;
        }
        public List<StudentSubmissionViewModel> GetSubmissionsByAssignmentGroup(string studentId, int problemId, int assignmentId)
        {
            var groupSubmissions = new List<StudentSubmissionViewModel>();

            var collaboratorQuery = GetCollaborators(assignmentId, studentId);
           
            foreach (var student in collaboratorQuery)
            {
                var submissionQuery = _db.Submissions.Where(x => x.AssignmentId == assignmentId && x.ProblemId == problemId && x.StudentId == student.Id);
                foreach (var submission in submissionQuery)
                {
                    groupSubmissions.Add(new StudentSubmissionViewModel()
                    {
                        Id = submission.Id,
                        SubmissionTime = submission.Time,
                        FailedTests = submission.FailedTests,
                        Owner = student.Id
                    });
                }
            }
            return groupSubmissions;
        }
    }
}