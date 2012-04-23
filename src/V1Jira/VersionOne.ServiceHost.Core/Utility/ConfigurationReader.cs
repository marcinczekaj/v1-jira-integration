/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using VersionOne.ServiceHost.Core.Configuration;

namespace VersionOne.ServiceHost.Core.Utility
{
	public class ConfigurationReader
	{
		private static string Read(XmlElement config, string name, string def)
		{
			return config[name] != null ? config[name].InnerText : def;
		}

		private static bool Read(XmlElement config, string name, bool def)
		{
			bool res = def;
			if (config[name] != null)
				if (!bool.TryParse(config[name].InnerText, out res))
					res = def;
			return res;
		}

		public static void ReadConfigurationValues<T>(T config, XmlElement configSection)
		{
			FieldInfo[] configFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

			foreach (FieldInfo fieldInfo in configFields)
			{
                if(fieldInfo.GetCustomAttributes(typeof(IgnoreConfigFieldAttribute), false).Length > 0) 
                {
                    continue;
                }

				object[] tagNames = fieldInfo.GetCustomAttributes(typeof(ConfigFileValueAttribute), false);

				if (tagNames.Length == 1)
				{
					ConfigFileValueAttribute tagName = tagNames[0] as ConfigFileValueAttribute;
					if (tagName.IsBool)
					{
						fieldInfo.SetValue(config, Read(configSection, tagName.Name, bool.Parse(tagName.DefaultValue)));
					}
					else
					{
						fieldInfo.SetValue(config, Read(configSection, tagName.Name, tagName.DefaultValue));
					}

					System.Diagnostics.Debug.WriteLine(string.Format("Set '{0}' to '{1}'.", fieldInfo.Name, fieldInfo.GetValue(config)));
				}
				else
				{
					throw new ConfigurationException(string.Format("No tag specified for {0}.", fieldInfo.Name));
				}
			}
		}

        /// <summary>
        /// Extract mappings information, add it to configuration entity.
        /// </summary>
        /// <param name="mappings">Mapping to fill.</param>
        /// <param name="mappingNode">Configuration XML element that contains mapping nodes.</param>
        /// <param name="nodeName1">Node name with information from first system.</param>
        /// <param name="nodeName2">Node name with information from second system.</param>
        public static void ProcessMappingSettings(IDictionary<MappingInfo, MappingInfo> mappings, XmlNode mappingNode, string nodeName1, string nodeName2) {
            ProcessMappingSettings(mappings, null, mappingNode, nodeName1, nodeName2);
        }

        public static void ProcessMappingSettings(IDictionary<MappingInfo, MappingInfo> mappings, IDictionary<MappingInfo, MappingInfo> reverseMappings, XmlNode mappingNode, string nodeName1, string nodeName2) {
            if (mappingNode == null) {
                return;
            }
            
            XmlNodeList nodeList = mappingNode.SelectNodes("Mapping");

            if (nodeList == null) {
                return;
            }
            for (int i = 0; i < nodeList.Count; i++) {
                XmlNode node = nodeList[i];
                XmlNode nodeData1 = node.SelectSingleNode(nodeName1);
                XmlNode nodeData2 = node.SelectSingleNode(nodeName2);
                MappingInfo firstSystem = ParseMappingNode(nodeData1);
                MappingInfo secondSystem = ParseMappingNode(nodeData2);
                if (mappings.ContainsKey(firstSystem)) {
                    throw new ConfigurationException(
                        string.Format("Can't add new mapping data. Already exist mapping for {0}.",
                            string.IsNullOrEmpty(firstSystem.Name) ? firstSystem.Id : firstSystem.Name));
                }
                mappings.Add(firstSystem, secondSystem);

                if (reverseMappings == null || reverseMappings.ContainsKey(secondSystem))
                    continue;

                reverseMappings.Add(secondSystem, firstSystem);
            }
        }
        
        private static MappingInfo ParseMappingNode(XmlNode node) {
            XmlAttribute idAttribute = node.Attributes["id"];
            string id = idAttribute != null ? idAttribute.Value : null;
            string name = node.InnerText;
            return new MappingInfo(id, name);
        }

        public static void ProcessMappingSettings(IDictionary<MappingInfo, IList<MappingInfo>> mappings, XmlNode mappingNode,string mappingNodeName, string nodeName1, string nodeName2) {
            if (mappingNode == null) {
                return;
            }

            XmlNodeList nodeList = mappingNode.SelectNodes(mappingNodeName);

            if (nodeList == null) {
                return;
            }
            for (int i = 0; i < nodeList.Count; i++) {
                XmlNode node = nodeList[i];
                XmlNode nodeData1 = node.SelectSingleNode(nodeName1);
                XmlNodeList nodeData2 = node.SelectNodes(nodeName2);
                MappingInfo firstSystem = ParseMappingNode(nodeData1);
                IList<MappingInfo> list = new List<MappingInfo>();
                foreach (XmlNode nodeElement in nodeData2) {
                    MappingInfo secondSystem = ParseMappingNode(nodeElement);
                    list.Add(secondSystem);
                }
                
                if (mappings.ContainsKey(firstSystem)) {
                    throw new ConfigurationException(
                        string.Format("Can't add new mapping data. Already exist mapping for {0}.",
                            string.IsNullOrEmpty(firstSystem.Name) ? firstSystem.Id : firstSystem.Name));
                }
                mappings.Add(firstSystem, list);
            }
        }


	}


	[AttributeUsage(AttributeTargets.Field)]
	public class ConfigFileValueAttribute : Attribute
	{
		public ConfigFileValueAttribute(string name)
		{
			_name = name;
		}

		public ConfigFileValueAttribute(string name, string defaultValue)
		{
			_name = name;
			_defaultValue = defaultValue;
		}

		public ConfigFileValueAttribute(string name, string @defaultValue, bool isBool)
		{
			_name = name;
			_defaultValue = defaultValue;
			_isBool = isBool;
		}

		#region Name property
		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		#endregion

		#region DefaultValue property
		private string _defaultValue = string.Empty;
		public string DefaultValue
		{
			get { return _defaultValue; }
			set { _defaultValue = value; }
		}
		#endregion

		#region IsBool property
		private bool _isBool = false;
		public bool IsBool
		{
			get { return _isBool; }
			set { _isBool = value; }
		}
		#endregion
	}

    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreConfigFieldAttribute : Attribute { }

	public class ConfigurationException : Exception
	{
		public ConfigurationException(string message)
			: base(message)
		{
		}
	}

}
