using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.MyIterator {
    public class PathIterator : Iterator {
        private List<string> pathList;

        private int index = -1;

        public PathIterator(List<string> pathList) {
            this.pathList = pathList;
        }

        public bool HasNext() {
            return this.pathList == null ? false : (index < this.pathList.Count - 1);
        }

        public object Next() {
            if (this.pathList != null && index < this.pathList.Count - 1) {
                return this.pathList[++index];
            }
            return null;
        }

        public object Remove() {
            if (this.pathList != null) {
                string path = this.pathList[index];
                this.pathList.RemoveAt(index);
                return path;
            }
            return null;
        }
    }
}
