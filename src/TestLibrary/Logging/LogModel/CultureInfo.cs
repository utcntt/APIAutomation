using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary.Logging.LogModel
{
    public class CultureInfo: TestObjectBase
    {
        public string CurrentCulture
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("total")) return base.PropertyDictionary["current-culture"] as string;
                return null;
            }
            set { base.PropertyDictionary["current-culture"] = value; }
        }

        public string CurrentUICulture
        {
            get
            {
                if (base.PropertyDictionary.ContainsKey("total"))
                    return base.PropertyDictionary["current-uiculture"] as string;
                return null;
            }
            set { base.PropertyDictionary["current-uiculture"] = value; }
        }
    }
}
