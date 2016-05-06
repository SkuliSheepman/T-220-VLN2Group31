using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codex.Models
{

    public class AssignmentViewModel
    {

        public int Id { get; set; }
        public int CourseInstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTIme { get; set; }
        public DateTime? EndTIme { get; set; }
        public int MaxCollaborators { get; set; }
        public List<ProblemViewModel> AssignmentProblems { get; set; }
        public List<UserViewModel> AssignmentCollaborators { get; set; }

    }

    public class AssignmentCreationViewModel
    {

        public int CourseInstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTIme { get; set; }
        public DateTime? EndTIme { get; set; }
        public int MaxCollaborators { get; set; }
        public List<int> AssignmentProblems { get; set; }


    }

}