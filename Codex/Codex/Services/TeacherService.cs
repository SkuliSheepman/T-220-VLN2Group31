using Codex.DAL;
using Codex.Models.TeacherViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Services
{
    public class TeacherService
    {
        private Database _db;
        public TeacherService()
        {
            _db = new Database();
        }
        public List<CourseViewModel> GetCoursesByUserId(string teacherId)
        {
            var teacherCoursesQuery = _db.Teachers.Where(x => x.UserId == teacherId);
            var courseList = new List<CourseViewModel>();
            foreach(var course in teacherCoursesQuery)
            {
                courseList
            }
        }
    }
}