/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections;
using System.Collections.Generic;
namespace VersionOne.ServiceHost.JiraServices {
    public class Issue {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Project { get; set; }
        public string IssueType { get; set; }
        public string Assignee { get; set; }
        public string Priority { get; set; }
        public string Environment { get; set; }
        public string Reporter { get; set; }
        public string AffectsVersion { get; set; }
        public IDictionary<string, string> CustomFields = new Dictionary<string, string>();

        public string GetCustomField(string id) {
            return CustomFields.ContainsKey(id) ? CustomFields[id] : null;
        }
        
        public override string ToString() {
            return string.Format("[{0}]: {1}", Key, Summary);
        }

        public System.DateTime? Updated { get; set; }
        public System.DateTime? Created { get; set; }
        public string ProjectKey { get; set; }
        public string Status { get; set; }
    }
}