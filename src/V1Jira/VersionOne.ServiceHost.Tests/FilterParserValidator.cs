/*(c) Copyright 2008, VersionOne, Inc. All rights reserved. (c)*/
using System.Xml;
using NUnit.Framework;
using VersionOne.APIClient;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Utility;

namespace VersionOne.ServiceHost.Tests {

	[TestFixture]
	public class FilterParserValidator {
		private XmlDocument _document;
		private IMetaModel _metaModel;

		[SetUp]
		public void setup()
		{
			_document = new XmlDocument();
			_metaModel = new MetaModel(new XmlResponseConnector("TestData.Xml", "meta.v1/", "FilterParserValidator"));
		}

		[Test]
		public void shouldReturnNullWhenElementIsEmpty() {
			XmlElement element = _document.CreateElement("QueryFilter");

			FilterParser parser = new FilterParser(_metaModel.GetAssetType("Test"));
			IFilterTerm term = parser.parse(element);
			Assert.IsNull(term);
		}

		[Test]
		public void shouldThrowConfigurationExceptionWhenElementIsInvalid()
		{
			XmlDocument document = new XmlDocument();
			XmlElement element = document.CreateElement("NotWhatWeExpect");

			FilterParser parser = new FilterParser(_metaModel.GetAssetType("Test"));

			try
			{
				parser.parse(element);	
				Assert.Fail("Expected Configuration Exception");
			}
			catch (ConfigurationException){}			
		}

		[Test]
		public void shouldThrowConfigurationExceptionWhenFilterTermElementIsInvalid() {
			XmlDocument document = new XmlDocument();
			XmlElement termElement = document.CreateElement("NotWhatWeExpectTerm");
			termElement.SetAttribute("attribute", "Name");
			termElement.SetAttribute("operator", "=");
			termElement.SetAttribute("value", "Fred");

			XmlElement filterElement = document.CreateElement("QueryFilter");
			filterElement.AppendChild(termElement);

			FilterParser parser = new FilterParser(_metaModel.GetAssetType("Test"));

			try 
			{
				parser.parse(filterElement);
				Assert.Fail("Expected Configuration Exception");
			}
			catch (ConfigurationException) {}
		}

		[Test]
		public void shouldReturnValidFilterTermWhenOnlyOneTermIsPresent()
		{
			XmlDocument document = new XmlDocument();
			
			XmlElement termElement = document.CreateElement("FilterTerm");
			termElement.SetAttribute("attribute", "Name");
			termElement.SetAttribute("operator", "=");
			termElement.SetAttribute("value", "Fred");

			XmlElement filterElement = document.CreateElement("QueryFilter");
			filterElement.AppendChild(termElement);

			FilterParser parser = new FilterParser(_metaModel.GetAssetType("Test"));
			IFilterTerm term = parser.parse(filterElement);
			Assert.IsNotNull(term);
			Assert.AreEqual("Name='Fred'", term.Token);
		}

		[Test]
		public void shouldTreatMultipleTermsAsANDFilter()
		{
			XmlDocument document = new XmlDocument();

			XmlElement term1Element = document.CreateElement("FilterTerm");
			term1Element.SetAttribute("attribute", "Name");
			term1Element.SetAttribute("operator", "=");
			term1Element.SetAttribute("value", "Fred");

			XmlElement term2Element = document.CreateElement("FilterTerm");
			term2Element.SetAttribute("attribute", "Reference");
			term2Element.SetAttribute("operator", "=");
			term2Element.SetAttribute("value", "Some Text Here");


			XmlElement filterElement = document.CreateElement("QueryFilter");
			filterElement.AppendChild(term1Element);
			filterElement.AppendChild(term2Element);

			FilterParser parser = new FilterParser(_metaModel.GetAssetType("Test"));
			IFilterTerm term = parser.parse(filterElement);
			Assert.IsNotNull(term);
			Assert.AreEqual("(Name='Fred';Reference='Some Text Here')", term.Token);			
		}

	}
}
