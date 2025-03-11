using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Paint2.ViewModels;
using System;

namespace Paint2.Views;

public partial class GroupsPanelView : UserControl
{
    public GroupsPanelView()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount != 2) { return; }
        
        var node = (sender as TextBlock)?.DataContext as Node;
        node?.OnNodeDoubleClick();
    }

    private void InputElement_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var node = (sender as TextBox)?.DataContext as Node;
        node?.OnTextBoxLostFocus();
    }
}