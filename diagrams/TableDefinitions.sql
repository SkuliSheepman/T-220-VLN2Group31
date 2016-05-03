/* The predefined AspNetUsers Table */
CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (128) NOT NULL,
    [Email]                NVARCHAR (256) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]         NVARCHAR (MAX) NULL,
    [SecurityStamp]        NVARCHAR (MAX) NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEndDateUtc]    DATETIME       NULL,
    [LockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]    INT            NOT NULL,
    [UserName]             NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Courses]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Description] NVARCHAR(MAX)
)

CREATE TABLE [dbo].[Semesters]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(MAX)
)

/*  */
CREATE TABLE [dbo].[CourseInstances]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[CourseId] INT NOT NULL, 
    [Year] INT NOT NULL, 
    [SemesterId] INT NOT NULL, 
    CONSTRAINT [FK_CourseInstances_Courses] FOREIGN KEY ([CourseId]) REFERENCES [Courses]([Id]),
    CONSTRAINT [FK_CourseInstances_Semesters] FOREIGN KEY ([SemesterId]) REFERENCES [Semesters]([Id])
)

/* Teachers many-to-many relation between AspNetUsers and CourseInstances with Assistant Flag */
CREATE TABLE [dbo].[Teachers]
(
    [Id] INT NOT NULL UNIQUE IDENTITY,
    [UserId] NVARCHAR(128) NOT NULL,
    [CourseInstanceId] INT NOT NULL,
    [IsAssistant] BIT DEFAULT 0,
    CONSTRAINT [FK_Teachers_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_Teachers_CourseInstances] FOREIGN KEY ([CourseInstanceId]) REFERENCES [CourseInstances]([Id]),
    CONSTRAINT [PK_Teachers] PRIMARY KEY ([UserId], [CourseInstanceId])
)

/* The same for Students */
CREATE TABLE [dbo].[Students]
(
    [Id] INT NOT NULL UNIQUE IDENTITY,
    [UserId] NVARCHAR(128) NOT NULL,
    [CourseInstanceId] INT NOT NULL,
    CONSTRAINT [FK_Students_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_Students_CourseInstances] FOREIGN KEY ([CourseInstanceId]) REFERENCES [CourseInstances]([Id]),
    CONSTRAINT [PK_Students] PRIMARY KEY ([UserId], [CourseInstanceId])
)

CREATE TABLE [dbo].[Filetypes]
(
    [Id] INT NOT NULL UNIQUE IDENTITY,
    [Type] NVARCHAR(10) NOT NULL PRIMARY KEY,
    [Description] NVARCHAR(MAX)
)

CREATE TABLE [dbo].[ProgrammingLanguages]
(
    [Language] NVARCHAR(50) NOT NULL PRIMARY KEY
)

/* Problem is part of an Assignment, what is called Milestones in Centris */
CREATE TABLE [dbo].[Problems]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [CourseId] INT NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Description] NVARCHAR(MAX),
    [Filetype] NVARCHAR(10) NOT NULL,
    [Attachment] NVARCHAR(MAX),
    [Language] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [FK_Problems_Courses] FOREIGN KEY ([CourseId]) REFERENCES [Courses]([Id]),
    CONSTRAINT [FK_Problems_Filetypes] FOREIGN KEY ([Filetype]) REFERENCES [Filetypes]([Type]),
    CONSTRAINT [FK_Problems_ProgrammingLanguages] FOREIGN KEY ([Language]) REFERENCES [ProgrammingLanguages]([Language])
)

/*
CREATE TABLE [dbo].[ProblemFiles]
(
    [ProblemId] INT NOT NULL,
    [Path] NVARCHAR(256) NOT NULL,
    CONSTRAINT [FK_ProblemFiles_Problems] FOREIGN KEY ([ProblemId]) REFERENCES [Problems]([Id]),
    PRIMARY KEY ([ProblemId], [Path])
)
*/

/* SmallDateTime stores YYYY-MM-DD HH:MM */
CREATE TABLE [dbo].[Assignments]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [CourseInstanceId] INT NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Description] NVARCHAR(MAX),
    [StartTime] SMALLDATETIME,
    [EndTime] SMALLDATETIME,
    [MaxCollaborators] INT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_Assignments_CourseInstances] FOREIGN KEY ([CourseInstanceId]) REFERENCES [CourseInstances]([Id])
)

