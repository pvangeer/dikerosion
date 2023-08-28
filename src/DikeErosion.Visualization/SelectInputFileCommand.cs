using System;
using System.IO;
using System.Windows.Input;
using DikeErosion.Data;
using DikeErosion.IO;
using Microsoft.Win32;

namespace DikeErosion.Visualization;

public class SelectInputFileCommand : ICommand
{
    private readonly DikeErosionProject project;

    public SelectInputFileCommand(DikeErosionProject project)
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
            Title = "Selecteer het invoerbestand",
            DefaultExt = "*.json",
            FileName = Path.GetFileName(project.InputFileName),
            InitialDirectory = Path.GetDirectoryName(project.InputFileName)
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            var importer = new DikeErosionInputImporter(project);
            importer.Import(dialog.FileName);
        }
    }

    public event EventHandler? CanExecuteChanged;
}