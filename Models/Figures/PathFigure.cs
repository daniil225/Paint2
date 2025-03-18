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
                    _parentGroup.SetIfParent(this, false);
                    _parentGroup = value;
                    _parentGroup.SetIfParent(this, true);
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
            _parentGroup.SetIfParent(this, true);

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

            Coordinates = ReflectionPoint(a, b, c, Coordinates);
            IsMirrored = !IsMirrored;
            Render();
        }
        public void MirrorHorizontal()
        {
            Mirror(Coordinates, Coordinates + new Point(1, 0));
        }
        public void MirrorVertical()
        {
            Mirror(Coordinates, Coordinates + new Point(0, 1));
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

            Coordinates += vector;
            Render();
        }

        public void Render() => Geometry = Renderer.RenderPathElements(pathElements);

        public void Rotate(double angle, Point Center)
        {
            double radians = angle * Math.PI / 180;
            double cosAngle = Math.Cos(radians);
            double sinAngle = Math.Sin(radians);

            foreach (var pathElement in pathElements)
            {
                if (pathElement is PathMoveTo pathMove)
                {
                    pathMove.dest = RotatePoint(pathMove.dest, Center, cosAngle, sinAngle);
                }
                else if (pathElement is PathLineTo pathLine)
                {
                    pathLine.dest = RotatePoint(pathLine.dest, Center, cosAngle, sinAngle);
                }
                else if (pathElement is PathArcTo pathArc)
                {
                    pathArc.dest = RotatePoint(pathArc.dest, Center, cosAngle, sinAngle);
                    pathArc.xAxisRotation += angle;
                }
                else if (pathElement is PathCubicBezierTo pathCubicBezier)
                {
                    pathCubicBezier.dest = RotatePoint(pathCubicBezier.dest, Center, cosAngle, sinAngle);
                    pathCubicBezier.controlPoint1 = RotatePoint(pathCubicBezier.controlPoint1, Center, cosAngle, sinAngle);
                    pathCubicBezier.controlPoint2 = RotatePoint(pathCubicBezier.controlPoint2, Center, cosAngle, sinAngle);
                }
            }

            Coordinates = RotatePoint(Coordinates, Center, cosAngle, sinAngle);
            Render();
        }

        public void Scale(double sx, double sy, Point Center)
        {
            foreach (var pathElement in pathElements)
            {
                if (pathElement is PathMoveTo pathMove)
                {
                    pathMove.dest = ScalePoint(pathMove.dest, Center, sx, sy);
                }
                else if (pathElement is PathLineTo pathLine)
                {
                    pathLine.dest = ScalePoint(pathLine.dest, Center, sx, sy);
                }
                else if (pathElement is PathArcTo pathArc)
                {
                    pathArc.dest = ScalePoint(pathArc.dest, Center, sx, sy);
                    pathArc.radiusX *= sx;
                    pathArc.radiusY *= sy;
                }
                else if (pathElement is PathCubicBezierTo pathCubicBezier)
                {
                    pathCubicBezier.dest = ScalePoint(pathCubicBezier.dest, Center, sx, sy);
                    pathCubicBezier.controlPoint1 = ScalePoint(pathCubicBezier.controlPoint1, Center, sx, sy);
                    pathCubicBezier.controlPoint2 = ScalePoint(pathCubicBezier.controlPoint2, Center, sx, sy);
                }
            }

            Coordinates = ScalePoint(Coordinates, Center, sx, sy);
            Render();
        }       

        public void Scale(double rad, Point Center)
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
