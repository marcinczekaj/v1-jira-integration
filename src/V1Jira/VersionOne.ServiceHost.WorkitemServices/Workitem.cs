/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections;
using System.Collections.Generic;
namespace VersionOne.ServiceHost.WorkitemServices {
//TODO combine with Workitem from ServerConnector
    public abstract class Workitem {
        protected Workitem(string title, string description, string project, string owners, string priority) {
            Title = title;
            Description = description;
            Project = project;
            Owners = owners;
            Priority = priority;
        }

        protected Workitem() {}

        public string ExternalSystemName { get; set; }
        public string ExternalId { get; set; }

        ///<summary> Pemalink to the defect in an external system.</summary>
        public UrlToExternalSystem ExternalUrl { get; set; }

        public string Url { get; set; }

        public string Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Project { get; set; }
        public string ProjectId { get; set; }

        public string CreatedBy { get; set; }

        ///<summary>A comma separated list of owner IDs.</summary>
        public string Owners { get; set; }

        //Defect params
        public string FoundBy { get; set; }
        public string VersionAffected { get; set; }
        public string Source { get; set; }

        public string Priority { get; set; }
        public string Environment { get; set; }
        public string BuildNumber { get; set; }
        public string SeverityLevel { get; set; }
        public abstract string Type { get; }

        public string Status { get; set; }


        public System.DateTime? Created { get; set; }
        public System.DateTime? Updated { get; set; }

        public override string ToString() {
            return string.Format("[ Number:{0}/ExternalId:{1} ] Title:{2} Priority:{3} Environment:{4} BuildNumber: {5} Owners:{6}", Number, ExternalId, Title, Priority, Environment, BuildNumber, Owners);
        }
    }
}