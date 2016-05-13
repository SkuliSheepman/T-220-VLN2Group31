using Codex.DAL;
using Codex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Services
{
    public class TeacherService
    {
        private readonly Database _db;
        private readonly CourseService _courseService;

        public TeacherService() {
            _db = new Database();
            _courseService = new CourseService();
        }

        public List<TeacherCourseViewModel> GetCoursesByUserId(string teacherId) {
            var teacherCoursesQuery = _db.Teachers.Where(x => x.UserId == teacherId);
            var courseList = new List<TeacherCourseViewModel>();
            foreach (var course in teacherCoursesQuery) {
                courseList.Add(new TeacherCourseViewModel {
                    Id = course.CourseInstance.Id,
                    Name = course.CourseInstance.Course.Name,
                    IsAssistant = course.IsAssistant,
                    Year = course.CourseInstance.Year,
                    Semester = course.CourseInstance.Semester.Name
                });
            }
            return courseList;
        }

        /// <summary>
        /// Get a list of semesters the teacher is teaching, past, present and future, by user ID
        /// </summary>
        public List<TeacherActiveSemesterViewModel> GetTeacherActiveSemestersById(string userId) {
            var teacherCourses = GetCoursesByUserId(userId);
            var teacherActiveSemesters = new List<TeacherActiveSemesterViewModel>();
            foreach (var course in teacherCourses) {
                var newActiveSemesterEntry = new TeacherActiveSemesterViewModel {
                    Year = course.Year,
                    Semester = course.Semester
                };
                if (!teacherActiveSemesters.Contains(newActiveSemesterEntry)) {
                    teacherActiveSemesters.Add(newActiveSemesterEntry);
                }
            }
            return teacherActiveSemesters.OrderByDescending(x => x.Year).ThenByDescending(y => y.Semester).ToList();
        }

        /// <summary>
        /// Get a TeacherActiveSemesterViewModel from a list of TeacherActiveSemesterViewModel that represents the
        /// semester closest to the current date
        /// </summary>
        public TeacherActiveSemesterViewModel GetClosestSemester(List<TeacherActiveSemesterViewModel> semesterList) {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            string currentSemester = "Summer";
            if (1 <= currentMonth && currentMonth <= 5) {
                currentSemester = "Spring";
            }
            else if (8 <= currentMonth && currentMonth <= 12) {
                currentSemester = "Fall";
            }

            var closestSemester = semesterList.SingleOrDefault(x => x.Year == DateTime.Now.Year && x.Semester == currentSemester);

            // If current semester is the closest semester
            if (closestSemester != null) {
                return closestSemester;
            }
            else {
                // Filter the list to only future courses
                var filteredList = semesterList.Where(x => x.Year <= DateTime.Now.Year).OrderBy(y => y.Year).ThenBy(z => z.Semester).ToList();

                // No future courses found, return the latest course
                if (!filteredList.Any()) {
                    return semesterList.First();
                }
                // More than 1 future semesters
                /*else if (1 < filteredList.Count) {
                    // TODO
                }*/
                // Only 1 future semester
                else {
                    return filteredList.First();
                }
            }
        }

        public List<TeacherCourseViewModel> GetTeacherCoursesByDate(string userId, int year, string semester) {
            var teacherCourses = GetCoursesByUserId(userId);
            var datedTeacherCourses = teacherCourses.Where(x => x.Year == year && x.Semester == semester).ToList();
            return datedTeacherCourses;
        }

        public List<TeacherAssignmentViewModel> GetAssignmentsInCourseInstanceById(int courseInstanceId) {
            var course = _db.CourseInstances.SingleOrDefault(x => x.Id == courseInstanceId);
            var assignments = course.Assignments.Select(assignment => new TeacherAssignmentViewModel {
                Id = assignment.Id,
                Name = assignment.Name,
                Course = assignment.CourseInstance.Course.Name,
                StartTime = assignment.StartTime,
                EndTime = assignment.EndTime,
                MaxCollaborators = assignment.MaxCollaborators
            }).ToList();

            return assignments;
        }

        public List<TeacherProblemViewModel> GetProblemsInAssignmentById(int assignmentId) {
            var problemQuery = _db.AssignmentProblems.Where(x => x.AssignmentId == assignmentId);
            var problemList = new List<TeacherProblemViewModel>();
            foreach (var problem in problemQuery) {
                problemList.Add(new TeacherProblemViewModel {
                    Id = problem.Problem.Id,
                    Name = problem.Problem.Name,
                    Weight = problem.Weight
                });
            }
            return problemList;
        }

        public List<TeacherAssignmentGroupViewModel> GetAssignmentGroups(int assignmentId) {
            var groupQuery = _db.AssignmentGroups.Where(x => x.AssignmentId == assignmentId);
            var groupNumberSet = new HashSet<int>();
            var assignmentGroupList = new List<TeacherAssignmentGroupViewModel>();
            foreach (var student in groupQuery) {
                groupNumberSet.Add(student.GroupNumber);
            }
            foreach (var group in groupNumberSet) {
                var assignmentGroup = new TeacherAssignmentGroupViewModel {
                    GroupNumber = group,
                    Names = new List<string>(),
                    Submissions = new List<TeacherSubmissionViewModel>(),
                    StudentIds = new List<string>()
                };
                var studentsGroupQuery = _db.AssignmentGroups.Where(x => x.GroupNumber == group && x.AssignmentId == assignmentId);
                foreach (var student in studentsGroupQuery) {
                    assignmentGroup.StudentIds.Add(student.UserId);
                    assignmentGroup.Names.Add(student.AspNetUser.FullName);
                }

                foreach (var studentId in assignmentGroup.StudentIds) {
                    var submissions = _db.Submissions.Where(x => x.StudentId == studentId && x.AssignmentId == assignmentId);

                    if (submissions.Any()) {
                        foreach (var submission in submissions) {
                            var model = new TeacherSubmissionViewModel {
                                Id = submission.Id,
                                SubmissionTime = submission.Time,
                                OriginalFilename = submission.OriginalFileName
                            };

                            if (submission.FailedTests.HasValue) {
                                model.FailedTests = submission.FailedTests.Value;
                            }

                            if (submission.SubmissionGrade != null) {
                                model.SubmissionGrade = submission.SubmissionGrade.Grade;
                            }

                            assignmentGroup.Submissions.Add(model);
                        }
                    }
                }

                // Get best submission
                assignmentGroup.BestSubmission = GetBestSubmissionFromSubmissionList(assignmentGroup.Submissions);

                assignmentGroupList.Add(assignmentGroup);
            }
            return assignmentGroupList;
        }

        public List<TeacherSubmissionViewModel> GetSubmissionsFromGroupByStudentIds(List<string> studentIds, int assignmentId, int problemId) {
            var submissionList = new List<TeacherSubmissionViewModel>();
            foreach (var studentId in studentIds) {
                var submissionQuery = _db.Submissions.Where(x => x.StudentId == studentId && x.AssignmentId == assignmentId && x.ProblemId == problemId);
                foreach (var submission in submissionQuery) {
                    submissionList.Add(new TeacherSubmissionViewModel {
                        Id = submission.Id,
                        StudentName = submission.AspNetUser.FullName,
                        SubmissionTime = submission.Time,
                        FailedTests = submission.FailedTests,
                        SubmissionGrade = submission.SubmissionGrade.Grade
                    });
                }
            }
            return submissionList;
        }

        // Gets submission with fewest FailedTests and Newest SubmissionTime
        public TeacherSubmissionViewModel GetBestSubmissionFromSubmissionList(List<TeacherSubmissionViewModel> submissions) {
            var bestSubmission = new TeacherSubmissionViewModel();
            foreach (var submission in submissions) {
                if (submission.FailedTests < bestSubmission.FailedTests) {
                    bestSubmission = submission;
                }
                else if (bestSubmission.FailedTests == submission.FailedTests && bestSubmission.SubmissionTime < submission.SubmissionTime) {
                    bestSubmission = submission;
                }
            }
            return bestSubmission;
        }

        public List<TeacherAssignmentViewModel> GetOpenAssignmentsFromList(List<TeacherAssignmentViewModel> assignments) {
            var openAssignments = assignments
                .Where(
                    x => x.StartTime < DateTime.Now
                         && DateTime.Now < x.EndTime)
                .ToList();

            return openAssignments;
        }

        public List<TeacherAssignmentViewModel> GetUpcomingAssignmentsFromList(List<TeacherAssignmentViewModel> assignments) {
            var upcomingAssignments = assignments
                .Where(
                    x => DateTime.Now < x.StartTime)
                .ToList();

            return upcomingAssignments;
        }

        public List<TeacherAssignmentViewModel> GetRequiresGradingAssignmentsFromList(List<TeacherAssignmentViewModel> assignments) {
            var notGradedAssignments = assignments
                .Where(
                    x => x.EndTime < DateTime.Now && x.IsGraded == false)
                .ToList();
            return notGradedAssignments;
        }

        public List<TeacherAssignmentViewModel> GetClosedAssignmentsFromList(List<TeacherAssignmentViewModel> assignments) {
            var closedAssignments = assignments
                .Where(
                    x => x.EndTime < DateTime.Now && x.IsGraded)
                .ToList();
            return closedAssignments;
        }

        public TeacherProblemUpdateViewModel UpdateProblem(TeacherProblemUpdateViewModel problemViewModel) {
            var problemExists = _db.Problems.SingleOrDefault(x => x.Id == problemViewModel.Id);
            /*var problem = new Problem
            {
                CourseId = problemViewModel.CourseId,
                Name = problemViewModel.Name,
                Description = problemViewModel.Description,
                Filetype = problemViewModel.Filetype,
                Attachment = problemViewModel.Attachment,
                Language = problemViewModel.Language
            };*/

            var problem = new Problem {
                CourseId = problemViewModel.CourseId,
                Name = problemViewModel.Name,
                Description = problemViewModel.Description,
                Filetype = problemViewModel.Filetype,
                Attachment = problemViewModel.AttachmentName,
                Language = problemViewModel.Language
            };

            if (problemExists != null) {
                problem.Id = problemViewModel.Id;
                problemExists = problem;
            }
            else {
                _db.Problems.Add(problem);
            }

            _db.SaveChanges();
            return problemViewModel;
        }

        /// <summary>
        /// Create a new problem via TeacherNewProblemViewModel
        /// </summary>
        public int CreateNewProblem(TeacherNewProblemViewModel problem) {
            var courseId = _courseService.GetCourseIdByCourseName(problem.CourseName);

            // Filetype check
            if (_db.Filetypes.SingleOrDefault(x => x.Type == problem.Filetype) == null) {
                var fileType = new Filetype {
                    Type = problem.Filetype
                };

                _db.Filetypes.Add(fileType);
            }

            // Language check
            if (_db.ProgrammingLanguages.SingleOrDefault(x => x.Language == problem.Language) == null) {
                var language = new ProgrammingLanguage {
                    Language = problem.Filetype
                };

                _db.ProgrammingLanguages.Add(language);
            }

            // Add the new problem
            var newProblem = new Problem {
                CourseId = courseId,
                Name = problem.Name,
                Description = problem.Description,
                Filetype = problem.Filetype,
                Language = problem.Language
            };

            _db.Problems.Add(newProblem);

            try {
                _db.SaveChanges();
                return newProblem.Id;
            }
            catch (Exception e) {
                return 0;
            }
        }

        public bool CreateNewAssignment(TeacherCreateAssignmentViewModel assignment) {
            var newAssignment = new Assignment {
                CourseInstanceId = assignment.CourseInstanceId,
                Description = assignment.Description,
                EndTime = DateTime.Parse(assignment.EndTime),
                StartTime = DateTime.Parse(assignment.StartTime),
                Name = assignment.Name,
                MaxCollaborators = assignment.MaxCollaborators,
                IsGraded = false
            };

            newAssignment = _db.Assignments.Add(newAssignment);

            foreach (var assignmentProblem in assignment.Problems) {
                var problem = _db.Problems.SingleOrDefault(x => x.Id == assignmentProblem.ProblemId);

                if (problem == null) {
                    continue;
                }

                var newAssignmentProblem = new AssignmentProblem {
                    ProblemId = problem.Id,
                    AssignmentId = newAssignment.Id,
                    Weight = (byte)assignmentProblem.Weight,
                    MaxSubmissions = assignmentProblem.MaxSubmissions
                };

                _db.AssignmentProblems.Add(newAssignmentProblem);
            }

            var students = _courseService.GetAllStudentsInCourseInstance(newAssignment.CourseInstanceId);

            // Create groups for students
            var count = 1;
            foreach (var student in students) {
                _db.AssignmentGroups.Add(new AssignmentGroup {
                    UserId = student.Id,
                    AssignmentId = newAssignment.Id,
                    GroupNumber = count
                });
                count++;
            }

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        public bool SetTestCasesForProblemByProblemId(int problemId, List<TeacherTestCaseViewModel> testCases) {
            foreach (var testCase in testCases) {
                var newTestCase = new TestCase {
                    ProblemId = problemId,
                    ExpectedOutput = testCase.Output,
                    Input = testCase.Input
                };

                _db.TestCases.Add(newTestCase);
            }

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        public bool SetAttachmentToProblemInDatabaseByProblemId(int problemId, string attachmentName) {
            var problem = _db.Problems.SingleOrDefault(x => x.Id == problemId);

            if (problem != null) {
                problem.Attachment = attachmentName;

                try {
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception e) {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Get all problems in a base course via a course instance Id.
        /// </summary>
        public List<TeacherProblemUpdateViewModel> GetProblemsInCourseById(int courseInstanceId) {
            var problemList = new List<TeacherProblemUpdateViewModel>();

            var courseId = _courseService.GetCourseIdByCourseCourseInstanceId(courseInstanceId);

            if (courseId != 0) {
                var problems = _db.Problems.Where(x => x.CourseId == courseId);

                foreach (var problem in problems) {
                    var p = new TeacherProblemUpdateViewModel {
                        Id = problem.Id,
                        Description = problem.Description,
                        Name = problem.Name,
                        AttachmentName = problem.Attachment,
                        CourseId = problem.CourseId,
                        Filetype = problem.Filetype,
                        Language = problem.Language,
                        TestCases = new List<TeacherTestCaseViewModel>()
                    };

                    foreach (var testCase in problem.TestCases) {
                        var t = new TeacherTestCaseViewModel {
                            Input = testCase.Input,
                            Output = testCase.ExpectedOutput
                        };

                        p.TestCases.Add(t);
                    }

                    problemList.Add(p);
                }
            }

            return problemList;
        }

        /// <summary>
        /// Delete an assignment from the database via it's Id
        /// </summary>
        public bool DeleteAssignmentById(int assignmentId) {
            var assignment = _db.Assignments.SingleOrDefault(x => x.Id == assignmentId);

            if (assignment != null) {
                // Remove the problems from the assignment first

                foreach (var assignmentProblem in _db.AssignmentProblems.Where(x => x.AssignmentId == assignmentId)) {
                    RemoveProblemFromAssignmentByIds(assignmentId, assignmentProblem.ProblemId);
                }

                _db.Assignments.Remove(assignment);

                // Remove all submissions for the assignment

                foreach(var submission in _db.Submissions.Where(x => x.AssignmentId == assignmentId)) {
                    RemoveSubmissionFromAssignmentById(assignmentId);
                }

                try {
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception e) {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Delete a connection between an assignment and a problem using the assignment ID and problem ID
        /// </summary>
        public bool RemoveProblemFromAssignmentByIds(int assignmentId, int problemId) {
            _db.AssignmentProblems.RemoveRange(_db.AssignmentProblems.Where(x => x.AssignmentId == assignmentId && x.ProblemId == problemId));

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Deletes all submissions for a specific assignment, used when deleteng an assignment from the database
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool RemoveSubmissionFromAssignmentById(int assignmentId)
        {
            // Remove all testResults from submission
            foreach(var testResult in _db.TestResults.Where(x => x.Submission.AssignmentId == assignmentId)) {
                RemoveTestResultFromSubmissionById(testResult.SubmissionId);
            }

            _db.Submissions.RemoveRange(_db.Submissions.Where(x => x.AssignmentId == assignmentId));

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Deletes all Test results connected to a submission. Used when deleting submission from the database
        /// </summary>
        /// <param name="submissionId"></param>
        /// <returns></returns>
        public bool RemoveTestResultFromSubmissionById(int submissionId)
        {
            _db.TestResults.RemoveRange(_db.TestResults.Where(x => x.SubmissionId == submissionId));

            try {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }
        /// <summary>
        /// Delete a problem from the database via it's Id
        /// </summary>

        public bool DeleteProblemById(int problemId) {
            var problem = _db.Problems.SingleOrDefault(x => x.Id == problemId);

            if (problem != null) {
                _db.Problems.Remove(problem);

                try {
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception e) {
                    return false;
                }
            }

            return false;
        }

        public void CheckUngradedAssignments(int courseInstanceId) {
            // Gets all closed and ungraded assignments in courseInstance
            var assignmentsQuery = _db.Assignments.Where(x => x.CourseInstanceId == courseInstanceId && x.EndTime < DateTime.Now && x.IsGraded == false);
            foreach (var assignment in assignmentsQuery) {
                // Assumes the assignment is graded
                bool IsGraded = true;

                // Gets all problems in assignment
                var problemQuery = _db.AssignmentProblems.Where(x => x.AssignmentId == assignment.Id);

                // Gets all students assigned to assignment and collects uniqe groupNumbers in HashSet<int>
                var groupQuery = _db.AssignmentGroups.Where(x => x.AssignmentId == assignment.Id);
                var groups = new HashSet<int>();
                foreach (var student in groupQuery) {
                    groups.Add(student.GroupNumber);
                }

                // Foreach problem in assignment
                foreach (var problem in problemQuery) {
                    // Foreach unique group assigned to assignment
                    foreach (var group in groups) {
                        // Gets all students in particular unique group for assignment
                        var studentsGroupQuery = _db.AssignmentGroups.Where(x => x.GroupNumber == group && x.AssignmentId == assignment.Id);

                        // Gets the list of all submissions from group members
                        var groupSubmissionsQuery = new List<Submission>();
                        foreach (var student in studentsGroupQuery) {
                            // Get all submission that have been graded
                            var studentSubmissionQuery = _db.Submissions.Where(x => x.StudentId == student.UserId && x.AssignmentId == assignment.Id && x.ProblemId == problem.ProblemId && x.SubmissionGrade != null);
                            foreach (var submission in studentSubmissionQuery) {
                                groupSubmissionsQuery.Add(submission);
                            }
                        }
                        // If no submission that is graded is found we break and render the assignment still NOT Graded
                        if (groupSubmissionsQuery == null) {
                            IsGraded = false;
                            break;
                        }
                    }
                    if (IsGraded == false) {
                        break;
                    }
                }
                // We have iterated through the entire assignment and there is no problem with at least one group with no graded submission
                // So we update the database and set the assignment to IsGraded = true
                if (IsGraded == true) {
                    assignment.IsGraded = true;
                    try {
                        _db.SaveChanges();
                    }
                    catch (Exception e) {}
                }
            }
        }

        /// <summary>
        /// Get how much time is left of an assignment in terms of days, hours, minutes or second, 
        /// depending on how much time is left
        /// </summary>
        public string GetAssignmentTimeRemaining(TeacherAssignmentViewModel assignment) {
            var timeRemaining = String.Empty;

            if (assignment.StartTime.HasValue && assignment.EndTime.HasValue) {
                if (0 < assignment.EndTime.Value.CompareTo(DateTime.Now)) {
                    var remainingTimeSpan = new TimeSpan(assignment.EndTime.Value.Ticks - DateTime.Now.Ticks);
                    if (0 < remainingTimeSpan.Days) {
                        timeRemaining = remainingTimeSpan.Days.ToString();
                        timeRemaining += (remainingTimeSpan.Days == 1 ? " day left" : " days left");
                    }
                    else if (0 < remainingTimeSpan.Hours) {
                        timeRemaining = remainingTimeSpan.Hours.ToString();
                        timeRemaining += (remainingTimeSpan.Hours == 1 ? " hour left" : " hours left");
                    }
                    else if (0 < remainingTimeSpan.Minutes) {
                        timeRemaining = remainingTimeSpan.Minutes.ToString();
                        timeRemaining += (remainingTimeSpan.Minutes == 1 ? " minute left" : " minutes left");
                    }
                    else if (0 < remainingTimeSpan.Seconds) {
                        timeRemaining = remainingTimeSpan.Seconds.ToString();
                        timeRemaining += (remainingTimeSpan.Seconds == 1 ? " second left" : " seconds left");
                    }
                }
            }

            return timeRemaining;
        }

        /// <summary>
        /// Grades a single submission
        /// </summary>
        public bool GradeSubmissionById(int submissionId, double grade)
        {
            var submission = _db.Submissions.SingleOrDefault(x => x.Id == submissionId);
            if(submission != null)
            {
                submission.SubmissionGrade.Grade = grade;

                try {
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception e) {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Used to calculate new total grade for assignment based on one submissionId, called right after grading that submission in the teacher controller.
        /// Updates the totalGrade for all collaborators in the AssignmentGroups table.
        /// </summary>
        public bool UpdateAssignmentGradeBySubmissionId(int submissionId)
        {
            // Get the initial submission
            var initialSubmission = _db.Submissions.SingleOrDefault(x => x.Id == submissionId);

            if(initialSubmission != null) {
                //Gets all collaborators in the assignment
                var studentService = new StudentService();
                var collaborators = studentService.GetCollaborators(initialSubmission.AssignmentId, initialSubmission.StudentId);

                // Gets all problems in the assignment
                var problemsInAssignemnt = _db.AssignmentProblems.Where(x => x.AssignmentId == initialSubmission.AssignmentId);

                // Initializes the totalGrade to 0.0
                var totalGrade = 0.0;

                if(collaborators != null && problemsInAssignemnt != null)
                {
                    foreach (var problem in problemsInAssignemnt)
                    {
                        var problemWeight = problem.Weight;
                        var problemGrade = 0.0;
                        foreach (var student in collaborators)
                        {
                            var submission = _db.Submissions.Where(x => x.StudentId == student.Id && x.AssignmentId == initialSubmission.AssignmentId && x.ProblemId == initialSubmission.ProblemId).OrderByDescending(y => y.SubmissionGrade.Grade).FirstOrDefault();
                            // Checks if the best submission from the student is the best amongst his collaborators. If it is it is assigned to the problemGrade variable
                            if (submission != null && submission.SubmissionGrade.Grade.Value * problemWeight > problemGrade)
                            {
                                problemGrade = submission.SubmissionGrade.Grade.Value * problemWeight;
                            }
                        }
                        // Update totalGrade with the best problemGrade
                        totalGrade += problemGrade;
                    }
                }

                // The database update is made here, assigning the totalGrade to all groupMembers in the assignment
                var group = initialSubmission.Assignment.AssignmentGroups.SingleOrDefault(x => x.AssignmentId == initialSubmission.AssignmentId && x.UserId == initialSubmission.StudentId);
                if(group != null)
                {
                    var groupMembers = _db.AssignmentGroups.Where(x => x.AssignmentId == initialSubmission.AssignmentId && x.GroupNumber == group.GroupNumber);
                    foreach (var member in groupMembers)
                    {
                        member.AssignmentGrade = totalGrade;
                    }
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
    }
}