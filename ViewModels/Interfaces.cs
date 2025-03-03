using Avalonia.Media;
using System.Collections.Generic;

namespace Interfaces
{
    public class Point
    {
        public double x, y;
    }
    public interface IFigureGraphicProperties
    {
        Color SolidColor { get; }
        Color BorderColor { get; }
    
    }
    public interface IRenderInterface
    {
        
    }

    public interface IFigure
    {
        // Наверное лучше в виде поля оставить, для имени нужен будет ReactiveUI, а через функции вроде так не делают
        string Name { get; set; }

        bool IsInternal(Point p, double eps);
        IFigure Intersect(IFigure other);
        IFigure Union(IFigure other);
        IFigure Subtract(IFigure other);
        void Move(Point vector);
        void Rotate(Point Center, double angle);
        void Scale(double x, double y);
        void Scale(Point Center, double rad);
        void Reflection(Point ax1, Point ax2);
        void Render(IRenderInterface toDraw);
        void SetParameters(IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams);
    }

}
