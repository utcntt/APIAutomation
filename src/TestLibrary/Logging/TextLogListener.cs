using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    /// <summary>
    /// Text Log Listener
    /// </summary>
    public class TextLogListener : BaseListener
    {
        #region Constructor
        /// <summary>
        /// Constructs the console listener with the given header and footer
        /// </summary>
        /// <param name="header"></param>
        /// <param name="footer"></param>
        public TextLogListener(string header = null, string footer = null)
            : base(header, footer)
        {
        }

        ///// <summary>
        ///// Constructs the console listener with the given header and footer
        ///// </summary>
        ///// <param name="header"></param>
        ///// <param name="footer"></param>
        //public TextLogListener(StreamWriter s, string header, string footer)
        //    : base(s, header, footer)
        //{
        //}

        //internal override void PrintFixtureTearDown()
        //{
        //    base.PrintFixtureTearDown();
        //    PrintSumary();
        //}
        #endregion
    }
}
