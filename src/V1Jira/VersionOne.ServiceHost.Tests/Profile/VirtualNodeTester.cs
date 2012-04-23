using System.IO;
using NUnit.Framework;
using VersionOne.Profile;
using VersionOne.ServiceHost.Tests.Utility;

namespace VersionOne.ServiceHost.Tests.Profile
{
	[TestFixture]
	public class VirtualNodeTester
	{
		private IProfileStore _profilestore;
		private IProfile _root;
		private IProfile _profile;

		[SetUp]
		public void Setup()
		{
			using (Stream xmlstream = ResourceLoader.LoadClassStream("VirtualNodeTester.xml", GetType()))
			{
				_profilestore = new XmlProfileStore(xmlstream);
				_root = _profilestore["/"];
				_profile = _profilestore["/profileroot"];
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
			string val = "Value of root";
			_profile.Value = val;
			Assert.AreEqual(val, _profile.Value);
			Assert.AreEqual(val, _root["profileroot"].Value);
		}

		[Test]
		public void SetNestedValue()
		{
			string val = "Changed to value";
			_profile["firstvalue"].Value = val;
			Assert.AreEqual(val, _profile["firstvalue"].Value);
			Assert.AreEqual(val, _root["profileroot/firstvalue"].Value);
		}

		[Test]
		public void SetDoubleNestedValue()
		{
			string val = "Changed to value";
			_profile["firstvalue/child"].Value = val;
			Assert.AreEqual(val, _profile["firstvalue/child"].Value);
			Assert.AreEqual(val, _root["profileroot/firstvalue/child"].Value);
		}

		[Test]
		public void SetNewNestedValue()
		{
			string val = "turd";
			_profile["third"].Value = val;
			Assert.AreEqual(val, _profile["third"].Value);
			Assert.AreEqual(val, _root["profileroot/third"].Value);
		}

		[Test]
		public void SetNewNewNestedValue()
		{
			string val = "turd";
			_profile["third/down"].Value = val;
			Assert.AreEqual(val, _profile["third/down"].Value);
			Assert.AreEqual(val, _root["profileroot/third/down"].Value);
		}

		[Test]
		public void SetNestedNewNewValue()
		{
			string val = "wow";
			_profile["firstvalue/child/granchild/greatgrand"].Value = val;
			Assert.AreEqual(val, _profile["firstvalue/child/granchild/greatgrand"].Value);
			Assert.AreEqual(val, _root["profileroot/firstvalue/child/granchild/greatgrand"].Value);
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
			string val = "wow";
			profile1.Value = val;
			Assert.AreEqual(val, profile2.Value);
			Assert.AreEqual(val, _profile["a/b/c"].Value);
			Assert.AreEqual(val, _root["profileroot/a/b/c"].Value);
		}

		[Test]
		public void DoubleHypo()
		{
			IProfile profile1 = _profile["a/b/c"];
			IProfile profile2 = profile1["d/e/f"];
			string val = "wow";
			profile2.Value = val;
			Assert.AreEqual(val, _profile["a/b/c/d/e/f"].Value);
			Assert.AreEqual(val, _root["profileroot/a/b/c/d/e/f"].Value);
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
			string val = "<!-- --> \" ' & ";
			_profile["a"].Value = val;
			Assert.AreEqual(val, _profile["a"].Value);
			Assert.AreEqual(val, _root["profileroot/a"].Value);
		}

		[Test]
		public void Nesting()
		{
			string val = "abc";
			_profile["x/y/z"].Value = val;
			Assert.AreEqual(val, _profile["x"]["y"]["z"].Value);
			Assert.AreEqual(val, _root["profileroot"]["x"]["y"]["z"].Value);
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
			Assert.IsNull(_root["profileroot/firstvalue"].Value);
		}

		[Test]
		public void RemoveNode()
		{
			_profile["firstvalue/child"].Value = null;
			Assert.IsNull(_profile["firstvalue/child"].Value);
			Assert.IsNull(_root["profileroot/firstvalue/child"].Value);
		}

		[Test]
		public void RemoveEmptyNodes()
		{
			_profile["firstvalue"].Value = null;
			_profile["firstvalue/child"].Value = null;
			Assert.IsNull(_profile["firstvalue"].Value);
			Assert.IsNull(_profile["firstvalue/child"].Value);
			Assert.IsNull(_root["profileroot/firstvalue"].Value);
			Assert.IsNull(_root["profileroot/firstvalue/child"].Value);
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
		public void CheckBogusFromRoot()
		{
			IProfile profile = _root["bogus/test"];
			Assert.AreEqual("Test", profile.Value);
		}

		[Test]
		public void BogusAbsolutePath()
		{
			IProfile profile = _profile["/bogus/test"];
			Assert.AreEqual("/bogus/test", profile.Path);
			Assert.IsNull(profile.Value);
		}

		[Test]
		public void BogusAbsolutePathFromChild()
		{
			IProfile profile1 = _profile["firstvalue/child"];
			IProfile profile2 = profile1["/bogus/test"];
			Assert.AreEqual("/bogus/test", profile2.Path);
			Assert.IsNull(profile2.Value);
		}

		[Test]
		public void BogusAbsolutePathFromChildHypo()
		{
			IProfile profile1 = _profile["a/b"];
			IProfile profile2 = profile1["/bogus/test"];
			Assert.AreEqual("/bogus/test", profile2.Path);
			Assert.IsNull(profile2.Value);
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
		public void NewVirtualProfile()
		{
			IProfile profile = _profilestore["/users"];
			Assert.AreEqual("/", profile.Path);
		}

		[Test]
		public void NewNestedVirtualProfile()
		{
			IProfile profile = _profilestore["/users/bubba"];
			Assert.AreEqual("/", profile.Path);
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
	}
}
