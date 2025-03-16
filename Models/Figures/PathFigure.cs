using Avalonia.Media;
using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using static Paint2.Models.Figures.TransformingAlgorithms;

namespace Paint2.Models.Figures
{
    public abstract class PathFigure : IFigure
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
                }
            }
        }
        public float Angle { get; private set; }
        [Reactive] public Geometry Geometry { get; set; }
        public bool IsActive { get; set; }
        public bool IsMirrored { get; set; }

        protected string name;
        protected Group _parentGroup;
        protected IList<IPathElement> pathElements;

        protected PathFigure(Group parentGroup, Point coordinates)
        {
            Coordinates = coordinates;
            IsActive = true;
            IsMirrored = false;
            _parentGroup = parentGroup;
            _parentGroup.childObjects.Add(this);
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
            double a = ax2.X - ax1.X;
            double b = ax2.Y - ax1.Y;
            double c = -(a * ax1.X + b * ax1.Y);

            foreach (var pathElement in pathElements)
            {
                if (pathElement is PathMoveTo pathMove)
                    pathMove.dest = ReflectionPoint(a, b, c, pathMove.dest);
                else if (pathElement is PathLineTo pathLine)
                    pathLine.dest = ReflectionPoint(a, b, c, pathLine.dest);
                else if (pathElement is PathArcTo pathArc)
                {
                    pathArc.dest = ReflectionPoint(a, b, c, pathArc.dest);
                    pathArc.xAxisRotation = ReflectionAngle(ax1, ax2, pathArc.xAxisRotation);
                    pathArc.sweepDirection = pathArc.sweepDirection == SweepDirection.Clockwise ?
                                             SweepDirection.CounterClockwise :
                                             SweepDirection.Clockwise;
                }
                else if (pathElement is PathCubicBezierTo pathCubicBezier)
                {
                    pathCubicBezier.dest = ReflectionPoint(a, b, c, pathCubicBezier.dest);
                    pathCubicBezier.controlPoint1 = ReflectionPoint(a, b, c, pathCubicBezier.controlPoint1);
                    pathCubicBezier.controlPoint2 = ReflectionPoint(a, b, c, pathCubicBezier.controlPoint2);
                }
            }
        }

        public void Move(Point vector)
        {
            foreach (var pathElement in pathElements)
            {
                if (pathElement is PathMoveTo pathMove)
                    pathMove.dest += vector;
                else if (pathElement is PathLineTo pathLine)
                    pathLine.dest += vector;
                else if (pathElement is PathArcTo pathArc)
                    pathArc.dest += vector;
                else if (pathElement is PathCubicBezierTo pathCubicBezier)
                {
                    pathCubicBezier.dest += vector;
                    pathCubicBezier.controlPoint1 += vector;
                    pathCubicBezier.controlPoint2 += vector;
                }
            }
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
