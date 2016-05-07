using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.SharedModels.SharedViewModels
{
    public class TestCaseViewModel
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public string Input { get; set; }
        public string ExpectedOutput { get; set; }
    }

    public class TestCaseCreationViewModel
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public string Input { get; set; }
        public string ExpectedOutput { get; set; }
    }
}