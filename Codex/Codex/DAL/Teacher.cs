namespace Codex.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Teacher
    {
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseInstanceId { get; set; }

        public bool? IsAssistant { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual CourseInstance CourseInstance { get; set; }
    }
}
