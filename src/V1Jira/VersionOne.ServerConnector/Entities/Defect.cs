/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using System.Diagnostics;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServerConnector.Entities {
    [DebuggerDisplay("{TypeName} {Name}, Id={Id}, Number={Number}")]
    public class Defect : PrimaryWorkitem {
        public override string TypeToken {
            get { return VersionOneProcessor.DefectType; }
        }

        public string FoundInBuild {
            get { return GetProperty<string>(FoundInBuildProperty); }
            set { SetProperty(FoundInBuildProperty, value); }
        }

        public string SeverityLevel {
            get {
                var oid = GetProperty<Oid>(SeverityLevelProperty);
                var listValue = ListValues[VersionOneProcessor.DefectSeverityType].Find(oid.Momentless.Token);
                return listValue == null ? null : listValue.Name;
            }
            set {
                var severity = ListValues[VersionOneProcessor.DefectSeverityType].FindByName(value);
                if (severity != null) {
                    SetProperty(StatusProperty, severity.Oid);
                }
            }
        }

        protected internal Defect(Asset asset, IDictionary<string, PropertyValues> listValues, IEntityFieldTypeResolver typeResolver) : base(asset, listValues, typeResolver) { }
    }
}