using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary.Logging.LogModel
{
    /// <summary>
    /// This class contains all information for a NUnit test.
    /// </summary>
    public class TestResult : TestObjectBase
    {
        public TestEnvironment Environment
        {
            get
            {
                if(base.PropertyDictionary.ContainsKey("environment"))
                    return base.PropertyDictionary["environment"] as TestEnvironment;
                return null;
            }
            set { base.PropertyDictionary["environment"] = value; }
        }

        public CultureInfo CultureInfo
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("culture-info"))
                    return base.PropertyDictionary["culture-info"] as CultureInfo;
                return null;
            }
            set { base.PropertyDictionary["culture-info"] = value; }
        }

        public string Name
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("name"))
                    return base.PropertyDictionary["name"] as string;
                return null;
            }
            set { base.PropertyDictionary["name"] = value;  }
        }

        public string Total
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("total"))
                    return base.PropertyDictionary["total"] as string;
                return null;
            }
            set { base.PropertyDictionary["total"] = value; }
        }

        public string Errors
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("errors"))
                    return base.PropertyDictionary["errors"] as string;
                return null;
            }
            set { base.PropertyDictionary["errors"] = value; }
        }

        public string Failures
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("failures"))
                    return base.PropertyDictionary["failures"] as string;
                return null;
            }
            set { base.PropertyDictionary["failures"] = value; }
        }

        public string NotRun
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("not-run"))
                    return base.PropertyDictionary["not-run"] as string;
                return null;
            }
            set { base.PropertyDictionary["not-run"] = value; }
        }

        public string Inconclusive
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("inconclusive"))
                    return base.PropertyDictionary["inconclusive"] as string;
                return null;
            }
            set { base.PropertyDictionary["inconclusive"] = value; }
        }

        public string Ignored
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("ignored"))
                    return base.PropertyDictionary["ignored"] as string;
                return null;
            }
            set { base.PropertyDictionary["ignored"] = value; }
        }

        public string Skipped
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("skipped"))
                    return base.PropertyDictionary["skipped"] as string;
                return null;
            }
            set { base.PropertyDictionary["skipped"] = value; }
        }

        public string Invalid
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("invalid"))
                    return base.PropertyDictionary["invalid"] as string;
                return null;
            }
            set { base.PropertyDictionary["invalid"] = value; }
        }

        public string Date
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("date"))
                    return base.PropertyDictionary["date"] as string;
                return null;
            }
            set { base.PropertyDictionary["date"] = value; }
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

        public TestSuite TestSuite
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("test-suite"))
                    return base.PropertyDictionary["test-suite"] as TestSuite;
                return null;
            }
            set { base.PropertyDictionary["test-suite"] = value; }
        }
    }
}
