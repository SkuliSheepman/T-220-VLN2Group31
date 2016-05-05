namespace Codex.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TestResult
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TestCaseId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubmissionId { get; set; }

        public bool? Passed { get; set; }

        public string ProgramOutput { get; set; }

        public virtual Submission Submission { get; set; }

        public virtual TestCase TestCase { get; set; }
    }
}
