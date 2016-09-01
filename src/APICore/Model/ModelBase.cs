using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace APICore.ModelBase
{
    /// <summary>
    /// 
    /// </summary>
    public class Model
    {
        private Dictionary<string, object> dictionaryValues = new Dictionary<string,object>();

        /// <summary>
        /// Dictionary is used to generate JSON string.
        /// </summary>
        //[ScriptIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public Dictionary<string, object> DictionaryValues
        {
            get
            {
                return dictionaryValues;
            }
            set
            {
                dictionaryValues = value;
            }
        }
    }
}
