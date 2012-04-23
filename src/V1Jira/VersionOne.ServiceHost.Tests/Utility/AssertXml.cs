using System.IO;
using System.Xml;

namespace NUnit.Framework
{
	public class AssertXml
	{
		public static void HasNode(string xpath, XmlNode xml, string message)
		{
			XmlNode found = xml.SelectSingleNode(xpath);
			Assert.IsNotNull(found, message);
		}
		public static void HasNode(string xpath, XmlNode xml)
		{
			HasNode(xpath, xml, "XML node not found: " + xpath);
		}

		public static void IsLike(string expectedxml, XmlNode actualxml, string message)
		{
			string xpath = XmlToXPath(expectedxml, false);
			HasNode(xpath, actualxml, message);
		}
		public static void IsLike(string expectedxml, XmlNode actualxml)
		{
			IsLike(expectedxml, actualxml, "XML is not like " + expectedxml);
		}

		public static void Matches(string expectedxml, XmlNode actualxml, string message)
		{
			string xpath = XmlToXPath(expectedxml, true);
			HasNode(xpath, actualxml, string.Format("{0}\n---Expected---\n{1}\n---Actual---\n{2}", message, expectedxml, FormattedXml(actualxml)));
		}
		public static void Matches(string expectedxml, XmlNode actualxml)
		{
			Matches(expectedxml, actualxml, "XML does not match");
		}

		private static string XmlToXPath(string xml, bool strict)
		{
			using (TextWriter writer = new StringWriter())
			{
				using (XmlReader reader = new XmlTextReader(new StringReader(xml)))
				{
					reader.MoveToContent();
					//writer.Write(strict? "/" : "//");
					XmlToXPathRoot(reader, writer, strict);
					//XmlToXPathElement(reader, writer, 1, strict);
				}
				return writer.ToString();
			}
		}

		private static void XmlToXPathRoot(XmlReader reader, TextWriter writer, bool strict)
		{
			writer.Write(strict ? "/" : "//");
			writer.Write(reader.LocalName);

			bool incondition = false;
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					StartCondition(writer, ref incondition);
					writer.Write("@{0}={1}", reader.LocalName, Quote(reader.Value));
				} while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}

			int childposition = 0;
			if (!reader.IsEmptyElement)
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						StartCondition(writer, ref incondition);
						XmlToXPathElement(reader, writer, ++childposition, strict);
					}
					else if (reader.NodeType == XmlNodeType.Text)
					{
						StartCondition(writer, ref incondition);
						writer.Write(".=");
						writer.Write(Quote(reader.Value));
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
						break;
				}
			}

			if (strict)
			{
				StartCondition(writer, ref incondition);
				writer.Write("count(child::*)=");
				writer.Write(childposition);
			}

			EndCondition(writer, ref incondition);
		}

		private static void XmlToXPathElement(XmlReader reader, TextWriter writer, int position, bool strict)
		{
			if (strict)
			{
				writer.Write("child::*[{0}][name()='{1}']", position, reader.LocalName);
				//				StartCondition(writer, ref incondition);
				//				writer.Write("position()=");
				//				writer.Write(position);
			}
			else
				writer.Write(reader.LocalName);

			bool incondition = false;
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					StartCondition(writer, ref incondition);
					writer.Write("@{0}={1}", reader.LocalName, Quote(reader.Value));
				} while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}

			int childposition = 0;
			if (!reader.IsEmptyElement)
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						StartCondition(writer, ref incondition);
						XmlToXPathElement(reader, writer, ++childposition, strict);
					}
					else if (reader.NodeType == XmlNodeType.Text)
					{
						StartCondition(writer, ref incondition);
						writer.Write(".=");
						writer.Write(Quote(reader.Value));
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
						break;
				}
			}

			if (strict)
			{
				StartCondition(writer, ref incondition);
				writer.Write("count(child::*)=");
				writer.Write(childposition);
			}

			EndCondition(writer, ref incondition);
		}
		private static void StartCondition(TextWriter writer, ref bool incondition)
		{
			if (!incondition)
			{
				writer.Write("[");
				incondition = true;
			}
			else
				writer.Write(" and ");
		}
		private static void EndCondition(TextWriter writer, ref bool incondition)
		{
			if (incondition)
			{
				writer.Write("]");
				incondition = false;
			}
		}
		private static string Quote(string value)
		{
			if (value.IndexOf('\'') < 0)
				return "'" + value + "'";
			return "\"" + value + "\"";
		}

		private static string FormattedXml(XmlNode node)
		{
			using (TextWriter textwriter = new StringWriter())
			{
				XmlTextWriter writer = new XmlTextWriter(textwriter);
				writer.Formatting = Formatting.Indented;
				node.WriteTo(writer);
				return textwriter.ToString();
			}
		}
	}
}
