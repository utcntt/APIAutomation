using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QnAMaker.Test.Model
{
    public class AnswerResponseData : APICore.Model.Model
    {
        [JsonProperty("answer")]
        public string Answer
        {
            get
            {
                if (DictionaryValues.ContainsKey("answer"))
                    return (string)base.DictionaryValues["answer"];
                return null;
            }
            set
            {
                base.DictionaryValues["answer"] = value;
            }
        }

        [JsonProperty("score")]
        public string Score
        {
            get
            {
                if (DictionaryValues.ContainsKey("score"))
                    return (string)base.DictionaryValues["score"];
                return null;
            }
            set
            {
                base.DictionaryValues["Score"] = value;
            }
        }
    }
}
