using System.IO;
using System.Xml;
using NUnit.Framework;
using VersionOne.Profile;
using VersionOne.ServiceHost.Tests.Utility;

namespace VersionOne.ServiceHost.Tests.Profile
{
	[TestFixture]
	public class XmlProfileStoreTester
	{
		private IProfileStore _profilestore;
		private IProfile _profile;

		[SetUp]
		public void Setup()
		{
			using (Stream xmlstream = ResourceLoader.LoadClassStream("XmlProfileStoreTester.xml", this.GetType()))
			{
				_profilestore = new XmlProfileStore(xmlstream);
				_profile = _profilestore["/"];
			}
		}

		[Ignore("Root name is implementation-specific")]
		[Test]
		public void RootName()
		{
			Assert.AreEqual("profileroot", _profile.Name);
		}

		[Test]
		public void GetNullValue()
		{
			Assert.IsNull(_profile.Value);
		}

		[Test]
		public void GetNestedValue()
		{
			Assert.AreEqual("Value 1", _profile["firstvalue"].Value);
		}

		[Test]
		public void GetDoubleNestedValue()
		{
			Assert.AreEqual("A child value", _profile["firstvalue/child"].Value);
		}

		[Test]
		public void NameOfNonexistNode()
		{
			Assert.AreEqual("blah", _profile["blah"].Name);
		}

		[Test]
		public void GetNonexistValue()
		{
			Assert.IsNull(_profile["firstvalue"]["blah"].Value);
			Assert.IsNull(_profile["blah"].Value);
		}

		[Test]
		public void SetRootValue()
		{
			_profile.Value = "Value of root";
			Assert.AreEqual("Value of root", _profile.Value);
		}

		[Test]
		public void SetNestedValue()
		{
			_profile["firstvalue"].Value = "Changed to value";
			Assert.AreEqual("Changed to value", _profile["firstvalue"].Value);
		}

		[Test]
		public void SetDoubleNestedValue()
		{
			_profile["firstvalue/child"].Value = "Changed to value";
			Assert.AreEqual("Changed to value", _profile["firstvalue/child"].Value);
		}

		[Test]
		public void SetNewNestedValue()
		{
			_profile["third"].Value = "turd";
			Assert.AreEqual("turd", _profile["third"].Value);
		}

		[Test]
		public void SetNewNewNestedValue()
		{
			_profile["third/down"].Value = "turd";
			Assert.AreEqual("turd", _profile["third/down"].Value);
		}

		[Test]
		public void SetNestedNewNewValue()
		{
			_profile["firstvalue/child/granchild/greatgrand"].Value = "wow";
			Assert.AreEqual("wow", _profile["firstvalue/child/granchild/greatgrand"].Value);
		}

		[Test]
		public void MultipleSetNewValue()
		{
			IProfile profile = _profile["firstvalue"];
			string original = profile.Value;

			profile.Value = "first";
			Assert.AreEqual("first", profile.Value);

			profile.Value = "second";
			Assert.AreEqual("second", profile.Value);

			profile.Value = null;
			Assert.IsNull(profile.Value);

			profile.Value = original;
			Assert.AreEqual(original, profile.Value);
		}

		[Test]
		public void MultipleSetNewValueHypo()
		{
			IProfile profile = _profile["a"];
			string original = profile.Value;

			profile.Value = "first";
			Assert.AreEqual("first", profile.Value);

			profile.Value = "second";
			Assert.AreEqual("second", profile.Value);

			profile.Value = null;
			Assert.IsNull(profile.Value);

			profile.Value = original;
			Assert.AreEqual(original, profile.Value);
		}

		[Test]
		public void Linked()
		{
			IProfile profile1 = _profile["a/b/c"];
			IProfile profile2 = _profile["a/b/c"];
			profile1.Value = "xyz";
			Assert.AreEqual("xyz", profile2.Value);
			Assert.AreEqual("xyz", _profile["a/b/c"].Value);
		}

		[Test]
		public void DoubleHypo()
		{
			IProfile profile1 = _profile["a/b/c"];
			IProfile profile2 = profile1["d/e/f"];
			profile2.Value = "xyz";
			Assert.AreEqual("xyz", _profile["a/b/c/d/e/f"].Value);
		}

		[Test]
		public void Parent()
		{
			IProfile childprofile = _profile["firstvalue/child"];
			childprofile.Value = "x";
			IProfile profile = childprofile.Parent;
			Assert.AreEqual("firstvalue", profile.Name);
			Assert.AreEqual("Value 1", profile.Value);
		}

		[Test]
		public void ParentHypo()
		{
			IProfile profile = _profile["firstvalue/child"].Parent;
			Assert.AreEqual("firstvalue", profile.Name);
			Assert.AreEqual("Value 1", profile.Value);
		}

		[Test]
		public void RootParent()
		{
			Assert.IsNull(_profile.Parent);
		}

		[Test]
		public void FunkyCharacters()
		{
			_profile["a"].Value = "<!-- --> \" ' & ";
			Assert.AreEqual("<!-- --> \" ' & ", _profile["a"].Value);
		}

