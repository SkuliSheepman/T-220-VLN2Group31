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
        public List<ProblemViewModel> AssignmentProblems { get; set; }

    }

}