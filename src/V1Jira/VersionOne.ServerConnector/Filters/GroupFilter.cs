/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Linq;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServerConnector.Filters {
    public class GroupFilter : IFilter {
        private readonly IFilter[] filters;
        private readonly FilterActions groupAction;

        private GroupFilter(FilterActions groupAction, params IFilter[] filters) {
            this.groupAction = groupAction;
            this.filters = filters;
        }

        public static GroupFilter And(params IFilter[] filters) {
            return new GroupFilter(FilterActions.And, filters);
        }

        public static GroupFilter Or(params IFilter[] filters) {
            return new GroupFilter(FilterActions.Or, filters);
        }

        public GroupFilterTerm GetFilter(IAssetType type) {
            var filterArray = filters.Select(item => item.GetFilter(type)).ToArray();
            
            switch(groupAction) {
                case FilterActions.And:
                    return new AndFilterTerm(filterArray);
                case FilterActions.Or:
                    return new OrFilterTerm(filterArray);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}