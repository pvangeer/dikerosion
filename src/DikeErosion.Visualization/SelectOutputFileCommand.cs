using System;
using System.IO;
using System.Windows.Input;
using DikeErosion.Data;
using Microsoft.Win32;

namespace DikeErosion.Visualization;

public class SelectOutputFileCommand : ICommand
{
    private readonly DikeErosionProject project;

    public SelectOutputFileCommand(DikeErosionProject project)
    {
        this.project = project;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }
    public void Execute(object? parameter)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Selecteer het uitvoerbestand",
            DefaultExt = "*.json",
            FileName = Path.GetFileName(project.OutputFileName),
            InitialDirectory = Path.GetDirectoryName(project.OutputFileName)
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            project.OutputFileName = dialog.FileName;
            project.OnPropertyChanged(nameof(DikeErosionProject.OutputFileName));

            // TODO: Validate input and gice feedback to the user (use log4net?).
            // TODO: Import values and place on datamodel (also adjust presented data
            //var output = DikeErosionJsonReader.ReadResults(dialog.FileName);
        }
    }

    public event EventHandler? CanExecuteChanged;
}