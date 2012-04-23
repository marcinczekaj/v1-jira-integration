using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;
using VersionOne.ServiceHost.Core;
using IntegrationTests.Enviornments;
using VersionOne.ServiceHost.Core.Logging;
using IntegrationTests.Logger;

namespace IntegrationTests.Factory {

    class V1ProcessorFactory {


        private static void InitializeProcessor(IVersionOneProcessor v1processor, string[] properties, string workitem, Boolean isList) {
            foreach (string property in properties) {
                v1processor.AddProperty(property, workitem, isList);
            }
        }

        private static void InitializaProcessorWithDefaultProperites(IVersionOneProcessor v1processor) {

            string [] PrimaryWorkitemProperties_noList = {
                Workitem.CreateDateUtcProperty,
                Workitem.ChangeDateUtcProperty,
                Workitem.NumberProperty,
                Entity.NameProperty,
                Workitem.DescriptionProperty,
                Entity.StatusProperty,
                Workitem.EstimateProperty,
                Workitem.AssetTypeProperty,
                Workitem.ParentNameProperty,
                Workitem.TeamNameProperty,
                Workitem.SprintNameProperty,
                Workitem.OrderProperty,
                Workitem.ReferenceProperty,
                Workitem.OwnersProperty,
            };
            InitializeProcessor(v1processor, PrimaryWorkitemProperties_noList, VersionOneProcessor.PrimaryWorkitemType, false);

            
            string [] PrimaryWorkitemProperties_List = {
                PrimaryWorkitem.PriorityProperty,
                PrimaryWorkitem.StatusProperty,
                PrimaryWorkitem.SourceProperty,
            };
            InitializeProcessor(v1processor, PrimaryWorkitemProperties_List, VersionOneProcessor.PrimaryWorkitemType, true);


            string[] StoryProperties = {
                Entity.NameProperty,
            };
            InitializeProcessor(v1processor, StoryProperties, VersionOneProcessor.StoryType, false);


            string[] FeatureGrouProperties = {
                Entity.NameProperty,
                Workitem.ReferenceProperty,
                VersionOneProcessor.OwnersAttribute,
            };
            InitializeProcessor(v1processor, FeatureGrouProperties, VersionOneProcessor.FeatureGroupType, false);

            string[] AttributeDefinitionProperties = {
                FieldInfo.NameProperty, 
                FieldInfo.AssetTypeProperty, 
                FieldInfo.AttributeTypeProperty,
                FieldInfo.IsReadOnlyProperty,
                FieldInfo.IsRequiredProperty,
            };
            InitializeProcessor(v1processor, AttributeDefinitionProperties, VersionOneProcessor.AttributeDefinitionType, false);


            string [] LinkProperties = {
                Entity.NameProperty,
                Link.UrlProperty,
                Link.OnMenuProperty,                     
            };
            InitializeProcessor(v1processor, LinkProperties, VersionOneProcessor.LinkType, false);


            string [] MemberProperties = {
                Entity.NameProperty,                   
                Member.EmailProperty,
                Member.UsernameProperty,
            };
            InitializeProcessor(v1processor, MemberProperties, VersionOneProcessor.MemberType, false);
        }

        private static void ExtraInitialization(IVersionOneProcessor v1processor) {

            string[] DefectProperties = {
                Entity.StatusProperty,
                Entity.StatusNameProperty

            };
            InitializeProcessor(v1processor, DefectProperties, VersionOneProcessor.DefectType, false);
        }


        public static VersionOneProcessor build(ILogger logger) {
            XmlElement config = Enviornment.instance.GetVersionOneConfiguration();
            
            VersionOneProcessor v1processor = new VersionOneProcessor(config, logger);
            InitializaProcessorWithDefaultProperites(v1processor);
            ExtraInitialization(v1processor);

            return v1processor;
        }

        public static void register() {
            var logger = ComponentRepository.Instance.Resolve<ILogger>();
            if (logger == null)
                throw new Exception("Logger not found in Component Repository");

            var v1process = build(logger);
            ComponentRepository.Instance.Register<IVersionOneProcessor>( v1process );
        }
    }
}
