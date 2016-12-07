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

        private IFileList ALL_FILE_LIST;

        private IForm mainForm;
        private IForm textForm;



        /// <summary>
        /// 全部文件List
        /// </summary>
        private const string FILE_NAME = "MyFile.bin";
        /// <summary>
        /// 被选择的树节点
        /// </summary>
        private TreeNode SELECTED_NODE = new TreeNode();
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
        private IFileList CLIPBOARD;


        private SystemController() {

        }

        public static SystemController GetInstance() {
            return systemController;
        }


        public IFileList GetAllFileList() {
            return ALL_FILE_LIST;
        }

        public void SetSelectedNode(TreeNode tree_node) {
            SELECTED_NODE = tree_node;
        }

        public TreeNode GetSelectedNode() {
            return SELECTED_NODE;
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






        public void ReadBinFile() {
            ALL_FILE_LIST = new FileList();
            BinaryFormatter bf = new BinaryFormatter();
            Stream st = new FileStream(FILE_NAME, FileMode.Open);
            do {
                if (st.Position == st.Length)
                    break;
                IFile my_file = bf.Deserialize(st) as IFile;
                ALL_FILE_LIST.Add(my_file);
                continue;
            } while (true);
            st.Close();
            return;
        }

        public void WriteBinFile() {
            BinaryFormatter bf = new BinaryFormatter();
            Stream st = new FileStream(FILE_NAME, FileMode.Create, FileAccess.Write, FileShare.None);
            Iterator iterator = ALL_FILE_LIST.ToIterator();
            while (iterator.HasNext()) {
                IFile a_file = (IFile)iterator.Next();
                bf.Serialize(st, a_file);
            }
            st.Close();
        }

        public void CreateTree() {
            Console.WriteLine("Creat Tree success");
        }

        public void Run(IForm form) {
            Console.WriteLine("启动界面");
        }

        public void SetMainForm(IForm form) {
            mainForm = form;
        }

        public void SetTextForm(IForm form) {

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
            ListViewItem result = new ListViewItem(new string[] { my_file.GetName(), my_file.GetFileSize(), my_file.ShowFileType(), my_file.GetTimeOfLastAlter() });
            result.Tag = my_file.GetFileType();
            return result;
        }

        public IFile GetFileByTreeNode(TreeNode tree_node) {
            Iterator iterator = ALL_FILE_LIST.ToIterator();
            while (iterator.HasNext()) {
                IFile a_file = (IFile)iterator.Next();
                if (a_file.GetIndex() == tree_node.Name && a_file.GetFileType() == FileType.Folder) {
                    return a_file;
                }
            }
            return null;
        }

        public IFile GetFileByListViewItem(ListViewItem list_view_item) {
            Iterator iterator = ALL_FILE_LIST.ToIterator();
            while (iterator.HasNext()) {
                IFile a_file = (IFile)iterator.Next();
                if (a_file.GetIndex() == list_view_item.Name) {
                    return a_file;
                }
            }
            return null;
        }

        public string GetNewName(TreeNode tree_node, FileType fileType) {
            string result = "新建" + MyType.TypeToString(fileType);
            string suffix = MyType.SuffixOfType(fileType);
            for (int i = 1; ; ++i) {
                bool hadNew = false;
                bool had = false;
                foreach (TreeNode a_node in tree_node.Nodes) {
                    string a_node_text = a_node.Text;
                    if (a_node_text == (result + suffix)) {
                        hadNew = true;
                        a_node_text = result + "（0）";
                    }
                    int indexOfStart = a_node_text.LastIndexOf("（");
                    int indexOfEnd = a_node_text.LastIndexOf("）");
                    if (indexOfStart == -1 || indexOfEnd == -1) {
                        continue;
                    }
                    string a_num = a_node_text.Substring(indexOfStart + 1, indexOfEnd - indexOfStart - 1);
                    if (i.ToString().Trim() == a_num.Trim()) {
                        had = true;
                        break;
                    }
                }
                if (!hadNew) {
                    return result + suffix;
                }
                if (!had) {
                    result += "（" + i.ToString().Trim() + "）" + suffix;
                    return result;
                }
            }
        }

        public IFile FindFileByIndex(string index) {
            Iterator iterator = ALL_FILE_LIST.ToIterator();
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
            TreeNode root = mainForm.GetTreeView().Nodes.Find("root", false).First();
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
            Iterator iterator = ALL_FILE_LIST.ToIterator();
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
            Iterator iterator = ALL_FILE_LIST.ToIterator();
            Iterator iterator1 = parent_file.GetFileList().ToIterator();
            while (iterator1.HasNext()) {
                var a_file1 = (IFile)iterator1.Next();
                while (iterator.HasNext()) {
                    var a_file = (IFile)iterator.Next();
                    if(a_file.GetName() == a_file1.GetName()) {
                        return false;
                    }
                }
            }
            return true;
        }

        public void FillTreeView() {
            mainForm.GetTreeView().Nodes.Clear();
            Iterator iterator = ALL_FILE_LIST.ToIterator();
            while (iterator.HasNext()) {
                var a_file = (IFile)iterator.Next();
                if (a_file.GetIndex() == "root") {
                    mainForm.GetTreeView().Nodes.Add(FileToTreeNode(a_file));
                }
            }
        }

        public void FillListView() {
            mainForm.GetListView().Items.Clear();
            IFile selectedFile = GetFileByTreeNode(SELECTED_NODE);
            if (selectedFile.GetFileType() == FileType.Folder) {
                Iterator iterator = selectedFile.GetFileList().ToIterator();
                while (iterator.HasNext()) {
                    var a_file = (IFile)iterator.Next();
                    ListViewItem a_item = FileToListViewItem(a_file);
                    mainForm.GetListView().Items.Add(a_item);
                }
            }
        }

        public void Expand() {
            TreeNode root = mainForm.GetTreeView().Nodes.Find("root", false).First();
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
            TreeNode root = mainForm.GetTreeView().Nodes.Find("root", false).First();
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

        }

        public void OpenTextFile(IFile my_file) {
            if (my_file.GetFileType() == FileType.Text) {
                //Form text = new 文本文档(my_file);
                //text.Show();
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
            ALL_FILE_LIST.Remove(my_file);
            RefreshForm();
        }

        public void Exit() {
            Application.Exit();
        }

        public IForm GetMainForm() {
            return mainForm;
        }

    }
}
