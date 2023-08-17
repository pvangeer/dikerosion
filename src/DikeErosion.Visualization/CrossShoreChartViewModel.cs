using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DikeErosion.Data;
using OxyPlot.Axes;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Legends;

namespace DikeErosion.Visualization
{
    public class CrossShoreChartViewModel : ViewModelBase
    {
        private AreaSeries? dikeProfileSeries;
        private ScatterSeries? characteristicPointsSeries;
        private readonly List<PointAnnotation> characteristicPointAnnotations;
        private ScatterSeries? outputLocationsSeries;
        private double currentTimeStep;
        private LineSeries? waterLevelSeries;
        private AreaSeries? waveHeightSeries;

        private DikeErosionProject Project { get; }

        public CrossShoreChartViewModel(DikeErosionProject project)
        {
            Project = project;
            PlotModel = new PlotModel
            {
                Title = "Dike Erosion Results"
            };
            characteristicPointAnnotations = new List<PointAnnotation>();
            
            InitializePlotModel();

            Project.Profile.Coordinates.CollectionChanged += ProfileCollectionChanged;
            Project.Profile.CharacteristicPoints.CollectionChanged += CharacteristicPointsCollectionChanged;
            Project.OutputLocations.CollectionChanged += OutputLocationsCollectionChanged;
        }

        public PlotModel PlotModel { get; }

        public double CurrentTimeStep
        {
            get => currentTimeStep;
            set
            {
                currentTimeStep = value;
                UpdateHydraulicConditions();
            }
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
        }

        private void OutputLocationsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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
            outputLocationsSeries.Points.AddRange(Project.OutputLocations.Select(l => new ScatterPoint(l.Coordinate.X, l.Coordinate.Z, tag:l)));

            PlotModel.Series.Add(outputLocationsSeries);
            PlotModel.InvalidatePlot(true);
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
                PlotModel.Series.Insert(0,waterLevelSeries);

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
                PlotModel.InvalidatePlot(true);
            }
        }

        private void CharacteristicPointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (characteristicPointsSeries != null)
            {
                PlotModel.Series.Remove(characteristicPointsSeries);
                characteristicPointsSeries = null;
            }

            foreach (var annotation in characteristicPointAnnotations.ToArray())
            {
                PlotModel.Annotations.Remove(annotation);
                characteristicPointAnnotations.Remove(annotation);
            }

            characteristicPointsSeries = new ScatterSeries
            {
                Title = "Karakteristieke punten",
                MarkerType = MarkerType.Diamond
            };
            characteristicPointsSeries.Points.AddRange(
                Project.Profile.CharacteristicPoints.Select(p => new ScatterPoint(p.X, p.Z, tag: p.Type)));
            PlotModel.Series.Add(characteristicPointsSeries);

            foreach (var characteristicPoint in Project.Profile.CharacteristicPoints)
            {
                var annotation = new PointAnnotation
                {
                    X = characteristicPoint.X,
                    Y = characteristicPoint.Z,
                    Text = characteristicPoint.Type.ToString(),
                    Size = 4,
                    Fill = OxyColors.Brown
                };
                characteristicPointAnnotations.Add(annotation);
                PlotModel.Annotations.Add(annotation);
            }

            PlotModel.InvalidatePlot(true);
        }

        //new StemSeries()
        
        private void ProfileCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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
            dikeProfileSeries.Points.AddRange(Project.Profile.Coordinates.Select(c => new DataPoint(c.X,c.Z)));
            var lowerLevel = Project.Profile.Coordinates.Min(c => c.Z) - 2;
            dikeProfileSeries.Points2.AddRange(new DataPoint[]
            {
                new(Project.Profile.Coordinates.Min(c => c.X), lowerLevel),
                new(Project.Profile.Coordinates.Max(c => c.X), lowerLevel),
            });
            PlotModel.Series.Add(dikeProfileSeries);
            PlotModel.InvalidatePlot(true);

            UpdateHydraulicConditions();
        }
    }
}
