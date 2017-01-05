using System;
using NUnit.Framework;
using APICore;
//using APITestFramework.Api.PrivateAPI;

using System.Collections.Generic;
using System.Reflection;
using System.Net;
using TestLibrary;

namespace QnAMaker.Test
{
    /// <summary>
    /// This is the SetUp class for PrivateAPI.
    /// </summary>
    [SetUpFixture]
    public class SetUp
    {
        /// <summary>
        /// MUST call this method in the SetUp method of the setup class.
        /// </summary>
        [OneTimeSetUp]
        public static void Init()
        {
            //Console.WriteLine("TestFixtureSetUp");
            Log.PrintTestInit();
        }

        /// <summary>
        /// MUST call this method in the Teardown method of the setup class.
        /// </summary>
        [OneTimeTearDown]
        public static void End()
        {
            Log.PrintTestEnd();
            Log.Dispose(false);
        }
    }
}
