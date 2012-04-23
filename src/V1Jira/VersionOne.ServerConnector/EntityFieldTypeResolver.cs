/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using System.Linq;

namespace VersionOne.ServerConnector {
    public class EntityFieldTypeResolver : IEntityFieldTypeResolver {
        internal readonly IDictionary<string, string> FieldMappings = new Dictionary<string, string>();

        public void AddMapping(string entityType, string fieldName, string resolvedTypeName) {
            FieldMappings.Add(GetKey(entityType, fieldName), resolvedTypeName);
        }

        public string Resolve(string entityType, string fieldName) {
            var key = GetKey(entityType, fieldName);
            return FieldMappings.ContainsKey(key) ? FieldMappings[key] : null;
        }

        public void Reset() {
            var keys = FieldMappings.Keys.ToList();

            foreach(var key in keys) {
                FieldMappings[key] = null;
            }
        }

        private static string GetKey(string entityType, string fieldName) {
            return entityType + "." + fieldName;
        }
    }
}