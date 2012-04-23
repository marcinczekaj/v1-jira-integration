/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Logging;
using System.Collections;

namespace VersionOne.ServiceHost.WorkitemServices {
    public class ExternalWorkitemQuerier {
        private static readonly string JiraClosedStatus = "Closed";

        private readonly ILogger logger;
        private readonly IVersionOneProcessor v1Processor;

        public ExternalWorkitemQuerier() {
            logger = ComponentRepository.Instance.Resolve<ILogger>();
            v1Processor = ComponentRepository.Instance.Resolve<IVersionOneProcessor>();
        }

        public WorkitemStateChangeCollection GetWorkitemsClosedSince(DateTime closedSince, string sourceName, string externalIdFieldName, string lastCheckedDefectId) {
            var results = new WorkitemStateChangeCollection();
            var lastCheckedDefectIdLocal = lastCheckedDefectId;
            var lastChangeDateLocal = closedSince;

            try {
                var filters = new List<IFilter> {
                    Filter.Closed(true),
                    Filter.OfTypes(VersionOneProcessor.StoryType, VersionOneProcessor.DefectType),
                    Filter.Equal(Entity.SourceNameProperty, sourceName),
                };

                if (closedSince != DateTime.MinValue) {
                    filters.Add(Filter.Greater(Entity.ChangeDateUtcProperty, closedSince));
                }

                var filter = GroupFilter.And(filters.ToArray());

                var workitems = v1Processor.GetPrimaryWorkitems(filter).Select(x => new WorkitemFromExternalSystem(x, externalIdFieldName));

                foreach(var item in workitems) {
                    var id = item.Number;
                    var changeDateUtc = item.ChangeDateUtc;

                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Processing V1 Defect {0} closed at {1}", id, changeDateUtc));

                    if(lastCheckedDefectId.Equals(id)) {
                        logger.Log(LogMessage.SeverityType.Debug, "\tSkipped because this ID was processed last time");
                        continue;
                    }

                    if(closedSince.CompareTo(changeDateUtc) >= 0) {
                        logger.Log(LogMessage.SeverityType.Debug, "\tSkipped because the ChangeDate is less than the date/time we last checked for changes");
                        continue;
                    }

                    if((lastChangeDateLocal == DateTime.MinValue && changeDateUtc != DateTime.MinValue) || changeDateUtc.CompareTo(lastChangeDateLocal) > 0) {
                        logger.Log(LogMessage.SeverityType.Debug, "\tCaused an update to LastChangeID and dateLastChanged");
                        lastChangeDateLocal = changeDateUtc;
                        lastCheckedDefectIdLocal = id;
                    }

                    var result = new WorkitemStateChangeResult(item.ExternalId, item.Number, item.ChangeDateUtc); 
                    result.FieldUpdates.Add(Entity.StatusNameProperty, JiraClosedStatus );                   
                    results.Add(result);
                }
            } catch(WebException ex) {
                string responseMessage = null;

                if(ex.Response != null) {
                    using(var reader = new StreamReader(ex.Response.GetResponseStream())) {
                        responseMessage = reader.ReadToEnd();
                    }
                }

                logger.Log(LogMessage.SeverityType.Error, string.Format("Error querying VersionOne ({0}) for closed external defects:\r\n{1}\r\n\r\n{2}", ex.Response.ResponseUri, responseMessage, ex));
            }

            results.LastCheckedDefectId = lastCheckedDefectIdLocal;
            results.QueryTimeStamp = lastChangeDateLocal;

            return results;
        }

        private IFilter FilterDefectsCreatedSince(DateTime createdSince, String scope)
        {

            var filters = new List<IFilter> {
                    Filter.OfTypes(VersionOneProcessor.DefectType),
                    Filter.Equal(Entity.InactiveProperty, false),
                    Filter.Equal(Entity.ScopeParentAndUpProperty, scope),
                    Filter.Equal(Entity.ScopeStateProperty, 64),
                    Filter.Equal(Entity.SourceNameProperty, "")
                };

            if (createdSince != DateTime.MinValue)
            {
                filters.Add(Filter.Greater(Entity.CreateDateUtcProperty, createdSince));
            }

            var filter = GroupFilter.And(filters.ToArray());

            return filter;
        }
        
        private IFilter FilterItemsModifiedSince(DateTime closedSince, string sourceName) {
            var filters = new List<IFilter> {
                    Filter.Closed(false),
                    Filter.OfTypes(VersionOneProcessor.StoryType, VersionOneProcessor.DefectType),
                    Filter.Equal(Entity.SourceNameProperty, sourceName),
                };

            if (closedSince != DateTime.MinValue) {
                filters.Add(Filter.Greater(Entity.ChangeDateUtcProperty, closedSince));
            }

            var filter = GroupFilter.And(filters.ToArray());
            return filter;
        }


        private void createJiraUpdateResult(WorkitemFromExternalSystem item, WorkitemStateChangeCollection results) {
            var jiraUpdate = new WorkitemStateChangeResult(item.ExternalId, item.Number, item.ChangeDateUtc);

            var StatusName = String.IsNullOrEmpty(item.StatusName) ? String.Empty : item.StatusName;
            jiraUpdate.FieldUpdates.Add(Entity.StatusNameProperty, StatusName);

            results.Add(jiraUpdate);
        }


