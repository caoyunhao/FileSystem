using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.FileBase.MyFile {
    public enum FileType {
        Folder,
        Text
    }

    public class MyType {
        static public string TypeToString(FileType fileType) {
            switch (fileType) {
                case FileType.Folder:
                    return "文件夹";
                case FileType.Text:
                    return "文本文档";
            }
            return null;
        }

        static public string SuffixOfType(FileType fileType) {
            switch (fileType) {
                case FileType.Folder:
                    return null;
                case FileType.Text:
                    return ".txt";
            }
            return null;
        }
    }
}
