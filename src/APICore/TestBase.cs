using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TestLibrary;
//using APICore.APIConnection.Maps;
//using APICore.APIConnection.Private;
using APICore.ModelBase;
//using APICore.Api.PrivateAPI;
using APICore.Helpers;
using System.Net;
//using APICore.Api.PrivateAPI.Rosetta;
//using APICore.Resources.PrivateAPI.Field;
//using APICore.Api.PrivateAPI.Bulk;
using System.Threading;

namespace APICore
{
    /// <summary>
    /// The base class for all test classes of API.
    /// All test classes MUST be inherited from this class
    /// </summary>
    public class TestBase
    {
        /// <summary>
        /// This method will be called once BEFORE any test cases is executed.
        /// </summary>
        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            Log.PrintTestFixtureSetup();
        }

        /// <summary>
        /// This method will be called once AFTER any test cases is executed.
        /// </summary>
        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
            Log.PrintTestFixtureTearDown();
        }
        /// <summary>
        /// This method will be called once BEFORE each test method.
        /// In case you override this method. Remember to call base.SetUp() in your method.
        /// </summary>
        [SetUp]
        public virtual void SetUp()
        {
            Log.PrintTestCaseDescription();
        }

        /// <summary>
        /// This method will be called once AFTER each test method.
        /// In case you override this method. Remember to call base.TearDown() in your method.
        /// </summary>
        [TearDown]
        public virtual void TearDown()
        {
            Log.PrintTestCaseTeardown();
        }
    }
}
