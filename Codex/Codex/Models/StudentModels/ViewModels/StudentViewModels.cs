using Codex.Models.StudentModels.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.StudentModels.ViewModels
{
    public class StudentViewModel
    {
        public List<AssignmentHelperModel> Assignments { get; set; }
    }
}