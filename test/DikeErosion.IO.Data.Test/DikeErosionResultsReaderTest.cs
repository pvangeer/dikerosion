using NUnit.Framework;

namespace DikeErosion.IO.Data.Test
{
    [TestFixture]
    public class DikeErosionResultsReaderTest
    {
        [Test]
        [TestCase("Asfaltbekleding-1_0.9_output.json")]
        [TestCase("Asfaltbekleding-1_2.4_output.json")]
        [TestCase("Asfaltbekleding-1_30_output.json")]
        [TestCase("Asfaltbekleding-3_output.json")]
        [TestCase("AsphaltWithMandatoryProperties - test output.json")]
        [TestCase("AsphaltWithOptionalProperties - test output.json")]
        [TestCase("GrassWaveImpactWithMandatoryProperties - test output.json")]
        [TestCase("GrassWaveImpactWithOptionalProperties - test output.json")]
        [TestCase("GrassWaveRunupWithMandatoryProperties - test output.json")]
        [TestCase("GrassWaveRunupWithOptionalProperties - test output.json")]
        [TestCase("NaturalStoneWithMandatoryProperties - test output.json")]
        [TestCase("NaturalStoneWithOptionalProperties - test output.json")]
        public void ReadOutput(string file)
        {
            var output = DikeErosionJsonReader.ReadOutput(Path.Combine(TestHelper.TestDataDir,file));

            Assert.IsNotNull(output);
            Assert.Greater(output.Length,0);
        }

        [Test]
        [TestCase("Asfaltbekleding-1_0.9.json")]
        [TestCase("Asfaltbekleding-1_2.4.json")]
        [TestCase("Asfaltbekleding-1_30.json")]
        [TestCase("Asfaltbekleding-3.json")]
        [TestCase("AsphaltWithMandatoryProperties.json")]
        [TestCase("AsphaltWithOptionalProperties.json")]
        [TestCase("GrassWaveImpactWithMandatoryProperties.json")]
        [TestCase("GrassWaveImpactWithOptionalProperties.json")]
        [TestCase("GrassWaveRunupWithMandatoryProperties.json")]
        [TestCase("GrassWaveRunupWithOptionalProperties.json")]
        [TestCase("NaturalStoneWithMandatoryProperties.json")]
        [TestCase("NaturalStoneWithOptionalProperties.json")]
        public void ReadInput(string file)
        {
            var input = DikeErosionJsonReader.ReadInput(Path.Combine(TestHelper.TestDataDir, file));

            Assert.IsNotNull(input);
            Assert.Greater(input.HydraulicConditions.Count,1);
            Assert.Greater(input.OutputLocations.Count(), 1);
            Assert.IsNotNull(input.Profile);
            Assert.Greater(input.TimeSteps.Count(),1);
        }
    }
}