using File_System.FileSystem.MyIterator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.FileBase.MyPathList {
    public class PathList : IPathList {
        private List<string> pathList;

        public PathList() {
            pathList = new List<string>();
        }

        public List<string> GetFileList() {
            return pathList;
        }

        public void Add(string iFile) {
            pathList.Add(iFile);
        }


        public Iterator ToIterator() {
            return new PathIterator(this.pathList);
        }

        public int GetSize() {
            return pathList.Count;
        }
    }
}
