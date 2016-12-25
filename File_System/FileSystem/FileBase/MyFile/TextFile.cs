using File_System.FileSystem.FileBase.MyFileList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.FileBase.MyFile {
    [Serializable]
    public class TextFile : File {

        public TextFile(string name = "") {
            this.name = name;
            this.content = "";
        }

        public TextFile(string name = "", string content = "") {
            this.name = name;
            this.content = content;
        }

        public override string GetContent() {
            return content;
        }

        public override FileType GetFileType() {
            return FileType.Text;
        }

        public override IFileList GetFileList() {
            throw new NotImplementedException();
        }

        public override void Add(IFile iFile) {
            throw new NotImplementedException();
        }

        public override string GetFileSize() {
            int result = 0;
            result = (int)Math.Ceiling((float)(content.Length * 0.001));
            return result.ToString() + " kb";
        }

        public override string ShowFileType() {
            return "文本文档";
        }

        public override void SetContent(string content) {
            if(content != this.content) {
                this.content = content;
                timeOfLastAlter = DateTime.Now.ToString();
            }
        }

        //public override void Open() {
        //    throw new NotImplementedException();
        //}

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
