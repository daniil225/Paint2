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
        Point Coordinates { get; set; }
        Group? Parent { get; set; }
        float Angle { get; set; }
        void Move(Point vector, bool isRaisedProperty = true);
        void Rotate(double angle, Point Center);
        void Scale(double x, double y, Point Center);
        void Scale(double rad, Point Center);
        void Mirror(Point ax1, Point ax2);
        void MirrorHorizontal();
        void MirrorVertical();
    }
}
