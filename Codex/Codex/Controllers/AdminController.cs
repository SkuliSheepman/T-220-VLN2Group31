using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        /// <summary>
        /// Index page for admin, lists all users for easy access and editability
        /// </summary>
        public ActionResult Users() {
            List<AdminCourseViewModel> allCourses = _courseService.GetAllCourseInstances();

            List<SelectListItem> allCourseItems = new List<SelectListItem>();
            foreach (var course in allCourses) {
                allCourseItems.Add(new SelectListItem {Text = course.Name + " - " + course.Year + " - " + course.Semester, Value = course.Id.ToString()});
            }

            AdminUsersViewModel model = new AdminUsersViewModel();
            model.Users = _userService.GetAllUsers();
            model.AvailableCourses = allCourseItems;

            ViewBag.UserName = User.Identity.Name;
            return View(model);
        }

        /// <summary>
        /// Secondary view for Admin, lists all courses to be easily accessed and edited
        /// </summary>
        public ActionResult Courses() {
            CourseService courseService = new CourseService();

            AdminCoursesViewModel model = new AdminCoursesViewModel();
            model.Courses = courseService.GetAllCourseInstances();

            ViewBag.UserName = User.Identity.Name;
            return View(model);
        }

        /// <summary>
        /// Create user, action invoked by passing viewModel with ajax
        /// </summary>
        public ActionResult CreateUser(AdminNewUserViewModel newUser) {
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

        /// <summary>
        /// Create course action, invoked by passing a course view model with ajax
        /// </summary>
        /// <param name="newCourse"></param>
        /// <returns></returns>
        public ActionResult CreateCourse(AdminNewCourseViewModel newCourse) {
            if (string.IsNullOrEmpty(newCourse.Name) || newCourse.Year == null || newCourse.Year < 2000) {
                return Json(false);
            }

            return Json(_courseService.CreateCourse(newCourse));
        }

        /// <summary>
        /// Action called by ajax request, submits a list of userIds to be deleted in bulk
        /// </summary>
        public ActionResult DeleteSelectedUsers(List<string> userIds) {
            if (userIds == null || userIds.Count == 0) {
                return Json(false);
            }

            return Json(_userService.DeleteUsersByIds(userIds));
        }

        /// <summary>
        /// Action called by ajax request, submits a list of courseInstance Ids to be deleted in bulk
        /// </summary>
        public ActionResult DeleteSelectedCourses(List<int> courseInstanceIds) {
            if (courseInstanceIds == null || +courseInstanceIds.Count == 0) {
                return Json(false);
            }

            return Json(_courseService.DeleteCourseInstancesById(courseInstanceIds));
        }

        /// <summary>
        /// Edit user action, sends a user with an ajax request with the changes to be edited
        /// </summary>
        public ActionResult EditUser(ApplicationUser user) {
            return Json(_userService.EditUser(user));
        }

        /// <summary>
        /// Changepassword, resets the password for a given user sent as an ajax request
        /// </summary>
        public ActionResult ChangePassword(string userId) {
            UserService userService = new UserService();

            return Json(userService.ResetPassword(userId));
        }

        /// <summary>
        /// Removes user from course by passing a AddCourseToUserViewModel with the courseInstanceId and userId
        /// </summary>
        public ActionResult RemoveUserFromCourse(AdminAddCourseToUserViewModel model) {
            return Json(_courseService.RemoveUserFromCourse(model));
        }

        /// <summary>
        /// Adds user to course by passing a view model including the userId and the courseInstanceId
        /// </summary>
        public ActionResult AddUserToCourse(AdminAddCourseToUserViewModel model) {
            return Json(_courseService.AddUserToCourse(model));
        }

        /// <summary>
        /// Edit course, passes a Course View Model with values that are updated in the database
        /// </summary>
        public ActionResult EditCourse(AdminCourseViewModel course) {
            if (string.IsNullOrEmpty(course.Name) || course.Year == 0 || course.Year < 2000) {
                return Json(false);
            }
            return Json(_courseService.UpdateCourse(course));
        }
    }
}