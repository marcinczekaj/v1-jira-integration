using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using VersionOne.ServiceHost.JiraServices;

namespace IntegrationTests.Enviornments
{
    abstract class Enviornment
    {
        public static Enviornment instance { get{ return new Development(); }}

        public abstract string GetVersionOneUrl { get; }
        public abstract string GetVersionOneUserName { get; }
        public abstract string GetVersionOnePassword { get; }
        public abstract string GetVersionOneReference { get; }

        public abstract string GetVersionOneSource { get; }
        public abstract string GetVersionOneMappingStatus { get; }


        public XmlElement GetVersionOneConfiguration()
        {
            XmlDocument xmlDocument = new XmlDocument();
            
            xmlDocument.LoadXml(string.Format(@"<Settings><ApplicationUrl>{0}</ApplicationUrl><Username>{1}</Username>
                    <Password>{2}</Password><APIVersion>7.2.0.0</APIVersion><IntegratedAuth>true</IntegratedAuth></Settings>", 
                    GetVersionOneUrl, GetVersionOneUserName, GetVersionOnePassword));
            
            return xmlDocument.DocumentElement;
        }

        public abstract JiraServiceConfiguration GetJiraConfiguration { get; }

        public abstract string[] GetExistingProjectNames { get; }
        public abstract string[] GetNonExistingProjectNames { get; }
    }
}
