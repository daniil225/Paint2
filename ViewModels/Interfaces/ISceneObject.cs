using Avalonia.Media;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;

namespace Paint2.ViewModels
{
    public interface ISceneObject
    {
        // Наверное лучше в виде поля оставить, для имени нужен будет ReactiveUI, а через функции вроде так не делают
        string Name { get; set; }
        bool IsActive { get; set; }
        bool IsMirrored { get; set; }
        // Координаты pivot'а фигуры
        Point Coordinates { get; }
        float Angle { get; }
        Geometry Geometry { get; set; } // геометрия объекта, хранит то, как объект выглядит на сцене
        void Move(Point vector);
        void Rotate(Point Center, double angle);
        void Scale(double x, double y);
        void Scale(Point Center, double rad);
        void Mirror(Point ax1, Point ax2);
        void Render(IRenderInterface toDraw);
    }
}
