using Formats.Json;
using Formats.Svg;
using Paint2.ViewModels.Interfaces;
using Paint2.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
