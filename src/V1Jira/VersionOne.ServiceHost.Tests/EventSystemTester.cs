using System;
using NUnit.Framework;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.Core.Tests
{
	[TestFixture]
	public class EventSystemTester
	{
		private string _lastheard;
		private int _heardcount;
		
		[SetUp] public void Setup()
		{
			_lastheard = null;
			_heardcount = 0;
		}
		
		[Test]
		public void Basic()
		{
			IEventManager mgr = new EventManager();
			mgr.Subscribe(typeof(string),BasicListener);
			mgr.Publish("Some Basic String");
			Assert.AreEqual(1,_heardcount);
			Assert.AreEqual("Some Basic String",_lastheard);
		}

		[Test]
		public void MultipleCalls()
		{
			IEventManager mgr = new EventManager();
			mgr.Subscribe(typeof(string), BasicListener);
			mgr.Subscribe(typeof(string), BasicListener);
			mgr.Publish("Some Basic String");
			Assert.AreEqual(2, _heardcount);
			Assert.AreEqual("Some Basic String", _lastheard);			
		}
		
		[Test]
		public void PubTwiceSubOnce()
		{
			IEventManager mgr = new EventManager();
			mgr.Subscribe(typeof(string), BasicListener);
			mgr.Publish("Some Basic String 1");
			mgr.Publish("Some Basic String 2");
			Assert.AreEqual(2, _heardcount);
		}

		[Test]
		public void PubTwiceSubTwice()
		{
			IEventManager mgr = new EventManager();
			mgr.Subscribe(typeof(string), BasicListener);
			mgr.Subscribe(typeof(string), BasicListener);
			mgr.Publish("Some Basic String 1");
			mgr.Publish("Some Basic String 2");
			Assert.AreEqual(4, _heardcount);
		}		

        [Test]
        public void UnsubscribeTest() 
        {
            const string shouldReceive = "This should be received";
            const string shouldNotReceive = "We aren't going to get this one";

            IEventManager mgr = new EventManager();
            mgr.Subscribe(typeof(string), BasicListener);
            mgr.Publish(shouldReceive);
            mgr.Unsubscribe(typeof(string), BasicListener);
            mgr.Publish(shouldNotReceive);

            Assert.AreEqual(_heardcount, 1);
            Assert.AreEqual(_lastheard, shouldReceive);
        }

		private void InterfaceListener(object pubobj)
		{
			Assert.IsInstanceOfType(typeof(ISomeInterface), pubobj);
			_lastheard = pubobj.ToString();
			_heardcount++;			
		}

		private void BasicListener(object pubobj)
		{
			_lastheard = (string) pubobj;
			_heardcount++;
		}

		private interface ISomeInterface { }

		private class SomeClass : ISomeInterface { }
	}
	
	
}
