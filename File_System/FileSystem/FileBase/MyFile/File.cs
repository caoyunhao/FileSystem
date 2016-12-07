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
    public abstract class File : IFile {


        protected string name;

        protected string index;

        protected string content;

        protected IFileList fileList;

        /// <summary>
        /// 文件创建时间
        /// </summary>
        public string timeOfCreate;

        /// <summary>
        /// 文件最后一次修改时间
        /// </summary>
        public string timeOfLastAlter;

        protected File() { }

        public string GetName() {
            return name;
        }

        public void SetName(string name) {
            this.name = name;
        }

        public string GetIndex() {
            return index;
        }

        public void SetIndex() {
            index = Guid.NewGuid().ToString();
        }

        public string GetTimeOfCreate() {
            return timeOfCreate;
        }

        public string GetTimeOfLastAlter() {
            return timeOfLastAlter;
        }

        public IFile DeepCopy() {
            IFile retval;
            using (MemoryStream ms = new MemoryStream()) {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Seek(0, SeekOrigin.Begin);
                retval = bf.Deserialize(ms) as IFile;
                ms.Close();
            }
            return retval as IFile;
        }


        public abstract string GetContent();

        public abstract void SetContent(string content);


        public abstract FileType GetFileType();

        //public abstract void GetContent();

        public abstract IFileList GetFileList();

        public abstract void Add(IFile iFile);

        public abstract string GetFileSize();

        public abstract string ShowFileType();
    }
}
