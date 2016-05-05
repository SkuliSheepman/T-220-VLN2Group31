using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.DAL;
using Codex.Models;

namespace Codex.Services
{

    public class AssignmentService
    {

        private Database _db;

        public AssignmentService() {

            _db = new Database();

        }

        // <summary>
        // creates an assignment
        // </summary>
        public void CreateAssignment(AssignmentCreationViewModel nu) {


        }

        // <summary>
        // gets an assignment and it's related problems by assignment id
        // </summary>
        public AssignmentViewModel GetAssignmentById(int Id)
        {

            var assignment = _db.Assignments.SingleOrDefault(x => x.Id == Id);
            if (assignment == null) {

                // throw something :D

            }

            var problems = (from _assignment in _db.Assignments
                            join _relation in _db.AssignmentProblems on _assignment.Id equals _relation.AssignmentId
                            join _problem in _db.Problems on _relation.ProblemId equals _problem.Id
                            select _problem).Select(_problem => new ProblemViewModel
                            {

                                Id          = _problem.Id,
                                Name        = _problem.Name,
                                Description = _problem.Description,
                                Filetype    = _problem.Filetype,
                                Attachment  = _problem.Attachment,
                                Language    = _problem.Language

                            }).ToList();

            var viewModel = new AssignmentViewModel
            {

                Id = assignment.Id,
                AssignmentProblems = problems

            };

            return viewModel;

        }

    }
}