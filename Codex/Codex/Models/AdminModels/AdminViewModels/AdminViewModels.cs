using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Codex.Models.AdminModels.AdminHelperModels;

namespace Codex.Models.AdminModels.AdminViewModels
{
    public class UserViewModel
    {
        public NewUserViewModel NewUserModel { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<CourseHelperModel> AvailableCourses { get; set; }
    }

    public class NewUserViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }

        [Display(Name = "Admin")]
        public bool Admin { get; set; }
        public List<UserCoursesHelperModel> UserCourses { get; set; }
    }

    public class CourseViewModel
    {
        public NewCourseViewModel NewCourseModel { get; set; }
        public List<CourseHelperModel> Courses { get; set; }
    }

    public class NewCourseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
    }
}