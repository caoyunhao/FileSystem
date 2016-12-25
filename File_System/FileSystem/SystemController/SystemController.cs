using File_System.FileSystem.FileBase.MyFile;
using File_System.FileSystem.FileBase.MyFileList;
using File_System.FileSystem.FileBase.MyPathList;
using File_System.FileSystem.MyForm;
using File_System.FileSystem.MyIterator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_System.FileSystem.SystemController {
    public class SystemController : ISystemController {
        private static SystemController systemController = new SystemController();

        private IFile ROOT;
        private IMainForm mainForm;
        private ITextForm textForm;

        /// <summary>
        /// 全部文件List
        /// </summary>
        private const string FILE_NAME = "MyFile.bin";
        /// <summary>
        /// 被选择的树节点
        /// </summary>
        private TreeNode SELECTED_NODE;
        /// <summary>
        /// 被展开的节点List
        /// </summary>
        private List<string> ALL_EXPANDED_LIST = new List<string>();
        /// <summary>
        /// 后退路径集合
        /// </summary>
        private Stack<string> HISTORY_PATH = new Stack<string>();
        /// <summary>
        /// 前进路径集合
        /// </summary>
        private Stack<string> FUTURE_PATH = new Stack<string>();
        /// <summary>
        /// 粘贴板
        /// </summary>
        private IFileList CLIPBOARD = new FileList();


        private SystemController() {

        }

        public static SystemController GetInstance() {
            return systemController;
        }


        //public IFileList GetAllFileList() {
        //    return GetAllFileList();
        //}

        public void SetSelectedNode(TreeNode tree_node) {
            if (tree_node == null) {
                return;
            }
            SELECTED_NODE = tree_node;
        }

        public TreeNode GetSelectedNode() {
            if(SELECTED_NODE == null) {
                return null;
            }
            TreeNode selectedNode = mainForm.GetTreeView().Nodes.Find(SELECTED_NODE.Name, true).First();
            return selectedNode;
        }

        public Stack<string> GetHistoryPath() {
            return HISTORY_PATH;
        }
        public Stack<string> GetFuturePath() {
            return FUTURE_PATH;
        }

        public IFileList GetClipBoard() {
            return CLIPBOARD;
        }

        public void SetClipBoard(IFileList fileList) {
            CLIPBOARD = fileList;
        }


        public void Init() {
            ReadBinFile();
            if (GetAllFileList().GetSize() == 0) {
                IFile root = new FolderFile("我的电脑");
                ROOT = root;
            }
            FillTreeView();
            SELECTED_NODE = mainForm.GetTreeView().Nodes.Find(ROOT.GetIndex(), false)[0];

        }

        public void ReadBinFile() {
            //GetAllFileList() = new FileList();
            //BinaryFormatter bf = new BinaryFormatter();
            //Stream st = new FileStream(FILE_NAME, FileMode.Open);
            //do {
            //    if (st.Position == st.Length)
            //        break;
            //    IFile my_file = bf.Deserialize(st) as FileBase.MyFile.File;
            //    GetAllFileList().Add(my_file);
            //    continue;
            //} while (true);
            //st.Close();
            //return;
            //using (MemoryStream ms = new MemoryStream()) {
            //    BinaryFormatter bf = new BinaryFormatter();
            //    bf.Serialize(ms, this);
            //    ms.Seek(0, SeekOrigin.Begin);
            //    retval = bf.Deserialize(ms) as IFile;
            //    ms.Close();
            //}

            using (Stream st = new FileStream(FILE_NAME, FileMode.Open)) {
                BinaryFormatter bf = new BinaryFormatter();
                do {
                    if (st.Position == st.Length)
                        break;
                    ROOT = bf.Deserialize(st) as FolderFile;
                    continue;
                } while (true);
                st.Close();
            }
            return;
        }

        public void WriteBinFile() {
            using(Stream st = new FileStream(FILE_NAME, FileMode.Create, FileAccess.Write, FileShare.None)) {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(st, ROOT);
                //Iterator iterator = GetAllFileList().ToIterator();
                //while (iterator.HasNext()) {
                //    IFile a_file = (IFile)iterator.Next();
                //    bf.Serialize(st, a_file);
                //}
                st.Close();
            }
        }

        public void CreateTree() {
            Console.WriteLine("Creat Tree success");
        }

        public void Run(IMainForm form) {
            Application.Run(form.ToForm());
        }

        public void SetMainForm(IMainForm form) {
            mainForm = form;
        }

        public void SetTextForm(ITextForm form) {
            textForm = form;
        }

        public TreeNode FileToTreeNode(IFile my_file) {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = my_file.GetName();
            treeNode.Name = my_file.GetIndex();
            if (my_file.GetFileType() == FileType.Folder) {
                if (my_file.GetFileList().GetSize() > 0) {
                    Iterator iterator = my_file.GetFileList().ToIterator();
                    while (iterator.HasNext()) {
                        IFile a_file = (IFile)iterator.Next();
                        treeNode.Nodes.Add(FileToTreeNode(a_file));
                    }
                }
            }
            return treeNode;
        }

        public ListViewItem FileToListViewItem(IFile my_file) {
            string strname = my_file.ShowFileType();
            ListViewItem result = new ListViewItem(new string[] { my_file.GetName(), my_file.GetFileSize(), my_file.ShowFileType(), my_file.GetTimeOfLastAlter() });
            result.Name = my_file.GetIndex();
            result.Tag = my_file.GetFileType();
            return result;
        }

        public IFile GetFileByTreeNode(TreeNode tree_node) {
            Iterator iterator = GetAllFileList().ToIterator();
            while (iterator.HasNext()) {
                IFile a_file = (IFile)iterator.Next();
                if (a_file.GetIndex() == tree_node.Name && a_file.GetFileType() == FileType.Folder) {
                    return a_file;
                }
            }
            return null;
        }

        public IFile GetFileByListViewItem(ListViewItem list_view_item) {
            Iterator iterator = GetAllFileList().ToIterator();
            while (iterator.HasNext()) {
                IFile a_file = (IFile)iterator.Next();
                if (a_file.GetIndex() == list_view_item.Name) {
                    return a_file;
                }
            }
            return null;
        }

        public string GetNewName(TreeNode tree_node, FileType file_type) {
            string prefix = "新建";
            string typeName = MyType.TypeToString(file_type);
            string suffix = MyType.SuffixOfType(file_type);//包含"."
            bool hasNew = false;

            foreach (TreeNode a_node in tree_node.Nodes) {
                if (a_node.Text == prefix + typeName + suffix) {
                    hasNew = true;
                }
            }

            if (hasNew) {
                for (int i = 1; ; ++i) {
                    bool has = false;
                    string newName = prefix + typeName + "（" + i + "）" + suffix;
                    foreach (TreeNode a_node in tree_node.Nodes) {
                        if (a_node.Text == newName) {
                            has = true;
                        }
                    }
                    if (!has) {
                        return newName;
                    }
                }
            }
            else {
                return prefix + typeName + suffix;
            }
        }

        public IFile FindFileByIndex(string index) {
            Iterator iterator = GetAllFileList().ToIterator();
            while (iterator.HasNext()) {
                IFile a_file = (IFile)iterator.Next();
                if (a_file.GetIndex() == index) {
                    return a_file;
                }
            }
            MessageBox.Show("未成功通过索引获取文件，系统错误！" + index);
            return null;
        }

        public void FindNodeByStringList(IPathList path_list) {
            TreeNode root = mainForm.GetTreeView().Nodes.Find(ROOT.GetIndex(), false).First();
            Iterator iterator = path_list.ToIterator();
            if (root.Text != (string)iterator.Next()) {
                MessageBox.Show("错误的路径！");
                return;
            }
            if (path_list.GetSize() == 1) {
                mainForm.GetTreeView().SelectedNode = root;
            }
            TreeNode temp = root;

            iterator = path_list.ToIterator();
            while (iterator.HasNext()) {
                bool has = true;
                foreach (TreeNode a_node in temp.Nodes) {
                    string str = (string)iterator.Next();
                    if (!iterator.HasNext() && a_node.Text == str) {
                        mainForm.GetTreeView().SelectedNode = a_node;
                        return;
                    }
                    if (a_node.Text == str) {
                        has = true;
                        temp.Expand();
                        temp = a_node;
                    }
                    if (!has) {
                        MessageBox.Show("错误的路径！");
                    }
                }
            }
        }

        public IFile FindParentFile(IFile child_file) {
            Iterator iterator = GetAllFileList().ToIterator();
            while (iterator.HasNext()) {
                var parent_file = (IFile)iterator.Next();
                if (parent_file.GetFileType() == FileType.Folder) {
                    Iterator iterator1 = parent_file.GetFileList().ToIterator();
                    while (iterator1.HasNext()) {
                        var a_child_file = (IFile)iterator1.Next();
                        if (a_child_file.GetIndex() == child_file.GetIndex()) {
                            return parent_file;
                        }
                    }
                }
            }
            MessageBox.Show("未找到该文件的父文件，系统错误！" + child_file.GetIndex());
            return null;
        }

        public bool CheckName(string new_name, IFile parent_file) {
            Iterator iterator1 = parent_file.GetFileList().ToIterator();
            while (iterator1.HasNext()) {
                var a_file1 = (IFile)iterator1.Next();
                if (a_file1.GetName() == new_name) {
                    MessageBox.Show("重复命名！");
                    return false;
                }
            }
            return true;
        }

        public void FillTreeView() {
            mainForm.GetTreeView().Nodes.Clear();
            Iterator iterator = GetAllFileList().ToIterator();
            while (iterator.HasNext()) {
                var a_file = (IFile)iterator.Next();
                if (a_file.GetIndex() == ROOT.GetIndex()) {
                    mainForm.GetTreeView().Nodes.Add(FileToTreeNode(a_file));
                }
            }
        }

        public void FillListView() {
            mainForm.GetListView().Items.Clear();
            IFile selectedFile = GetFileByTreeNode(GetSelectedNode());
            if (selectedFile != null) {
                if (selectedFile.GetFileType() == FileType.Folder) {
                    Iterator iterator = selectedFile.GetFileList().ToIterator();
                    while (iterator.HasNext()) {
                        var a_file = (IFile)iterator.Next();
                        ListViewItem a_item = FileToListViewItem(a_file);
                        mainForm.GetListView().Items.Add(a_item);
                    }
                }
            }
        }

        public void Expand() {
            TreeNode root = mainForm.GetTreeView().Nodes.Find(ROOT.GetIndex(), false).First();
            ExpandedSupport(root);
        }

        public void ExpandedSupport(TreeNode root) {
            if (root.Nodes.Count > 0) {
                foreach (string a_index in ALL_EXPANDED_LIST) {
                    if (root.Name == a_index) {
                        root.Expand();
                    }
                }
                foreach (TreeNode a_node in root.Nodes) {
                    ExpandedSupport(a_node);
                }
            }
        }

        public void Sign() {
            TreeNode root = mainForm.GetTreeView().Nodes.Find(ROOT.GetIndex(), false).First();
            SignSupport(root);
        }

        public void SignSupport(TreeNode root) {
            if (root.Nodes.Count > 0) {
                if (root.IsExpanded) {
                    string index = root.Name;
                    ALL_EXPANDED_LIST.Add(index);
                }
                foreach (TreeNode a_node in root.Nodes) {
                    SignSupport(a_node);
                }
            }
        }

        public void RefreshForm() {
            Sign();
            FillTreeView();
            Expand();
            FillListView();
        }

        public void ShowPath() {
            mainForm.GetPathTextBox().Text = SELECTED_NODE.FullPath;
        }

        public void PathToSelect(string path) {
            FindNodeByStringList(PathToList(path));
        }

        public IPathList PathToList(string path) {
            IPathList result = new PathList();
            string temp = path;
            do {
                int a_index = temp.IndexOf("\\");
                if (a_index == -1) {
                    result.Add(temp);
                    break;
                }
                string str = temp.Substring(0, a_index);
                temp = temp.Substring(str.Length + 1, temp.Length - str.Length - 1);
                result.Add(str);
            } while (true);
            return result;
        }

        public void OpenFile(IFile file) {
            FileType fileType = file.GetFileType();
            switch (fileType) {
                case FileType.Folder:
                    OpenFolder(file); break;
                case FileType.Text:
                    OpenTextFile(file);break;
                default:
                    break;
            }
        }

        public void OpenTextFile(IFile my_file) {
            if (my_file.GetFileType() == FileType.Text) {
                ITextForm new_TextForm = new Form2();
                new_TextForm.SetTextFile(my_file);
                new_TextForm.ShowTextForm();
            }
            else {
                MessageBox.Show("打开错误！");
            }
        }

        public void OpenFolder(IFile my_file) {
            if (my_file.GetFileType() == FileType.Folder) {
                mainForm.GetTreeView().SelectedNode = mainForm.GetTreeView().Nodes.Find(my_file.GetIndex(), true)[0];
            }
            else {
                MessageBox.Show("打开错误！");
            }
        }

        public void DeleteFile(IFile my_file) {
            IFile temp = FindParentFile(my_file);
            temp.GetFileList().Remove(my_file);
            RefreshForm();
        }

        public void Exit() {
            Application.Exit();
        }

        public IMainForm GetMainForm() {
            return mainForm;
        }

        public IFileList GetAllFileList() {
            IFileList All_FILE_LIST = new FileList();
            AddFile(ROOT, All_FILE_LIST);
            return All_FILE_LIST;
        }

        private void AddFile(IFile file, IFileList fileList) {
            if (file == null || fileList == null) {
                return;
            }   
            fileList.Add(file);
            if(file.GetFileType() == FileType.Folder) {
                Iterator iterator = file.GetFileList().ToIterator();
                while (iterator.HasNext()) {
                    var a_file = (IFile)iterator.Next();
                    AddFile(a_file, fileList);
                }
            }
        }
    }
}
