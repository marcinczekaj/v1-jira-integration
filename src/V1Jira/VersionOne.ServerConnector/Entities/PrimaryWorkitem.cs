/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServerConnector.Entities {
    public class PrimaryWorkitem : Workitem {


        public string FeatureGroupName { get { return GetProperty<string>(ParentNameProperty); } }
        public string Team { get { return GetProperty<string>(TeamNameProperty); } }
        public string SprintName { get { return GetProperty<string>(SprintNameProperty); } }

        public int Order {
            get {
                int order;
                int.TryParse(GetProperty<Rank>(OrderProperty).ToString(), out order);
                return order;
            }
        }


        public override string TypeToken {
            get { return VersionOneProcessor.PrimaryWorkitemType; }
        }

        internal protected PrimaryWorkitem(Asset asset, IDictionary<string, PropertyValues> listValues, IEntityFieldTypeResolver typeResolver) : base(asset, listValues, typeResolver) { }

        internal protected PrimaryWorkitem() { }
    }
}