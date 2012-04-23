/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServiceHost.JiraServices {
    public class Item {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public Item(string id, string name) {
            Id = id;
            Name = name;
        }

        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != typeof(Item)) {
                return false;
            }

            var other = (Item)obj;
            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name);
        }

        public override int GetHashCode() {
            var code = 31;
            code += Id != null ? Id.GetHashCode() : 0;
            code += Name != null ? Name.GetHashCode() : 0;

            return code;
        }

        public override string ToString() {
            return string.Format("'{0}' ({1})", Name, Id);
        }
    }
}