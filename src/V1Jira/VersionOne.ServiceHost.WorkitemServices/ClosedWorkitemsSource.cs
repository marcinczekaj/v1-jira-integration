/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServiceHost.WorkitemServices {
    public class ClosedWorkitemsSource {
        public ClosedWorkitemsSource(string sourceValue) {
            SourceValue = sourceValue;
        }

        public string SourceValue { get; private set; }
    }
}