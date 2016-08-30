using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    /// <summary>
    /// Interface for Trace Listener
    /// </summary>
    public interface IListener
    {
        /// <summary>
        /// Writes a message
        /// </summary>
        /// <param name="message"></param>
        void Write(string message);
        /// <summary>
        /// Writes a message and appends a newline
        /// </summary>
        /// <param name="message"></param>
        void WriteLine(string message);
        /// <summary>
        /// Writes a message with category and appends a newline
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        void WriteLine(string message, string category);
    }
}
