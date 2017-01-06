using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICore.Model;
using Newtonsoft.Json;

namespace QnAMaker.Test.Model
{
    public class QuestionRequest : APICore.Model.Model
    {
        [JsonProperty("question")]
        public string Question
        {
            get
            {
                if (DictionaryValues.ContainsKey("question"))
                    return (string)base.DictionaryValues["question"];
                return null;
            }
            set
            {
                base.DictionaryValues["question"] = value;
            }
        }
    }
}
