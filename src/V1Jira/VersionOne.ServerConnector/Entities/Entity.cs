/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using System.Diagnostics;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServerConnector.Entities {
    [DebuggerDisplay("{TypeName} {Name}, Id={Id}")]
    // TODO resolve TypeName and TypeToken clashes, if any
    public abstract class Entity : BaseEntity {
        public const string NameProperty = "Name";
        public const string InactiveProperty = "Inactive";
        public const string CustomPrefix = "Custom_";
        public const string ChangeDateUtcProperty = "ChangeDateUTC";
        public const string CreateDateUtcProperty = "CreateDateUTC";
        public const string SourceProperty = "Source";
        public const string SourceNameProperty = "Source.Name";
        public const string ScopeProperty = "Scope";
        public const string ScopeNameProperty = "Scope.Name";
        public const string ScopeStateProperty = "Scope.AssetState";
        public const string ParentAndUpProperty = "ParentAndUp";
        public const string ScopeParentAndUpProperty = "Scope.ParentMeAndUp";
        public const string StatusProperty = "Status";
        public const string StatusNameProperty = "Status.Name";
        public const string ParentProperty = "Parent";
        public const string CreatedByUsernameProperty = "CreatedBy.Username";

        public string Id { get; protected set; }
        public string TypeName { get; protected set; }

        protected internal IDictionary<string, PropertyValues> ListValues { get; set; }
        protected internal IEntityFieldTypeResolver TypeResolver;

        internal Entity(Asset asset, IEntityFieldTypeResolver typeResolver) : base(asset) {
            Id = asset.Oid.Momentless.ToString();
            TypeName = asset.AssetType.Token;
            TypeResolver = typeResolver;
        }

        protected Entity() { }

        public virtual bool HasChanged() {
            return Asset.HasChanged;
        }

        public string GetCustomFieldValue(string fieldName) {
            fieldName = NormalizeCustomFieldName(fieldName);
            var value = GetProperty<object>(fieldName);

            if (value != null && value is Oid && ((Oid)value).IsNull) {
                return null;
            }

            return value != null ? value.ToString() : null;
        }

        public ValueId GetListValue(string fieldName) {
            var value = GetProperty<Oid>(fieldName);
            var type = TypeResolver.Resolve(TypeToken, fieldName);
            return ListValues[type].Find(value.Token);            
        }

        public ValueId GetCustomListValue(string fieldName) {
            fieldName = NormalizeCustomFieldName(fieldName);
            return GetListValue(fieldName);
        }

        public void SetListValue(string fieldName, string value) {
            var type = TypeResolver.Resolve(TypeToken, fieldName);
            var valueData = ListValues[type].Find(value);

            if (valueData != null) {
                SetProperty(fieldName, valueData.Oid);
            }
        }

        private static string NormalizeCustomFieldName(string fieldName) {
            return fieldName.StartsWith(CustomPrefix) ? fieldName : CustomPrefix + fieldName;
        }

        public void SetCustomNumericValue(string fieldName, double value) {
            SetProperty(fieldName, value);
        }

        public double? GetCustomNumericValue(string fieldName) {
            return GetProperty<double?>(fieldName);
        }
    }
}