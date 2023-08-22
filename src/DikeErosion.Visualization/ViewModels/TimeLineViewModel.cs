using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DikeErosion.Data;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace DikeErosion.Visualization.ViewModels;

public class TimeLineViewModel : ViewModelBase
{
    private readonly DikeErosionProject project;
    private OutputLocation? selectedOutputLocation;
    private TimeDependentOutputVariable? selectedOutputVariable;
    private readonly LinearAxis valueAxis;
    private readonly LineSeries variableSeries;

    public TimeLineViewModel(DikeErosionProject project)
    {
        OutputVariables = new ObservableCollection<TimeDependentOutputVariable>();
        OutputLocations = new ObservableCollection<OutputLocation>();

        this.project = project;
        project.PropertyChanged += ProjectPropertyChanged;
        
        PlotModel = new PlotModel();
        Controller = new PlotController();
        valueAxis = new LinearAxis
        {
            MajorGridlineColor = OxyColors.IndianRed,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineThickness = 1.0,
            MinorGridlineColor = OxyColors.PaleVioletRed,
            MinorGridlineStyle = LineStyle.Dot,
            MinorGridlineThickness = 0.5,
            IsZoomEnabled = true,
            IsPanEnabled = true,
            Position = AxisPosition.Left
        };
        variableSeries = new LineSeries
        {
            Color = OxyColors.Black,
            MarkerType = MarkerType.Circle,
            MarkerFill = OxyColors.IndianRed,
            MarkerStroke = OxyColors.Gray,
            MarkerStrokeThickness = 1,
            MarkerSize = 5,
            IsVisible = false
        };

        InitializePlotModel();
    }

    private void ProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(DikeErosionProject.OutputFileName):
                OnPropertyChanged(nameof(OutputVariables));
                OutputVariables.Clear();
                foreach (var variable in project.TimeDependentOutputVariables.Where(v => Type.GetTypeCode(v.ValueType) == TypeCode.Double))
                {
                    OutputVariables.Add(variable);
                }

                OutputLocations.Clear();
                foreach (var location in project.OutputLocations)
                {
                    OutputLocations.Add(location);
                }

                selectedOutputVariable = OutputVariables.FirstOrDefault();
                SelectedOutputLocation = OutputLocations.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedOutputVariable));
                OnPropertyChanged(nameof(SelectedOutputLocation));
                break;
        }
    }

    public override string Title => "Tijdlijn";

    public PlotModel PlotModel { get; }

    public PlotController Controller { get; }

    public ObservableCollection<TimeDependentOutputVariable> OutputVariables { get; }

    public ObservableCollection<OutputLocation> OutputLocations { get; }

    public OutputLocation? SelectedOutputLocation
    {
        get => selectedOutputLocation;
        set
        {
            selectedOutputLocation = value;
            UpdateChartInformation();
        }
    }

    public TimeDependentOutputVariable? SelectedOutputVariable
    {
        get => selectedOutputVariable;
        set
        {
            selectedOutputVariable = value;
            UpdateChartInformation();
        }
    }

    private void UpdateChartInformation()
    {
        valueAxis.Title = "";
        variableSeries.Points.Clear();
        variableSeries.Title = "";
        variableSeries.IsVisible = false;

        if (SelectedOutputVariable != null && SelectedOutputLocation != null)
        {
            variableSeries.Points.AddRange(SelectedOutputVariable.Values
                .Where(v => Math.Abs(v.Coordinate.X - SelectedOutputLocation.Coordinate.X) < 1E-8)
                .Select(v => new DataPoint(v.Time, (double)v.Value)));

            if (variableSeries.Points.Count > 0)
            {
                valueAxis.Title = SelectedOutputVariable.ToTitle();
                variableSeries.Title = SelectedOutputVariable.ToTitle();
                variableSeries.IsVisible = true;
            }
        }
        PlotModel.InvalidatePlot(true);
    }

    private void InitializePlotModel()
    {
        PlotModel.Axes.Add(new LinearAxis
        {
            MajorGridlineColor = OxyColors.Gray,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineThickness = 1.0,
            MinorGridlineColor = OxyColors.LightGray,
            MinorGridlineStyle = LineStyle.Dot,
            MinorGridlineThickness = 0.5,
            IsZoomEnabled = true,
            IsPanEnabled = true,
            Position = AxisPosition.Bottom,
            Title = "Tijd [m]"
        });
        PlotModel.Axes.Add(valueAxis);
        PlotModel.Series.Add(variableSeries);
        PlotModel.Legends.Add(new Legend
        {
            LegendPlacement = LegendPlacement.Inside,
            LegendPosition = LegendPosition.LeftTop,
            LegendBackground = OxyColors.Brown,
            LegendTextColor = OxyColors.White
        });
        
        PlotModel.IsLegendVisible = true;
    }
}