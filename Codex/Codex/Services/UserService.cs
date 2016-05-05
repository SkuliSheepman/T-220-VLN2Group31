using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codex.DAL;
using Codex.Models;

namespace Codex.Services
{
    public class UserService
    {

        private Database _db;

        public UserService()
        {

            _db = new Database();

        }

        public void CreateUser(NewUserViewModel newUserViewModel)
        {

        }

    }
}