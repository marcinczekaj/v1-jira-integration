/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using Ninject;
using VersionOne.Profile;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;
using System.Collections.Generic;


namespace VersionOne.ServiceHost.WorkitemServices {
    public class WorkitemWriterHostedService : IHostedService {
        private XmlElement configElement;
        private IEventManager eventManager;
        private string externalIdFieldName;
        private ExternalWorkitemQuerier externalWorkitemsQuerier;
        private ILogger logger;
        private IProfile profile;
        private IVersionOneProcessor v1Processor;
        private WorkitemWriter workitemWriter;
        private LastValidSynchronisationProfile lastValidSynchronisationProfile;


        public void Initialize(XmlElement configuration, IEventManager manager, IProfile profile) {
            eventManager = manager;
            this.profile = profile;
            logger = new Logger(eventManager);
            configElement = configuration;
            this.lastValidSynchronisationProfile = new LastValidSynchronisationProfile(profile);

            try {
                externalIdFieldName = configuration["ExternalIdFieldName"].InnerText;
            } catch(Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, "Error during reading settings from configuration file.", ex);
                throw;
            }

            InitializeComponents();

            eventManager.Subscribe(typeof(Defect), ProcessWorkitem);
            eventManager.Subscribe(typeof(Story), ProcessWorkitem);
            eventManager.Subscribe(typeof(ClosedWorkitemsSource), GetClosedExternalWorkitems);
            eventManager.Subscribe(typeof(CreatedWorkitemsSource), GetCreatedWorkitems);
            eventManager.Subscribe(typeof(WorkitemsToUpdate), UpdateOpenWorkitems);
            eventManager.Subscribe(typeof(IssueCreatedResult), UpdateNewWorkitem);
            
        }

        public void Start() {
            // TODO move subscriptions to timer events, etc. here
        }

