using System.IO;
using System.Linq;

namespace LocalStorageBookmarker
{
    public class FileItem
    {
        public string path { get; set; }
        public string name { get; set; }

        public FileItem(string fullPath)
        {
            this.path = fullPath;
            this.name = fullPath.Split('\\').Last();
        }

        public bool IsFileValid()
        {
            return File.Exists(this.path);
        }
    }
}
