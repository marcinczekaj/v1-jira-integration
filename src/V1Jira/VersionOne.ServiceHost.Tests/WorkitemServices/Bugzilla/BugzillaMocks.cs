using Rhino.Mocks;
using VersionOne.Bugzilla.XmlRpcProxy;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.Tests.WorkitemServices.Bugzilla
{
	/// <summary>
	/// A convenient place to create and store all of the mocks and stubs needed for testing.
	/// </summary>
	public class BugzillaMocks
	{
		public BugzillaMocks()
		{
			repository = new MockRepository();
			client = repository.StrictMock<IBugzillaClient>();
			serviceFactory = repository.DynamicMock<IBugzillaClientFactory>();
			logger = repository.Stub<ILogger>();
		}

		private MockRepository repository;
		public MockRepository Repository
		{
			get { return repository; }
		}

		private IBugzillaClient client;
		public IBugzillaClient Client
		{
			get { return client; }
		}

		private IBugzillaClientFactory serviceFactory;
		public IBugzillaClientFactory ServiceFactory
		{
			get { return serviceFactory; }
		}

		private ILogger logger;
		public ILogger Logger
		{
			get { return logger; }
		}
	}
}
