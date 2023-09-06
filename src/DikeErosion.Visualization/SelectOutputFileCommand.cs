using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
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


        }
    }

    public event EventHandler? CanExecuteChanged;
}