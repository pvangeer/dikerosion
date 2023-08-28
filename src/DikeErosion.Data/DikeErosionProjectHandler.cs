namespace DikeErosion.Data
{
    public static class DikeErosionProjectHandler
    {
        public static void ClearOutput(DikeErosionProject project)
        {
            project.OutputFileName = "";
            project.TimeDependentOutputVariables.Clear();
            project.OnPropertyChanged(nameof(DikeErosionProject.OutputFileName));
        }
    }
}
