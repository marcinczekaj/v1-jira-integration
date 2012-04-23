/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using Ninject;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.Core.Eventing {
    public class EventManager : IEventManager {
        private readonly IDictionary<Type, EventDelegate> subscriptions = new Dictionary<Type, EventDelegate>();

        private readonly ILogger logger;

        [Inject]
        public EventManager() {
            logger = new Logger(this);
        }

        public void Publish(object pubobj) {
            EventDelegate subs;
            if(subscriptions.TryGetValue(pubobj.GetType(), out subs)) {
                try {
                    subs(pubobj);
                } catch(Exception ex) {
                    logger.Log("Event Manager Caught Unhandled Exception", ex);
                    logger.Log(ex.Message);

                    //TODO find smart way to make startup validation crash
                    if(pubobj is ServiceHostState && ServiceHostState.Validate.Equals(pubobj)) {
                        throw;
                    }
                }
            }
        }

        public void Subscribe(Type pubtype, EventDelegate listener) {
            EventDelegate subs;
            if(!subscriptions.TryGetValue(pubtype, out subs)) {
                subscriptions[pubtype] = listener;
            } else {
                subscriptions[pubtype] = (EventDelegate)Delegate.Combine(subs, listener);
            }
        }

        public void Unsubscribe(Type pubtype, EventDelegate listener) {
            EventDelegate subscription;

            if(subscriptions.TryGetValue(pubtype, out subscription)) {
                var updatedSubscription = (EventDelegate)Delegate.Remove(subscription, listener);

                if(updatedSubscription == null) {
                    subscriptions.Remove(pubtype);
                    return;
                }

                subscriptions[pubtype] = updatedSubscription;
            }
        }
    }
}