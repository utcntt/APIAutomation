using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary.Logging.LogModel
{
    public class Failure : TestObjectBase
    {
        public string Message
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("message"))
                    return base.PropertyDictionary["message"] as string;
                return null;
            }
            set
            {   base.PropertyDictionary["message"] = value; }
        }

        public string StackTrace
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("stack-trace"))
                    return base.PropertyDictionary["stack-trace"] as string;
                return null;
            }
            set { base.PropertyDictionary["stack-trace"] = value; }
        }
    }
}
