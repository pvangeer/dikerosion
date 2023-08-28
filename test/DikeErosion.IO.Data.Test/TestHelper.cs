namespace DikeErosion.IO.Data.Test;

public static class TestHelper
{
    public static string TestDataDir
    {
        get
        {
            var solutionDirParts = AppDomain.CurrentDomain.BaseDirectory.Split("test\\");
            return Path.Combine(solutionDirParts[0], "test", "testdata");
        }
    }
}