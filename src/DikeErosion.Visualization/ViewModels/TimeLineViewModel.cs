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
    private readonly LinearAxis valueAxis;
    private readonly LineSeries variableDoubleSeries;
    private readonly LinearBarSeries variableFalseSeries;
    private readonly LinearBarSeries variableTrueSeries;
    private OutputLocation? selectedOutputLocation;
    private TimeDependentOutputVariable? selectedOutputVariable;

    public TimeLineViewModel(DikeErosionProject project)
    {
        OutputVariables = new ObservableCollection<TimeDependentOutputVariable>();
        OutputLocations = new ObservableCollection<OutputLocation>();

        this.project = project;
        project.PropertyChanged += ProjectPropertyChanged;

        PlotModel = InitializePlotModel();
        valueAxis = InitializeValueAxis();
        variableDoubleSeries = InitializeVariableDoubleSeries();
        variableTrueSeries = InitializeVariableTrueSeries();
        variableFalseSeries = InitializeVariableFalseSeries();

        InitializePlotModel();
    }

    public override string Title => "Tijdlijn";

    public PlotModel PlotModel { get; }

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

    private void ProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(DikeErosionProject.OutputFileName):
                OutputVariables.Clear();
                foreach (var variable in project.TimeDependentOutputVariables.Where(v =>
                             Type.GetTypeCode(v.ValueType) == TypeCode.Double || Type.GetTypeCode(v.ValueType) == TypeCode.Boolean))
                    OutputVariables.Add(variable);

                OutputLocations.Clear();
                foreach (var location in project.OutputLocations)
                    OutputLocations.Add(location);

                selectedOutputVariable = OutputVariables.FirstOrDefault();
                SelectedOutputLocation = OutputLocations.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedOutputVariable));
                OnPropertyChanged(nameof(SelectedOutputLocation));
                break;
        }
    }

    private static PlotModel InitializePlotModel()
    {
        var plotModel = new PlotModel();
        plotModel.Axes.Add(new LinearAxis
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
        plotModel.Legends.Add(new Legend
        {
            LegendPlacement = LegendPlacement.Inside,
            LegendPosition = LegendPosition.LeftTop,
            LegendBackground = OxyColors.Brown,
            LegendTextColor = OxyColors.White
        });

        plotModel.IsLegendVisible = true;

        return plotModel;
    }

    private LinearBarSeries InitializeVariableFalseSeries()
    {
        var series = new LinearBarSeries
        {
            FillColor = OxyColors.DarkRed,
            IsVisible = false
        };
        PlotModel.Series.Add(series);

        return series;
    }

    private LinearBarSeries InitializeVariableTrueSeries()
    {
        var series = new LinearBarSeries
        {
            FillColor = OxyColors.ForestGreen,
            IsVisible = false
        };
        PlotModel.Series.Add(series);

        return series;
    }

    private LineSeries InitializeVariableDoubleSeries()
    {
        var series = new LineSeries
        {
            Color = OxyColors.Black,
            MarkerType = MarkerType.Circle,
            MarkerFill = OxyColors.IndianRed,
            MarkerStroke = OxyColors.Gray,
            MarkerStrokeThickness = 1,
            MarkerSize = 5,
            IsVisible = false
        };
        PlotModel.Series.Add(series);

        return series;
    }

    private LinearAxis InitializeValueAxis()
    {
        var newValueAxis = new LinearAxis
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
        PlotModel.Axes.Add(newValueAxis);

        return newValueAxis;
    }

    private void UpdateChartInformation()
    {
        valueAxis.Title = "";
        variableDoubleSeries.Points.Clear();
        variableDoubleSeries.Title = string.Empty;
        variableDoubleSeries.IsVisible = false;
        variableTrueSeries.Points.Clear();
        variableTrueSeries.Title = string.Empty;
        variableTrueSeries.IsVisible = false;
        variableFalseSeries.Points.Clear();
        variableFalseSeries.Title = string.Empty;
        variableFalseSeries.IsVisible = false;

        if (SelectedOutputVariable != null && SelectedOutputLocation != null)
        {
            var outputVariableValues = SelectedOutputVariable.Values
                .Where(v => Math.Abs(v.Coordinate.X - SelectedOutputLocation.Coordinate.X) < 1E-8 &&
                            v.Time > project.TimeSteps.Min() + 1E-8).ToArray();

            if (Type.GetTypeCode(SelectedOutputVariable.ValueType) == TypeCode.Double)
            {
                variableDoubleSeries.Points.AddRange(
                    outputVariableValues.Select(v => new DataPoint(v.Time, (double)v.Value)));

                if (variableDoubleSeries.Points.Count > 0)
                {
                    valueAxis.Title = SelectedOutputVariable.ToTitle();
                    variableDoubleSeries.Title = SelectedOutputVariable.ToTitle();
                    variableDoubleSeries.IsVisible = true;
                }
            }

            if (Type.GetTypeCode(SelectedOutputVariable.ValueType) == TypeCode.Boolean)
            {
                variableTrueSeries.Points.AddRange(outputVariableValues
                    .Where(v => (bool)v.Value)
                    .Select(v => new DataPoint(v.Time, 1)));
                variableFalseSeries.Points.AddRange(outputVariableValues
                    .Where(v => !(bool)v.Value)
                    .Select(v => new DataPoint(v.Time, -1)));

                if (outputVariableValues.Length > 0)
                {
                    valueAxis.Title = SelectedOutputVariable.ToTitle();
                    variableTrueSeries.Title = $"{SelectedOutputVariable.ToTitle()} (Ja)";
                    variableFalseSeries.Title = $"{SelectedOutputVariable.ToTitle()} (Nee)";
                    variableTrueSeries.IsVisible = true;
                    variableFalseSeries.IsVisible = true;
                }
            }
        }

        PlotModel.InvalidatePlot(true);
    }
}