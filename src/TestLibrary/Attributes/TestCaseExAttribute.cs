using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLibrary
{
    public class TestCaseExAttribute : TestCaseAttribute
    {
        public TestCaseExAttribute(object arg) : base(arg)
        { }
        //
        // Summary:
        //     Construct a TestCaseAttribute with a two arguments
        //
        // Parameters:
        //   arg1:
        //
        //   arg2:
        public TestCaseExAttribute(object arg1, object arg2) : base(arg1, arg2)
        { }
        //
        // Summary:
        //     Construct a TestCaseAttribute with a three arguments
        //
        // Parameters:
        //   arg1:
        //
        //   arg2:
        //
        //   arg3:
        public TestCaseExAttribute(object arg1, object arg2, object arg3): base(arg1, arg2, arg3) { }

        public TestCaseExAttribute(params object[] args) : base(args) { }

        public string Bug
        {
            get
            {
                return Properties.ContainsKey("Bug") ? string.Join(",", Properties["Bug"]) : string.Empty;
            }
            set
            {
                if(this.Properties.ContainsKey("Bug"))
                {
                    Properties["Bug"] = Properties["Bug"].OfType<string>().Concat(value.Split(',')).Where(str => str != string.Empty).ToList();
                }
                else
                {
                    Properties["Bug"] = value.Split(',').ToList();
                }
            }
        }
    }
}
