using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace DikeErosion.Visualization.ViewModels;

public class TimeStepForwardCommand : ICommand
{
    private readonly CrossShoreChartViewModel viewModel;

    public TimeStepForwardCommand(CrossShoreChartViewModel viewModel)
    {
        this.viewModel = viewModel;
        this.viewModel.PropertyChanged += ViewModelPropertyChanged;
    }

    public bool CanExecute(object? parameter)
    {
        return viewModel.CanStepForwardInTime;
    }

    public void Execute(object? parameter)
    {
        if (!viewModel.TimeSteps.Any())
            viewModel.CurrentTimeStep = 1;
        else
            viewModel.CurrentTimeStep = viewModel.TimeSteps.Where(t => t > viewModel.CurrentTimeStep).Min();
    }

    public event EventHandler? CanExecuteChanged;

    private void ViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CrossShoreChartViewModel.TimeSteps):
            case nameof(CrossShoreChartViewModel.CurrentTimeStep):
                CanExecuteChanged?.Invoke(this, e);
                break;
        }
    }
}