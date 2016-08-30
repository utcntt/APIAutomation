using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    /// <summary>
    /// Console Listener
    /// </summary>
    public class ConsoleListener : BaseListener
    {
        #region Constructor
        /// <summary>
        /// Constructs the console listener with the given header and footer
        /// </summary>
        /// <param name="header"></param>
        /// <param name="footer"></param>
        public ConsoleListener(string header = null, string footer = null)
            : base(Console.Out, header, footer)
        {
            //this.Writer = Console.Out;
        }

        ///// <summary>
        ///// Constructs the console listener with the given header and footer
        ///// </summary>
        ///// <param name="header"></param>
        ///// <param name="footer"></param>
        //public ConsoleListener(string header, string footer)
        //    : base(Console.OpenStandardOutput(), header, footer)
        //{
        //    this.Writer = Console.Out;
        //}
        #endregion

        #region Write/ Writeline
        //public override void Write(string message)
        //{
        //    Console.Write(message);
        //    //base.WriteLine(message);
        //}

        //public override void WriteLine(string message, string category)
        //{
        //    if (IsShowing(category))
        //    {
        //        SetColor(category);
        //        Console.WriteLine(FormatMessage(message, category));
        //        //base.WriteLine(message, category);
        //        Console.ResetColor();
        //    }
        //}

        public void SetColor(string category)
        {
            switch (category.ToLower())
            {
                case "trace":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case "debug":
                case "enter":
                case "exit":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case "warn":
                case "warning":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "error":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "blocked":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case "success":
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case "pass":
                case "passed":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "fail":
                case "failed":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "repro":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "verify":
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case "time":
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case "info":
                case "none":
                default:
                    //Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }
        #endregion

        #region Close
        ///// <summary>
        ///// Closes the writer and writes the footer if available
        ///// </summary>
        //public override void Close()
        //{
        //    if (!string.IsNullOrEmpty(footer))
        //    {
        //        Console.WriteLine(footer);
        //    }
        //
        //    base.Close();
        //}

        #endregion
    }
}
