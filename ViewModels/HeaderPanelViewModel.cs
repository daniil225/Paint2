using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace Paint2.ViewModels;

public class HeaderPanelViewModel : ViewModelBase
{
    [Reactive] public ObservableCollection<FigureMenuItem> FiguresInMenu { get; set; }
    [Reactive] public FigureMenuItem SelectedFigureMenuItem { get; set; }
    
    [Reactive] public bool IsCreateButtonChecked { get; set; }
    [Reactive] public bool IsSelectButtonChecked { get; set; }
    [Reactive] public bool IsMoveButtonChecked { get; set; }
    [Reactive] public bool IsRotateButtonChecked { get; set; }
    [Reactive] public bool IsScaleButtonChecked { get; set; }
    [Reactive] public bool IsIntersectButtonChecked { get; set; }
    [Reactive] public bool IsUnionButtonChecked { get; set; }
    [Reactive] public bool IsSubtractButtonChecked { get; set; }
    [Reactive] public bool IsZoomInButtonChecked { get; set; }
    [Reactive] public bool IsZoomOutButtonChecked { get; set; }

    private MenuModesEnum _menuMode;
    public MenuModesEnum MenuMode
    {
        get => _menuMode;
        set
        {
            _menuMode = value;
            UnCheckAllButtons();
            HandleCheckingButton(_menuMode);
        }
    }
    
    public ReactiveCommand<Unit, Unit> AddFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> MoveFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> RotateFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> ScaleFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> IntersectFiguresCommand { get; }
    public ReactiveCommand<Unit, Unit> UnionFiguresCommand { get; }
    public ReactiveCommand<Unit, Unit> SubtractFiguresCommand { get; }
    public ReactiveCommand<Unit, Unit> ZoomInCommand { get; }
    public ReactiveCommand<Unit, Unit> ZoomOutCommand { get; }

    public HeaderPanelViewModel(ObservableCollection<GeometryViewModel> figures)
    {
        UnCheckAllButtons();
        HandleCheckingButton(MenuModesEnum.CreationMode);
        
        FiguresInMenu =
        [
            new FigureMenuItem("/Assets/Figures/rectangle.svg", "Rectangle"),
            new FigureMenuItem("/Assets/Figures/triangle.svg", "Triangle"),
            new FigureMenuItem("/Assets/Figures/rhombus.svg", "Rhombus"),
            new FigureMenuItem("/Assets/Figures/trapezoid.svg", "Trapezoid"),
            new FigureMenuItem("/Assets/Figures/parallelogram.svg", "Parallelogram"),
            new FigureMenuItem("/Assets/Figures/pentagon.svg", "Pentagon"),
            new FigureMenuItem("/Assets/Figures/circle.svg", "Circle"),
            new FigureMenuItem("/Assets/Figures/oval.svg", "Oval"),
            new FigureMenuItem("/Assets/Figures/line.svg", "Line"),
            new FigureMenuItem("/Assets/Figures/bezier-curve.svg", "Cubic Bezier curve")
        ];
        SelectedFigureMenuItem = FiguresInMenu.First();

        AddFigureCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.CreationMode;
        });
        SelectFigureCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.SelectionMode;
        });
        MoveFigureCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.MoveFigureMode;
        });
        RotateFigureCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.RotateFigureMode;
        });
        ScaleFigureCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.ScaleFigureMode;
        });
        IntersectFiguresCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.IntersectFiguresMode;
        });
        UnionFiguresCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.UnionFiguresMode;
        });
        SubtractFiguresCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.SubtractFiguresMode;
        });
        ZoomInCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.ZoomInMode;
        });
        ZoomOutCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.ZoomOutMode;
        });
    }
    
    private void UnCheckAllButtons()
    {
        IsCreateButtonChecked = false;
        IsSelectButtonChecked = false;
        IsMoveButtonChecked = false;
        IsRotateButtonChecked = false;
        IsScaleButtonChecked = false;
        IsIntersectButtonChecked = false;
        IsUnionButtonChecked = false;
        IsSubtractButtonChecked = false;
        IsZoomInButtonChecked = false;
        IsZoomOutButtonChecked = false;
    }

    private void HandleCheckingButton(MenuModesEnum mode)
    {
        switch (mode)
        {
            case MenuModesEnum.CreationMode:
                IsCreateButtonChecked = true;
                break;
            case MenuModesEnum.SelectionMode:
                IsSelectButtonChecked = true;
                break;
            case MenuModesEnum.MoveFigureMode:
                IsMoveButtonChecked = true;
                break;
            case MenuModesEnum.RotateFigureMode:
                IsRotateButtonChecked = true;
                break;
            case MenuModesEnum.ScaleFigureMode:
                IsScaleButtonChecked = true;
                break;
            case MenuModesEnum.IntersectFiguresMode:
                IsIntersectButtonChecked = true;
                break;
            case MenuModesEnum.UnionFiguresMode:
                IsUnionButtonChecked = true;
                break;
            case MenuModesEnum.SubtractFiguresMode:
                IsSubtractButtonChecked = true;
                break;
            case MenuModesEnum.ZoomInMode:
                IsZoomInButtonChecked = true;
                break;
            case MenuModesEnum.ZoomOutMode:
                IsZoomOutButtonChecked = true;
                break;
            default:
                IsCreateButtonChecked = true;
                break;
        }
    }
}

public class FigureMenuItem(string iconPath, string iconName)
{
    public string IconPath { get; set; } = iconPath;
    public string IconName { get; set; } = iconName;
}

public enum MenuModesEnum
{
    CreationMode,
    SelectionMode,
    MoveFigureMode,
    RotateFigureMode,
    ScaleFigureMode,
    IntersectFiguresMode,
    UnionFiguresMode,
    SubtractFiguresMode,
    ZoomInMode,
    ZoomOutMode
}