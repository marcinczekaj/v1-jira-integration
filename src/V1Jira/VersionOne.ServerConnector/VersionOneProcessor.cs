/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;
using VersionOne.ServiceHost.Core.Logging;
using System.Xml;
using System.Collections;

namespace VersionOne.ServerConnector {
    // TODO extract hardcoded strings to constants
    // TODO this one is getting huge - it should be split
    // TODO change attribute to property in field names and move them to entity classes
    public class VersionOneProcessor : IVersionOneProcessor {
        public const string FeatureGroupType = "Theme";
        public const string StoryType = "Story";
        public const string DefectType = "Defect";
        public const string PrimaryWorkitemType = "PrimaryWorkitem";
        public const string MemberType = "Member";
        public const string LinkType = "Link";
        public const string AttributeDefinitionType = "AttributeDefinition";
        
        public const string OwnersAttribute = "Owners";
        public const string AssetStateAttribute = "AssetState";
        public const string AssetTypeAttribute = "AssetType";
        public const string EnvironmentAttribute = "Environment";

        public const string DeleteOperation = "Delete";
        public const string InactivateOperation = "Inactivate";
        public const string ReactivateOperation = "Reactivate";

        public const string WorkitemPriorityType = "WorkitemPriority";
        public const string WorkitemSourceType = "StorySource";
        public const string WorkitemStatusType = "StoryStatus";
        public const string DefectSeverityType = "Custom_SeverityLevel";
       
        private const string IdAttribute = "ID";
        private const string AssetAttribute = "Asset";

        private IServices services;
        private IMetaModel metaModel;
        private readonly ILogger logger; 
        private readonly XmlElement configuration;

        private IQueryBuilder queryBuilder;

        private IDictionary<string, PropertyValues> ListPropertyValues {
            get { return queryBuilder.ListPropertyValues; }
        }

        [Inject]
        public VersionOneProcessor(XmlElement config, ILogger logger) {
            configuration = config;
            this.logger = logger;

            queryBuilder = new QueryBuilder();
        }

        private void Connect() {
            var connector = new V1Central(configuration);
            connector.Validate();
            services = connector.Services;
            metaModel = connector.MetaModel;

            queryBuilder.Setup(services, metaModel, connector.Loc);
        }

        protected internal void Connect(IServices testServices, IMetaModel testMetaData, IQueryBuilder testQueryBuilder) {
            services = testServices;
            metaModel = testMetaData;
            queryBuilder = testQueryBuilder;
        }

        public bool ValidateConnection() {
            try {
                Connect();                
            } catch(Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, "Connection is not valid. " + ex.Message);
                return false;
            }

            return true;
        }

        // TODO use GetPrimaryWorkitems()
        public IList<PrimaryWorkitem> GetWorkitemsByProjectId(string projectId) {
            var projectOid = Oid.FromToken(projectId, metaModel);
            var filter = GroupFilter.And(Filter.Closed(false), Filter.Equal(Entity.ScopeProperty, projectOid));

            return queryBuilder
                .Query(PrimaryWorkitemType, filter)
                .Select(asset => new PrimaryWorkitem(asset, ListPropertyValues, queryBuilder.TypeResolver)).ToList();
        }

        // TODO use GetPrimaryWorkitems()
        public IList<PrimaryWorkitem> GetClosedWorkitemsByProjectId(string projectId) {
            var projectOid = Oid.FromToken(projectId, metaModel);
            var filter = GroupFilter.And(Filter.Closed(true), Filter.Equal(Entity.ScopeProperty, projectOid));

            return queryBuilder
                .Query(PrimaryWorkitemType, filter)
                .Select(asset => new PrimaryWorkitem(asset, ListPropertyValues, queryBuilder.TypeResolver)).ToList();
        }

        // TODO make this Story-agnostic. In case of criteria based ex. on Story-only custom fields current filter approach won't let an easy solution.
        public IList<FeatureGroup> GetFeatureGroups(IFilter filter, IFilter childrenFilter) {
            var featureGroupType = metaModel.GetAssetType(FeatureGroupType);
            var ownersDefinition = featureGroupType.GetAttributeDefinition(OwnersAttribute);

            return queryBuilder.Query(FeatureGroupType, filter)
                .Select(asset => new FeatureGroup(
                    asset, ListPropertyValues, 
                    GetWorkitems(StoryType, GroupFilter.And(Filter.Equal(Entity.ParentAndUpProperty, asset.Oid.Momentless.Token.ToString()), childrenFilter)), 
                    GetMembersByIds(asset.GetAttribute(ownersDefinition).ValuesList),
                    queryBuilder.TypeResolver))
                .ToList();
        }

