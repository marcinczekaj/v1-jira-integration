/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Core.Utility;
using VersionOne.ServiceHost.Core.Configuration;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.JiraServices.Exceptions;
using VersionOne.ServiceHost.JiraServices.StartupValidation;
using VersionOne.ServiceHost.WorkitemServices;
using ConfigurationException = VersionOne.ServiceHost.Core.Utility.ConfigurationException;

namespace VersionOne.ServiceHost.JiraServices {
	/// <summary>
	/// A service that:
	///		1) Polls JIRA on a configurable interval for Issues to create in VersionOne.
	///		2) Listens for Defects created in VersionOne and updates the corresponding issues in JIRA.
	/// </summary>
	public class JiraHostedService : IHostedService {
		public class IntervalSync {}

		private IJiraIssueProcessor jiraProcessor;
		private IEventManager eventManager;
		private string sourceFieldValue;
		private ILogger logger;
		private string serviceFactoryName;
		private JiraServiceConfiguration jiraConfig;
		private XmlElement config;
		private SynchronizationProfile profile;

		public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
			this.eventManager = eventManager;
			this.config = config;
			logger = new Logger(eventManager);

			try {
				jiraConfig = GetValuesFromConfiguration();
			} catch(Exception ex) {
				logger.Log(LogMessage.SeverityType.Error, "Error during reading settings from configuration file.", ex);
				return;
			}
			
			serviceFactoryName = config["JIRAServiceFactory"].InnerText;
			sourceFieldValue = config["SourceFieldValue"].InnerText;

			this.profile = new SynchronizationProfile(profile);

			InitializeComponents();

			#region Guard Conditions

			if(jiraConfig.Url == null) {
				throw new ConfigurationException("Cannot initialize JIRA Service without a URL");
			}

			if(serviceFactoryName == null) {
				throw new ConfigurationException("Cannot initialize JIRA Service without the fully-qualified name of an IJiraServiceFactory.");
			}

			#endregion
			
			//tu nawiazuje sie polaczenei do V1 i inicjalizuje cala reszta....
			var checker = new StartupChecker(jiraConfig, this.eventManager);
			checker.Initialize();

			this.eventManager.Subscribe(typeof(IntervalSync), OnInterval);
			this.eventManager.Subscribe(typeof(WorkitemCreationResult), OnWorkitemCreated);
			this.eventManager.Subscribe(typeof(WorkitemCreationFailureResult), OnWorkitemCreationFailureResult);
			this.eventManager.Subscribe(typeof(WorkitemStateChangeCollection), OnWorkitemUpdate);
			this.eventManager.Subscribe(typeof(NewVersionOneWorkitemsCollection), OnNewWorkitemsInVersionOne);
		}

		public void Start() {
			// TODO move subscriptions to timer events, etc. here
		}

		private JiraServiceConfiguration GetValuesFromConfiguration() {
			jiraConfig = new JiraServiceConfiguration();

			ConfigurationReader.ReadConfigurationValues(jiraConfig, config);
			ConfigurationReader.ProcessMappingSettings(jiraConfig.ProjectMappings,
				config["ProjectMappings"],
				"JIRAProject",
				"VersionOneProject");
			ConfigurationReader.ProcessMappingSettings(jiraConfig.PriorityMappings, jiraConfig.ReversePriorityMapping,
				config["PriorityMappings"],
				"JIRAPriority",
				"VersionOnePriority");
			ConfigurationReader.ProcessMappingSettings(jiraConfig.SeverityMappings, jiraConfig.ReverseSeverityMapping,
				config["SeverityMappings"],
				"JiraSeverity",
				"VersionOneSeverity");
			ConfigurationReader.ProcessMappingSettings(jiraConfig.FieldMappings,
			   config["FieldMappings"],
			   "VersionOneFieldName",
			   "JiraFieldName");
			ConfigurationReader.ProcessMappingSettings(jiraConfig.TransitionMappings, 
				config["StatusTransitionsMappings"], 
				"Mapping", 
				"Status", 
				"Transition");
			ConfigurationReader.ProcessMappingSettings(jiraConfig.StatusMappings,
				config["StatusMappings"],
				"Mapping",
				"V1Status",
				"JiraStatus"
				);
			jiraConfig.ReverseStatusMapping = CreateReverseStatusMapping(jiraConfig.StatusMappings);

			jiraConfig.OpenDefectFilter = GetFilterFromConfiguration("CreateDefectFilter");
			jiraConfig.OpenStoryFilter = GetFilterFromConfiguration("CreateStoryFilter");
			jiraConfig.UpdateWorkitemFilter = GetFilterFromConfiguration("UpdateWorkitemFilter");

			var settingsNode = config["Settings"];

			if(settingsNode != null && !settingsNode["ApplicationUrl"].InnerText.EndsWith("/")) {
				settingsNode["ApplicationUrl"].InnerText += "/";
			}

			return jiraConfig;
		}

