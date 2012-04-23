using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServiceHost.Tests
{
	internal delegate string ResolveDelegate(XmlNode node);

	internal abstract class ResponseConnector : IAPIConnector
	{
		private IDictionary _data = new Hashtable();
		private string _prefix = string.Empty;

		public ResponseConnector(string datafile, string prefix, string keys, ResolveDelegate resolver)
		{
			_prefix = prefix;

			if (keys == null || keys == string.Empty)
				return;

			XmlDocument doc = new XmlDocument();
			doc.Load(datafile);

			string[] parts = keys.Split(';');

			foreach (string part in parts)
			{
				XmlNodeList nodes = doc.DocumentElement.SelectNodes("Test[@name='" + part + "']");
				if (nodes.Count == 0)
					continue;
				XmlNode node = nodes[0];
				XmlNodeList responses = node.SelectNodes("Response");
				foreach (XmlElement response in responses)
					_data[response.GetAttribute("path")] = resolver(response);
			}
		}

		public Stream GetData()
		{
			string response = FindData(string.Empty);
			OnBeforeGetData(_prefix,string.Empty,response);
			return MakeStream(response);
		}

		public Stream GetData(string path)
		{
			string response = FindData(path);
			OnBeforeGetData(_prefix, path, response);
			return MakeStream(response);
		}


		public Stream SendData(string path, string data)
		{
			string response = FindData(path);
			OnBeforeSendData(_prefix, path, data, response);
			return MakeStream(response);
		}

	    public bool Instrument
	    {
            get { return false; }
	        set { ; }
	    }

	    private Stream MakeStream(string data)
		{
			return new MemoryStream(Encoding.ASCII.GetBytes(data));
		}
		
		private string FindData(string path)
		{
			string data = (string)_data[_prefix + path];
			if (data == null)
				throw new ApplicationException("Response Connector missing data for path: " + _prefix + path);
			return data;
		}

		internal delegate void OnDataHandler(object sender, DataRequestEventArgs e);

		private void OnBeforeGetData(string prefix, string path, string response)
		{
			if (_beforeGetData != null)
				_beforeGetData(this, new DataRequestEventArgs(prefix, path, response));
		}

		private event OnDataHandler _beforeGetData;
		internal event OnDataHandler BeforeGetData
		{
			add { _beforeGetData += value; }
			remove { _beforeGetData -= value; }
		}

		private void OnBeforeSendData(string prefix, string path, string data, string response)
		{
			if (_beforeSendData != null)
				_beforeSendData(this, new DataRequestEventArgs(prefix, path, data, response));
		}

		private event OnDataHandler _beforeSendData;
		internal event OnDataHandler BeforeSendData
		{
			add { _beforeSendData += value; }
			remove { _beforeSendData -= value; }
		}

        #region IAPIConnector Members - unused but must-have
        // TODO write implementations if required

        public Stream BeginRequest(string path) 
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public Stream EndRequest(string path, string contentType) 
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

	    public IDictionary<string, string> CustomHttpHeaders {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
	    }
	    #endregion
    }

	internal class XmlResponseConnector : ResponseConnector
	{
		public XmlResponseConnector(string datafile, string prefix, string keys) : base(datafile, prefix, keys, ResolveContent) { }
		private static string ResolveContent(XmlNode node)
		{
			return node.InnerXml;
		}
	}

	internal class TextResponseConnector : ResponseConnector
	{
		public TextResponseConnector(string datafile, string prefix, string keys) : base(datafile, prefix, keys, ResolveContent) { }
		private static string ResolveContent(XmlNode node)
		{
			return node.InnerText;
		}
	}

	internal class DataRequestEventArgs
	{
		private readonly string _prefix;
		private readonly string _path;
		private readonly string _data;
		private readonly string _response;

		internal DataRequestEventArgs(string prefix, string path, string response) : this(prefix, path, string.Empty, response) { }
		internal DataRequestEventArgs(string prefix, string path, string data, string response)
		{
			_prefix = prefix;
			_path = path;
			_data = data;
			_response = response;
		}

		internal string Prefix { get { return _prefix; } }
		internal string Path { get { return _path; } }
		internal string Data { get { return _data; } }
		internal string Response { get { return _response; } }
	}
}