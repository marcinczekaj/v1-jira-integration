/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServerConnector.StartupValidation {
    public class V1FieldValidator : BaseValidator {
        private readonly string fieldName;
        private readonly string containingTypeToken;

        public V1FieldValidator(string fieldName, string containingTypeToken) {
            this.fieldName = fieldName;
            this.containingTypeToken = containingTypeToken;
        }

        public override bool Validate() {
            Logger.Log(LogMessage.SeverityType.Info, string.Format("Checking custom field for {0}...", containingTypeToken.Equals("Theme") ? "Feature Group" : containingTypeToken));

            if (!V1Processor.AttributeExists(containingTypeToken, fieldName)) {
                Logger.Log(LogMessage.SeverityType.Error, string.Format("Custom field {0} is not assigned to type {1}", fieldName, containingTypeToken));
                return false;
            }

            Logger.Log(LogMessage.SeverityType.Info, "Custom field check successful.");
            return true;
        }
    }
}