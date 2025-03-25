using Formats.Json;
using Paint2.ViewModels.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Paint2.ViewModels
{
    public static class HistoryManager
    {
        private static List<string> pathsToSnapshots = [];
        private static int pointer = -1;

        public static void MakeSceneSnapshot()
        {
            pathsToSnapshots.RemoveRange(pointer, pathsToSnapshots.Count - 1);

            string tempFileName = Path.GetTempFileName();
            pathsToSnapshots.Add(tempFileName);
            if (pathsToSnapshots.Count > 128 )
                pathsToSnapshots.RemoveAt(0);
            else
                pointer++;
            IExportSnapshot snapshot = new JsonSnapshot();
            Scene.Current.SaveScene(snapshot, tempFileName);
        }
        public static void Undo()
        {
            if (pointer <= 0)
            {
                pointer--;
                Scene.Current.LoadScene(pathsToSnapshots[pointer]);
            }
        }
        public static void Redo()
        {
            if (pointer < pathsToSnapshots.Count)
            {
                pointer++;
                Scene.Current.LoadScene(pathsToSnapshots[pointer]);
            }
        }
    }
}
