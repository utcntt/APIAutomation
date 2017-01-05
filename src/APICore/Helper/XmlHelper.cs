using System;
using System.Collections;
using System.Collections.Generic;
//using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using NUnit.Framework;
using TestLibrary;

using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Serialization;
using System.Globalization;
using APICore.Model;

namespace APICore.Helpers
{
    /// <summary>
    /// This is the most important class of PublicAPIs, use this class to send any request and verify any response of Public APIs
    /// </summary>
    public class XmlHelper
    {
        const string XML_ID = "Id";
        const string XML_CODE = "Code";
        const string XML_ITEM = "Item";
        const string XML_SYSTEM = "System";

        const string XML_ATTRIBUTE_COUNT = "Count";
        const string XML_ATTRIBUTE_TOTAL = "Total";
        const string XML_ATTRIBUTE_START = "Start";

        /// <summary>
        /// Generates the XML string from resource list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceList"></param>
        /// <returns></returns>
        public static string GenerateXMLString<T>(List<T> resourceList) where T : Model.Model
        {
            XDocument doc = new XDocument();
            XElement root = new XElement(typeof(T).Name);

            /* XML STRUCTURE
            <T>
                <item>
                    <T.xxx>...</T.xxx>
                    ...
                    <T.xxx>...</T.xxx>
                </item>
                <item>...</item>
            </T> */

            foreach (T item in resourceList)
            {
                // <item> should be added as many times as the number of elements inside the resourceList
                XElement itemElement = new XElement(XML_ITEM);

                AddResourceToXML(itemElement, item.DictionaryValues);
                root.Add(itemElement);
            }
            doc.Add(root);
            doc.Declaration = new XDeclaration("1.0", "UTF-8", "yes");

            //Use the following code to get xml string from doc in order to avoid XML Declaration missing
            return doc.Declaration.ToString() + Environment.NewLine + doc.ToString();
        }

        /// <summary>
        /// Parse an XML string to object
        /// </summary>
        /// <typeparam name="T">Support: ReadResponseData, List of WriteItem and ErrorCode</typeparam>
        /// <param name="xmlString">String for parsing</param>
        /// <param name="resourceElementName">Usually name of Resource: Client, Contract...</param>
        /// <exception cref="NotImplementedException">An exception will be thrown in case of unsupported type</exception>
        /// <returns></returns>
        public static T ParseXMLString<T>(string xmlString, string resourceElementName = null) where T : class, new()
        {
            T result = null;
            XDocument xmlDoc = XDocument.Parse(xmlString);
            Type type = typeof(T);
            string resourceName = type.Name;
            
            //if (resourceName.Contains("ReadResponseData"))
            //{
            //    Type[] elementType = type.GetTypeInfo().GetGenericArguments();
            //    MethodInfo method = typeof(XmlHelper).GetTypeInfo().GetMethod("GetResourcesFromXML", BindingFlags.NonPublic | BindingFlags.Static);
            //    MethodInfo generic = method.MakeGenericMethod(elementType[0]);
            //    object[] parameters = new object[] { xmlDoc };
            //    result = (T)generic.Invoke(null, parameters);
            //}
            //else if (type.GetTypeInfo().IsGenericType && type.FullName.Contains("List"))
            //{
            //    List<WriteResultItem> resultList = new List<WriteResultItem>();
            //    XElement root = xmlDoc.Root;
            //    foreach (XElement item in root.Elements(XML_ITEM))
            //    {
            //        WriteResultItem temp = new WriteResultItem();
            //        temp.Code = int.Parse(item.Element(XML_CODE).Value);
            //        temp.Id = item.Element(XML_ID).Value;
            //        resultList.Add(temp);
            //    }
            //    result = resultList as T;
            //}
            //else if (resourceName.Contains("ErrorCode"))
            //{
            //    IEnumerable<XElement> codeList = xmlDoc.Descendants(XML_CODE);
            //    if (codeList == null || codeList.Count() <= 0)
            //    {
            //        Log.Warn("Code element is not found!");
            //    }
            //    else
            //    {
            //        ErrorCode temp = new ErrorCode();
            //        XElement codeElement = codeList.First();
            //        temp.Code = int.Parse(codeElement.Value);
            //        temp.Type = codeElement.Parent.Name.ToString();
            //        result = temp as T;
            //    }
            //}
            //else
            //{
            //    throw new NotImplementedException("Parsing XML method for this type is not implemented!");
            //}

            return result;
        }

