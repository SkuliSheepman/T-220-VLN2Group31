namespace Codex.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Problem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Problem()
        {
            AssignmentProblems = new HashSet<AssignmentProblem>();
            Submissions = new HashSet<Submission>();
            TestCases = new HashSet<TestCase>();
        }

        public int Id { get; set; }

        public int CourseId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(10)]
        public string Filetype { get; set; }

        public string Attachment { get; set; }

        [Required]
        [StringLength(50)]
        public string Language { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssignmentProblem> AssignmentProblems { get; set; }

        public virtual Course Course { get; set; }

        public virtual Filetype Filetype1 { get; set; }

        public virtual ProgrammingLanguage ProgrammingLanguage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Submission> Submissions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestCase> TestCases { get; set; }
    }
}
