using System;
using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.SourceServices.Perforce;

namespace VersionOne.ServiceHost.Tests.SourceServices.Perforce {
    [TestFixture]
    public class P4ConnectorTester {
        private readonly IList<P4ChangeSetEventArgs> list = new List<P4ChangeSetEventArgs>();

        private static XmlElement config;

        private static XmlElement Config {
            get {
                if(config == null) {
                    var doc = new XmlDocument();
                    doc.LoadXml("<Config><Port>perforce:1666</Port>" +
                        "<View>//Depot/VersionOne/...</View>" +
                        "<User></User><Password></Password>" +
                        "<ReferenceExpression>[BD]{1}-[0-9]+</ReferenceExpression></Config>");
                    config = doc.DocumentElement;
                }

                return config;
            }
        }

        [Ignore("This test should only be run manually.")]
        [Test]
        public void Foo() {
            var reader = new TestableP4ReaderService();
            IEventManager eventManager = new EventManager();
            reader.Initialize(Config, eventManager, null, null);

            eventManager.Publish(new P4ReaderHostedService.P4ReaderIntervalSync());

            reader.Dispose();
        }

        [Ignore("This test should only be run manually.")]
        [Test]
        public void Bar() {
            var connector = new P4Connector();
            connector.NewChange += LogChangeSet;
            connector.Error += connector_Error;
            connector.Configure(Config["Port"].InnerText, Config["User"].InnerText, Config["Password"].InnerText);
            connector.DetectNewChanges(Config["View"].InnerText, 0);

            connector.Dispose();
        }
        
        private static void connector_Error(object sender, P4ExceptionEventArgs e) {
            Console.WriteLine(e.Exception.ToString());
            throw e.Exception;
        }

        private static void LogChangeSet(object sender, P4ChangeSetEventArgs e) {
            Console.WriteLine(e);
        }

        private void CollectChangeSet(object sender, P4ChangeSetEventArgs e) {
            list.Add(e);
        }

        [Ignore("This test should only be run manually.")]
        [Test]
        public void TestGetRevisionProperty() {
            list.Clear();
            P4Connector connector = new P4Connector();
            connector.NewChange += CollectChangeSet;
            connector.Error += connector_Error;

            connector.Configure((Config["Port"].InnerText), (Config["User"].InnerText), (Config["Password"].InnerText));
            connector.DetectNewChanges(Config["View"].InnerText, 0);
            connector.Dispose();

            P4ChangeSetEventArgs cs = list[25];
            Assert.AreEqual("rozhnev", cs.User);
            Assert.AreEqual("Effort done\n", cs.Message);
            Assert.AreEqual(new DateTime(2008, 07, 07, 23, 30, 32), cs.ChangeDate);
            Assert.AreEqual(381537, cs.Change);
            Assert.AreEqual(1, cs.Files.Count);
            Assert.AreEqual("//depot/VersionOne/JSDK/Doc/Status/VersionOne tracking.xls", cs.Files[0]);

            cs = list[208];
            Assert.AreEqual("Solomin", cs.User);
            Assert.AreEqual("testing -----------testB-01382test\n", cs.Message);
            Assert.AreEqual(new DateTime(2008, 09, 22, 12, 06, 51), cs.ChangeDate);
            Assert.AreEqual(403312, cs.Change);
            Assert.AreEqual(2, cs.Files.Count);
            Assert.AreEqual("//depot/VersionOne/JSDK/Doc/Status/todoVersionOne.txt", cs.Files[0]);
            Assert.AreEqual("//depot/VersionOne/JSDK/Src/AllTestsDone-ver/todo.txt", cs.Files[1]);

            list.Clear();
        }
    }
}