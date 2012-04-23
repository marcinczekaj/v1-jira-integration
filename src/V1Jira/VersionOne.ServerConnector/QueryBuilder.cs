/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.Linq;
using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;

namespace VersionOne.ServerConnector {
    public class QueryBuilder : IQueryBuilder {
        private IServices services;
        private IMetaModel metaModel;
        private ILocalizer localizer;
        
        private readonly LinkedList<AttributeInfo> attributesToQuery = new LinkedList<AttributeInfo>();
        private readonly EntityFieldTypeResolver typeResolver = new EntityFieldTypeResolver();
        
        public IDictionary<string, PropertyValues> ListPropertyValues { get; private set; }
        public IEntityFieldTypeResolver TypeResolver { get { return typeResolver; } }

        public void Setup(IServices services, IMetaModel metaModel, ILocalizer localizer) {
            this.services = services;
            this.metaModel = metaModel;
            this.localizer = localizer;
            TypeResolver.Reset();
            ListPropertyValues = GetListPropertyValues();
        }

        //TODO why we can't add property(if it's list) to typeResolver?
        public void AddProperty(string attr, string prefix, bool isList) {
            attributesToQuery.AddLast(new AttributeInfo(attr, prefix, isList, false));
        }

        //TODO why we can't add property also to attributesToQuery collection?
        public void AddListProperty(string fieldName, string typeToken) {
            typeResolver.AddMapping(typeToken, fieldName, null);
        }

        /// <summary>
        /// Add not list property which can be doesn't exist at start.
        /// </summary>
        /// <param name="attr">Attribute name</param>
        /// <param name="prefix">Prefix, usually matching attribute type</param>
        public void AddOptionalProperty(string attr, string prefix) {
            attributesToQuery.AddLast(new AttributeInfo(attr, prefix, false, true));
        }

        public AssetList Query(string workitemTypeName, IFilterTerm filter) {
            try {
                var workitemType = metaModel.GetAssetType(workitemTypeName);
                var query = new Query(workitemType) { Filter = filter};

                AddSelection(query, workitemTypeName, workitemType);
                return services.Retrieve(query).Assets;
            } catch (Exception ex) {
                throw new VersionOneException(ex.Message);
            }
        }

        public AssetList Query(string workitemTypeName, IFilter filter) {
            return Query(workitemTypeName, filter.GetFilter(metaModel.GetAssetType(workitemTypeName)));
        }

        private void AddSelection(Query query, string typePrefix, IAssetType type) {
            foreach (var attrInfo in attributesToQuery) {
                if(attrInfo.Prefix != typePrefix) {
                    continue;
                }                
                IAttributeDefinition def;
                
                if (attrInfo.IsOptional) {
                    if (!type.TryGetAttributeDefinition(attrInfo.Attr, out def)) {
                        continue;
                    }
                } else {
                    def = type.GetAttributeDefinition(attrInfo.Attr);
                }
                query.Selection.Add(def);
            }
        }

        private IDictionary<string, PropertyValues> GetListPropertyValues() {
            ProcessUnresolvedTypeMappings();

            var res = new Dictionary<string, PropertyValues>(attributesToQuery.Count);

            foreach(var attrInfo in attributesToQuery) {
                if(!attrInfo.IsList) {
                    continue;
                }

                var propertyAlias = attrInfo.Attr;
                
                if(!attrInfo.Attr.StartsWith(Entity.CustomPrefix)) {
                    propertyAlias = attrInfo.Prefix + propertyAlias;
                }
                
                if(res.ContainsKey(propertyAlias)) {
                    continue;
                }
                
                var propertyName = ResolvePropertyKey(propertyAlias);

                PropertyValues values;
                
                if(res.ContainsKey(propertyName)) {
                    values = res[propertyName];
                } else {
                    values = QueryPropertyValues(propertyName);
                    res.Add(propertyName, values);
                }

                if(!res.ContainsKey(propertyAlias)) {
                    res.Add(propertyAlias, values);
                }
            }

            return res;
        }

        private void ProcessUnresolvedTypeMappings() {
            foreach(var fieldMapping in typeResolver.FieldMappings.ToList()) {
                if(fieldMapping.Value == null) {
                    var attributeParts = fieldMapping.Key.Split('.');
                    var typeName = attributeParts[0];
                    var fieldName = attributeParts[1];
                    var resolvedType = GetFieldType(typeName, fieldName);
                    typeResolver.FieldMappings[fieldMapping.Key] = resolvedType;
                    AddProperty(resolvedType, string.Empty, true);
                }
            }
        }

        private string GetFieldType(string typeToken, string fieldName) {
            var type = metaModel.GetAssetType(typeToken);
            var attributeDefinition = type.GetAttributeDefinition(fieldName);
                
            if(attributeDefinition.AttributeType != AttributeType.Relation) {
                throw new VersionOneException("Not a Relation field");
            }

            return attributeDefinition.RelatedAsset.Token;
        }

        public PropertyValues QueryPropertyValues(string propertyName) {
            var res = new PropertyValues();
            IAttributeDefinition nameDef;
            var query = GetPropertyValuesQuery(propertyName, out nameDef);

            foreach (var asset in services.Retrieve(query).Assets) {
                var name = asset.GetAttribute(nameDef).Value as string;
                res.Add(new ValueId(asset.Oid, name));
            }

            return res;
        }

        public string Localize(string text) {
            return text == null ? "" : localizer.Resolve(text);
        }

        private Query GetPropertyValuesQuery(string propertyName, out IAttributeDefinition nameDef) {
            var assetType = metaModel.GetAssetType(propertyName);
            nameDef = assetType.GetAttributeDefinition(Entity.NameProperty);

            IAttributeDefinition inactiveDef;

            var query = new Query(assetType);
            query.Selection.Add(nameDef);

            if (assetType.TryGetAttributeDefinition(Entity.InactiveProperty, out inactiveDef)) {
                var filter = new FilterTerm(inactiveDef);
                filter.Equal("False");
                query.Filter = filter;
            }

            query.OrderBy.MajorSort(assetType.DefaultOrderBy, OrderBy.Order.Ascending);
            return query;
        }

        private static string ResolvePropertyKey(string propertyAlias) {
            switch (propertyAlias) {
                case "DefectStatus":
                case "PrimaryWorkitemStatus":
                    return "StoryStatus";
                case "PrimaryWorkitemSource":
                case "DefectSource":
                    return "StorySource";
                case "ScopeBuildProjects":
                    return "BuildProject";
                case "TaskOwners":
                case "StoryOwners":
                case "DefectOwners":
                case "TestOwners":
                case "ThemeOwners":
                    return "Member";
                case "PrimaryWorkitemPriority":                      
                    return "WorkitemPriority";
                case "Custom_SeverityLevel":
                    return "Custom_Severity_Level";
            }

            return propertyAlias;
        }
    }
}