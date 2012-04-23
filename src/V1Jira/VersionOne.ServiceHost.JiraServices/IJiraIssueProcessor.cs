/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using VersionOne.ServiceHost.WorkitemServices;

namespace VersionOne.ServiceHost.JiraServices {
    public interface IJiraIssueProcessor {
        IList<Workitem> GetIssues<T>(string filterId) where T : Workitem, new();
        void OnWorkitemCreated(WorkitemCreationResult createdResult);
        bool OnWorkitemStateChanged(WorkitemStateChangeResult stateChangeResult);
        bool OnWorkitemUpdate(WorkitemStateChangeResult stateChangeResult);

        void OnWorkitemCreationFailure(WorkitemCreationFailureResult creationResult);

        bool OnNewWorkitem(NewVersionOneWorkitem stateChangeResult, string systemName);
    }
}