		private IDictionary<string, HashSet<string>> CreateReverseStatusMapping(IDictionary<MappingInfo, IList<MappingInfo>> v1ToJiraMappings) {

			IDictionary<string, HashSet<string>> reverseMapping = new Dictionary<string, HashSet<string>>();

			foreach (var v1ToJiraMapping in v1ToJiraMappings) { 
				var v1status = v1ToJiraMapping.Key;
				
				foreach(var jiraStatus in v1ToJiraMapping.Value){
					if( reverseMapping.ContainsKey(jiraStatus.Name) == false)
						reverseMapping.Add(jiraStatus.Name, new HashSet<string>());

					reverseMapping[jiraStatus.Name].Add(v1status.Name);
				}
			}
			return reverseMapping;
		}

		private JiraFilter GetFilterFromConfiguration(string nodeName) {
			var node = config[nodeName];
			
			if(node == null) {
				throw new JiraConfigurationException("Can't read filter information");
			}

			var idAttribute = node.Attributes["id"];
			var id = idAttribute != null ? idAttribute.Value : string.Empty;

			var disabledAttribute = node.Attributes["disabled"];
			var disabled = disabledAttribute != null && disabledAttribute.Value == "1";

			return new JiraFilter(id, !disabled);
		}

		private void InitializeComponents() {
			var serviceFactoryType = Type.GetType(serviceFactoryName);

			if(serviceFactoryType == null) {
				throw new InvalidOperationException(string.Format("Cannot find type by name \"{0}\".  Are you missing an Assembly (dll)?", serviceFactoryName));
			}

			var serviceFactory = (IJiraServiceFactory)Activator.CreateInstance(serviceFactoryType);

			ComponentRepository.Instance.Register(logger);
			ComponentRepository.Instance.Register(serviceFactory);

			jiraProcessor = new JiraIssueReaderUpdater(jiraConfig);
			ComponentRepository.Instance.Register(jiraProcessor);
		}