        // TODO avoid ancient non generic collections
        private IList<Member> GetMembersByIds(IList oids) {
            if (oids.Count == 0) {
                return new List<Member>();
            }

            var memberType = metaModel.GetAssetType(MemberType);

            var terms = new OrFilterTerm();
            
            foreach(var oid in oids) {
                var term = new FilterTerm(memberType.GetAttributeDefinition(IdAttribute));
                term.Equal(oid);
                terms.Or(term);
            }
            
            var members = queryBuilder.Query(MemberType, terms).Select(asset => new Member(asset)).ToList();
            return members;
        }

        public void SaveWorkitems(ICollection<Workitem> workitems) {
            if(workitems == null || workitems.Count == 0) {
                return;
            }

            foreach(var workitem in workitems) {
                SaveWorkitem(workitem);
            }
        }

        public void SaveWorkitem(Workitem workitem) {
                try {
                    services.Save(workitem.Asset);
                } catch(V1Exception ex) {
                    logger.Log(LogMessage.SeverityType.Error, string.Format(queryBuilder.Localize(GetMessageFromException(ex)) + " '{0}' {2} ({1}).", workitem.Name, workitem.Number, workitem.TypeName));
                } catch (Exception ex) {
                    logger.Log(LogMessage.SeverityType.Error, "Internal error: " + ex.Message);
                }
        }

        private static string GetMessageFromException(V1Exception exception) {
            var message = exception.Message;

            return message.Split(':')[0];
        }

        public void CloseWorkitem(PrimaryWorkitem workitem) {
            try {
                var closeOperation = workitem.Asset.AssetType.GetOperation(InactivateOperation);
                services.ExecuteOperation(closeOperation, workitem.Asset.Oid);
            } catch (V1Exception ex) {
                throw new VersionOneException(queryBuilder.Localize(ex.Message));
            } catch(Exception ex) {
                throw new VersionOneException(ex.Message);
            }
        }

        public IList<ValueId> GetWorkitemStatuses() {
            try {
                return queryBuilder.QueryPropertyValues(WorkitemStatusType).ToList();
            } catch (V1Exception ex) {
                throw new VersionOneException(queryBuilder.Localize(ex.Message));
            } catch(Exception ex) {
                throw new VersionOneException(ex.Message);
            }
        }

        public ValueId CreateWorkitemStatus(string statusName) {
            try {
                var primaryWorkitemStatusType = metaModel.GetAssetType(WorkitemStatusType);
                var status = new Asset(primaryWorkitemStatusType);
                status.SetAttributeValue(primaryWorkitemStatusType.NameAttribute, statusName);
                services.Save(status);

                return new ValueId(status.Oid.Momentless, statusName);
            } catch (V1Exception ex) {
                throw new VersionOneException(queryBuilder.Localize(ex.Message));
            } catch(Exception ex) {
                throw new VersionOneException(ex.Message);
            }
        }

        // TODO refactor
        public void UpdateProject(string projectId, Link link) {
            try {
                if(link != null && !string.IsNullOrEmpty(link.Url)) {
                    var projectAsset = GetProjectById(projectId);
                    AddLinkToAsset(projectAsset, link);
                }
            } catch (V1Exception ex) {
                throw new VersionOneException(queryBuilder.Localize(ex.Message));
            } catch(Exception ex) {
                throw new VersionOneException(ex.Message);
            }
        }

        public string GetWorkitemLink(Workitem workitem) {
            return string.Format("{0}assetdetail.v1?oid={1}", configuration["ApplicationUrl"].InnerText, workitem.Id);
        }

        public PropertyValues GetAvailableListValues(string typeToken, string fieldName) {
            try {
                var type = metaModel.GetAssetType(typeToken);
                var attributeDefinition = type.GetAttributeDefinition(fieldName);
                
                if(attributeDefinition.AttributeType != AttributeType.Relation) {
                    throw new VersionOneException("Not a Relation field");
                }

                var listTypeToken = attributeDefinition.RelatedAsset.Token;
                return queryBuilder.QueryPropertyValues(listTypeToken);
            } catch(MetaException) {
                throw new VersionOneException("Invalid type or field name");
            }
        }

        public IList<ValueId> GetWorkitemPriorities() {
            try {
                return queryBuilder.QueryPropertyValues(WorkitemPriorityType).ToList();
            } catch (V1Exception ex) {
                throw new VersionOneException(queryBuilder.Localize(ex.Message));
            } catch(Exception ex) {
                throw new VersionOneException(ex.Message);
            }
        }

