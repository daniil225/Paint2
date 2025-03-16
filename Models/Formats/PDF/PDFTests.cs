using Formats.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formats.PDF;

public static class PDFTests
{
    public static void RunAll()
    {
        var exporter = new PDFExporter();

        var snap = new PDFSnapshot(100, 100);
        Formats.Tests.SnapshotGeneral(snap);
        exporter.SaveTo(snap, "general.pdf");

        snap = new PDFSnapshot(200, 200);
        Formats.Tests.SnapshotOnlyPaths(snap);
        exporter.SaveTo(snap, "only_paths.pdf");

        snap = new PDFSnapshot(200, 200);
        Formats.Tests.SnapshotMirror(snap);
        exporter.SaveTo(snap, "mirror.pdf");
    }
}
