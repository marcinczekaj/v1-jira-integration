/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServiceHost.WorkitemServices {
    /// <summary>
    /// What gets returned when we attempt to create a Workitem to match a workitem in an external system.
    /// </summary>
    public class IssueCreatedResult : WorkitemUpdateResult {
        public IssueCreatedResult(Workitem source)
        {
            Source = source;
        }

        public Workitem Source { get; private set; }
        public string Permalink { get; set; }
    }
}