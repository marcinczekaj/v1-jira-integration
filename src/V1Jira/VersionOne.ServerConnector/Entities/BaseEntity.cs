/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.SDK.APIClient;
using System.Collections;

namespace VersionOne.ServerConnector.Entities {
    public abstract class BaseEntity {
        internal readonly Asset Asset;

        public virtual string TypeToken { get { return Asset.AssetType.Token; } }

        internal BaseEntity(Asset asset) {
            Asset = asset;
        }

        protected BaseEntity() { }

        protected virtual T GetProperty<T>(string name) {
            var attributeDefinition = Asset.AssetType.GetAttributeDefinition(name);
            return (T) (Asset.GetAttribute(attributeDefinition) != null ? Asset.GetAttribute(attributeDefinition).Value : null);
        }

        protected IList GetMultiProperty(string name) {
            var attributeDefinition = Asset.AssetType.GetAttributeDefinition(name);
            return (Asset.GetAttribute(attributeDefinition) != null ? Asset.GetAttribute(attributeDefinition).ValuesList : null);
        }

        protected virtual void SetProperty<T>(string name, T value) {
            var attributeDefinition = Asset.AssetType.GetAttributeDefinition(name);
            Asset.SetAttributeValue(attributeDefinition, value);
        }
    }
}