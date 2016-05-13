using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models
{
    /// <summary>
    /// Main submission view model
    /// </summary>
    public class SubmissionViewModel
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string FileName { get; set; }
        public int SubmissionNumber { get; set; }
        public SubmissionAssignmentViewModel Assignment { get; set; }
        public SubmissionProblemViewModel Problem { get; set; }
        public List<SubmissionTestCaseViewModel> FailedTestCases { get; set; }
        public List<SubmissionTestCaseViewModel> PassedTestCases { get; set; }
    }

    /// <summary>
    /// View model for submission
    /// </summary>
    public class SubmissionAssignmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Course { get; set; }
        public string TimeRemaining { get; set; }
        public bool IsDone { get; set; }
    }

    /// <summary>
    /// View model for problem
    /// </summary>
    public class SubmissionProblemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
    }


    /// <summary>
    /// View model for test cases
    /// </summary>
    public class SubmissionTestCaseViewModel
    {
        public int TestCaseNumber { get; set; }
        public string Input { get; set; }
        public string SubmissionOutput { get; set; }
        public string ExpectedOutput { get; set; }
    }
}