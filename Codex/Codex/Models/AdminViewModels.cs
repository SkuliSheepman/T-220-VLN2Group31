using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Codex.Models
{
    /// <summary>
    /// The main view model for the users page
    /// </summary>
    public class AdminUsersViewModel
    {
        public AdminNewUserViewModel NewUserModel { get; set; }
        public List<AdminUserViewModel> Users { get; set; }
        public List<SelectListItem> AvailableCourses { get; set; }
    }

    /// <summary>
    /// View model for new user creation
    /// </summary>
    public class AdminNewUserViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Admin")]
        public bool Admin { get; set; }

        public List<AdminUserCoursesViewModel> UserCourses { get; set; }
    }


    /// <summary>
    /// Single user view model
    /// </summary>
    public class AdminUserViewModel
    {
        public ApplicationUser UserInfo { get; set; }
        public bool IsAdmin { get; set; }
        public List<AdminUserCoursesViewModel> UserCourses { get; set; }
    }

    // List all courses for user, previously called "UserCourseHelperModel"
    public class AdminUserCoursesViewModel
    {
        public int CourseInstanceId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int Semester { get; set; }
        public string SemesterName { get; set; }
        public int Position { get; set; }
    }


    /// <summary>
    /// Main view model for courses page
    /// </summary>
    public class AdminCoursesViewModel
    {
        public AdminNewCourseViewModel NewCourseModel { get; set; }
        public List<AdminCourseViewModel> Courses { get; set; }
    }


    /// <summary>
    /// Single course view model
    /// </summary>
    public class AdminCourseViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public int SemesterId { get; set; }
        public string Semester { get; set; }
        public int StudentsCount { get; set; }
        public List<AdminCourseTeacherViewModel> Teachers { get; set; }
    }


    /// <summary>
    /// View model for course teachers
    /// </summary>
    public class AdminCourseTeacherViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool? IsAssistant { get; set; }
    }

    /// <summary>
    /// View model for course creation
    /// </summary>
    public class AdminNewCourseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
    }


    /// <summary>
    /// View model for adding a user to a course
    /// </summary>
    public class AdminAddCourseToUserViewModel
    {
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public int Position { get; set; }
    }
}