        public WorkitemStateChangeCollection GetWorkitemsReadyForSynchronisation(DateTime modifiedSince, string sourceName, string externalIdFieldName, string lastModifiedItemId) {
            var results = new WorkitemStateChangeCollection();
            var lastChangeDateLocal = modifiedSince;
            var lastLocalModifiedItemId = lastModifiedItemId;

            try {
                var filter = FilterItemsModifiedSince(modifiedSince, sourceName);
                var workitems = v1Processor.GetPrimaryWorkitems(filter).Select(x => new WorkitemFromExternalSystem(x, externalIdFieldName));

                foreach (var item in workitems) {
                    var id = item.Number;
                    var changeDateUtc = item.ChangeDateUtc;

                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Processing V1 Workitem {0} modified at {1}", id, changeDateUtc));

                    if (lastLocalModifiedItemId.Equals(id) == true && modifiedSince.CompareTo(changeDateUtc) == 0) {
                        logger.Log(LogMessage.SeverityType.Debug, "\tSkipped because this ID was processed last time");
                        continue;
                    }

                    if (lastChangeDateLocal.CompareTo(changeDateUtc) < 0) {
                        logger.Log(LogMessage.SeverityType.Debug, "\tCaused an update to lastChangeDateLocal and QueryTimeStamp");
                        lastChangeDateLocal = changeDateUtc;
                        lastLocalModifiedItemId = id;
                    }

                    createJiraUpdateResult(item, results);

                }
            } catch (WebException ex) {
                string responseMessage = null;

                if (ex.Response != null) {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream())) {
                        responseMessage = reader.ReadToEnd();
                    }
                }

                logger.Log(LogMessage.SeverityType.Error, string.Format("Error querying VersionOne ({0}) for modified external defects:\r\n{1}\r\n\r\n{2}", ex.Response.ResponseUri, responseMessage, ex));
            } catch (Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, string.Format("Error while creating update result, reason: {0}", ex.Message));
            }

            results.QueryTimeStamp = lastChangeDateLocal;
            results.LastCheckedDefectId = lastLocalModifiedItemId;

            return results;
        }

        public NewVersionOneWorkitemsCollection GetNewWorkitems(DateTime lastCreatedTimestamp, string lastCreatedWorkitemId, IEnumerable<string> scopes) {
            var results = new NewVersionOneWorkitemsCollection();

            var lastLocalCreatedTimestamp = lastCreatedTimestamp;
            var lastLocalCreatedWorkitemId = lastCreatedWorkitemId;

            try {
                foreach (string scope in scopes) {
                    var filter = FilterDefectsCreatedSince(lastCreatedTimestamp, scope);

                    var workitems = v1Processor.GetWorkitems(VersionOneProcessor.DefectType, filter);

                    foreach (VersionOne.ServerConnector.Entities.Defect item in workitems) {
                        var id = item.Number;
                        var createDateUtc = item.CreateDateUtc;

                        logger.Log(LogMessage.SeverityType.Debug, string.Format("Processing V1 Workitem {0} created at {1}", id, createDateUtc));

                        if (lastLocalCreatedWorkitemId.Equals(id) == true && lastCreatedTimestamp.CompareTo(createDateUtc) == 0) {
                            logger.Log(LogMessage.SeverityType.Debug, "\tSkipped because this ID was processed last time");
                            continue;
                        }

                        if (lastLocalCreatedTimestamp.CompareTo(createDateUtc) < 0) {
                            logger.Log(LogMessage.SeverityType.Debug, "\tCaused an update to lastCreatedTimestamp and lastCreatedWorkitemId");
                            lastLocalCreatedTimestamp = createDateUtc;
                            lastLocalCreatedWorkitemId = id;
                        }

                        //TODO extract to new method
                        var workitem = new NewVersionOneWorkitem(scope) {
                            Number = item.Number,
                            Title = item.Name,
                            Description = item.Description,
                            ProjectId = item.Project.Key,
                            Project = item.Project.Value,
                            Priority = item.Priority,
                            Environment = item.Environment,
                            Url = v1Processor.GetWorkitemLink(item), 
                            BuildNumber = item.FoundInBuild,
                            SeverityLevel = item.SeverityLevel,
                            Created = item.CreateDateUtc,
                            Updated = item.ChangeDateUtc,
                        };

                        if (item.CreatedByUsername != null)
                            workitem.CreatedBy = ExtractUsername(item.CreatedByUsername);

                        if (item.OwnersUsernames != null)
                            workitem.Owners = ExtractUsernames(item.OwnersUsernames);

                        workitem.Messages.Add("VersionOne URL: " + workitem.Url);

                        results.Add(workitem);
                    }
                }
            } catch (WebException ex) {
                string responseMessage = null;

                if (ex.Response != null) {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream())) {
                        responseMessage = reader.ReadToEnd();
                    }
                }

                logger.Log(LogMessage.SeverityType.Error, string.Format("Error querying VersionOne ({0}) for newly created defects:\r\n{1}\r\n\r\n{2}", ex.Response.ResponseUri, responseMessage, ex));
            } catch (Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, string.Format("Error while creating new workitems list, reason: {0}", ex.Message));
            }

            results.QueryTimeStamp = lastLocalCreatedTimestamp;
            results.LastCheckedDefectId = lastLocalCreatedWorkitemId;

            return results;
        }

        private string ExtractUsernames(List<string> list) {
            var result = list.Where(name => name != null)
                .Select(username => ExtractUsername(username))
                .ToArray();

            if (result.Length < 1)
                return null;

            return String.Join(",", result);
        }

        private string ExtractUsername(string username) {
            if (username.Contains('\\'))
                return username.Substring(username.LastIndexOf("\\") + 1);
            
            return username;
        }

    }
}