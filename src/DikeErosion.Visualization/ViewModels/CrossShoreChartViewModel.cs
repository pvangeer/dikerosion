﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ScatterSeries? outputLocationsSeries;
        private double currentTimeStep;
        private LineSeries? waterLevelSeries;
        private AreaSeries? waveHeightSeries;

        private DikeErosionProject Project { get; }

        public CrossShoreChartViewModel(DikeErosionProject project)
        {
            TimeSteps = new double[]{};

            Project = project;
            Project.PropertyChanged += ProjectPropertyChanged;

            PlotModel = new PlotModel
            {
                Title = "Dike Erosion Results"
            };
            Controller = new PlotController();

            InitializePlotModel();

            UpdateTimeSliderAndCurrentTimeStep();
        }

        public PlotController Controller { get; }

        public override string Title => "Dwarsdoorsnede";

        public PlotModel PlotModel { get; }

        public double CurrentTimeStep
        {
            get => currentTimeStep;
            set
            {
                currentTimeStep = value;
                UpdateHydraulicConditions();
                PlotModel.InvalidatePlot(true);
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanStepBackInTime));
                OnPropertyChanged(nameof(CanStepForwardInTime));
            }
        }

        public double[] TimeSteps { get; set; }

        public ICommand TimeStepBackCommand => new TimeStepBackCommand(this);

        public ICommand TimeStepForwardCommand => new TimeStepForwardCommand(this);

        public bool CanStepBackInTime => TimeSteps.Any() && CurrentTimeStep > TimeSteps.Min();

        public bool CanStepForwardInTime => TimeSteps.Any() && CurrentTimeStep < TimeSteps.Max();

        public TimeDependentOutputVariable[] OutputVariables => Project.TimeDependentOutputVariables.ToArray();

        public TimeDependentOutputVariable SelectedOutputVariable { get; set; }

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
                Title = "Afstand langs dwarsraai [m]"
            });

            PlotModel.Axes.Add(new LinearAxis
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

            PlotModel.Legends.Add(new Legend
            {
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.LeftTop,
                LegendBackground = OxyColors.Brown,
                LegendTextColor = OxyColors.White
            });
            PlotModel.IsLegendVisible = true;

            PlotModel.Updated += PlotModelUpdating;

            var command = new DelegatePlotCommand<OxyMouseDownEventArgs>(MouseButtonDown);
            Controller.BindMouseDown(OxyMouseButton.Left, command);
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

        private void MouseButtonDown(IPlotView arg1, IController arg2, OxyMouseDownEventArgs arg3)
        {
            // TODO: Implement if necessary
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
            if (outputLocationsSeries != null)
            {
                PlotModel.Series.Remove(outputLocationsSeries);
                outputLocationsSeries = null;
            }

            outputLocationsSeries = new ScatterSeries
            {
                Title = "Uitvoerlocaties",
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.Transparent,
                MarkerSize = 5,
                MarkerStroke = OxyColors.DarkGray,
                MarkerStrokeThickness = 2
            };
            outputLocationsSeries.Points.AddRange(Project.OutputLocations.Select(l => new ScatterPoint(l.Coordinate.X, l.Coordinate.Z, tag: l)));

            PlotModel.Series.Add(outputLocationsSeries);
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