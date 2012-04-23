/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.SDK.APIClient;

namespace VersionOne.ServerConnector.Entities {
    public class ValueId {
        internal readonly Oid Oid;
        private readonly string name;

        public string Name { get { return name; } }
        public string Token { get { return Oid.Token; } }

        public ValueId() : this(Oid.Null, string.Empty) { }

        protected internal ValueId(Oid oid, string name) {
            Oid = oid.Momentless;
            this.name = name;
        }

        public override string ToString() {
            return name;
        }

        public override bool Equals(object obj) {
            if (obj == null || (obj.GetType() != typeof(ValueId) && obj.GetType().Name != "TestValueId")) {
                return false;
            }

            if(ReferenceEquals(this, obj)) {
                return true;
            }

            var other = (ValueId) obj;
            return Equals(Oid, other.Oid);
        }

        public override int GetHashCode() {
            return Oid.GetHashCode();
        }
    }
}