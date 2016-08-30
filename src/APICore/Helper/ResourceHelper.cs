using APICore.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestLibrary;
using System.Globalization;
using System.Collections;

namespace APICore.Helper
{
    /// <summary>
    /// This class contains all utitity methods related to Resource classes
    /// </summary>
    public class ResourceHelper
    {
        /// <summary>
        /// Compare two lists of resource by using a element such as P_Id.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compareList"></param>
        /// <param name="compareElement">Element name such as: Client.P_Id, Sales.P_Id...</param>
        /// <returns>
        /// true: equal.
        /// <para/> false: not equal.
        /// </returns>
        public static bool CompareList<TResource, TElement>(List<TResource> source, List<TResource> compareList, string compareElement) 
            where TResource : ResourceBase
        {
            if (source.Count != compareList.Count)
            {
                Log.Info("Compare list: not equal! Count is not equal!");
                return false;
            }
            Type propType = typeof(TElement);
            for (int i = 0; i < source.Count; i++)
            {
                switch (propType.Name.ToLower())
                {
                    case "boolean":
                        bool? boolElement1 = GetBooleanTypeValue(source[i], compareElement);
                        bool? boolElement2 = GetBooleanTypeValue(compareList[i], compareElement);
                        if (boolElement1 != boolElement2)
                        {
                            Log.Info("Compare list: Item {0} with {1} is not equal to {2}", i, boolElement1, boolElement2);
                            return false;
                        }
                        break;
                    case "int32":
                        int? intElement1 = GetIntTypeValue(source[i], compareElement);
                        int? intElement2 = GetIntTypeValue(compareList[i], compareElement);
                        if (intElement1 != intElement2)
                        {
                            Log.Info("Compare list: Item {0} with {1} is not equal to {2}", i, intElement1, intElement2);
                            return false;
                        }
                        break;
                    case "string":
                        string string1 = GetStringTypeValue(source[i], compareElement);
                        string string2 = GetStringTypeValue(compareList[i], compareElement);
                        if (string.Compare(string1, string2) != 0)
                        {
                            Log.Info("Compare list: Item {0} with {1} is not equal to {2}", i, string1, string2);
                            return false;
                        }
                        break;
                    case "double":
                        double? double1 = GetDoubleTypeValue(source[i], compareElement);
                        double? double2 = GetDoubleTypeValue(compareList[i], compareElement);
                        if (double1 != double2)
                        {
                            Log.Info("Compare list: Item {0} with {1} is not equal to {2}", i, double1, double2);
                            return false;
                        }

                        break;
                    case "datetime":
                        DateTime? date1 = GetDateTimeTypeValue(source[i], compareElement);
                        DateTime? date2 = GetDateTimeTypeValue(compareList[i], compareElement);
                        if (date1 != date2)
                        {
                            Log.Info("Compare list: Item {0} with {1} is not equal to {2}", i, date1, date2);
                            return false;
                        }
                        break;
                    case "int64":
                        long? long1 = GetLongTypeValue(source[i], compareElement);
                        long? long2 = GetLongTypeValue(compareList[i], compareElement);
                        if (long1 != long2)
                        {
                            Log.Info("Compare list: Item {0} with {1} is not equal to {2}", i, long1, long2);
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Verify whether a list of resources contains some element
        /// </summary>
        /// <param name="resourceList">List of resources.</param>
        /// <param name="elementList">List of elements.</param>
        /// <returns>true: All items contain all element.</returns>
        public static bool VerifyListContainElement<T>(List<T> resourceList, List<string> elementList) where T : ResourceBase
        {
            for (int i = 0; i < resourceList.Count; i++)
            {
                for (int j = 0; j < elementList.Count; j++)
                {
                    if (resourceList[i].DictionaryValues.ContainsKey(elementList[j]))
                    {
                        //Log.Info("Item {0} doesn't contain element '{1}'", i, elementList[j]);
                        Assert.IsTrue(resourceList[i].DictionaryValues.ContainsKey(elementList[j]),
                        string.Format("Item {0} contains {1}", i, elementList[j]));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Verify whether a list of resources not contains some element
        /// </summary>
        /// <param name="resourceList">List of resources.</param>
        /// <param name="elementList">List of elements.</param>
        public static void VerifyListNotContainElement<T>(List<T> resourceList, List<string> elementList) where T : ResourceBase
        {
            for (int i = 0; i < resourceList.Count; i++)
            {
                for (int j = 0; j < elementList.Count; j++)
                {
                    Assert.IsTrue(!resourceList[i].DictionaryValues.ContainsKey(elementList[j]),
                        string.Format("Test case fail: Item {0} does contain {1}", i, elementList[j]));
                }
            }
        }

        /// <summary>
        /// Verify suffix number type
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="expected">expected</param>
        /// <param name="actual">actual</param>
        public static void VerifySuffixNumberType(string type, IComparable expected, IComparable actual)
        {
            string errorMessage = string.Format("Unable to verify that actual value {0} matches the expected value {1} using {2} operator", actual, expected, type);
            switch (type)
            {
                case "gt":
                    Assert.Greater(expected, actual, errorMessage);
                    break;

                case "ge":
                    Assert.GreaterOrEqual(expected, actual, errorMessage);
                    break;

                case "lt":
                    Assert.Less(expected, actual, errorMessage);
                    break;

                case "le":
                    Assert.LessOrEqual(expected, actual, errorMessage);
                    break;

                case "=":
                case "eq":
                    Assert.AreEqual(expected, actual, errorMessage);
                    break;
            }
        }

        /// <summary>
        /// Verify suffix text type
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="expected">expected</param>
        /// <param name="actual">actual</param>
        public static void VerifySuffixTextType(string type, string expected, string actual)
        {
            string errorMessage = string.Format("Unable to verify that actual value {0} matches the expected value {1} using {2} operator", actual, expected, type);
            switch (type)
            {
                case "part":
                    StringAssert.Contains(expected, actual, errorMessage);
                    break;

                case "full":
                    Assert.AreEqual(expected, actual, errorMessage);
                    break;

                case "startswith":
                    StringAssert.StartsWith(expected, actual, errorMessage);
                    break;
            }
        }

        /// <summary>
        /// Sort a list of resources
        /// </summary>
        /// <typeparam name="T">Resource Type</typeparam>
        /// <param name="originalList">Original list</param>
        /// <param name="sortOrders">
        /// <para/> Key: string with format ElementName:[asc/desc], example: "Client.P_Id:asc"
        /// <para/> Value: Type, Just support: int, long, double, string, DateTime
        /// </param>
        /// <exception cref="FormatException">In case any value of sortOrders has wrong format.</exception>
        /// <returns>Sorted list</returns>
        public static List<T> SortList<T>(List<T> originalList, Dictionary<string, Type> sortOrders) where T : ResourceBase
        {
            IQueryable<T> result = originalList.AsQueryable();

            bool isFirst = true;
            //IOrderedEnumerable<T> temp = null;
            //Expression lamda = null;
            Expression resultEx = null;
            //var resourceType = typeof(T);
            //ParameterExpression xParameter = Expression.Parameter(typeof(T), "x");
            foreach (var item in sortOrders)
            {
                string[] splitedKey = item.Key.Split(':');
                if (splitedKey.Length < 2)
                {
                    throw new FormatException("Wrong sortOrders key format! It should be formatted 'ElementName:[asc/desc]'.");
                }
                bool isAsc = splitedKey[1].ToLower() == "asc";
                Type propType = item.Value;
                string propName = splitedKey[0];

                switch (propType.Name.ToLower())
                {
                    case "boolean":
                        resultEx = MakeExpression(propType, propName, "GetBooleanTypeValue", isAsc, isFirst, result);
                        break;
                    case "int32":
                        resultEx = MakeExpression(propType, propName, "GetIntTypeValue", isAsc, isFirst, result);
                        break;
                    case "string":
                        resultEx = MakeExpression(propType, propName, "GetStringTypeValue", isAsc, isFirst, result);
                        break;
                    case "double":
                        resultEx = MakeExpression(propType, propName, "GetDoubleTypeValue", isAsc, isFirst, result);
                        break;
                    case "datetime":
                        resultEx = MakeExpression(propType, propName, "GetDateTimeTypeValue", isAsc, isFirst, result);
                        break;
                    case "int64":
                        resultEx = MakeExpression(propType, propName, "GetLongTypeValue", isAsc, isFirst, result);
                        break;
                }
                if (isFirst)
                    isFirst = false;
                result = result.Provider.CreateQuery<T>(resultEx);
            }
            return result.ToList<T>();
        }

        private static Expression MakeExpression<T>(Type propType, string propName, string getPropertyValueMethod,
            bool isAsc, bool isFirst, IQueryable<T> resultQuery) where T : ResourceBase
        {
            Expression resultEx = null;
            ParameterExpression xParameter = Expression.Parameter(typeof(T), "x");

            //Get property value: x => GetIntTypeValue(x,propName)
            ConstantExpression propNameEx = Expression.Constant(propName, typeof(string));
            //ParameterExpression typeEx = Expression.Parameter(typeof(Type), "propType");
            MethodInfo method = typeof(ResourceHelper).GetTypeInfo().GetMethod(getPropertyValueMethod);
            MethodCallExpression call = Expression.Call(method, xParameter, propNameEx);
            Expression lamda = Expression.Lambda(call, xParameter);
            //Call orderBy
            var methodName = isFirst ? (isAsc ? "OrderBy" : "OrderByDescending") : (isAsc ? "ThenBy" : "ThenByDescending");
            //var methodName = isAsc ? "OrderBy" : "OrderByDescending";
            Type[] typeArguments = lamda.Type.GetTypeInfo().GetGenericArguments();

            if (isFirst)
            {
                resultEx = Expression.Call(typeof(Queryable), methodName, typeArguments, resultQuery.Expression, Expression.Quote(lamda));
            }
            else
            {
                IOrderedQueryable<T> newQuery = resultQuery as IOrderedQueryable<T>;
                //typeArguments = lamda.Type.GetGenericArguments();
                resultEx = Expression.Call(typeof(Queryable), methodName, typeArguments, newQuery.Expression, Expression.Quote(lamda));
            }
            //resultQuery = resultQuery.Provider.CreateQuery<T>(resultEx);

            return resultEx;
        }

        /// <summary>
        /// Get bool value of an property of a resource by using an element name.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static bool? GetBooleanTypeValue(ResourceBase resource, string elementName)
        {
            if (!resource.DictionaryValues.ContainsKey(elementName))
                return null;
            if (resource is XmlResource)
            {
                string valueTemp = resource.DictionaryValues[elementName] as string;
                if (string.IsNullOrEmpty(valueTemp))
                    return null;
                return bool.Parse(valueTemp);
            }
            return resource.DictionaryValues[elementName] as bool?;
        }

        /// <summary>
        /// Get int value of an property of a resource by using an element name.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static int? GetIntTypeValue(ResourceBase resource, string elementName)
        {
            if (!resource.DictionaryValues.ContainsKey(elementName))
                return null;
            if (resource is XmlResource)
            {
                string valueTemp = resource.DictionaryValues[elementName] as string;
                if (string.IsNullOrEmpty(valueTemp))
                    return null;
                return int.Parse(valueTemp);
            }
            return resource.DictionaryValues[elementName] as int?;
        }

        /// <summary>
        /// Get long value of an property of a resource by using an element name.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static long? GetLongTypeValue(ResourceBase resource, string elementName)
        {
            if (!resource.DictionaryValues.ContainsKey(elementName))
                return null;
            if (resource is XmlResource)
            {
                string valueTemp = resource.DictionaryValues[elementName] as string;
                if (string.IsNullOrEmpty(valueTemp))
                    return null;
                return long.Parse(valueTemp);
            }
            return resource.DictionaryValues[elementName] as long?;
        }

        /// <summary>
        /// Get double value of an property of a resource by using an element name.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static double? GetDoubleTypeValue(ResourceBase resource, string elementName)
        {
            if (!resource.DictionaryValues.ContainsKey(elementName))
                return null;
            if (resource is XmlResource)
            {
                string valueTemp = resource.DictionaryValues[elementName] as string;
                if (string.IsNullOrEmpty(valueTemp))
                    return null;
                return double.Parse(valueTemp);
            }
            return resource.DictionaryValues[elementName] as double?;
        }

        /// <summary>
        /// Get string value of an property of a resource by using an element name.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string GetStringTypeValue(ResourceBase resource, string elementName)
        {
            if (!resource.DictionaryValues.ContainsKey(elementName))
                return null;
            return resource.DictionaryValues[elementName] as string;
        }

        /// <summary>
        /// Get DateTime value of an property of a resource by using an element name.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeTypeValue(ResourceBase resource, string elementName)
        {
            if (!resource.DictionaryValues.ContainsKey(elementName))
                return null;
            string valueTemp = resource.DictionaryValues[elementName] as string;
            if (string.IsNullOrEmpty(valueTemp))
                return null;
            try
            {
                return TestLibrary.Util.ParseDateTime(valueTemp);
            }
            catch (FormatException exception)
            {
                Log.Info("Exception:{0}", exception.Message);
                return TestLibrary.Util.ParseDate(valueTemp);
            }
        }
    }
}
