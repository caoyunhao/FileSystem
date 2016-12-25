using File_System.FileSystem.FileBase.MyFile;
using File_System.FileSystem.MyIterator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.FileBase.MyFileList {
    [Serializable]
    public class FileList : IFileList {

        private List<IFile> fileList;

        public FileList() {
            fileList = new List<IFile>();
        }

        public FileList(List<IFile> fileList) {
            this.fileList = fileList;
        }

        public List<IFile> GetFileList() {
            return fileList;
        }

        public void Add(IFile iFile) {
            fileList.Add(iFile);
        }


        public Iterator ToIterator() {
            return new FileIterator(this.fileList);
        }

        public int GetSize() {
            return fileList.Count;
        }

        public void Remove(IFile file) {
            fileList.Remove(file);
        }

        public void Clear() {
            fileList.Clear();
        }
    }
}
