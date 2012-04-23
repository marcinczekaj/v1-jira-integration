using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.BZ {
    [TestFixture]
    [Ignore("Tests involve third party systems and require connection settings to be valid.")]
    public class ConnectionValidatorTester 
    {
        [Test]
        public void SvnConnectionValidationTest() 
        {
            const string path = "http://jsdksrv01:1080/svn/Repo3";
            const string user = "user3";
            const string password = "user3";

            SvnServiceEntity entity = new SvnServiceEntity();
            entity.Path = path;
            entity.UserName = user;
            entity.Password = password;
            Assert.IsTrue(Facade.Instance.ValidateConnection(entity));

            entity.Password = "wrong password";
            Assert.IsFalse(Facade.Instance.ValidateConnection(entity));

            entity.Path = "http://example.com/wrong/path";
            Assert.IsFalse(Facade.Instance.ValidateConnection(entity));
        }

        [Test]
        // NOTE that this test requires ClearQuest client with COM libraries registered on the current machine, not only valid credentials
        public void ClearQuestConnectionValidationTest() 
        {
            ClearQuestServiceEntity entity = new ClearQuestServiceEntity();
            entity.UserName = "Admin";
            entity.Password = "";
            entity.ConnectionName = "cq";
            entity.DataBase = "SAMPL";
            
            Assert.IsTrue(Facade.Instance.ValidateConnection(entity));

            entity.UserName = "Invalid user";
            entity.Password = "Invalid password";

            Assert.IsFalse(Facade.Instance.ValidateConnection(entity));
        }

        [Test]
        public void JiraConnectionValidationTest() 
        {
            JiraServiceEntity entity = new JiraServiceEntity();
            entity.Url = "http://integsrv01:8080/rpc/soap/jirasoapservice-v2";
            entity.UserName = "admin";
            entity.Password = "admin";

            Assert.IsTrue(Facade.Instance.ValidateConnection(entity));

            entity.Password = "wrong password";
            Assert.IsFalse(Facade.Instance.ValidateConnection(entity));
        }
 
        [Test]
        public void QualityCenterValidationTest() {
            QCServiceEntity entity = new QCServiceEntity();
            entity.Connection.ApplicationUrl = "http://integsrv01:8081/qcbin";
            entity.Connection.Username = "admin";
            entity.Connection.Password = "admin";

            Assert.IsTrue(Facade.Instance.ValidateConnection(entity));

            entity.Connection.Username = "nobody";
            Assert.IsFalse(Facade.Instance.ValidateConnection(entity));
        }
    }
}