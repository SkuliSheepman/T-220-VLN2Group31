using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models
{
    public class TeacherViewModel
    {
        public List<TeacherActiveSemesterViewModel> ActiveSemesters { get; set; }
        public List<TeacherCourseViewModel> TeacherCourses { get; set; }
        public TeacherCourseViewModel CourseSelected { get; set; }
    }
    public class TeacherActiveSemesterViewModel
    {
        public int Year { get; set; }
        public string Semester { get; set; }
    }
    public class TeacherCourseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsAssistant { get; set; }
        public int Year { get; set; }
        public string Semester { get; set; }
        public List<TeacherAssignmentViewModel> RequiresGradingAssignments { get; set; }
        public List<TeacherAssignmentViewModel> OpenAssignments { get; set; }
        public List<TeacherAssignmentViewModel> UpcomingAssignments { get; set; }
        public List<TeacherAssignmentViewModel> ClosedAssignments { get; set; }
    }
    public class TeacherAssignmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int MaxCollaborators { get; set; }
        public bool IsGraded { get; set; }
        public string NumberOfProblems { get; set; }
        public string TimeRemaining { get; set; }
        public List<TeacherProblemViewModel> Problems { get; set; }
    }

    public class TeacherProblemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public List<TeacherAssignmentGroupViewModel> Groups { get; set; }
    }

    public class TeacherAssignmentGroupViewModel
    {
        public int GroupNumber { get; set; }
        public List<string> StudentIds { get; set; }
        public List<string> Names { get; set; }
        public TeacherSubmissionViewModel BestSubmission { get; set; }
        public List<TeacherSubmissionViewModel> Submissions { get; set; }
    }

    public class TeacherProblemUpdateViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Filetype { get; set; }
        public string Attachment { get; set; }
        public string Language { get; set; }
        public int Weight { get; set; }
        //public List<TestCaseViewModel> TestCases { get; set; }
    }

    public class TeacherSubmissionViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string OriginalFilename { get; set; }
        public DateTime SubmissionTime { get; set; }
        public int? FailedTests { get; set; }
        public double? SubmissionGrade { get; set; }
    }
}
