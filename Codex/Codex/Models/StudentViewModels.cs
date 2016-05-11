using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models
{
    public class StudentViewModel
    {
        public List<StudentAssignmentViewModel> Assignments { get; set; }
    }

    public class StudentAssignmentViewModel
    {
        public int Id { get; set; }
        public string Course { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string TimeRemaining { get; set; }
        public string NumberOfProblems { get; set; }
        public int MaxCollaborators { get; set; }
        public double? AssignmentGrade { get; set; }
        public bool IsDone { get; set; }
        public List<CollaboratorViewModel> Collaborators { get; set; }
        public List<StudentProblemViewModel> Problems { get; set; }
    }

    public class CollaboratorViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int GroupNumber { get; set; }
    }

    public class StudentProblemViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Filetype { get; set; }
        public string Attachment { get; set; }
        public string Language { get; set; }
        public List<StudentTestCaseViewModel> testCases { get; set; }
        public int Weight { get; set; }
        public int MaxSubmissions { get; set; }
        public bool IsAccepted { get; set; }
        public StudentSubmissionViewModel BestSubmission { get; set; }
        public List<StudentSubmissionViewModel> Submissions { get; set; }
    }

    public class StudentSubmissionViewModel
    {
        public int Id { get; set; }
        public string OriginalFilename { get; set; }
        public double? Grade { get; set; }
        public DateTime SubmissionTime { get; set; }
        public int? FailedTests { get; set; }
        public string Owner { get; set; }
    }

    public class StudentTestCaseViewModel
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public string Input { get; set; }
        public string ExpectedOutput { get; set; }
    }
}