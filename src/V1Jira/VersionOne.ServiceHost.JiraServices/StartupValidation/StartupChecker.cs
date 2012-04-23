/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using VersionOne.ServerConnector.StartupValidation;
using VersionOne.ServiceHost.Core.StartupValidation;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.JiraServices.StartupValidation {
    public class StartupChecker : StartupCheckerBase {
        private readonly JiraServiceConfiguration config;

        public StartupChecker(JiraServiceConfiguration config, IEventManager eventManager) : base(eventManager) {
            this.config = config;            
        }

        protected override IEnumerable<IValidationStep> CreateValidators() {
            return new List<IValidationStep> {
                new ValidationSimpleStep(new V1ConnectionValidator(), null),
                new ValidationSimpleStep(new JiraConnectionValidator(config.Url, config.UserName, config.Password), null),
                new ValidationSimpleStep(new MappingValidator(config.ProjectMappings, "Project"), null),
                new ValidationSimpleStep(new MappingValidator(config.PriorityMappings, "Priority"), null),
                new ValidationSimpleStep(new V1ProjectsValidator(config.ProjectMappings.Values), null),
                new NonStrictValidationSimpleStep(new JiraCustomFieldsValidator(config.Url, config.UserName, config.Password, config.OnCreateFieldName, config.OnStateChangeFieldName, config.WorkitemLinkField), null),
                new ValidationSimpleStep(new JiraFilterValidation(config.Url, config.UserName, config.Password, config.OpenDefectFilter), null),
                new ValidationSimpleStep(new JiraFilterValidation(config.Url, config.UserName, config.Password, config.OpenStoryFilter), null),
                new ValidationSimpleStep(new JiraFilterValidation(config.Url, config.UserName, config.Password, config.UpdateWorkitemFilter), null),
                new ValidationSimpleStep(new V1PrioritiesValidator(config.PriorityMappings.Values), null),
                new ValidationSimpleStep(new JiraPrioritiesValidator(config.Url, config.UserName, config.Password, config.PriorityMappings.Keys), null),
                new ValidationSimpleStep(new JiraProjectsValidator(config.Url, config.UserName, config.Password, config.ProjectMappings.Keys), null),
            };            
        }
    }
}