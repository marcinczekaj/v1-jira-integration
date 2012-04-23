using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Logging;
using IntegrationTests.Logger;

namespace IntegrationTests.Factory {
    class LoggerFactory {

        public static ILogger register() {
            return register(new LoggerImpl());
        }

        public static ILogger register(ILogger logger) {
            ComponentRepository.Instance.Register<ILogger>(logger);
            return logger;
        }
    }
}
