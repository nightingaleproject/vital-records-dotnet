using Xunit;
using canary.Controllers;
using VRDR;
using BFDR;
using System.IO;
using Newtonsoft.Json;
using System;

namespace canary.tests
{
    public class ConnectathonControllerTests
    {
        [Fact]
        public void Index_ReturnsDeathRecordArray()
        {
            // Arrange
            var controller = new ConnectathonController();

            // Act
            var result = controller.IndexVRDR();

            // Assert
            var deathRecordArray = Assert.IsType<DeathRecord[]>(result);
            Assert.Equal(4, result.Length);
        }

        [Fact]
        public void BFDRConnectathon()
        {
            BirthRecord br = new BirthRecord(File.ReadAllText(RecordTests.FixturePath("fixtures/json/BirthRecordR.json")));
            var controller = new ConnectathonController();
            var response = controller.IndexBFDR();
            var result = controller.IndexBFDR();
            BirthRecord romeroConnectathon = result[0];
            string timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(new DateTime(2000, 1, 1)).ToString()[..6];
            if (timeZoneOffset == "00:00:")
            {
                timeZoneOffset = "+00:00";
            }
            Assert.Equal(JsonConvert.SerializeObject(br).Replace("-05:00", timeZoneOffset), JsonConvert.SerializeObject(romeroConnectathon));
        }
    }
}
