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
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserService _userService;
        private readonly CourseService _courseService;

        public AdminController() {
            _userService = new UserService();
            _courseService = new CourseService();
        }

        public ActionResult Users() {
            List<CourseHelperModel> allCourses = _courseService.GetAllCourseInstances();

            List<SelectListItem> allCourseItems = new List<SelectListItem>();
            foreach (var course in allCourses) {
                allCourseItems.Add(new SelectListItem {Text = course.Name + " - " + course.Year + " - " + course.Semester, Value = course.Id.ToString()});
            }

            UserViewModel model = new UserViewModel();
            model.Users = _userService.GetAllUsers();
            model.AvailableCourses = allCourseItems;

            ViewBag.UserName = User.Identity.Name;
            return View(model);
        }

        public ActionResult Courses() {
            CourseService courseService = new CourseService();

            CourseViewModel model = new CourseViewModel();
            model.Courses = courseService.GetAllCourseInstances();

            ViewBag.UserName = User.Identity.Name;
            return View(model);
        }

        public ActionResult CreateUser(NewUserViewModel newUser) {
            if (newUser.Name == null && newUser.Email == null) {
                return Json(false);
            }

            if (!_userService.UserExistsByUsername(newUser.Email)) {
                ApplicationUser userToBeCreated = new ApplicationUser {UserName = newUser.Email, Email = newUser.Email, FullName = newUser.Name};
                if (_userService.CreateUser(userToBeCreated)) {
                    if (newUser.Admin) {
                        _userService.AddUserToRoleByUserId(userToBeCreated.Id, "Admin");
                    }

                    return Json(true);
                }
            }

            return Json(false);
        }

        public ActionResult CreateCourse(NewCourseViewModel newCourse) {
            if (string.IsNullOrEmpty(newCourse.Name) || newCourse.Year == null || newCourse.Year < 2000) {
                return Json(false);
            }

            return Json(_courseService.CreateCourse(newCourse));
        }

        public ActionResult DeleteSelectedUsers(List<string> userIds) {
            if (userIds == null || userIds.Count == 0) {
                return Json(false);
            }

            return Json(_userService.DeleteUsersByIds(userIds));
        }

        public ActionResult DeleteSelectedCourses(List<int> courseInstanceIds) {
            if (courseInstanceIds == null || courseInstanceIds.Count == 0) {
                return Json(false);
            }

            return Json(_courseService.DeleteCourseInstancesById(courseInstanceIds));
        }

        public ActionResult EditUser(ApplicationUser user) {
            return Json(_userService.EditUser(user));
        }

        public ActionResult ChangePassword(string userId) {
            UserService userService = new UserService();

            return Json(userService.ResetPassword(userId));
        }

        public ActionResult RemoveUserFromCourse(UserAddCourseHelperModel model) {
            return Json(_courseService.RemoveUserFromCourse(model));
        }

        public ActionResult AddUserToCourse(UserAddCourseHelperModel model) {
            return Json(_courseService.AddUserToCourse(model));
        }

        public ActionResult EditCourse(CourseHelperModel course) {
            if (string.IsNullOrEmpty(course.Name) || course.Year == null || course.Year < 2000) {
                return Json(false);
            }
            return Json(_courseService.UpdateCourse(course));
        }
    }
}