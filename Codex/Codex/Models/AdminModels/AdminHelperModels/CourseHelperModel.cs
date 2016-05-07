using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.AdminModels.AdminHelperModels
{
    public class CourseHelperModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public int SemesterId { get; set; }
        public string Semester { get; set; }
        public int StudentsCount { get; set; }
        public List<CourseTeacherHelperModel> Teachers { get; set; }

        public class CourseTeacherHelperModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public bool IsAssistant { get; set; }
        }
    }
}