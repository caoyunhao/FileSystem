using File_System.FileSystem.FileBase.MyFileList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.FileBase.MyFile {
    public class FolderFile : File {

        public FolderFile(string name = "") {
            this.name = name;
            this.fileList = new FileList();
        }

        public override string GetContent() {
            throw new NotImplementedException();
        }

        public override FileType GetFileType() {
            return FileType.Folder;
        }

        public override IFileList GetFileList() {
            return fileList;
        }

        public override void Add(IFile iFile) {
            fileList.Add(iFile);
        }

        public override string GetFileSize() {
            return null;
        }

        public override string ShowFileType() {
            return "文件夹";
        }

        public override void SetContent(string content) {
            throw new NotImplementedException();
        }

        //public override IFile DeepCopy() {
        //    IFile retval;
        //    using (MemoryStream ms = new MemoryStream()) {
        //        BinaryFormatter bf = new BinaryFormatter();
        //        bf.Serialize(ms, this);
        //        ms.Seek(0, SeekOrigin.Begin);
        //        retval = bf.Deserialize(ms) as IFile;
        //        ms.Close();
        //    }
        //    return retval;
        //}
    }
}
