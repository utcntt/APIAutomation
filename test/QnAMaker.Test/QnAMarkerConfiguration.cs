using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestLibrary;
using APICore;

namespace QnAMaker.Test
{
    public class QnAMarkerConfiguration: TestConfig
    {
        public static string KnowledgeBaseID
        {
            get
            {
                return GetValueFromConfig("KnowledgeBaseID");
            }
        }

        public static string SubscriptionKey
        {
            get
            {
                return GetValueFromConfig("SubscriptionKey");
            }
        }
    }
}
