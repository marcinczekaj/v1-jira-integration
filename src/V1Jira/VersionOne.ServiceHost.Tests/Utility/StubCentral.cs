using VersionOne.SDK.APIClient;
using VersionOne.ServiceHost.Tests.Utility;

namespace VersionOne.ServiceHost.Tests
{
	public abstract class BaseStubCentral : ICentral
	{
		protected virtual string ServicesKeys { get { return null; } }
		protected virtual string MetaKeys { get { return null; } }
		protected virtual string LocKeys { get { return null; } }

		IServices ICentral.Services { get { return Services; } }

		private ServicesWrapper _services;		
		internal ServicesWrapper Services
		{
			get
			{
				if (_services == null)
					_services = new ServicesWrapper(new Services(MetaModel, ServicesConnector));
				return _services;				
			}
		}

		private XmlResponseConnector _servicesconnector;
		internal XmlResponseConnector ServicesConnector
		{
			get
			{
				if (_servicesconnector == null)
					_servicesconnector = new XmlResponseConnector("TestData.xml", "rest-1.v1/", ServicesKeys);
				return _servicesconnector;
			}
		}

		private IMetaModel _metamodel;
		public IMetaModel MetaModel
		{
			get
			{
				if (_metamodel == null)
					_metamodel = new MetaModel(MetaModelConnector);
				return _metamodel;
			}
		}

		private XmlResponseConnector _metamodelconnector;
		internal XmlResponseConnector MetaModelConnector
		{
			get
			{
				if (_metamodelconnector == null)
					_metamodelconnector = new XmlResponseConnector("TestData.Xml", "meta.v1/", MetaKeys);
				return _metamodelconnector;
			}
		}

		private ILocalizer _loc;
		public ILocalizer Loc
		{
			get
			{
				if (_loc == null)
					_loc = new Localizer(LocConnector);
				return _loc;
			}
		}

		private TextResponseConnector _locconnector;
		internal TextResponseConnector LocConnector
		{
			get
			{
				if (_locconnector == null)
					_locconnector = new TextResponseConnector("TestData.Xml", "loc.v1/", LocKeys);
				return _locconnector;
			}
		}
	}
}