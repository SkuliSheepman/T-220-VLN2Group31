namespace Codex.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AssignmentGroup
    {
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AssignmentId { get; set; }

        public int GroupNumber { get; set; }

        public double? AssignmentGrade { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Assignment Assignment { get; set; }
    }
}
