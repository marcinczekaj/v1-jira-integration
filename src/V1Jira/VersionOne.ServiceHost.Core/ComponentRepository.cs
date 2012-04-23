/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace VersionOne.ServiceHost.Core {
    //TODO remove it when we will have IoC (good IoC).
    public class ComponentRepository {
        private static ComponentRepository instance;
        private readonly IList<object> components = new List<object>();

        private ComponentRepository() { }

        public static ComponentRepository Instance {
            get { return instance ?? (instance = new ComponentRepository()); }
        }

        public T Resolve<T>() where T : class {
            return (T) components.Where(item => item is T).FirstOrDefault();
        }

        public void Register<T>(T component) where T : class {
            if(component == null) {
                throw new ArgumentNullException("component");
            }

            var existing = components.Where(item => item is T).FirstOrDefault();

            if(existing != null) {
                var index = components.IndexOf(existing);
                components[index] = component;
            } else {
                components.Add(component);
            }
        }

        public void Unregister<T>(T component) where T : class {
            if(components.Any(item => item == component)) {
                components.Remove(component);
            }
        }
    }
}