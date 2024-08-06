using Xunit;
using canary.Controllers;
using VRDR;

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
    }
}
