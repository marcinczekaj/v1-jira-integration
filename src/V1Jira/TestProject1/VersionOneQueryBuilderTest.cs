using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;
using VersionOne.ServiceHost.Core.Logging;

using IntegrationTests.Enviornments;


namespace IntegrationTests
{
    [TestClass]
    public class VersionOneQueryBuilderTest
    {
        private IServices services;
        private IMetaModel metaModel;
        private IQueryBuilder queryBuilder = new QueryBuilder();

        [TestInitialize]
        public void Initialize() {
            var connector = new V1Central(Enviornment.instance.GetVersionOneConfiguration());
            connector.Validate();
            services = connector.Services;
            metaModel = connector.MetaModel;
            queryBuilder.Setup(services, metaModel, connector.Loc);
        }

        [TestMethod]
        public void SearchSource() {
            
            //given
            const string WorkitemSourceType = "StorySource";
            string externalSystemName = "JIRA";

            var sourceValues = queryBuilder.QueryPropertyValues(WorkitemSourceType);
            var source = sourceValues.Where(item => string.Equals(item.Name, externalSystemName)).FirstOrDefault();

            Assert.IsNotNull(source);
        }




        [TestMethod]
        public void QueryForWorkitem_QueryForNameAndStatus() {

            Oid defectId = Oid.FromToken("Defect:2258150", metaModel);

            Query query = new Query(defectId);
            IAssetType defectType = metaModel.GetAssetType("Defect");
            IAttributeDefinition nameAttribute = defectType.GetAttributeDefinition("Name");
            IAttributeDefinition statusAttribute = defectType.GetAttributeDefinition("Status.Name");
            query.Selection.Add(nameAttribute);
            query.Selection.Add(statusAttribute);

            QueryResult result = services.Retrieve(query);

            Asset defect = result.Assets[0];
            string name = defect.GetAttribute(nameAttribute).Value.ToString();
            string status = defect.GetAttribute(statusAttribute).Value.ToString();

            Console.WriteLine(string.Format("{0} - {1} - {2}", defect.Oid.Token, name, status));

        }

        [TestMethod]
        public void QueryForNameAndStatus() {

            Oid defectId = Oid.FromToken("Defect:2258150", metaModel);

            Query query = new Query(defectId);
            IAssetType defectType = metaModel.GetAssetType("Defect");
            IAttributeDefinition nameAttribute = defectType.GetAttributeDefinition("Name");
            IAttributeDefinition statusAttribute = defectType.GetAttributeDefinition(Entity.StatusProperty);
            IAttributeDefinition statusNameAttribute = defectType.GetAttributeDefinition(Entity.StatusNameProperty);
            query.Selection.Add(nameAttribute);
            query.Selection.Add(statusAttribute);
            query.Selection.Add(statusNameAttribute);                

            QueryResult result = services.Retrieve(query);

            Asset defect = result.Assets[0];
            string name = defect.GetAttribute(nameAttribute).Value.ToString();
            string status = defect.GetAttribute(statusAttribute).Value.ToString();
            string statusName = defect.GetAttribute(statusNameAttribute).Value.ToString();

            defect.SetAttributeValue(statusAttribute, "StoryStatus:135");
            services.Save(defect);
            string newStatusName = defect.GetAttribute(statusAttribute).Value.ToString();

            Console.WriteLine(string.Format("{0} - {1} ", statusName, newStatusName));
        }

        [TestMethod]
        public void QueryForIssueFromExternalId() { 
            string PrimaryWorkitemType = "PrimaryWorkitem";
            string externalSystemName = Enviornment.instance.GetVersionOneSource;
            string externalFieldName = Enviornment.instance.GetVersionOneReference;
            string externalId = "VOID-41";

            queryBuilder.AddProperty (Entity.NameProperty,VersionOneProcessor.PrimaryWorkitemType, false);
            queryBuilder.AddProperty( Workitem.DescriptionProperty,VersionOneProcessor.PrimaryWorkitemType, false);
            queryBuilder.AddProperty( Entity.StatusProperty,VersionOneProcessor.PrimaryWorkitemType, false);
            queryBuilder.AddProperty( Workitem.ReferenceProperty,VersionOneProcessor.PrimaryWorkitemType, false);
            queryBuilder.AddProperty( Workitem.AssetTypeProperty,VersionOneProcessor.PrimaryWorkitemType, false);

             var filters = new List<IFilter> {
                    Filter.Closed(false),
                    Filter.OfTypes(VersionOneProcessor.StoryType, VersionOneProcessor.DefectType),
                    Filter.Equal(Entity.SourceNameProperty, externalSystemName),
                    Filter.Equal(externalFieldName, externalId),
                };

            var filter = GroupFilter.And(filters.ToArray());


            var result = queryBuilder.Query(PrimaryWorkitemType, filter);

            Console.WriteLine(string.Format("Returned count: {0}", result.Count));
            Assert.IsTrue(result.Count == 1);
        }

        [TestMethod]
        public void UpdateIssueFromExternalId() {
            //member
            string PrimaryWorkitemType = "PrimaryWorkitem";
            //given
            string externalSystemName = Enviornment.instance.GetVersionOneSource;
            string externalFieldName = Enviornment.instance.GetVersionOneReference;
            string externalId = "VOID-41";

            //when
            var defectType = metaModel.GetAssetType(PrimaryWorkitemType);

            IAttributeDefinition externalFieldAttribute = defectType.GetAttributeDefinition(externalFieldName);
            IAttributeDefinition statusAttribute = defectType.GetAttributeDefinition(Entity.StatusProperty);
            
            var filters = new List<IFilter> {
                    Filter.Closed(false),
                    Filter.OfTypes(VersionOneProcessor.StoryType, VersionOneProcessor.DefectType),
                    Filter.Equal(Entity.SourceNameProperty, externalSystemName),
                    Filter.Equal(externalFieldName, externalId),
                };

            var filter = GroupFilter.And(filters.ToArray());
            
            var workitemType = metaModel.GetAssetType(PrimaryWorkitemType);
            var terms = filter.GetFilter(workitemType);


            var query = new Query(defectType) { Filter = terms };
            query.Selection.Add(statusAttribute);
            query.Selection.Add(externalFieldAttribute);

            //then
            var queryResult = services.Retrieve(query);
            Assert.IsTrue(queryResult.Assets.Count >0 );

            var workitem = queryResult.Assets.First();
            workitem.SetAttributeValue(statusAttribute, "StoryStatus:135");
            services.Save(workitem);
        }

    }
}
