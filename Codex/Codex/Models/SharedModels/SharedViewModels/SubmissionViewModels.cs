using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.SharedModels.SharedViewModels
{
    public class SubmissionViewModel
    {
        public int Id { get; set; }
        public string OriginalFilename { get; set; }
        public DateTime SubmissionTime { get; set; }
        public int? FailedTests { get; set; }
        public string Owner { get; set; }
    }
}