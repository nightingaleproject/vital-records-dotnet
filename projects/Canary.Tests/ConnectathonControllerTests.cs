using Xunit;
using canary.Controllers;
using VRDR;
using BFDR;
using System.IO;
using Newtonsoft.Json;

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
            Assert.Equal(JsonConvert.SerializeObject(br), JsonConvert.SerializeObject(romeroConnectathon));
        }
    }
}
