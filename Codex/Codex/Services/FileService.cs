using Codex.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Net;
using Codex.Models;

namespace Codex.Services
{
    public class FileService
    {
        private readonly Database _db;
        private readonly StudentService _studentService;

        public FileService() {
            _db = new Database();
            _studentService = new StudentService();
        }

        /// <summary>
        /// Get the absolute path to the base of the submissions folder
        /// </summary>
        public string GetSubmissionsPath() {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["SubmissionsFolder"]);
        }

        /// <summary>
        /// Get the absolute path to the base of the attachments folder
        /// </summary>
        public string GetAttachmentsPath() {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["AttachmentsFolder"]);
        }

        /// <summary>
        /// Uploads a submission to the server by assignment Id, problem Id and user Id
        /// </summary>
        public bool UploadSubmissionToServer(HttpPostedFileBase file, int assignmentId, int problemId, int submissionId) {
            string uploadPath = GetSubmissionsPath() + assignmentId + "\\" + problemId + "\\" + submissionId + "\\";

            if (!Directory.Exists(uploadPath)) {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = uploadPath + submissionId + Path.GetExtension(file.FileName);

            file.SaveAs(fileName);

            if (File.Exists(fileName)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Uploads a problem attachment to the server by problemId
        /// </summary>
        public bool UploadAttachmentToServer(HttpPostedFileBase file, int problemId) {
            string uploadPath = GetAttachmentsPath() + problemId + "\\";

            if (!Directory.Exists(uploadPath)) {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = uploadPath + Path.GetExtension(file.FileName);

            file.SaveAs(fileName);

            if (File.Exists(fileName)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Compile a C++ program via submission ID
        /// </summary>
        public bool CompileCPlusPlusBySubmissionId(int submissionId) {
            var submission = _db.Submissions.SingleOrDefault(x => x.Id == submissionId);

            if (submission != null) {
                var directory = GetSubmissionsPath() + submission.AssignmentId + "\\" + submission.ProblemId + "\\" + submission.Id + "\\";

                var exePath = directory + submission.Id + ".exe";
                var files = submission.Id + ".cpp";

                var compilerFolder = ConfigurationManager.AppSettings["VisualStudioCPlusPlusCompilerFolder"];

                // Compile
                Process compiler = new Process();
                compiler.StartInfo.FileName = "cmd.exe";
                compiler.StartInfo.WorkingDirectory = directory;
                compiler.StartInfo.RedirectStandardInput = true;
                compiler.StartInfo.RedirectStandardOutput = true;
                compiler.StartInfo.UseShellExecute = false;

                compiler.Start();
                compiler.StandardInput.WriteLine("\"" + compilerFolder + "vcvars32.bat" + "\"");
                compiler.StandardInput.WriteLine("cl.exe /nologo /EHsc " + files);
                compiler.StandardInput.WriteLine("exit");
                string output = compiler.StandardOutput.ReadToEnd();
                compiler.WaitForExit();
                compiler.Close();

                // Delete .obj files
                DeleteFileFromDirectoryByExtension(directory, ".obj");

                return File.Exists(exePath);
            }

            return false;
        }

        /// <summary>
        /// Run test cases for a submission via the submission's ID
        /// </summary>
        public bool RunTestCasesBySubmissionId(int submissionId) {
            var submission = _db.Submissions.SingleOrDefault(x => x.Id == submissionId);

            if (submission != null) {
                var directory = GetSubmissionsPath() + submission.AssignmentId + "\\" + submission.ProblemId + "\\" + submission.Id + "\\";

                var exePath = directory + submission.Id + ".exe";

                if (File.Exists(exePath)) {
                    var testCases = _db.TestCases.Where(x => x.ProblemId == submission.ProblemId);

                    var failedTestCases = testCases.Count();

                    foreach (var testCase in testCases) {
                        // Setup
                        var processInfoExe = new ProcessStartInfo(exePath, "");
                        processInfoExe.UseShellExecute = false;
                        processInfoExe.RedirectStandardInput = true;
                        processInfoExe.RedirectStandardOutput = true;
                        processInfoExe.RedirectStandardError = true;
                        processInfoExe.CreateNoWindow = true;

                        // Run
                        var lines = new List<string>();
                        using (var processExe = new Process()) {
                            processExe.StartInfo = processInfoExe;
                            processExe.Start();
                            processExe.StandardInput.WriteLine(testCase.Input);

                            // Read output
                            while (!processExe.StandardOutput.EndOfStream) {
                                lines.Add(processExe.StandardOutput.ReadLine());
                            }
                        }

                        // Output
                        string output = string.Join(Environment.NewLine, lines);

                        bool passed = (output == testCase.ExpectedOutput);

                        if (passed) {
                            failedTestCases--;
                        }

                        // Add to db
                        var result = new TestResult {
                            TestCaseId = testCase.Id,
                            SubmissionId = submission.Id,
                            ProgramOutput = output,
                            Passed = passed
                        };

                        _db.TestResults.Add(result);
                    }

                    // Delete .exe file
                    DeleteFileFromDirectoryByExtension(directory, ".exe");

                    // Update failed tests field
                    submission.FailedTests = failedTestCases;

                    try {
                        _db.SaveChanges();
                        return true;
                    }
                    catch (Exception e) {
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Delete all files from a specified directory with specified extension
        /// </summary>
        private void DeleteFileFromDirectoryByExtension(string path, string extension) {
            var files = Directory.GetFiles(path, "*" + extension);

            foreach (var file in files) {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
        }

        /// <summary>
        /// Download a submission
        /// Unfinished
        /// </summary>
        public void DownloadSubmission(string userid, int? submissionId)
        {
            var user = _db.AspNetUsers.SingleOrDefault(x => x.Id == userid);
            if (submissionId.HasValue && user != null)
            {
                var submission = _db.Submissions.SingleOrDefault(x => x.Id == submissionId);
                if (submission != null)
                {
                    var collaborators = new List<CollaboratorViewModel>();
                    bool isTeacher = submission.Assignment.CourseInstance.Teachers.Any(x => x.AspNetUser == user);

                    if (!isTeacher)
                    {
                        collaborators = _studentService.GetCollaborators(submission.AssignmentId, userid);
                    }

                    if (collaborators.Count != 0 || isTeacher)  // check if user is related to the submission group
                    {
                        var path = GetSubmissionsPath() +
                                   submission.AssignmentId + "\\" +
                                   submission.ProblemId + "\\" +
                                   submission.Id + "\\" +
                                   submission.Id + "." + submission.Problem.Filetype;

                        DownloadFile(path, submission.OriginalFileName);
                    }
                }
            }
        }

        /// <summary>
        /// Download an attachment
        /// Unfinished
        /// </summary>
        public void DownloadAttachment(string userid, int? problemid)
        {
            var user = _db.AspNetUsers.SingleOrDefault(x => x.Id == userid);
            if (problemid.HasValue && user != null)
            {
                var problem = _db.Problems.SingleOrDefault(x => x.Id == problemid);
                if (problem != null)
                {
                    var assignment = (from assign in _db.Assignments
                                      join relation in _db.AssignmentProblems
                                      on assign.Id equals relation.AssignmentId
                                      where relation.ProblemId == problemid
                                      select assign).SingleOrDefault();

                    if (assignment != null)
                    {
                        if (user.CourseInstances.Contains(assignment.CourseInstance))
                        {
                            if (problem.Attachment != null)
                            {
                                var path = GetAttachmentsPath() +
                                       problem.Id + "\\" +
                                       problem.Id + "." + problem.Attachment.Split('.')[1];

                                DownloadFile(path, problem.Attachment);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Download a file
        /// Unfinished
        /// </summary>
        public void DownloadFile(string path, string originalFileName)
        {
            string filePath = path;
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                WebClient req = new WebClient();
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + originalFileName + "\"");
                byte[] data = req.DownloadData(filePath);
                response.BinaryWrite(data);
                response.End();
            }
        }
    }
}