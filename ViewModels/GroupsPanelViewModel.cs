using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Paint2.ViewModels;

public class GroupsPanelViewModel : ViewModelBase
{
    public ObservableCollection<Node> Nodes { get; }

    public GroupsPanelViewModel()
    {
        Nodes = new ObservableCollection<Node>
        {
            new Node("Figures",
                new ObservableCollection<Node>
                {
                    new Node("Circles",
                        new ObservableCollection<Node>
                        {
                            new Node("Circle 1"),
                            new Node("Circle 2"),
                            new Node("Circle 3")
                        }),
                    new Node("Triangle")
                }),
            new Node("Rectangle")
        };
        
        foreach (Node node in Nodes)
        {
            node.NodeDeleted += OnNodeDeleted;
        }
    }
    
    private void OnNodeDeleted(Node deletedNode)
    {
        Nodes.Remove(deletedNode);
    }
}

public class Node : ReactiveObject
{
    [Reactive] public string Title { get; set; }
    [Reactive] public bool IsEditing { get; set; } = false;
    public ObservableCollection<Node> SubNodes { get; }
    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    private Node? Parent { get; set; }
    
    public event Action<Node>? NodeDeleted;

    public Node(string title)
    {
        Title = title;
        SubNodes = [];

        AddCommand = ReactiveCommand.Create(Add);
        DeleteCommand = ReactiveCommand.Create(Delete);
    }

    public Node(string title, ObservableCollection<Node> subNodes)
    {
        Title = title;
        SubNodes = subNodes;

        AddCommand = ReactiveCommand.Create(Add);
        DeleteCommand = ReactiveCommand.Create(Delete);
        
        foreach (var child in SubNodes)
        {
            child.Parent = this;
        }
    }

    private void Add()
    {
        var newNode = new Node("New Node");
        SubNodes.Add(newNode);
    }

    private void Delete()
    {
        // Если у узла есть родитель - удаляем его
        if (Parent != null)
        {
            Parent.SubNodes.Remove(this);
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