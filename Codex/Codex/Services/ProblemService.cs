using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.DAL;
using Codex.Models;

namespace Codex.Services
{
    public class ProblemService
    {
        private Database _db;

        public ProblemService()
        {
            _db = new Database();
        }
        public List<ProblemViewModel> GetAllProblemsInAssignment(int Id)
        {
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
            return problems;
        }
        public void DeleteProblem(int Id)
        {
            var problem = _db.Problems.Where(x => x.Id == Id).FirstOrDefault();

            if (problem != null)
            {
                _db.Problems.Remove(problem);
            }
            _db.SaveChanges();
        }
        public List<ProblemViewModel> GetAllProblemsInCourse(int Id)
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

    }
}