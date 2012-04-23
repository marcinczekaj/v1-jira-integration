/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServerConnector {
    public interface IEntityFieldTypeResolver {
        void AddMapping(string entityType, string fieldName, string resolvedTypeName);
        string Resolve(string entityType, string fieldName);
        void Reset();
    }
}