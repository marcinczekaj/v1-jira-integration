/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using System.Linq;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Configuration;
namespace VersionOne.ServerConnector.StartupValidation {
    public class V1ProjectsValidator : BaseValidator {
        private readonly ICollection<MappingInfo> v1Projects;

        public V1ProjectsValidator(ICollection<MappingInfo> v1Projects) {
            this.v1Projects = v1Projects;
        }

        public override bool Validate() {
            Logger.Log(LogMessage.SeverityType.Info, "Checking VersionOne projects");
            var result = true;

            foreach(var project in v1Projects.Where(project => !V1Processor.ProjectExists(project.Id))) {
                Logger.Log(LogMessage.SeverityType.Error, string.Format("Project with '{0}' id doesn't exist in VersionOne", project.Id));
                result = false;
            }

            Logger.Log(LogMessage.SeverityType.Info, "All projects are checked");
            return result;
        }
    }
}