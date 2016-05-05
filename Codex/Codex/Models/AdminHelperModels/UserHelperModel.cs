using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.AdminHelperModels
{
    public class UserHelperModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<UserCoursesHelperModel> UserCourses { get; set; }
        public List<UserCoursesHelperModel> AvailableCourses { get; set; }
    }
    public class UserCoursesHelperModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int Semester { get; set; }
        public int Position { get; set; }
    }
}