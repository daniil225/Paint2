using Avalonia.Media;
using Formats;
using Paint2.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Paint2.ViewModels.Utils;
using Paint2.Models.Figures;
using Avalonia.Controls.Shapes;

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
        PDFCircle pDFCircle = new PDFCircle(circle);
        _currentGroup.Children.Add(pDFCircle);
    }

    public void AppendLine(DocLine line)
    {
        PDFLine pDFLine = new PDFLine(line);
        _currentGroup.Children.Add(pDFLine);
    }

    public void AppendPath(DocPath path)
    {
        PDFPath pDPath = new PDFPath(path);
        _currentGroup.Children.Add(pDPath);
    }

    public void AppendPolygon(DocPolygon polygon)
    {
        PDFPolygon pDPolygon = new PDFPolygon(polygon);
        _currentGroup.Children.Add(pDPolygon);
    }

    public void AppendPolyline(DocPolyline polyline)
    {
        PDFPolyline pDPolyline = new PDFPolyline(polyline);
        _currentGroup.Children.Add(pDPolyline);
    }

    public void AppendRect(DocRect rect)
    {
        PDFRect pDRect = new PDFRect(rect);
        _currentGroup.Children.Add(pDRect);
    }

    public void Pop()
    {
        if(_currentGroup.Parent != null) 
            _currentGroup = _currentGroup.Parent;
    }

    public void PushGroup(DocGroup group)
    {
        PDFGroup pDFGroup = new PDFGroup(_currentGroup,group);

        _currentGroup.Children.Add(pDFGroup);
        _currentGroup = pDFGroup;
    }
}

public abstract class PDFElement
{
    public string? Name { get; set; }
    public IEnumerable<ITransform>? Transforms { get; set; }
    public Formats.Brush? Brush { get; set; }
}

public class PDFGroup : PDFElement
{
    public List<PDFElement> Children { get; set; } = new();
    public PDFGroup? Parent { get; set; }

    public PDFGroup(PDFGroup? parent, DocGroup group)
    {
        Parent = parent;
        Name = group.Name;
        Children = new();
        Transforms = group.Transforms;
        this.Brush = Brush;
    }

    public PDFGroup() { }
}

public class PDFRect : PDFElement
{
    //public double PositionX { get; set; }
    public Point Position { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public PDFRect(DocRect rect)
    {
        Position = rect.Position;
        //PositionY = rect.Position.Y;
        Width = rect.Width;
        Height = rect.Height;
        Name = rect.Name;
        Transforms = rect.Transforms;
        this.Brush = Brush;
    }
}

public class PDFCircle : PDFElement
{
    public Point Center { get; set; }
    //public double CenterY { get; set; }
    public double Radius { get; set; }

    public PDFCircle(DocCircle circle)
    {
        //Center = circle.Center;
        Center = circle.Center;
        Radius = circle.Radius;
        Name = circle.Name;
        Transforms = circle.Transforms;
        this.Brush = Brush;
    }
}

public class PDFLine : PDFElement
{
    public Point Start { get; set; }
    public Point End { get; set; }

    public PDFLine(DocLine line)
    {
        Start = line.Start;
        End = line.End;
        Name = line.Name;
        Transforms = line.Transforms;
        this.Brush = Brush;
    }

}

public class PDFPolyline : PDFElement
{
    public IEnumerable<Point> Points { get; set; }
    public PDFPolyline(DocPolyline polyline)
    {
        Points = polyline.Points;
        Name = polyline.Name;
        Transforms = polyline.Transforms;
        this.Brush = Brush;
    }
}

public class PDFPolygon : PDFElement
{
    public IEnumerable<Point> Points { get; set; }

    public PDFPolygon(DocPolygon polygon)
    {
        Points = polygon.Points;
        Name = polygon.Name;
        Transforms = polygon.Transforms;
        this.Brush = Brush;
    }
}

public class PDFPath : PDFElement
{
    public IEnumerable<IPathElement> Elements { get; set; }

    public PDFPath(DocPath path)
    {
        Elements = path.Elements;
        Name = path.Name;
        Transforms = path.Transforms;
        this.Brush = Brush;
    }
}
