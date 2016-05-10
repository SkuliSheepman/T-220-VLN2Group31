using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codex.Models.TeacherModels.HelperModels;

namespace Codex.Models.TeacherModels.ViewModels
{
    public class TeacherViewModel
    {
        public int YearSelected { get; set; }
        public string SemesterSelected { get; set; }
        public List<Tuple<int, string>> TeacherActiveSemesters { get; set; }
        public List<CourseHelperModel> TeacherCourses { get; set; }
    }
}