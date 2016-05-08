using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.AdminModels.AdminHelperModels
{
    public class UserHelperModel
    {
        public ApplicationUser UserInfo { get; set; }
        public List<UserCoursesHelperModel> UserCourses { get; set; }
    }

    public class UserCoursesHelperModel
    {
        public int CourseInstanceId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int Semester { get; set; }
        public int Position { get; set; }
    }

    public class UserAddCourseHelperModel
    {
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public int Position { get; set; }
    }
}