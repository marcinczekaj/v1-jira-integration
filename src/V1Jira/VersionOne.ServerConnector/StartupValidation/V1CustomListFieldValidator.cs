/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Linq;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServerConnector.StartupValidation {
    public class V1CustomListFieldValidator : BaseValidator {
        private readonly string fieldName;
        private readonly string containingTypeToken;
        private readonly string[] listValueTokens;
        
        public V1CustomListFieldValidator(string fieldName, string containingTypeToken, params string[] listValueTokens) {
            this.fieldName = fieldName;
            this.containingTypeToken = containingTypeToken;
            this.listValueTokens = listValueTokens;
        }
        
        public override bool Validate() {
            Logger.Log(LogMessage.SeverityType.Info, string.Format("Checking custom field for {0}...", containingTypeToken.Equals("Theme") ? "Feature Group" : containingTypeToken));
            
            if(!V1Processor.AttributeExists(containingTypeToken, fieldName)) {
                Logger.Log(LogMessage.SeverityType.Error, string.Format("Custom field {0} is not assigned to type {1}", fieldName, containingTypeToken));
                return false;
            }

            try {
                var availableValues = V1Processor.GetAvailableListValues(containingTypeToken, fieldName);

                if(listValueTokens != null && listValueTokens.Length > 0) {
                    foreach(var token in listValueTokens.Where(token => availableValues.Find(token) == null)) {
                        Logger.Log(LogMessage.SeverityType.Error, string.Format("Possible custom field value '{0}' is not within available values.", token));
                        return false;
                    }
                }
            } catch(VersionOneException ex) {
                Logger.Log(LogMessage.SeverityType.Debug, "Exception during custom field validation: " + ex.Message + "; proceeding.");
                return false;
            }

            Logger.Log(LogMessage.SeverityType.Info, "Custom field check successful.");
            return true;
        }
    }
}