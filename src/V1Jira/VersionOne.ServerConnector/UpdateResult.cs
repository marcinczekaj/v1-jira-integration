using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VersionOne.ServerConnector {
    public class UpdateResult {
        public DateTime modificationTime { get; set; }
        public string number {get; set; }

        public UpdateResult(DateTime modificationTime, string number) {
            this.modificationTime = modificationTime;
            this.number = number;
        }

        public UpdateResult() {
            this.modificationTime = DateTime.MinValue;
            this.number = string.Empty;
        }

        public bool isNewer( UpdateResult newer ){
            if (newer.modificationTime > this.modificationTime)
                return true;
            return false;
        }

        public bool isDefault() {
            return (this.modificationTime == DateTime.MinValue && string.IsNullOrEmpty(number) == true) ? true : false ;
        }
    }
}
