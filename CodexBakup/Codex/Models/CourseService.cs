using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codex.DAL;
using Codex.Models;

namespace Codex.Models
{
    public class CourseService
    {

        private Database _db;

        public CourseService()
        {
            _db = new Database();
        }

        public void CreateCourse(NewCourseViewModel newCourseViewModel)
        {

            var _course = _db.Courses.SingleOrDefault(x => x.Name == newCourseViewModel.Name);
            Course newCourse = new Course()
            {
                Name = newCourseViewModel.Name,
                Description = newCourseViewModel.Description
            };

            if (_course == null)
                _course = _db.Courses.Add(newCourse);

            CourseInstance newCourseInstance = new CourseInstance()
            {
                CourseId   = _course.Id,
                Year       = newCourseViewModel.Year,
                SemesterId = newCourseViewModel.Semester
            };

            try
            {

                _db.SaveChanges();

            } catch ( Exception e )
            {
                // halp
                return;
            }

        }

    }
}