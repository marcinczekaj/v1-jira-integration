/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.SDK.APIClient;
namespace VersionOne.ServerConnector.Entities {
    public class Link {
        public const string OnMenuProperty = "OnMenu";
        public const string UrlProperty = "URL";

        public string Title { get; set; }
        public string Url { get; set; }
        public bool OnMenu { get; set; }

        public Link(string url, string title, bool onMenu) {
            Title = title;
            Url = url;
            OnMenu = onMenu;
        }

        public Link(string url, string title) : this(url, title, true) {
        }

        public Link(Asset asset) {
            Title = asset.GetAttribute(asset.AssetType.GetAttributeDefinition(Entity.NameProperty)).Value.ToString();
            Url = asset.GetAttribute(asset.AssetType.GetAttributeDefinition(UrlProperty)).Value.ToString();
            OnMenu = (bool)asset.GetAttribute(asset.AssetType.GetAttributeDefinition(OnMenuProperty)).Value;
        }

        public override bool Equals(object obj) {
            if(ReferenceEquals(null, obj)) {
                return false;
            }
            if(ReferenceEquals(this, obj)) {
                return true;
            }
            return obj.GetType() == typeof(Link) && Equals((Link) obj);
        }

        private bool Equals(Link other) {
            return Equals(other.Title, Title) && Equals(other.Url, Url) && other.OnMenu.Equals(OnMenu);
        }

        public override int GetHashCode() {
            var result = (Title != null ? Title.GetHashCode() : 0);
            result = (result*397) ^ (Url != null ? Url.GetHashCode() : 0);
            result = (result*397) ^ OnMenu.GetHashCode();
            return result;
        }
    }
}