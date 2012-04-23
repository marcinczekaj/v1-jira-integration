/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServerConnector.Entities {
    public class WorkitemFromExternalSystem : Workitem {
        private readonly string externalIdFieldName;

        public WorkitemFromExternalSystem(Workitem item, string externalIdFieldName)
            : base(item.Asset, item.ListValues, item.TypeResolver) {
            this.externalIdFieldName = externalIdFieldName;
        }

        public string ExternalId {
            get { return GetProperty<string>(externalIdFieldName); }
        }
    }
}