/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.SDK.APIClient;

namespace VersionOne.ServerConnector.Entities {
    public class Member : Entity {
        new public const string NameProperty = "Name";
        public const string EmailProperty = "Email";
        public const string UsernameProperty = "Username";

        public Member(){}
        public Member(Asset asset) : base(asset, null) {}

        public string Name {
            get { return GetProperty<string>(NameProperty); }
            set { SetProperty(NameProperty, value); }
        }

        public string Email {
            get { return GetProperty<string>(EmailProperty); }
            set { SetProperty(EmailProperty, value); }
        }

        public string Username {
            get { return GetProperty<string>(UsernameProperty); }
            set { SetProperty(UsernameProperty, value); }
        }
    }
}