namespace Codex.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SubmissionGrade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubmissionId { get; set; }

        public double? Grade { get; set; }

        [StringLength(128)]
        public string TeacherId { get; set; }

        public string Feedback { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Submission Submission { get; set; }
    }
}
