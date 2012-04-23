using System.Collections.Generic;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;

namespace VersionOne.ServiceHost.Tests.ServerConnector.TestEntity {
    public class TestStory : Story {
        private readonly IDictionary<string, object> properties = new Dictionary<string, object>();
        private bool changed;

        public TestStory(string id, IDictionary<string, PropertyValues> listValues, IEntityFieldTypeResolver typeResolver) : base() {
            Id = id;
            TypeName = VersionOneProcessor.StoryType;
            ListValues = listValues;
            TypeResolver = typeResolver;
        }

        public TestStory(string id, IEntityFieldTypeResolver typeResolver) : this(id, null, typeResolver) { }

        public TestStory Set(string propertyName, object value) {
            SetProperty(propertyName, value);
            changed = false;
            return this;
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
            return changed;
        }
    }
}