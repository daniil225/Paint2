using Avalonia.Media;
using Formats;
using Paint2.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public Brush? Brush { get; set; }

    public PDFSnapshot(float width, float height)
    {
        _width = width;
        _height = height;

        _tree = new PDFGroup();
        _currentGroup = _tree;
    }

    public void AppendCircle(DocCircle circle)
    {
        PDFCircle pDFCircle = new PDFCircle(circle, Brush);
        _currentGroup.Children.Add(pDFCircle);
    }

    public void AppendLine(DocLine line)
    {
        PDFLine pDFLine = new PDFLine(line, Brush);
        _currentGroup.Children.Add(pDFLine);
    }

    public void AppendPath(DocPath path, Point _)
    {
        PDFPath pDPath = new PDFPath(path, Brush);
        _currentGroup.Children.Add(pDPath);
    }

    public void AppendPolygon(DocPolygon polygon)
    {
        PDFPolygon pDPolygon = new PDFPolygon(polygon, Brush);
        _currentGroup.Children.Add(pDPolygon);
    }

    public void AppendPolyline(DocPolyline polyline)
    {
        PDFPolyline pDPolyline = new PDFPolyline(polyline, Brush);
        _currentGroup.Children.Add(pDPolyline);
    }

    public void AppendRect(DocRect rect)
    {
        PDFRect pDRect = new PDFRect(rect, Brush);
        _currentGroup.Children.Add(pDRect);
    }

    public void Pop()
    {
        if(_currentGroup.Parent != null) 
            _currentGroup = _currentGroup.Parent;
    }

    public void PushGroup(DocGroup group)
    {
        PDFGroup pDFGroup = new PDFGroup(_currentGroup,group, Brush);

        _currentGroup.Children.Add(pDFGroup);
        _currentGroup = pDFGroup;
    }

}

public abstract class PDFElement
{
    public string? Name { get; set; }
    public IEnumerable<ITransform>? Transforms { get; set; }
    public Formats.Brush? Brush { get; set; }

    private void SetBrush(Brush brush)
    {
        Brush = new(brush);
        if (brush == null)
        {
            Brush = new(new(0, 0, 0, 0), new(0, 0, 0, 0), 1, new List<double> {1,0});
        }
        else if (Brush.Fill == null)
            Brush.Fill = new(0, 0, 0, 0);
        else if (Brush.Stroke == null)
            Brush.Stroke = new(0, 0, 0, 0);
        else if (Brush.Dash == null)
            Brush.Dash = new List<double> { 1, 0 };
    }

    public PDFElement(Brush? brush, string? name, IEnumerable<ITransform>? transforms) {
        SetBrush(brush);
        Name = name;
        Transforms = transforms;
    }
}

public class PDFGroup : PDFElement
{
    public List<PDFElement> Children { get; set; } = new();
    public PDFGroup? Parent { get; set; }

    public PDFGroup(PDFGroup? parent, DocGroup group, Brush brush) : base(brush, group.Name,group.Transforms)
    {
        Parent = parent;
        Children = new();
    }

    public PDFGroup() : base(null, "Root", null) { }
}

public class PDFRect : PDFElement
{
    public Point Position { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public PDFRect(DocRect rect, Brush brush) : base(brush,rect.Name,rect.Transforms)
    {
        Position = rect.Position;
        Width = rect.Width;
        Height = rect.Height;
    }
}

public class PDFCircle : PDFElement
{
    public Point Center { get; set; }

    public double Radius { get; set; }

    public PDFCircle(DocCircle circle, Brush brush) : base(brush, circle.Name, circle.Transforms)
    {
        Center = circle.Center;
        Radius = circle.Radius;
    }
}

public class PDFLine : PDFElement
{
    public Point Start { get; set; }
    public Point End { get; set; }

    public PDFLine(DocLine line, Brush brush) : base(brush, line.Name, line.Transforms)
    {
        Start = line.Start;
        End = line.End;
    }

}

public class PDFPolyline : PDFElement
{
    public IEnumerable<Point> Points { get; set; }
    public PDFPolyline(DocPolyline polyline, Brush brush) : base(brush, polyline.Name, polyline.Transforms)
    {
        Points = polyline.Points;
    }
}

public class PDFPolygon : PDFElement
{
    public IEnumerable<Point> Points { get; set; }

    public PDFPolygon(DocPolygon polygon, Brush brush) :base(brush, polygon.Name, polygon.Transforms)
    {
        Points = polygon.Points;
    }
}

public class PDFPath : PDFElement
{
    public IEnumerable<IPathElement> Elements { get; set; }

    public PDFPath(DocPath path, Brush brush) : base(brush, path.Name, path.Transforms)
    {
        Elements = path.Elements;
    }
}
