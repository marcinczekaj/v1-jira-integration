using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool;
using VersionOne.ServiceHost.ConfigurationTool.DL;
using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Entities {
    public abstract class BaseServiceEntityTester<TEntity> : BaseTester where TEntity : BaseEntity {
        protected readonly Dictionary<string, string> ExpectMap = new Dictionary<string, string>();
        protected abstract TEntity CreateEntity();

        protected void ValidateXml(ServicesMap service, ServiceHostConfiguration settings) {
            var serializer = new XmlEntitySerializer();
            serializer.Serialize(settings.Services);
            
            var services = serializer.OutputDocument.SelectNodes(XmlEntitySerializer.RootNodeXPath + "/*[starts-with(@class,'" + service.FullTypeName + "')]");

            Assert.AreEqual(1, services.Count);

            foreach(string tagName in ExpectMap.Keys) {
                var list = services[0].SelectNodes(tagName);

                Assert.AreEqual(1, list.Count, tagName + " not found.");
                Assert.AreEqual(ExpectMap[tagName], list[0].InnerText, tagName + " is wrong");
            }
        }
    }
}
