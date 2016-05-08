using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.DAL;
using Codex.Models.SharedModels.SharedViewModels;
using Codex.Models;
using Codex.Services;

namespace Codex.Services
{

    public class AssignmentService
    {

        private Database _db;
        private ProblemService _problemService;
        private CourseService _courseService;
        

        public AssignmentService()
        {
            _db = new Database();
            _problemService = new ProblemService();
            _courseService = new CourseService();
        }

        /// <summary>
        /// creates a new assignment
        /// </summary>
        public bool CreateAssignment(AssignmentCreationViewModel newAssignmentViewModel)
        {

            // new assignment entry entity
            var newAssignment = new Assignment
            {
                CourseInstanceId = newAssignmentViewModel.CourseInstanceId,
                Name = newAssignmentViewModel.Name,
                Description = newAssignmentViewModel.Description,
                StartTime = newAssignmentViewModel.StartTime,
                EndTime = newAssignmentViewModel.EndTime,
                MaxCollaborators = newAssignmentViewModel.MaxCollaborators
            };

            newAssignment = _db.Assignments.Add(newAssignment);

            // assign problems to the assignment
            foreach (var _problemId in newAssignmentViewModel.AssignmentProblems)
            {

                var relation = new AssignmentProblem
                {
                    AssignmentId = newAssignment.Id,
                    ProblemId = _problemId
                };

                _db.AssignmentProblems.Add(relation);

            }

            var students = _courseService.GetAllStudentsInCourseInstance(newAssignmentViewModel.CourseInstanceId);

            // create groups for students
            foreach (var _student in students)
            {

                _db.AssignmentGroups.Add(new AssignmentGroup
                {

                    UserId       = _student.Id,
                    AssignmentId = newAssignment.Id,

                });

            }

            try
            {
                _db.SaveChanges();
                return true;
            } catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// Gets an assignment with the specified argument ID
        /// </summary>
        public AssignmentViewModel GetAssignment(int assignmentId)
        {

            // gets the assignment with the specified ID if it exists
            var assignment = _db.Assignments.SingleOrDefault(x => x.Id == assignmentId);

            // if it doesn't exist and the query has returned null the function returns an empty viewmodel
            if (assignment == null)
                return new AssignmentViewModel();

            // if it does exist then we populate a new view model with the returned query results
            return new AssignmentViewModel
            {
                Id = assignment.Id,
                Name = assignment.Name,
                Description = assignment.Description,
                CourseInstanceId = assignment.CourseInstanceId,
                StartTime = assignment.StartTime,
                EndTime = assignment.EndTime,
                MaxCollaborators = assignment.MaxCollaborators,
                AssignmentProblems = _problemService.GetAllProblemsInAssignment(assignmentId)
            };

        }

        ///<summary>
        /// get all assignments and their problems from a course instance
        ///</summary>
        public List<AssignmentViewModel> GetAssignmentsInCourseInstance(int courseInstanceId)
        {

            // gets all assignment ids related to a course instance
            var assignmentIds = (from _assignment in _db.Assignments
                               where _assignment.CourseInstanceId == courseInstanceId
                               select _assignment.Id).ToList();

            // creates a new list of view models which is to be returned
            var assignments = new List<AssignmentViewModel>();

            // populates the list defined above with view models
            foreach (var _assignmentId in assignmentIds)
                assignments.Add(GetAssignment(_assignmentId));

            return assignments;

        }

        /// <summary>
        /// Removes all problems from the marked assignment and the then deletes it
        /// </summary>
        public bool DeleteAssignment(int assignmentId)
        {

            var assignmentToDelete = _db.Assignments.SingleOrDefault(x => x.Id == assignmentId);

            if (assignmentToDelete == null)
                return false;

            if (_problemService.RemoveProblemsFromAssignment(assignmentToDelete.Id)) {

                _db.Assignments.Remove(assignmentToDelete);

            } else {

                return false;

            }

            try
            {
                _db.SaveChanges();
                return true;
            }
            catch ( Exception e )
            {
                return false;
            }

        }

    }

}
