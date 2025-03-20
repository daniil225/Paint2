using Avalonia;
using Avalonia.Media;
using Formats;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Paint2.ViewModels.Utils;

public static class Renderer
{
    public static Geometry RenderPathElements(IReadOnlyCollection<IPathElement> pathElements)
    {
        PathFigures pathFigures = [];
        PathFigure pathFigure = new() { IsFilled = true, IsClosed = true};
        PathSegments segments = [];
        
        foreach(IPathElement pathElement in pathElements)
        {
            switch (pathElement)
            {
                case PathMoveTo pathMove:
                    {
                        Point startPoint = pathMove.dest;
                        var aStartPoint = new Avalonia.Point(startPoint.X, startPoint.Y);
                        pathFigure.StartPoint = aStartPoint;
                        break;
                    }
                case PathLineTo pathLine:
                    {
                        var aDest = new Avalonia.Point(pathLine.dest.X, pathLine.dest.Y);
                        LineSegment lineSegment = new() { IsStroked = true, Point = aDest };
                        segments.Add(lineSegment);
                        break;
                    }
                case PathArcTo pathArc:
                    {
                        var aDest = new Avalonia.Point(pathArc.dest.X, pathArc.dest.Y);
                        ArcSegment arcSegment = new()
                        {
                            IsLargeArc = pathArc.largeArcFlag,
                            IsStroked = true,
                            Size = new Size(pathArc.radiusX, pathArc.radiusY),
                            Point = aDest,
                            RotationAngle = pathArc.xAxisRotation,
                            SweepDirection = pathArc.sweepDirection
                        };
                        segments.Add(arcSegment);
                        break;
                    }
                case PathCubicBezierTo cubicBezier:
                    {
                        var aDest = new Avalonia.Point(cubicBezier.dest.X, cubicBezier.dest.Y);
                        var aControlPoint1 = new Avalonia.Point(
                            cubicBezier.controlPoint1.X, cubicBezier.controlPoint1.Y);
                        var aControlPoint2 = new Avalonia.Point(
                            cubicBezier.controlPoint2.X, cubicBezier.controlPoint2.Y);
                        BezierSegment bezierSegment = new()
                        {
                            IsStroked = true, Point1 = aControlPoint1, Point2 = aControlPoint2, Point3 = aDest
                        };
                        segments.Add(bezierSegment);
                        break;
                    }
            }
        }
        
        pathFigure.Segments = segments;
        pathFigures.Add(pathFigure);
        return new PathGeometry { Figures = pathFigures };
    }
}