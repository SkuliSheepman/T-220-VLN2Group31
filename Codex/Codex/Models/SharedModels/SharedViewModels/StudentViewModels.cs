using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.SharedModels.SharedViewModels
{
    public class StudentViewModel
    {
        //public UserViewModel User { get; set; }
        public List<AssignmentViewModel> Assignments { get; set; }
    }

}