        private void InitializeComponents() {
            v1Processor = new VersionOneProcessor(configElement["Settings"], logger);
            ComponentRepository.Instance.Register(v1Processor);
            ComponentRepository.Instance.Register(logger);

            workitemWriter = new WorkitemWriter(externalIdFieldName);
            externalWorkitemsQuerier = new ExternalWorkitemQuerier();

            v1Processor.AddProperty(Entity.NameProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.DescriptionProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.NumberProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            //v1Processor.AddProperty(ServerConnector.Entities.Workitem.OwnersProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.OwnersUsernamesProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            v1Processor.AddProperty(externalIdFieldName, VersionOneProcessor.PrimaryWorkitemType, false);
            v1Processor.AddProperty(Entity.ChangeDateUtcProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            v1Processor.AddProperty(Entity.ScopeProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            v1Processor.AddProperty(Entity.ScopeNameProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(Entity.CreateDateUtcProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            v1Processor.AddProperty(Entity.CreatedByUsernameProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            v1Processor.AddProperty(Entity.StatusNameProperty, VersionOneProcessor.PrimaryWorkitemType, false);
            v1Processor.AddProperty(Entity.StatusProperty, VersionOneProcessor.PrimaryWorkitemType, true);
            v1Processor.AddProperty(Entity.SourceProperty, VersionOneProcessor.PrimaryWorkitemType, true);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.PriorityProperty, VersionOneProcessor.PrimaryWorkitemType, true); // to get options list
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.PriorityProperty, VersionOneProcessor.PrimaryWorkitemType, false);

            v1Processor.AddProperty(Entity.NameProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.DescriptionProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.NumberProperty, VersionOneProcessor.DefectType, false);
            //v1Processor.AddProperty(ServerConnector.Entities.Workitem.OwnersProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.OwnersUsernamesProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(externalIdFieldName, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(Entity.ChangeDateUtcProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(Entity.ScopeProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(Entity.ScopeNameProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(Entity.CreateDateUtcProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(Entity.CreatedByUsernameProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(Entity.StatusProperty, VersionOneProcessor.DefectType, true);
            v1Processor.AddProperty(Entity.SourceProperty, VersionOneProcessor.DefectType, true);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.PriorityProperty, VersionOneProcessor.DefectType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.FoundInBuildProperty, VersionOneProcessor.DefectType, false); // defect-only
            v1Processor.AddProperty(VersionOneProcessor.EnvironmentAttribute, VersionOneProcessor.DefectType, false); // defect-only
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.SeverityLevelProperty, VersionOneProcessor.DefectType, true);


            v1Processor.AddProperty(Entity.NameProperty, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.DescriptionProperty, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.NumberProperty, VersionOneProcessor.StoryType, false);
            //v1Processor.AddProperty(ServerConnector.Entities.Workitem.OwnersProperty, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.OwnersUsernamesProperty, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(externalIdFieldName, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(Entity.ChangeDateUtcProperty, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(Entity.ScopeProperty, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(Entity.ScopeNameProperty, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(ServerConnector.Entities.Workitem.PriorityProperty, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(Entity.CreateDateUtcProperty, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(Entity.CreatedByUsernameProperty, VersionOneProcessor.StoryType, false);
            v1Processor.AddProperty(Entity.StatusProperty, VersionOneProcessor.StoryType, true);
            v1Processor.AddProperty(Entity.SourceProperty, VersionOneProcessor.StoryType, true);

            v1Processor.AddProperty(Member.NameProperty, VersionOneProcessor.MemberType, false);
            v1Processor.AddProperty(Member.UsernameProperty, VersionOneProcessor.MemberType, false);

            ComponentRepository.Instance.Register(workitemWriter);
            ComponentRepository.Instance.Register(externalWorkitemsQuerier);
        }

        private void GetClosedExternalWorkitems(object pubobj) {
            var sourceToCheckFor = pubobj as ClosedWorkitemsSource;

            string LastClosedWorkitemId = lastValidSynchronisationProfile.LastClosedWorkitemId;
            DateTime LastCheckForClosedWorkitems = lastValidSynchronisationProfile.LastCheckForClosedWorkitems;

            if(sourceToCheckFor == null || string.IsNullOrEmpty(sourceToCheckFor.SourceValue)) {
                return;
            }

            logger.Log(LogMessage.SeverityType.Info,
                string.Format("Checking V1 workitems closed since {0} was closed at {1}.", LastClosedWorkitemId, LastCheckForClosedWorkitems));
            var closedWorkitems = externalWorkitemsQuerier.GetWorkitemsClosedSince(LastCheckForClosedWorkitems, sourceToCheckFor.SourceValue, externalIdFieldName,
                LastClosedWorkitemId);

            logger.Log(LogMessage.SeverityType.Info,
                string.Format("Found {0} workitems closed since {1} where Source is '{2}'.", closedWorkitems.Count, LastCheckForClosedWorkitems,
                    sourceToCheckFor.SourceValue));

            if(closedWorkitems.Count > 0) {
                logger.Log(LogMessage.SeverityType.Info, string.Format("Closing issues in {0}.", sourceToCheckFor.SourceValue));
                eventManager.Publish(closedWorkitems);

                foreach(var result in closedWorkitems.Where(result => result.ChangesProcessed)) {
                    logger.Log(LogMessage.SeverityType.Info, string.Format("Issue {0} closed successfully.", result.ExternalId));
                }
            }

            lastValidSynchronisationProfile.LastCheckForClosedWorkitems = closedWorkitems.QueryTimeStamp;
            lastValidSynchronisationProfile.LastClosedWorkitemId = closedWorkitems.LastCheckedDefectId;
            logger.Log(LogMessage.SeverityType.Debug, string.Format("Updating last check time to {0} on item {1}.",
                lastValidSynchronisationProfile.LastCheckForClosedWorkitems, lastValidSynchronisationProfile.LastClosedWorkitemId));
        }

        private void GetCreatedWorkitems(object pubobj) {
            var source = pubobj as CreatedWorkitemsSource;

            string LastCreatedWorkitemId = lastValidSynchronisationProfile.LastCreatedWorkitemId;
            DateTime LastCreatedTimestamp = lastValidSynchronisationProfile.LastCheckForCreatedWorkitems;

            logger.Log(LogMessage.SeverityType.Info, string.Format("Checking V1 workitems created since {0} was created at {1}.", LastCreatedWorkitemId, LastCreatedTimestamp));

            var workitems = externalWorkitemsQuerier.GetNewWorkitems(LastCreatedTimestamp, LastCreatedWorkitemId, source.Scopes);

            logger.Log(LogMessage.SeverityType.Info, string.Format("Found {0} workitems created since {1} where Source is empty.", workitems.Count, LastCreatedTimestamp));

            if (workitems.Count > 0) {
                eventManager.Publish(workitems);

                foreach (var result in workitems.Where(result => result.ChangesProcessed)) {
                    logger.Log(LogMessage.SeverityType.Info, string.Format("Issue {0} created successfully.", result.ExternalId));
                }
            }

            lastValidSynchronisationProfile.LastCheckForCreatedWorkitems = workitems.QueryTimeStamp;
            lastValidSynchronisationProfile.LastCreatedWorkitemId = workitems.LastCheckedDefectId;

            logger.Log(LogMessage.SeverityType.Debug, string.Format("Updating last check time to {0}.", LastCreatedTimestamp));
        }


        private void UpdateNewWorkitem(object pubobj) {
            var result = pubobj as IssueCreatedResult;

            workitemWriter.UpdateExternalWorkitem(result.Source);
        }


        private void ProcessWorkitem(object pubobj) {
            var toSendToV1 = pubobj as Workitem;

            if(toSendToV1 == null) {
                return;
            }

            try {
                if(workitemWriter.CheckForDuplicate(toSendToV1)) {
                    logger.Log(LogMessage.SeverityType.Info,
                        string.Format("Found existing workitem for {0} issue {1}.", toSendToV1.ExternalSystemName, toSendToV1.ExternalId));
                    return;
                }

                var result = workitemWriter.CreateWorkitem(toSendToV1);

                foreach(var warning in result.Warnings) {
                    logger.Log(LogMessage.SeverityType.Info, string.Format("\t{0}", warning));
                }

                eventManager.Publish(result);
            } catch(Exception ex) {
                logger.Log(LogMessage.SeverityType.Error,
                    string.Format("Error trying to create Workitem in VersionOne for {0} in {1}:", toSendToV1, toSendToV1.ExternalSystemName));
                logger.Log(LogMessage.SeverityType.Error, ex.ToString());
            }
        }

        private void UpdateOpenWorkitems(object pubobj) {
            var itemsModifiedInJira = pubobj as WorkitemsToUpdate;
 
            string Source = itemsModifiedInJira.Source;
            DateTime LastCheckForSynchronizedWorkitems = lastValidSynchronisationProfile.LastCheckForSynchronizedWorkitems;
            string LastSynchronizedWorkitemId = lastValidSynchronisationProfile.LastSynchronizedWorkitemId;


            WorkitemStateChangeCollection itemsModifiedInVersionOne = null;
            try {
                logger.Log(LogMessage.SeverityType.Info, string.Format("Checking V1 workitems modified since {0}.", LastCheckForSynchronizedWorkitems));
                itemsModifiedInVersionOne = externalWorkitemsQuerier.GetWorkitemsReadyForSynchronisation(LastCheckForSynchronizedWorkitems, Source, externalIdFieldName, LastSynchronizedWorkitemId);

            } catch (Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, string.Format("Error trying to synchronize open issues between Jira-VersionOne"));
                logger.Log(LogMessage.SeverityType.Error, ex.ToString());
            }

            lastValidSynchronisationProfile.LastCheckForSynchronizedWorkitems = itemsModifiedInVersionOne.QueryTimeStamp;
            lastValidSynchronisationProfile.LastSynchronizedWorkitemId = itemsModifiedInVersionOne.LastCheckedDefectId;
            logger.Log(LogMessage.SeverityType.Debug, string.Format("Updating last check for modification time to {0} on item {1}.",
                lastValidSynchronisationProfile.LastCheckForSynchronizedWorkitems, lastValidSynchronisationProfile.LastSynchronizedWorkitemId));

            SynchronizeItems(itemsModifiedInJira, itemsModifiedInVersionOne);

        }

        private void SynchronizeItems(WorkitemsToUpdate itemsModifiedInJira, WorkitemStateChangeCollection itemsModifiedInVersionOne) {

            logger.Log(string.Format("There are {0} items modified in Jira", itemsModifiedInJira.workitemsForUpdate.Count));
            logger.Log(string.Format("There are {0} items modified in VersionOne", itemsModifiedInVersionOne.Count));

            WorkitemStateChangeCollection readyForSynchronisationToJira = new WorkitemStateChangeCollection();
            readyForSynchronisationToJira.copyFrom(itemsModifiedInVersionOne);

            List<Workitem> readyForSynchronizationToVersionOne = new List<Workitem>();


            foreach (Workitem jiraIssue in itemsModifiedInJira.workitemsForUpdate) {
                WorkitemStateChangeResult v1item = itemsModifiedInVersionOne.Find(x => x.ExternalId.Equals(jiraIssue.ExternalId)  );
                if (v1item == null) {
                    logger.Log(string.Format("Issue {0} modifie in JIRA ONLY and not modified in VersionOne", jiraIssue.ExternalId));
                    readyForSynchronizationToVersionOne.Add(jiraIssue);
                    continue;
                }

                if (jiraIssue.Updated.Value.CompareTo(v1item.ChangeDateUtc) > 0) {
                    logger.Log(string.Format("Issue {0} modified in JIRA on {1} and in VersionOne on {2} - JIRA update is newer", jiraIssue.ExternalId, jiraIssue.Updated.Value, v1item.ChangeDateUtc));
                    readyForSynchronizationToVersionOne.Add(jiraIssue);
                } else {
                    logger.Log(string.Format("Issue {0} modified in JIRA on {1} and in VersionOne on {2} - VersionOne update is newer", jiraIssue.ExternalId, jiraIssue.Updated, v1item.ChangeDateUtc));
                    readyForSynchronisationToJira.Add(v1item);
                }
            }

            foreach (var v1item in itemsModifiedInVersionOne) {
                if (itemsModifiedInJira.workitemsForUpdate.Exists(x => x.ExternalId.Equals(v1item.ExternalId)) == false) {
                    logger.Log(string.Format("Issue {0} modified in VersionOne ONLY and not modified in JIRA", v1item.ExternalId));
                    readyForSynchronisationToJira.Add(v1item);
                } 
                else {
                    logger.Log(string.Format("Issue {0} modified in VersionOne and JIRA", v1item.ExternalId));
                    //poprzednia pętla mi załatwia ten warunek - nic nie trzeba juz tu dodawać
                }
            }

            SynchronizeItemsToJira(readyForSynchronisationToJira);
            SynchronizeItemsToVersionOne(readyForSynchronizationToVersionOne);

        }

        private void SynchronizeItemsToJira(WorkitemStateChangeCollection readyForSynchronisationToJira) {

            if (readyForSynchronisationToJira.Count <= 0) {
                logger.Log(LogMessage.SeverityType.Info, string.Format("Skipping Jira update - nothing to synchronize", readyForSynchronisationToJira.Count));
                return;
            }
         
            logger.Log(LogMessage.SeverityType.Info, string.Format("There are {0} items to update in Jira.", readyForSynchronisationToJira.Count));
            eventManager.Publish(readyForSynchronisationToJira);

            foreach (var result in readyForSynchronisationToJira.Where(result => result.ChangesProcessed)) {
                logger.Log(LogMessage.SeverityType.Info, string.Format("Issue {0} updated successfully in Jira.", result.ExternalId));
            }
         }

        private void SynchronizeItemsToVersionOne(List<Workitem> readyForSynachronizationToVersionOne) {
            
            if (readyForSynachronizationToVersionOne.Count <= 0) {
                logger.Log(LogMessage.SeverityType.Info, string.Format("Skipping VersionOne update - nothing to synchronize"));
                return;
            }

            UpdateResult theNewestUpdate = new UpdateResult(DateTime.MinValue, string.Empty);

            logger.Log(LogMessage.SeverityType.Info, string.Format("There are {0} items to update in VersionOne.", readyForSynachronizationToVersionOne.Count));
            foreach (var item in readyForSynachronizationToVersionOne) {
                var result = workitemWriter.UpdateWorkitem(item);

                if (result != null && result.isDefault() == false && result.isNewer(theNewestUpdate) == true) {
                    theNewestUpdate = result;
                }
            }

            if (theNewestUpdate.modificationTime > lastValidSynchronisationProfile.LastCheckForSynchronizedWorkitems) {
                lastValidSynchronisationProfile.LastCheckForSynchronizedWorkitems = theNewestUpdate.modificationTime;
                lastValidSynchronisationProfile.LastClosedWorkitemId = theNewestUpdate.number;

                logger.Log(LogMessage.SeverityType.Debug, string.Format("Updating last synchronization time to {0} on item {1}.",
                lastValidSynchronisationProfile.LastCheckForClosedWorkitems, lastValidSynchronisationProfile.LastClosedWorkitemId));
            }
        }

    }
}