        /// <summary>
        /// Parses XML into a list of ModelBase
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlDom"></param>
        /// <returns></returns>
        //internal static ReadResponseData<T> GetResourcesFromXML<T>(XDocument xmlDom) where T : XmlModel, new()
        //{
        //    ReadResponseData<T> result = null;

        //    try
        //    {
        //        result = new ReadResponseData<T>();
        //        //XElement resourceElement = xmlDom.Element(typeof(T).Name);
        //        XElement resourceElement = xmlDom.Root;

        //        // COUNT
        //        if (resourceElement.Attribute(XML_ATTRIBUTE_COUNT) != null)
        //        {
        //            result.Count = int.Parse(resourceElement.Attribute(XML_ATTRIBUTE_COUNT).Value);
        //        }

        //        // TOTAL
        //        if (resourceElement.Attribute(XML_ATTRIBUTE_TOTAL) != null)
        //        {
        //            result.Total = int.Parse(resourceElement.Attribute(XML_ATTRIBUTE_TOTAL).Value);
        //        }

        //        // START
        //        if (resourceElement.Attribute(XML_ATTRIBUTE_START) != null)
        //        {
        //            result.Start = int.Parse(resourceElement.Attribute(XML_ATTRIBUTE_START).Value);
        //        }

        //        // CODE
        //        if (resourceElement.Element(XML_CODE) != null)
        //        {
        //            result.Code = int.Parse(resourceElement.Element(XML_CODE).Value);
        //        }
        //        List<T> list = new List<T>();

        //        if (resourceElement.Elements(XML_ITEM) != null)
        //        {
        //            foreach (XElement item in resourceElement.Elements(XML_ITEM))
        //            {
        //                T temp = new T();
        //                //temp.DictionaryValues = new Dictionary<string, object>();

        //                foreach (XNode prop in item.Nodes())
        //                {
        //                    if (prop.NodeType == XmlNodeType.Element)
        //                    {
        //                        ParseResource<T>((XElement)prop, temp.DictionaryValues);
        //                    }
        //                }
        //                list.Add(temp);
        //            }
        //        }
        //        result.Items = list;
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Info("Parsing XML exception:{0}", e.Message);
        //    }
        //    return result;
        //}

        /// <summary>
        /// Parses a XML into a Dictionary of string and object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        //internal static Dictionary<string, object> XMLtoDictionary<T>(string xml) where T : XmlModel
        //{
        //    Dictionary<string, object> xmlDictionary = null;

        //    try
        //    {
        //        XDocument xmlDom = XDocument.Parse(xml);
        //        XElement xmlRoot = xmlDom.Element(typeof(T).Name);

        //        ParseResource<T>(xmlRoot, xmlDictionary);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Info("Parsing XML exception:{0}", e.Message);
        //    }
        //    return xmlDictionary;
        //}


        /// Create an intance of WriteDataBase from  a XLM file
        /// </summary>
        /// <typeparam name="TResource">Resource type: Job, Field, User...</typeparam>
        /// <param name="filePath">Full path of the XML file</param>
        /// <returns>An instance of WriteDataBase.</returns>
        //public static WriteRequestData<TResource> ParseXMLFile<TResource>(string filePath) where TResource : XmlModel, new()
        //{
        //    WriteRequestData<TResource> result = new WriteRequestData<TResource>();
        //    XDocument doc = XDocument.Load(filePath);
        //    string resourceName = typeof(TResource).Name;
        //    XElement resourceElement = doc.Element(resourceName);
        //    List<TResource> list = new List<TResource>();
        //    foreach (XElement item in resourceElement.Elements("Item"))
        //    {
        //        TResource temp = new TResource();
        //        temp.DictionaryValues = new Dictionary<string, object>();
        //        foreach (XNode prop in item.Nodes())
        //        {
        //            if (prop.NodeType == XmlNodeType.Element)
        //            {
        //                ParseResource<TResource>((XElement)prop, temp.DictionaryValues);
        //            }
        //        }
        //        list.Add(temp);
        //    }
        //    result.Items = list;
        //    return result;
        //}

