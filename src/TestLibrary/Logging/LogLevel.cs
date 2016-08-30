using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    /// <summary>
    /// Common LogLevel in UI Automation
    /// Authored by Dimas Buditanoyo
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// No log level
        /// </summary>
        None = 0,
        /// <summary>
        /// Debug level
        /// </summary>
        Debug = 1 << 1,
        /// <summary>
        /// Informational level
        /// </summary>
        Info = 1 << 2,
        /// <summary>
        /// Informational level
        /// </summary>
        Warn = 1 << 3,
        /// <summary>
        /// Informational level
        /// </summary>
        Error = 1 << 4,
        /// <summary>
        /// Request/response level
        /// </summary>
        Request = 1 << 5,
        /// <summary>
        /// Test case level
        /// </summary>
        TestCase = 1 << 6,
        /// <summary>
        /// Test fixture level
        /// </summary>
        Fixture = 1 << 7,
        /// <summary>
        /// Namespace level
        /// </summary>
        Namespace = 1 << 8,
        /// <summary>
        /// Assembly level
        /// </summary>
        Assembly = 1 << 9,
    }

    static class LogLevelExtension
    {
        public static LogLevel ToLogLevel(this string category)
        {
            LogLevel result = LogLevel.None;

            // Enum.TryParse<LogLevel>(category, out result); too slow

            switch (category.ToLower())
            {
                case "assembly":
                    result = LogLevel.Assembly;
                    break;
                case "namespace":
                    result = LogLevel.Namespace;
                    break;
                case "fixture":
                    result = LogLevel.Fixture;
                    break;
                case "testCase":
                    result = LogLevel.TestCase;
                    break;
                case "request":
                    result = LogLevel.Request;
                    break;
                case "info":
                    result = LogLevel.Info;
                    break;
                case "warn":
                    result = LogLevel.Warn;
                    break;
                case "error":
                    result = LogLevel.Error;
                    break;
                case "debug":
                    result = LogLevel.Debug;
                    break;
                case "none":
                default:
                    result = LogLevel.None;
                    break;
            }
            return result;
        }
    }
}
