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

        public List<ProblemViewModel> GetAllProblemInCourse(int courseId)
        {
            var problem = 

            return 
        }
    }
}