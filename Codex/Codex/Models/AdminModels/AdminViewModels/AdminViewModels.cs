using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codex.Models.AdminModels.AdminHelperModels;

namespace Codex.Models.AdminModels.AdminViewModels
{
    public class UserViewModel
    {
        public NewUserViewModel NewUserModel { get; set; }
        public List<UserHelperModel> Users { get; set; }
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