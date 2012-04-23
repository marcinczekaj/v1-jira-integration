/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Linq;
using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServerConnector.Entities {
    public class FeatureGroup : Workitem {
        public IList<Workitem> Children {get; protected set;}
        
        public override string TypeToken {
            get { return VersionOneProcessor.FeatureGroupType; }
        }

        protected FeatureGroup() { }

        protected internal FeatureGroup(Asset asset, IDictionary<string, PropertyValues> listValues, IList<Workitem> children, IList<Member> owners, IEntityFieldTypeResolver typeResolver)
                : base(asset, listValues, owners, typeResolver) {
            Children = children;
        }

        public override bool HasChanged() {
            return Asset.HasChanged || Children.Any(x => x.HasChanged());
        }
    }
}