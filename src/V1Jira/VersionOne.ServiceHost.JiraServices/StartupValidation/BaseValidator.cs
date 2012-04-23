/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.ServerConnector;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.StartupValidation;

namespace VersionOne.ServiceHost.JiraServices.StartupValidation {
    public abstract class BaseValidator : ISimpleValidator {
        protected readonly ILogger Logger;
        protected readonly IVersionOneProcessor V1Processor;
        private readonly IJiraServiceFactory jiraServiceFactory;
        private readonly string url;


        protected  BaseValidator(string url) {
            Logger = ComponentRepository.Instance.Resolve<ILogger>();
            V1Processor = ComponentRepository.Instance.Resolve<IVersionOneProcessor>();
            jiraServiceFactory = ComponentRepository.Instance.Resolve<IJiraServiceFactory>();
            this.url = url;
        }

        public abstract bool Validate();

        protected IJiraProxy GetJiraService() {
            return jiraServiceFactory.CreateNew(url);
        }
    }
}