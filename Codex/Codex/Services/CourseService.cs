using System;
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
        public bool CreateCourse(NewCourseViewModel newCourseViewModel) {
            var _course = _db.Courses.SingleOrDefault(x => x.Name == newCourseViewModel.Name);
            Course newCourse = new Course() {
                Name = newCourseViewModel.Name,
                Description = newCourseViewModel.Description
            };

            if (_course == null) {
                _course = _db.Courses.Add(newCourse);
            }

            CourseInstance newCourseInstance = new CourseInstance() {
                CourseId = _course.Id,
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
        public bool UpdateCourse(CourseHelperModel course) {
            var courseInstance = _db.CourseInstances.SingleOrDefault(x => x.Id == course.Id);
            var baseCourse = _db.Courses.SingleOrDefault(x => x.Id == courseInstance.CourseId);

            if (courseInstance != null) {
                // Check if name changed, create base course if it doesn't exist
                if (baseCourse != null && baseCourse.Name != course.Name) {
                    baseCourse = _db.Courses.SingleOrDefault(x => x.Name == course.Name);

                    if (baseCourse == null) {
                        Course newCourse = new Course() {
                            Name = course.Name,
                            Description = course.Description
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
        public List<CourseTeacherHelperModel> GetTeachersInCourseInstanceByCourseInstanceId(int courseInstanceId) {
            var course = _db.CourseInstances.SingleOrDefault(x => x.Id == courseInstanceId);

            if (course == null) {
                return new List<CourseTeacherHelperModel>();
            }

            var teachers = course.Teachers.Select(teacher => new CourseTeacherHelperModel {
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
        public List<CourseHelperModel> GetAllCourseInstances() {
            var courseInstances = (from _courseInstance in _db.CourseInstances
                                   join _course in _db.Courses on _courseInstance.CourseId equals _course.Id
                                   join _semester in _db.Semesters on _courseInstance.SemesterId equals _semester.Id
                                   select new {_courseInstance, _course, _semester}).Select(_coursePair => new CourseHelperModel {
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
        public bool AddUserToCourse(UserAddCourseHelperModel model) {
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
        /// Get all courses a user is in via User ID
        /// </summary>
        public List<UserCoursesHelperModel> GetCoursesByUserId(string userId) {
            var studentCourses = (from _courseInstance in _db.CourseInstances
                                  join _course in _db.Courses on _courseInstance.CourseId equals _course.Id
                                  join _user in _db.AspNetUsers on userId equals _user.Id
                                  where _courseInstance.AspNetUsers.Contains(_user)
                                  select new {_courseInstance, _course}).Select(pair => new UserCoursesHelperModel {
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
                                  select new {_courseInstance, _course}).Select(pair => new UserCoursesHelperModel {
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
                                    select new {_courseInstance, _course}).Select(pair => new UserCoursesHelperModel {
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
        public bool RemoveUserFromCourse(UserAddCourseHelperModel model) {
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

        public List<Codex.Models.TeacherModels.HelperModels.CourseHelperModel> GetTeacherCoursesByDate(string teacherName, int year, string semester) {
            var teacherId = _userService.GetUserIdByName(teacherName);
            var userCourses = GetCoursesByUserId(teacherId);
            var teacherCourses = new List<Codex.Models.TeacherModels.HelperModels.CourseHelperModel>();

            foreach (var course in userCourses) {
                if (course.Year == year && course.SemesterName == semester) {
                    if (course.Position != 1) {
                        var teacherCourseHelperModel = new Codex.Models.TeacherModels.HelperModels.CourseHelperModel {
                            Id = course.CourseInstanceId,
                            Year = course.Year,
                            Semester = course.SemesterName,
                            Name = course.Name
                        };

                        if (!teacherCourses.Contains(teacherCourseHelperModel)) {
                            teacherCourses.Add(teacherCourseHelperModel);
                        }
                    }
                }
            }

            return teacherCourses;
        }
    }
}