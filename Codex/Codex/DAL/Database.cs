namespace Codex.DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Database : DbContext
    {
        public Database()
            : base("name=Database")
        {
        }

        public DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<AssignmentGroup> AssignmentGroups { get; set; }
        public DbSet<AssignmentProblem> AssignmentProblems { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<CourseInstance> CourseInstances { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Filetype> Filetypes { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<SubmissionGrade> SubmissionGrades { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TestCase> TestCases { get; set; }
        public DbSet<TestResult> TestResults { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AssignmentGroups)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.SubmissionGrades)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.TeacherId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Submissions)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.StudentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Teachers)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.CourseInstances)
                .WithMany(e => e.AspNetUsers)
                .Map(m => m.ToTable("Students").MapLeftKey("UserId").MapRightKey("CourseInstanceId"));

            modelBuilder.Entity<Assignment>()
                .HasMany(e => e.AssignmentGroups)
                .WithRequired(e => e.Assignment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Assignment>()
                .HasMany(e => e.AssignmentProblems)
                .WithRequired(e => e.Assignment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Assignment>()
                .HasMany(e => e.Submissions)
                .WithRequired(e => e.Assignment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseInstance>()
                .HasMany(e => e.Assignments)
                .WithRequired(e => e.CourseInstance)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseInstance>()
                .HasMany(e => e.Teachers)
                .WithRequired(e => e.CourseInstance)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseInstances)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.Problems)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Filetype>()
                .HasMany(e => e.Problems)
                .WithRequired(e => e.Filetype1)
                .HasForeignKey(e => e.Filetype)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Problem>()
                .HasMany(e => e.AssignmentProblems)
                .WithRequired(e => e.Problem)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Problem>()
                .HasMany(e => e.Submissions)
                .WithRequired(e => e.Problem)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Problem>()
                .HasMany(e => e.TestCases)
                .WithRequired(e => e.Problem)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProgrammingLanguage>()
                .HasMany(e => e.Problems)
                .WithRequired(e => e.ProgrammingLanguage)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Semester>()
                .HasMany(e => e.CourseInstances)
                .WithRequired(e => e.Semester)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Submission>()
                .HasOptional(e => e.SubmissionGrade)
                .WithRequired(e => e.Submission);

            modelBuilder.Entity<Submission>()
                .HasMany(e => e.TestResults)
                .WithRequired(e => e.Submission)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TestCase>()
                .HasMany(e => e.TestResults)
                .WithRequired(e => e.TestCase)
                .HasForeignKey(e => e.TestCaseId)
                .WillCascadeOnDelete(false);
        }
    }
}
