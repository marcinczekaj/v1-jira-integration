using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VersionOne.Profile;
using System.Globalization;

namespace VersionOne.ServiceHost.WorkitemServices {
    public class LastValidSynchronisationProfile {
        private readonly string LastCreatedWorkitemIdKey = "LastCreatedWorkitemId";
        private readonly string LastCheckForCreatedWorkitemsKey = "LastCheckForCreatedWorkitems";

        private readonly string LastClosedWorkitemIdKey = "LastClosedWorkitemId";
        private readonly string LastCheckForClosedWorkitemsKey = "LastCheckForClosedWorkitems";

        private readonly string LastCheckForSynchronizedWorkitemsKey = "LastCheckForSynchronizedWorkitems";
        private readonly string LastSynchronizedWorkitemIdKey = "LastSynchronizedWorkitemId";

        private const string DateFormat = "MM/dd/yyyy HH:mm:ss.fff";
        private IProfile profile;

        public LastValidSynchronisationProfile(IProfile profile) {
            this.profile = profile;
        }

        public DateTime LastCheckForCreatedWorkitems
        {
            get
            {
                var lastCheckString = profile[LastCheckForCreatedWorkitemsKey].Value;
                DateTime lastCreatedWorkitemCheck;

                if (!DateTime.TryParseExact(lastCheckString, DateFormat, null, DateTimeStyles.None, out lastCreatedWorkitemCheck))
                {
                    // Assuming that a failed parse means we haven't checked before, use beginning of today
                    // lastCreatedDefectCheck = DateTime.Today;

                    // Assuming that a failed parse means we haven't checked before, use minimal value of time filtering for time will not use
                    lastCreatedWorkitemCheck = DateTime.MinValue;
                }

                return lastCreatedWorkitemCheck;
            }

            set { profile[LastCheckForCreatedWorkitemsKey].Value = value.ToString(DateFormat); }
        }

        public string LastCreatedWorkitemId
        {
            get
            {
                var lastCheckString = string.Empty;

                if (profile[LastCreatedWorkitemIdKey] != null && profile[LastCreatedWorkitemIdKey].Value != null)
                {
                    lastCheckString = profile[LastCreatedWorkitemIdKey].Value;
                }

                return lastCheckString;
            }

            set { profile[LastCreatedWorkitemIdKey].Value = value; }
        }


        public DateTime LastCheckForClosedWorkitems {
            get {
                var lastCheckString = profile[LastCheckForClosedWorkitemsKey].Value;
                DateTime lastClosedWorkitemCheck;

                if (!DateTime.TryParseExact(lastCheckString, DateFormat, null, DateTimeStyles.None, out lastClosedWorkitemCheck))
                    lastClosedWorkitemCheck = DateTime.MinValue;

                return lastClosedWorkitemCheck;
            }

            set { profile[LastCheckForClosedWorkitemsKey].Value = value.ToString(DateFormat); }
        }

        public string LastClosedWorkitemId {
            get {
                var lastCheckString = string.Empty;

                if (profile[LastClosedWorkitemIdKey] != null && profile[LastClosedWorkitemIdKey].Value != null) 
                    lastCheckString = profile[LastClosedWorkitemIdKey].Value;

                return lastCheckString;
            }

            set { profile[LastClosedWorkitemIdKey].Value = value; }
        }

        public DateTime LastCheckForSynchronizedWorkitems {
            get {
                var lastCheckString = profile[LastCheckForSynchronizedWorkitemsKey].Value;
                DateTime lastSynchronizedWorkitemCheck;

                if (!DateTime.TryParseExact(lastCheckString, DateFormat, null, DateTimeStyles.None, out lastSynchronizedWorkitemCheck)) 
                    lastSynchronizedWorkitemCheck = DateTime.MinValue;

                return lastSynchronizedWorkitemCheck;
            }

            set { profile[LastCheckForSynchronizedWorkitemsKey].Value = value.ToString(DateFormat); }
        }



        public string LastSynchronizedWorkitemId {
            get {
                var lastCheckString = string.Empty;

                if (profile[LastSynchronizedWorkitemIdKey] != null && profile[LastSynchronizedWorkitemIdKey].Value != null) {
                    lastCheckString = profile[LastSynchronizedWorkitemIdKey].Value;
                }

                return lastCheckString;
            }

            set { profile[LastSynchronizedWorkitemIdKey].Value = value; }
        }

    }
}