        // TODO get rid of it
        public bool ProjectExists(string projectId) {
            return GetProjectById(projectId) != null;
        }

        public bool AttributeExists(string typeName, string attributeName) {
            try {
                var type = metaModel.GetAssetType(typeName);
                var attributeDefinition = type.GetAttributeDefinition(attributeName);
                return attributeDefinition != null;
            } catch(MetaException) {
                return false;
            }
        }

        public void AddProperty(string attr, string prefix, bool isList) {
            queryBuilder.AddProperty(attr, prefix, isList);
        }

        public void AddListProperty(string fieldName, string typeToken) {
            queryBuilder.AddListProperty(fieldName, typeToken);
        }

        public void AddOptionalProperty(string attr, string prefix) {
            if (!string.IsNullOrEmpty(attr)) {
                queryBuilder.AddOptionalProperty(attr, prefix);
            }
        }

        // TODO use filters
        private Asset GetProjectById(string projectId) {
            var scopeType = metaModel.GetAssetType(Entity.ScopeProperty);
            var scopeState = scopeType.GetAttributeDefinition(AssetStateAttribute);

            var scopeStateTerm = new FilterTerm(scopeState);
            scopeStateTerm.NotEqual(AssetState.Closed);

            var query = new Query(Oid.FromToken(projectId, metaModel)) {Filter = scopeStateTerm};
            var result = services.Retrieve(query);

            return result.Assets.FirstOrDefault();
        }


        private List<Asset> GetAssetLinks(Oid assetOid, IFilter filter) {
            var fullFilter = GroupFilter.And(filter, Filter.Equal(AssetAttribute, assetOid.Momentless));

            return queryBuilder.Query(LinkType, fullFilter);
        }

        public List<Link> GetWorkitemLinks(Workitem workitem, IFilter filter) {
            return GetAssetLinks(Oid.FromToken(workitem.Id, metaModel), filter).Select(x => new Link(x)).ToList();
        }

        private void AddLinkToAsset(Asset asset, Link link) {
            if (asset == null) {
                return;
            }

            var linkType = metaModel.GetAssetType(LinkType);

            var existedLinks = GetAssetLinks(asset.Oid, Filter.Equal(Link.UrlProperty, link.Url));

            if(existedLinks.Count > 0) {
                logger.Log(LogMessage.SeverityType.Debug, string.Format("No need to create link - it already exists."));
                return;
            }

            logger.Log(LogMessage.SeverityType.Info, string.Format("Creating new link with title {0} for asset {1}", link.Title, asset.Oid));

            var linkAsset = services.New(linkType, asset.Oid.Momentless);
            linkAsset.SetAttributeValue(linkType.GetAttributeDefinition(Entity.NameProperty), link.Title);
            linkAsset.SetAttributeValue(linkType.GetAttributeDefinition(Link.OnMenuProperty), link.OnMenu);
            linkAsset.SetAttributeValue(linkType.GetAttributeDefinition(Link.UrlProperty), link.Url);

            services.Save(linkAsset);
            logger.Log(LogMessage.SeverityType.Info, string.Format("{0} link saved", link.Title));
        }

        public void AddLinkToWorkitem(Workitem workitem, Link link) {            
            try {
                if (link != null && !string.IsNullOrEmpty(link.Url)) {
                    AddLinkToAsset(workitem.Asset, link);
                }
            } catch (V1Exception ex) {
                throw new VersionOneException(queryBuilder.Localize(ex.Message));
            } catch (Exception ex) {
                throw new VersionOneException(ex.Message);
            }
        }

        public IList<Workitem> GetPrimaryWorkitems(IFilter filter) {
            return GetWorkitems(PrimaryWorkitemType, filter);
        }

        public IList<Workitem> GetWorkitems(string type, IFilter filter) {
            var workitemType = metaModel.GetAssetType(type);
            var terms = filter.GetFilter(workitemType);

            return queryBuilder.Query(type, terms).Select(asset => Workitem.Create(asset, ListPropertyValues, queryBuilder.TypeResolver)).ToList();
        }

        public Workitem GetPrimaryWorkitemByNumber(string number)
        {
            Workitem result = null;

            if (!string.IsNullOrEmpty(number))
            {
                var filter = Filter.Equal(Workitem.NumberProperty, number);
                result = GetPrimaryWorkitems(filter).FirstOrDefault();  
            }

            return result;
        }

