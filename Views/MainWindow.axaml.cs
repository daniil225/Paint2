using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Paint2.ViewModels;
using Paint2.ViewModels.Enums;
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
        private bool _isDragging;
        private Avalonia.Point _initialMousePosition;
        private Point _initialCanvasPosition;
        
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
            
            _vm?.HeaderPanel
                .WhenAnyValue(hvm => hvm.SelectedZoomMenuItem.Mode)
                .Subscribe(OnZoomModeChanged);
            
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
        
        private void OnZoomModeChanged(ZoomModeEnum newZoomMode)
        {
            if (_vm is null || _canvas is null)
            {
                return;
            }

            if (newZoomMode == ZoomModeEnum.ZoomDefault)
            {
                ResetCanvasZoom();
            }
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

            if (_vm.HeaderPanel.MenuMode == MenuModesEnum.SceneMoveMode && _isDragging)
            {
                var deltaX = point.Position.X - _initialMousePosition.X;
                var deltaY = point.Position.Y - _initialMousePosition.Y;

                // Обновляем сдвиг канваса
                var transformGroup = _canvas?.RenderTransform as TransformGroup ?? new TransformGroup();
                var translateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();
                if (translateTransform is null)
                {
                    translateTransform = new TranslateTransform();
                    transformGroup.Children.Add(translateTransform);
                }

                // Применяем новый сдвиг
                translateTransform.X = _initialCanvasPosition.X + deltaX;
                translateTransform.Y = _initialCanvasPosition.Y + deltaY;

                if (_canvas != null)
                {
                    _canvas.RenderTransform = transformGroup;
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
            
            var point = e.GetCurrentPoint(sender as Control);
            Point pointerCoordinates = new(point.Position.X, point.Position.Y);

            switch (_vm.HeaderPanel.MenuMode)
            {
                case MenuModesEnum.CreationMode:
                    {
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
                case MenuModesEnum.ToggleZoomMode:
                    double scaleFactor;
                    var activeZoomMode = _vm.HeaderPanel.SelectedZoomMenuItem.Mode;

                    switch (activeZoomMode)
                    {
                        case ZoomModeEnum.ZoomIn:
                            scaleFactor = 1.2;
                            break;
                        case ZoomModeEnum.ZoomOut:
                            scaleFactor = 0.8;
                            break;
                        case ZoomModeEnum.ZoomDefault:
                            ResetCanvasZoom();
                            return;
                        default:
                            scaleFactor = 1;
                            break;
                    }
                    ZoomToPoint(pointerCoordinates, scaleFactor);
                    break;
            }
            
            if (_vm.HeaderPanel.MenuMode == MenuModesEnum.SceneMoveMode && e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed)
            {
                _initialMousePosition = point.Position;
                    
                if (_canvas is not null)
                {
                    TransformGroup transformGroup = _canvas.RenderTransform as TransformGroup ?? new TransformGroup();
                    TranslateTransform? translateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();
                    _initialCanvasPosition = translateTransform is not null ? new Point(translateTransform.X, translateTransform.Y) : new Point(0, 0);
                }

                _isDragging = true;
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
        
        private void ZoomToPoint(Point zoomCenter, double scaleFactor)
        {
            if (_canvas is null)
            {
                return;
            }
            
            const double minZoom = 0.4;
            const double maxZoom = 8.0;
            
            TransformGroup transformGroup = _canvas.RenderTransform as TransformGroup ?? new TransformGroup();
            ScaleTransform? scaleTransform = transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault();
            TranslateTransform? translateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();
            
            if (translateTransform is null)
            {
                translateTransform = new TranslateTransform();
                transformGroup.Children.Add(translateTransform);
            }
            
            if (scaleTransform is null)
            {
                scaleTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
                transformGroup.Children.Add(scaleTransform);
            }
            
            double newScaleX = scaleTransform.ScaleX * scaleFactor;
            double newScaleY = scaleTransform.ScaleY * scaleFactor;
            newScaleX = Math.Max(minZoom, Math.Min(maxZoom, newScaleX));
            newScaleY = Math.Max(minZoom, Math.Min(maxZoom, newScaleY));
            scaleTransform.ScaleX = newScaleX;
            scaleTransform.ScaleY = newScaleY;

            translateTransform.X = (_canvas.Bounds.Width / 2) - zoomCenter.X;
            translateTransform.Y = (_canvas.Bounds.Height / 2) - zoomCenter.Y;

            _canvas.RenderTransform = transformGroup;
        }
        
        private void ResetCanvasZoom()
        {
            if (_canvas is null)
            {
                return;
            }

            TransformGroup transformGroup = _canvas.RenderTransform as TransformGroup ?? new TransformGroup();
            
            ScaleTransform? scaleTransform = transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault();
            if (scaleTransform is null)
            {
                scaleTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
                transformGroup.Children.Add(scaleTransform);
            }
            else
            {
                scaleTransform.ScaleX = 1;
                scaleTransform.ScaleY = 1;
            }
            
            TranslateTransform? translateTransform = transformGroup.Children.OfType<TranslateTransform>().FirstOrDefault();
            if (translateTransform is null)
            {
                translateTransform = new TranslateTransform();
                transformGroup.Children.Add(translateTransform);
            }
            else
            {
                translateTransform.X = 0;
                translateTransform.Y = 0;
            }

            _canvas.RenderTransform = transformGroup;
        }

        private void Canvas_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isDragging = false;
        }
    }
}