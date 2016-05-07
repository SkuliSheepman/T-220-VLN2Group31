using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Models.AdminModels.AdminViewModels;
using Codex.Models.AdminModels.AdminHelperModels;
using Codex.Services;
using Codex.Models;

namespace Codex.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Users()
        {
            UserService userService = new UserService();
            CourseService courseService = new CourseService();

            UserViewModel model = new UserViewModel();
            model.Users = userService.GetAllUsers();
            model.AvailableCourses = courseService.GetAllCourseInstances();

            return View(model);
        }

        public ActionResult Courses()
        {
            CourseService courseService = new CourseService();

            CourseViewModel model = new CourseViewModel();
            model.Courses = courseService.GetAllCourseInstances();

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
            if (userIds == null || userIds.Count == 0) {
                return Json(false);
            }

            UserService userService = new UserService();

            var result = userService.DeleteUsersByIds(userIds);

            return Json(result);
        }

        public ActionResult EditUser(ApplicationUser user) {
            return null;
        }
    }
}