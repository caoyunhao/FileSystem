using File_System.FileSystem.FileBase.MyFile;
using File_System.FileSystem.MyIterator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.FileBase.MyFileList {
    public interface IFileList {

        List<IFile> GetFileList();

        void Add(IFile iFile);

        int GetSize();

        void Remove(IFile file);

        void Clear();

        Iterator ToIterator();
    }
}
