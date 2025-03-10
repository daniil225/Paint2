using System;
using System.Collections.Generic;

namespace Paint2.ViewModels
{
    public static class Scene
    {
        // Тут хранятся все группы сцены. Корневой группы явно нет, по сути сама сцена ей является
        public static IList<Group> Groups { get; set; }
        //public static IFileStrategy FileStrategy { get; set; }

        static Scene()
        {
            Groups = [];
            //FileStrategy = new SVGStrategy();
        }
        // Не помню какие именно тут параметры нужны были
        public static void SaveScene(string path)
        {
            throw new NotImplementedException();
        }
        public static void LoadScene(string path)
        {
            throw new NotImplementedException();
        }
        public static void ResetScene()
        {
            Groups.Clear();
        }
    }
}
