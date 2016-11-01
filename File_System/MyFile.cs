using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System {

    [Serializable]
    public partial class MyFile {

        /// <summary>
        /// MyFile构造函数
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_isFolder"></param>
        /// <param name="_layer"></param>
        public MyFile(string _index = "-1", string _name = "", bool _isFolder = true) {
            index = _index;
            name = _name;
            isFolder = _isFolder;
            elementIndex = new List<string>();
        }

        /// <summary>
        /// 索引号
        /// </summary>
        public string index = "-1";

        /// <summary>
        /// 文件名
        /// </summary>
        public string name = "";

        /// <summary>
        /// 该文件是否为文件夹
        /// </summary>
        public bool isFolder = true;

        /// <summary>
        /// 文件内容
        /// </summary>
        public string contant = "";

        /// <summary>
        /// 文件创建时间
        /// </summary>
        public string timeOfCreate = DateTime.Now.ToString();

        /// <summary>
        /// 文件最后一次修改时间
        /// </summary>
        public string timeOfLastAlter = DateTime.Now.ToString();

        /// <summary>
        /// 成员索引集合
        /// </summary>
        public List<string> elementIndex = new List<string>();

        /// <summary>
        /// 获取成员集合
        /// </summary>
        /// <param name="file_list"></param>
        /// <returns></returns>
        public List<MyFile> GetElementCollection(List<MyFile> file_list) {
            List<MyFile> result = new List<MyFile>();
            foreach(string a_index in elementIndex) {
                foreach(MyFile a_file in file_list) {
                    if(a_file.index == a_index) {
                        result.Add(a_file);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <returns></returns>
        public string GetFileSize() {
            int result = 0;
            result = (int)Math.Ceiling((float)(contant.Length * 0.001));
            return isFolder ? "" : result.ToString() + " kb";
        }

        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <returns></returns>
        public string GetFileType() {
            return isFolder ? "文件夹" : "文本文档";
        }

        /// <summary>
        /// 对文件进行拷贝，保存到out_file
        /// </summary>
        /// <param name="out_file"></param>
        public void CopyTo(out MyFile out_file) {
            out_file = new MyFile();
            out_file.index = this.index;
            out_file.name = this.name;
            out_file.isFolder = this.isFolder;
            out_file.contant = this.contant;
            out_file.timeOfCreate = this.timeOfCreate;
            out_file.timeOfLastAlter = this.timeOfLastAlter;
        }
    }
}
