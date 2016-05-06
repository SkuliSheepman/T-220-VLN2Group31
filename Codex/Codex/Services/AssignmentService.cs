using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.DAL;
using Codex.Models;
using Codex.Services;

namespace Codex.Services
{

    public class AssignmentService
    {

        private Database _db;

        public AssignmentService()
        {

            _db = new Database();

        }

        /// <summary>
        /// creates an assignment
        /// </summary>
        public void CreateAssignment(AssignmentCreationViewModel nu)
        {


        }
        /*
        public List<UserViewModel> GetAssignmentCollaborators(int Id)
        {
            var collabrators = (from _assignment in _db.Assignments
                                join _relation in _db.AssignmentGroups on _assignment.Id equals _relation.AssignmentId
                                join _users in _db.AspNetUsers on _relation.UserId equals _users.Id
                                select _users).Select(_users => new UserViewModel
                                {
                                    
                                })
        }
        */
        /// <summary>
        /// gets an assignment and it's related problems by assignment id
        /// </summary>
        public AssignmentViewModel GetAssignmentById(int Id)
        {

            var assignment = _db.Assignments.SingleOrDefault(x => x.Id == Id);
            if (assignment == null)
            {

                // throw something :D

            }

            var problems = (from _assignment in _db.Assignments
                            join _relation in _db.AssignmentProblems on _assignment.Id equals _relation.AssignmentId
                            join _problem in _db.Problems on _relation.ProblemId equals _problem.Id
                            select _problem).Select(_problem => new ProblemViewModel
                            {

                                Id = _problem.Id,
                                Name = _problem.Name,
                                Description = _problem.Description,
                                Filetype = _problem.Filetype,
                                Attachment = _problem.Attachment,
                                Language = _problem.Language

                            }).ToList();

            var viewModel = new AssignmentViewModel
            {

                Id = assignment.Id,
                AssignmentProblems = problems

            };

            return viewModel;

        }

        ///<summary>
        /// get all assignments and their problems from a course instance
        ///</summary>
        public List<AssignmentViewModel> GetAssignmentsInCourseInstance(int Id)
        {
            ProblemService prob = new ProblemService();


            var assignments = (from _courseInstance in _db.CourseInstances
                               join _assignment in _db.Assignments on _courseInstance.Id equals _assignment.Id
                               select _assignment).Select(_assignment => new AssignmentViewModel
                               {

                                   Id = _assignment.Id,
                                   CourseInstanceId = _assignment.CourseInstanceId,
                                   Name = _assignment.Name,
                                   Description = _assignment.Description,
                                   AssignmentProblems = prob.GetAllProblemsInAssignment(Id),
                                   AssignmentCollaborators = GetAssignmentCollaborators(Id);



        }).ToList();
            return assignments;
        

    /// <summary>
    /// Removes all problems from the marked assignment and the then deletes it
    /// </summary>
    public void DeleteAssignmentById(int Id)
    {
        ProblemService prob = new ProblemService();
        prob.RemoveProblemsFromAssignment(Id);

        var assignment = _db.Assignments.Where(x => x.Id == Id).FirstOrDefault();

        if (assignment != null)
        {
            _db.Assignments.Remove(assignment);
        }
        try
        {
            _db.SaveChanges();
        }
        catch
        {
            // Some Error message
        }
    }

   }
}
