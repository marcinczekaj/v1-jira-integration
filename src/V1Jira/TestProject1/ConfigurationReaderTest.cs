using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using VersionOne.ServiceHost.Core.Utility;
using VersionOne.ServiceHost.Core.Configuration;

namespace IntegrationTests {
    [TestClass]
    public class ConfigurationReaderTest {

        [TestMethod]
        public void ProcessMappingSettings_TestMultivalueMapping_JiraStatus() { 
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(@"   
       <JiraStatuses>
        <Mapping>
          <Status>Open</Status>
          <Transition>Create Issue</Transition>
          <Transition>Stop Progress</Transition>
        </Mapping>
        <Mapping>
          <Status>In Progress</Status>
          <Transition>Start Progress</Transition>
        </Mapping>
        <Mapping>
          <Status>Resolved</Status>
          <Transition>Resolve Issue</Transition>
        </Mapping>
        <Mapping>
          <Status>Reopened</Status>
          <Transition>Reopen Issue</Transition>
        </Mapping>
        <Mapping>
          <Status>Closed</Status>
          <Transition>Close Issue</Transition>
        </Mapping>
      </JiraStatuses>");

            XmlElement inputConfiguration = doc.DocumentElement;

            Dictionary<MappingInfo, IList<MappingInfo>> result = new Dictionary<MappingInfo, IList<MappingInfo> >() ;
            ConfigurationReader.ProcessMappingSettings(result, inputConfiguration, "Mapping", "Status", "Transition");
            Assert.AreEqual(5, result.Count);

            foreach (KeyValuePair<MappingInfo, IList<MappingInfo>> pair in result) { 
                Console.WriteLine(string.Format("You can get to issue {0} using this transitions: {1}", pair.Key.Name, string.Join(" ,", pair.Value.Select( x => x.Name).ToArray()) ));
            }

       }
        
        
        [TestMethod]
        public void ProcessMappingSettings_TestSinglMapping() {

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
                @"<ProjectMappings>
                    <Mapping>
                        <JIRAProject id='VOID'></JIRAProject>
                        <VersionOneProject id='Scope:2257958'/>
                    </Mapping>
                    <Mapping>
                        <JIRAProject>VersionOne Integration Demo</JIRAProject>
                        <VersionOneProject id='Scope:2257958'/>
                    </Mapping>       
                    <Mapping>
                        <JIRAProject>JIRAProjectName 1</JIRAProject>
                        <VersionOneProject id='Scope:0'></VersionOneProject>
                    </Mapping>
                    <Mapping>
                        <JIRAProject>JIRAProjectName 2</JIRAProject>
                        <VersionOneProject id='Scope:2'></VersionOneProject>
                    </Mapping>
                </ProjectMappings>");
            XmlElement inputConfiguration = doc.DocumentElement;


            Dictionary<MappingInfo, MappingInfo> result = new Dictionary<MappingInfo, MappingInfo>();
           
            ConfigurationReader.ProcessMappingSettings(result, inputConfiguration, "JIRAProject" , "VersionOneProject");
            Assert.AreEqual(4, result.Count);


            foreach(KeyValuePair<MappingInfo, MappingInfo> mapping in result){
                Console.WriteLine(string.Format("{0}[{1}] maps to {2}[{3}]", mapping.Key.Name, mapping.Key.Id, mapping.Value.Name, mapping.Value.Id));
            }
        
        }

        [TestMethod]
        public void TestReverseMapping() {
            // given
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(
                @"<PriorityMappings>
        <Mapping>
          <JIRAPriority id='1'>Blocker</JIRAPriority>
          <VersionOnePriority id='WorkitemPriority:140'>High</VersionOnePriority>
        </Mapping>
        <Mapping>
          <JIRAPriority id='2'>Critical</JIRAPriority>
          <VersionOnePriority id='WorkitemPriority:140'>High</VersionOnePriority>
        </Mapping>
        <Mapping>
          <JIRAPriority id='3'>Major</JIRAPriority>
          <VersionOnePriority id='WorkitemPriority:139'>Medium</VersionOnePriority>
        </Mapping>
      </PriorityMappings>");

            XmlElement inputConfiguration = doc.DocumentElement;

            Dictionary<MappingInfo, MappingInfo> mapping = new Dictionary<MappingInfo, MappingInfo>();
            Dictionary<MappingInfo, MappingInfo> reverseMapping = new Dictionary<MappingInfo, MappingInfo>();
            var v1priority = new MappingInfo("WorkitemPriority:140", "High");

            //when
            ConfigurationReader.ProcessMappingSettings(mapping, reverseMapping, inputConfiguration, "JIRAPriority", "VersionOnePriority");

            //then
            Assert.AreEqual(3, mapping.Count);
            Assert.AreEqual("Blocker", reverseMapping[v1priority].Name);
            Assert.AreNotEqual("Critical", reverseMapping[v1priority].Name);

            foreach (KeyValuePair<MappingInfo, MappingInfo> pair in mapping) {
                Console.WriteLine(string.Format("{0}[{1}] maps to {2}[{3}]", pair.Key.Name, pair.Key.Id, pair.Value.Name, pair.Value.Id));
            }

        }



    }
}
