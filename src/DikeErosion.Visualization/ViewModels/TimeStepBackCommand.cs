using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace DikeErosion.Visualization.ViewModels;

public class TimeStepBackCommand : ICommand
{
    private readonly CrossShoreChartViewModel viewModel;

    public TimeStepBackCommand(CrossShoreChartViewModel viewModel)
    {
        this.viewModel = viewModel;
        this.viewModel.PropertyChanged += ViewModelPropertyChanged;
    }

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

    public bool CanExecute(object? parameter)
    {
        return viewModel.CanStepBackInTime;
    }

    public void Execute(object? parameter)
    {
        if (!viewModel.TimeSteps.Any())
        {
            viewModel.CurrentTimeStep = 0;
        }
        else
        {
            viewModel.CurrentTimeStep = viewModel.TimeSteps.Where(t => t < viewModel.CurrentTimeStep).Max();
        }
    }

    public event EventHandler? CanExecuteChanged;
}