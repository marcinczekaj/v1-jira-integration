/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Net;
using System.Linq;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Logging;
using System.Collections.Generic;
using System.Collections;

namespace VersionOne.ServiceHost.WorkitemServices {
    /// <summary>
    /// Creates workitems in VersionOne to match issues that come from an external system.
    /// </summary>
    public class WorkitemWriter {
        private readonly string externalFieldName;
        private readonly IVersionOneProcessor v1Processor;
        private readonly ILogger logger;
        private IDictionary<string, string> statusMapping = null;

        public WorkitemWriter(string externalIdFieldName) {
            externalFieldName = externalIdFieldName;
            logger = ComponentRepository.Instance.Resolve<ILogger>();
            v1Processor = ComponentRepository.Instance.Resolve<IVersionOneProcessor>();
        }

        public WorkitemUpdateResult CreateWorkitem(Workitem item) {
            if(item == null) {
                throw new ArgumentNullException("item");
            }

            var type = item.Type;

            logger.Log(LogMessage.SeverityType.Info,
                string.Format("Creating VersionOne {0} for item from {1} system with identifier {2}", type, item.ExternalSystemName, item.ExternalId));

            var url = string.Empty;
            var urlTitle = string.Empty;

            if(item.ExternalUrl != null) {
                url = item.ExternalUrl.Url;
                urlTitle = item.ExternalUrl.Title;
            }

            ServerConnector.Entities.Workitem newWorkitem = null;
            string failureMessage = null;

            try {
                newWorkitem = v1Processor.CreateWorkitem(type, item.Title, item.Description, item.ProjectId, item.Project, externalFieldName, item.ExternalId,
                    item.ExternalSystemName, item.Priority, item.Owners, urlTitle, url, item.Environment, item.FoundBy, item.VersionAffected, item.BuildNumber, item.SeverityLevel);
                logger.Log(LogMessage.SeverityType.Info, string.Format("VersionOne asset {0} succesfully created.", newWorkitem.Id));
            } catch (Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, string.Format("Error during saving workitems: {0}", ex.Message));
                failureMessage = string.Format("Faild to create item in versionOne, reason: \"{0}\"", ex.Message);
            }

            if(newWorkitem != null) {
                var result = new WorkitemCreationResult(item) {
                    Source = {
                        Number = newWorkitem.Number, 
                        ExternalId = item.ExternalId,
                        Description = newWorkitem.Description,
                        ExternalSystemName = item.ExternalSystemName,
                        ProjectId = newWorkitem.Project.Key,
                        Project = newWorkitem.Project.Value,
                        Title = newWorkitem.Name,
                        Priority = newWorkitem.PriorityToken,
                        Environment = newWorkitem.Environment,
                    },
                    WorkitemId = newWorkitem.Id,
                    Permalink = v1Processor.GetWorkitemLink(newWorkitem),
                };

                result.Messages.Add(string.Format("Created item \"{0}\" ({1}) in Project \"{2}\" URL: {3}",
                    item.Title,
                    result.Source.Number,
                    item.Project,
                    result.Permalink));

                return result;
            }

            var failureResult = new WorkitemCreationFailureResult(item);
            failureResult.Messages.Add(failureMessage);
            failureResult.Warnings.Add(string.Format("[{0}] {1}", item.ExternalId, failureMessage));
            return failureResult;
        }

        public bool UpdateExternalWorkitem(Workitem workitem)
        {
            var primaryWorkitem = v1Processor.GetPrimaryWorkitemByNumber(workitem.Number);

            if (primaryWorkitem == null)
                return false;

            primaryWorkitem.Source = workitem.ExternalSystemName;
            primaryWorkitem.Reference = workitem.ExternalId;
            v1Processor.SaveWorkitem(primaryWorkitem);

            Link link = new Link(workitem.ExternalUrl.Url, workitem.ExternalUrl.Title, true);
            v1Processor.AddLinkToWorkitem(primaryWorkitem, link);

            return true;
        }

        public bool CheckForDuplicate(Workitem item) {
            if(string.IsNullOrEmpty(item.ExternalSystemName) || string.IsNullOrEmpty(externalFieldName)) {
                // Can't check for duplicates if we don't have the Source and the ID.
                return false;
            }

            try {
                var filter = GroupFilter.And(
                    Filter.Equal(Entity.SourceNameProperty, item.ExternalSystemName),
                    Filter.Equal(externalFieldName, item.ExternalId)
                );

                var duplicates = v1Processor.GetPrimaryWorkitems(filter);
                return duplicates.Count > 0;
            } catch(WebException) {
                //LogMessage.Log(LogMessage.SeverityType.Error, string.Format("Error querying VersionOne ({0}) for closed external defects:\r\n{1}", ex.Response.ResponseUri, ex.ToString()), _eventManager);
            }

            return false;
        }

        public UpdateResult UpdateWorkitem(Workitem item) {

            if (item == null)
                return null;

            if(statusMapping == null)
                statusMapping = v1Processor.GetAvailableStatuses();

            try {

                string externalId = item.ExternalId;
                string externalSystemName = item.ExternalSystemName;
                string statusId = ResolveStatusIdFromName(item.Status);
                UpdateResult result = new UpdateResult();
                v1Processor.UpdateWorkitem(externalFieldName, externalId, externalSystemName, statusId, result);
                return result;
                

            }catch (Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, string.Format("Faild to update item in versionOne, reason: \"{0}\"", ex.Message));
            }

            return null;
        }

        private string ResolveStatusIdFromName(string v1statusName) {

            if (statusMapping.ContainsKey(v1statusName) == false)
                return String.Empty;

            return statusMapping[v1statusName];
        }

    }
}