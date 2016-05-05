//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntityTest.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Submission
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Submission()
        {
            this.TestResults = new HashSet<TestResult>();
        }
    
        public int Id { get; set; }
        public string StudentId { get; set; }
        public int ProblemId { get; set; }
        public int AssignmentId { get; set; }
        public System.DateTime Time { get; set; }
        public Nullable<int> PassedTests { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Assignment Assignment { get; set; }
        public virtual Problem Problem { get; set; }
        public virtual SubmissionGrade SubmissionGrade { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestResult> TestResults { get; set; }
    }
}