		[Test]
		public void Nesting()
		{
			_profile["x/y/z"].Value = "abc";
			Assert.AreEqual("abc", _profile["x"]["y"]["z"].Value);
		}

		[Test]
		public void RootPath()
		{
			Assert.AreEqual("/", _profile.Path);
		}

		[Test]
		public void DeepPath()
		{
			IProfile profile = _profile["a/b/c"];
			profile.Value = "x";
			profile = profile["x/y/z"];
			profile.Value = "a";
			Assert.AreEqual("/a/b/c/x/y/z", profile.Path);
		}

		[Test]
		public void DeepPathHypo()
		{
			IProfile profile = _profile["a/b/c"];
			profile = profile["x/y/z"];
			Assert.AreEqual("/a/b/c/x/y/z", profile.Path);
		}

		[Test]
		public void RemoveValue()
		{
			_profile["firstvalue"].Value = null;
			Assert.IsNull(_profile["firstvalue"].Value);
		}

		[Test]
		public void RemoveNode()
		{
			_profile["firstvalue/child"].Value = null;
			Assert.IsNull(_profile["firstvalue/child"].Value);
		}

		[Test]
		public void RemoveEmptyNodes()
		{
			_profile["firstvalue"].Value = null;
			_profile["firstvalue/child"].Value = null;
			Assert.IsNull(_profile["firstvalue"].Value);
			Assert.IsNull(_profile["firstvalue/child"].Value);
		}

		[Test]
		public void GetByFullPathHypo()
		{
			IProfile profile1 = _profile["firstvalue/child"];
			IProfile profile2 = _profile["/secondvalue"];
			Assert.AreEqual("Value 2", profile2.Value);
		}

		[Test]
		public void GetByFullPath()
		{
			IProfile profile1 = _profile["firstvalue/child"];
			profile1.Value = "test";
			IProfile profile2 = _profile["/secondvalue"];
			Assert.AreEqual("Value 2", profile2.Value);
		}

		[Test]
		public void CheckAbsolutePathHypo()
		{
			IProfile profile1 = _profile["firstvalue/child"];
			IProfile profile2 = _profile["/secondvalue"];
			Assert.AreEqual("/secondvalue", profile2.Path);
		}

		[Test]
		public void CheckAbsolutePath()
		{
			IProfile profile1 = _profile["firstvalue/child"];
			profile1.Value = "x";
			IProfile profile2 = _profile["/secondvalue"];
			Assert.AreEqual("/secondvalue", profile2.Path);
		}

		[Test]
		public void CheckAbsolutePathFromChild()
		{
			IProfile profile1 = _profile["firstvalue/child"];
			IProfile profile2 = profile1["/secondvalue"];
			Assert.AreEqual("/secondvalue", profile2.Path);
		}

		[Test]
		public void CheckAbsolutePathFromChildHypo()
		{
			IProfile profile1 = _profile["a/b"];
			IProfile profile2 = profile1["/secondvalue"];
			Assert.AreEqual("/secondvalue", profile2.Path);
		}

		[Test]
		public void BogusAbsolutePath()
		{
			IProfile profile = _profile["/bogus/test"];
			Assert.AreEqual("/bogus/test", profile.Path);
		}

		[Test]
		public void BogusAbsolutePathFromChild()
		{
			IProfile profile1 = _profile["firstvalue/child"];
			IProfile profile2 = profile1["/bogus/test"];
			Assert.AreEqual("/bogus/test", profile2.Path);
		}

		[Test]
		public void BogusAbsolutePathFromChildHypo()
		{
			IProfile profile1 = _profile["a/b"];
			IProfile profile2 = profile1["/bogus/test"];
			Assert.AreEqual("/bogus/test", profile2.Path);
		}

		[Test]
		public void CheckRootRelativePath()
		{
			IProfile profile = _profile["//firstvalue/child"];
			Assert.AreEqual("/firstvalue/child", profile.Path);
		}

		[Test]
		public void CheckRootRelativePathHypo()
		{
			IProfile profile = _profile["//a/b"];
			Assert.AreEqual("/a/b", profile.Path);
		}

		[Test]
		public void CheckRelativePath()
		{
			IProfile profile = _profile["firstvalue"];
			IProfile other = profile["../secondvalue"];
			Assert.AreEqual("/secondvalue", other.Path);
		}

		[Test]
		public void CheckRelativePathValue()
		{
			IProfile profile = _profile["firstvalue"];
			IProfile other = profile["../secondvalue"];
			Assert.AreEqual("Value 2", other.Value);
		}

		[Test]
		public void CheckRelativePathHypo()
		{
			IProfile profile = _profile["A/B"];
			IProfile other = profile["../Q"];
			Assert.AreEqual("/A/Q", other.Path);
		}

		[Test]
		public void CheckRelativeNestedPath()
		{
			IProfile profile = _profile["firstvalue/child"];
			IProfile other = profile["../../secondvalue"];
			Assert.AreEqual("/secondvalue", other.Path);
		}

