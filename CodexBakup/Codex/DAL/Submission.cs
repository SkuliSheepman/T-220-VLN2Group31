namespace Codex.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Submission
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Submission()
        {
            TestResults = new HashSet<TestResult>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string StudentId { get; set; }

        public int ProblemId { get; set; }

        public int AssignmentId { get; set; }

        public DateTime Time { get; set; }

        public int? FailedTests { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Assignment Assignment { get; set; }

        public virtual Problem Problem { get; set; }

        public virtual SubmissionGrade SubmissionGrade { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestResult> TestResults { get; set; }
    }
}
