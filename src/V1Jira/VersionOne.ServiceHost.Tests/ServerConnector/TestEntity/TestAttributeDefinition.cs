using VersionOne.SDK.APIClient;

namespace VersionOne.ServiceHost.Tests.ServerConnector.TestEntity {
    public class TestAttributeDefinition : IAttributeDefinition {
        private readonly IAssetType assetType;
        private readonly bool isMultiValue;
        private readonly bool isReadOnly;
        private readonly bool isRequired;

        public TestAttributeDefinition(IAssetType assetType) : this(assetType, false, false, false) {
        }

        public TestAttributeDefinition(IAssetType assetType, bool isMultiValue, bool isReadOnly, bool isRequired) {
            this.assetType = assetType;
            this.isMultiValue = isMultiValue;
            this.isReadOnly = isReadOnly;
            this.isRequired = isRequired;
        }

        public IAttributeDefinition Aggregate(Aggregate aggregate) {
            throw new System.NotImplementedException();
        }

        public IAssetType AssetType {
            get { return assetType; }
        }

        public AttributeType AttributeType {
            get { throw new System.NotImplementedException(); }
        }

        public IAttributeDefinition Base {
            get { throw new System.NotImplementedException(); }
        }

        public object Coerce(object value) {
            return value;
        }

        public string DisplayName {
            get { throw new System.NotImplementedException(); }
        }

        public IAttributeDefinition Downcast(IAssetType assetType) {
            throw new System.NotImplementedException();
        }

        public IAttributeDefinition Filter(IFilterTerm filter) {
            throw new System.NotImplementedException();
        }

        public bool IsMultiValue {
            get { return isMultiValue; }
        }

        public bool IsReadOnly {
            get { return isReadOnly; }
        }

        public bool IsRequired {
            get { return isRequired; }
        }

        public IAttributeDefinition Join(IAttributeDefinition joined) {
            throw new System.NotImplementedException();
        }

        public string Name {
            get { return assetType.Token; }
        }

        public IAssetType RelatedAsset {
            get { throw new System.NotImplementedException(); }
        }

        public string Token {
            get { return assetType.Token; }
        }
    }
}