using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.SharedModels.SharedViewModels
{
    public class ProblemViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Filetype { get; set; }
        public string Attachment { get; set; }
        public string Language { get; set; }
        public List<TestCaseViewModel> testCases { get; set; }
    }

    public class ProblemCreationViewModel
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Filetype { get; set; }
        public string Attachment { get; set; }
        public string Language { get; set; }
        public List<TestCaseCreationViewModel> testCases { get; set; }
    }

    public class StudentProblemViewModel : ProblemViewModel
    {
        public int Weight { get; set; }
        public bool IsAccepted { get; set; }
        public List<SubmissionViewModel> Submissions { get; set; }
    }
}