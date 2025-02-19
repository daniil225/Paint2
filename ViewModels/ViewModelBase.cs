
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using DynamicData;
using veceditor.MVVM.Model;
using System.Reactive.Linq;

namespace Paint2.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
      string modelName = "sdfsdf";
      public string ModelName { get=>modelName; set=>this.RaiseAndSetIfChanged(ref modelName,value); }

      [Reactive] public string ModelName2 { get; set; }

      [Reactive] public IFigure? SelectedFigure { get; set; }
      public SourceCache<IFigure, string> Figures { get; } = new(figure => figure.Name);

      public ViewModelBase()
      {
         ModelName2 = "asdads";

         Delete = ReactiveCommand.Create(() =>  Figures.Remove(SelectedFigure),
            this.WhenAnyValue(t=>t.SelectedFigure).Select(f=>f != null));
      }


      public ReactiveCommand<Unit,Unit> Delete { get; }


   }
}
