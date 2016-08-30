using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary.Logging.LogModel
{
    public class TestObjectBase
    {
        protected Dictionary<string, object> propertyDictionary = new Dictionary<string, object>();

        public Dictionary<string, object> PropertyDictionary
        {
            get
            {
                return propertyDictionary;
            }
        }
    }
}
