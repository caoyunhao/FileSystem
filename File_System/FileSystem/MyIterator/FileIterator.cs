using File_System.FileSystem.FileBase.MyFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.MyIterator {
    public class FileIterator : Iterator {

        private List<IFile> fileList;

        private int index = -1;

        public FileIterator(List<IFile> fileList) {
            this.fileList = fileList;
        }

        public bool HasNext() {
            return this.fileList == null ? false : (index < this.fileList.Count - 1);
        }

        public object Next() {
            if (this.fileList != null && index < this.fileList.Count - 1) {
                return this.fileList[++index];
            }
            return null;
        }

        public object Remove() {
            if (this.fileList != null) {
                IFile file = this.fileList[index];
                this.fileList.RemoveAt(index);
                return file;
            }
            return null;
        }

    }
}