		[Test]
		public void CheckRelativeNestedPathValue()
		{
			IProfile profile = _profile["firstvalue/child"];
			IProfile other = profile["../../secondvalue"];
			Assert.AreEqual("Value 2", other.Value);
		}

		[Test]
		public void CheckRelativeNestedPathHypo()
		{
			IProfile profile = _profile["A/B/C"];
			IProfile other = profile["../../Q"];
			Assert.AreEqual("/A/Q", other.Path);
		}

		[Test]
		public void CheckRelativeStopOnRoot()
		{
			IProfile profile = _profile["firstvalue/child"];
			IProfile other = profile["../../../../../../secondvalue"];
			Assert.AreEqual("/secondvalue", other.Path);
		}

		[Test]
		public void CheckRelativeStopOnRootValue()
		{
			IProfile profile = _profile["firstvalue/child"];
			IProfile other = profile["../../../../../../secondvalue"];
			Assert.AreEqual("Value 2", other.Value);
		}

		[Test]
		public void CheckRelativeStopOnRootHypo()
		{
			IProfile profile = _profile["A/B/C"];
			IProfile other = profile["../../../../../../X"];
			Assert.AreEqual("/X", other.Path);
		}

		[Test]
		public void CheckRelativeStopOnRootHypoHypo()
		{
			IProfile profile = _profile["A/B/C"];
			IProfile other = profile["../R/../S/../../../X"];
			Assert.AreEqual("/X", other.Path);
		}

		[Test]
		public void GetSelf()
		{
			IProfile profile = _profile["firstvalue"][""]["child"];
			Assert.AreEqual("/firstvalue/child", profile.Path);
		}

		[Test]
		public void GetRoot()
		{
			IProfile profile = _profile["firstvalue"]["/"]["secondvalue"];
			Assert.AreEqual("/secondvalue", profile.Path);
		}

		[Test]
		public void GetParent()
		{
			IProfile profile = _profile["firstvalue"][".."]["secondvalue"];
			Assert.AreEqual("/secondvalue", profile.Path);
		}

		[Test]
		public void GetEncodedNestedValue()
		{
			Assert.AreEqual("Value 3", _profile["third:value"].Value);
			Assert.AreEqual("Value 4", _profile["forth@value"].Value);
		}

		[ExpectedException(typeof(System.InvalidOperationException))]
		[Test]
		public void StreamCannotBeWritten()
		{
			_profile["firstvalue"].Value = "value 999";
			_profilestore.Flush();
		}

		[Test]
		public void OverwriteStream()
		{
			string xml = "<root />";
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(xml);
			System.IO.Stream stream = new System.IO.MemoryStream();
			stream.Write(bytes, 0, bytes.Length);

			XmlProfileStore store = new XmlProfileStore(stream);
			IProfile root = store["/"];

			root["firstvalue"].Value = "value 999";
			store.Flush();

			stream.Seek(0, SeekOrigin.Begin);
			StreamReader reader = new StreamReader(stream);
			string newxml = reader.ReadToEnd();
			reader.Close();

			Assert.AreNotEqual(newxml, xml);

			System.Xml.XmlDocument doc = new XmlDocument();
			doc.LoadXml(newxml);

			AssertXml.Matches("<root><firstvalue value=\"value 999\" /></root>", doc.DocumentElement);
		}

		[Test]
		public void UnderwriteStream()
		{
			string xml = "<root><A><B><C value=\"1234567890\" /></B></A></root>";
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(xml);
			System.IO.Stream stream = new System.IO.MemoryStream();
			stream.Write(bytes, 0, bytes.Length);

			XmlProfileStore store = new XmlProfileStore(stream);
			IProfile root = store["/"];
			root["A/B/C"].Value = "5";
			store.Flush();

			stream.Seek(0, SeekOrigin.Begin);
			StreamReader reader = new StreamReader(stream);
			string newxml = reader.ReadToEnd();
			reader.Close();

			Assert.AreNotEqual(newxml, xml);

			System.Xml.XmlDocument doc = new XmlDocument();
			doc.LoadXml(newxml);

			AssertXml.Matches("<root><A><B><C value=\"5\" /></B></A></root>", doc.DocumentElement);
		}
		[Test]
		public void DeleteFromStream()
		{
			string xml = "<root><A><B><C value=\"1234567890\" /></B></A></root>";
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(xml);
			System.IO.Stream stream = new System.IO.MemoryStream();
			stream.Write(bytes, 0, bytes.Length);

			XmlProfileStore store = new XmlProfileStore(stream);
			IProfile root = store["/"];
			root["A/B/C"].Value = null;
			store.Flush();

			stream.Seek(0, SeekOrigin.Begin);
			StreamReader reader = new StreamReader(stream);
			string newxml = reader.ReadToEnd();
			reader.Close();

			Assert.AreNotEqual(newxml, xml);

			System.Xml.XmlDocument doc = new XmlDocument();
			doc.LoadXml(newxml);

			AssertXml.Matches("<root />", doc.DocumentElement);
		}
	}
}
