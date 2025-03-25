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
            if (pointer == -1)
                pathsToSnapshots.Clear();
            else if (pointer < pathsToSnapshots.Count - 1)
                pathsToSnapshots.RemoveRange(pointer, pathsToSnapshots.Count - 1);

            string tempFileName = Path.GetTempFileName();
            pathsToSnapshots.Add(tempFileName);
            if (pathsToSnapshots.Count > 128)
            {
                File.Delete(pathsToSnapshots[0]);
                pathsToSnapshots.RemoveAt(0);
            }
            else
                pointer++;
            IExportSnapshot snapshot = new JsonSnapshot();
            Scene.Current.SaveScene(snapshot, tempFileName);
        }
        public static void Undo()
        {
            if (pointer >= 0)
            {
                string path = pathsToSnapshots[pointer];
                //Scene.Current.LoadScene(path);
                pointer--;
            }
        }
        public static void Redo()
        {
            if (pointer < pathsToSnapshots.Count - 1)
            {
                pointer++;
                //Scene.Current.LoadScene(pathsToSnapshots[pointer]);
            }
        }
    }
}
