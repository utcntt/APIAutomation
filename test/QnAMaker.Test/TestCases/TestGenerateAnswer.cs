using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using APICore;
using TestLibrary;
using QnAMaker.Test.Model;
using QnAMaker.Test.API;

namespace QnAMaker.Test.TestCases
{
    [TestFixture]
    public class TestGenerateAnswer : TestBase
    {
        [TestCaseEx("Hi", "Xin chào")]
        public void TestGoodAnswer(string question, string expectedAnswer)
        {
            QuestionRequest requestData = new QuestionRequest() { Question = question };
            AnswerResponseData result = AnswerAPI.GenerateAnswerAsync(requestData).Result;
            Assert.That(result, Is.Not.Null, "Response Data is null! There might be an issue when calling the request!");
            Assert.That(result.Answer, Does.Contain(expectedAnswer), "The answer doenn't contain the expected string");
        }
    }
}
