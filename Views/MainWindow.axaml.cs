using System;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Alias;
using Paint2.ViewModels;
using System.Collections.ObjectModel;

namespace veceditor
{
   public partial class MainWindow : Window
   {

      ViewModelBase vm = new ();
      ReadOnlyObservableCollection<string> figures;
      public ReadOnlyObservableCollection<string> Figures => Figures;
      public MainWindow()
      {
         InitializeComponent();
         vm.Figures.Connect().Select(f => f.Name).SortAndBind(out figures);
         vm.Figures.CountChanged.Subscribe(c => { });
      }
   }
}