        //internal static void ParseResource<TResource>(XElement parent, Dictionary<string, object> parentDic)
        //{
        //    if (parentDic.ContainsKey(parent.Name.LocalName))
        //        Log.Warn("The element '{0}' appeals more than 1 time!", parent.Name.LocalName);

        //    if (!parent.HasElements)
        //    {
        //        parentDic[parent.Name.LocalName] = parent.Value;
        //    }
        //    else
        //    {
        //        string[] propNames = parent.Name.LocalName.Split('.');
        //        string propertyName = propNames[propNames.Length - 1];

        //        if (propertyName.StartsWith("P_"))
        //        {
        //            propertyName = propertyName.Substring(2);
        //        }
        //        PropertyInfo propertyInfo = typeof(TResource).GetProperty(propertyName);
        //        //There is a property in the current Resource class
        //        if (propertyInfo != null)
        //        {
        //            ParseProperty<TResource>(propertyInfo, parent, parentDic);
        //        }
        //        else
        //        {
        //            //Try to use XmlElementAttribute
        //            PropertyInfo[] properties = typeof(TResource).GetTypeInfo().GetProperties();
        //            foreach(PropertyInfo property in properties)
        //            {
        //                XmlElementAttribute attribute = property.GetCustomAttribute(typeof(XmlElementAttribute)) as XmlElementAttribute;
        //                if (attribute != null && propertyName.Equals(attribute.ElementName))
        //                {
        //                    propertyInfo = property;
        //                    break;
        //                }
        //            }
        //            if(propertyInfo != null)
        //            {
        //                ParseProperty<TResource>(propertyInfo, parent, parentDic);
        //            }
        //            else //Custom field
        //            {
        //                Log.Info("Cannot find property '{0}' in the class {1}. A XmlModel instance will be created!", 
        //                    propertyName, typeof(TResource).FullName);

        //                XmlModel resource = new XmlModel();

        //                foreach (XElement childElement in parent.Elements())
        //                {
        //                    ParseResource<XmlModel>(childElement, resource.DictionaryValues);
        //                }
        //                parentDic[parent.Name.LocalName] = resource;
        //            }
        //        }
        //    }
        //}

        //internal static void ParseProperty<TResource>(PropertyInfo propertyInfo, XElement parent, Dictionary<string, object> parentDic)
        //{
        //    if (propertyInfo != null)
        //    {
        //        Type propType = propertyInfo.PropertyType;
        //        XElement resourceNode = (XElement)parent.FirstNode;
        //        string[] strTemp = resourceNode.Name.LocalName.Split('.');
        //        Type resourceType = HttpUtil.GetType("APICore.Resources.PublicAPI." + strTemp[0]);
        //        //In case of Element "Item", TResource will be used
        //        if (resourceType == null)
        //            resourceType = typeof(TResource);

        //        if (propType.Name == "ArrayList")
        //        {
        //            ArrayList tempArray = new ArrayList();
        //            foreach (XElement childElement in parent.Elements())
        //            {
        //                //Type type = Type.GetType("APICore.Resources.PublicAPI." + resourceNode.Name);
        //                XmlModel temp = (XmlModel)Activator.CreateInstance(resourceType);
        //                //temp.DictionaryValues = new Dictionary<string, object>();
        //                if (strTemp.Length > 1)
        //                {
        //                    temp.ActualXMLTag = childElement.Name.LocalName;
        //                }

        //                foreach (XElement grandElement in childElement.Elements())
        //                {
        //                    ParseResource<TResource>(grandElement, temp.DictionaryValues);
        //                }
        //                tempArray.Add(temp);
        //            }
        //            parentDic[parent.Name.LocalName] = tempArray;
        //        }
        //        else
        //        {
        //            XmlModel resource = (XmlModel)Activator.CreateInstance(resourceType);
        //            //In case of Option.P_JobMatching, we have to keep the actual XML tag.
        //            if (strTemp.Length > 1)
        //            {
        //                resource.ActualXMLTag = resourceNode.Name.LocalName;
        //            }

