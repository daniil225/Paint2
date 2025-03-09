using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint2.ViewModels
{
    public interface IFileStrategy
    {
        void Save(string path);
        void Load(string path);
    }
    public class SVGStrategy : IFileStrategy
    {
        public void Load(string path)
        {
            throw new NotImplementedException();
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }
    }
    public class PDFStrategy : IFileStrategy
    {
        public void Load(string path)
        {
            throw new NotImplementedException();
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }
    }
    public class CustomStratrgy : IFileStrategy
    {
        public void Load(string path)
        {
            throw new NotImplementedException();
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }
    }
}
