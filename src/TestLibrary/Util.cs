using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.ComponentModel;
using Microsoft.Extensions.DependencyModel;
using NUnit.Framework.Internal;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestLibrary
{
    public static class Util
    {
        const int HEXADECIMAL = 16;
        const int UUID_BITS_PARTITION = 8;

        #region String Extension

        public static string[] GetKeywordList(this string str)
        {
            string[] result = null;
            if (!string.IsNullOrEmpty(str))
            {
                string[] temp = str.Trim().Split(' ');
                IEnumerable<string> query = from item in temp where !string.IsNullOrEmpty(item) select item;
                result = query.ToArray();
            }
            return result;
        }

        /// <summary>
        /// Extension for string to convert to string in lowercase
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToLowerString(this object str)
        {
            return str.ToString().ToLower();
        }

        /// <summary>
        /// Extension method to re-format string
        /// </summary>
        /// <param name="string">a composite string value to be formatted</param>
        /// <param name="value">value used to formatted string</param>
        /// <returns>formatted string</returns>
        public static string StringReformat(this string @string, string value)
        {
            return string.Format(@string, value);
        }

        /// <summary>
        /// Return true if exactly equals to one from expectations
        /// </summary>
        /// <param name="actual">string to contain</param>
        /// <param name="expectations">array of expectation strings</param>
        /// <param name="ignoreCase">ignore case</param>
        /// <returns></returns>
        public static bool EqualsToStrings(this string actual, string[] expectations, bool ignoreCase = true)
        {
            bool result = false;
            foreach (string str in expectations)
            {
                if (ignoreCase)
                {
                    result |= string.Equals(actual, str, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    result |= string.Equals(actual, str, StringComparison.Ordinal);
                }
                if (result)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Return true if able to contain at least one from expectations
        /// </summary>
        /// <param name="actual">string to contain</param>
        /// <param name="expectations">array of expectation strings</param>
        /// <returns></returns>
        public static bool ContainsStrings(this string actual, string[] expectations)
        {
            bool result = false;
            foreach (string str in expectations)
            {
                result |= actual.Contains(str);

                if (result)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Fills Left side of a string N times with the specified character
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="total">Total times the character will be included</param>
        /// <param name="character">The character used to fill string (default value is space)</param>
        /// <returns>The formated string</returns>
        public static string FillWithCharactersLeft(this string input, int total, char? character = null)
        {
            if (total < 1) { input.ToString(); }

            int totalLength = input.Length + total;

            if(null != character)
            {
                return input.PadLeft(totalLength, character.Value);
            }
            return input.PadLeft(totalLength);
        }

        /// <summary>
        /// Fills Right side of a string N times with the specified character
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="total">Total times the character will be included</param>
        /// <param name="character">The character used to fill string (default value is space)</param>
        /// <returns>The formated string</returns>
        public static string FillWithCharactersRight(this string input, int total, char? character = null)
        {
            if (total < 1) { input.ToString(); }

            int totalLength = input.Length + total;

            if (null != character)
            {
                return input.PadRight(totalLength, character.Value);
            }
            return input.PadRight(totalLength);
        }

        #endregion

        #region IList Extension
        /// <summary>
        /// Get Random item in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T Random<T>(this IList<T> items)
        {
            int index = GetRandomNumber(items.Count - 1);
            return items[index];
        }

        /// <summary>
        /// Get Random item in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="index">random index generated</param>
        /// <returns></returns>
        public static T Random<T>(this IList<T> items, out int index)
        {
            index = GetRandomNumber(items.Count - 1);
            return items[index];
        }

        public static TValue Random<TKey, TValue>(this IDictionary<TKey, TValue> items)
        {
            int index = GetRandomNumber(items.Count - 1);
            int i = 0;
            foreach (KeyValuePair<TKey, TValue> item in items)
            {
                if (i >= items.Count)
                    break;
                if (i == index)
                    return item.Value;
                i++;
            }
            return default(TValue);
        }
        #endregion

        #region Namespace and Method
        ///// <summary>
        ///// This will return the namespace and classname that calls this method.
        ///// </summary>
        ///// <returns></returns>
        //public static string GetTestMethod(int level = 1)
        //{
        //    StackTrace stackTrace = new StackTrace();
        //    MethodBase methodBase = stackTrace.GetFrame(level).GetMethod();
        //    string result = string.Format("{0}.{1}", methodBase.DeclaringType.FullName, methodBase.Name);
        //    return result;
        //}
        #endregion

        #region Random

        /// <summary>
        /// Returns nonnegative number
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int GetRandomNumber(int maxValue = int.MaxValue)
        {
            if (maxValue > 0)
            {
                return Rand.Next(maxValue);
            }
            Log.Info("Parameter is less than or equal to 0");
            return maxValue;
        }

        /// <summary>
        /// Get random number between minValue to maxValue
        /// value must be nonnegative number
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int GetRandomNumber(int minValue, int maxValue)
        {
            if (minValue < 0 || maxValue < 0 || maxValue < minValue)
            {
                Log.Info("Argument to get Random Number is incorrect");
            }
            else
            {
                return Rand.Next(minValue, maxValue);
            }
            return maxValue;
        }

        public static Random Rand = new Random();
        #endregion

        #region Unique String

        /// <summary>
        /// Unique string
        /// if length less than or equal to 0: using DateTime Now
        /// else using random character as long as length
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueString(int length = 0, bool withSpecialCharacters = true)
        {
            string result = string.Empty;
            if (length <= 0)
            {
                result = DateTime.Now.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                int number = '9' - '0' + 1; // 10
                int capital = 'Z' - 'A' + 1; //26
                int alpha = 'z' - 'a' + 1; // 26
                int max = number + capital + alpha;
                while (length-- > 0)
                {
                    if (withSpecialCharacters)
                    {
                        sb.Append((char)GetRandomNumber(33, 126));
                    }
                    else
                    {
                        int temp = GetRandomNumber(max);
                        if (0 <= temp && temp < number) // 0 - 9
                        {
                            sb.Append((char)(temp + '0'));
                        }
                        else if (temp < (capital + number)) // 10 - 35
                        {
                            sb.Append((char)(temp - number + 'A'));
                        }
                        else// (temp < max) // 36 - 61
                        {
                            sb.Append((char)(temp - number - capital + 'a'));
                        }
                    }
                }
                result = sb.ToString();
            }
            return result;
        }

        public static string Repeat(this string str, int count)
        {
            StringBuilder result = new StringBuilder();
            while (count-- > 0)
            {
                result.Append(str);
            }
            return result.ToString();
        }
        #endregion

        #region Reflection

        /// <summary>
        /// Gets a method within a class
        /// </summary>
        /// <param name="classFullName">Class full Name (including namespace)</param>
        /// <param name="methodName">Method's name</param>
        /// <param name="bindingFlags">Flags used for filtering methods</param>
        /// <returns>MethodInfo object of the searched method</returns>
        public static MethodInfo GetMethod(string classFullName, string methodName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            Type methodType = GetType(classFullName);
            return GetMethod(methodType, methodName, bindingFlags);
        }

        /// <summary>
        /// Gets a method within a class
        /// </summary>
        /// <param name="classType">Class type that contains the generic method</param>
        /// <param name="methodName">Method's name</param>
        /// <param name="bindingFlags">Flags used for filtering methods</param>
        /// <param name="generic">Looks for a generic method</param>
        /// <param name="parameters">Method Parameters' types</param>
        /// <returns>MethodInfo object of the searched method</returns>
        public static MethodInfo GetMethod(Type classType, string methodName, BindingFlags bindingFlags = BindingFlags.Default, 
            bool generic = false, Type[] parametersType = null)
        {
            MethodInfo searchedMethod = null;
            MethodInfo[] methodArray;

            // When default BindingFlags are set, we use parameterless method to obtain all methods 
            // other wise some methods are omitted (Reflection's Bug ?)
            if (bindingFlags == BindingFlags.Default)
            {
                methodArray = classType.GetTypeInfo().GetMethods();
            }
            else 
            {
                methodArray = classType.GetTypeInfo().GetMethods(bindingFlags);
            }

            foreach (MethodInfo method in methodArray)
            {
                if (method.Name == methodName && (generic == method.IsGenericMethod))
                {
                    bool found = true;

                    // Additional verirication when Method's parameters are specified
                    if (null != parametersType)
                    {
                        ParameterInfo[] methodParams = method.GetParameters();

                        // #1 Verify Total expected parameters match
                        if (methodParams.Length != parametersType.Length)
                        {
                            continue;
                        }

                        // #2 Verify Parameters' type match
                        for (int i = 0; i < methodParams.Length; i++)
                        {
                            if (methodParams[i].ParameterType != parametersType[i])
                            {
                                found = false;
                                break;
                            }
                        }

                        // Verify method was found
                        if (!found)
                        {
                            continue;
                        }
                        searchedMethod = method;
                        break;
                    }
                    else
                    {
                        searchedMethod = method;
                        break;
                    }
                }
            }
            return searchedMethod;
        }

        /// <summary>
        /// Gets a generic method inside a class
        /// </summary>
        /// <param name="classFullName">Class full Name (including namespace)</param>
        /// <param name="methodName">Method's name</param>
        /// <param name="genericTypes">The types that generic method expects</param>
        /// <param name="bindingFlags">Flags used for filtering methods</param>
        /// <returns>MethodInfo object of the searched method</returns>
        public static MethodInfo GetMethodGeneric(string classFullName, string methodName, Type[] genericTypes, BindingFlags bindingFlags = BindingFlags.Default)
        {
            Type classType = GetType(classFullName);
            return GetMethodGeneric(classType, methodName, genericTypes, bindingFlags);
        }

        /// <summary>
        /// Gets a generic method inside a class
        /// </summary>
        /// <param name="classType">Class type that contains the generic method</param>
        /// <param name="methodName">Method's name</param>
        /// <param name="genericTypes">The types that generic method expects</param>
        /// <param name="bindingFlags">Flags used for filtering methods</param>
        /// <returns>MethodInfo object of the searched method</returns>
        public static MethodInfo GetMethodGeneric(Type classType, string methodName, Type[] genericTypes, BindingFlags bindingFlags = BindingFlags.Default) 
        {
            MethodInfo genericMethod = GetMethod(classType, methodName, bindingFlags, true);

            if (null != genericMethod)
            {
                genericMethod = genericMethod.MakeGenericMethod(genericTypes);
            }
            return genericMethod;
        }

        /// <summary>
        /// Gets a generic method inside a class
        /// </summary>
        /// <param name="classType">Class type that contains the generic method</param>
        /// <param name="methodName">Method's name</param>
        /// <param name="genericType">The type that generic method expects</param>
        /// <param name="bindingFlags">Flags used for filtering methods</param>
        /// <returns>MethodInfo object of the searched method</returns>
        public static MethodInfo GetMethodGeneric(Type classType, string methodName, Type genericType, BindingFlags bindingFlags = BindingFlags.Default)
        {
            MethodInfo genericMethod = GetMethod(classType, methodName, bindingFlags);

            if (null != genericMethod)
            {
                genericMethod = genericMethod.MakeGenericMethod(genericType);
            }
            return genericMethod;
        }

        /// <summary>
        /// Gets a generic method inside a class
        /// </summary>
        /// <param name="classFullName">Class full Name (including namespace)</param>
        /// <param name="methodName">Method's name</param>
        /// <param name="genericType">The type that generic method expects</param>
        /// <param name="bindingFlags">Flags used for filtering methods</param>
        /// <returns>MethodInfo object of the searched method</returns>
        public static MethodInfo GetMethodGeneric(string classFullName, string methodName, Type genericType, BindingFlags bindingFlags = BindingFlags.Default)
        {
            Type classType = GetType(classFullName);
            return GetMethodGeneric(classType, methodName, genericType, bindingFlags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="methodName"></param>
        /// <param name="methodParameters"></param>
        /// <param name="genericType"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static dynamic ExecuteStaticMethod(string nameSpace, string methodName, object[] methodParameters,
            Type genericType = null, BindingFlags bindingFlags = BindingFlags.Default)
        {
            MethodInfo method;

            if (null != genericType)
            {
                method = GetMethodGeneric(nameSpace, methodName, genericType, bindingFlags);
            }
            else
            {
                method = GetMethod(nameSpace, methodName, bindingFlags);
            }

            // Parameters less methods
            if (null == methodParameters || methodParameters.Length == 0)
            {
                return method.Invoke(null, null);
            }
            return method.Invoke(null, methodParameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="methodName"></param>
        /// <param name="methodParameters"></param>
        /// <param name="genericType"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static dynamic ExecuteMethod(string nameSpace, string methodName, object[] methodParameters,
            Type genericType = null, BindingFlags bindingFlags = BindingFlags.Default, object[] instanceParameters = null)
        {
            MethodInfo method;

            if (null != genericType)
            {
                method = GetMethodGeneric(nameSpace, methodName, genericType, bindingFlags);
            }
            else
            {
                method = GetMethod(nameSpace, methodName, bindingFlags);
            }
            Type instanceType = GetType(nameSpace);

            if (null == instanceParameters)
            {
                // Parameterless Constructor
                var instance = Activator.CreateInstance(instanceType);
                return ExecuteMethod(instance, method, methodParameters);
            }
            else
            {
                var instance = Activator.CreateInstance(instanceType, instanceParameters);
                return ExecuteMethod(instance, method, methodParameters);
            }
        }

        public static dynamic ExecuteMethod(object instance, MethodInfo method, object[] parameters = null)
        {
            // Parameters less methods
            if (null == parameters || parameters.Length == 0)
            {
                return method.Invoke(instance, null);
            }
            return method.Invoke(instance, parameters);
        }

        #endregion

        #region Images (bmp, jpeg, jpg, png, etc.)

        ///// <summary>
        ///// Creates an Image using an specific color as the background
        ///// </summary>
        ///// <param name="width">Image width</param>
        ///// <param name="height">Image height</param>
        ///// <param name="color">Image background color</param>
        ///// <returns>Image object</returns>
        //public static Image CreateImage(int width = 10, int height = 10, Color? color = null)
        //{
        //    Image image = new Bitmap(width, height);
        //    Graphics imgGraphic = Graphics.FromImage(image);
        //    Color background = color ?? Color.Navy;

        //    // Set Background Color
        //    imgGraphic.Clear(background);
        //    imgGraphic.Save();

        //    return image;
        //}

        #endregion

        #region 64Based

        /// <summary>
        /// Convert string into Based64 String (without care about Encoding)
        /// </summary>
        /// <param name="input">string value to convert into 64 Based</param>
        /// <returns></returns>
        public static string StringToBase64String(string input)
        {
            byte[] bytes = new byte[input.Length * sizeof(char)];
            System.Buffer.BlockCopy(input.ToCharArray(), 0, bytes, 0, bytes.Length);
            return BytesToBase64String(bytes);
        }

        /// <summary>
        /// Convert UTF8 Encoded string into Based64 String
        /// </summary>
        /// <param name="input">string value to convert into 64 Based</param>
        /// <returns></returns>
        public static string UTF8StringToBase64String(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input.ToCharArray());
            return BytesToBase64String(bytes);
        }

        /// <summary>
        /// Convert ASCII Encoded string into Based64 String
        /// </summary>
        /// <param name="input">string value to convert into 64 Based</param>
        /// <returns></returns>
        public static string ASCIIStringToBase64String(string input)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input.ToCharArray());
            return BytesToBase64String(bytes);
        }

        /// <summary>
        /// Convert bytes into Based64 String
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        ///// <summary>
        ///// Converts Image into Based64 String (jpeg format used by default)
        ///// </summary>
        ///// <param name="image">Image object</param>
        ///// <returns></returns>
        //public static string ImageToBase64String(Image image) 
        //{
        //    return ImageToBase64String(image, ImageFormat.Jpeg);
        //}

        ///// <summary>
        ///// Converts Image into Based64 String
        ///// </summary>
        ///// <param name="image">Image object</param>
        ///// <param name="format">Supported Imageformat (gif, jpeg, png, etc.)</param>
        ///// <returns></returns>
        //public static string ImageToBase64String(Image image, ImageFormat format)
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    image.Save(memoryStream, format);

        //    byte[] imageBytes = memoryStream.ToArray();
        //    return BytesToBase64String(imageBytes);
        //}

        #endregion

        #region UUID

        /// <summary>
        /// Convert Partition Id into Hex with the correct format for UUID
        /// </summary>
        /// <param name="partitionId">Partition Id (When not specified it will be retrieved from configuration file)</param>
        /// <returns></returns>
        public static string PartitionIntoUUIDHex(string partitionId = null) 
        {
            // Get partition from configuration file when it is not specified
            if(partitionId == null)
            {
                partitionId = TestLibrary.TestConfig.GetValueFromConfig("Partition");
            }
            // Get the Hexadecimal representation of the Partition Id
            string bynary = Convert.ToString(Int32.Parse(partitionId), HEXADECIMAL);
            int missingChars = UUID_BITS_PARTITION - bynary.Length;

            // Add missing elements for hex string to match the total number of UUID_BITS_PARTITION bits
            return TestLibrary.Util.FillWithCharactersLeft(bynary, missingChars, '0').ToUpper();
        }

        #endregion

        #region Enums

        /// <summary>
        /// Returns the value associated to the attribute "Description" of any Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>
        /// Enum's Description attribute
        /// string.Empty if no description has been set
        /// </returns>
        public static string GetEnumDescription<T>(T obj)
        {
            Type type = obj.GetType();

            if(!type.GetTypeInfo().IsEnum)
            {
                Log.Info("Parameter <T> is not a enum, only enums are acepted");
                return string.Empty;
            }
            MemberInfo[] info = type.GetTypeInfo().GetMember(obj.ToString());

            if(null != info && info.Length > 0)
            {
                DescriptionAttribute attributes = info[0].GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;

                if(null != attributes)
                {
                    return attributes.Description;
                }
            }
            return string.Empty;
        }

        #endregion

        /// <summary>
        /// Get Type based-on class name
        /// </summary>
        /// <param name="typeName">a Class name or full class name with namespace.</param>
        /// <returns>Type</returns>
        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;

            var deps = DependencyContext.Default;
            foreach (var compilationLibrary in deps.CompileLibraries)
            {
                if (compilationLibrary.Name.Contains("APITestFramework"))
                {
                    var assembly = Assembly.Load(new AssemblyName(compilationLibrary.Name));
                    type = assembly.GetType(typeName);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }

            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    type = a.GetType(typeName);

            //    if (type != null)
            //    {
            //        return type;
            //    }
            //}
            //Assembly assem = Assembly.GetExecutingAssembly();

            //type = assem.GetType(typeName);

            //if (type != null) 
            //{
            //    return type;
            //}
            return null;
        }

        /// <summary>
        /// Parse a string of datetime (yyyy/MM/dd HH:mm:ss) to a DateTime. 
        /// </summary>
        /// <param name="dateTimeString">Input string</param>
        /// <returns>DateTime</returns>
        public static DateTime ParseDateTime(string dateTimeString)
        {
            Log.Info("String:{0}", dateTimeString);
            return DateTime.ParseExact(dateTimeString.Trim(), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse a string of date (yyyy/MM/dd) to a DateTime. 
        /// </summary>
        /// <param name="dateString">Input string.</param>
        /// <returns>DateTime</returns>
        public static DateTime ParseDate(string dateString)
        {
            return DateTime.ParseExact(dateString.Trim(), "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Generate string value that represend the value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isDate">
        /// true: generate date time (yyyy/MM/dd HH:mm:ss)
        /// <para/> false: generate date (yyyy/MM/dd)
        /// </param>
        /// <returns>String</returns>
        public static string ToString(DateTime value, bool isDate = false)
        {
            if (isDate)
                return value.ToString("yyyy/MM/dd");
            return value.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        /// Convert a Datetime to Unix time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>A number represent Unit time</returns>
        public static long ToUnixTime(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((dateTime - epoch).TotalSeconds);
        }

        /// <summary>
        /// Convert Unix time to DateTime
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns>DateTime</returns>
        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
        private static KeyValuePair<int, string>? lastestResponseData;

        public static TimeSpan Benchmark(string description, Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// Read text content from a http response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string ReadTextFromHttpResponse(HttpWebResponse response)
        {
            string result = string.Empty;
            if (lastestResponseData != null && lastestResponseData.Value.Key == response.GetHashCode())
            {
                return lastestResponseData.Value.Value;
            }
            else
            {
                Stream stream = response.GetResponseStream();
                if (stream.CanRead)
                {
                    using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        StringBuilder sb = new StringBuilder();
                        if (!string.IsNullOrEmpty(response.Headers["Transfer-Encoding"]) && response.Headers["Transfer-Encoding"].ToLower().Equals("chunked"))
                        {
                            try
                            {
                                while (!sr.EndOfStream)
                                {
                                    sb.Append((char)sr.Read());
                                }
                            }
                            catch (System.IO.IOException exception)
                            {
                                Console.Write("Exception:{0}", exception);
                            }
                            result = sb.ToString();
                        }
                        else
                        {
                            try
                            {
                                result = sr.ReadToEnd();
                            }
                            catch (System.IO.IOException exception)
                            {
                                Console.Write("Exception:{0}", exception);
                            }
                        }
                    }
                }
                lastestResponseData = new KeyValuePair<int, string>(response.GetHashCode(), result);
            }

            return result;
        }

        /// <summary>
        /// Read text content from a http response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<string> ReadTextFromHttpResponse(HttpResponseMessage response)
        {
            string result = string.Empty;
            if (lastestResponseData != null && lastestResponseData.Value.Key == response.GetHashCode())
            {
                return lastestResponseData.Value.Value;
            }
            else
            {
                HttpContent content = response.Content;
                result = await content.ReadAsStringAsync();
                
                lastestResponseData = new KeyValuePair<int, string>(response.GetHashCode(), result);
            }

            return result;
        }

        /// <summary>
        /// Read a text file
        /// </summary>
        /// <param name="filePath">The path of the input file.</param>
        /// <returns>Text is read from the file</returns>
        public static string ReadTextFile(string filePath)
        {
            string result = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(System.IO.File.Open(filePath, FileMode.Open)))
                {
                    result = sr.ReadToEnd();
                    //sr.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Info("Cannot open file {0}. \nError:{1}", filePath, ex.Message);
            }
            Log.Info("File content:\n{0}", result);
            return result;
        }

        /// <summary>
        /// Read a binary file
        /// </summary>
        /// <param name="filePath">The path of the input file.</param>
        /// <returns>Binary is read from the file</returns>
        public static byte[] ReadBinaryFile(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// Verify type
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static bool VerifyType(string type, string value)
        {
            bool result = false;
            switch (type)
            {
                case "string":
                    result = true;
                    break;

                case "numeric":
                    int temp;
                    result = Int32.TryParse(value, out temp);
                    break;

                case "double":
                    double tempDouble;
                    result = double.TryParse(value, out tempDouble);
                    break;

                case "date":
                case "datetime":
                    if (!string.IsNullOrEmpty(value))
                    {
                        DateTime dateTime;
                        string format = "yyyy/MM/dd HH:mm:ss";
                        if (type == "date") format = "yyyy/MM/dd";
                        result = DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
                    }
                    else result = true;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Remove elements from a Dictionary
        /// </summary>
        /// <param name="listToRemove">list to remove</param>
        /// <param name="dictionary">dictionary</param>
        /// <returns>Elements is not removed</returns>
        public static Dictionary<string, Dictionary<string, object>> RemoveElements(List<string> listToRemove, Dictionary<string, Dictionary<string, object>> dictionary)
        {
            Dictionary<string, Dictionary<string, object>> tempDic = new Dictionary<string, Dictionary<string, object>>(dictionary);
            listToRemove.ForEach(x => { tempDic.Remove(x); });
            return tempDic;
        }

        /// <summary>
        /// Get elements from a Dictionary
        /// </summary>
        /// <param name="listToGet">list to get</param>
        /// <param name="dictionary">dictionary</param>
        /// <returns>Elements is from list</returns>
        public static Dictionary<string, Dictionary<string, object>> GetElements(List<string> listToGet, Dictionary<string, Dictionary<string, object>> dictionary)
        {
            Dictionary<string, Dictionary<string, object>> tempDic = new Dictionary<string, Dictionary<string, object>>();
            foreach (string key in listToGet)
            {
                tempDic[key] = dictionary[key];
            }
            return tempDic;
        }

        /// <summary>
        /// Convert list to string
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>string</returns>
        public static string ListToString(List<string> list)
        {
            string listToString = string.Empty;
            foreach (string key in list)
                listToString += String.Join(",", key);
            return listToString;
        }

        /// <summary>
        /// Convert string to list
        /// </summary>
        /// <param name="string">string</param>
        /// <returns>list</returns>
        public static List<string> StringToList(string str)
        {
            string[] key = str.Split(',');
            List<string> list = new List<string>() { };
            foreach (string k in key)
            {
                list.Add(k);
            }
            return list;
        }

        /// <summary>
        /// Added util function for getting the Current Test Directory
        /// since it's not provided by NUnit as of the moment
        /// Code is base on the Git of NUnit 3.4.1
        /// https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/TestContext.cs
        /// </summary>
        /// <param name="string">string</param>
        /// <returns>list</returns>
        public static string GetTestDirectory()
        {
            Test test = TestExecutionContext.CurrentContext.CurrentTest;
            if (test != null)
            {
                if(test is TestAssembly)
                {
                    return GetDirectoryName(((TestAssembly)test).Assembly);
                }
                else
                {
                    return GetDirectoryName(test.TypeInfo.Assembly);
                }
            }
                

            // Test is null, we may be loading tests rather than executing.
            // Assume that calling assembly is the test assembly.
            return GetDirectoryName(Assembly.GetEntryAssembly());
        }
        
        /// <summary>
        /// Gets the path to the directory from which an assembly was loaded.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The path.</returns>
        private static string GetDirectoryName(Assembly assembly)
        {
            return Path.GetDirectoryName(AssemblyHelper.GetAssemblyPath(assembly));
        }
    }
}
