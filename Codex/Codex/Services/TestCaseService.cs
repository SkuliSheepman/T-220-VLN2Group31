using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codex.DAL;
using Codex.Models.SharedModels.SharedViewModels;

namespace Codex.Services
{
    public class TestCaseService
    {

        private Database _db;

        public TestCaseService()
        {

            _db = new Database();

        }

        /// <summary>
        /// 
        /// </summary>
        public bool CreateTestCase(TestCaseCreationViewModel newTestCaseViewModel)
        {

            var newTestCase = new TestCase
            {
                ProblemId      = newTestCaseViewModel.ProblemId,
                Input          = newTestCaseViewModel.Input,
                ExpectedOutput = newTestCaseViewModel.ExpectedOutput
            };

            _db.TestCases.Add(newTestCase);

            try
            {
                _db.SaveChanges();
                return true;
            } catch ( Exception e )
            {
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public bool DeleteTestCase(int testCaseId)
        {

            var testCaseToBeDeleted = _db.TestCases.SingleOrDefault(x => x.Id == testCaseId);

            if (testCaseToBeDeleted == null)
                return false;

            _db.TestCases.Remove(testCaseToBeDeleted);

            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public bool EditTestCase(TestCaseViewModel editTestCaseViewModel)
        {

            var testCaseToBeEdited = _db.TestCases.SingleOrDefault(x => x.Id == editTestCaseViewModel.Id);

            if (testCaseToBeEdited == null)
                return false;

            //testCaseToBeEdited.ProblemId = editTestCaseViewModel.ProblemId;
            testCaseToBeEdited.Input = editTestCaseViewModel.Input;
            testCaseToBeEdited.ExpectedOutput = editTestCaseViewModel.ExpectedOutput;

            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

    }
}