/* The many-to-many relation between Assignments and Problems */
CREATE TABLE [dbo].[AssignmentProblems]
(
    [Id] INT NOT NULL UNIQUE IDENTITY,
    [ProblemId] INT NOT NULL,
    [AssignmentId] INT NOT NULL,
    [MaxSubmissions] INT NOT NULL DEFAULT 0,
    [Weight] TINYINT NOT NULL DEFAULT 0,
    CONSTRAINT [FK_AssignmentProblems_Problems] FOREIGN KEY ([ProblemId]) REFERENCES [Problems]([Id]),
    CONSTRAINT [FK_AssignmentProblems_Assignments] FOREIGN KEY ([AssignmentId]) REFERENCES [Assignments]([Id]),
    CONSTRAINT [PK_AssignmentProblems] PRIMARY KEY ([ProblemId], [AssignmentId])
)

/* When assignments are created, all Students in that course instance are assigned a group.
Students can then be joined together in a group by editing so they have the same groupNumber for that Assignment. */
CREATE TABLE [dbo].[AssignmentGroups]
(
    [Id] INT NOT NULL UNIQUE IDENTITY,
    [UserId] NVARCHAR(128) NOT NULL,
    [AssignmentId] INT NOT NULL,
    [GroupNumber] INT NOT NULL,
    CONSTRAINT [FK_AssignmentGroups_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_AssignmentGroups_Assignments] FOREIGN KEY ([AssignmentId]) REFERENCES [Assignments]([Id]),
    CONSTRAINT [PK_AssignmentGroups] PRIMARY KEY ([UserId], [AssignmentId])
)

/* A student can submit a solution to a Problem in an Assignment, these are called Submissions.
The amount of possible Submissions can be limited in the AssignmentProblems table */
CREATE TABLE [dbo].[Submissions]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [StudentId] NVARCHAR(128) NOT NULL,
    [ProblemId] INT NOT NULL,
    [AssignmentId] INT NOT NULL,
    [Time] DATETIME NOT NULL,
    [PassedTests] INT,
    CONSTRAINT [FK_Submissions_AspNetUsers] FOREIGN KEY ([StudentId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_Submissions_Problems] FOREIGN KEY ([ProblemId]) REFERENCES [Problems]([Id]),
    CONSTRAINT [FK_Submissions_Assignments] FOREIGN KEY ([AssignmentId]) REFERENCES [Assignments]([Id]),
)

CREATE TABLE [dbo].[SubmissionGrades]
(
    [Id] INT NOT NULL,
    [Grade] FLOAT,
    [TeacherId] NVARCHAR(128) NOT NULL,
    [Feedback] NVARCHAR(MAX),
    CONSTRAINT [FK_SubmissionGrades_Submissions] FOREIGN KEY ([Id]) REFERENCES [Submissions]([Id]),
    CONSTRAINT [FK_SubmissionGrades_AspNetUsers] FOREIGN KEY ([TeacherId]) REFERENCES [AspNetUsers]([Id]),
    PRIMARY KEY ([Id])
)

CREATE TABLE [dbo].[TestCases]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [ProblemId] INT NOT NULL,
    [Input] NVARCHAR(MAX),
    [Output] NVARCHAR(MAX),
    CONSTRAINT [FK_TestCases_Problems] FOREIGN KEY ([ProblemId]) REFERENCES [Problems]([Id])
)

CREATE TABLE [dbo].[TestResults]
(
    [Id] INT NOT NULL UNIQUE IDENTITY,
    [TestCaseId] INT NOT NULL,
    [SubmissionId] INT NOT NULL,
    [Passed] BIT DEFAULT 0,
    [ProgramOutput] NVARCHAR(MAX),
    CONSTRAINT [FK_TestResults_TestCases] FOREIGN KEY ([TestCaseId]) REFERENCES [TestCases]([Id]),
    CONSTRAINT [FK_TestResults_Submissions] FOREIGN KEY ([SubmissionId]) REFERENCES [Submissions]([Id]),
    CONSTRAINT [PK_TestResults] PRIMARY KEY ([TestCaseId], [SubmissionId])
)