        //            foreach (XElement childElement in resourceNode.Elements())
        //            {
        //                ParseResource<TResource>(childElement, resource.DictionaryValues);
        //            }
        //            parentDic[parent.Name.LocalName] = resource;
        //        }
        //    }
        //    else
        //    {
        //        throw new ArgumentNullException("propertyInfo", "The information of the property cannot be null!");
        //    }
        //}
        /// <summary>
        /// Generate XML elements based-on DictionaryValues.
        /// </summary>
        /// <param name="parent">XML Parent Node where all the elements will be added to</param>
        /// <param name="resource">Dictionary that contains all the information to be added</param>
        internal static void AddResourceToXML(XElement parent, Dictionary<string, object> resource)
        {
            foreach (KeyValuePair<string, object> item in resource)
            {
                XElement element = new XElement(item.Key);
                if (item.Value is Dictionary<string, object>)
                {
                    AddResourceToXML(element, (Dictionary<string, object>)item.Value);
                }
                else if (item.Value is XmlModel)
                {
                    XmlModel subResource = (XmlModel)item.Value;

                    // When ActualXMLTag is set, XML node is added using ActualXMLTag's value as the XML Node's name
                    // ActualXMLTag is used for <Option> purpuse only, DictionaryValues is not processed (options do not have internal nodes)
                    if (!string.IsNullOrEmpty(subResource.ActualXMLTag))
                    {
                        XElement actualElement = new XElement(subResource.ActualXMLTag);
                        element.Add(actualElement);
                    }
                    else
                    {
                        Dictionary<string, object> dicTemp = ((XmlModel)item.Value).DictionaryValues;
                        AddResourceToXML(element, dicTemp);
                    }
                }
                else if (item.Value is ArrayList)
                {
                    ArrayList arrTemp = item.Value as ArrayList;

                    if (arrTemp != null)
                    {
                        bool notResourceBaseList = false;
                        foreach (object subItem in arrTemp)
                        {
                            // ListArray of <Options>
                            if (subItem is XmlModel)
                            {
                                XmlModel subResource = subItem as XmlModel;

                                // When ActualXMLTag is set, XML node is added using ActualXMLTag's value as the XML Node's name
                                // ActualXMLTag is used for <Option> purpuse only, DictionaryValues is not processed (options do not have internal nodes)
                                if (!string.IsNullOrEmpty(subResource.ActualXMLTag))
                                {
                                    XElement actualElement = new XElement(subResource.ActualXMLTag);
                                    element.Add(actualElement);
                                }

                                Dictionary<string, object> subDic = subResource.DictionaryValues;
                                if (subDic != null)
                                {
                                    AddResourceToXML(element, subDic);
                                }
                            }
                            // ArrayList of Resources
                            else if (subItem is Model.Model)
                            {
                                Dictionary<string, object> subDic = ((Model.Model)subItem).DictionaryValues;
                                AddResourceToXML(element, subDic);
                            }
                            else
                            {
                                // When subItem do not extend ModelBase, the XML Node is created using item.key as Node's name
                                // It allows adding XML Node with the same NAME to the XML Parent Node

                                // <xxx.FieldName>
                                //      <xxx.InternalFieldName>1</xxx.InternalFieldName>
                                //      <xxx.InternalFieldName>2</xxx.InternalFieldName>
                                //      <xxx.InternalFieldName>3</xxx.InternalFieldName>
                                // </xxx.FieldName>

                                XElement xmlSubNode = new XElement(item.Key, subItem.ToString());
                                parent.Add(xmlSubNode);
                                notResourceBaseList = true;
                            }
                        }

                        // When items at ArrayList do not extends from ModelBase, XML node "element"
                        // Should not be added to XML Parent Node
                        if (notResourceBaseList)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    element.Value = item.Value == null ? string.Empty : item.Value.ToString();
                }
                parent.Add(element);
            }
        }
    }
}
