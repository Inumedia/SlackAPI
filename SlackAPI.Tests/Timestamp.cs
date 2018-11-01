using Newtonsoft.Json;
using System;
using System.IO;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Unit tests")]
    public class Timestamp
    {
        [Theory]
        [InlineData("12345.000000")]
        [InlineData("12345.900000")]
        [InlineData("12345.123456")]
        [InlineData("12345.123450")]
        [InlineData("12345.12345")]
        [InlineData("12345")]
        public void TestTimestampConversion(string originalTimestamp)
        {
            // Arrange
            JavascriptDateTimeConverter converter = new JavascriptDateTimeConverter();
            var jsonReader = new JsonTextReader(new StringReader($"\"{originalTimestamp}\""));
            jsonReader.Read();

            // Act
            DateTime timestampDateTime = (DateTime)converter.ReadJson(jsonReader, null, null, null);
            var newTimestamp = timestampDateTime.ToProperTimeStamp();

            // Assert
            Assert.Equal(double.Parse(originalTimestamp), double.Parse(newTimestamp));
            Assert.Equal(6, newTimestamp.Substring(newTimestamp.IndexOf(".") + 1).Length);
        }
    }
}
