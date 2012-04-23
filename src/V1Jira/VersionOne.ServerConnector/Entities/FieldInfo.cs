/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.SDK.APIClient;

namespace VersionOne.ServerConnector.Entities {
    public class FieldInfo : BaseEntity {       
        public const string AssetTypeProperty = "AssetType";
        public const string IsReadOnlyProperty = "IsReadOnly";
        public const string IsRequiredProperty = "IsRequired";
        public const string NameProperty = "Name";
        public const string AttributeTypeProperty = "AttributeType";

        internal FieldInfo(Asset asset) : base(asset) {
        }        

        public string Name {
            get { return GetProperty<string>(NameProperty); }
        }

        public string AttributeType {
            get { return GetProperty<string>(AttributeTypeProperty); }
        }

        public string AssetType {
            get { return GetProperty<string>(AssetTypeProperty); }
        }

        public bool IsReadOnly {
            get { return GetProperty<bool>(IsReadOnlyProperty); }
        }

        public bool IsRequired {
            get { return GetProperty<bool>(IsRequiredProperty); }
        }
    }
}