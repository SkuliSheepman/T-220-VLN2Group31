using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codex.DAL;
using Codex.Models;
using Codex.Models.AdminHelperModels;

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

            _db.CourseInstances.Add(newCourseInstance);

            try
            {

                _db.SaveChanges();

            } catch ( Exception e )
            {
                // halp
                return;
            }

        }

        public List<CourseHelperModel> GetAllCourseInstances()
        {

            var courseInstances = (from _courseInstance in _db.CourseInstances
                                   join _course in _db.Courses on _courseInstance.CourseId equals _course.Id
                                   select new { _courseInstance, _course }).Select(_coursePair => new CourseHelperModel
                                   {
                                       Id          = _coursePair._courseInstance.Id,
                                       Name        = _coursePair._course.Name,
                                       Description = _coursePair._course.Description,
                                       Year        = _coursePair._courseInstance.Year,
                                       Semester    = _coursePair._courseInstance.SemesterId

                                   }).ToList();

            return courseInstances;

        }

    }
}