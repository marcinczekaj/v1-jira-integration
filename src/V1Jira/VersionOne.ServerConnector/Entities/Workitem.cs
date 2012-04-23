/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Linq;
using System.Collections.Generic;
using VersionOne.SDK.APIClient;
using System.Collections;

namespace VersionOne.ServerConnector.Entities {
    public abstract class Workitem : Entity {
        public const string AssetTypeProperty = "AssetType";
        public const string NumberProperty = "Number";
        public const string EstimateProperty = "Estimate";
        public const string PriorityProperty = "Priority";
        public const string ParentNameProperty = "Parent.Name";
        public const string TeamNameProperty = "Team.Name";
        public const string SprintNameProperty = "Timebox.Name";
        public const string DescriptionProperty = "Description";
        public const string EnvironmentProperty = "Environment";
        public const string OrderProperty = "Order";
        public const string ReferenceProperty = "Reference";
        public const string OwnersProperty = "Owners";
        public const string OwnersUsernamesProperty = "Owners.Username";
        public const string FoundInBuildProperty = "FoundInBuild";
        public const string SeverityLevelProperty = "Custom_SeverityLevel";

        public string Number { get { return GetProperty<string>(NumberProperty); } }
        
        //wstawione przez Gle
        public string StatusName {
            get { return GetProperty<string>(StatusNameProperty); }
            set { SetProperty(StatusNameProperty, value); }
        }

        public string Name {
            get { return GetProperty<string>(NameProperty); }
            set { SetProperty(NameProperty, value); }
        }

        public string Description {
            get { return GetProperty<string>(DescriptionProperty); }
            set { SetProperty(DescriptionProperty, value); }
        }

        public string Environment // move to defect
        {
            get { return GetProperty<string>(EnvironmentProperty); }
            set { SetProperty(EnvironmentProperty, value); }
        }

        public string Reference {
            get { return GetProperty<string>(ReferenceProperty); }
            set { SetProperty(ReferenceProperty, value); }
        }

        public double? Estimate {
            get { return GetProperty<double?>(EstimateProperty); }
            set { SetProperty(EstimateProperty, value);}
        }

        public DateTime ChangeDateUtc {
            get { return GetProperty<DateTime>(ChangeDateUtcProperty); }
            set { SetProperty(ChangeDateUtcProperty, value); }
        }

        public DateTime CreateDateUtc
        {
            get { return GetProperty<DateTime>(CreateDateUtcProperty); }
            set { SetProperty(CreateDateUtcProperty, value); }
        }

        public string CreatedByUsername {
            get { return GetProperty<string>(CreatedByUsernameProperty); }
            set { SetProperty(CreatedByUsernameProperty, value); }
        }
        
        public string PriorityToken {
            get {
                var oid = GetProperty<Oid>(PriorityProperty);
                return oid.IsNull ? null : oid.Momentless.Token;
            }
            set {
                var priority = ListValues[VersionOneProcessor.WorkitemPriorityType].Find(value);
                if (priority != null) {
                    SetProperty(PriorityProperty, priority.Oid);
                }
            }
        }

        public string Priority {
            get {
                var oid = GetProperty<Oid>(PriorityProperty);
                var listValue = ListValues[VersionOneProcessor.WorkitemPriorityType].Find(oid.Momentless.Token);
                return listValue == null ? null : listValue.Name;
            }
            set {
                var priority = ListValues[VersionOneProcessor.WorkitemPriorityType].FindByName(value);
                if (priority != null) {
                    SetProperty(PriorityProperty, priority.Oid);
                }
            }
        }

        public string Status
        {
            get
            {
                var oid = GetProperty<Oid>(StatusProperty);
                var listValue = ListValues[VersionOneProcessor.WorkitemStatusType].Find(oid.Momentless.Token);
                return listValue == null ? null : listValue.Name;
            }
            set
            {
                var status = ListValues[VersionOneProcessor.WorkitemStatusType].FindByName(value);
                if (status != null)
                {
                    SetProperty(StatusProperty, status.Oid);
                }
            }
        }

        public string Source
        {
            get
            {
                var oid = GetProperty<Oid>(SourceProperty);
                var listValue = ListValues[VersionOneProcessor.WorkitemSourceType].Find(oid.Momentless.Token);
                return listValue == null ? null : listValue.Name;
            }
            set
            {
                var source = ListValues[VersionOneProcessor.WorkitemSourceType].FindByName(value);
                if (source != null)
                {
                    SetProperty(SourceProperty, source.Oid);
                }
            }
        }
        public List<string> OwnersUsernames {
            get {
                IList names = GetMultiProperty(OwnersUsernamesProperty);
                return (names != null ? names.OfType<string>().ToList<string>() : null);
            }
            set { }
        }

        public IList<Member> Owners { get; protected set; }

        public KeyValuePair<string, string> Project {
            get { return new KeyValuePair<string, string>(GetProperty<Oid>(ScopeProperty).Momentless.ToString(), GetProperty<string>(ScopeNameProperty)); }
        }

        internal Workitem(Asset asset, IDictionary<string, PropertyValues> listValues, IList<Member> owners, IEntityFieldTypeResolver typeResolver) 
                : this(asset, listValues, typeResolver) {
            Owners = owners;
        }

        internal Workitem(Asset asset, IDictionary<string, PropertyValues> listValues, IEntityFieldTypeResolver typeResolver) : this(asset, typeResolver) {
            ListValues = listValues;
        }

        private Workitem(Asset asset, IEntityFieldTypeResolver typeResolver) : base(asset, typeResolver) {}

        protected Workitem() { }

        internal static Workitem Create(Asset asset, IDictionary<string, PropertyValues> listPropertyValues, IEntityFieldTypeResolver typeResolver) {
            switch(asset.AssetType.Token) {
                case VersionOneProcessor.StoryType:
                    return new Story(asset, listPropertyValues, typeResolver);
                case VersionOneProcessor.DefectType:
                    return new Defect(asset, listPropertyValues, typeResolver);
                default:
                    throw new NotSupportedException("Type " + asset.AssetType.Token + " is not supported in factory method");
            }
        }
    }
}