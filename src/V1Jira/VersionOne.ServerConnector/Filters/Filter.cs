/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServerConnector.Filters {
    public class Filter : IFilter {
        public readonly string Name;
        public readonly FilterActions Operation;

        private readonly IList<FilterValue> values;

        public ReadOnlyCollection<FilterValue> Values {
            get { return values.ToList().AsReadOnly(); }
        }

        private Filter(string name, FilterActions operation) : this(name, operation, new List<FilterValue>()) { }

        private Filter(string name, FilterActions operation, IList<FilterValue> values) {
            Name = name;
            Operation = operation;
            this.values = values;
        }

        public static Filter Or(string fieldName) {
            return new Filter(fieldName, FilterActions.Or);
        }

        public static Filter And(string fieldName) {
            return new Filter(fieldName, FilterActions.And);
        }

        public static Filter Empty() {
            return new Filter(null, FilterActions.Or);
        }

        public static IFilter Equal(string fieldName, object value) {
            return And(fieldName).Equal(value);
        }

        public static IFilter NotEqual(string fieldName, object value) {
            return And(fieldName).NotEqual(value);
        }

        public static IFilter Closed(bool isClosed) {
            var filter = And(VersionOneProcessor.AssetStateAttribute);
            return isClosed ? filter.Equal(AssetState.Closed) : filter.NotEqual(AssetState.Closed);
        }

        public static IFilter Greater(string fieldName, object value) {
            return And(fieldName).Greater(value);
        }

        public Filter Greater(object value) {
            values.Add(new FilterValue(value, FilterValuesActions.Greater));
            return this;
        }
        
        public Filter Equal(object value) {
            values.Add(new FilterValue(value, FilterValuesActions.Equal));
            return this;
        }

        public Filter NotEqual(object value) {
            values.Add(new FilterValue(value, FilterValuesActions.NotEqual));
            return this;
        }

        public static Filter OfTypes(params string[] types) {
            var filter = Or(VersionOneProcessor.AssetTypeAttribute);
            
            foreach(var type in types) {
                filter.Equal(type);
            }

            return filter;
        }

        public GroupFilterTerm GetFilter(IAssetType type) {
            var terms = new List<IFilterTerm>();

            foreach (var value in values) {
                var term = new FilterTerm(type.GetAttributeDefinition(Name));

                switch(value.Action) {
                    case FilterValuesActions.Equal:
                        term.Equal(value.Value);
                        break;
                    case FilterValuesActions.NotEqual:
                        term.NotEqual(value.Value);
                        break;
                    case FilterValuesActions.Greater:
                        term.Greater(value.Value);
                        break;
                    default:
                        throw new NotSupportedException();
                }

                terms.Add(term);
            }

            return Operation == FilterActions.And ? (GroupFilterTerm) new AndFilterTerm(terms.ToArray()) : new OrFilterTerm(terms.ToArray());
        }
    }
}