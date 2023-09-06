using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using DikeErosion.Data;
using DikeErosion.IO;

namespace DikeErosion.Visualization;

public class ReadOutputFileCommand : ICommand
{
    private readonly DikeErosionProject project;

    public ReadOutputFileCommand(DikeErosionProject project)
    {
        this.project = project;
        project.PropertyChanged += ProjectPropertyChanged;
    }

    private void ProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(DikeErosionProject.OutputFileName):
                CanExecuteChanged?.Invoke(this, e);
                break;
        }
    }


    public bool CanExecute(object? parameter)
    {
        return !string.IsNullOrWhiteSpace(project.OutputFileName) && 
               File.Exists(project.OutputFileName);
    }

    public void Execute(object? parameter)
    {
        try
        {
            var importer = new DikeErosionOutputImporter(project);
            importer.Import(project.OutputFileName);
            project.OverwriteOutput = false;
            project.OnPropertyChanged(nameof(DikeErosionProject.OverwriteOutput));
        }
        catch (Exception)
        {
            var messageResult =
                MessageBox.Show(
                    "Deze uitvoer hoort niet bij het gespecificeerde invoerbestand. Bestaande uitvoer wordt gewist. Wilt u bij herberkeenen de uitvoer in het geselecteerde bestand overschrijven?",
                    "Uitvoerbestand overschrijven", MessageBoxButton.YesNo);

            switch (messageResult)
            {
                case MessageBoxResult.Cancel:
                    return;
                case MessageBoxResult.Yes:
                    DikeErosionProjectHandler.ClearOutput(project);
                    project.OverwriteOutput = true;
                    project.OnPropertyChanged(nameof(DikeErosionProject.OverwriteOutput));
                    break;
                case MessageBoxResult.No:
                    DikeErosionProjectHandler.ClearOutput(project);
                    project.OverwriteOutput = false;
                    project.OnPropertyChanged(nameof(DikeErosionProject.OverwriteOutput));
                    break;
            }
        }
    }

    public event EventHandler? CanExecuteChanged;
}