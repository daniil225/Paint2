using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicData;
using DynamicData.Alias;
using Paint2.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace Paint2.Views
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
            Draw();
      }
        void Draw()
        {
            Pen shapeOutlinePen = new Pen(Brushes.Black, 2);

            // Create a DrawingGroup
            DrawingGroup dGroup = new DrawingGroup();

            // Obtain a DrawingContext from 
            // the DrawingGroup.
            using (DrawingContext dc = dGroup.Open())
            {
                // Draw a rectangle at full opacity.
                dc.DrawRectangle(Brushes.Blue, shapeOutlinePen, new Rect(0, 0, 25, 25));

                // This rectangle is drawn at 50% opacity.
                dc.DrawRectangle(Brushes.Blue, shapeOutlinePen, new Rect(25, 25, 25, 25));

                // This rectangle is blurred and drawn at 50% opacity (0.5 x 0.5). 
                dc.DrawRectangle(Brushes.Blue, shapeOutlinePen, new Rect(50, 50, 25, 25));

                // This rectangle is also blurred and drawn at 50% opacity.
                dc.DrawRectangle(Brushes.Blue, shapeOutlinePen, new Rect(75, 75, 25, 25));


                // This rectangle is drawn at 50% opacity with no blur effect.
                dc.DrawRectangle(Brushes.Blue, shapeOutlinePen, new Rect(100, 100, 25, 25));
            //                dc.DrawGeometry(Brushes.Red, shapeOutlinePen,new PathGeometry();
            //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/path-markup-syntax?view=netframeworkdesktop-4.8
            }
            DrawingImage drawing = new(dGroup);
            Picture.Source = drawing;
        }
   }
}