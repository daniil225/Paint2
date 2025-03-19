using Avalonia.Media;
using Formats;
using Paint2.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Paint2.ViewModels.Utils;
using Paint2.Models.Figures;
using Avalonia.Controls.Shapes;
//using iText.Kernel.Geom;

namespace Formats.PDF;

public class PDFSnapshot : IExportSnapshot
{
    public float _width { get; }
    public float _height { get; }
    public PDFGroup _tree { get; }
    PDFGroup _currentGroup;

    public Formats.Brush? Brush { get; set; }

    public PDFSnapshot(float width, float height)
    {
        _width = width;
        _height = height;

        _tree = new PDFGroup();
        _currentGroup = _tree;
    }

    public void AppendCircle(DocCircle circle)
    {
        PDFCircle pDFCircle = new PDFCircle(circle, _height);
        _currentGroup.Children.Add(pDFCircle);
    }

    public void AppendLine(DocLine line)
    {
        PDFLine pDFLine = new PDFLine(line, _height);
        _currentGroup.Children.Add(pDFLine);
    }

    public void AppendPath(DocPath path)
    {
        PDFPath pDPath = new PDFPath(path, _height);
        _currentGroup.Children.Add(pDPath);
    }

    public void AppendPolygon(DocPolygon polygon)
    {
        PDFPolygon pDPolygon = new PDFPolygon(polygon, _height);
        _currentGroup.Children.Add(pDPolygon);
    }

    public void AppendPolyline(DocPolyline polyline)
    {
        PDFPolyline pDPolyline = new PDFPolyline(polyline, _height);
        _currentGroup.Children.Add(pDPolyline);
    }

    public void AppendRect(DocRect rect)
    {
        PDFRect pDRect = new PDFRect(rect, _height);
        _currentGroup.Children.Add(pDRect);
    }

    public void Pop()
    {
        if(_currentGroup.Parent != null) 
            _currentGroup = _currentGroup.Parent;
    }

    public void PushGroup(DocGroup group)
    {
        PDFGroup pDFGroup = new PDFGroup(_currentGroup,group, _height);

        _currentGroup.Children.Add(pDFGroup);
        _currentGroup = pDFGroup;
    }
};

public static class PDFDataChange
{
    public static void TransformPoint(Point point, double height)
    {
        point.Y = height - point.Y;
    }

    public static void TransformPath(IEnumerable<IPathElement> pathElement, double height)
    {
        foreach(IPathElement element in pathElement) 
        {
            if(element is not PathClose)
                TransformPathElement((dynamic)element, height); 
        }
    }

    private static void TransformPathElement(PathMoveTo path, double height)
    {
        TransformPoint(path.dest, height);
    }

    private static void TransformPathElement(PathLineTo line, double height) {TransformPoint(line.dest, height); }

    private static void TransformPathElement(PathArcTo arc, double height) {  TransformPoint(arc.dest, height); }

    private static void TransformPathElement(PathCubicBezierTo curve, double height)
    { 
        TransformPoint(curve.controlPoint1, height);
        TransformPoint(curve.controlPoint2, height);
        TransformPoint(curve.dest, height);
    }

    public static void TransformTransformations(IEnumerable<ITransform> transforms, double height)
    {
        if (transforms == null)
            return;
        foreach (ITransform transform in transforms)
        {
            switch (transform)
            {
                case (Translate translate):
 //               {
                    if(translate.Y > 0)
                        translate.Y = height -translate.Y;
                    else 
                        translate.Y = -translate.Y - height;
                    break;
//                }
                case (Rotate rotate):
//                {
                    if(rotate.Pivot != null)
                        TransformPoint(rotate.Pivot,height);
                    break;
 //               }
            }
        }
    }
}

public abstract class PDFElement
{
    public string? Name { get; set; }
    public IEnumerable<ITransform>? Transforms { get; set; }
    public Formats.Brush? Brush { get; set; }

    public PDFElement(IEnumerable<ITransform> Transforms_, double height)
    {
        Transforms = Transforms_;
        PDFDataChange.TransformTransformations(Transforms, height); 
    }
    
}

public class PDFGroup : PDFElement
{
    public List<PDFElement> Children { get; set; } = new();
    public PDFGroup? Parent { get; set; }

    public PDFGroup(PDFGroup? parent, DocGroup group, double height) : base(group.Transforms, height)
    {
        Parent = parent;
        Name = group.Name;
        Children = new();
        //Transforms = group.Transforms;
        this.Brush = Brush;
    }

    public PDFGroup() : base(null,0) { }
}

public class PDFRect : PDFElement
{
    //public double PositionX { get; set; }
    public Point Position { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public PDFRect(DocRect rect, double height) : base(rect.Transforms, height)
    {
        Position = rect.Position;
        PDFDataChange.TransformPoint(Position,height);
        //PositionY = rect.Position.Y;
        Width = rect.Width;
        Height = rect.Height;
        Name = rect.Name;
        //Transforms = rect.Transforms;
        this.Brush = Brush;
    }
}

public class PDFCircle : PDFElement
{
    public Point Center { get; set; }
    //public double CenterY { get; set; }
    public double Radius { get; set; }

    public PDFCircle(DocCircle circle, double height) :base(circle.Transforms, height)
    {
        //Center = circle.Center;
        Center = circle.Center;
        PDFDataChange.TransformPoint(Center, height);
        Radius = circle.Radius;
        Name = circle.Name;
        //Transforms = circle.Transforms;
        this.Brush = Brush;
    }
}

public class PDFLine : PDFElement
{
    public Point Start { get; set; }
    public Point End { get; set; }

    public PDFLine(DocLine line, double height) : base(line.Transforms,height)
    {
        Start = line.Start;
        PDFDataChange.TransformPoint(Start, height);
        End = line.End;
        PDFDataChange.TransformPoint(End, height);
        Name = line.Name;
        //Transforms = line.Transforms;
        this.Brush = Brush;
    }

}

public class PDFPolyline : PDFElement
{
    public IEnumerable<Point> Points { get; set; }
    public PDFPolyline(DocPolyline polyline, double height) : base (polyline.Transforms, height)
    {
        Points = polyline.Points;

        foreach (Point point in Points)
            PDFDataChange.TransformPoint(point, height);
        Name = polyline.Name;
        //Transforms = polyline.Transforms;
        this.Brush = Brush;
    }
}

public class PDFPolygon : PDFElement
{
    public IEnumerable<Point> Points { get; set; }

    public PDFPolygon(DocPolygon polygon,double height) : base(polygon.Transforms, height)
    {
        Points = polygon.Points;

        foreach (Point point in Points)
            PDFDataChange.TransformPoint(point, height);

        Name = polygon.Name;
        //Transforms = polygon.Transforms;
        this.Brush = Brush;
    }
}

public class PDFPath : PDFElement
{
    public IEnumerable<IPathElement> Elements { get; set; }

    public PDFPath(DocPath path, double height) : base(path.Transforms, height) 
    {
        Elements = path.Elements;
        PDFDataChange.TransformPath(Elements, height);
        Name = path.Name;
        //Transforms = path.Transforms;
        this.Brush = Brush;
    }
}