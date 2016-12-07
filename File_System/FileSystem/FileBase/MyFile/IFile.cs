using File_System.FileSystem.FileBase.MyFileList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.FileBase.MyFile {
    public interface IFile {

        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <returns></returns>
        string GetName();

        void SetName(string name);

        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <returns></returns>
        FileType GetFileType();

        string ShowFileType();

        string GetIndex();

        void SetIndex();
        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <returns></returns>
        string GetContent();

        void SetContent(string content);

        IFileList GetFileList();

        void Add(IFile iFile);
        string GetFileSize();

        string GetTimeOfCreate();

        string GetTimeOfLastAlter();

        IFile DeepCopy();
    }
}
