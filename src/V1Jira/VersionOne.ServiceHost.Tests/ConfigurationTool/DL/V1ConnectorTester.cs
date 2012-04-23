using System.Collections.Generic;
using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.DL;
using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.DL {
    [TestFixture]
    public class V1ConnectorTester {
        private const string V1Url = "http://integsrv01.internal.corp/VersionOne/";
        private const string V1WindowsAuthUrl = "http://integsrv01/VersionOneWin/";
        private const string V1Username = "admin";
        private const string V1Password = "admin";
        private const string ProxyPath = "http://integvm01:3128";
        private const string ProxyUsername = "user1";
        private const string ProxyPassword = "user1";
        private const string ProxyDomain = "";

        [Test]
        [Ignore("This test requires running V1 server with credential authentication.")]
        public void ValidateConnectionTest() {
            VersionOneSettings settings = GetVersionOneSettingsWihthoutProxy(false);
            bool result = V1Connector.Instance.ValidateConnection(settings);
            Assert.IsTrue(result);
            settings.Password = "wrong password";
            result = V1Connector.Instance.ValidateConnection(settings);
            Assert.IsFalse(result);
        }

        [Test]
        [Ignore("This test requires running V1 server with Windows integrated authentication and current user having sufficient permissions to login.")]
        public void ValidateConnectionWinIntegratedSecurityTest() {
            VersionOneSettings settings = GetVersionOneSettingsWihthoutProxy(true);
            settings.ApplicationUrl = V1WindowsAuthUrl;
            bool result = V1Connector.Instance.ValidateConnection(settings);
            Assert.IsTrue(result);
            settings.IntegratedAuth = false;
            result = V1Connector.Instance.ValidateConnection(settings);
            Assert.IsFalse(result);
        }

        [Test]
        [Ignore("This test needs internet access and working DNS.")]
        public void ValidateConnectionWrongUrlTest() {
            // example.com domain is reserved, so it is safe to assume that there's no V1 server.
            VersionOneSettings settings = GetVersionOneSettingsWihthoutProxy(false);
            settings.ApplicationUrl = "http://example.com/versionone/";
            bool result = V1Connector.Instance.ValidateConnection(settings);
            Assert.IsFalse(result);
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication.")]
        public void ConnectTest() {
            VersionOneSettings settings = GetVersionOneSettingsWihthoutProxy(false);
            V1Connector.Instance.Connect(settings);
            Assert.IsTrue(V1Connector.Instance.IsConnected);
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication and default test statuses.")]
        public void GetTestStatusListTest() {
            VersionOneSettings settings = GetVersionOneSettingsWihthoutProxy(false);
            V1Connector.Instance.Connect(settings);
            IDictionary<string, string> result = V1Connector.Instance.GetTestStatuses();
            Assert.AreEqual(result.Count, 2);
            Assert.IsTrue(result["Passed"] == "TestStatus:129");
            Assert.IsTrue(result["Failed"] == "TestStatus:155");
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication and default Defect entity setup.")]
        public void GetReferenceFieldListTest() {
            VersionOneSettings settings = GetVersionOneSettingsWihthoutProxy(false);
            V1Connector.Instance.Connect(settings);
            IList<string> fields = V1Connector.Instance.GetReferenceFieldList(V1Connector.DefectTypeToken);
            ListAssert.Contains("Reference", fields);
            Assert.AreEqual(fields.Count, 1);
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication and proxy server.")]
        public void ValidateConnectionWithProxyTest() {
            VersionOneSettings settings = GetVersionOneSettingsWithProxy(false);
            bool result = V1Connector.Instance.ValidateConnection(settings);
            Assert.IsTrue(result);
            settings.ProxySettings.Password = "wrong password";
            result = V1Connector.Instance.ValidateConnection(settings);
            Assert.IsFalse(result);
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication and proxy server.")]
        public void ConnectWithProxyTest() {
            VersionOneSettings settings = GetVersionOneSettingsWithProxy(false);
            V1Connector.Instance.Connect(settings);
            Assert.IsTrue(V1Connector.Instance.IsConnected);
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication and default test statuses and proxy server.")]
        public void GetTestStatusListWithProxyTest() {
            VersionOneSettings settings = GetVersionOneSettingsWithProxy(false);
            V1Connector.Instance.Connect(settings);
            IDictionary<string, string> result = V1Connector.Instance.GetTestStatuses();
            Assert.AreEqual(result.Count, 2);
            Assert.IsTrue(result["Passed"] == "TestStatus:129");
            Assert.IsTrue(result["Failed"] == "TestStatus:155");
        }

        [Test]
        [Ignore("This test requires running V1 server with credential authentication, default Defect entity setup and proxy server.")]
        public void GetReferenceFieldListWithProxyTest() {
            VersionOneSettings settings = GetVersionOneSettingsWithProxy(false);
            V1Connector.Instance.Connect(settings);
            IList<string> fields = V1Connector.Instance.GetReferenceFieldList(V1Connector.DefectTypeToken);
            ListAssert.Contains("Reference", fields);
            Assert.AreEqual(fields.Count, 1);
        }

        private VersionOneSettings GetVersionOneSettingsWithProxy(bool integratedAuth) {
            VersionOneSettings settings = GetVersionOneSettingsWihthoutProxy(integratedAuth);
            settings.ProxySettings = new ProxyConnectionSettings();
            settings.ProxySettings.Uri = ProxyPath;
            settings.ProxySettings.UserName = ProxyUsername;
            settings.ProxySettings.Password = ProxyPassword;
            settings.ProxySettings.Domain = ProxyDomain;
            settings.ProxySettings.Enabled = true;
            return settings;
        }

        private VersionOneSettings GetVersionOneSettingsWihthoutProxy(bool integratedAuth) {
            VersionOneSettings settings = new VersionOneSettings();
            settings.ApplicationUrl = V1Url;
            settings.IntegratedAuth = integratedAuth;
            settings.Password = V1Password;
            settings.Username = V1Username;
            return settings;
        }

    }
}