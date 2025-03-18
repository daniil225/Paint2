using Avalonia.Threading;
using System.Composition;
using Paint2.ViewModels;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using Formats;

namespace Paint2.Models.Figures
{
    public class Line : PathFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), nameof(Line))]
        class LineCreator : IFigureCreator
        {
            public IFigure Create(Group parentGroup, Point coordinates)
            {
                return new Line(parentGroup, coordinates);
            }
        }
        Line(Group parentGroup, Point coordinates) : base(parentGroup, coordinates)
        {
            Name = "Line";

            double lengthLine = 50.0;

            pathElements.Add(new PathMoveTo() { dest = new Point(coordinates.X - lengthLine / 2.0, coordinates.Y) });
            pathElements.Add(new PathLineTo() { dest = new Point(coordinates.X + lengthLine / 2.0, coordinates.Y) });
            
            Dispatcher.UIThread.Invoke(() => Geometry = Renderer.RenderPathElements(pathElements));
        }
    }
}
