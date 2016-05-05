namespace Codex.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AssignmentProblem
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProblemId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AssignmentId { get; set; }

        public int MaxSubmissions { get; set; }

        public byte Weight { get; set; }

        public virtual Assignment Assignment { get; set; }

        public virtual Problem Problem { get; set; }
    }
}
