using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using Interfaces;
using Serilog;
using Point = Interfaces.Point;


class FigureMetadata
{
    public string Name { get; }
}
public interface IFigureCreator
{
    int NumberOfDoubleParameters { get; internal set; }
    int NumberOfPointParameters { get; internal set; }
    IEnumerable<string> PointParametersNames { get; internal set; }
    IEnumerable<string> DoubleParametersNames { get; internal set; }
    IFigure Create(IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams);
}

public static class FigureFabric
{
    class ImportInfo
    {
        [ImportMany]
        public IEnumerable<Lazy<IFigureCreator, FigureMetadata>> AvailableFigures { get; set; } = [];
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
        catch (Exception e)
        {
            Log.Error(e, "Couldn't get available figures.");
        }

        var cont = conf.CreateContainer();
        info = new ImportInfo();
        cont.SatisfyImports(info);
    }

    public static IEnumerable<string> AvailableFigures => info.AvailableFigures.Select(f => f.Metadata.Name);
    public static IFigure CreateFigure(string FigureName, IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams)
    {
        return info.AvailableFigures.First(f => f.Metadata.Name == FigureName).Value
            .Create(doubleParams,pointParams);
    }
}

public class Circle : IFigure
{
    [Export(typeof(IFigureCreator))]
    [ExportMetadata(nameof(FigureMetadata.Name), nameof(Circle))]
    class CircleCreator : IFigureCreator
    {
        public int NumberOfDoubleParameters { get => 1; set => NumberOfDoubleParameters = value; }

        public int NumberOfPointParameters { get => 1; set => NumberOfPointParameters = value; }

        public IEnumerable<string> PointParametersNames
        {
            get
            {
                yield return "Center";
            }
            set => PointParametersNames = value;
        }

        public IEnumerable<string> DoubleParametersNames
        {
            get
            {
                yield return "Radius";
            }
            set => DoubleParametersNames = value;
        }

        public IFigure Create(IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams)
        {
            return new Circle(pointParams["Center"], doubleParams["Radius"]);
        }

        IFigure IFigureCreator.Create(IDictionary<string, double> doubleParams, IDictionary<string, Point> pointParams)
        {
            throw new NotImplementedException();
        }
    }
    Point Center { get; set; }
    double Radius { get; set; }
    Circle(Point c,double r)
    {
        Center = c;
        Radius = r;
    }
    public double Width => throw new NotImplementedException();

    public double Height => throw new NotImplementedException();

    public string Name => throw new NotImplementedException();

    public void Render(IRenderInterface toDraw)
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
