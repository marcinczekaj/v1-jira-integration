/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServerConnector.StartupValidation {
    public class V1ProjectValidator : BaseValidator {
        private readonly string projectToken;

        public V1ProjectValidator(string projectToken) {
            this.projectToken = projectToken;
        }
        
        public override bool Validate() {
            Logger.Log(LogMessage.SeverityType.Info, "Checking VersionOne project");

            if(!V1Processor.ProjectExists(projectToken)) {
                Logger.Log(LogMessage.SeverityType.Error, string.Format("VersionOne project with '{0}' id doesn't exist", projectToken));
                return false;
            }

            Logger.Log(LogMessage.SeverityType.Info, string.Format("VersionOne project with '{0}' id successfully found", projectToken));
            return true;
        }
    }
}