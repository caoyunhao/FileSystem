using File_System.FileSystem.MyIterator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.FileBase.MyPathList {
    public interface IPathList {

        List<string> GetFileList();

        void Add(string iFile);

        int GetSize();

        Iterator ToIterator();
    }
}
