using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.TeacherViewModels
{
    public class TeacherViewModel
    {
        public int YearSelected { get; set; }
        public string SemesterSelected { get; set; }
        public List<ActiveSemesterViewModel> ActiveSemesters { get; set; }
        public List<CourseViewModel> TeacherCourses { get; set; }
    }
    public class ActiveSemesterViewModel
    {
        public int Year { get; set; }
        public string Semester { get; set; }
    }
    public class CourseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsAssistant { get; set; }
        public int Year { get; set; }
        public string Semester { get; set; }
        public List<AssignmentViewModel> RequiresGradingAssignments { get; set; }
        public List<AssignmentViewModel> OpenAssignments { get; set; }
        public List<AssignmentViewModel> UpcomingAssignments { get; set; }
        public List<AssignmentViewModel> ClosedAssignments { get; set; }
    }
    public class AssignmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int MaxCollaborators { get; set; }
        public bool IsGraded { get; set; }
        public List<ProblemViewModel> Problems { get; set; }
    }

    public class ProblemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public List<AssignmentGroupViewModel> Groups { get; set; }
        public List<SubmissionViewModel> BestSubmissions { get; set; }
    }

    public class AssignmentGroupViewModel
    {
        public int GroupNumber { get; set; }
        public List<string> StudentIds { get; set; }
        public SubmissionViewModel BestSubmission { get; set; }
        public List<SubmissionViewModel> Submissions { get; set; }
    }

    public class ProblemUpdateViewModel
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

    public class SubmissionViewModel
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public int ProblemId { get; set; }
        public int AssignmentId { get; set; }
        public DateTime SubmissionTime { get; set; }
        public int FailedTests { get; set; }
        public float SubmissionGrade { get; set; }
    }
}
