using NUnit.Framework;
using VersionOne.Profile;

namespace VersionOne.ServiceHost.Tests.Profile
{
	[TestFixture]
	public class XmlNormalizerTester
	{
		[Test]
		public void None()
		{
			Assert.AreEqual("normalstring", XmlNormalizer.TagEncode("normalstring"));
		}

		[Test]
		public void Space()
		{
			TestPair("a b", "a_x20b");
		}

		[Test]
		public void Pound()
		{
			TestPair("a#b", "a_x23b");
		}

		[Test]
		public void At()
		{
			TestPair("a@b", "a_x40b");
		}

		[Test]
		public void Colon()
		{
			TestPair("a:b", "a_x3Ab");
		}

		[Test]
		public void StartOfString()
		{
			TestPair(" ab", "_x20ab");
		}

		[Test]
		public void EndOfString()
		{
			TestPair("ab ", "ab_x20");
		}

		[Test]
		public void Multiple()
		{
			TestPair("a b c", "a_x20b_x20c");
		}

		private void TestPair(string input, string output)
		{
			Assert.AreEqual(output, XmlNormalizer.TagEncode(input));
			Assert.AreEqual(input, XmlNormalizer.TagDecode(output));
		}
	}
}
