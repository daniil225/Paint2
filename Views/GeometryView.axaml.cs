using Avalonia.Controls;
using Avalonia.Input;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using System;

namespace Paint2.Views
{
    public partial class GeometryView : UserControl
    {
        private GeometryViewModel? _vm;
        private bool _isPointerPressed;
        public GeometryView()
        {
            InitializeComponent();
            this.WhenAnyValue(t => t.DataContext)
                .Subscribe(d => _vm = d as GeometryViewModel);
        }

        private void Path_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _isPointerPressed = true;
            if (_vm is null)
            {
                return;
            }
            
            switch (_vm.MainWindowViewModel.HeaderPanel.MenuMode)
            {
                case MenuModesEnum.SelectionMode:
                    _vm.MainWindowViewModel.SelectedFigure = _vm.Figure;
                    break;
                case MenuModesEnum.MoveFigureMode:
                    _vm.Figure.Move(_vm.MainWindowViewModel.MovementVector);
                    break;
            }
        }

        private void Path_OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_vm is null)
            {
                return;
            }
            
            switch (_vm.MainWindowViewModel.HeaderPanel.MenuMode)
            {
                case MenuModesEnum.MoveFigureMode when _isPointerPressed:
                    _vm.Figure.Move(_vm.MainWindowViewModel.MovementVector);
                    break;
                case MenuModesEnum.RotateFigureMode when _isPointerPressed:
                    {
                        Point center = _vm.Figure.Coordinates;
                        Point movementVector = _vm.MainWindowViewModel.MovementVector;
                        Point inverseMovementVector = -movementVector;
                        Point centerToPointVector = center - _vm.MainWindowViewModel.PrevPointerCoordinates;
                        Point centerToPrevPointVector = inverseMovementVector - centerToPointVector;
                        if (Math.Abs(centerToPointVector.Magnitude()) < 1e-15
                            || Math.Abs(centerToPrevPointVector.Magnitude()) < 1e-15)
                        {
                            return;
                        }
                        double skewProduct = centerToPrevPointVector.X * movementVector.Y -
                                      movementVector.X * centerToPrevPointVector.Y;
                        double angleInRad = Math.Acos(
                            (-centerToPointVector).Normalize() * centerToPrevPointVector.Normalize()) * 180 / Math.PI;
                        if (skewProduct < 0)
                        {
                            angleInRad = -angleInRad;
                        }
                        _vm.Figure.Rotate(angleInRad, center);
                    }
                    break;
                case MenuModesEnum.ScaleFigureMode when _isPointerPressed:
                    {
                        Point center = _vm.Figure.Coordinates;
                        Point movementVector = _vm.MainWindowViewModel.MovementVector;
                        Point inverseMovementVector = -movementVector;
                        Point centerToPointVector = center - _vm.MainWindowViewModel.PrevPointerCoordinates;
                        Point centerToPrevPointVector = inverseMovementVector - centerToPointVector;
                        double lengthPrev = centerToPrevPointVector.Magnitude();
                        double lengthCurrent = centerToPointVector.Magnitude();
                        double scale = lengthCurrent / lengthPrev;
                        _vm.Figure.Scale(scale, scale, center);
                    }
                    break;
            }
        }

        private void Path_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isPointerPressed = false;
        }
    }
}