		/// <summary>
		/// Timer interval on which to poll JIRA. See app config file for time in milliseconds.
		/// </summary>
		/// <param name="pubobj">Not used</param>
		private void OnInterval(object pubobj) {

			for (int i = 0; i < 5; i++ )
				logger.Log(string.Empty);

			logger.Log(LogMessage.SeverityType.Info, "-------  Starting processing... -------\n\n");


           
            if (jiraConfig.UpdateWorkitemFilter.Enabled) {
                logger.Log(LogMessage.SeverityType.Info, "------- Synchronization -------");
                
			    List<Workitem> workitemsForUpdate = new List<Workitem>();

                try {
                    logger.Log(LogMessage.SeverityType.Info, "Getting issues from JIRA.");

                    var lastQueryTime = profile.LastQueryForModifiedItems;
                    var issues = jiraProcessor.GetIssues<Defect>(jiraConfig.UpdateWorkitemFilter.Id);

                    if (issues.Count > 0) {
                        logger.Log(string.Format("Found {0} defects in JIRA that came from VersionOne. We will filter issues modified after {1}", issues.Count, lastQueryTime));
                    }

                    DateTime theNewestItem = lastQueryTime;
                    foreach (var issue in issues) {
                        var modificationTime = issue.Updated.Value;

                        if (modificationTime > lastQueryTime) {
                            logger.Log(LogMessage.SeverityType.Info, string.Format("\tissue {0} was modified on {1} and will be processed", issue.ExternalId, issue.Updated.Value));

                            issue.ExternalSystemName = sourceFieldValue;
                            workitemsForUpdate.Add(issue);

                            if (modificationTime > theNewestItem) {
                                logger.Log(LogMessage.SeverityType.Debug, string.Format("\tissue {0} is possibly the newsest one in this query", issue.ExternalId));
                                theNewestItem = modificationTime;
                            }
                        } else {
                            logger.Log(LogMessage.SeverityType.Debug, string.Format("\tskipping issue {0} since it was modified earlier than last query", issue.ExternalId));
                        }
                    }

                    if (theNewestItem > lastQueryTime) {
                        logger.Log(LogMessage.SeverityType.Debug, string.Format("Setting last query time to {0}", theNewestItem));
                        profile.LastQueryForModifiedItems = theNewestItem;
                    }

                    var itemsOpenInJira = new WorkitemsToUpdate(workitemsForUpdate, sourceFieldValue);
                    eventManager.Publish(itemsOpenInJira);
                } catch (Exception ex) {
                    logger.Log(LogMessage.SeverityType.Error, string.Format("Error getting Issues for update from JIRA: {0}", ex.Message));
                    return;
                }
            }





            logger.Log(LogMessage.SeverityType.Info, "------- Create New Workitems from Jira -------");
			
            IList<Workitem> workitems = new List<Workitem>();
            try {
                logger.Log(LogMessage.SeverityType.Info, "Getting issues from JIRA.");

                if (jiraConfig.OpenDefectFilter.Enabled) {
                    var bugs = jiraProcessor.GetIssues<Defect>(jiraConfig.OpenDefectFilter.Id);

                    if (bugs.Count > 0) {
                        logger.Log(string.Format("Found {0} defects in JIRA to create in VersionOne.", bugs.Count));
                    }

                    foreach (var bug in bugs) {
                        workitems.Add(bug);
                    }
                }

                if (jiraConfig.OpenStoryFilter.Enabled) {
                    var stories = jiraProcessor.GetIssues<Story>(jiraConfig.OpenStoryFilter.Id);

                    if (stories.Count > 0) {
                        logger.Log(string.Format("Found {0} stories in JIRA to create in VersionOne.", stories.Count));
                    }

                    foreach (var story in stories) {
                        workitems.Add(story);
                    }
                }

            } catch (Exception ex) {
                logger.Log(LogMessage.SeverityType.Error, string.Format("Error getting Issues from JIRA: {0}", ex.Message));
                return;
            }

			logger.Log(LogMessage.SeverityType.Info, "Checking JIRA issues for duplicates.");
			workitems = workitems.Distinct().ToList();

			foreach(var item in workitems) {
				item.ExternalSystemName = sourceFieldValue;
				eventManager.Publish(item);
			}



			logger.Log(LogMessage.SeverityType.Info, "------- Close JIRA bugs (that were closed in VersionOne) -------");
			var sourceClosed = new ClosedWorkitemsSource(sourceFieldValue);
			eventManager.Publish(sourceClosed);
            

			logger.Log(LogMessage.SeverityType.Info, "------- Push VersionOne defects to JIRA -------");
			var v1scopes = GetV1ScopesFromMappings();
			var sourceCreated = new CreatedWorkitemsSource(v1scopes);
			eventManager.Publish(sourceCreated);


			logger.Log(LogMessage.SeverityType.Info, "------- Processing finished. ------- ");
		}

        private IEnumerable<String> GetV1ScopesFromMappings() {
            return jiraConfig.ProjectMappings.Values.Select(value => value.Id).Distinct();
        }

		/// <summary>
		/// A Defect or Story was created in V1 that corresponds to an Issue in JIRA. 
		/// We update the Issue in JIRA to reflect that.
		/// </summary>
		/// <param name="pubobj">WorkitemCreationResult of created defect.</param>
		private void OnWorkitemCreated(object pubobj) {
			var creationResult = pubobj as WorkitemCreationResult;

			if(creationResult != null) {
				jiraProcessor.OnWorkitemCreated(creationResult);
			}
		}


        private void OnWorkitemCreationFailureResult(object pubobj) {
            var creationResult = pubobj as WorkitemCreationFailureResult;

            if (creationResult != null) {
                jiraProcessor.OnWorkitemCreationFailure(creationResult);
            }
        }

        private void OnNewWorkitemsInVersionOne(object pubobj) {
            var workitems = pubobj as NewVersionOneWorkitemsCollection;

            if (workitems == null) {
                return;
            }

            logger.Log(LogMessage.SeverityType.Info, "Creating issues in JIRA");

            foreach (var workitem in workitems) {
                workitem.ChangesProcessed = true;

                if (!jiraProcessor.OnNewWorkitem(workitem, sourceFieldValue)) {
                    workitem.ChangesProcessed = false;
                } else {
                    IssueCreatedResult result = new IssueCreatedResult(workitem);
                    eventManager.Publish(result);
                }
            }
        }

		private void OnWorkitemUpdate(object pubobj) {
			var stateChangeCollection = pubobj as WorkitemStateChangeCollection;

			if(stateChangeCollection == null) {
				return;
			}

			foreach(var stateChangeResult in stateChangeCollection) {
				stateChangeResult.ChangesProcessed = true;

				if (!jiraProcessor.OnWorkitemUpdate(stateChangeResult)) {
					stateChangeResult.ChangesProcessed = false;
				}
			}
		}

	}
}