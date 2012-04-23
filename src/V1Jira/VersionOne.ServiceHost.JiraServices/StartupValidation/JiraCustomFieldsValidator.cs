/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using System.Linq;
using VersionOne.ServiceHost.Core.Logging;
using System;
using VersionOne.ServiceHost.JiraServices.Exceptions;

namespace VersionOne.ServiceHost.JiraServices.StartupValidation {
    public class JiraCustomFieldsValidator : BaseValidator {
        private readonly string[] fields;
        private readonly string username;
        private readonly string password;

        public JiraCustomFieldsValidator(string url, string username, string password, params string[] fields)
            : base(url) {
            this.fields = fields;
            this.password = password;
            this.username = username;
        }

        public override bool Validate() {
            var result = true;
            string token = null;

            Logger.Log(LogMessage.SeverityType.Info, "Checking custom fields.");

            using (var service = GetJiraService()) {
                try {
                    token = service.Login(username, password);

                    var customFields = service.GetCustomFields(token).ToList();
                    if(customFields.Count() == 0) {
                        Logger.Log(LogMessage.SeverityType.Error, "JIRA doesn't contain custom fields.");
                        return false;
                    }

                    foreach(var field in fields.Distinct()) {
                        if(string.IsNullOrEmpty(field)) {
                            Logger.Log(LogMessage.SeverityType.Debug, "At least one custom field id is empty.");
                            continue;
                        }

                        Logger.Log(LogMessage.SeverityType.Info, string.Format("Checking {0} field.", field));
                        if(!ValidateField(field, customFields)) {
                            result = false;
                        }
                    }
                } catch (JiraPermissionException ex) {
                    Logger.Log(LogMessage.SeverityType.Error, "You don't have permission to get custom fields: " + ex.Message);
                    return false;
                } catch (Exception) {
                    Logger.Log(LogMessage.SeverityType.Error, "Can't get inforamtion about custom fields.");
                    return false;
                } finally {
                    if (!string.IsNullOrEmpty(token)) {
                        service.Logout(token);
                    }
                }
            }

            Logger.Log(LogMessage.SeverityType.Info, "All fields checked.");
            return result;
        }

        public virtual bool ValidateField(string field, IEnumerable<Item> customFields) {
            var fieldExist = customFields.Any(x => x.Id.Equals(field));
            if(!fieldExist) {
                Logger.Log(LogMessage.SeverityType.Error, string.Format("Field {0} doesn't exist.", field));
                return false;
            }
            return true;
        }
    }
}