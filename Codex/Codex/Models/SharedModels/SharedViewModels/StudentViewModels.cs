using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.SharedModels.SharedViewModels
{
    public class StudentViewModel
    {
        //public SomeViewModel User { get; set; }  todo
        public List<StudentAssignmentViewModel> Assignments { get; set; }
    }

    public class HomeStudentViewModel
    {
        public List<StudentAssignmentViewModel> Assignments { get; set; }
    }
}