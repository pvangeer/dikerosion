using NUnit.Framework;

namespace DikeErosion.IO.Data.Test
{
    [TestFixture]
    public class DikeErosionResultsReaderTest
    {
        [Test]
        public void ReadOutput()
        {
            
            var dataDir = TestHelper.TestDataDir;
            var fileName = Path.Combine(dataDir,"GrassWaveRunupWithMandatoryProperties - test output.json");

            var output = DikeErosionJsonReader.ReadResults(fileName);

            Assert.IsNotNull(output);
            Assert.AreEqual(2, output.Length);
        }

        [Test]
        public void ReadInput()
        {

            var dataDir = TestHelper.TestDataDir;
            var fileName = Path.Combine(dataDir, "GrassWaveRunupWithMandatoryProperties.json");

            var input = DikeErosionJsonReader.ReadInput(fileName);

            Assert.IsNotNull(input);
            Assert.AreEqual(4, input.HydraulicConditions.Count);
        }

    }
}