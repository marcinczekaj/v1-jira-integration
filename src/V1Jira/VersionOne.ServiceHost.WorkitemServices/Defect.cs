/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServiceHost.WorkitemServices {
    public class Defect : Workitem {
        public Defect(string title, string description, string project, string owners, string priority, string environment)
            : base(title, description, project, owners, priority){}

        public Defect() {}

        public override string Type { get { return "Defect"; } }

        public override string ToString() {
            return base.ToString() + string.Format(" Reporter:{0} VersionAffected:{1} SeverityLevel: {2}", FoundBy, VersionAffected, SeverityLevel);
        }
    }
}
