using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.JiraServices;
using VersionOne.ServiceHost.WorkitemServices;

namespace VersionOne.ServiceHost.Tests.WorkitemServices.Jira {
    [TestFixture]
    public class JiraProxyTester : BaseJiraTester {
        private JiraIssueReaderUpdater jiraComponent;

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            var config = new JiraServiceConfiguration { Url = Url, UserName = Username, Password = Password, };
            jiraComponent = new JiraIssueReaderUpdater(config);
        }

        [Test]
        public void GetIssues() {
            const string filterId = "FilterId";
            var remoteIssues = new[] {new Issue {Key = "Id1", Summary = "Name1"}, new Issue {Key = "Id2", Summary = "Name2"}};

            SetupResult.For(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.GetIssuesFromFilter(Token, filterId)).Return(remoteIssues);
            Expect.Call(SoapService.Logout(Token)).Return(true);
            Expect.Call(() => SoapService.Dispose());

            Repository.ReplayAll();
            var items = jiraComponent.GetIssues<Defect>(filterId);

            Repository.VerifyAll();
            Assert.AreEqual(remoteIssues.Length, items.Count);

            foreach(var issue in remoteIssues) {
                Assert.That(items.Select(x => x.ExternalId), Has.Some.EqualTo(issue.Key));
                Assert.That(items.Select(x => x.Title), Has.Some.EqualTo(issue.Summary));
            }
        }
    }
}