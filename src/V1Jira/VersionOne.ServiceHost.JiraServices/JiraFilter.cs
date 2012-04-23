/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Xml.Serialization;

namespace VersionOne.ServiceHost.JiraServices {
    public class JiraFilter {
        public string Id { get; set; }

        [XmlIgnore]
        public bool Enabled { get; set; }

        [XmlAttribute("disabled")]
        public int DisabledNumeric {
            get { return Convert.ToInt32(!Enabled); }
            set { Enabled = !Convert.ToBoolean(value); }
        }

        public JiraFilter(string id, bool enabled) {
            Id = id;
            Enabled = enabled;
        }
    }
}