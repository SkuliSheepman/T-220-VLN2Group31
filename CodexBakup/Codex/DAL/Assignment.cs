namespace Codex.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Assignment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Assignment()
        {
            AssignmentGroups = new HashSet<AssignmentGroup>();
            AssignmentProblems = new HashSet<AssignmentProblem>();
            Submissions = new HashSet<Submission>();
        }

        public int Id { get; set; }

        public int CourseInstanceId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? StartTime { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EndTime { get; set; }

        public int MaxCollaborators { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssignmentGroup> AssignmentGroups { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssignmentProblem> AssignmentProblems { get; set; }

        public virtual CourseInstance CourseInstance { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
