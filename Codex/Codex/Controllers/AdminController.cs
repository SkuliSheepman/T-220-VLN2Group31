﻿using System;
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
            UserViewModel model = new UserViewModel();
            return View(model);
        }

        public ActionResult Courses()
        {
            CourseViewModel model = new CourseViewModel();
            return View(model);
        }

        public ActionResult CreateUser(NewUserViewModel newUser) {
            UserService userService = new UserService();

            if (!userService.UserExists(newUser.Email)) {
                ApplicationUser userToBeCreated = new ApplicationUser { UserName = newUser.Email, Email = newUser.Email, FullName = newUser.Name };
                return Json(userService.CreateUser(userToBeCreated));
            }

            return Json(false);
        }

        public ActionResult CreateCourse(NewCourseViewModel newCourse) {
            CourseService courseService = new CourseService();

            courseService.CreateCourse(newCourse);

            return Json(newCourse);
        }
    }
}