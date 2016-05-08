using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.DAL;
using Codex.Models;
using Codex.Models.SharedModels.SharedViewModels;

namespace Codex.Services
{

    public class ProblemService
    {

        // <summary>
        // privatized database property
        // </summary>
        private Database _db;
        private TestCaseService _testCaseService;

        // <summary>
        // problem service constructor
        // </summary>
        public ProblemService()
        {
            _db = new Database();
            _testCaseService = new TestCaseService();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CreateProblem(ProblemCreationViewModel newProblemViewModel)
        {

            // form an entity from the view model for the database
            var newProblem = new Problem
            {
                CourseId = newProblemViewModel.CourseId,
                Name = newProblemViewModel.Name,
                Description = newProblemViewModel.Description,
                Filetype = newProblemViewModel.Filetype,
                Attachment = newProblemViewModel.Attachment,
                Language = newProblemViewModel.Language
            };

            // add the new problem
            newProblem = _db.Problems.Add(newProblem);

            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                return false;
            }

            // create test cases
            foreach (var _testCase in newProblemViewModel.testCases)
            {
                _testCase.ProblemId = newProblem.Id;
                _testCaseService.CreateTestCase(_testCase);
            }

            return true;

        }

        // <summary>
        // Gets assignment details
        // </summary>
        public ProblemViewModel GetProblem(int problemId)
        {
            var problem = _db.Problems.SingleOrDefault(x => x.Id == problemId);
            if (problem == null)
                return new ProblemViewModel();

            return new ProblemViewModel
            {
                Id = problem.Id,
                Name = problem.Name,
                Description = problem.Description,
                Filetype = problem.Filetype,
                Attachment = problem.Attachment,
                Language = problem.Language
            };
            
        }

        // <summary>
        // Gets all problems in an assignment provided with an ID
        // </summary>
        public List<ProblemViewModel> GetAllProblemsInAssignment(int assignmentId)
        {

            var problemIds = (from _relation in _db.AssignmentProblems
                              where _relation.AssignmentId == assignmentId
                              select _relation.ProblemId);

            var rtrn = new List<ProblemViewModel>();
            foreach (var _problemId in problemIds)
                rtrn.Add(GetProblem(_problemId));

            return rtrn;

        }

        /// <summary>
        /// Gets all problems related to a specific course instances
        /// </summary>
        public List<ProblemViewModel> GetAllProblemsInCourseInstance(int Id)
        {

            var problems = (from _course in _db.Courses
                            join _problem in _db.Problems on _course.Id equals _problem.CourseId
                            select _problem).Select(_problem => new ProblemViewModel
                            {

                                Id = _problem.Id,
                                Name = _problem.Name,
                                Description = _problem.Description,
                                Filetype = _problem.Filetype,
                                Attachment = _problem.Attachment,
                                Language = _problem.Language

                            }).ToList();

            return problems;

        }

        /// <summary>
        /// Removes a problem from all assignments
        /// </summary>
        public bool RemoveProblem(int problemId)
        {

            var relations = _db.AssignmentProblems.Where(x => x.ProblemId == problemId);

            foreach (var _relation in relations)
                _db.AssignmentProblems.Remove(_relation);

            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// Deletes a problem in the database that exists with the parameter Id as problem.Id
        /// </summary>
        public bool DeleteProblem(int problemId)
        {

            var problem = _db.Problems.Where(x => x.Id == problemId).SingleOrDefault();
            // Remove all relations the specified problem has with assignments
            RemoveProblem(problemId);

            if (problem != null)
                _db.Problems.Remove(problem);

            try
            {
                _db.SaveChanges();
                return true;
            } catch ( Exception e )
            {
                return false;
            }

        }

        /// <summary>
        /// Removes a problem from a specified assignment
        /// </summary>
        public bool RemoveProblemFromAssignment(int problemId, int assignmentId)
        {

            var relation = (from _relation in _db.AssignmentProblems
                            where _relation.ProblemId == problemId
                            && _relation.AssignmentId == assignmentId
                            select _relation);

            foreach (var _relation in relation)
                _db.AssignmentProblems.Remove(_relation);

            try
            {
                _db.SaveChanges();
                return true;
            } catch ( Exception e )
            {
                return false;
            }

        }

        /// <summary>
        /// Removes all problems from a specified assignment
        /// </summary>
        public bool RemoveProblemsFromAssignment(int assignmentId)
        {

            var problemIds = (from _relation in _db.AssignmentProblems
                              where _relation.AssignmentId == assignmentId
                              select _relation.ProblemId);

            // Destroy the relations
            foreach (var _problemId in problemIds)
                RemoveProblemFromAssignment(_problemId, assignmentId);

            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

    }

}