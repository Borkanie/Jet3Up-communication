using Interfaces.Factories;

namespace Jet3UpUnitTests
{
    internal class AerotecJobTest
    {
        private IClientFactory clientFactory;

        private string readMessageFromResources(string resourceFileName)
        {
            try
            {
                return File.ReadAllText(resourceFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read file {resourceFileName} form disk because {ex.ToString()}");
                return null;
            }
        }

        [SetUp]
        public void Setup()
        {
            
            Assert.IsNotNull(clientFactory);
        }

        [Test]
        public void Test1()
        {
            // Arrange
            var expected = readMessageFromResources("MasinaAlbaJob.txt");
            Assert.IsNotNull(expected);
            var client = clientFactory.createClient("0.0.0.0", 8080, "Dummy");

            // Act

            // Assert
            Assert.Pass();
        }
    }
}
