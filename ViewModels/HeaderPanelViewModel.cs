using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Formats.Json;
using Formats.Svg;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.IO;

namespace Paint2.ViewModels;

public class HeaderPanelViewModel : ViewModelBase
{
    [Reactive] public ObservableCollection<FigureMenuItem> FiguresInMenu { get; set; }
    [Reactive] public ObservableCollection<ZoomOption> ZoomOptions { get; set; }
    [Reactive] public FigureMenuItem SelectedFigureMenuItem { get; set; }

    private string? _currentSavedToPath;
    public string? CurrentSavedToPath
    {
        get => _currentSavedToPath;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentSavedToPath, value);
            _mainWindow.FooterPanel.CurrentDocument = value;
        }
    }
    
    [Reactive] public ZoomOption SelectedZoomMenuItem { get; set; }
    [Reactive] public bool IsCreateButtonChecked { get; set; }
    [Reactive] public bool IsSelectButtonChecked { get; set; }
    [Reactive] public bool IsMoveButtonChecked { get; set; }
    [Reactive] public bool IsRotateButtonChecked { get; set; }
    [Reactive] public bool IsScaleButtonChecked { get; set; }
    [Reactive] public bool IsHorizontalReflectionButtonChecked { get; set; }
    [Reactive] public bool IsVerticalReflectionButtonChecked { get; set; }
    [Reactive] public bool IsLineReflectionButtonChecked { get; set; }
    [Reactive] public bool IsIntersectButtonChecked { get; set; }
    [Reactive] public bool IsUnionButtonChecked { get; set; }
    [Reactive] public bool IsSubtractButtonChecked { get; set; }
    [Reactive] public bool IsSceneMoveButtonChecked { get; set; }
    [Reactive] public bool IsZoomButtonChecked { get; set; }
    
    private MenuModesEnum _menuMode;
    public MenuModesEnum MenuMode
    {
        get => _menuMode;
        private set
        {
            UnCheckAllButtons();
            HandleCheckingButton(value);
            this.RaiseAndSetIfChanged(ref _menuMode, value);
        }
    }
    
    public ReactiveCommand<Unit, Unit> AddFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> MoveFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> RotateFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> ScaleFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> HorizontalReflectionFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> VerticalReflectionFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> LineReflectionFigureCommand { get; }
    public ReactiveCommand<Unit, Unit> IntersectFiguresCommand { get; }
    public ReactiveCommand<Unit, Unit> UnionFiguresCommand { get; }
    public ReactiveCommand<Unit, Unit> SubtractFiguresCommand { get; }
    public ReactiveCommand<Unit, Unit> SceneMoveCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleZoomCommand { get; }
    
    public ReactiveCommand<Unit, Unit> CreateCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }

    private readonly MainWindowViewModel _mainWindow;
    
    public HeaderPanelViewModel(MainWindowViewModel mainWindow)
    {
        _mainWindow = mainWindow;
        UnCheckAllButtons();
        HandleCheckingButton(MenuModesEnum.CreationMode);
        
        FiguresInMenu =
        [
            new FigureMenuItem(
                "/Assets/Figures/rectangle.svg", 
                "Rectangle", 
                StandardFiguresEnum.Rectangle),
            new FigureMenuItem(
                "/Assets/Figures/triangle.svg", 
                "Triangle", 
                StandardFiguresEnum.Triangle),
            new FigureMenuItem(
                "/Assets/Figures/rhombus.svg", 
                "Rhombus", 
                StandardFiguresEnum.Rhombus),
            new FigureMenuItem(
                "/Assets/Figures/trapezoid.svg", 
                "Trapezoid", 
                StandardFiguresEnum.Trapezoid),
            new FigureMenuItem(
                "/Assets/Figures/parallelogram.svg", 
                "Parallelogram", 
                StandardFiguresEnum.Parallelogram),
            new FigureMenuItem(
                "/Assets/Figures/pentagon.svg", 
                "Pentagon", 
                StandardFiguresEnum.Pentagon),
            new FigureMenuItem(
                "/Assets/Figures/circle.svg", 
                "Circle", 
                StandardFiguresEnum.Circle),
            new FigureMenuItem(
                "/Assets/Figures/oval.svg", 
                "Oval", 
                StandardFiguresEnum.Oval),
            new FigureMenuItem(
                "/Assets/Figures/line.svg", 
                "Line", 
                StandardFiguresEnum.Line),
            new FigureMenuItem(
                "/Assets/Figures/bezier-curve.svg", 
                "Cubic Bezier curve", 
                StandardFiguresEnum.CubicBezierCurve)
        ];
        SelectedFigureMenuItem = FiguresInMenu.First();

        ZoomOptions =
        [
            new ZoomOption(
                "Zoom default", 
                "/Assets/ZoomIcons/ZoomDefault.svg",
                ZoomModeEnum.ZoomDefault),
            new ZoomOption(
                "Zoom In",
                "/Assets/ZoomIcons/ZoomIn.svg",
                ZoomModeEnum.ZoomIn),
            new ZoomOption(
                "Zoom Out", 
                "/Assets/ZoomIcons/ZoomOut.svg", 
                ZoomModeEnum.ZoomOut
                )
        ];
        SelectedZoomMenuItem = ZoomOptions.First();

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
        HorizontalReflectionFigureCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.HorizontalReflectionFigureMode;
        });
        VerticalReflectionFigureCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.VerticalReflectionFigureMode;
        });
        LineReflectionFigureCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.LineReflectionFigureMode;
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
        SceneMoveCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.SceneMoveMode;
        });
        ToggleZoomCommand = ReactiveCommand.Create(() =>
        {
            MenuMode = MenuModesEnum.ToggleZoomMode;
        });
        
        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Run(() =>
            {
                if (CurrentSavedToPath is null)
                {
                    return;
                }
                string path = CurrentSavedToPath;
                string extension = Path.GetExtension(path);
                 IExportSnapshot snapshot = extension switch
                 {
                     ".json" => new JsonSnapshot(),
                     ".svg" => new SvgSnapshot(100, 100),
                     _ => new JsonSnapshot()
                 };
                Scene.Current.SaveScene(snapshot, path);
            });
        });
        CreateCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Run(() =>
            {
                SaveCommand.Execute();
                Scene.Current.ResetScene();
                _mainWindow.SelectedFigure = null;
            });
        });
        ExitCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Run(() =>
            {
                SaveCommand.Execute();
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
                {
                    desktopApp.Shutdown();
                }
            });
        });
        OpenCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Run(() =>
            {
                SaveCommand.Execute();
            });
        });
    }
    
    private void UnCheckAllButtons()
    {
        IsCreateButtonChecked = false;
        IsSelectButtonChecked = false;
        IsMoveButtonChecked = false;
        IsRotateButtonChecked = false;
        IsScaleButtonChecked = false;
        IsHorizontalReflectionButtonChecked = false;
        IsVerticalReflectionButtonChecked = false;
        IsLineReflectionButtonChecked = false;
        IsIntersectButtonChecked = false;
        IsUnionButtonChecked = false;
        IsSubtractButtonChecked = false;
        IsSceneMoveButtonChecked = false;
        IsZoomButtonChecked = false;
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
            case MenuModesEnum.HorizontalReflectionFigureMode:
                IsHorizontalReflectionButtonChecked = true;
                break;
            case MenuModesEnum.VerticalReflectionFigureMode:
                IsVerticalReflectionButtonChecked = true;
                break;
            case MenuModesEnum.LineReflectionFigureMode:
                IsLineReflectionButtonChecked = true;
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
            case MenuModesEnum.SceneMoveMode:
                IsSceneMoveButtonChecked = true;
                break;
            case MenuModesEnum.ToggleZoomMode:
                IsZoomButtonChecked = true;
                break;
            default:
                IsCreateButtonChecked = true;
                break;
        }
    }
}

public class FigureMenuItem(string iconPath, string iconName, StandardFiguresEnum figureType)
{
    public string IconPath { get; set; } = iconPath;
    public string IconName { get; set; } = iconName;
    public StandardFiguresEnum FigureType { get; set; } = figureType;
}


public class ZoomOption(string name, string iconPath, ZoomModeEnum mode)
{
    public string Name { get; } = name;
    public string IconPath { get; set; } = iconPath;
    public ZoomModeEnum Mode { get; set; } = mode;
}

public enum MenuModesEnum
{
    CreationMode,
    SelectionMode,
    MoveFigureMode,
    RotateFigureMode,
    ScaleFigureMode,
    HorizontalReflectionFigureMode,
    VerticalReflectionFigureMode,
    LineReflectionFigureMode,
    IntersectFiguresMode,
    UnionFiguresMode,
    SubtractFiguresMode,
    SceneMoveMode,
    ToggleZoomMode
}

public enum StandardFiguresEnum
{
    Rectangle,
    Triangle,
    Rhombus,
    Trapezoid,
    Parallelogram, 
    Pentagon,
    Circle,
    Oval,
    Line,
    CubicBezierCurve
}