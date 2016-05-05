using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Controllers;
using Codex.DAL;
using Codex.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Codex.Services
{
    public class UserService
    {

        private Database _db;

        public UserService()
        {

            _db = new Database();

        }

        public bool CreateUser(NewUserViewModel newUserViewModel) {
            /*if (!(_db.AspNetUsers.Any(u => u.UserName == newUserViewModel.Email))) {
                var userStore = new UserStore<ApplicationUser>(_db);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var newUser = new ApplicationUser { UserName = newUserViewModel.Email, Email = newUserViewModel.Email, FullName = newUserViewModel.Name };
                var result = userManager.Create(newUser, newUserViewModel.Email);

                if (result.Succeeded) {
                    return true;
                }
            }*/

            return false;
        }

    }
}