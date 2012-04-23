/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServerConnector {
    internal class AttributeInfo {
        public readonly string Attr;
        public readonly string Prefix;
        public readonly bool IsList;
        public readonly bool IsOptional;

        public AttributeInfo(string attr, string prefix, bool isList, bool isOptional) {
            Attr = attr;
            Prefix = prefix;
            IsList = isList;
            IsOptional = isOptional;
        }

        public override string ToString() {
            return string.Format("{0}.{1} (List:{2}, Optional:{3})", Prefix, Attr, IsList, IsOptional);
        }
    }
}