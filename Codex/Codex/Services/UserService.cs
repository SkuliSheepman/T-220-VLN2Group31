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
        /* Most methods here use code given in Lab 7 in Web Programming by Patrekur Patreksson */

        // The UserService uses the ApplicationDbContext in order to utilize the Identity model
        private readonly ApplicationDbContext _db;

        public UserService() {
            _db = new ApplicationDbContext();
        }

        /// <summary>
        /// Delete a user via the user's ID
        /// </summary>
        public bool DeleteUserById(string id) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var appUser = um.FindById(id);
            var result = um.Delete(appUser);

            return result.Succeeded;
        }

        /// <summary>
        /// Delete multiple users via a list of user IDs
        /// </summary>
        public bool DeleteUsersByIds(List<string> userIds) {
            foreach (var id in userIds) {
                var result = DeleteUserById(id);

                if (!result) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get all users in the database
        /// </summary>
        public List<UserHelperModel> GetAllUsers() {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var users = um.Users.ToList();

            CourseService courseService = new CourseService();

            var userModels = new List<UserHelperModel>();
            foreach (var user in users) {
                var model = new UserHelperModel() {
                    UserInfo = user,
                    IsAdmin = um.IsInRole(user.Id, "Admin"),
                    UserCourses = courseService.GetCoursesByUserId(user.Id)
                };

                userModels.Add(model);
            }

            return userModels;
        }

        /// <summary>
        /// Insert a user into the database with a given ApplicationUser variable
        /// </summary>
        public bool CreateUser(ApplicationUser user) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var idResult = um.Create(user, user.Email);
            return idResult.Succeeded;
        }

        /// <summary>
        /// Update a user with a given ApplicationUser variable
        /// </summary>
        public bool EditUser(ApplicationUser user) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));

            var completeUser = GetUserById(user.Id);

            completeUser.UserName = user.Email;
            completeUser.Email = user.Email;
            completeUser.FullName = user.FullName;

            um.Update(completeUser);

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Reset a user's password to the email address of the user via User ID
        /// </summary>
        public bool ResetPassword(string userId) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));

            var user = GetUserById(userId);

            um.RemovePassword(userId);
            um.AddPassword(userId, user.Email);

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Find out whether or not a username is available
        /// </summary>
        public bool UserExistsByUsername(string name) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return um.FindByName(name) != null;
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        public bool CreateRole(string name) {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_db));
            var idResult = rm.Create(new IdentityRole(name));
            return idResult.Succeeded;
        }

        /// <summary>
        /// Add a user to a role with a given user's ID and a role's name
        /// </summary>
        public bool AddUserToRoleByUserId(string userId, string roleName) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var idResult = um.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }

        /// <summary>
        /// Get a ApplicationUser variable from a user's ID
        /// </summary>
        public ApplicationUser GetUserById(string id) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return um.FindById(id);
        }

        /// <summary>
        /// Check if a user is in a role via user ID
        /// </summary>
        public bool IsUserInRoleByUserId(string id, string role) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return um.IsInRole(id, role);
        }

        /// <summary>
        /// Check if a user is in a role via user ID
        /// </summary>
        public bool RemoveUserFromRoleByUserId(string id, string role)
        {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            return um.RemoveFromRole(id, role).Succeeded;
        }

        // Used with User.Identity.Name
        public string GetUserIdByName(string name) {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            ApplicationUser user = new ApplicationUser();
            user = um.FindByName(name);
            //If user not empty return Id, else throw exception
            return user.Id;
        }
    }
}