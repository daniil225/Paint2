using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Threading;
using Formats;
using Paint2.ViewModels;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels.Utils;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Runtime.CompilerServices;
using static Paint2.Models.Figures.TransformingAlgorithms;

namespace Paint2.Models.Figures
{
    public partial class PathFigure : ReactiveObject, IFigure
    {
        [Export(typeof(IFigureCreator))]
        [ExportMetadata(nameof(FigureMetadata.Name), "Figure")]
        private class FigureCreator : IFigureCreator
        {
            public virtual IFigure Create(Group parentGroup, Point[] coordinates)
                => throw new NotSupportedException();

            public IFigure Create(Group parentGroup, Point[] coordinates, IList<IPathElement> pathElements, string name)
            {
                PathFigure newFigure = new(parentGroup, coordinates[0])
                {
                    Name = name,
                    pathElements = pathElements
                };

                return newFigure;
            }
        }
        public string Name
        {
            get => name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    name = value;
                this.RaisePropertyChanged();
            }
        }
        bool _supressMove = false;
        public Point Coordinates
        {
            get => coordinates;
            set
            {
                if (!_supressMove)
                    Move(value - coordinates);

                this.RaiseAndSetIfChanged(ref coordinates, value);
            }
        }
        private Point coordinates;
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
                    Scene.Current.OnHierarchyChanged();
                }
            }
        }

        bool isTransform;

        public float Angle
        {
            get => _angle;
            set
            {
                float newAngle = Math.Abs(value) > 180 ? ((value + 180) % 360 + 360) % 360 - 180 : value;
                if (isTransform)
                {
                    this.RaiseAndSetIfChanged(ref _angle, newAngle);
                }
                else
                {
                    Rotate(newAngle - _angle);
                    this.RaiseAndSetIfChanged(ref _angle, newAngle);
                }
            }
        }
        private float _angle;

        public event PropertyChangedEventHandler GeometryChanged;
        [Reactive] public bool IsActive { get; set; }
        public bool IsMirrored { get; set; }
        public bool IsClosed { get; set; }
        public IFigureGraphicProperties? GraphicProperties
        {
            get => _graphicProperties ?? Parent.GraphicProperties;
            set => this.RaiseAndSetIfChanged(ref _graphicProperties, value);
        }
        public IReadOnlyCollection<IPathElement> PathElements => pathElements.AsReadOnly();

        private string name;
        private Group _parentGroup;
        private IList<IPathElement> pathElements;
        private IFigureGraphicProperties? _graphicProperties;

        protected PathFigure(Group parentGroup, Point coordinates)
        {
            pathElements = [];
            _supressMove = true;
            Coordinates = coordinates;
            _supressMove = false;
            IsActive = true;
            IsMirrored = false;
            _parentGroup = parentGroup;
            _parentGroup.SetIfParent(this, true);

            Scene.Current.OnHierarchyChanged();
        }

        private void OnGeometryChanged([CallerMemberName] string prop = "")
        {
            GeometryChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void Export(IExportSnapshot snapshot)
        {
            snapshot.Brush = new(GraphicProperties.BorderColor, GraphicProperties.SolidColor, GraphicProperties.BorderThickness);

            snapshot.AppendPath(new DocPath(pathElements, IsClosed), Coordinates);
        }

        public IFigure Intersect(IFigure other)
        {
            throw new NotImplementedException();
        }

        public void Mirror(Point ax1, Point ax2)
        {
            double a = ax2.Y - ax1.Y;
            double b = -(ax2.X - ax1.X);
            double c = ax2.X * ax1.Y - ax1.X * ax2.Y;

            for (int i = 0; i < pathElements.Count; i++)
            {
                pathElements[i] = pathElements[i] switch
                {
                    PathMoveTo pathMove => new PathMoveTo() { dest = ReflectionPoint(a, b, c, pathMove.dest) },
                    PathLineTo pathLine => new PathLineTo() { dest = ReflectionPoint(a, b, c, pathLine.dest) },
                    PathArcTo pathArc => new PathArcTo()
                    {
                        radiusX = pathArc.radiusX,
                        radiusY = pathArc.radiusY,
                        xAxisRotation = ReflectionAngle(ax1, ax2, pathArc.xAxisRotation),
                        largeArcFlag = pathArc.largeArcFlag,
                        sweepDirection = pathArc.sweepDirection == SweepDirection.Clockwise ?
                                                                   SweepDirection.CounterClockwise :
                                                                   SweepDirection.Clockwise,
                        dest = ReflectionPoint(a, b, c, pathArc.dest)
                    },
                    PathCubicBezierTo pathCubicBezier => new PathCubicBezierTo()
                    {
                        dest = ReflectionPoint(a, b, c, pathCubicBezier.dest),
                        controlPoint1 = ReflectionPoint(a, b, c, pathCubicBezier.controlPoint1),
                        controlPoint2 = ReflectionPoint(a, b, c, pathCubicBezier.controlPoint2)
                    },
                    _ => pathElements[i]
                };
            }

            _supressMove = true;
            Coordinates = ReflectionPoint(a, b, c, Coordinates);
            _supressMove = false;

            float angle = (float)ReflectionAngle(ax1, ax2, _angle);
            float newAngle = Math.Abs(angle) > 180 ? ((angle + 180) % 360 + 360) % 360 - 180 : angle;
            _angle = newAngle;
            this.RaisePropertyChanged(nameof(Angle));

            IsMirrored = !IsMirrored;

            OnGeometryChanged();
        }
        public void MirrorHorizontal()
        {
            Mirror(Coordinates, Coordinates + new Point(1, 0));

            OnGeometryChanged();
        }
        public void MirrorVertical()
        {
            Mirror(Coordinates, Coordinates + new Point(0, 1));

            OnGeometryChanged();
        }

        public void Move(Point vector, bool isRaisedProperty = true)
        {
            for (int i = 0; i < pathElements.Count; i++)
            {
                pathElements[i] = pathElements[i] switch
                {
                    PathMoveTo pathMove => new PathMoveTo() { dest = pathMove.dest + vector },
                    PathLineTo pathLine => new PathLineTo() { dest = pathLine.dest + vector },
                    PathArcTo pathArc => new PathArcTo()
                    {
                        radiusX = pathArc.radiusX,
                        radiusY = pathArc.radiusY,
                        xAxisRotation = pathArc.xAxisRotation,
                        largeArcFlag = pathArc.largeArcFlag,
                        sweepDirection = pathArc.sweepDirection,
                        dest = pathArc.dest + vector,
                    },
                    PathCubicBezierTo pathCubicBezier => new PathCubicBezierTo()
                    {
                        controlPoint1 = pathCubicBezier.controlPoint1 + vector,
                        controlPoint2 = pathCubicBezier.controlPoint2 + vector,
                        dest = pathCubicBezier.dest + vector
                    },
                    _ => pathElements[i]
                };
            }

            if (isRaisedProperty)
            {
                _supressMove = true;
                Coordinates += vector;
                _supressMove = false;
            }
            else
            {
                coordinates += vector;
            }
            _supressMove = false;

            OnGeometryChanged();
        }

        public void Rotate(double angle, Point Center)
        {
            double radians = angle * Math.PI / 180;
            double cosAngle = Math.Cos(radians);
            double sinAngle = Math.Sin(radians);

            RotateElements(pathElements, angle, Center);

            _supressMove = true;
            Coordinates = RotatePoint(Coordinates, Center, cosAngle, sinAngle);
            _supressMove = false;

            isTransform = true;
            Angle += (float)angle;
            isTransform = false;

            OnGeometryChanged();
        }

        private void Rotate(double angle)
        {
            double radians = angle * Math.PI / 180;
            double cosAngle = Math.Cos(radians);
            double sinAngle = Math.Sin(radians);

            RotateElements(pathElements, angle, Coordinates);

            _supressMove = true;
            Coordinates = RotatePoint(Coordinates, Coordinates, cosAngle, sinAngle);
            _supressMove = false;

            OnGeometryChanged();
        }

        public void Scale(double sx, double sy, Point Center)
        {
            for (int i = 0; i < pathElements.Count; i++)
            {
                pathElements[i] = pathElements[i] switch
                {
                    PathMoveTo pathMove => new PathMoveTo() { dest = ScalePoint(pathMove.dest, Center, sx, sy) },
                    PathLineTo pathLine => new PathLineTo() { dest = ScalePoint(pathLine.dest, Center, sx, sy) },
                    PathArcTo pathArc => new PathArcTo()
                    {
                        radiusX = pathArc.radiusX * sx,
                        radiusY = pathArc.radiusY * sy,
                        xAxisRotation = pathArc.xAxisRotation,
                        largeArcFlag = pathArc.largeArcFlag,
                        sweepDirection = pathArc.sweepDirection,
                        dest = ScalePoint(pathArc.dest, Center, sx, sy)
                    },
                    PathCubicBezierTo pathCubicBezier => new PathCubicBezierTo()
                    {
                        dest = ScalePoint(pathCubicBezier.dest, Center, sx, sy),
                        controlPoint1 = ScalePoint(pathCubicBezier.controlPoint1, Center, sx, sy),
                        controlPoint2 = ScalePoint(pathCubicBezier.controlPoint2, Center, sx, sy)
                    },
                    _ => pathElements[i]
                };
            }

            _supressMove = true;
            Coordinates = ScalePoint(Coordinates, Center, sx, sy);
            _supressMove = false;

            OnGeometryChanged();
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
