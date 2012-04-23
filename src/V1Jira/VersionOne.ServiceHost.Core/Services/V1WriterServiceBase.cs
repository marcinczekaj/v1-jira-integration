/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.Xml;
using Ninject;
using VersionOne.SDK.APIClient;
using VersionOne.ServiceHost.Eventing;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.Core.Services {
    public abstract class V1WriterServiceBase : IHostedService {
        private ICentral central;
        protected XmlElement Config;
        protected IEventManager EventManager;
        protected ILogger Logger;

        protected virtual ICentral Central {
            get {
                if(central == null) {
                    try {
                        var c = new V1Central(Config["Settings"]);
                        c.Validate();
                        central = c;
                    } catch(Exception ex) {
                        Logger.Log("Failed to connect to VersionOne server", ex);
                        throw;
                    }
                }

                return central;
            }
        }

        public virtual void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
            Config = config;
            EventManager = eventManager;
            Logger = new Logger(eventManager);
        }

        public void Start() {
            // TODO move subscriptions to timer events, etc. here
        }

        protected abstract IEnumerable<NeededAssetType> NeededAssetTypes { get; }

        protected void VerifyMeta() {
            try {
                VerifyNeededMeta(NeededAssetTypes);
                VerifyRuntimeMeta();
            } catch(MetaException ex) {
                throw new ApplicationException("Necessary meta is not present in this VersionOne system", ex);
            }
        }

        protected virtual void VerifyRuntimeMeta() {
        }

        protected struct NeededAssetType {
            public readonly string Name;
            public readonly string[] AttributeDefinitionNames;

            public NeededAssetType(string name, string[] attributedefinitionnames) {
                Name = name;
                AttributeDefinitionNames = attributedefinitionnames;
            }
        }

        protected void VerifyNeededMeta(IEnumerable<NeededAssetType> neededassettypes) {
            foreach(var neededAssetType in neededassettypes) {
                var assettype = Central.MetaModel.GetAssetType(neededAssetType.Name);

                foreach(var attributeDefinitionName in neededAssetType.AttributeDefinitionNames) {
                    var attribdef = assettype.GetAttributeDefinition(attributeDefinitionName);
                }
            }
        }

        #region Meta wrappers

        protected IAssetType RequestType {
            get { return Central.MetaModel.GetAssetType("Request"); }
        }

        protected IAssetType DefectType {
            get { return Central.MetaModel.GetAssetType("Defect"); }
        }

        protected IAssetType StoryType {
            get { return Central.MetaModel.GetAssetType("Story"); }
        }

        protected IAssetType ReleaseVersionType {
            get { return Central.MetaModel.GetAssetType("StoryCategory"); }
        }

        protected IAssetType LinkType {
            get { return Central.MetaModel.GetAssetType("Link"); }
        }

        protected IAssetType NoteType {
            get { return Central.MetaModel.GetAssetType("Note"); }
        }

        protected IAttributeDefinition DefectName {
            get { return DefectType.GetAttributeDefinition("Name"); }
        }

        protected IAttributeDefinition DefectDescription {
            get { return DefectType.GetAttributeDefinition("Description"); }
        }

        protected IAttributeDefinition DefectOwners {
            get { return DefectType.GetAttributeDefinition("Owners"); }
        }

        protected IAttributeDefinition DefectScope {
            get { return DefectType.GetAttributeDefinition("Scope"); }
        }

        protected IAttributeDefinition DefectAssetState {
            get { return RequestType.GetAttributeDefinition("AssetState"); }
        }

        protected IAttributeDefinition RequestCompanyName {
            get { return RequestType.GetAttributeDefinition("Name"); }
        }

        protected IAttributeDefinition RequestNumber {
            get { return RequestType.GetAttributeDefinition("Number"); }
        }

        protected IAttributeDefinition RequestSuggestedInstance {
            get { return RequestType.GetAttributeDefinition("Reference"); }
        }

        protected IAttributeDefinition RequestMethodology {
            get { return RequestType.GetAttributeDefinition("Source"); }
        }

        protected IAttributeDefinition RequestMethodologyName {
            get { return RequestType.GetAttributeDefinition("Source.Name"); }
        }

        protected IAttributeDefinition RequestCommunityEdition {
            get { return RequestType.GetAttributeDefinition("Custom_CommunityEdition"); }
        }

        protected IAttributeDefinition RequestAssetState {
            get { return RequestType.GetAttributeDefinition("AssetState"); }
        }

        protected IAttributeDefinition RequestCreateDate {
            get { return RequestType.GetAttributeDefinition("CreateDate"); }
        }

        protected IAttributeDefinition RequestCreatedBy {
            get { return RequestType.GetAttributeDefinition("CreatedBy"); }
        }

        protected IOperation RequestInactivate {
            get { return Central.MetaModel.GetOperation("Request.Inactivate"); }
        }

        protected IAttributeDefinition StoryName {
            get { return StoryType.GetAttributeDefinition("Name"); }
        }

        protected IAttributeDefinition StoryActualInstance {
            get { return StoryType.GetAttributeDefinition("Reference"); }
        }

        protected IAttributeDefinition StoryRequests {
            get { return StoryType.GetAttributeDefinition("Requests"); }
        }

        protected IAttributeDefinition StoryReleaseVersion {
            get { return StoryType.GetAttributeDefinition("Category"); }
        }

        protected IAttributeDefinition StoryMethodology {
            get { return StoryType.GetAttributeDefinition("Source"); }
        }

        protected IAttributeDefinition StoryCommunitySite {
            get { return StoryType.GetAttributeDefinition("Custom_CommunitySite"); }
        }

        protected IAttributeDefinition StoryScope {
            get { return StoryType.GetAttributeDefinition("Scope"); }
        }

        protected IAttributeDefinition StoryOwners {
            get { return StoryType.GetAttributeDefinition("Owners"); }
        }

        protected IAttributeDefinition ReleaseVersionName {
            get { return ReleaseVersionType.GetAttributeDefinition("Name"); }
        }

        protected IAttributeDefinition LinkAsset {
            get { return LinkType.GetAttributeDefinition("Asset"); }
        }

        protected IAttributeDefinition LinkOnMenu {
            get { return LinkType.GetAttributeDefinition("OnMenu"); }
        }

        protected IAttributeDefinition LinkUrl {
            get { return LinkType.GetAttributeDefinition("URL"); }
        }

        protected IAttributeDefinition LinkName {
            get { return LinkType.GetAttributeDefinition("Name"); }
        }

        protected IAttributeDefinition NoteName {
            get { return NoteType.GetAttributeDefinition("Name"); }
        }

        protected IAttributeDefinition NoteAsset {
            get { return NoteType.GetAttributeDefinition("Asset"); }
        }

        protected IAttributeDefinition NotePersonal {
            get { return NoteType.GetAttributeDefinition("Personal"); }
        }

        protected IAttributeDefinition NoteContent {
            get { return NoteType.GetAttributeDefinition("Content"); }
        }

        #endregion
    }
}