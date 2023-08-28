using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using DikeErosion.Data;
using DikeErosion.IO;
using Microsoft.Win32;

namespace DikeErosion.Visualization;

public class SelectOutputFileCommand : ICommand
{
    private readonly DikeErosionProject project;

    public SelectOutputFileCommand(DikeErosionProject project)
    {
        this.project = project;
        project.PropertyChanged += ProjectPropertyChanged;
    }

    public bool CanExecute(object? parameter)
    {
        return !project.OverwriteOutput;
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
            var importer = new DikeErosionOutputImporter(project);
            importer.Import(dialog.FileName);
        }
    }

    public event EventHandler? CanExecuteChanged;

    private void ProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(DikeErosionProject.OverwriteOutput):
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                break;
        }
    }
}