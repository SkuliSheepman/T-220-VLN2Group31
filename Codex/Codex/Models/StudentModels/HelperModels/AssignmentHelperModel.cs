using Codex.Models.SharedModels.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.StudentModels.HelperModels
{
    public class AssignmentHelperModel : TeacherAssignmentViewModel
    {
        public double? AssignmentGrade { get; set; }
        public bool IsDone { get; set; }
        public List<CollaboratorHelperModel> Collaborators { get; set; }
        public new List<ProblemHelperModel> AssignmentProblems { get; set; }
    }
}