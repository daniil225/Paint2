using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Paint2.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel? _vm;
        private readonly List<Point> _activeCoordinates = [];
        private readonly ObservableCollection<Point> _toDrawPoints = [];
        private Canvas? _canvas;
        
        public MainWindow()
        {
            InitializeComponent();
            this.WhenAnyValue(t => t.DataContext).Subscribe(OnViewModelChanged);
            _toDrawPoints.CollectionChanged += ToDrawPointsOnCollectionChanged;
        }

        private void OnViewModelChanged(object? vm)
        {
            _vm = vm as MainWindowViewModel;
            _vm?.HeaderPanel
                .WhenAnyValue(
                    hvm => hvm.SelectedFigureMenuItem,
                    hvm => hvm.MenuMode)
                .Subscribe(_ => ClearActiveCoordinates());
            
        }

        private void ClearActiveCoordinates()
        {
            _activeCoordinates.Clear();
            _toDrawPoints.Clear();
            if (_vm is null)
            {
                return;
            }
            _vm.ReflectionLineCoordinates.Clear();
            _vm.IsReflectionLineComplete = false;
        }
        
        private void ToDrawPointsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_canvas is null)
            {
                return;
            }
            Control? pathInCanvas = _canvas.Children.FirstOrDefault(x => x.GetType() == typeof(Path));
            if (pathInCanvas is not null)
            {
                _canvas.Children.Remove(pathInCanvas);
            }
            Path? path = _toDrawPoints.Count switch
            {
                2 => CreateLine(),
                3 => CreateQuadraticBezierCurve(),
                4 => CreateCubicBezierCurve(),
                _ => null
            };
            if (path is not null)
            {
                _canvas.Children.Add(path);
            }
        }

        private Path CreateLine()
        {
            Avalonia.Point aPoint0 = new(_toDrawPoints[0].X, _toDrawPoints[0].Y);
            Avalonia.Point aPoint1 = new(_toDrawPoints[1].X, _toDrawPoints[1].Y);
            LineSegment line = new() { IsStroked = true, Point = aPoint1};
            PathFigure pathFigure = new()
            {
                IsClosed = false, IsFilled = false, StartPoint = aPoint0, Segments = [line]
            };
            PathGeometry pathGeometry = new() { Figures = [pathFigure] };
            SolidColorBrush strokeBrush = new() { Color = DefaultFigureGraphicProperties.ActiveFigureBorderColor };
            return new Path
            {
                StrokeThickness = DefaultFigureGraphicProperties.StandardFigureBorderThickness,
                Data = pathGeometry,
                Stroke = strokeBrush
            };
        }

        private Path CreateQuadraticBezierCurve()
        {
            Avalonia.Point aPoint0 = new(_toDrawPoints[0].X, _toDrawPoints[0].Y);
            Avalonia.Point aPoint1 = new(_toDrawPoints[1].X, _toDrawPoints[1].Y);
            Avalonia.Point aPoint2 = new(_toDrawPoints[2].X, _toDrawPoints[2].Y);
            QuadraticBezierSegment bezierCurve = new() { IsStroked = true, Point1 = aPoint1, Point2 = aPoint2 };
            PathFigure pathFigure = new()
            {
                IsClosed = false, IsFilled = false, StartPoint = aPoint0, Segments = [bezierCurve]
            };
            PathGeometry pathGeometry = new() { Figures = [pathFigure] };
            SolidColorBrush strokeBrush = new() { Color = DefaultFigureGraphicProperties.ActiveFigureBorderColor };
            return new Path
            {
                StrokeThickness = DefaultFigureGraphicProperties.StandardFigureBorderThickness,
                Data = pathGeometry,
                Stroke = strokeBrush
            };
        }

        private Path CreateCubicBezierCurve()
        {
            Avalonia.Point aPoint0 = new(_toDrawPoints[0].X, _toDrawPoints[0].Y);
            Avalonia.Point aPoint1 = new(_toDrawPoints[1].X, _toDrawPoints[1].Y);
            Avalonia.Point aPoint2 = new(_toDrawPoints[2].X, _toDrawPoints[2].Y);
            Avalonia.Point aPoint3 = new(_toDrawPoints[3].X, _toDrawPoints[3].Y);
            BezierSegment bezierCurve = new() { IsStroked = true, Point1 = aPoint1, Point2 = aPoint2, Point3 = aPoint3 };
            PathFigure pathFigure = new()
            {
                IsClosed = false, IsFilled = false, StartPoint = aPoint0, Segments = [bezierCurve]
            };
            PathGeometry pathGeometry = new() { Figures = [pathFigure] };
            SolidColorBrush strokeBrush = new() { Color = DefaultFigureGraphicProperties.ActiveFigureBorderColor };
            return new Path
            {
                StrokeThickness = DefaultFigureGraphicProperties.StandardFigureBorderThickness,
                Data = pathGeometry,
                Stroke = strokeBrush
            };
        }

        private void Canvas_OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            if (_vm is null)
            {
                return;
            }
            _vm.FooterPanel.UpdatePointerCoordinates(point.Position.X, point.Position.Y);
            Point currentPoint = new(point.Position.X, point.Position.Y);
            _vm.MovementVector = currentPoint - _vm.PrevPointerCoordinates;
            _vm.PrevPointerCoordinates = currentPoint;

            if (_activeCoordinates.Count > 0 && _toDrawPoints.Count > 0 && !_vm.IsReflectionLineComplete)
            {
                if (_activeCoordinates.Count == _toDrawPoints.Count)
                {
                    _toDrawPoints.Add(currentPoint);
                }
                else
                {
                    _toDrawPoints[^1] = currentPoint;
                }
            }
        }

        private void Canvas_OnPointerExited(object? sender, PointerEventArgs e)
        {
            _vm?.FooterPanel.ClearPointerCoordinates();
        }

        private void Canvas_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_vm is null)
            {
                return;
            }

            switch (_vm.HeaderPanel.MenuMode)
            {
                case MenuModesEnum.CreationMode:
                    {
                        var point = e.GetCurrentPoint(sender as Control);
                        Point pointerCoordinates = new(point.Position.X, point.Position.Y);

                        if (_vm.HeaderPanel.SelectedFigureMenuItem.FigureType is StandardFiguresEnum.CubicBezierCurve)
                        {
                            _activeCoordinates.Add(pointerCoordinates);
                            _toDrawPoints.Add(pointerCoordinates);
                            if (_activeCoordinates.Count == 4)
                            {
                                _vm.CreateFigureCommand.Execute(_activeCoordinates.ToArray());
                                ClearActiveCoordinates();
                            }
                            break;
                        }
                        
                        _vm.CreateFigureCommand.Execute([pointerCoordinates]);
                        break;
                    }
                case MenuModesEnum.LineReflectionFigureMode when _vm.IsReflectionLineComplete == false:
                    {
                        var point = e.GetCurrentPoint(sender as Control);
                        Point pointerCoordinates = new(point.Position.X, point.Position.Y);
                        _activeCoordinates.Add(pointerCoordinates);
                        _toDrawPoints.Add(pointerCoordinates);
                        _vm.ReflectionLineCoordinates.Add(pointerCoordinates);
                        if (_activeCoordinates.Count == 2)
                        {
                            _vm.IsReflectionLineComplete = true;
                        }
                    }
                    break;
                case MenuModesEnum.SelectionMode:
                    _vm.SelectedFigure = null;
                    break;
            }
        }

        private void Canvas_OnPointerEntered(object? sender, PointerEventArgs e)
        {
            if (_vm is null)
            {
                return;
            }
            var point = e.GetCurrentPoint(sender as Control);
            _vm.PrevPointerCoordinates = new Point(point.Position.X, point.Position.Y);
        }

        private void Canvas_OnLoaded(object? sender, RoutedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                _canvas = canvas;
            }
        }

        private void MainWindow_OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key is Key.Escape
                && _vm?.HeaderPanel.MenuMode is MenuModesEnum.CreationMode or MenuModesEnum.LineReflectionFigureMode)
            {
                ClearActiveCoordinates();
            }
        }
    }
}