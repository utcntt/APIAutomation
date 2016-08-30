using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary.Logging.LogModel
{
    public class TestEnvironment: TestObjectBase
    {
        public string NunitVersion
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("nunit-version"))
                    return base.PropertyDictionary["nunit-version"] as string;
                return null;
            }
            set { base.PropertyDictionary["nunit-version"] = value; }
        }

        public string ClrVersion
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("clr-version"))
                    return base.PropertyDictionary["clr-version"] as string;
                return null;
            }
            set { base.PropertyDictionary["clr-version"] = value; }
        }

        public string OsVersion
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("os-version"))
                    return base.PropertyDictionary["os-version"] as string;
                return null;
            }
            set { base.PropertyDictionary["os-version"] = value; }
        }

        public string Platform
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("platform"))
                    return base.PropertyDictionary["platform"] as string;
                return null;
            }
            set { base.PropertyDictionary["platform"] = value; }
        }

        public string Cwd
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("cwd"))
                    return base.PropertyDictionary["cwd"] as string;
                return null;
            }
            set { base.PropertyDictionary["cwd"] = value; }
        }

        public string MachineName
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("machine-name"))
                    return base.PropertyDictionary["machine-name"] as string;
                return null;
            }
            set { base.PropertyDictionary["machine-name"] = value; }
        }

        public string User
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("user"))
                    return base.PropertyDictionary["user"] as string;
                return null;
            }
            set { base.PropertyDictionary["user"] = value; }
        }

        public string UserDomain
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("user-domain"))
                    return base.PropertyDictionary["user-domain"] as string;
                return null;
            }
            set { base.PropertyDictionary["user-domain"] = value; }
        }
    }
}
