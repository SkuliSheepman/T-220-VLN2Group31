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
                courseList.Add(new CourseViewModel
                {
                    Id = course.CourseInstance.Id,
                    Name = course.CourseInstance.Course.Name,
                    IsAssistant = course.IsAssistant,
                    Year = course.CourseInstance.Year,
                    Semester = course.CourseInstance.Semester.Name
                });
            }
            return courseList;
        }

        public List<ActiveSemesterViewModel> GetTeacherActiveSemestersById(string userId)
        {
            var teacherCourses = GetCoursesByUserId(userId);
            var teacherActiveSemesters = new List<ActiveSemesterViewModel>();
            foreach (var _course in teacherCourses)
            {
                var newActiveSemesterEntry = new ActiveSemesterViewModel
                {
                    Year = _course.Year,
                    Semester = _course.Semester
                };
                if (!teacherActiveSemesters.Contains(newActiveSemesterEntry))
                {
                    teacherActiveSemesters.Add(newActiveSemesterEntry);
                }
            }
            return teacherActiveSemesters;
        }

        public List<CourseViewModel> GetTeacherCoursesByDate(string userId, int year, string semester)
        {
            var teacherCourses = GetCoursesByUserId(userId);
            var datedTeacherCourses = teacherCourses.Where(x => x.Year == year && x.Semester == semester).ToList();
            return datedTeacherCourses;
        }
    }
}