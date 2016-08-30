using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Linq;

namespace TestLibrary
{
    /// <summary>
    /// Test Logger
    /// Authored by Dimas Buditanoyo
    /// This class uses 2 methods:
    ///    1. (default) Simple logging to local file 
    ///    2. Using Log4Net library to log with configuration (currently disabled)
    /// 
    /// <example>
    /// Common Methods:
    /// Log.Info()      : Information. Written to Console.
    /// Log.Debug()     : Information, but can be ignored. Only useful for debugging.
    /// Log.Warn()      : Error, but can be ignored. Written to Console.
    /// Log.Error()     : Error, but need attention. Written to Console.
    /// Log.ReproStep() : Useful to record Repro Step of test case.
    ///     The recorded repro steps can be accessed from Log.ReproSteps
    /// using(Log.ReproStepWrapper()) : useful to understand what scope of Repro step it is in
    /// using(Log.EnterExitWrapper()) : useful to understand what method it is currently in
    /// using(Log.TimeSpanWrapper()) : useful to understand the time spent to do the statements in the block
    /// </example>
    /// </summary>
    public partial class Log
    {
        #region Public Members
        public static string NOW = DateTime.Now.ToString("yyyy_MM_dd-HH_mm");
        public static string LogFilePath = string.Format("{2}{3}Log{1}APIAuto.log", NOW, Path.DirectorySeparatorChar, Util.GetTestDirectory(), Path.DirectorySeparatorChar);
        public static string HtmlLogFilePath = string.Format("{2}{3}Log{1}APIAutoLog.html", NOW, Path.DirectorySeparatorChar, Util.GetTestDirectory(), Path.DirectorySeparatorChar);
        //public static string PerformanceFilePath = string.Format("Log{1}APIAuto.performance.html", NOW, Path.DirectorySeparatorChar);

        /// <summary>
        /// Log Footer. This is written at the end of the log.
        /// </summary>
        public static string Footer
        {
            set
            {
                foreach (TraceListener listener in Trace.Listeners)
                {
                    ((BaseListener)listener).Footer = value;
                }
            }
            get
            {
                if (Trace.Listeners.Count > 0)
                {
                    // the first Listener's footer is the representative,
                    // because there is no difference between other Listeners' footer
                    BaseListener listener = (BaseListener)Trace.Listeners[0];
                    return listener.Footer;
                }
                return string.Empty;
            }
        }

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

        ~Log()
        {
            Log.Dispose(false);
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
            //if (safe)
            //{
            //    if (PerformanceSummary != null && PerformanceSummary.Count > 0)
            //    {
            //        PerformanceSummary.Clear();
            //    }
            //}
            initialized = false;
        }

        /// <summary>
        /// Initialize logging using Trace.Listener
        /// </summary>
        internal static void InitializeLogging()
        {
            if (!initialized)
            {
                CreateDirectoryIfItDoesNotExist(LogFilePath); // Assuming other log files are located in the same directory
                CreateHtmlResourceFiles(HtmlLogFilePath);
                try
                {
                    _text = new StreamWriter(System.IO.File.Open(LogFilePath, FileMode.Create)); //  Create or Overwrite Existing
                    _htmlStream = File.Open(HtmlLogFilePath, FileMode.Create); //  Create or Overwrite Existing
                    StreamWriter htmlWriter = new StreamWriter(_htmlStream);
                    if (Trace.Listeners.Count > 0)
                    {
                        Trace.Listeners.Clear();
                    }
                    Trace.AutoFlush = true;
                    Trace.IndentSize = 3; // DefaultIndentSize; 

                    // Trace.Listeners.Add(new ConsoleTraceListener()); // Native Console Listener (NUnit)
                    Trace.Listeners.Add(new ConsoleListener("API Automation Console Log", null)); // For NUnit
                    Trace.Listeners.Add(new TextLogListener(_text, "API Automation Text Log", null)); // for txt log file
                    Trace.Listeners.Add(new HtmlListener(htmlWriter, "API Automation Html Log", null)); // For Html log
                    Trace.Listeners.Add(new NUnitListener("API Automation Html Log", null)); // For Html log
                                                                                                        //Trace.Listeners.Add(new HtmlPerfListener(_performanceStream, "UI Automation perspective Performance Log", null)); // For Performance

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
        private static StreamWriter _text; //  Create or Overwrite Existing
        private static Stream _htmlStream; //  Create or Overwrite Existing
        private static bool initialized = false;

        //private static Stream _performanceStream = File.Open(PerformanceFilePath, FileMode.Create); //  Create or Overwrite Existing
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
            Trace.Write(message);
            //}
        }
        /// <summary>
        /// Internal method to actually WriteLine using Trace
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        internal static void WriteLine(string category, string message)
        {
            //if (!useLog4)
            //{
            Trace.WriteLine(message, category);
            //}
            //else // if (useLog4)
            //{
            //    switch (category.ToLower())
            //    {
            //        case "debug":
            //            log4.Debug(message);
            //            break;
            //        case "error":
            //            log4.Error(message);
            //            break;
            //        case "warn":
            //            log4.Warn(message);
            //            break;
            //        case "info":
            //        default:
            //            log4.Info(message);
            //            //Console.WriteLine(message);
            //            break;
            //    }
            //}
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
            Trace.WriteLine("SetUp", "testcase");
        }

        public static void PrintTestCaseTeardown()
        {
            Trace.WriteLine("Teardown", "testcase");
        }

        public static void PrintTestFixtureSetup()
        {
            InitializeLogging();
            Trace.WriteLine("Setup", "fixture");
        }

        public static void PrintTestFixtureTeardown()
        {
            Trace.WriteLine("Teardown", "fixture");
        }

        public static void PrintTestFixtureSumary()
        {
            Trace.WriteLine("Sumary", "fixture");
        }

        public static void PrintRequest(HttpWebRequest request)
        {
            Trace.WriteLine(request, "Request");
        }

        public static void RequestData(string data)
        {
            Trace.WriteLine(data, "RequestData");
        }

        public static void PrintResponse(HttpWebResponse response)
        {
            Trace.WriteLine(response, "Response");
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
