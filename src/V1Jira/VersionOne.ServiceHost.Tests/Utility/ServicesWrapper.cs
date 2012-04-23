using System;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServiceHost.Tests.Utility
{
	public class ServicesWrapper : IServices
	{
		private IServices _wrapped;

		public ServicesWrapper(IServices wrapped)
		{
			_wrapped = wrapped;
		}

		public QueryResult Retrieve(Query query)
		{
			return _wrapped.Retrieve(query);
		}

		public void Save(Asset asset)
		{
			OnBeforeSave(asset, null);
			_wrapped.Save(asset);
		}

		public void Save(Asset asset, string comment)
		{
			OnBeforeSave(asset, comment);
			_wrapped.Save(asset, comment);
		}

		public void Save(AssetList assetList)
		{
			OnBeforeSave(assetList);
			_wrapped.Save(assetList);
		}

		public Asset New(IAssetType assetType, Oid context)
		{
			return _wrapped.New(assetType, context);
		}

		public Oid GetOid(string token)
		{
			return _wrapped.GetOid(token);
		}

		public Oid ExecuteOperation(IOperation op, Oid oid)
		{
			return _wrapped.ExecuteOperation(op, oid);
		}

		public Oid LoggedIn { get { return _wrapped.LoggedIn; } }
		
		public event EventHandler<SavedEventArgs> BeforeSave;
		private void OnBeforeSave(Asset asset, string comment)
		{
			if (BeforeSave != null)
				BeforeSave(this, new SavedEventArgs(asset, comment));
		}
		
		private void OnBeforeSave(AssetList assetList)
		{
			if (BeforeSave != null)
				BeforeSave(this, new SavedEventArgs(assetList));
		}
	}

	public class SavedEventArgs : EventArgs
	{
		public readonly string Comment;
		public AssetList Assets = new AssetList();
		public SavedEventArgs(Asset asset, string comment)
		{
			Assets.Add(asset);
			Comment = comment;
		}
		
		public SavedEventArgs(AssetList assetList)
		{
			Assets.AddRange(assetList);
		}
	}
}