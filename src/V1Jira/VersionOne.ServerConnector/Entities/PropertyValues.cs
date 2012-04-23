/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServerConnector.Entities {
    public class PropertyValues : IEnumerable<ValueId> {
        private readonly IDictionary<Oid, ValueId> dictionary = new Dictionary<Oid, ValueId>();

        public PropertyValues(IEnumerable<ValueId> valueIds) {
            foreach (var id in valueIds) {
                Add(id);
            }
        }

        public PropertyValues() { }

        public IEnumerator<ValueId> GetEnumerator() {
            return dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            return string.Join(", ", this.Select(item => item.ToString()).ToArray());
        }

        public virtual ValueId Find(string token) {
            return dictionary.Where(pair => token.Equals(pair.Key.Momentless.Token)).Select(pair => pair.Value).FirstOrDefault();
        }

        public virtual ValueId FindByName(string name) {
            return dictionary.Where(pair => string.Equals(pair.Value.Name, name)).Select(pair => pair.Value).FirstOrDefault();
        }

        internal void Add(ValueId value) {
            dictionary.Add(value.Oid, value);
        }
    }
}