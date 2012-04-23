/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServiceHost.WorkitemServices {

    public enum WorkitemFailureReason { 
        NoSuchProject
    }
    
    
    /// <summary>
    /// What gets returned when we attempt to create a Workitem and fail, we want to set this failure status in Jira.
    /// </summary>
    public class WorkitemCreationFailureResult : WorkitemUpdateResult {
        public WorkitemCreationFailureResult(Workitem source )
        {
            Source = source;
        }

        public Workitem Source { get; private set; }
        public WorkitemFailureReason Reason { get; set; }
    }
}