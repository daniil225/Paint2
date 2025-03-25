using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

namespace Paint2.ViewModels;

public class GroupsPanelViewModel : ViewModelBase
{
    public ObservableCollection<Node> Nodes { get; }
    public ReactiveCommand<string?, Unit> AddGroupCommand { get; }

    private Node? _selectedNode;
    public Node? SelectedNode
    {
        get => _selectedNode;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedNode, value);
            _mainWindow.SelectedFigure = _selectedNode?.NodeSceneObject as IFigure;
        }
    }

    private readonly MainWindowViewModel _mainWindow;

    public GroupsPanelViewModel(MainWindowViewModel mainWindow)
    {
        _mainWindow = mainWindow;
        Nodes = [];

        mainWindow.WhenAnyValue(mvm => mvm.SelectedFigure)
            .Subscribe(f =>
            {
                _selectedNode = f is null ? null : FindNode(f);
                this.RaisePropertyChanged(nameof(SelectedNode));
            });

        AddGroupCommand = ReactiveCommand.CreateFromTask(async (string? name) =>
        {
            await Task.Run(() => AddRootNode(name));
        });

        foreach (Node node in Nodes)
        {
            node.NodeDeleted += OnNodeDeleted;
        }
    }

    private void AddRootNode(string? name)
    {
        string newGroupName = name ?? "New group";
        Node newNode = new(newGroupName, _mainWindow);
        Nodes.Add(newNode);
        FigureGraphicProperties properties = new()
        {
            SolidColor = DefaultGraphicProperties.StandardGroupSolidColor,
            BorderColor = DefaultGraphicProperties.StandardGroupBorderColor,
            BorderThickness = DefaultGraphicProperties.StandardGroupBorderThickness,
            BorderStyle = []
        };
        newNode.NodeSceneObject = Scene.Current.CreateGroup(newGroupName, properties);
        newNode.NodeDeleted += OnNodeDeleted;
    }

    private void OnNodeDeleted(Node deletedNode)
    {
        Scene.Current.RemoveObject(deletedNode.NodeSceneObject);
        Nodes.Remove(deletedNode);
    }

    private Node? FindNode(ISceneObject sceneObject)
    {
        Stack<IEnumerator<Node>> stack = new();
        foreach (Node node in Nodes)
        {
            if (node.NodeSceneObject == sceneObject)
            {
                return node;
            }
            stack.Push(node.SubNodes.GetEnumerator());
            while (stack.Count > 0)
            {
                IEnumerator<Node> enumerator = stack.Peek();
                if (enumerator.MoveNext())
                {
                    Node currentNode = enumerator.Current;
                    if (currentNode.NodeSceneObject == sceneObject)
                    {
                        return currentNode;
                    }
                    if (currentNode.SubNodes.Count > 0)
                    {
                        stack.Push(currentNode.SubNodes.GetEnumerator());
                    }
                }
                else
                {
                    stack.Pop();
                }
            }
        }
        return null;
    }
}

public class Node : ReactiveObject
{
    private string _title;

    public string Title
    {
        get => _title;
        set
        {
            this.RaiseAndSetIfChanged(ref _title, value);
            if (NodeSceneObject is null)
            {
                return;
            }
            NodeSceneObject.Name = value;
        }
    }
    [Reactive] public bool IsEditing { get; set; }
    public ObservableCollection<Node> SubNodes { get; }
    public ReactiveCommand<ISceneObject?, Unit> AddCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public Node? Parent { get; set; }
    public ISceneObject NodeSceneObject { get; set; }

    public event Action<Node>? NodeDeleted;

    private MainWindowViewModel _mainWindow;

    public Node(string title, MainWindowViewModel mainWindow)
    {
        Title = title;
        SubNodes = [];
        _mainWindow = mainWindow;

        AddCommand = ReactiveCommand.CreateFromTask(
            async (ISceneObject? sceneObject) =>
            {
                await Task.Run(() => Add(sceneObject));
            });
        DeleteCommand = ReactiveCommand.Create(Delete);
    }

    public Node(string title, ObservableCollection<Node> subNodes)
    {
        Title = title;
        SubNodes = subNodes;

        AddCommand = ReactiveCommand.CreateFromTask(
            async (ISceneObject? sceneObject) =>
            {
                await Task.Run(() => Add(sceneObject));
            });
        DeleteCommand = ReactiveCommand.Create(Delete);

        foreach (Node child in SubNodes)
        {
            child.Parent = this;
        }
    }

    private void Add(ISceneObject? sceneObject)
    {
        if (sceneObject is null)
        {
            if (NodeSceneObject is not Group parentGroup)
            {
                return;
            }
            _mainWindow.CreateFigureInGroupCommand.Execute(parentGroup)
                .Subscribe(so =>
                {
                    if (so is null)
                    {
                        return;
                    }
                    Node newNode = new(so.Name, _mainWindow) { NodeSceneObject = so, Parent = this };
                    SubNodes.Add(newNode);
                });
            return;
        }
        Node newNode = new(sceneObject.Name, _mainWindow) { NodeSceneObject = sceneObject, Parent = this };
        SubNodes.Add(newNode);
    }

    private void Delete()
    {
        // Если у узла есть родитель - удаляем его
        if (Parent != null)
        {
            Parent.SubNodes.Remove(this);
            Scene.Current.RemoveObject(NodeSceneObject);
            return;
        }

        // Иначе - отправляем событие удаления корневого узла
        NodeDeleted?.Invoke(this);
    }

    public void OnNodeDoubleClick()
    {
        IsEditing = true;
    }

    public void OnTextBoxLostFocus()
    {
        IsEditing = false;
    }
}