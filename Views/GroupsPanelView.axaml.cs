using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Paint2.ViewModels;
using System;

namespace Paint2.Views;

public partial class GroupsPanelView : UserControl
{
    private Point _dragStartPoint;
    private Node? _draggedNode;

    public GroupsPanelView()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2 && sender is TextBlock { DataContext: Node node })
        {
            node.OnNodeDoubleClick();
        }
        else if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _dragStartPoint = e.GetPosition(this);
        }
    }

    private void InputElement_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        Node? node = (sender as TextBox)?.DataContext as Node;
        node?.OnTextBoxLostFocus();
    }

    private async void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        try
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed ||
                sender is not TextBlock { DataContext: Node node })
            {
                return;
            }

            Point currentPosition = e.GetPosition(this);
            if (Math.Abs(currentPosition.X - _dragStartPoint.X) > 3 ||
                Math.Abs(currentPosition.Y - _dragStartPoint.Y) > 3)
            {
                _draggedNode = node;
                DataObject dataObject = new();
                dataObject.Set("Node", node);

                await DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Move);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains("Node")) return;
        e.DragEffects = DragDropEffects.Move;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains("Node") || sender is not TextBlock { DataContext: Node targetNode })
            return;

        if (_draggedNode == null || _draggedNode == targetNode) return;

        if (DataContext is not GroupsPanelViewModel viewModel) { return; }
        
        Group? targetGroup = targetNode.NodeSceneObject as Group;
        if (targetGroup is null)
        {
            return;
        }

        // Если узел корневой
        if (_draggedNode.Parent == null)
        {
            viewModel.Nodes.Remove(_draggedNode);
        }
        else
        {
            _draggedNode.Parent.SubNodes.Remove(_draggedNode);
        }

        // Устанавливаем нового родителя
        _draggedNode.NodeSceneObject.Parent = targetGroup;
        _draggedNode.Parent = targetNode;
        targetNode.SubNodes.Add(_draggedNode);

        _draggedNode = null;
    }
}