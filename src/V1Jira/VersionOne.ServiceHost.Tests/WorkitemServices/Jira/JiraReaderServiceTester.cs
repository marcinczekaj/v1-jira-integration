using System;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.JiraServices;
using VersionOne.ServiceHost.JiraServices.Exceptions;
using VersionOne.ServiceHost.WorkitemServices;
using System.Collections.Generic;

namespace VersionOne.ServiceHost.Tests.WorkitemServices.Jira {
    //TODO add test for GetIssues
    [TestFixture]
    public class JiraReaderServiceTester : BaseJiraTester {
        private JiraIssueReaderUpdater reader;
        private JiraServiceConfiguration config;
        private const string ExternalId = "external id";

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            config = new JiraServiceConfiguration {
                OnCreateFieldName = "oncreate_field_name", 
                OnCreateFieldValue = "oncreate_field_value", 
                OnStateChangeFieldName = "onchange_field_name",
                OnStateChangeFieldValue = "onchange_field_value",
                ProgressWorkflow = "workflow 1", 
                WorkitemLinkField = "LinkField", 
                ProgressWorkflowStateChanged = "workflow 2",
                AssigneeStateChanged = "-1",
                Url = Url,
                UserName = Username,
                Password = Password,
            };
            reader = new JiraIssueReaderUpdater(config);
        }

        [Test]
        public void OnWorkitemCreated() {
            var workitem = new Story("Title", "description", "project", "owners", "priority");
            var workitemResult = new WorkitemCreationResult(workitem) {
                Source = { ExternalId = ExternalId, }, 
                Permalink = "link",
            };
            workitemResult.Messages.Add("external id"); ;

            Expect.Call(ServiceFactory.CreateNew(Url)).Repeat.Times(3).Return(SoapService);
            Expect.Call(SoapService.Dispose).Repeat.Times(3);

            FullUpdateJiraIssue(ExternalId, config.OnCreateFieldName, config.OnCreateFieldValue,
                workitemResult.Messages, config.ProgressWorkflow, null);

            UpdateWorkitemLinkInJira(workitemResult);

            Repository.ReplayAll();
            reader.OnWorkitemCreated(workitemResult);
            Repository.VerifyAll();
        }

        private void UpdateWorkitemLinkInJira(WorkitemCreationResult workitemResult) {
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.UpdateIssue(Token, workitemResult.Source.ExternalId, config.WorkitemLinkField,
                workitemResult.Permalink)).Return(null);
            Expect.Call(SoapService.Logout(Token)).Return(true);
        }

        [Test]
        public void OnWorkitemStateChanged() {
            const string workitemId = "D-00001";
            var workitemResult = new WorkitemStateChangeResult(ExternalId, workitemId);
            workitemResult.Messages.Add("message 1");

            Expect.Call(ServiceFactory.CreateNew(Url)).Repeat.Twice().Return(SoapService);
            Expect.Call(SoapService.Dispose).Repeat.Twice();

            FullUpdateJiraIssue(ExternalId, config.OnStateChangeFieldName, config.OnStateChangeFieldValue,
                                workitemResult.Messages, config.ProgressWorkflowStateChanged, config.AssigneeStateChanged);

            Repository.ReplayAll();
            reader.OnWorkitemStateChanged(workitemResult);
            Repository.VerifyAll();            
        }

        private void FullUpdateJiraIssue(string externalId, string fieldName, string fieldValue, List<string> messages, string workflowId, string assignee) {
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.UpdateIssue(Token, externalId, fieldName, fieldValue)).Return(null);
            Expect.Call(() => SoapService.AddComment(Token, externalId, messages[0])).
                Repeat.Once();

            Expect.Call(SoapService.Login(Username, Password)).Return(Token + "1");
            Expect.Call(SoapService.GetAvailableActions(Token + "1", externalId))
                .Return(new List<Item> { new Item(workflowId, "Name") });
            Expect.Call(SoapService.Logout(Token + "1")).Return(true);

            Expect.Call(() => SoapService.ProgressWorkflow(Token, externalId, workflowId, assignee));
            Expect.Call(SoapService.Logout(Token)).Return(true);
        }


        [Test]
        public void OnWorkitemStateChangedWithoutWorkflowProgress() {
            const string workitemId = "D-00001";
            var workitemResult = new WorkitemStateChangeResult(ExternalId, workitemId);
            workitemResult.Messages.Add("message 1");

            Expect.Call(ServiceFactory.CreateNew(Url)).Repeat.Twice().Return(SoapService);
            Expect.Call(SoapService.Dispose).Repeat.Twice();


            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.UpdateIssue(Token, ExternalId, config.OnStateChangeFieldName, config.OnStateChangeFieldValue)).Return(null);
            Expect.Call(() => SoapService.AddComment(Token, ExternalId, workitemResult.Messages[0])).
                Repeat.Once();

            Expect.Call(SoapService.Login(Username, Password)).Return(Token + "1");
            Expect.Call(SoapService.GetAvailableActions(Token + "1", ExternalId))
                .Return(new List<Item>());
            Expect.Call(SoapService.Logout(Token + "1")).Return(true);

            Expect.Call(SoapService.Logout(Token)).Return(true);

            Repository.ReplayAll();
            reader.OnWorkitemStateChanged(workitemResult);
            Repository.VerifyAll();             
        }

        [Test]
        public void OnWorkitemStateChangedWithEmptyData() {
            const string workitemId = "D-00001";
            var workitemResult = new WorkitemStateChangeResult(ExternalId, workitemId);
            var localReader = new JiraIssueReaderUpdater(new JiraServiceConfiguration());

            Expect.Call(ServiceFactory.CreateNew(null)).IgnoreArguments().Repeat.Once().Return(SoapService);
            Expect.Call(SoapService.Dispose).Repeat.Once();


            Expect.Call(SoapService.Login(null, null)).Return(Token);
            Expect.Call(SoapService.Logout(Token)).Return(true);

            Repository.ReplayAll();
            localReader.OnWorkitemStateChanged(workitemResult);
            Repository.VerifyAll();               
        }

        [Test]
        public void OnWorkitemCreatedInsufficientPermissions() {
            var workitem = new Story("Title", "description", "project", "owners", "priority");
            var workitemResult = new WorkitemCreationResult(workitem) {
                Source = { ExternalId = ExternalId, },
                Permalink = "link",
            };
            workitemResult.Messages.Add("external id"); ;

            Expect.Call(ServiceFactory.CreateNew(Url)).Repeat.Twice().Return(SoapService);
            Expect.Call(SoapService.Dispose).Repeat.Twice();

            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.UpdateIssue(Token, ExternalId, config.OnCreateFieldName, config.OnCreateFieldValue)).Throw(new JiraException("Can't update issue", new Exception()));
            Expect.Call(SoapService.Logout(Token)).Return(true);

            UpdateWorkitemLinkInJira(workitemResult);

            Repository.ReplayAll();
            reader.OnWorkitemCreated(workitemResult);
            Repository.VerifyAll();
        }
    }
}