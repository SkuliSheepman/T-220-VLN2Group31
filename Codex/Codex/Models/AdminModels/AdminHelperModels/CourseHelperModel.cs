using Codex.Models.SharedModels.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.AdminModels.AdminHelperModels
{
    public class CourseHelperModel : TeacherCourseViewModel
    {
        public int StudentsCount { get; set; }
        public List<CourseTeacherHelperModel> Teachers { get; set; }
    }

    public class CourseTeacherHelperModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool? IsAssistant { get; set; }
    }
}