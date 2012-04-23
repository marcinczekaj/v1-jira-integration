/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Diagnostics;

namespace VersionOne.ServiceHost.Core.Configuration {
    [DebuggerDisplay("Mapping (Id={Id}, Name={Name})")]
    public class MappingInfo {
        public readonly string Id;
        public readonly string Name;

        public MappingInfo(string id, string name) {
            Id = id;
            Name = name;
        }

        public bool IsNullOrEmpty() {
            return string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(Name);
        }

        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != typeof(MappingInfo)) {
                return false;
            }

            var other = (MappingInfo)obj;
            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name);
        }

        public override int GetHashCode() {
            int code = 31;
            code += Id != null ? Id.GetHashCode() : 0;
            code += Name != null ? Name.GetHashCode() : 0;

            return code;
        }
    }
}
