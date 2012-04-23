/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServerConnector.StartupValidation {
    public class V1ConnectionValidator : BaseValidator {
        public override bool Validate() {
            Logger.Log(LogMessage.SeverityType.Info, "Validating connection to VersionOne");

            if(!V1Processor.ValidateConnection()) {
                Logger.Log(LogMessage.SeverityType.Error, "Cannot establish connection to VersionOne");
                return false;
            }

            Logger.Log(LogMessage.SeverityType.Info, "Connection to VersionOne is established successfully");
            return true;
        }
    }
}