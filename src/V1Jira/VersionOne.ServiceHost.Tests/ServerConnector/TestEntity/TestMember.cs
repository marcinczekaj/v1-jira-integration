using System.Collections.Generic;
using VersionOne.ServerConnector.Entities;

namespace VersionOne.ServiceHost.Tests.ServerConnector.TestEntity {
    public class TestMember : Member {
        internal readonly IDictionary<string, object> Properties = new Dictionary<string, object>();

        public TestMember(string id, IDictionary<string, PropertyValues> listValues) {
            Id = id;
            TypeName = "Member";
            ListValues = listValues;
        }

        public TestMember(string id) : this(id, null) { }

        public TestMember Set(string propertyName, object value) {
            SetProperty(propertyName, value);
            return this;
        }

        protected override T GetProperty<T>(string name) {
            return Properties.ContainsKey(name) ? (T)Properties[name] : default(T);
        }

        protected override void SetProperty<T>(string name, T value) {
            if (!Properties.ContainsKey(name)) {
                Properties.Add(name, value);
            } else {
                Properties[name] = value;
            }
        }
    }
}