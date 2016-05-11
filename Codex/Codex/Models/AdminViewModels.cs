using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Codex.Models
{
    // List all users View, Previously Named "UserViewModel"
    public class AdminUsersViewModel
    {
        public AdminNewUserViewModel NewUserModel { get; set; }
        public List<AdminUserViewModel> Users { get; set; }
        public List<SelectListItem> AvailableCourses { get; set; }
    }
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
    // Single user view model, listed in "UsersViewModel", previously called "UserHelperModel"
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
    // List all courses View, previously called "CourseViewModel"
    public class AdminCoursesViewModel
    {
        public AdminNewCourseViewModel NewCourseModel { get; set; }
        public List<AdminCourseViewModel> Courses { get; set; }
    }
    // Single course view model, listed in "CoursesViewModel", previously called "CourseHelperModel"
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
    // Single Teacher in Course, previously called "CourseTeacherHelperModel"
    public class AdminCourseTeacherViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool? IsAssistant { get; set; }
    }
    public class AdminNewCourseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
    }
    // Add course to user model, previously called "UserAddCourseHelperModel"
    public class AdminAddCourseToUserViewModel
    {
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public int Position { get; set; }
    }
}