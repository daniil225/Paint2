using Paint2.ViewModels.Interfaces;

namespace Formats.Svg
{
    public class SvgExporter : IExportFormat
    {
        public void SaveTo(IExportSnapshot snapshot, string destinationPath)
        {
            // ожидается что SaveTo будет вызван с SvgSnapshot
            var svgSnap = snapshot as SvgSnapshot;
            svgSnap!.Tree.Save(destinationPath);
        }
    }

    public static class Tests
    {
        public static void RunAll()
        {
            var exporter = new SvgExporter();
            
            var snap = new SvgSnapshot(100, 100);
            Formats.Tests.SnapshotGeneral(snap);
            exporter.SaveTo(snap, "general.svg");
            
            snap = new SvgSnapshot(200, 200);
            Formats.Tests.SnapshotOnlyPaths(snap);
            exporter.SaveTo(snap, "only_paths.svg");
            
            snap = new SvgSnapshot(200, 200);
            Formats.Tests.SnapshotMirror(snap);
            exporter.SaveTo(snap, "mirror.svg");
        }
    }
}
