using Avalonia.Media;
using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System;
using System.Collections.Generic;

namespace Paint2.Models.Figures
{
    public abstract class PathFigure : ReactiveObject, IFigure
    {
        public string Name
        {
            get => name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    name = value;
            }
        }
        public Point Coordinates { get; private set; }
        public Group? Parent
        {
            get => _parentGroup;
            set
            {
                if (value is null)
                    Log.Error($"Попытка удалить родителя у {Name}. Фигуры не могут быть сами по себе.");
                else
                {
                    _parentGroup.childObjects.Remove(this);
                    _parentGroup = value;
                    _parentGroup.childObjects.Add(this);
                    Scene.Current.TriggerOnHeirarchyUpdate();
                }
            }
        }
        public float Angle { get; private set; }
        [Reactive] public Geometry Geometry { get; set; }
        public bool IsActive { get; set; }
        public bool IsMirrored { get; set; }
        public IFigureGraphicProperties? GraphicProperties
        {
            get
            {
                if (_graphicProperties is null)
                    return Parent.GraphicProperties;
                else
                    return _graphicProperties;
            }
            set => this.RaiseAndSetIfChanged(ref _graphicProperties, value);
        }

        protected string name;
        protected Group _parentGroup;
        protected IList<IPathElement> pathElements;
        protected IFigureGraphicProperties? _graphicProperties;

        protected PathFigure(Group parentGroup, Point coordinates)
        {
            Coordinates = coordinates;
            IsActive = true;
            IsMirrored = false;
            _parentGroup = parentGroup;
            _parentGroup.childObjects.Add(this);

            Scene.Current.TriggerOnHeirarchyUpdate();
        }

        public void Export(IExportSnapshot snapshot)
        {
            throw new NotImplementedException();
        }

        public IFigure Intersect(IFigure other)
        {
            throw new NotImplementedException();
        }

        public bool IsInternal(Point p, double eps)
        {
            throw new NotImplementedException();
        }

        public void Mirror(Point ax1, Point ax2)
        {
            throw new NotImplementedException();
        }

        public void Move(Point vector)
        {
            throw new NotImplementedException();
        }

        public void Render(IRenderInterface toDraw)
        {
            throw new NotImplementedException();
        }

        public void Rotate(Point Center, double angle)
        {
            throw new NotImplementedException();
        }

        public void Scale(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void Scale(Point Center, double rad)
        {
            throw new NotImplementedException();
        }

        public void SetParameters(IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams)
        {
            throw new NotImplementedException();
        }

        public IFigure Subtract(IFigure other)
        {
            throw new NotImplementedException();
        }

        public IFigure Union(IFigure other)
        {
            throw new NotImplementedException();
        }
    }
}
