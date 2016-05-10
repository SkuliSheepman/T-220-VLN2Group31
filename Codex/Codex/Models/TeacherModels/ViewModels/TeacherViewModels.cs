using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codex.Models.TeacherModels.ViewModels
{
    public class TeacherViewModel
    {
        public int YearSelected { get; set; }
        public int SemesterSelected { get; set; }
        public List<int> TeacherYearsActive { get; set; }
    }
}