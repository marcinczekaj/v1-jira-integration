using System.Collections.Generic;
using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Entities {
    internal static class EntityFactory {
        /// <summary>
        /// Create valid TestServiceConfigurationEntity
        /// </summary>
        internal static TestServiceEntity CreateTestServiceEntity() {
            var entity = new TestServiceEntity {
                ReferenceAttribute = "Reference",
                BaseQueryFilter = "Reference='';Owners.Nickname='qc';Status.Name=''",
                PassedOid = "TestStatus:129",
                FailedOid = "TestStatus:155",
                CreateDefect = "All",
                DescriptionSuffix = "Description suffix",
                ChangeComment = "Updated by VersionOne.ServiceHost"
            };

            return entity;
        }

        /// <summary>
        /// Create valid QC Service Entity
        /// </summary>
        internal static QCServiceEntity CreateQCServiceEntity() {
            var entity = new QCServiceEntity {
                CreateStatus = "Created",
                CloseStatus = "Closed",
                SourceField = "Quality Center"
            };
            SetQCConnectionParameters(entity, "http://localhost:8080/qcbin", "alex_qc", string.Empty);

            return entity;
        }

        internal static QCProject CreateQCProject(string id) {
            var project = new QCProject {
                Domain = "DEFAULT",
                Id = id,
                TestFolder = @"c:\test\",
				TestStatus = "Imported",
                Project = "Call Center",
                VersionOneIdField = "V1Id",
                VersionOneProject = "Call Center"
            };

            return project;
        }

        internal static QCDefectFilter CreateDefectFilter(string fieldName, string fieldValue) {
            var filter = new QCDefectFilter {FieldName = fieldName, FieldValue = fieldValue};

            return filter;
        }

        internal static void SetQCConnectionParameters(QCServiceEntity entity, string applicationUrl, string username, string password) {
            entity.Connection.ApplicationUrl = applicationUrl;
            entity.Connection.Username = username;
            entity.Connection.Password = password;
        }

        internal static SvnServiceEntity CreateSvnServiceEntity() {
            var entity = new SvnServiceEntity {
                Path = "svn://MySourceControlServer/MyRepo",
                Password = string.Empty,
                UserName = string.Empty,
                ReferenceExpression = "<![CDATA[[A-Z]{1,2}-[0-9]+]]>"
            };

            return entity;
        }

        public static LkkServiceEntity CreateLkkServiceEntity() {
            var entity = new LkkServiceEntity {
                Account = "myAccount",
                Username = "myUsername@mydomain.com",
                Password = "myPassword",
                OverrideWipLimit = false,
                OverrideWipLimitComment = string.Empty,
                V1ActiveStatus = "LK - Active",
                V1BacklogStatus = "LK - Backlog",
                V1ArchivedStatus = "LK - Archived",
                ProjectMappings = new List<LkkProjectMapping> { 
                    new LkkProjectMapping { 
                        LkkBoard = new Mapping { Id = "1000", Name = "Development", }, 
                        VersionOneProject = new Mapping { Id = "Scope:0", Name = "System (All Projects)", }, 
                    } 
                },
                PriorityMappings = new List<LkkPriorityMapping> { 
                    new LkkPriorityMapping { 
                        LkkPriority = new Mapping { Id = "0", },
                        VersionOnePriority = new Mapping { Id = "WorkitemPriority:100", Name = "High", },
                    } 
                },
                TypeMappings = new List<LkkTypeMapping> {
                    new LkkTypeMapping {
                        LkkType = new Mapping { Name = "Lkk card", Id = string.Empty, },
                        VersionOneType = new Mapping { Name = "V1 type", Id = string.Empty, },
                    }
                },
                Settings = new VersionOneSettings(),
            };

            return entity;
        }

        public static BafServiceEntity CreateBafEntity() {
            var entity = new BafServiceEntity {
                BafApiId = "myUserApiId",
                BafSigningKey = "myUserSigningKey",
                Project = "Scope:0",
                FgStatusFieldName = "FgStatusField",
                FgStatusReady = "Ready",
                FgStatusPorted = "Ported",
                FgStatusCompleted = "Completed",
                ItemStatusFieldName = "StoryStatus",
                ItemStatusReady = "Ready",
                ItemStatusPorted = "Ported",
                ItemStatusPurchased = "Purchased",
                ItemStatusNotPurchased = "NotPurchased",
            };

            return entity;
        }
    }
}