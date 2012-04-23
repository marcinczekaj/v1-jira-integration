using System;
using System.Collections.Generic;
using System.Xml;
using Ninject;
using VersionOne.Profile;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.SourceServices;
using VersionOne.ServiceHost.SourceServices.Perforce;

namespace VersionOne.ServiceHost.Tests.SourceServices.Perforce {
    internal class TestableP4ReaderService : P4ReaderHostedService {
        internal ChangeSetInfo LastChangeSet { get; private set; }

        protected override int LastChange { get; set; }

        public int LastRevisionNumber {
            get { return LastChange; }
            set { LastChange = value; }
        }

        public void TestOnRevision(P4ChangeSetEventArgs e, string referenceExpression) {
            ReferenceExpression = referenceExpression;
            Connector_NewChange(this, e);
        }

        public void TestProcessChange(int revision, string author, DateTime changeDate, string message, IList<string> filesChanged, string referenceExpression) {
            ReferenceExpression = referenceExpression;
            ProcessChange(revision, author, message, changeDate, filesChanged);
        }

        protected override void PublishChange(ChangeSetInfo changeSet) {
            LastChangeSet = changeSet;
        }

        internal new void Initialize(XmlElement config, IEventManager eventManager, IProfile profile, IKernel container) {
            base.Initialize(config, eventManager, profile);
        }
    }
}