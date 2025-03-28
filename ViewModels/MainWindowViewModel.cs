﻿using Avalonia.Collections;
using Avalonia.Controls;
using DynamicData;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Point = Paint2.ViewModels.Utils.Point;

namespace Paint2.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public GroupsPanelViewModel GroupsPanel { get; }
    public PropertiesPanelViewModel PropertiesPanel { get; }
    public HeaderPanelViewModel HeaderPanel { get; }
    public FooterPanelViewModel FooterPanel { get; }
    
    [Reactive] public IFigure? SelectedFigure { get; set; }
    [Reactive] public bool IsPropertiesPanelVisible { get; set; }
    [Reactive] public bool IsGroupsPanelVisible { get; set; }
    [Reactive] public GridLength PropertiesColumnWidth { get; set; }
    [Reactive] public GridLength GroupsColumnWidth { get; set; }
    public ReactiveCommand<Unit, Unit> HidePropertiesPanelCommand { get; }
    public ReactiveCommand<Unit, Unit> HideGroupsPanelCommand { get; }
    public ReactiveCommand<Point[], Unit> CreateFigureCommand { get; }
    public ReactiveCommand<Group, ISceneObject?> CreateFigureInGroupCommand { get; }
    public ReactiveCommand<Unit, Unit> UndoCommand { get; }
    public ReactiveCommand<Unit, Unit> RedoCommand { get; }
    public ObservableCollection<GeometryViewModel> Figures { get; }
    public Point MovementVector { get; set; }
    public Point PrevPointerCoordinates { get; set; }
    public bool IsReflectionLineComplete { get; set; }
    public List<Point> ReflectionLineCoordinates { get; set; } = [];
    
    public Canvas Canvas { get; set; }

    public MainWindowViewModel()
    {
        Figures = [];
        Scene.CreateScene();
        // Подписываю Figures на обновление иерархии сцены
        Scene.Current.HierarchyChanged += UpdateFigures;
        Scene.Current.SceneReloaded += UpdateNodes;

        HeaderPanel = new HeaderPanelViewModel(this);
        PropertiesPanel = new PropertiesPanelViewModel(this);
        GroupsPanel = new GroupsPanelViewModel(this);
        FooterPanel = new FooterPanelViewModel();
        
        IsPropertiesPanelVisible = true;
        PropertiesColumnWidth = new GridLength(0, GridUnitType.Auto);

        HidePropertiesPanelCommand = ReactiveCommand.CreateFromTask(async() =>
        {
            await Task.Run(() =>
            {
                IsPropertiesPanelVisible = !IsPropertiesPanelVisible;
                PropertiesColumnWidth = IsPropertiesPanelVisible
                    ? new GridLength(0, GridUnitType.Auto)
                    : new GridLength(0);
            });
        });
        
        IsGroupsPanelVisible = true;
        GroupsColumnWidth = new GridLength(0, GridUnitType.Auto);
        
        HideGroupsPanelCommand = ReactiveCommand.CreateFromTask(async() =>
        {
            await Task.Run(() =>
            {
                IsGroupsPanelVisible = !IsGroupsPanelVisible;
                GroupsColumnWidth = IsGroupsPanelVisible
                    ? new GridLength(0, GridUnitType.Auto)
                    : new GridLength(0);
            });
        });
        UndoCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Run(() =>
            {
                HistoryManager.Undo();
            });
        });
        RedoCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Run(() =>
            {
                HistoryManager.Redo();
            });
        });

        CreateFigureCommand = ReactiveCommand.CreateFromTask(async (Point[] pointerCoordinates) =>
        {
            await Task.Run(async () =>
            {
                FigureGraphicProperties defaultProperties = new()
                {
                    SolidColor = DefaultGraphicProperties.StandardFigureSolidColor,
                    BorderColor = DefaultGraphicProperties.StandardFigureBorderColor,
                    BorderThickness = DefaultGraphicProperties.StandardFigureBorderThickness,
                    BorderStyle = []
                };
                Node? defaultNode = GroupsPanel.Nodes.FirstOrDefault(x => x.Title == "Default");
                Node? lastCreateNode;
                if (defaultNode is null)
                {
                    await GroupsPanel.AddGroupCommand.Execute("Default");
                    lastCreateNode = GroupsPanel.Nodes.LastOrDefault();
                }
                else
                {
                    lastCreateNode = defaultNode;
                }
                if (lastCreateNode is null)
                {
                    return;
                }
                Group? group = lastCreateNode.NodeSceneObject as Group;
                if (group is null)
                {
                    return;
                }
                string figureClassName = HeaderPanel.SelectedFigureMenuItem.FigureType.ToString();
                IFigure? figure = FigureFabric.CreateFigure(figureClassName, group, pointerCoordinates, defaultProperties);
                if (figure is null)
                {
                    return;
                }
                await lastCreateNode.AddCommand.Execute(figure);
            });
        });
        
        CreateFigureInGroupCommand = ReactiveCommand.CreateFromTask(async 
            Task<ISceneObject?> (Group group) =>
        {
            return await Task.Run(ISceneObject? () =>
            {
                FigureGraphicProperties defaultProperties = new()
                {
                    SolidColor = DefaultGraphicProperties.StandardFigureSolidColor,
                    BorderColor = DefaultGraphicProperties.StandardFigureBorderColor,
                    BorderThickness = DefaultGraphicProperties.StandardFigureBorderThickness,
                    BorderStyle = []
                };
                if (HeaderPanel.SelectedFigureMenuItem.FigureType is StandardFiguresEnum.CubicBezierCurve)
                {
                    return null;
                }
                if (Canvas is null)
                {
                    return null;
                }
                Point center = new(Canvas.Bounds.Width / 2d, Canvas.Bounds.Height / 2d);
                string figureClassName = HeaderPanel.SelectedFigureMenuItem.FigureType.ToString();
                IFigure? figure = FigureFabric.CreateFigure(figureClassName, group, [center], defaultProperties);
                return figure ?? null;
            });
        });
    }

    private void UpdateFigures(object? sender, PropertyChangedEventArgs e)
    {
        var hierarchy = Scene.Current.Groups;
        var figures = new ObservableCollection<GeometryViewModel>();
        foreach (var obj in hierarchy)
        {
            if (obj is Group group)
            {
                figures.AddRange(ScanBranch(group.ChildObjects));
            }
            else
            {
                figures.Add(new GeometryViewModel((IFigure)obj, this));
            }
        }
        Figures.Clear();
        Figures.AddRange(figures);
    }
    private ObservableCollection<GeometryViewModel> ScanBranch(IReadOnlyList<ISceneObject> branch)
    {
        var figures = new ObservableCollection<GeometryViewModel>();
        foreach (var obj in branch)
        {
            if (obj is Group group)
            {
                figures.AddRange(ScanBranch(group.ChildObjects));
            }
            else
            {
                figures.Add(new GeometryViewModel((IFigure)obj, this) { Figure = (IFigure)obj});
            }
        }
        return figures;
    }
    private void UpdateNodes(object? sender, PropertyChangedEventArgs e)
    {
        GroupsPanel?.ClearSceneHierarchy();
        GroupsPanel?.BuildGroupsBySceneHierarchy();
    }
}