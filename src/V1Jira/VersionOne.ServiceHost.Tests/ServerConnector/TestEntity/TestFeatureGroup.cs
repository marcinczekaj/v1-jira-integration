using System.Linq;
using System.Collections.Generic;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;

namespace VersionOne.ServiceHost.Tests.ServerConnector.TestEntity {
    internal class TestFeatureGroup : FeatureGroup {
        private const string FeatureGroupTypeName = "Theme";
        private bool changed;

        private readonly IDictionary<string, object> properties = new Dictionary<string, object>();

        public TestFeatureGroup(string id, IDictionary<string, PropertyValues> listValues, IList<Workitem> children, IEntityFieldTypeResolver typeResolver) : this(id, listValues, typeResolver) {
            Children = children;
        }

        private TestFeatureGroup(string id, IDictionary<string, PropertyValues> listValues, IEntityFieldTypeResolver typeResolver) {
            Id = id;
            TypeName = FeatureGroupTypeName;
            ListValues = listValues;
            TypeResolver = typeResolver;
        }

        public TestFeatureGroup Set(string propertyName, object value) {
            SetProperty(propertyName, value);
            changed = false;
            return this;
        }

        public new IList<Member> Owners { 
            set { base.Owners = value; }
        }

        protected override T GetProperty<T>(string name) {
            return properties.ContainsKey(name) ? (T) properties[name] : default(T);
        }

        protected override void SetProperty<T>(string name, T value) {
            if(!properties.ContainsKey(name)) {
                properties.Add(name, value);
            } else {
                properties[name] = value;
            }
            changed = true;
        }

        public override bool HasChanged() {
            return changed || Children.Any(x => x.HasChanged());
        }
    }
}