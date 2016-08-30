using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary.Logging.LogModel
{
    public class TestSuite : TestObjectBase
    {
        public string Type
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("type"))
                    return base.PropertyDictionary["type"] as string;
                return null;
            }
            set { base.PropertyDictionary["type"] = value; }
        }

        public string Name
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("name"))
                    return base.PropertyDictionary["name"] as string;
                return null;
            }
            set { base.PropertyDictionary["name"] = value; }
        }

        public string Executed
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("executed"))
                    return base.PropertyDictionary["executed"] as string;
                return null;
            }
            set { base.PropertyDictionary["executed"] = value; }
        }

        public string Result
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("result"))
                    return base.PropertyDictionary["result"] as string;
                return null;
            }
            set { base.PropertyDictionary["result"] = value; }
        }

        public string Success
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("success"))
                    return base.PropertyDictionary["success"] as string;
                return null;
            }
            set { base.PropertyDictionary["success"] = value; }
        }

        public string Time
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("time"))
                    return base.PropertyDictionary["time"] as string;
                return null;
            }
            set { base.PropertyDictionary["time"] = value; }
        }

        public string Asserts
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("asserts"))
                    return base.PropertyDictionary["asserts"] as string;
                return null;
            }
            set { base.PropertyDictionary["asserts"] = value; }
        }

        public string Description
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("description"))
                    return base.PropertyDictionary["description"] as string;
                return null;
            }
            set { base.PropertyDictionary["description"] = value; }
        }

        public List<string> Categories
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("categories"))
                    return base.PropertyDictionary["categories"] as List<string>;
                return null;
            }
            set { base.PropertyDictionary["categories"] = value; }
        }

        public List<TestCase> TestCases
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("TestCases"))
                    return base.PropertyDictionary["TestCases"] as List<TestCase>;
                return null;
            }
            set { base.PropertyDictionary["TestCases"] = value; }
        }

        public List<TestSuite> TestSuites
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("TestSuites"))
                    return base.PropertyDictionary["TestSuites"] as List<TestSuite>;
                return null;
            }
            set { base.PropertyDictionary["TestSuites"] = value; }
        }

        public TestSuite ParentTestSuite
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("ParentTestSuite"))
                    return base.PropertyDictionary["ParentTestSuite"] as TestSuite;
                return null;
            }
            set { base.PropertyDictionary["ParentTestSuite"] = value; }
        }
    }
}
