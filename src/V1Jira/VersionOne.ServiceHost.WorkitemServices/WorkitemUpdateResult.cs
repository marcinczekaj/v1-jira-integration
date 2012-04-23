/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using System.Text;

namespace VersionOne.ServiceHost.WorkitemServices {
    public class WorkitemUpdateResult {
        protected WorkitemUpdateResult() {
            Warnings = new List<string>();
            Messages = new List<string>();
            FieldUpdates = new Dictionary<string, string>();
        }

        public List<string> Warnings { get; private set; }
        public List<string> Messages { get; private set; }
        public Dictionary<string, string> FieldUpdates { get; set; }

        public string WorkitemId { get; set; }

        public override string ToString() {
            var warningBuffer = new StringBuilder();

            foreach(var warningValue in Warnings) {
                warningBuffer.AppendLine(warningValue);
            }
            var messageBuffer = new StringBuilder();

            foreach(var messageValue in Messages) {
                messageBuffer.AppendLine(messageValue);
            }

            var updatePropertiesBuffer = new StringBuilder();
            foreach (var pair in FieldUpdates) {
                updatePropertiesBuffer.AppendLine(string.Format("{0}=>{1}", pair.Key, pair.Value));
            }

            return string.Format("{0}\n\tWorkitem Id: {1}\n\tWarnings: {2}\n\tMessages: {3}\nField Updates: {4}", base.ToString(), WorkitemId, warningBuffer, messageBuffer, updatePropertiesBuffer);
        }
    }
}