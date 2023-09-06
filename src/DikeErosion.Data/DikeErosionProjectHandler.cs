namespace DikeErosion.Data;

public static class DikeErosionProjectHandler
{
    public static void ClearOutput(DikeErosionProject project)
    {
        project.TimeDependentOutputVariables.Clear();
    }
}