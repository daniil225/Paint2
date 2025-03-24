using System.IO;
using Paint2.ViewModels.Interfaces;
using System;

namespace Formats.Json
{
    public class JsonExporter : IExportFormat
    {
        public void SaveTo(IExportSnapshot snapshot, string destinationPath)
        {
            var jsonSnapshot = snapshot as JsonSnapshot ?? throw new InvalidOperationException("Snapshot is not of type JsonSnapshot.");

            var jsonOutput = jsonSnapshot.Serialize();
            File.WriteAllText(destinationPath, jsonOutput);
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