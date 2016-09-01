using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.ModelBase
{
    /// <summary>
    /// Base class for all Resource created by XML
    /// </summary>
    public class XmlModel: Model
    {
        /// <summary>
        /// XML Tag name used in case of serializing XML.
        /// This field stores value for ActualXMLTag and it is used for Options purposes only
        /// </summary>
        protected string actualXMLTag;

        /// <summary>
        /// XML Tag name is used in case of serializing XML. 
        /// </summary>
        public string ActualXMLTag
        {
            get
            {
                return actualXMLTag;
            }
            set
            {
                actualXMLTag = value;
            }
        }
    }
}