        //TODO refactor
        public Workitem CreateWorkitem(string assetType, string title, string description, string projectId, string projectName, string externalFieldName, string externalId, string externalSystemName,
            string priorityId, string owners, string urlTitle, string url, string environment, string foundBy, string versionAffected, string buildNumber, string severityLevel)
        {
            if(string.IsNullOrEmpty(title)) {
                throw new ArgumentException("Empty title");
            }

            Oid projectOid = Oid.Null;

            if(!string.IsNullOrEmpty(projectId)) {
                projectOid = Oid.FromToken(projectId, metaModel);
            } else if(!string.IsNullOrEmpty(projectName)) {
                var project = GetProjectByName(projectName);
                projectOid = project != null ? project.Oid.Momentless : Oid.Null;
            } 
     
            if(projectOid == Oid.Null) {
                throw new ArgumentException("Can't find proper project");
            }

            var sourceValues = queryBuilder.QueryPropertyValues(WorkitemSourceType);
            var source = sourceValues.Where(item => string.Equals(item.Name, externalSystemName)).FirstOrDefault();

            if(source == null) {
                throw new ArgumentException("Can't find proper source");
            }

            var sourceOid = source.Oid.Momentless;
            var workitemType = metaModel.GetAssetType(assetType);
            var newWorkitem = services.New(workitemType, Oid.Null);

            newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("Name"), title);
            newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("Scope"), projectOid);
            newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("Description"), description);
            newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("Source"), sourceOid);
            newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition(externalFieldName), externalId);

            foreach(var ownerOid in GetOwnerOids(owners)) {
                newWorkitem.AddAttributeValue(workitemType.GetAttributeDefinition("Owners"), ownerOid);
            }

            if(!string.IsNullOrEmpty(priorityId)) {
                newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("Priority"), Oid.FromToken(priorityId, metaModel));
            }

            switch (workitemType.Token) {
                case VersionOneProcessor.StoryType:
                    if (!string.IsNullOrEmpty(buildNumber)) {
                        newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("LastVersion"), buildNumber);
                    }
                    break;
                case VersionOneProcessor.DefectType:
                    newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("Environment"), environment);

                    if (!string.IsNullOrEmpty(foundBy))
                    {
                        newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("FoundBy"), foundBy);
                    }

                    if (!string.IsNullOrEmpty(versionAffected))
                    {
                        newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("VersionAffected"), versionAffected);
                    }

                    if (!string.IsNullOrEmpty(buildNumber))
                    {
                        newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("FoundInBuild"), buildNumber);
                    }

                    if (!string.IsNullOrEmpty(severityLevel))
                    {
                        newWorkitem.SetAttributeValue(workitemType.GetAttributeDefinition("Custom_SeverityLevel"), severityLevel);
                    }
                    break;
            }


            services.Save(newWorkitem);

            if(!string.IsNullOrEmpty(url)) {
                var linkType = metaModel.GetAssetType("Link");
                var newlink = services.New(linkType, newWorkitem.Oid.Momentless);
                newlink.SetAttributeValue(linkType.GetAttributeDefinition("Name"), !string.IsNullOrEmpty(urlTitle) ? urlTitle : url);
                newlink.SetAttributeValue(linkType.GetAttributeDefinition("URL"), url);
                newlink.SetAttributeValue(linkType.GetAttributeDefinition("OnMenu"), true);
                services.Save(newlink);
            }

            //TODO refactor
            //NOTE Save doesn't return all the needed data, therefore we need another query
            return GetWorkitems(newWorkitem.AssetType.Token, Filter.Equal("ID", newWorkitem.Oid.Momentless.Token)).FirstOrDefault();
        }

        //TODO refactor
        public Asset GetProjectByName(string projectName)
        {
            var scopeType = metaModel.GetAssetType(Entity.ScopeProperty);
            var scopeName = scopeType.GetAttributeDefinition(Entity.NameProperty);

            var scopeNameTerm = new FilterTerm(scopeName);
            scopeNameTerm.Equal(projectName);

            var scopeState = scopeType.GetAttributeDefinition(AssetStateAttribute);
            var scopeStateTerm = new FilterTerm(scopeState);
            scopeStateTerm.NotEqual(AssetState.Closed);

            var query = new Query(scopeType);
            query.Selection.Add(scopeName);
            var terms  = new AndFilterTerm(scopeNameTerm, scopeStateTerm);

            var result = queryBuilder.Query(Entity.ScopeProperty, terms);

            return result.FirstOrDefault();
        }

        //TODO refactor
        private Asset GetRootProject() {
            var scopeType = metaModel.GetAssetType(Entity.ScopeProperty);
            var scopeName = scopeType.GetAttributeDefinition(Entity.NameProperty);

            var scopeState = scopeType.GetAttributeDefinition(AssetStateAttribute);
            var scopeStateTerm = new FilterTerm(scopeState);
            scopeStateTerm.NotEqual(AssetState.Closed);

            var scopeQuery = new Query(scopeType, scopeType.GetAttributeDefinition(Entity.ParentProperty)) { Filter = scopeStateTerm };
            scopeQuery.Selection.Add(scopeName);

            var nameQueryResult = services.Retrieve(scopeQuery);

            return nameQueryResult.Assets.FirstOrDefault();
        }

        /// <summary>
        /// Attempts to match owners of the workitem in the external system to users in VersionOne.
        /// </summary>
        /// <param name="ownerNames">Comma seperated list of usernames.</param>
        /// <returns>Oids of matching users in VersionOne.</returns>
        //TODO refactor
        private IEnumerable<Oid> GetOwnerOids(string ownerNames) {
            var result = new List<Oid>();

            if(!string.IsNullOrEmpty(ownerNames)) {
                var memberType = metaModel.GetAssetType("Member");

                foreach(var ownerName in ownerNames.Split(',')) {
                    if (!string.IsNullOrEmpty(ownerName)) {
                        var ownerQuery = new Query(memberType);
                        var nameSelection = new AttributeSelection(memberType.GetAttributeDefinition("Username"));
                        ownerQuery.Find = new QueryFind(ownerName, nameSelection);
                        var matches = services.Retrieve(ownerQuery).Assets;
                        result.AddRange(matches.Select(owner => owner.Oid));
                    }
                }
            }

            return result.ToArray();
        }

        public IDictionary<string,string> GetAvailableStatuses() {

            var assetType = metaModel.GetAssetType(WorkitemStatusType);
            var nameDef = assetType.GetAttributeDefinition(Entity.NameProperty);
            var query = new Query(assetType);
            query.Selection.Add(nameDef);

            QueryResult result = services.Retrieve(query);

            IDictionary<string, string> mapping = new Dictionary<string, string>();
            foreach (var status in result.Assets) {
                string statusName = status.GetAttribute(nameDef).Value.ToString();
                mapping.Add(statusName, status.Oid.Token);
            }

            return mapping;
        }

        public bool UpdateWorkitem(string externalFieldName, string externalId, string externalSystemName, string statusId, UpdateResult result) {
            
            var defectType = metaModel.GetAssetType(PrimaryWorkitemType);
            IAttributeDefinition externalFieldAttribute = defectType.GetAttributeDefinition(externalFieldName);
            IAttributeDefinition statusAttribute = defectType.GetAttributeDefinition(Entity.StatusProperty);
            IAttributeDefinition modificationTimeAttrubute = defectType.GetAttributeDefinition(Entity.ChangeDateUtcProperty);
            IAttributeDefinition numberAttribute = defectType.GetAttributeDefinition(PrimaryWorkitem.NumberProperty);

            var filters = new List<IFilter> {
                    Filter.Closed(false),
                    Filter.OfTypes(VersionOneProcessor.StoryType, VersionOneProcessor.DefectType),
                    Filter.Equal(Entity.SourceNameProperty, externalSystemName),
                    Filter.Equal(externalFieldName, externalId),
                };

            var filter = GroupFilter.And(filters.ToArray());
            var filterTerms = filter.GetFilter(defectType);

            var query = new Query(defectType) { Filter = filterTerms };
            query.Selection.Add(statusAttribute);
            query.Selection.Add(externalFieldAttribute);
            query.Selection.Add(modificationTimeAttrubute);
            query.Selection.Add(numberAttribute);
            
            try {

                var queryResult = services.Retrieve(query);
                if (queryResult.Assets.Count <= 0) {
                    logger.Log(LogMessage.SeverityType.Error, string.Format("Failed to update {0} - issue not found", externalId ));
                    return false;
                }

                var workitem = queryResult.Assets.First();

                string existingStatusId = workitem.GetAttribute(statusAttribute).Value.ToString();
                if (existingStatusId.Equals(statusId) == false) {
                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Updating issue {0} with modification time: {1}", externalId, workitem.GetAttribute(modificationTimeAttrubute).Value));
                    workitem.SetAttributeValue(statusAttribute, statusId);
                    services.Save(workitem);

                    result.number = workitem.GetAttribute(numberAttribute).Value.ToString();
                    result.modificationTime =  (DateTime)workitem.GetAttribute(modificationTimeAttrubute).Value;
                    return true;
                } else {
                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Skipping update of {0} in VersionOne since status is already set to {1}", externalId, statusId));
                }
            }
            catch (V1Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, string.Format(GetMessageFromException(ex)));
            }
            catch (Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, "Internal error: " + ex.Message);
            }

            return false;
        }

    }
}