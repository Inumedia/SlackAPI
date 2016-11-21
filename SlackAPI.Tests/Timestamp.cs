using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestHacks;
using Newtonsoft.Json;
using SlackAPI;
using System;
using System.Collections.Generic;
using System.IO;

namespace IntegrationTest
{
    [TestClass]
    public class Timestamp : TestBase
    {

        private IEnumerable<string> TestTimeStamps
        {
            get
            {
                yield return "12345.000000";
                yield return "12345.900000";
                yield return "12345.123456";
                yield return "12345.123450";
                yield return "12345.12345";
                yield return "12345";
            }
        }

        [TestMethod]
        [DataSource("IntegrationTest.Timestamp.TestTimeStamps")]
        public void TestTimestampConversion()
        {
            // Arrange
            var originalTimestamp = this.TestContext.GetRuntimeDataSourceObject<string>();
            JavascriptDateTimeConverter converter = new JavascriptDateTimeConverter();
            var jsonReader = new JsonTextReader(new StringReader($"\"{originalTimestamp}\""));
            jsonReader.Read();

            // Act
            DateTime timestampDateTime = (DateTime)converter.ReadJson(jsonReader, null, null, null);
            var newTimestamp = timestampDateTime.ToProperTimeStamp();

            // Assert
            Assert.AreEqual(double.Parse(originalTimestamp), double.Parse(originalTimestamp));
            Assert.AreEqual(6, newTimestamp.Substring(newTimestamp.IndexOf(".") + 1).Length);
        }
    }
}
