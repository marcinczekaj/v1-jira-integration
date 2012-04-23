using System;
using System.Xml;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.Tests.ServerConnector {
    public abstract class BaseIntegrationTester {
        protected readonly MockRepository MockRepository = new MockRepository();
        protected VersionOneProcessor V1Processor;
        protected IMetaModel MetaModel;
        protected IServices Services;
        
        protected AssetDisposer AssetDisposer;

        private const string DataUrl = "rest-1.v1/";
        private const string MetaUrl = "meta.v1/";

        protected const string V1Url = "http://integsrv01/VersionOne11/";
        protected const string Username = "admin";
        protected const string Password = "admin";

        protected const string CustomFieldName = "Custom_BaFstatus2";
        protected const string CustomTypeName = "Custom_BaF_Status";

        protected const string RootProjectToken = "Scope:0";

        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            IAPIConnector metaConnector = new V1APIConnector(V1Url + MetaUrl, Username, Password);
            IAPIConnector serviceConnector = new V1APIConnector(V1Url + DataUrl, Username, Password);
            MetaModel = new MetaModel(metaConnector);
            Services = new Services(MetaModel, serviceConnector);

            var doc = new XmlDocument();
            doc.LoadXml(string.Format(@"<Settings> 
                            <APIVersion>7.2.0.0</APIVersion>
                            <ApplicationUrl>{0}</ApplicationUrl>
                            <Username>{1}</Username>
                            <Password>{2}</Password> 
                            <IntegratedAuth>false</IntegratedAuth>
                            <ProxySettings disabled='1'>
                                 <Uri>http://proxyhost:3128</Uri>
                                    <UserName>username</UserName>
                                    <Password>password</Password> 
                                    <Domain>domain</Domain>
                                </ProxySettings>
                            </Settings>", V1Url, Username, Password));

            var loggerMock = MockRepository.Stub<ILogger>();
            V1Processor = new VersionOneProcessor(doc["Settings"], loggerMock);
            V1Processor.AddProperty(Workitem.NumberProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Entity.NameProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Workitem.DescriptionProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Workitem.PriorityProperty, VersionOneProcessor.PrimaryWorkitemType, true);
            V1Processor.AddProperty(Entity.StatusProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Workitem.EstimateProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Workitem.AssetTypeProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Workitem.ParentNameProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Workitem.TeamNameProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Workitem.SprintNameProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Workitem.OrderProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Workitem.ReferenceProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            V1Processor.AddProperty(Entity.NameProperty, VersionOneProcessor.StoryType, false);

            V1Processor.AddProperty(CustomFieldName, VersionOneProcessor.FeatureGroupType, false);
            V1Processor.AddProperty(CustomTypeName, string.Empty, true);
            V1Processor.AddProperty(Entity.NameProperty, VersionOneProcessor.FeatureGroupType, false);
            V1Processor.AddProperty(Workitem.ReferenceProperty, VersionOneProcessor.FeatureGroupType, false);
            V1Processor.AddProperty(VersionOneProcessor.OwnersAttribute, VersionOneProcessor.FeatureGroupType, false);

            V1Processor.AddProperty(FieldInfo.NameProperty, VersionOneProcessor.AttributeDefinitionType, false);
            V1Processor.AddProperty(FieldInfo.AssetTypeProperty, VersionOneProcessor.AttributeDefinitionType, false);
            V1Processor.AddProperty(FieldInfo.AttributeTypeProperty, VersionOneProcessor.AttributeDefinitionType, false);
            V1Processor.AddProperty(FieldInfo.IsReadOnlyProperty, VersionOneProcessor.AttributeDefinitionType, false);
            V1Processor.AddProperty(FieldInfo.IsRequiredProperty, VersionOneProcessor.AttributeDefinitionType, false);

            V1Processor.AddListProperty(CustomFieldName, VersionOneProcessor.FeatureGroupType);

            V1Processor.AddProperty(Entity.NameProperty, VersionOneProcessor.LinkType, false);
            V1Processor.AddProperty(Link.UrlProperty, VersionOneProcessor.LinkType, false);
            V1Processor.AddProperty(Link.OnMenuProperty, VersionOneProcessor.LinkType, false);

            V1Processor.AddProperty(Entity.NameProperty, VersionOneProcessor.MemberType, false);
            V1Processor.AddProperty(Member.EmailProperty, VersionOneProcessor.MemberType, false);
            //V1Processor.AddProperty(Member.DefaultRoleNameProperty, VersionOneProcessor.MemberType, false);

            Assert.IsTrue(V1Processor.ValidateConnection(), "Connection is not valid");
        }

        [SetUp]
        public void SetUp() {
            AssetDisposer = new AssetDisposer(Services);
        }

        [TearDown]
        public void TearDown() {
            AssetDisposer.Dispose();
        }

        protected Asset CreateProject(string scopeName, Oid scheduleOid, string parentProjectToken) {
            var scopeNameDef = MetaModel.GetAttributeDefinition("Scope.Name");
            var scopeParentIdDef = MetaModel.GetAttributeDefinition("Scope.Parent");
            var scopeBeginDateDef = MetaModel.GetAttributeDefinition("Scope.BeginDate");
            var scopeScheduleDef = MetaModel.GetAttributeDefinition("Scope.Schedule");

            var assetScope = new Asset(MetaModel.GetAssetType("Scope"));
            assetScope.SetAttributeValue(scopeNameDef, scopeName);
            assetScope.SetAttributeValue(scopeParentIdDef, Oid.FromToken(parentProjectToken, MetaModel));
            assetScope.SetAttributeValue(scopeBeginDateDef, DateTime.Now);
            
            if (scheduleOid != null) {
                assetScope.SetAttributeValue(scopeScheduleDef, scheduleOid.Momentless.Token);
            }

            Services.Save(assetScope);

            return assetScope;
        }

        protected Asset CreateStory(string name, string description, Oid scopeOid, Oid featureGroupOid, Oid teamOid, Oid sprintOid) {
            var storyNameDef = MetaModel.GetAttributeDefinition("Story.Name");
            var storyScopeDef = MetaModel.GetAttributeDefinition("Story.Scope");
            var storyDescDef = MetaModel.GetAttributeDefinition("Story.Description");
            var storyParentDef = MetaModel.GetAttributeDefinition("Story.Parent");
            var storyTeamDef = MetaModel.GetAttributeDefinition("Story.Team");
            var storyTimeBoxDef = MetaModel.GetAttributeDefinition("Story.Timebox");

            var assetStory = new Asset(MetaModel.GetAssetType("Story"));
            assetStory.SetAttributeValue(storyNameDef, name);
            assetStory.SetAttributeValue(storyScopeDef, scopeOid);
            assetStory.SetAttributeValue(storyDescDef, description);
            if (featureGroupOid != null) {
                assetStory.SetAttributeValue(storyParentDef, featureGroupOid.Momentless.Token);
            }
            if (teamOid != null) {
                assetStory.SetAttributeValue(storyTeamDef, teamOid.Momentless.Token);
            }
            if (sprintOid != null) {
                assetStory.SetAttributeValue(storyTimeBoxDef, sprintOid.Momentless.Token);
            }
            Services.Save(assetStory);

            return assetStory;
        }

        protected Asset CreateDefect(string name, string description, Oid scopeOid, Oid featureGroupOid, Oid teamOid, Oid sprintOid) {
            var defectNameDef = MetaModel.GetAttributeDefinition("Defect.Name");
            var defectScopeDef = MetaModel.GetAttributeDefinition("Defect.Scope");
            var defectDescDef = MetaModel.GetAttributeDefinition("Defect.Description");
            var defectParentDef = MetaModel.GetAttributeDefinition("Defect.Parent");
            var defectTeamDef = MetaModel.GetAttributeDefinition("Defect.Team");
            var defectTimeBoxDef = MetaModel.GetAttributeDefinition("Defect.Timebox");

            var assetDefect = new Asset(MetaModel.GetAssetType("Defect"));
            assetDefect.SetAttributeValue(defectNameDef, name);
            assetDefect.SetAttributeValue(defectScopeDef, scopeOid);
            assetDefect.SetAttributeValue(defectDescDef, description);

            if (featureGroupOid != null) {
                assetDefect.SetAttributeValue(defectParentDef, featureGroupOid.Momentless.Token);
            }
            if (teamOid != null) {
                assetDefect.SetAttributeValue(defectTeamDef, teamOid.Momentless.Token);
            }
            if (sprintOid != null) {
                assetDefect.SetAttributeValue(defectTimeBoxDef, sprintOid.Momentless.Token);
            }
            Services.Save(assetDefect);

            return assetDefect;
        }

        protected Asset CreateFeatureGroup(string name, Oid scopeOid, Oid parentFGroup) {
            var themeNameDef = MetaModel.GetAttributeDefinition("Theme.Name");
            var themeScopeDef = MetaModel.GetAttributeDefinition("Theme.Scope");
            var themeParentDef = MetaModel.GetAttributeDefinition("Theme.Parent");

            var assetFGroup = new Asset(MetaModel.GetAssetType("Theme"));
            assetFGroup.SetAttributeValue(themeNameDef, name);
            assetFGroup.SetAttributeValue(themeScopeDef, scopeOid);
            if (parentFGroup != null) {
                assetFGroup.SetAttributeValue(themeParentDef, parentFGroup.Momentless.Token);
            }
            Services.Save(assetFGroup);

            return assetFGroup;
        }

        protected Asset CreateTeam(string name) {
            var teamNameDef = MetaModel.GetAttributeDefinition("Team.Name");

            var assetTeam = new Asset(MetaModel.GetAssetType("Team"));
            assetTeam.SetAttributeValue(teamNameDef, name);
            Services.Save(assetTeam);

            return assetTeam;
        }

        protected Asset CreateSprint(string name, Oid scheduleOid) {
            var timeboxNameDef = MetaModel.GetAttributeDefinition("Timebox.Name");
            var timeboxEndDateDef = MetaModel.GetAttributeDefinition("Timebox.EndDate");
            var timeboxBeginDateDef = MetaModel.GetAttributeDefinition("Timebox.BeginDate");
            var timeboxStateDef = MetaModel.GetAttributeDefinition("Timebox.State");
            var timeboxScheduleDef = MetaModel.GetAttributeDefinition("Timebox.Schedule");

            var assetTimeBox = new Asset(MetaModel.GetAssetType("Timebox"));
            assetTimeBox.SetAttributeValue(timeboxNameDef, name);
            assetTimeBox.SetAttributeValue(timeboxEndDateDef, DateTime.Now.AddDays(5));
            assetTimeBox.SetAttributeValue(timeboxBeginDateDef, DateTime.Now);
            assetTimeBox.SetAttributeValue(timeboxStateDef, Oid.FromToken("State:100", MetaModel));
            assetTimeBox.SetAttributeValue(timeboxScheduleDef, scheduleOid.Momentless.Token);
            Services.Save(assetTimeBox);

            return assetTimeBox;
        }

        protected Asset CreateSchedule(string name) {
            var scheduleNameDef = MetaModel.GetAttributeDefinition("Schedule.Name");
            var scheduleTimeboxGapDef = MetaModel.GetAttributeDefinition("Schedule.TimeboxGap");
            var scheduleTimeboxLengthDef = MetaModel.GetAttributeDefinition("Schedule.TimeboxLength");

            var assetSchedule = new Asset(MetaModel.GetAssetType("Schedule"));
            assetSchedule.SetAttributeValue(scheduleNameDef, name);
            assetSchedule.SetAttributeValue(scheduleTimeboxGapDef, "0 Days");
            assetSchedule.SetAttributeValue(scheduleTimeboxLengthDef, "14 Days");
            Services.Save(assetSchedule);

            return assetSchedule;
        }

        protected void ExecuteOperation(Asset subject, string operationName) {
            if(subject == null) {
                return;
            }

            var operation = subject.AssetType.GetOperation(operationName);
            Services.ExecuteOperation(operation, subject.Oid);
        }

        protected Asset LoadAsset(Oid oid, string assetTypeName, params string[] attribs) {
            var assetType = MetaModel.GetAssetType(assetTypeName);

            var query = new Query(oid);
            foreach(var attrib in attribs) {
                query.Selection.Add(assetType.GetAttributeDefinition(attrib));
            }
            
            return Services.Retrieve(query).Assets[0];
        }
    }
}