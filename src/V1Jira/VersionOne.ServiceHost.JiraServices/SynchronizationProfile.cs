using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using VersionOne.Profile;

namespace VersionOne.ServiceHost.JiraServices {
    class SynchronizationProfile {

        private readonly string LastQueryForModifiedItemsKey = "LastQueryForModifiedItems";

        private const string DateFormat = "MM/dd/yyyy HH:mm:ss.fff";
        IProfile profile;

        public SynchronizationProfile(IProfile profile) {
            this.profile = profile;        
        }

        public DateTime LastQueryForModifiedItems {
            get {
                var lastQueryString = profile[LastQueryForModifiedItemsKey].Value;
                DateTime lastQueryDate;

                if (!DateTime.TryParseExact(lastQueryString, DateFormat, null, DateTimeStyles.None, out lastQueryDate)) 
                    lastQueryDate = DateTime.MinValue;
                
                return lastQueryDate;
            }

            set { profile[LastQueryForModifiedItemsKey].Value = value.ToString(DateFormat); }
        }
    }
}
