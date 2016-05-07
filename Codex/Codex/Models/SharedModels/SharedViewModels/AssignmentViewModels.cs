﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Codex.Models.AdminModels.AdminViewModels;

namespace Codex.Models.SharedModels.SharedViewModels
{

    public class AssignmentViewModel
    {

        public int Id { get; set; }
        public int CourseInstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int MaxCollaborators { get; set; }
        public List<ProblemViewModel> AssignmentProblems { get; set; }

    }

    public class AssignmentCreationViewModel
    {

        public int CourseInstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int MaxCollaborators { get; set; }
        public List<int> AssignmentProblems { get; set; }

    }

}