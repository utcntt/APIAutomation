using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Common;
using NUnit.Framework.Internal;

namespace TestLibrary
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BugAttribute : PropertyAttribute
    {
        public const string PropertyName = "Bug";

        public BugAttribute(int ticketNumber) : this(ticketNumber.ToString())
        {
            
        }

        public BugAttribute(string bugSpec) : base(PropertyName, bugSpec)
        {

        }
    }
}
