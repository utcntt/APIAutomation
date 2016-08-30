using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TestLibrary;
//using APICore.APIConnection.Maps;
//using APICore.APIConnection.Private;
using APICore.Model;
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
            //Log.Info("Tear down!");
            //mapsConnection = null;
            //privateConnection = null;
            //publicConnection = null;
        }

        

        ///// <summary>
        ///// PublicAPIConnection
        ///// </summary>
        //public static PublicAPIConnection PublicConnection
        //{
        //    get
        //    {
        //        if (publicConnection == null)
        //            publicConnection = PublicAPIConnection.Instance;
        //        return publicConnection;
        //    }
        //}

        ///// <summary>
        ///// PrivateAPIConnection
        ///// </summary>
        //public static PrivateAPIConnection PrivateConnection
        //{
        //    get
        //    {
        //        if (privateConnection == null)
        //            privateConnection = PrivateAPIConnection.Instance;
        //        return privateConnection;
        //    }
        //}

        ///// <summary>
        ///// PrivateAPIAuthetication
        ///// </summary>
        //public static PrivateAPIAuthentication PrivateAuthentication
        //{
        //    get
        //    {
        //        return PrivateConnection.Authentication;
        //    }
        //}

        ///// <summary>
        ///// PublicAPIAuthetication
        ///// </summary>
        //public static PublicAPIAuthentication PublicAuthentication
        //{
        //    get
        //    {
        //        return PublicConnection.Authentication;
        //    }
        //}

        
    }
}
