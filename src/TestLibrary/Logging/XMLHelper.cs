using TestLibrary.Logging.LogModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NUnitReportToExcel
{
    public class XMLHelper
    {
        /// <summary>
        /// Create an intance of TestResult from  a XLM file
        /// </summary>
        /// <typeparam name="TResource">Resource type: Job, Field, User...</typeparam>
        /// <param name="filePath">Full path of the XML file</param>
        /// <returns>An instance of WriteDataBase.</returns>
        public static TestResult ParseXMLFile(string filePath)
        {
            try
            {
                TestResult result = new TestResult();
                XDocument doc = XDocument.Load(filePath);

                XElement resultElement = doc.Root;
                ParseAttribute(resultElement, result);
                if (resultElement.Element("environment") != null)
                {
                    TestEnvironment environment = new TestEnvironment();
                    ParseAttribute(resultElement.Element("environment"), environment);
                    result.Environment = environment;
                }

                if (resultElement.Element("culture-info") != null)
                {
                    CultureInfo culture = new CultureInfo();
                    ParseAttribute(resultElement.Element("culture-info"), culture);
                    result.CultureInfo = culture;
                }

                if (resultElement.Element("test-suite") != null)
                {
                    TestSuite suite = ParseTestSuite(resultElement.Element("test-suite"));
                    result.TestSuite = suite;
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There is an error when parsing the NUnit report file.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack trace:  {0}", ex.StackTrace);
                Console.WriteLine("Source:  {0}", ex.Source);
                return null;
            }
            
        }

        internal static void ParseAttribute(XElement element, TestObjectBase result)
        {
            if (element.HasAttributes)
                foreach (XAttribute attribute in element.Attributes())
                {
                    result.PropertyDictionary[attribute.Name.LocalName] = attribute.Value;
                }
        }

        internal static TestSuite ParseTestSuite(XElement element)
        {
            TestSuite result = new TestSuite();
            ParseAttribute(element, result);
            if (element.Element("categories") != null)
            {
                List<string> categoryList = new List<string>();
                XElement categories = element.Element("categories");
                foreach (XElement category in categories.Elements("category"))
                {
                    categoryList.Add(category.Attribute("name").Value);
                }
                result.Categories = categoryList;
            }
            if (element.Element("results") != null)
            {
                XElement subResult = element.Element("results");
                List<TestCase> testCases = new List<TestCase>();
                List<TestSuite> testSuites = new List<TestSuite>();
                foreach (XElement subResultElement in subResult.Elements())
                {
                    if (subResultElement.Name.LocalName == "test-suite")
                    {
                        TestSuite subTestSuite = ParseTestSuite(subResultElement);
                        subTestSuite.ParentTestSuite = result;
                        testSuites.Add(subTestSuite);
                    }
                    else if (subResultElement.Name.LocalName == "test-case")
                    {
                        TestCase testCase = ParseTestCase(subResultElement);
                        testCase.ParentTestSuite = result;
                        testCases.Add(testCase);
                    }
                }
                result.TestCases = testCases;
                result.TestSuites = testSuites;
            }
            return result;
        }

        internal static TestCase ParseTestCase(XElement element)
        {
            TestCase result = new TestCase();
            ParseAttribute(element, result);
            if (element.Element("categories") != null)
            {
                List<string> categoryList = new List<string>();
                XElement categories = element.Element("categories");
                foreach (XElement category in categories.Elements("category"))
                {
                    categoryList.Add(category.Attribute("name").Value);
                }
                result.Categories = categoryList;
            }
            if (element.Element("failure") != null)
            {
                Failure failure = new Failure();
                XElement failureElement = element.Element("failure");
                foreach(XElement subElement in failureElement.Elements())
                {
                    failure.PropertyDictionary[subElement.Name.LocalName] = subElement.Value;
                }
                result.Failure = failure;
            }
            return result;
        }
    }
}
