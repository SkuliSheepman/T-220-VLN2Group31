using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codex.DAL;
namespace Codex.Services
{
    public class TeacherService
    {

        private Database _db;

        public TeacherService()
        {

            _db = new Database();

        }

        /// <summary>
        /// 
        /// </summary>
        /*public List<int> GetYearsTought(string userId)
        {

            var user = _db.AspNetUsers.Single(x => x.Id == userId);


        }*/

    }
}