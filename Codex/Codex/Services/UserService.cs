using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Controllers;
using Codex.DAL;
using Codex.Models;
using Codex.Models.AdminModels.AdminHelperModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Codex.Services
{
    public class UserService
    {
        private ApplicationDbContext _db;

        public UserService() {
            _db = new ApplicationDbContext();
        }

        public bool DeleteUserById(string id) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var appUser = um.FindById(id);
            var result = um.Delete(appUser);

            return result.Succeeded;
        }

        public bool DeleteUsersByIds(List<string> userIds) {
            foreach (var id in userIds) {
                var result = DeleteUserById(id);

                if (!result) {
                    return false;
                }
            }

            return true;
        }

        public List<ApplicationUser> GetAllUsers() {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return um.Users.ToList();
        }

        /* The methods CreateUser, GetUser, UserExists, CreateRole and AddUserToRole
         * are methods given in Lab 7 in Web Programming by Patrekur Patreksson
         */

        public bool CreateUser(ApplicationUser user) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var idResult = um.Create(user, user.Email);
            return idResult.Succeeded;
        }

        public bool UserExists(string name) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return um.FindByName(name) != null;
        }

        public bool CreateRole(string name) {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_db));
            var idResult = rm.Create(new IdentityRole(name));
            return idResult.Succeeded;
        }

        public bool AddUserToRole(string userId, string roleName) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var idResult = um.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }

        public ApplicationUser GetUser(string name) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return um.FindByName(name);
        }
    }
}