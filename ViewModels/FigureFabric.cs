﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using Paint2.Models.Figures;
using Paint2.ViewModels.Interfaces;
using Paint2.ViewModels;
using Point = Paint2.ViewModels.Utils.Point;
using Serilog;


class FigureMetadata
{
    public string Name { get; init; }
}
public interface IFigureCreator
{
    IReadOnlyCollection<string> PointParametersNames { get; }
    IReadOnlyCollection<string> DoubleParametersNames { get; }
    IFigure Create(Group parentGroup);
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
            Log.Error($"Couldn't get available figures during initialization: {e}");
        }

        var cont = conf.CreateContainer();
        info = new ImportInfo();
        cont.SatisfyImports(info);
    }

    public static IEnumerable<string> AvailableFigures => info.AvailableFigures.Select(f => f.Metadata.Name);
    public static IFigure CreateFigure(string FigureName, Group parentGroup)
    {
        return info.AvailableFigures.First(f => f.Metadata.Name == FigureName).Value
            .Create(parentGroup);
    }
}
