using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models
{
    public class ProblemViewModel
    {

        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Filetype { get; set; }
        public string Attachment { get; set; }
        public string Language { get; set; }

    }
}