/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServiceHost.WorkitemServices {
    public class Story : Workitem {
        public Story(string title, string description, string project, string owners, string priority) : base(title, description, project, owners, priority) { }

        public Story() {}

        public override string Type { get { return "Story"; } }
    }
}