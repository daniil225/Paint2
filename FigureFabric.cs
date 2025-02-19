using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Text.Json.Serialization;
using Interfaces;


class FigureMetadata
{
    public string Name { get; }
    public int NumberOfDoubleParameters { get; }
    public int NumberOfPointParameters { get; }
    public IEnumerable<string> PointParametersNames { get; }
    public IEnumerable<string> DoubleParametersNames { get; }
}

public static class FigureFabric
{
    class ImportInfo
    {
        [ImportMany]
        public IEnumerable<Lazy<IFigure, FigureMetadata>> AvailableFigures { get; set; } = [];
    }
    static ImportInfo info;
    static FigureFabric()
    {
        var assemblies = new[] { typeof(Circle).Assembly };
        var conf = new ContainerConfiguration();
        try
        {
            conf = conf.WithAssemblies(assemblies);
        }
        catch (Exception)
        {
            // ignored
        }

        var cont = conf.CreateContainer();
        info = new ImportInfo();
        cont.SatisfyImports(info);
    }

    public static IEnumerable<string> AvailableFigures => info.AvailableFigures.Select(f => f.Metadata.Name);
    public static IFigure CreateFigure(string FigureName)
    {
        return info.AvailableFigures.First(f => f.Metadata.Name == FigureName).Value;
    }
}

[Export(typeof(IFigure))]
[ExportMetadata(nameof(FigureMetadata.Name), nameof(Circle))]
[ExportMetadata(nameof(FigureMetadata.NumberOfPointParameters), 1)]
[ExportMetadata(nameof(FigureMetadata.NumberOfDoubleParameters), 1)]
[ExportMetadata(nameof(FigureMetadata.PointParametersNames), new string[] {"Center"})]
[ExportMetadata(nameof(FigureMetadata.DoubleParametersNames), new string[] { "Radius" })]
public class Circle : IFigure
{
    public void Draw(IGraphicInterface toDraw)
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

    public void Move(Point vector)
    {
        throw new NotImplementedException();
    }

    public void Reflection(Point ax1, Point ax2)
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
