using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.SharedModels.SharedViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public int SemesterId { get; set; }
        public string Semester { get; set; }
    }

    public class CourseCreationViewModel
    {}
}