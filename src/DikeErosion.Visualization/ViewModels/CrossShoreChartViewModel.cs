using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DikeErosion.Data;
using OxyPlot.Axes;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Legends;

namespace DikeErosion.Visualization.ViewModels
{
    public class CrossShoreChartViewModel : ViewModelBase
    {
        private AreaSeries? dikeProfileSeries;
        private readonly Dictionary<ScatterSeries, Annotation> characteristicPointsSeries = new();
        private readonly List<ScatterSeries> outputLocationsSeries = new();
        private double currentTimeStep;
        private LineSeries? waterLevelSeries;
        private AreaSeries? waveHeightSeries;
        private TimeDependentOutputVariable? selectedOutputVariable;
        private readonly List<Series> selectedOutputSeries = new();

        private const string OutputVariableAxisKey = "OutputVariableAxis";
        private readonly LinearAxis selectedOutputAxes = new()
        {
            Position = AxisPosition.Right,
            Key = OutputVariableAxisKey
        };

        private DikeErosionProject Project { get; }

        public CrossShoreChartViewModel(DikeErosionProject project)
        {
            TimeSteps = new double[]{};

            Project = project;
            Project.PropertyChanged += ProjectPropertyChanged;

            PlotModel = InitializePlotModel();

            UpdateTimeSliderAndCurrentTimeStep();
        }

        public override string Title => "Dwarsdoorsnede";

        public PlotModel PlotModel { get; }

