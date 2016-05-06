using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codex.Models.AdminHelperModels;

namespace Codex.Models
{
    public class UserViewModel
    {
        public NewUserViewModel NewUserModel { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<UserCoursesHelperModel> AvailableCourses { get; set; }
    }

    public class NewUserViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
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