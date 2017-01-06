using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;

namespace TestLibrary
{
    /// <summary>
    /// Test Logger
    /// Authored by Truong Pham
    /// <example>
    /// Common Methods:
    /// Log.Info()      : Information. Written to Console.
    /// Log.Debug()     : Information, but can be ignored. Only useful for debugging.
    /// Log.Warn()      : Error, but can be ignored. Written to Console.
    /// Log.Error()     : Error, but need attention. Written to Console.
    /// </example>
    /// </summary>
    public partial class Log
    {
        public enum ParamLogLevel { Full, Fail, None }
        #region Public Members
        public static string NOW = DateTime.Now.ToString("yyyy_MM_dd-HH_mm");
        public static string LogFilePath = string.Format("{2}{3}Log{1}", NOW, Path.DirectorySeparatorChar, Util.GetTestDirectory(), Path.DirectorySeparatorChar);
        public static string HtmlLogFilePath = string.Format("{2}{3}Log{1}", NOW, Path.DirectorySeparatorChar, Util.GetTestDirectory(), Path.DirectorySeparatorChar);
        private static Dictionary<string, BaseListener> tempLog = new Dictionary<string, BaseListener>();
        private static BaseListener CurrentListener = null;
        private static ParamLogLevel logLevel = ParamLogLevel.None;
        #endregion

        #region Constructor
        /// <summary>
        /// Static Constructor
        /// </summary>
        static Log()
        {
            // if enabled, otherwise this doees not do anything
            //InitializeLog4Net();
            //InitializeLogging();
        }

