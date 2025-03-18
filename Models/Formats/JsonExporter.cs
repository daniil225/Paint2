using System;
using System.IO;

using Paint2.ViewModels.Interfaces;

namespace Formats.Json
{
    public class JsonExporter : IExportFormat
    {
        public void SaveTo(IExportSnapshot snapshot, string destinationPath)
        {
            var jsonSnap = snapshot as JsonSnapshot;
            if (jsonSnap == null)
            {
                throw new ArgumentException("Неверный снимок передан для JsonExporter.");
            }
            var jsonContent = jsonSnap.ToJson();
            File.WriteAllText(destinationPath, jsonContent);
        }
    }

    public static class Tests
    {
        public static void RunAll()
        {
            var exporter = new JsonExporter();
            
            var snap = new JsonSnapshot();
            Formats.Tests.SnapshotGeneral(snap);
            exporter.SaveTo(snap, "general.json");
            
            snap = new JsonSnapshot();
            Formats.Tests.SnapshotOnlyPaths(snap);
            exporter.SaveTo(snap, "only_paths.json");
            
            snap = new JsonSnapshot();
            Formats.Tests.SnapshotMirror(snap);
            exporter.SaveTo(snap, "mirror.json");
        }
    }
}
