using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Codex.Models.AdminViewModels
{
    // List all users View, Previously Named "UserViewModel"
    public class UsersViewModel
    {
        public NewUserViewModel NewUserModel { get; set; }
        public List<UserViewModel> Users { get; set; }
        public List<SelectListItem> AvailableCourses { get; set; }
    }
    public class NewUserViewModel
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
        public List<UserCoursesViewModel> UserCourses { get; set; }
    }
    // Single user view model, listed in "UsersViewModel", previously called "UserHelperModel"
    public class UserViewModel
    {
        public ApplicationUser UserInfo { get; set; }
        public bool IsAdmin { get; set; }
        public List<UserCoursesViewModel> UserCourses { get; set; }
    }
    // List all courses for user, previously called "UserCourseHelperModel"
    public class UserCoursesViewModel
    {
        public int CourseInstanceId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int Semester { get; set; }
        public string SemesterName { get; set; }
        public int Position { get; set; }
    }
    // List all courses View, previously called "CourseViewModel"
    public class CoursesViewModel
    {
        public NewCourseViewModel NewCourseModel { get; set; }
        public List<CourseViewModel> Courses { get; set; }
    }
    // Single course view model, listed in "CoursesViewModel", previously called "CourseHelperModel"
    public class CourseViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public int SemesterId { get; set; }
        public string Semester { get; set; }
        public int StudentsCount { get; set; }
        public List<CourseTeacherViewModel> Teachers { get; set; }
    }
    // Single Teacher in Course, previously called "CourseTeacherHelperModel"
    public class CourseTeacherViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool? IsAssistant { get; set; }
    }
    public class NewCourseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
    }
    // Add course to user model, previously called "UserAddCourseHelperModel"
    public class AddCourseToUserViewModel
    {
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public int Position { get; set; }
    }
}