        public void Dispose()
        {
            Log.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static void Dispose(bool safe)
        {
            //printedMethods = null;
            foreach (TraceListener listener in Trace.Listeners)
            {
                //listener.Close();
                listener.Dispose();
            }
            initialized = false;
        }

        /// <summary>
        /// Initialize logging using Trace.Listener
        /// </summary>
        internal static void InitializeLogging()
        {
            string logParam = TestContext.Parameters.Get("Log", string.Empty);
            logParam = logParam == string.Empty ? TestContext.Parameters.Get("log", string.Empty) : logParam;
            if(logParam.ToLower() == "failed")
            {
                logLevel = ParamLogLevel.Fail;
                isLogAllowed = true;
            } 
            else if(logParam.ToLower() == "full")
            {
                logLevel = ParamLogLevel.Full;
                isLogAllowed = true;
            }
            else
            {
                logLevel = ParamLogLevel.None;
                isLogAllowed = false;
            }
            if (!initialized && isLogAllowed)
            {
                try
                {
                    CreateDirectoryIfItDoesNotExist(LogFilePath); // Assuming other log files are located in the same directory
                    CreateHtmlResourceFiles(HtmlLogFilePath);
                    initialized = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void CreateDirectoryIfItDoesNotExist(string path)
        {
            string dirName = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
        }

        /// <summary>
        /// Create css/javascript/image files for Html Log
        /// </summary>
        private static void CreateHtmlResourceFiles(string path)
        {
            string folderPath = Path.GetDirectoryName(path);
            Assembly assembly = typeof(Log).GetTypeInfo().Assembly;
            string[] resoureList = assembly.GetManifestResourceNames();
            if(resoureList != null && resoureList.Length > 0)
            {
                var query = from t in resoureList where t.Contains(".Log.") select t;
                string[] logResourceList = query.ToArray();
                foreach(var resourceName in logResourceList)
                {
                    string[] temp = resourceName.Split('.');
                    string filePath = folderPath + Path.DirectorySeparatorChar + temp[temp.Length - 2] + "." + temp[temp.Length - 1];
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (var fileStream = File.Create(filePath))
                    {
                        stream.CopyTo(fileStream);
                    }
                } 
            }
            
        }
        #endregion

        #region private members
        private static bool initialized = false;
        private static bool isLogAllowed = false;
        #endregion

        #region General Write / WriteLine
        /// <summary>
        /// Internal method to actually Write using Trace
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        internal static void Write(string category, string message)
        {
            //if (!useLog4)
            //{
            //    Trace.Write(message, category);
            //}
            //else
            //{
            // log4 uses WriteLine only
            WriteLine(category, message);
            //}
        }

        internal static void Write(string message)
        {
            //if (!useLog4)
            //{
            //    Trace.Write(message, category);
            //}
            //else
            //{
            // log4 uses WriteLine only
            //Trace.Write(message);
            //}
        }
        /// <summary>
        /// Internal method to actually WriteLine using Trace
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        internal static void WriteLine(string category, string message)
        {
            try
            {
                TestContext context = TestContext.CurrentContext;
                if (isLogAllowed && context != null && context.Test != null)
                {
                    string className = context.Test.ClassName;
                    if (tempLog.ContainsKey(className))
                    {
                        BaseListener listener = tempLog[className];
                        listener.WriteLine(message, category);
                    }
                }
            }
            catch (Exception ex)
            {
                if (isLogAllowed)
                {
                    Console.WriteLine("Exception: {0}. Stacktrace: {1}", ex.Message, ex.StackTrace);
                }
                if (CurrentListener != null)
                {
                    CurrentListener.WriteLine(category, message);
                }
            }
            
        }

        //internal static void WriteLine(string message)
        //{
        //    Trace.WriteLine(message);
        //}

        /// <summary>
        /// General Write Line method
        /// It prints out "Date LogLevel Message args"
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void WriteLine(LogLevel level, string message, params object[] args)
        {
            WriteLine(level, string.Format(message, args));
        }

        /// <summary>
        /// General Write Line method
        /// It prints out "Date LogLevel Message"
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public static void WriteLine(LogLevel level, string message)
        {
            WriteLine(level.ToString(), message);
        }
        #endregion

        #region Info
        /// <summary>
        /// WriteLine with LogLevel.Info
        /// </summary>
        /// <param name="message">message</param>
        public static void Info(string message)
        {
            WriteLine(LogLevel.Info, message);
        }
        /// <summary>
        /// WriteLine with LogLevel.Info
        /// </summary>
        /// <param name="format">string format</param>
        /// <param name="args">arguments</param>
        public static void Info(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }
        #endregion

        #region Debug
        /// <summary>
        /// WriteLine with LogLevel.Debug
        /// </summary>
        /// <param name="message">message</param>
        public static void Debug(string message)
        {
            WriteLine(LogLevel.Debug, message);
        }
        /// <summary>
        /// WriteLine with LogLevel.Debug
        /// </summary>
        /// <param name="format">string format</param>
        /// <param name="args">arguments</param>
        public static void Debug(string format, params object[] args)
        {
            Debug(string.Format(format, args));
        }
        #endregion

        #region Warn
        /// <summary>
        /// WriteLine with LogLevel.Warn
        /// </summary>
        /// <param name="message">message</param>
        public static void Warn(string message)
        {
            WriteLine(LogLevel.Warn, message);
        }
        /// <summary>
        /// WriteLine with LogLevel.Warn
        /// </summary>
        /// <param name="format">string format</param>
        /// <param name="args">arguments</param>
        public static void Warn(string format, params object[] args)
        {
            Warn(string.Format(format, args));
        }
        /// <summary>
        /// WriteLine e.ToString() with LogLevel.Warn
        /// </summary>
        /// <param name="e">Exception</param>
        public static void Warn(Exception e)
        {
            Warn(e.ToString());
        }
        #endregion

        #region Error
        /// <summary>
        /// WriteLine with LogLevel.Error
        /// </summary>
        /// <param name="message">message</param>
        public static void Error(string message)
        {
            WriteLine(LogLevel.Error, message);
        }
        /// <summary>
        /// WriteLine with LogLevel.Error
        /// </summary>
        /// <param name="format">string format</param>
        /// <param name="args">arguments</param>
        public static void Error(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }
        /// <summary>
        /// WriteLine e.ToString() with LogLevel.Error
        /// </summary>
        /// <param name="e">Exception</param>
        public static void Error(Exception e)
        {
            Error(e.ToString());
        }
        #endregion

        /// <summary>
        /// Print a test case description to the first standard output.
        /// </summary>
        public static void PrintTestCaseDescription()
        {
            TestContext context = TestContext.CurrentContext;
            if (isLogAllowed && context != null && context.Test != null)
            {
                string className = context.Test.ClassName;
                if (tempLog.ContainsKey(className))
                {
                    BaseListener listener = tempLog[className];
                    listener.LogTestCaseDescription();
                    CurrentListener = listener;
                }
            }
        }

        public static void PrintTestCaseTeardown()
        {
            TestContext context = TestContext.CurrentContext;
            if (isLogAllowed && context != null && context.Test != null)
            {
                string className = context.Test.ClassName;
                if (tempLog.ContainsKey(className))
                {
                    BaseListener listener = tempLog[className];
                    listener.LogTestCaseResult();
                    CurrentListener = listener;

                }
            }
        }

        public static void PrintTestInit()
        {
            InitializeLogging();
            //Trace.WriteLine("Setup", "test");
            TestContext context = TestContext.CurrentContext;
            if (isLogAllowed && context != null && context.Test != null)
            {
                string className = context.Test.ClassName;
                if (!tempLog.ContainsKey(className))
                {
                    tempLog[className] = new HtmlListener();
                }
                CurrentListener = tempLog[className];
            }
        }
        public static void PrintTestEnd()
        {
            if(isLogAllowed)
            {
                foreach (var item in tempLog)
                {
                    using (Stream htmlStream = File.Open(HtmlLogFilePath + item.Key + ".html", FileMode.Create))
                    {
                        using (TextWriter writer = new StreamWriter(htmlStream))
                        {
                            item.Value.WriteToOutput(writer, logLevel);
                        }
                    }
                }
            }
            
        }

        public static void PrintTestFixtureSetup()
        {
            try
            {
                TestContext context = TestContext.CurrentContext;
                if (isLogAllowed && tempLog.Count > 0 && context != null && context.Test != null)
                {
                    string className = context.Test.ClassName;
                    if (!tempLog.ContainsKey(className))
                    {
                        tempLog[className] = new HtmlListener();
                    }
                    BaseListener listener = tempLog[className];
                    CurrentListener = listener;
                    listener.LogSetUpDescription();
                }
            } 
            catch (Exception ex)
            {
                if (isLogAllowed)
                {
                    Console.WriteLine("Exception: {0}. Stacktrace: {1}", ex.Message, ex.StackTrace);
                }
            }
            
        }

        public static void PrintTestFixtureTearDown()
        {
            try
            {
                TestContext context = TestContext.CurrentContext;
                if (isLogAllowed && context != null && context.Test != null)
                {
                    string className = context.Test.ClassName;
                    if (tempLog.ContainsKey(className))
                    {
                        BaseListener listener = tempLog[className];
                        CurrentListener = listener;
                        listener.LogTeardownDescription();
                    }
                }
            } 
            catch (Exception ex)
            {
                if (isLogAllowed)
                {
                    Console.WriteLine("Exception: {0}. Stacktrace: {1}", ex.Message, ex.StackTrace);
                }
            }
        }

        //public static void PrintTestSumary()
        //{
        //    Trace.WriteLine("Sumary", "test");
        //}

        public static void PrintRequest(HttpRequestMessage request)
        {
            try
            {
                TestContext context = TestContext.CurrentContext;
                if (isLogAllowed && context != null && context.Test != null)
                {
                    string className = context.Test.ClassName;
                    if (tempLog.ContainsKey(className))
                    {
                        BaseListener listener = tempLog[className];
                        listener.LogRequest(request);
                    }
                }
            }
            catch (Exception ex)
            {
                if (isLogAllowed)
                {
                    Console.WriteLine("Exception: {0}. Stacktrace: {1}", ex.Message, ex.StackTrace);
                }
                if (CurrentListener != null)
                {
                    CurrentListener.LogRequest(request);
                }
            }
        }

        public static void RequestData(string data)
        {
            try
            {
                TestContext context = TestContext.CurrentContext;
                if (isLogAllowed && context != null && context.Test != null)
                {
                    string className = context.Test.ClassName;
                    if (tempLog.ContainsKey(className))
                    {
                        BaseListener listener = tempLog[className];
                        listener.LogRequestData(data);
                    }
                }
            }
            catch (Exception ex)
            {
                if (isLogAllowed)
                {
                    Console.WriteLine("Exception: {0}. Stacktrace: {1}", ex.Message, ex.StackTrace);
                }
                if (CurrentListener != null)
                {
                    CurrentListener.LogRequestData(data);
                }
            }
        }

        public static void PrintResponse(HttpResponseMessage response)
        {
            try
            {
                TestContext context = TestContext.CurrentContext;
                if (isLogAllowed && context != null && context.Test != null)
                { 
                    string className = context.Test.ClassName;
                    if (tempLog.ContainsKey(className))
                    {
                        BaseListener listener = tempLog[className];
                        listener.LogResponse(response);
                    }
                }
            }
            catch (Exception ex)
            {
                if(isLogAllowed)
                {
                    Console.WriteLine("Exception: {0}. Stacktrace: {1}", ex.Message, ex.StackTrace);
                }
                if (CurrentListener != null)
                {
                    CurrentListener.LogResponse(response);
                }
            }
        }
        /// <summary>
        /// Print a line of info to standard out put
        /// </summary>
        /// <param name="info"></param>

        #region Indent
        /// <summary>
        /// Default Indent size
        /// </summary>
        public static int DefaultIndentSize = Trace.IndentSize;

        /// <summary>
        /// Current Indent Size
        /// Commonly used in the wrapper
        /// </summary>
        public static int IndentSize
        {
            set
            {
                Trace.IndentSize = value;
            }
            get
            {
                return Trace.IndentSize;
            }
        }

        /// <summary>
        /// If get, true if IndentSize is bigger than 0
        /// If set, IndentSize is set to Default Indent Size
        /// </summary>
        public static bool IsIndentEnabled
        {
            get
            {
                return IndentSize > 0;
            }
            set
            {
                if (value)
                {
                    if (DefaultIndentSize <= 0)
                    {
                        Log.Info("Default Indent size cannot be less than or equal to 0.");
                        Log.Info("Please set proper DefaultIndentSize before enabling Indent");
                    }
                    IndentSize = DefaultIndentSize;
                }
            }
        }
        #endregion
    }
}
