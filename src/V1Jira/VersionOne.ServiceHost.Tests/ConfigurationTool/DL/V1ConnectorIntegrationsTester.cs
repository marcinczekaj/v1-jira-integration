using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.DL;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.DL.Exceptions;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.DL {
    [TestFixture]
    public class V1ConnectorIntegrationsTester {
        private V1Connector v1Connector;
        private const string V1Url = "myVersionOneApplicationUrl";
        private const string V1Username = "admin";
        private const string V1Password = "admin";

        [SetUp]
        public void Setup() {
            v1Connector = V1Connector.Instance;
            v1Connector.Connect(GetVersionOneSettings());
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication.")]
        public void GetStoryListFields() {
            var list = v1Connector.GetCustomFieldList("Story", FieldType.List);
            Assert.AreNotEqual(0, list.Count);
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication.")]
        public void GetStoryNumericFields() {
            var list = v1Connector.GetCustomFieldList("Story", FieldType.Numeric);
            Assert.AreNotEqual(0, list.Count);
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication.")]
        public void GetTypeValues() {
            var list = v1Connector.GetValuesForType("Custom_Story_BaF_Status");
            Assert.AreEqual(4, list.Count);
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication.")]
        [ExpectedException(typeof(AssetTypeException))]
        public void GetMissedTypeValues() {
            v1Connector.GetValuesForType("dont_exist_type");
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication.")]
        public void GetFieldTypeByFieldName() {
            var typeName = v1Connector.GetTypeByFieldName("Custom_BaFstatus4", "Story");
            Assert.AreEqual("Custom_Story_BaF_Status", typeName);
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication.")]
        [ExpectedException(typeof(AssetTypeException))]
        public void GetFieldTypeByFieldNameWithUnknownType() {
            v1Connector.GetTypeByFieldName("Custom_BaFstatus4", "Story1");
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication.")]
        [ExpectedException(typeof(FieldNameException))]
        public void GetFieldTypeByFieldNameWithUnknownFieldName() {
            v1Connector.GetTypeByFieldName("Custom_BaFstatus4_123", "Story");
        }

        private static VersionOneSettings GetVersionOneSettings() {
            var settings = new VersionOneSettings {
                ApplicationUrl = V1Url,
                Password = V1Password,
                Username = V1Username
            };
            return settings;
        }
    }
}