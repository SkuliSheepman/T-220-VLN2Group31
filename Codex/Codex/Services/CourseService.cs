﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codex.DAL;
using Codex.Models.AdminModels.AdminViewModels;
using Codex.Models.AdminModels.AdminHelperModels;
using Codex.Models;

namespace Codex.Services
{
    public class CourseService
    {

        private Database _db;
        private UserService _userService;

        public CourseService()
        {
            _db = new Database();
            _userService = new UserService();
        }

        // <summary>
        // Create a course
        // </summary>
        public bool CreateCourse(NewCourseViewModel newCourseViewModel)
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
                return true;
            } catch ( Exception e )
            {
                return false;
            }

        }

        // <summary>
        // Get all course instances
        // </summary>
        public List<CourseHelperModel> GetAllCourseInstances()
        {

            var courseInstances = (from _courseInstance in _db.CourseInstances
                                   join _course in _db.Courses on _courseInstance.CourseId equals _course.Id
                                   join _semester in _db.Semesters on _courseInstance.SemesterId equals _semester.Id
                                   select new { _courseInstance, _course, _semester }).Select(_coursePair => new CourseHelperModel
                                   {

                                       Id = _coursePair._courseInstance.Id,
                                       Name = _coursePair._course.Name,
                                       Description = _coursePair._course.Description,
<<<<<<< HEAD
                                       Year = _coursePair._courseInstance.Year,
                                       Semester = _coursePair._courseInstance.SemesterId
=======
                                       Year        = _coursePair._courseInstance.Year,
                                       SemesterId  = _coursePair._courseInstance.SemesterId,
                                       Semester    = _coursePair._semester.Name
>>>>>>> 88b1f8bf07096874f45dbcdc772e1304dc27aa90

                                   }).ToList();

            return courseInstances;

        }

        public bool DeleteCourseById(int courseId) {

            /*var course = _db.Courses.FirstOrDefault(x => x.Id == courseId);
            var courseInstances = _db.CourseInstances.Where(x => x.CourseId == course.Id);

            foreach (var _courseInstance in courseInstances)
            {
                _db.CourseInstances.Remove(_courseInstance);
            }

            try
            {
                _db.SaveChanges();
                return true;
            } catch ( Exception e )
            {
                return false;
            }*/

            return false;

        }

        // <summary>
        // Get all students in a course instance with a given id
        // </summary>
        public List<ApplicationUser> GetAllUsersInCourseInstance(int courseInstanceId)
        {

            var users = (from _user in _db.AspNetUsers
                         where _user.CourseInstances.Any(x => x.Id == courseInstanceId)
                         select _user);

            var applicationUsers = new List<ApplicationUser>();

            foreach (var _user in users)
                applicationUsers.Add(_userService.GetUserById(_user.Id));


            return applicationUsers;
                             
        }

        // <summary>
        // Get all students in a course instance with a given id
        // </summary>
        public List<ApplicationUser> GetAllStudentsInCourseInstance(int courseInstanceId)
        {

            var students = (from _user in _db.AspNetUsers
                            where _user.CourseInstances.Any(x => x.Id == courseInstanceId)
                            select _user);

            var applicationStudents = new List<ApplicationUser>();

            foreach (var _student in students)
                applicationStudents.Add(_userService.GetUserById(_student.Id));

            return applicationStudents;

        }

        // <summary>
        // Get all students in a course instance with a given id
        // </summary>
        public bool DeleteCourseInstance(int courseInstanceId)
        {

            var courseInstance = _db.CourseInstances.FirstOrDefault(x => x.Id == courseInstanceId);

            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

    }
}