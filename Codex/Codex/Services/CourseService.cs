﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codex.DAL;
using Codex.Models;

namespace Codex.Services
{
    public class CourseService
    {
        // The database connection
        private readonly Database _db;

        // The UserService instance that is used when dealing with relation tables
        // between Courses and Users
        private readonly UserService _userService;

        public CourseService() {
            _db = new Database();
            _userService = new UserService();
        }

        /// <summary>
        /// Create a course from given NewCourseViewModel
        /// </summary>
        public bool CreateCourse(AdminNewCourseViewModel newCourseViewModel) {
            var course = _db.Courses.SingleOrDefault(x => x.Name == newCourseViewModel.Name);
            Course newCourse = new Course {
                Name = newCourseViewModel.Name,
                Description = newCourseViewModel.Description
            };

            if (course == null) {
                course = _db.Courses.Add(newCourse);
            }

            CourseInstance newCourseInstance = new CourseInstance() {
                CourseId = course.Id,
                Year = newCourseViewModel.Year,
                SemesterId = newCourseViewModel.Semester
            };

            _db.CourseInstances.Add(newCourseInstance);

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Update a course with given CourseHelperModel
        /// </summary>
        public bool UpdateCourse(AdminCourseViewModel course) {
            var courseInstance = _db.CourseInstances.SingleOrDefault(x => x.Id == course.Id);
            var baseCourse = _db.Courses.SingleOrDefault(x => x.Id == courseInstance.CourseId);

            if (courseInstance != null) {
                // Check if name changed, create base course if it doesn't exist
                if (baseCourse != null && baseCourse.Name != course.Name) {
                    baseCourse = _db.Courses.SingleOrDefault(x => x.Name == course.Name);

                    if (baseCourse == null) {
                        Course newCourse = new Course() {
                            Name = course.Name
                        };

                        baseCourse = _db.Courses.Add(newCourse);
                    }

                    courseInstance.CourseId = baseCourse.Id;
                }

                courseInstance.SemesterId = course.SemesterId;
                courseInstance.Year = course.Year;
            }

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Get all teachers in a course instance by the course instance's ID
        /// </summary>
        public List<AdminCourseTeacherViewModel> GetTeachersInCourseInstanceByCourseInstanceId(int courseInstanceId) {
            var course = _db.CourseInstances.SingleOrDefault(x => x.Id == courseInstanceId);

            if (course == null) {
                return new List<AdminCourseTeacherViewModel>();
            }

            var teachers = course.Teachers.Select(teacher => new AdminCourseTeacherViewModel {
                Id = teacher.AspNetUser.Id,
                Email = teacher.AspNetUser.Email,
                IsAssistant = teacher.IsAssistant,
                Name = teacher.AspNetUser.FullName
            }).ToList();


            return teachers;
        }

        /// <summary>
        /// Get all course instances
        /// </summary>
        public List<AdminCourseViewModel> GetAllCourseInstances() {
            var courseInstances = (from _courseInstance in _db.CourseInstances
                                   join _course in _db.Courses on _courseInstance.CourseId equals _course.Id
                                   join _semester in _db.Semesters on _courseInstance.SemesterId equals _semester.Id
                                   select new {_courseInstance, _course, _semester}).Select(_coursePair => new AdminCourseViewModel {
                                       Id = _coursePair._courseInstance.Id,
                                       CourseId = _coursePair._course.Id,
                                       Name = _coursePair._course.Name,
                                       Description = _coursePair._course.Description,
                                       Year = _coursePair._courseInstance.Year,
                                       SemesterId = _coursePair._courseInstance.SemesterId,
                                       Semester = _coursePair._semester.Name,
                                       StudentsCount = _coursePair._courseInstance.AspNetUsers.Count
                                   }).ToList();

            foreach (var course in courseInstances) {
                course.Teachers = GetTeachersInCourseInstanceByCourseInstanceId(course.Id);
            }

            return courseInstances;
        }

        /// <summary>
        /// Delete a course instance via its ID
        /// </summary>
        public bool DeleteCourseInstanceById(int courseInstanceId) {
            var courseInstance = _db.CourseInstances.SingleOrDefault(x => x.Id == courseInstanceId);

            _db.CourseInstances.Remove(courseInstance);

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Delete multiple courses instances via a list of course instance IDs
        /// </summary>
        public bool DeleteCourseInstancesById(List<int> courseInstanceIds) {
            foreach (var courseInstanceId in courseInstanceIds) {
                if (!DeleteCourseInstanceById(courseInstanceId)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get all students in a course instance with a given id
        /// </summary>
        public List<ApplicationUser> GetAllStudentsInCourseInstance(int courseInstanceId) {
            var students = (from _user in _db.AspNetUsers
                            where _user.CourseInstances.Any(x => x.Id == courseInstanceId)
                            select _user);

            var applicationStudents = new List<ApplicationUser>();

            foreach (var _student in students)
                applicationStudents.Add(_userService.GetUserById(_student.Id));

            return applicationStudents;
        }

        /// <summary>
        /// Delete course instance by ID
        /// </summary>
        public bool DeleteCourseInstance(int courseInstanceId) {
            var courseInstance = _db.CourseInstances.FirstOrDefault(x => x.Id == courseInstanceId);

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Add user to course via UserAddCourseHelperModel
        /// </summary>
        public bool AddUserToCourse(AdminAddCourseToUserViewModel model) {
            var courseInstance = _db.CourseInstances.SingleOrDefault(x => x.Id == model.CourseId);
            if (courseInstance == null) {
                return false;
            }

            var user = _db.AspNetUsers.SingleOrDefault(x => x.Id == model.UserId);
            if (user == null) {
                return false;
            }

            if (model.Position == 1) {
                courseInstance.AspNetUsers.Add(user);
                AddUserToCourseAssignments(user.Id, courseInstance.Id);
            }
            else {
                Teacher teacher = new Teacher {
                    AspNetUser = user,
                    IsAssistant = model.Position != 2
                };
                courseInstance.Teachers.Add(teacher);

                if (!_userService.IsUserInRoleByUserId(user.Id, "Teacher")) {
                    _userService.AddUserToRoleByUserId(user.Id, "Teacher");
                }
            }

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Helper function for Adding user to course, adds him to all unclosed assignments in the course
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseInstanceId"></param>
        public void AddUserToCourseAssignments(string userId, int courseInstanceId) {
            var assignments = _db.Assignments.Where(x => x.CourseInstanceId == courseInstanceId && DateTime.Now < x.EndTime);
            var user = _db.AspNetUsers.SingleOrDefault(x => x.Id == userId);
            if (user == null) {}
            else {
                foreach (var assignment in assignments) {
                    AssignNewGroupForUserInAssignment(userId, assignment.Id);
                }
            }
        }

        /// <summary>
        /// Helper function for adding a user to assignments, adds him to a single assignment in course
        /// </summary>
        public void AssignNewGroupForUserInAssignment(string userId, int assignmentId) {
            var highestAssignmentProblemGroupNumber = _db.AssignmentGroups.Where(x => x.AssignmentId == assignmentId).OrderByDescending(y => y.GroupNumber).ToList();

            if (highestAssignmentProblemGroupNumber != null && highestAssignmentProblemGroupNumber.Any()) {
                var assGroup = new AssignmentGroup {
                    UserId = userId,
                    AssignmentId = assignmentId,
                    GroupNumber = highestAssignmentProblemGroupNumber[0].GroupNumber + 1
                };
                _db.AssignmentGroups.Add(assGroup);
            }
        }

        /// <summary>
        /// Get all courses a user is in via User ID
        /// </summary>
        public List<AdminUserCoursesViewModel> GetCoursesByUserId(string userId) {
            var studentCourses = (from _courseInstance in _db.CourseInstances
                                  join _course in _db.Courses on _courseInstance.CourseId equals _course.Id
                                  join _user in _db.AspNetUsers on userId equals _user.Id
                                  where _courseInstance.AspNetUsers.Contains(_user)
                                  select new {_courseInstance, _course}).Select(pair => new AdminUserCoursesViewModel {
                                      CourseInstanceId = pair._courseInstance.Id,
                                      Name = pair._course.Name,
                                      Position = 1,
                                      Semester = pair._courseInstance.SemesterId,
                                      SemesterName = pair._courseInstance.Semester.Name,
                                      Year = pair._courseInstance.Year
                                  }).ToList();

            var teacherCourses = (from _courseInstance in _db.CourseInstances
                                  join _course in _db.Courses on _courseInstance.CourseId equals _course.Id
                                  where _courseInstance.Teachers.Any(user => user.UserId == userId)
                                        && _courseInstance.Teachers.Any(user => user.IsAssistant == false)
                                  select new {_courseInstance, _course}).Select(pair => new AdminUserCoursesViewModel {
                                      CourseInstanceId = pair._courseInstance.Id,
                                      Name = pair._course.Name,
                                      Position = 2,
                                      Semester = pair._courseInstance.SemesterId,
                                      SemesterName = pair._courseInstance.Semester.Name,
                                      Year = pair._courseInstance.Year
                                  }).ToList();

            var assistantCourses = (from _courseInstance in _db.CourseInstances
                                    join _course in _db.Courses on _courseInstance.CourseId equals _course.Id
                                    where _courseInstance.Teachers.Any(user => user.UserId == userId)
                                          && _courseInstance.Teachers.Any(user => user.IsAssistant == true)
                                    select new {_courseInstance, _course}).Select(pair => new AdminUserCoursesViewModel {
                                        CourseInstanceId = pair._courseInstance.Id,
                                        Name = pair._course.Name,
                                        Position = 3,
                                        Semester = pair._courseInstance.SemesterId,
                                        SemesterName = pair._courseInstance.Semester.Name,
                                        Year = pair._courseInstance.Year
                                    }).ToList();

            var userCourses = studentCourses.Concat(teacherCourses).Concat(assistantCourses).ToList();

            return userCourses;
        }

        /// <summary>
        /// Remove a user from a course via UserAddCourseHelperModel which contains the course instance ID,
        /// the user ID and the position the user currently occupies in the course.
        /// </summary>
        public bool RemoveUserFromCourse(AdminAddCourseToUserViewModel model) {
            var courseInstance = _db.CourseInstances.SingleOrDefault(x => x.Id == model.CourseId);

            if (courseInstance == null) {
                return false;
            }

            var user = _db.AspNetUsers.SingleOrDefault(x => x.Id == model.UserId);
            if (user == null) {
                return false;
            }

            if (model.Position == 1) {
                courseInstance.AspNetUsers.Remove(user);
            }
            else {
                var teacher = courseInstance.Teachers.SingleOrDefault(x => x.AspNetUser == user && x.IsAssistant == (model.Position != 2));
                courseInstance.Teachers.Remove(teacher);

                var teacherCourses = _db.CourseInstances.SingleOrDefault(x => x.Teachers.Any(y => y.AspNetUser.Id == user.Id));

                if (teacherCourses != null && !teacherCourses.Teachers.Any()) {
                    _userService.RemoveUserFromRoleByUserId(user.Id, "Teacher");
                }
            }

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Get the name of a base course by base course ID
        /// </summary>
        public int GetCourseIdByCourseName(string name) {
            var course = _db.Courses.SingleOrDefault(x => x.Name == name);

            if (course != null) {
                return course.Id;
            }
            else {
                return 0;
            }
        }

        /// <summary>
        /// Get the ID of a base course by course instance ID
        /// </summary>
        public int GetCourseIdByCourseCourseInstanceId(int id) {
            var course = _db.CourseInstances.SingleOrDefault(x => x.Id == id);

            if (course != null) {
                return course.CourseId;
            }
            else {
                return 0;
            }
        }
    }
}