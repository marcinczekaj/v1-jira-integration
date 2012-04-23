/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using System.Linq;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Configuration;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.StartupValidation;

namespace VersionOne.ServiceHost.JiraServices.StartupValidation {
    public class MappingValidator : ISimpleValidator {
        private readonly IDictionary<MappingInfo, MappingInfo> mappings;
        private readonly ILogger logger;
        private readonly string mappingName;

        public MappingValidator(IDictionary<MappingInfo, MappingInfo> mappings, string mappingName) {
            logger = ComponentRepository.Instance.Resolve<ILogger>();
            this.mappings = mappings;
            this.mappingName = mappingName;
        }

        public bool Validate() {
            logger.Log(LogMessage.SeverityType.Info, string.Format("Checking JIRA {0} mappings.", mappingName));

            var emptyCounter = mappings.Count(IsMappingEmpty);

            if (emptyCounter > 0) {
                logger.Log(LogMessage.SeverityType.Error, string.Format("Mapping contains {0} empty mapping(s).", emptyCounter));
            }
            
            logger.Log(LogMessage.SeverityType.Info, string.Format("JIRA {0} mappings are checked.", mappingName));

            return emptyCounter == 0;
        }

        private static bool IsMappingEmpty(KeyValuePair<MappingInfo, MappingInfo> mapping) {
            return mapping.Key.IsNullOrEmpty() || mapping.Value.IsNullOrEmpty();
        }
    }
}