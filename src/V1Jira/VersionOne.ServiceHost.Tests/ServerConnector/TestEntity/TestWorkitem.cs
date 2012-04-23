using System.Collections.Generic;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServiceHost.Tests.ServerConnector.TestEntity {
    internal class TestWorkitem : PrimaryWorkitem {
        internal readonly IDictionary<string, object> Properties = new Dictionary<string, object>();

        public TestWorkitem(Asset asset, IDictionary<string, PropertyValues> listValues)
            : base(asset, listValues, null) {
        }

        public TestWorkitem(string id, string typeName, IDictionary<string, PropertyValues> listValues) : this(id, typeName, listValues, null) {
        }

        public TestWorkitem(string id, string typeName, IDictionary<string, PropertyValues> listValues, IEntityFieldTypeResolver typeResolver) {
            Id = id;
            TypeName = typeName;
            ListValues = listValues;
            TypeResolver = typeResolver;
        }

        public TestWorkitem(string id, string typeName) : this(id, typeName, null) {
        }

        public TestWorkitem Set(string propertyName, object value) {
            SetProperty(propertyName, value);
            return this;
        }

        protected override T GetProperty<T>(string name) {
            return Properties.ContainsKey(name) ? (T) Properties[name] : default(T);
        }

        protected override void SetProperty<T>(string name, T value) {
            if(!Properties.ContainsKey(name)) {
                Properties.Add(name, value);
            } else {
                Properties[name] = value;
            }
        }
    }
}