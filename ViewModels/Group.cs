using System.Collections.Generic;
using Paint2.ViewModels.Utils;
using Paint2.ViewModels.Interfaces;
using Serilog;

namespace Paint2.ViewModels
{
    public class Group : ISceneObject
    {
        // Видимое имы группы
        public string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _name = value;
            }
        }
        public Group? Parent
        {
            get => _parentGroup;
            set
            {
                _parentGroup?.childObjects.Remove(this);
                if (value is null)
                    _parentGroup = null;
                else
                {
                    _parentGroup = value;
                    _parentGroup.childObjects.Add(this);
                }
            }
        }
        public IList<ISceneObject> childObjects;
        public Point Coordinates { get; private set; }
        public float Angle { get; private set; }
        public bool IsActive { get; set; }
        public bool IsMirrored { get; set; }

        private string _name;
        private Group? _parentGroup;

        public Group(string name)
        {
            _name = name;
            Coordinates = new();
            Angle = 0f;
            childObjects = [];
            IsActive = true;
            IsMirrored = false;
        }
        public void Move(Point vector)
        {
            foreach (ISceneObject obj in childObjects)
                obj.Move(vector);
        }

        public void Rotate(Point Center, double angle)
        {
            foreach (ISceneObject obj in childObjects)
                obj.Rotate(Center, angle);
        }

        public void Scale(double x, double y)
        {
            foreach (ISceneObject obj in childObjects)
                obj.Scale(x, y);
            // Проблемка, каждый объект будет масштабироваться относительно себя, а не центра группы.
            // Как решение - сделать скейл относттельно точки, но не радиальный, а по x и y
        }

        public void Scale(Point Center, double rad)
        {
            foreach (ISceneObject obj in childObjects)
                obj.Scale(Center, rad);
        }

        public void Mirror(Point ax1, Point ax2)
        {
            // Чет не понял зачем здесь две точки и как это должно работать
            throw new System.NotImplementedException();
        }

        public void Render(IRenderInterface toDraw)
        {
            foreach (ISceneObject obj in childObjects)
                obj.Render(toDraw);
        }
    }
}