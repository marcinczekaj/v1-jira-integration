using System.Linq;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool;
using VersionOne.ServiceHost.ConfigurationTool.DL;
using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Entities {
    [TestFixture]
    public class ClearQuestServiceConfigurationTester : BaseServiceEntityTester<ClearQuestServiceEntity> {
        private XmlEntitySerializer serializer;

        [SetUp]
        public override void SetUp() {
            serializer = new XmlEntitySerializer();
        }

        [Test]
        public void SerializeTest() {
            const string urlValue = "http://versionone/";
            const long timerValue = 12345;

            var entity = new ClearQuestServiceEntity {
                Timer = {TimeoutMilliseconds = timerValue},
                ConnectionName = urlValue
            };

            serializer.Serialize(new BaseServiceEntity[] { entity });

            var nodeList = serializer.OutputDocument.SelectNodes(XmlEntitySerializer.RootNodeXPath +
                "/*[@class='VersionOne.ServiceHost.ClearQuestServices.ClearQuestHostedService, VersionOne.ServiceHost.ClearQuestServices']");
            
            Assert.IsNotNull(nodeList);
            Assert.AreEqual(nodeList.Count, 1);
            Assert.AreEqual(((XmlElement)nodeList[0]).GetElementsByTagName("ClearQuestConnectionName")[0].InnerText, urlValue);
        }

        [Test]
        public void DeserializeTest() {
            serializer.Document.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <configuration>
	              <Services>
                	<ClearQuestHostedService class=""VersionOne.ServiceHost.ClearQuestServices.ClearQuestHostedService, VersionOne.ServiceHost.ClearQuestServices"">
                      <ClearQuestConnectionName>CQ</ClearQuestConnectionName>
                      <ClearQuestMandatoryFields>
                        <Field name=""Resolution"">Fixed</Field>
                        <Field name=""Resolution2"">Fixed2</Field>
                      </ClearQuestMandatoryFields>
                    </ClearQuestHostedService>
                  </Services>
                </configuration>");
            
            var entities = serializer.Deserialize().ToList();
            
            Assert.AreEqual(entities.Count, 1);
            Assert.AreEqual(typeof(ClearQuestServiceEntity), entities[0].GetType());
            
            var entity = (ClearQuestServiceEntity)entities[0];
            
            Assert.AreEqual(entity.MandatoryFields.Count, 2);
            Assert.AreEqual(entity.MandatoryFields[0].Value, "Fixed");
            Assert.AreEqual(entity.MandatoryFields[1].Value, "Fixed2");
            Assert.AreEqual(entity.MandatoryFields[0].Name, "Resolution");
            Assert.AreEqual(entity.MandatoryFields[1].Name, "Resolution2");
        }

        protected override ClearQuestServiceEntity CreateEntity() {
            var entity = new ClearQuestServiceEntity {
                TagName = "tag_name",
                ConnectionName = "ConnectionName",
                Password = "password",
                UserName = "user_name",
                DataBase = "database",
                SourceValue = "source_value",
                UrlTemplate = "url_template",
                UrlTitle = "url_title",
                WaitedSubmitToV1State = "waited_submit_to_V1State",
                SubmittedToV1Action = "SubmittedToV1Action",
                SubmittedToV1State = "SubmittedToV1State",
                CloseAction = "CloseAction",
                EntityType = "EntityType",
                IdField = "IdField",
                DefectTitleField = "DefectTitleField",
                DescriptionField = "DescriptionField",
                ProjectNameField = "ProjectNameField",
                OwnerLoginField = "OwnerLoginField",
                StateField = "StateField",
                ModifyAction = "ModifyAction",
                Timer = new TimerEntity {
                    PublishClass = "VersionOne.ServiceHost.ClearQuestServices.ClearQuestHostedService+IntervalSync, VersionOne.ServiceHost.ClearQuestServices"
                }
            };

            return entity;
        }

        [Test]
        public void CreateCQEntity() {
            var entity = CreateEntity();
            var settings = new ServiceHostConfiguration();
            settings.AddService(entity);

            ExpectMap.Add("ClearQuestConnectionName", entity.ConnectionName);
            ExpectMap.Add("ClearQuestUsername", entity.UserName);
            ExpectMap.Add("ClearQuestPassword", entity.Password);
            ExpectMap.Add("ClearQuestDatabase", entity.DataBase);
            ExpectMap.Add("SourceFieldValue", entity.SourceValue);
            ExpectMap.Add("ClearQuestWebUrlTemplate", entity.UrlTemplate);
            ExpectMap.Add("ClearQuestWebUrlTitle", entity.UrlTitle);
            ExpectMap.Add("ClearQuestWaitedSubmitToV1State", entity.WaitedSubmitToV1State);
            ExpectMap.Add("ClearQuestSubmittedToV1Action", entity.SubmittedToV1Action);
            ExpectMap.Add("ClearQuestSubmittedToV1State", entity.SubmittedToV1State);
            ExpectMap.Add("ClearQuestCloseAction", entity.CloseAction);
            ExpectMap.Add("ClearQuestEntityType", entity.EntityType);
            ExpectMap.Add("ClearQuestIDField", entity.IdField);
            ExpectMap.Add("ClearQuestDefectTitleField", entity.DefectTitleField);
            ExpectMap.Add("ClearQuestDescriptionField", entity.DescriptionField);
            ExpectMap.Add("ClearQuestProjectNameField", entity.ProjectNameField);
            ExpectMap.Add("ClearQuestOwnerLoginField", entity.OwnerLoginField);
            ExpectMap.Add("ClearQuestStateField", entity.StateField);
            ExpectMap.Add("ClearQuestModifyAction", entity.ModifyAction);

            ValidateXml(ServicesMap.ClearQuestService, settings);
        }
    }
}