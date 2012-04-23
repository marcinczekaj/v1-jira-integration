using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests {
    [TestClass]
    public class JiraTimeZoneTest {
        
        [TestMethod]
        public void ConvertTime_ValidJiraTimeZone() {
            string jiraTimeZoneName = "Central Standard Time";
            DateTime jiraDate = new DateTime(2012,3,16,7,44,40);

            TimeZoneInfo jiraTimeZone = TimeZoneInfo.GetSystemTimeZones().Where(x => x.Id.Equals(jiraTimeZoneName)).FirstOrDefault();
            DateTime nowUTC = TimeZoneInfo.ConvertTimeToUtc(jiraDate, jiraTimeZone);
            Console.WriteLine(string.Format("converted {0} into {1} UTC ", jiraDate, nowUTC ));
        }

        [TestMethod]
        public void ConvertTime_InvalidJiraTimeZone() {
            string jiraTimeZoneName = "nei wiem nie powiem";
            TimeZoneInfo jiraTimeZone = TimeZoneInfo.GetSystemTimeZones().Where(x => x.Id.Equals(jiraTimeZoneName)).FirstOrDefault();
            Assert.IsNull(jiraTimeZone);
        }

    }
}
