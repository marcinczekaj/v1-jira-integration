/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using System;
namespace VersionOne.ServiceHost.WorkitemServices {
    public class CreatedWorkitemsSource {
        public CreatedWorkitemsSource(IEnumerable<String> scopes)
        {
            Scopes = scopes;
        }

        public IEnumerable<String> Scopes { get; private set; }
    }

}