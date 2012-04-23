using System;
using System.Collections.Generic;
using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector;

namespace VersionOne.ServiceHost.Tests.ServerConnector {
    public class AssetDisposer : IDisposable {
        private readonly Stack<Asset> assets = new Stack<Asset>();
        private readonly IServices services;

        public delegate Asset CreateAssetOperation();

        public AssetDisposer(IServices services) {
            this.services = services;
        }
        
        public Asset CreateAndRegisterForDisposal(CreateAssetOperation createOperation) {
            var asset = createOperation.Invoke();
            assets.Push(asset);
            return asset;
        }

        public void Dispose() {
            while(assets.Count > 0) {
                var asset = assets.Pop();
                DeleteAsset(asset);
            }
        }

        private void DeleteAsset(Asset subject) {
            if(subject == null) {
                return;
            }

            var operation = subject.AssetType.GetOperation(VersionOneProcessor.DeleteOperation);
            services.ExecuteOperation(operation, subject.Oid);
        }
    }
}