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
using static Paint2.Models.Figures.TransformingAlgorithms;

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
            pathElements = [];
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

        public void Mirror(Point ax1, Point ax2)
        {
            double a = ax2.X - ax1.X;
            double b = ax2.Y - ax1.Y;
            double c = -(a * ax1.X + b * ax1.Y);

            for (int i = 0; i < pathElements.Count; i++)
            {
                if (pathElements[i] is PathMoveTo pathMove)
                {
                    pathElements[i] = new PathMoveTo() { dest = ReflectionPoint(a, b, c, pathMove.dest) };
                }
                else if (pathElements[i] is PathLineTo pathLine)
                {
                    pathElements[i] = new PathLineTo() { dest = ReflectionPoint(a, b, c, pathLine.dest) };
                }
                else if (pathElements[i] is PathArcTo pathArc)
                {
                    pathArc.dest = ReflectionPoint(a, b, c, pathArc.dest);
                    pathArc.xAxisRotation = ReflectionAngle(ax1, ax2, pathArc.xAxisRotation);
                    pathArc.sweepDirection = pathArc.sweepDirection == SweepDirection.Clockwise ?
                                             SweepDirection.CounterClockwise :
                                             SweepDirection.Clockwise;
                    pathElements[i] = pathArc;
                }
                else if (pathElements[i] is PathCubicBezierTo pathCubicBezier)
                {
                    pathElements[i] = new PathCubicBezierTo()
                    {
                        dest = ReflectionPoint(a, b, c, pathCubicBezier.dest),
                        controlPoint1 = ReflectionPoint(a, b, c, pathCubicBezier.controlPoint1),
                        controlPoint2 = ReflectionPoint(a, b, c, pathCubicBezier.controlPoint2)
                    };
                }
            }

            Coordinates = ReflectionPoint(a, b, c, Coordinates);
            IsMirrored = !IsMirrored;
            Render();
        }

        public void Move(Point vector)
        {
            for (int i = 0; i < pathElements.Count; i++)
            {
                if (pathElements[i] is PathMoveTo pathMove)
                {
                    pathElements[i] = new PathMoveTo() { dest = pathMove.dest + vector };
                }
                else if (pathElements[i] is PathLineTo pathLine)
                {
                    pathElements[i] = new PathLineTo() { dest = pathLine.dest + vector };
                }
                else if (pathElements[i] is PathArcTo pathArc)
                {
                    pathArc.dest += vector;
                    pathElements[i] = pathArc;
                }
                else if (pathElements[i] is PathCubicBezierTo pathCubicBezier)
                {
                    pathElements[i] = new PathCubicBezierTo()
                    {
                        controlPoint1 = pathCubicBezier.controlPoint1 + vector,
                        controlPoint2 = pathCubicBezier.controlPoint2 + vector,
                        dest = pathCubicBezier.dest + vector
                    };
                }
            }

            Coordinates += vector;
            Render();
        }

        public void Render() => Geometry = Renderer.RenderPathElements(pathElements);

        public void Rotate(Point Center, double angle)
        {
            double radians = angle * Math.PI / 180;
            double cosAngle = Math.Cos(radians);
            double sinAngle = Math.Sin(radians);

            for (int i = 0; i < pathElements.Count; i++)
            {
                if (pathElements[i] is PathMoveTo pathMove)
                {
                    pathElements[i] = new PathMoveTo() { dest = RotatePoint(pathMove.dest, Center, cosAngle, sinAngle) };
                }
                else if (pathElements[i] is PathLineTo pathLine)
                {
                    pathElements[i] = new PathLineTo() { dest = RotatePoint(pathLine.dest, Center, cosAngle, sinAngle) };
                }
                else if (pathElements[i] is PathArcTo pathArc)
                {
                    pathArc.dest = RotatePoint(pathArc.dest, Center, cosAngle, sinAngle);
                    pathArc.xAxisRotation += angle;
                    pathElements[i] = pathArc;
                }
                else if (pathElements[i] is PathCubicBezierTo pathCubicBezier)
                {
                    pathElements[i] = new PathCubicBezierTo()
                    {
                        dest = RotatePoint(pathCubicBezier.dest, Center, cosAngle, sinAngle),
                        controlPoint1 = RotatePoint(pathCubicBezier.controlPoint1, Center, cosAngle, sinAngle),
                        controlPoint2 = RotatePoint(pathCubicBezier.controlPoint2, Center, cosAngle, sinAngle)
                    };
                }
            }

            Coordinates = RotatePoint(Coordinates, Center, cosAngle, sinAngle);
            Render();
        }

        public void Scale(Point Center, double sx, double sy)
        {
            for (int i = 0; i < pathElements.Count; i++)
            {
                if (pathElements[i] is PathMoveTo pathMove)
                {
                    pathElements[i] = new PathMoveTo() { dest = ScalePoint(pathMove.dest, Center, sx, sy) };
                }
                else if (pathElements[i] is PathLineTo pathLine)
                {
                    pathElements[i] = new PathLineTo() { dest = ScalePoint(pathLine.dest, Center, sx, sy) };
                }
                else if (pathElements[i] is PathArcTo pathArc)
                {
                    pathArc.dest = ScalePoint(pathArc.dest, Center, sx, sy);
                    pathArc.radiusX *= sx;
                    pathArc.radiusY *= sy;
                    pathElements[i] = pathArc;
                }
                else if (pathElements[i] is PathCubicBezierTo pathCubicBezier)
                {
                    pathElements[i] = new PathCubicBezierTo()
                    {
                        dest = ScalePoint(pathCubicBezier.dest, Center, sx, sy),
                        controlPoint1 = ScalePoint(pathCubicBezier.controlPoint1, Center, sx, sy),
                        controlPoint2 = ScalePoint(pathCubicBezier.controlPoint2, Center, sx, sy)
                    };
                }
            }

            Coordinates = ScalePoint(Coordinates, Center, sx, sy);
            Render();
        }

        public void Scale(Point Center, double rad)
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
