using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Models;
using Codex.Services;

namespace Codex.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Users()
        {
            UserService userService = new UserService();

            UserViewModel model = new UserViewModel();
            model.Users = userService.GetAllUsers();
            return View(model);
        }

        public ActionResult Courses()
        {
            CourseViewModel model = new CourseViewModel();
            return View(model);
        }

        public ActionResult CreateUser(NewUserViewModel newUser) {
            UserService userService = new UserService();

            if (!userService.UserExistsByUsername(newUser.Email)) {
                ApplicationUser userToBeCreated = new ApplicationUser { UserName = newUser.Email, Email = newUser.Email, FullName = newUser.Name };
                if (userService.CreateUser(userToBeCreated)) {
                    if (newUser.Admin) {
                        userService.AddUserToRoleByUserId(userToBeCreated.Id, "Admin");
                    }

                    return Json(true);
                }
            }

            return Json(false);
        }

        public ActionResult CreateCourse(NewCourseViewModel newCourse) {
            CourseService courseService = new CourseService();

            courseService.CreateCourse(newCourse);

            return Json(newCourse);
        }

        public ActionResult DeleteSelectedUsers(List<string> userIds) {
            UserService userService = new UserService();

            var result = userService.DeleteUsersByIds(userIds);

            return Json(result);
        }
    }
}