        public double CurrentTimeStep
        {
            get => currentTimeStep;
            set
            {
                currentTimeStep = value;
                UpdateHydraulicConditions();
                UpdateSelectedOutputVariableSeriesData();
                PlotModel.InvalidatePlot(true);
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanStepBackInTime));
                OnPropertyChanged(nameof(CanStepForwardInTime));
            }
        }

        private void UpdateSelectedOutputVariableSeriesData()
        {
            if (selectedOutputSeries.Any() && selectedOutputVariable != null)
            {
                switch (Type.GetTypeCode(selectedOutputVariable.ValueType))
                {
                    case TypeCode.Double:
                        var stemSeries = ((StemSeries)selectedOutputSeries[0]);
                        stemSeries.Points.Clear();
                        stemSeries.Points
                            .AddRange(selectedOutputVariable.Values
                                .Where(v => Math.Abs(v.Time - CurrentTimeStep) < 1E-8)
                                .Select(v => new DataPoint(v.Coordinate.X, (double)v.Value)));
                        break;
                    case TypeCode.Boolean:
                        var trueSeries = (ScatterSeries)selectedOutputSeries[0];
                        trueSeries.Points.Clear();
                        trueSeries.Points.AddRange(selectedOutputVariable.Values
                            .Where(v => Math.Abs(v.Time - CurrentTimeStep) < 1E-8 && (bool)v.Value)
                            .Select(v => new ScatterPoint(v.Coordinate.X, v.Coordinate.Z)));

                        var falseSeries = (ScatterSeries)selectedOutputSeries[1];
                        falseSeries.Points.Clear();
                        falseSeries.Points.AddRange(selectedOutputVariable.Values
                            .Where(v => Math.Abs(v.Time - CurrentTimeStep) < 1E-8 && !(bool)v.Value)
                            .Select(v => new ScatterPoint(v.Coordinate.X, v.Coordinate.Z)));
                        break;
                }

                PlotModel.Title = $"Tijdstap {Array.IndexOf(TimeSteps,CurrentTimeStep)}: {CurrentTimeStep:F2} [sec]";
            }
        }

        public double[] TimeSteps { get; set; }

        public ICommand TimeStepBackCommand => new TimeStepBackCommand(this);

        public ICommand TimeStepForwardCommand => new TimeStepForwardCommand(this);

        public bool CanStepBackInTime => TimeSteps.Any() && CurrentTimeStep > TimeSteps.Min();

        public bool CanStepForwardInTime => TimeSteps.Any() && CurrentTimeStep < TimeSteps.Max();

        public TimeDependentOutputVariable[] OutputVariables => Project.TimeDependentOutputVariables.ToArray();

        public TimeDependentOutputVariable? SelectedOutputVariable
        {
            get => selectedOutputVariable;
            set
            {
                selectedOutputVariable = value;
                UpdateSelectedOutputVariableSeries();
                PlotModel.InvalidatePlot(true);
            }
        }

        private void ProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DikeErosionProject.InputFileName):
                    UpdateInputRelatedSeriesAndData();
                    break;
                case nameof(DikeErosionProject.OutputFileName):
                    UpdateOutputRelatedInformation();
                    break;
            }
        }

        private void UpdateSelectedOutputVariableSeries()
        {
            if (selectedOutputSeries.Any())
            {
                foreach (var series in selectedOutputSeries.ToArray())
                {
                    PlotModel.Series.Remove(series);
                    selectedOutputSeries.Remove(series);
                }
                PlotModel.Axes.Remove(selectedOutputAxes);
            }

            if (selectedOutputVariable == null)
            {
                return;
            }

            selectedOutputAxes.Title = selectedOutputVariable.ToTitle();
            PlotModel.Axes.Add(selectedOutputAxes);

            switch (Type.GetTypeCode(selectedOutputVariable.ValueType))
            {
                case TypeCode.Double:
                    var stemSeries = new StemSeries
                    {
                        Title = selectedOutputVariable.ToTitle(),
                        MarkerStroke = OxyColors.Black,
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 5,
                        MarkerFill = OxyColors.Transparent,
                        MarkerStrokeThickness = 1.5,
                        StrokeThickness = 1.5,
                        Dashes = new []{1.0,1},
                        Color = OxyColors.Black,
                        YAxisKey = OutputVariableAxisKey
                    };
                    stemSeries.Points
                        .AddRange(selectedOutputVariable.Values
                            .Where(v => Math.Abs(v.Time - CurrentTimeStep) < 1E-8)
                            .Select(v => new DataPoint(v.Coordinate.X, (double)v.Value)));
                    selectedOutputSeries.Add(stemSeries);
                    PlotModel.Series.Add(stemSeries);
                    break;
                case TypeCode.Boolean:
                    var trueSeries = new ScatterSeries
                    {
                        Title = selectedOutputVariable.ToTitle(),
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 5,
                        MarkerFill = OxyColors.DarkOliveGreen,
                        MarkerStroke = OxyColors.Black,
                        MarkerStrokeThickness = 1,
                    };
                    trueSeries.Points.AddRange(selectedOutputVariable.Values
                        .Where(v => Math.Abs(v.Time - CurrentTimeStep) < 1E-8 && (bool)v.Value)
                        .Select(v => new ScatterPoint(v.Coordinate.X, v.Coordinate.Z)));
                    PlotModel.Series.Add(trueSeries);
                    selectedOutputSeries.Add(trueSeries);
                    var falseSeries = new ScatterSeries
                    {
                        Title = selectedOutputVariable.ToTitle(),
                        MarkerType = MarkerType.Cross,
                        MarkerSize = 5,
                        MarkerStroke = OxyColors.DarkRed,
                        MarkerStrokeThickness = 1,
                    };
                    falseSeries.Points.AddRange(selectedOutputVariable.Values
                        .Where(v => Math.Abs(v.Time - CurrentTimeStep) < 1E-8 && !(bool)v.Value)
                        .Select(v => new ScatterPoint(v.Coordinate.X, v.Coordinate.Z)));
                    PlotModel.Series.Add(falseSeries);
                    selectedOutputSeries.Add(falseSeries);
                    break;
            }
            PlotModel.Title = $"Tijdstap {Array.IndexOf(TimeSteps, CurrentTimeStep)}: {CurrentTimeStep:F2} [sec]";
        }

        private void UpdateOutputRelatedInformation()
        {
            UpdateOutputComboboxItems();
        }

        private void UpdateOutputComboboxItems()
        {
            var variable = OutputVariables.FirstOrDefault();
            if (variable != null)
            {
                SelectedOutputVariable = variable;
            }
            OnPropertyChanged(nameof(OutputVariables));
            OnPropertyChanged(nameof(SelectedOutputVariable));
        }

        private PlotModel InitializePlotModel()
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
                Title = "Afstand langs dwarsraai [m]"
            });

            plotModel.Axes.Add(new LinearAxis
            {
                MajorGridlineColor = OxyColors.IndianRed,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineThickness = 1.0,
                MinorGridlineColor = OxyColors.PaleVioletRed,
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineThickness = 0.5,
                IsZoomEnabled = true,
                IsPanEnabled = true,
                Position = AxisPosition.Left,
                Title = "Hoogte [m + NAP]"
            });

            plotModel.Legends.Add(new Legend
            {
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.LeftTop,
                LegendBackground = OxyColors.Brown,
                LegendTextColor = OxyColors.White
            });
            plotModel.IsLegendVisible = true;

            plotModel.Updated += PlotModelUpdating;

            return plotModel;
        }

        private void PlotModelUpdating(object? sender, EventArgs e)
        {
            foreach (var kvPair in characteristicPointsSeries)
            {
                if (kvPair.Key.IsVisible != PlotModel.Annotations.Contains(kvPair.Value))
                {
                    switch (kvPair.Key.IsVisible)
                    {
                        case true:
                            PlotModel.Annotations.Add(kvPair.Value);
                            break;
                        case false:
                            PlotModel.Annotations.Remove(kvPair.Value);
                            break;
                    }
                }
            }
        }

        private void UpdateInputRelatedSeriesAndData()
        {
            UpdateTimeSliderAndCurrentTimeStep();
            UpdateDikeProfileSeries();
            UpdateCharacteristicPointsSeries();
            UpdateOutputLocations();
            UpdateHydraulicConditions();
            PlotModel.InvalidatePlot(true);
        }

        private void UpdateTimeSliderAndCurrentTimeStep()
        {
            TimeSteps = Project.TimeSteps.ToArray();
            currentTimeStep = TimeSteps.Any() ? TimeSteps.Min() : 0.0;
            OnPropertyChanged(nameof(CurrentTimeStep));
            OnPropertyChanged(nameof(TimeSteps));
            OnPropertyChanged(nameof(CanStepBackInTime));
            OnPropertyChanged(nameof(CanStepForwardInTime));
        }

        private void UpdateOutputLocations()
        {
            if (outputLocationsSeries.Any())
            {
                foreach (var series in outputLocationsSeries)
                {
                    PlotModel.Series.Remove(series);
                }
                outputLocationsSeries.Clear();
            }

            var types = Project.OutputLocations.Select(l => l.TopLayerType).Distinct();
            foreach (var type in types)
            {
                var series = new ScatterSeries
                {
                    Title = $"Uitvoerlocaties ({type.ToTitle()})",
                    MarkerType = MarkerType.Circle,
                    MarkerFill = OxyColors.Transparent,
                    MarkerSize = 5,
                    MarkerStroke = type.ToStrokeColor(),
                    MarkerStrokeThickness = 2
                };
                series.Points.AddRange(Project.OutputLocations
                    .Where(l => l.TopLayerType == type)
                    .Select(l => new ScatterPoint(l.Coordinate.X, l.Coordinate.Z, tag: l)));

                PlotModel.Series.Add(series);
                outputLocationsSeries.Add(series);
            }
        }

        private void UpdateHydraulicConditions()
        {
            if (waterLevelSeries != null)
            {
                PlotModel.Series.Remove(waterLevelSeries);
                waterLevelSeries = null;
            }
            if (waveHeightSeries != null)
            {
                PlotModel.Series.Remove(waveHeightSeries);
                waveHeightSeries = null;
            }

            if (Project.HydraulicConditions.Any())
            {
                waterLevelSeries = new LineSeries
                {
                    Title = "Waterstand",
                    Color = OxyColors.DeepSkyBlue,
                    BrokenLineThickness = 2
                };
                var currentTimeStepIndex = Project.TimeSteps.IndexOf(CurrentTimeStep);
                var currentHydraulicCondition = Project.HydraulicConditions[Math.Max(0, currentTimeStepIndex - 1)];
                waterLevelSeries.Points.AddRange(new DataPoint[]
                {
                    new(Project.Profile.Coordinates.Min(c => c.X), currentHydraulicCondition.WaterLevel),
                    new(Project.Profile.Coordinates.Max(c => c.X), currentHydraulicCondition.WaterLevel)
                });
                PlotModel.Series.Insert(0, waterLevelSeries);

                waveHeightSeries = new AreaSeries
                {
                    Color = OxyColors.CadetBlue,
                    Color2 = OxyColors.CadetBlue,
                    Fill = OxyColors.LightBlue,
                    Title = "Golfhoogte"
                };
                var waveHeight = currentHydraulicCondition.WaveHeight;
                waveHeightSeries.Points.AddRange(new DataPoint[]
                {
                    new(Project.Profile.Coordinates.Min(c => c.X), currentHydraulicCondition.WaterLevel + currentHydraulicCondition.WaveHeight/2.0),
                    new(Project.Profile.Coordinates.Max(c => c.X), currentHydraulicCondition.WaterLevel + currentHydraulicCondition.WaveHeight/2.0)
                });
                waveHeightSeries.Points2.AddRange(new DataPoint[]
                {
                    new(Project.Profile.Coordinates.Min(c => c.X), currentHydraulicCondition.WaterLevel - currentHydraulicCondition.WaveHeight/2.0),
                    new(Project.Profile.Coordinates.Max(c => c.X), currentHydraulicCondition.WaterLevel - currentHydraulicCondition.WaveHeight/2.0)
                });
                PlotModel.Series.Insert(0, waveHeightSeries);
            }
        }

        private void UpdateCharacteristicPointsSeries()
        {
            if (characteristicPointsSeries.Any())
            {
                foreach (var series in characteristicPointsSeries.ToArray())
                {
                    PlotModel.Series.Remove(series.Key);
                    PlotModel.Annotations.Remove(series.Value);
                    characteristicPointsSeries.Remove(series.Key);
                }
            }

            foreach (var characteristicPoint in Project.Profile.CharacteristicPoints)
            {
                var title = characteristicPoint.Type.ToTitle();
                var series = new ScatterSeries
                {
                    Title = title,
                    MarkerType = characteristicPoint.Type.ToMarkerType(),
                    MarkerFill = characteristicPoint.Type.ToColor(),
                    MarkerSize = characteristicPoint.Type.ToMarkerSize(),
                    MarkerStroke = OxyColors.Gray,
                    MarkerStrokeThickness = 1
                };
                series.Points.Add(new ScatterPoint(characteristicPoint.X, characteristicPoint.Z, tag: characteristicPoint.Type));
                PlotModel.Series.Add(series);

                var annotation = new PointAnnotation
                {
                    X = characteristicPoint.X,
                    Y = characteristicPoint.Z,
                    Text = title,
                    Size = 0,
                };
                PlotModel.Annotations.Add(annotation);

                characteristicPointsSeries[series] = annotation;
            }
        }

        private void UpdateDikeProfileSeries()
        {
            if (dikeProfileSeries != null)
            {
                PlotModel.Series.Remove(dikeProfileSeries);
                dikeProfileSeries = null;
            }

            if (Project.Profile.Coordinates.Count <= 2)
            {
                return;
            }

            dikeProfileSeries = new AreaSeries
            {
                Title = "Dwarsprofiel",
                Color = OxyColors.Gray,
                Color2 = OxyColors.Gray,
                Fill = OxyColors.LightGray
            };
            foreach (var coordinate in Project.Profile.Coordinates)
            {

            }
            dikeProfileSeries.Points.AddRange(Project.Profile.Coordinates.Select(c => new DataPoint(c.X, c.Z)));
            var lowerLevel = Project.Profile.Coordinates.Min(c => c.Z) - 2;
            dikeProfileSeries.Points2.AddRange(new DataPoint[]
            {
                new(Project.Profile.Coordinates.Min(c => c.X), lowerLevel),
                new(Project.Profile.Coordinates.Max(c => c.X), lowerLevel),
            });
            PlotModel.Series.Add(dikeProfileSeries);
            PlotModel.InvalidatePlot(true);
        }
    }

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


    public class TimeStepForwardCommand : ICommand
    {
        private readonly CrossShoreChartViewModel viewModel;

        public TimeStepForwardCommand(CrossShoreChartViewModel viewModel)
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
            return viewModel.CanStepForwardInTime;
        }

        public void Execute(object? parameter)
        {
            if (!viewModel.TimeSteps.Any())
            {
                viewModel.CurrentTimeStep = 1;
            }
            else
            {
                viewModel.CurrentTimeStep = viewModel.TimeSteps.Where(t => t > viewModel.CurrentTimeStep).Min();
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}
