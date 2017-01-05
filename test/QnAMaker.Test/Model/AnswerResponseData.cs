using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QnAMaker.Test.Model
{
    public class AnswerResponseData : APICore.Model.Model
    {
        [JsonProperty("Answer")]
        public string Answer { get; set; }

        [JsonProperty("Score")]
        public string Score { get; set; }
    }
}
