using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using Paint2.Models.Figures;
using Interfaces;
using Point = Interfaces.Point;


class FigureMetadata
{
    public string Name { get; init; }
}
public interface IFigureCreator
{
    int NumberOfDoubleParameters { get; }
    int NumberOfPointParameters { get; }
    IEnumerable<string> PointParametersNames { get; }
    IEnumerable<string> DoubleParametersNames { get; }
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
        catch (Exception)
        {
            // ignored
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
