using System;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

using System.Collections;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APICore.Helper
{
    /// <summary>
    /// This class helps on Parsing/Generating a Json string.
    /// To-do: Use Callback methods to modify the result.
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// Parse Json Stream to objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T PasreJsonStream<T>(Stream stream, Encoding encoding)
        {
            Type type = typeof(T);
            //var serializer = new DataContractJsonSerializer(type);
            StreamReader reader = new StreamReader(stream, encoding);
            string input = reader.ReadToEnd();
            T result = ParseJSONString<T>(input);
            return result;
        }


        /// <summary>
        /// Parse Json string to objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T ParseJSONString<T>(string input)
        {
            Type type = typeof(T);
            T result = JsonConvert.DeserializeObject<T>(input);
            #region TO-DO: Port to .Net Core code
            if (typeof(Model.Model).IsAssignableFrom(type))
            {
                Dictionary<string, object> temp = JsonConvert.DeserializeObject<Dictionary<string, object>>(input);
                ModifyDictionary(result as Model.Model, temp);
            }
            else if (type.IsGenericParameter && type.FullName.Contains("List"))
            {
                Type[] elementType = type.GetGenericArguments();
                if (typeof(Model.Model).IsAssignableFrom(elementType[0]))
                {
                    List<Dictionary<string, object>> temp = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(input);

                    for (int i = 0; i < temp.Count; i++)
                    {
                        Model.Model element = GetListItemNew(elementType[0], i, result);
                        ModifyDictionary(element, temp[i]);
                    }
                }
            }
            else if (type.IsGenericParameter && type.FullName.Contains("Dictionary"))
            {
                Type[] elementType = type.GetGenericArguments();
                if (typeof(Model.Model).IsAssignableFrom(elementType[1]))
                {
                    Dictionary<string, Dictionary<string, object>> temp =
                        JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(input);
                    foreach (var item in temp)
                    {
                        Model.Model element = GetDictionaryItemNew(elementType[1], item.Key, result);
                        ModifyDictionary(element, temp[item.Key]);
                    }
                }
                else if (elementType[1].IsGenericParameter && elementType[1].FullName.Contains("List"))
                {
                    Type[] subElementType = elementType[1].GetGenericArguments();
                    if (typeof(Model.Model).IsAssignableFrom(subElementType[0]))
                    {
                        Dictionary<string, List<Dictionary<string, object>>> temp =
                            JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(input);
                        foreach (var item in temp)
                        {
                            for (int i = 0; i < item.Value.Count; i++)
                            {
                                Model.Model subElement = GetDictionaryListItemNew(subElementType[0], item.Key, i, result);
                                ModifyDictionary(subElement, temp[item.Key][i]);
                            }
                        }
                    }
                }
            }
            #endregion
            return result;
            
        }

        /// <summary>
        /// Se
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="stream"></param>
        public static void GenerateJSONStream<T>(T resource, Stream stream)
        {
            Type type = typeof(T);
                  
            object data = ModifyValue(resource);
           
            string result = JsonConvert.SerializeObject(data);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(result);
        }

        /// <summary>
        /// Generate a JSON string from an object
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="resource"></param>
        /// <returns>A StringBuilder that contains the result</returns>
        public static string GenerateJSONString<T>(T resource)
        {
            object data = ModifyValue(resource);
            return JsonConvert.SerializeObject(data);
            
        }
        
        /// <summary>
        /// Modify an object and its property: all ModelBase objects will be replaced by their DictionaryValues.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        internal static object ModifyValue(object oldValue)
        {
            if (oldValue == null) { return oldValue; }

            TypeInfo type = oldValue.GetType().GetTypeInfo();

            if (oldValue is Model.Model)
            {
                Dictionary<string, object> subOldDic = ((Model.Model)oldValue).DictionaryValues;
                Dictionary<string, object> subNewDic = new Dictionary<string, object>(subOldDic);

                foreach (KeyValuePair<string, object> item in subOldDic)
                {
                    var newItem = ModifyValue(item.Value);

                    // Avoid including NULL elements for JSON STRING
                    if(null != newItem)
                    {
                        subNewDic[item.Key] = newItem;
                    }
                }
                return subNewDic;
                //return oldValue;
            }
            else if (type.IsGenericParameter && type.GetGenericTypeDefinition().Name.Contains("Dictionary"))
            {
                return ModifyDictionaryValueNew(oldValue);
            }
            else if (type.IsGenericParameter && type.GetGenericTypeDefinition().Name.Contains("List"))
            {
                return ModifyListValueNew(oldValue);
            }
            return oldValue;
        }
              

        /// <summary>
        /// Parses a dictionary by casting its elements based on the type specified in the parameter 'object'
        /// </summary>
        /// <param name="oldValue">Raw dictionary to parse items into ModelBase type</param>
        /// <returns>A parsed version of the original Dictionary</returns>
        internal static object ModifyDictionaryValueNew(object oldValue)
        {
            TypeInfo type = oldValue.GetType().GetTypeInfo();
            Type[] itemTypes = type.GetGenericArguments();

            MethodInfo genericMethod = GetMethodGeneric(typeof(JsonHelper), "ModifyDictionaryValue", itemTypes, 
                BindingFlags.NonPublic | BindingFlags.Static);

            if (genericMethod != null)
            {
                return genericMethod.Invoke(null, new object[] { oldValue });
            }
            return oldValue;
        }

        

        /// <summary>
        /// Parses a list by casting its elements based on the type specified in the parameter 'object'
        /// </summary>
        /// <param name="oldValue">Raw dictionary to parse items into ModelBase type</param>
        /// <returns>A parsed version of the original List</returns>
        internal static object ModifyListValueNew(object oldValue)
        {
            IEnumerable castingValue = oldValue as IEnumerable;
            if (castingValue != null)
            {
                List<object> newList = new List<object>();
                foreach (var item in castingValue)
                {
                    newList.Add(ModifyValue(item));
                }
                return newList;
            }
            return null;
        }

        
        internal static object ModifyListValue<T>(object oldValue)
        {
            List<object> newList = new List<object>();
            foreach (T oldItem in oldValue as List<T>)
            {
                newList.Add(ModifyValue(oldItem));
            }
            return newList;
        }

        private static void ModifyDictionary(Model.Model resource, Dictionary<string, object> originalDic)
        {
            TypeInfo resourceType = resource.GetType().GetTypeInfo();
            Dictionary<string, object> resultDic = new Dictionary<string, object>(resource.DictionaryValues);
            foreach (var item in originalDic)
            {
                if (item.Value != null)
                {
                    PropertyInfo prop = resourceType.GetProperty(item.Key);

                    // Try to get property's information using Item's Name
                    // Formatting Item's Name, First letter must be Capital Letter
                    if (prop == null)
                    {
                        string firstLetter = item.Key.Substring(0,1);
                        StringBuilder formattedName = new StringBuilder(firstLetter.ToUpper());

                        // Verify Property Name's length is not a Single Letter
                        if(item.Key.Length > 1)
                        {
                            formattedName.Append(item.Key.Substring(1));
                        }
                        prop = resourceType.GetProperty(formattedName.ToString());
                    }

                    if (prop == null)  ///// Try to find property in case of ResourceEntry such as: Client.P_Name
                    {
                        string[] split = item.Key.Split('.');
                        string propName = split[split.Length - 1];
                        if (propName.StartsWith("P_"))
                            propName = propName.Substring(2);
                        prop = resourceType.GetProperty(propName);
                    }
                    if (prop != null)
                    {
                        #region To-do: port to .Net Core
                        Type propType = prop.PropertyType;
                        if (propType.IsGenericParameter && propType.FullName.Contains("Dictionary"))
                        {
                            Type[] subPropType = propType.GetGenericArguments();
                            if (typeof(Model.Model).IsAssignableFrom(subPropType[1]))
                            {
                                Dictionary<string, object> temp = originalDic[item.Key] as Dictionary<string, object>;
                                foreach (var subItem in temp)
                                {
                                    Dictionary<string, object> subTemp = CreateDictionary(subItem.Value as JObject);
                                    Model.Model subResource = GetDictionaryItemNew(subPropType[1], subItem.Key, resultDic[item.Key]);

                                    if (subResource != null)
                                    {
                                        ModifyDictionary(subResource, subTemp);
                                    }
                                }
                            }
                        }
                        else if (typeof(Model.Model).IsAssignableFrom(propType))
                        {
                            JObject temp = item.Value as JObject;
                            Dictionary<string, object> tempDic = CreateDictionary(temp);

                            Model.Model propValue = resultDic[item.Key] as Model.Model;
                            if (propValue != null)
                                ModifyDictionary(propValue, tempDic);
                            resultDic[item.Key] = propValue;
                        }
                        else if (propType.IsGenericParameter && propType.FullName.Contains("List"))
                        {
                            Type[] subPropType = propType.GetGenericArguments();
                            object propValue = resultDic[item.Key];

                            if (typeof(Model.Model).IsAssignableFrom(subPropType[0]))
                            {
                                //Old code:
                                //ArrayList arrayList = item.Value as ArrayList;
                                //New code: not sure It can work.
                                List<object> arrayList = item.Value as List<object>;
                                for (int i = 0; i < arrayList.Count; i++)
                                {
                                    Dictionary<string, object> temp = CreateDictionary(arrayList[i] as JObject);
                                    Model.Model propItem = GetListItemNew(subPropType[0], i, propValue);

                                    if (propItem != null)
                                        ModifyDictionary(propItem, temp);
                                }
                            }
                        }
                        #endregion
                    }
                }
                if (!resultDic.ContainsKey(item.Key))
                    resultDic.Add(item.Key, item.Value);
            }
            resource.DictionaryValues = resultDic;
        }

        /// <summary>
        /// Retrieves an element from a list casting it into the specified type(T)
        /// </summary>
        /// <typeparam name="T">The type used to cast element in the list</typeparam>
        /// <param name="index">index of the element to retrieve from the list</param>
        /// <param name="resultList">the list with the elements</param>
        /// <returns>An object type(T)</returns>
        private static T GetListItemGeneric<T>(int index, object resultList) where T : Model.Model
        {
            List<T> castedList = resultList as List<T>;
            return castedList[index];
        }

        /// <summary>
        /// Gets an ModelBase object from a list
        /// </summary>
        /// <param name="resourceType">ModelBase type</param>
        /// <param name="index">index at the list</param>
        /// <param name="resultList">list with many resources</param>
        /// <returns>Object of the ModelBase based on the specified resource type</returns>
        private static Model.Model GetListItemNew(Type resourceType, int index, object resultList)
        {
            MethodInfo genericMethod = GetMethodGeneric(typeof(JsonHelper), "GetListItemGeneric", resourceType, 
                BindingFlags.NonPublic | BindingFlags.Static);

            Model.Model resource = null;

            if (genericMethod != null)
            {
                resource = genericMethod.Invoke(null, new object[] { index, resultList }) as Model.Model;
            }
            return resource;
        }

        /// <summary>
        /// Retrieves an element from a dictionary casting it into the specified type (T)
        /// </summary>
        /// <typeparam name="T">The type used to cast element in the list</typeparam>
        /// <param name="key">the key used to obtain element from dictionary</param>
        /// <param name="resultList">the dictionary that contains the inf</param>
        /// <returns>An object type(T)</returns>
        private static T GetDictionaryItemGeneric<T>(string key, object resultList) where T : Model.Model
        {
            Dictionary<string, T> dictionary = resultList as Dictionary<string, T>;

            if(dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            return null;
        }

        /// <summary>
        /// Retrieves an element from a dictionary casting it into ModelBase class
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="key">the key used to obtain element from dictionary</param>
        /// <param name="resultList">the dictionary that contains the inf</param>
        /// <returns>An object of the specified type 'resourceType' (derived from ModelBase)</returns>
        private static Model.Model GetDictionaryItemNew(Type resourceType, string key, object resultList) 
        {
            MethodInfo genericMethod = GetMethodGeneric(typeof(JsonHelper), "GetDictionaryItemGeneric", resourceType,
                BindingFlags.NonPublic | BindingFlags.Static);

            Model.Model resource = null;

            if (genericMethod != null)
            {
                resource = genericMethod.Invoke(null, new object[] { key, resultList }) as Model.Model;
            }
            return resource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <param name="resultList"></param>
        /// <returns></returns>
        internal static T GetDictionaryListItemGeneric<T>(string key, int index, object resultList) where T : Model.Model
        {
            Dictionary<string, List<T>> dictionary = new Dictionary<string, List<T>>();

            if(dictionary.ContainsKey(key))
            {
                List<T> list = dictionary[key];

                if(list.Count > index)
                {
                    return list[index];
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <param name="resultList"></param>
        /// <returns></returns>
        internal static Model.Model GetDictionaryListItemNew(Type resourceType, string key, int index, object resultList) 
        {
            MethodInfo genericMethod = GetMethodGeneric(typeof(JsonHelper), "GetDictionaryListItemGeneric", resourceType,
                BindingFlags.NonPublic | BindingFlags.Static);

            Model.Model resource = null;

            if (genericMethod != null)
            {
                resource = genericMethod.Invoke(null, new object[] { key, index, resultList }) as Model.Model;
            }
            return resource;
        }

        private static Model.Model CreateInstance(Type resourceType, Dictionary<string, object> dictionaryValues)
        {
            Model.Model result = (Model.Model)Activator.CreateInstance(resourceType);
            Dictionary<string, object> resultDic = new Dictionary<string, object>(dictionaryValues);

            result.DictionaryValues = resultDic;
            return result;
        }

        private static Dictionary<string, object> CreateDictionary(JObject input)
        {
            
            IDictionary<string,JToken> temp = input as IDictionary<string, JToken>;
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (var item in temp)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }

        #region Reflection Utils - To-do: Move them to an Util class so that everyones can use them

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
                methodArray = classType.GetMethods();
            }
            else
            {
                methodArray = classType.GetMethods(bindingFlags);
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

        /// <summary>
        /// Get Type based-on class name
        /// </summary>
        /// <param name="typeName">a Class name or full class name with namespace.</param>
        /// <returns>Type</returns>
        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;

            #region To-do: port to .Net Core
            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    type = a.GetType(typeName);

            //    if (type != null)
            //    {
            //        return type;
            //    }
            //}
            #endregion  
            Assembly assem = Assembly.GetEntryAssembly();
            type = assem.GetType(typeName);
            return type;
        }
        #endregion
    }
}
