using Codex.Models.SharedModels.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.StudentModels.HelperModels
{
    public class ProblemHelperModel : ProblemViewModel
    {
        public int Weight { get; set; }
        public int MaxSubmissions { get; set; }
        public bool IsAccepted { get; set; }
        public SubmissionHelperModel BestSubmission { get; set; }
        public List<SubmissionHelperModel> Submissions { get; set; }
    }
}