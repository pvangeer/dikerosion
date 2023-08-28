using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using DikeErosion.Data;
using DikeErosion.IO;

namespace DikeErosion.Visualization.ViewModels;

public class RecalculateCommand : ICommand
{
    private readonly DikeErosionProject project;

    public RecalculateCommand(DikeErosionProject project)
    {
        this.project = project;
        project.PropertyChanged += ProjectPropertyChanged;
    }

    public bool CanExecute(object? parameter)
    {
        return !string.IsNullOrWhiteSpace(project.OutputFileName) &&
               Directory.Exists(Path.GetDirectoryName(project.OutputFileName)) &&
               ((project.OverwriteOutput && File.Exists(project.OutputFileName)) || !File.Exists(project.OutputFileName));
    }

    public void Execute(object? parameter)
    {
        var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (string.IsNullOrWhiteSpace(assemblyFolder))
            throw new Exception("Executable not found.");
        var pathToDiKErnel = Path.Combine(assemblyFolder, "DiKErnel", "DiKErnel-cli.exe");

        ProcessStartInfo startInfo = new()
        {
            FileName = pathToDiKErnel,
            Arguments = $"--invoerbestand {project.InputFileName} --uitvoerbestand {project.OutputFileName} --uitvoerniveau fysica",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new()
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        bool result;
        try
        {
            process.Start();
            var reader = process.StandardOutput;
            string output = reader.ReadToEnd();
            Console.WriteLine(output);
            process.WaitForExit(4000); // TODO: which time-out is still ok? Make it input to?
            result = process.ExitCode == 0;
        }
        catch (Exception e)
        {
            result = false;
            // TODO: Log or catch exception.
        }

        if (!result)
            // TODO: Log. Invalid run.
            return;

        var importer = new DikeErosionOutputImporter(project);
        importer.Import(project.OutputFileName);
    }

    public event EventHandler? CanExecuteChanged;

    private void ProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(DikeErosionProject.OutputFileName):
                CanExecuteChanged?.Invoke(project, EventArgs.Empty);
                break;
            case nameof(DikeErosionProject.OverwriteOutput):
                CanExecuteChanged?.Invoke(project, EventArgs.Empty);
                break;
        }
    }
}