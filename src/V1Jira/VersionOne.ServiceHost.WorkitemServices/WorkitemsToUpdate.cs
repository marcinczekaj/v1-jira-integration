using System.Collections.Generic;
using VersionOne.ServiceHost.WorkitemServices;

namespace VersionOne.ServiceHost.WorkitemServices {
    public class WorkitemsToUpdate {

        public List<Workitem> workitemsForUpdate {get; private set;}
        public string Source { get; private set; }


        public WorkitemsToUpdate(List<Workitem> workitemsForUpdate, string Source) {
            this.workitemsForUpdate = workitemsForUpdate;
            this.Source = Source;
        }

    }
}
