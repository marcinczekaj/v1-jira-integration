using NUnit.Framework;
using System.Xml;
using VersionOne.ServiceHost.ClearQuestServices;

namespace VersionOne.ServiceHost.Tests.WorkitemServices.ClearQuest
{
	[TestFixture]
	public class ClearQuestHostedServiceTester
	{

		[Test]
		public void TestMandatoryField()
		{
		    const string messageProcessed = "Processed by VersionOne";
		    const string messageClosed = "Was closed in the VersionOne system";

			string mandatoryFields = string.Format("<ClearQuestMandatoryFields><!--- testing --><Field name=\"Description\">{0}</Field><Field name=\"Reason\">{1}</Field></ClearQuestMandatoryFields>", 
                messageProcessed, messageClosed);
			ClearQuestServiceConfiguration config = new ClearQuestServiceConfiguration();
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(mandatoryFields);
			ClearQuestHostedService service = new ClearQuestHostedService();

			service.ProcessMandatoryFieldsSettings(doc.DocumentElement, config);

			Assert.AreEqual(messageProcessed, config.MandatoryFields["Description"]);
			Assert.AreEqual(messageClosed, config.MandatoryFields["Reason"]);
		}
	}
}
