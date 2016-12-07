using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using File_System.FileSystem.MyForm;
using File_System.FileSystem.SystemController;
using File_System.FileSystem.FileBase.MyFile;
using File_System.FileSystem.MyIterator;

namespace File_System {

    public partial class Form1 : Form, IForm {
        /*
                private const string FILE_NAME = "MyFile.bin";
                /// <summary>
                /// 全部文件List
                /// </summary>
                public static List<MyFile> ALL_FILE_LIST = new List<MyFile>();
                /// <summary>
                /// 被选择的树节点
                /// </summary>
                private TreeNode SELECT_NODE = new TreeNode();
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
                private List<MyFile> CLIPBOARD = new List<MyFile>();



                /// <summary>
                /// 读二进制文件
                /// </summary>
                /// <param name="file_name"></param>
                /// <param name="file_list"></param>
                /// <returns></returns>
                private List<MyFile> ReadBinFile(string file_name, out List<MyFile> file_list) {
                    file_list = new List<MyFile>();
                    BinaryFormatter bf = new BinaryFormatter();
                    Stream st = new FileStream(file_name, FileMode.Open);
                    do {
                        if (st.Position == st.Length)
                            break;
                        MyFile my_file = bf.Deserialize(st) as MyFile;
                        file_list.Add(my_file);
                        continue;
                    } while (true);
                    st.Close();
                    return file_list;
                }

                /// <summary>
                /// 写二进制文件
                /// </summary>
                /// <param name="file_name"></param>
                /// <param name="file_list"></param>
                private void WriteBinFile(string file_name, List<MyFile> file_list) {
                    BinaryFormatter bf = new BinaryFormatter();
                    Stream st = new FileStream(file_name, FileMode.Create, FileAccess.Write, FileShare.None);
                    foreach (MyFile a_file in file_list) {
                        bf.Serialize(st, a_file);
                    }
                    st.Close();
                }

                /// <summary>
                /// 文件转换成TreeNode
                /// </summary>
                /// <param name="my_file"></param>
                /// <returns></returns>
                private TreeNode FileToTreeNode(MyFile my_file) {
                    TreeNode treeNode = new TreeNode();
                    string parent_index = my_file.index;
                    treeNode.Text = my_file.name;
                    treeNode.Name = my_file.index;
                    if (my_file.isFolder) {
                        if (my_file.elementIndex.Count > 0) {
                            var i = 0;
                            var temp_index = new List<string>();
                            var temp_list = my_file.GetElementCollection(ALL_FILE_LIST);
                            foreach (var a_file in temp_list) {
                                a_file.index = parent_index + "_" + (i++);
                                temp_index.Add(a_file.index);
                                treeNode.Nodes.Add(FileToTreeNode(a_file));
                            }
                            my_file.elementIndex = temp_index;
                        }
                    }
                    return treeNode;
                }

                /// <summary>
                /// 文件转换为ListViewItem
                /// </summary>
                /// <param name="my_file"></param>
                /// <returns></returns>
                private ListViewItem FileToListViewItem(MyFile my_file) {
                    ListViewItem result = new ListViewItem(new string[] { my_file.name, my_file.GetFileSize(), my_file.GetFileType(), my_file.timeOfLastAlter });
                    result.Name = my_file.index;
                    result.Tag = my_file.GetFileType();
                    return result;
                }

                /// <summary>
                /// 通过节点获取文件
                /// </summary>
                /// <param name="tree_node"></param>
                /// <returns></returns>
                private MyFile GetFileByTreeNode(TreeNode tree_node) {
                    MyFile result = new MyFile();
                    foreach (MyFile a_file in ALL_FILE_LIST) {
                        if (a_file.index == tree_node.Name) {
                            result = a_file;
                        }
                    }
                    return result;
                }

                private MyFile GetFileByListViewItem(ListViewItem list_view_item) {
                    var result = new MyFile();
                    foreach (MyFile a_file in ALL_FILE_LIST) {
                        if (a_file.index == list_view_item.Name) {
                            result = a_file;
                        }
                    }
                    return result;
                }

                /// <summary>
                /// 获取新的索引号
                /// </summary>
                /// <param name="tree_node"></param>
                /// <returns></returns>
                private string GetNewIndex(TreeNode tree_node) {
                    string result = "";
                    string nodeIndex = tree_node.Name;
                    result += nodeIndex;
                    for (int i = 0; ; ++i) {
                        bool had = false;
                        foreach (TreeNode a_node in tree_node.Nodes) {
                            string a_index = a_node.Name;
                            int a_last_index = a_index.LastIndexOf("_");

                            string a_head = a_index.Substring(0, a_last_index);
                            string a_tail = a_index.Remove(0, a_head.Length + 1);
                            if (i.ToString().Trim() == a_tail.Trim()) {
                                had = true;
                                break;
                            }
                        }
                        if (!had) {
                            result += "_" + i.ToString().Trim();
                            break;
                        }
                    }
                    return result;
                }

                /// <summary>
                /// 获取一个新的文件名
                /// </summary>
                /// <param name="tree_node"></param>
                /// <param name="is_folder"></param>
                /// <returns></returns>
                private string GetNewName(TreeNode tree_node, bool is_folder) {
                    string result = "";
                    if (is_folder) {
                        result = "新建文件夹";
                        for (int i = 1; ; ++i) {
                            bool hadNew = false;
                            bool had = false;
                            foreach (TreeNode a_node in tree_node.Nodes) {
                                string a_node_text = a_node.Text;
                                if (a_node_text == result) {
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
                                return result;
                            }
                            if (!had) {
                                result += "（" + i.ToString().Trim() + "）";
                                return result;
                            }
                        }
                    }
                    else {
                        result = "新建文本文档";
                        for (int i = 1; ; ++i) {
                            bool hadNew = false;
                            bool had = false;
                            foreach (TreeNode a_node in tree_node.Nodes) {
                                string a_node_text = a_node.Text;
                                if (a_node_text == result + ".txt") {
                                    hadNew = true;
                                    a_node_text = result + "（0）.txt";
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
                                return result + ".txt";
                            }
                            if (!had) {
                                result += "（" + i.ToString().Trim() + "）" + ".txt";
                                return result;
                            }
                        }
                    }
                }

                /// <summary>
                /// 通过唯一索引号查找文件
                /// </summary>
                /// <param name="index"></param>
                /// <returns></returns>
                private MyFile FindFileByIndex(string index) {
                    var result = new MyFile();
                    foreach(var a_file in ALL_FILE_LIST) {
                        if(a_file.index == index) {
                            return a_file;
                        }
                    }
                    MessageBox.Show("未成功通过索引获取文件，系统错误！" + index);
                    return result;
                }

                /// <summary>
                /// 通过string list查找节点
                /// </summary>
                /// <param name="path_list"></param>
                private void FindNodeByStringList(List<string> path_list) {
                    TreeNode root = treeView1.Nodes.Find("root", false).First();
                    if (root.Text != path_list[0]) {
                        MessageBox.Show("错误的路径！");
                        return;
                    }
                    if (path_list.Count == 1) {
                        treeView1.SelectedNode = root;
                    }
                    TreeNode temp = root;
                    for (int i = 0; i < path_list.Count; ++i) {
                        bool has = true;
                        foreach (TreeNode a_node in temp.Nodes) {
                            string str = path_list[i];
                            if (i == path_list.Count - 1 && a_node.Text == str) {
                                treeView1.SelectedNode = a_node;
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

                /// <summary>
                /// 查找父文件
                /// </summary>
                /// <param name="child_file"></param>
                /// <returns></returns>
                private MyFile FindParentFile(MyFile child_file) {
                    var resule = new MyFile();
                    foreach(var parent_file in ALL_FILE_LIST) {
                        if (parent_file.isFolder) {
                            foreach (var child_index in parent_file.elementIndex) {
                                if (child_file.index == child_index) {
                                    resule = parent_file;
                                }
                            }
                        }
                    }
                    return resule;
                }

                /// <summary>
                /// 在文件List里的索引值
                /// </summary>
                /// <param name="my_file"></param>
                /// <returns></returns>
                public static int IndexInFileList(MyFile my_file) {
                    int result = 0;
                    for (; result < ALL_FILE_LIST.Count; ++result) {
                        if (ALL_FILE_LIST[result].index == my_file.index) {
                            return result;
                        }
                    }
                    return -1;
                }

                /// <summary>
                /// 检测该文件夹中是否存在该名字
                /// </summary>
                /// <param name="new_name"></param>
                /// <param name="parent_file"></param>
                /// <returns></returns>
                private bool CheckName(string new_name, MyFile parent_file) {
                    var children_index = parent_file.elementIndex;
                    foreach (var a_index in children_index) {
                        foreach (var a_file in ALL_FILE_LIST) {
                            if (a_file.index == a_index) {
                                if (new_name == a_file.name) {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }

                /// <summary>
                /// 填充TreeList
                /// </summary>
                /// <param name="file_list"></param>
                private void FillTreeView() {
                    treeView1.Nodes.Clear();
                    foreach (MyFile a_file in ALL_FILE_LIST) {
                        if (a_file.index == "root") {
                            treeView1.Nodes.Add(FileToTreeNode(a_file));
                        }
                    }
                }

                /// <summary>
                /// 填充ListView
                /// </summary>
                /// <param name="tree_node"></param>
                private void FillListView() {
                    listView1.Items.Clear();
                    MyFile selectedFile = GetFileByTreeNode(SELECT_NODE);
                    if (selectedFile.elementIndex.Count > 0) {
                        foreach (MyFile a_file in selectedFile.GetElementCollection(ALL_FILE_LIST)) {
                            ListViewItem a_item = FileToListViewItem(a_file);
                            listView1.Items.Add(a_item);
                        }
                    }
                }

                /// <summary>
                /// 恢复展开
                /// </summary>
                private void Expand() {
                    TreeNode root = treeView1.Nodes.Find("root", false).First();
                    ExpandedSupport(root);
                }

                /// <summary>
                /// 恢复展开 - 支持函数
                /// </summary>
                /// <param name="root"></param>
                private void ExpandedSupport(TreeNode root) {
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

                /// <summary>
                /// 记录展开
                /// </summary>
                private void Sign() {
                    TreeNode root = treeView1.Nodes.Find("root", false).First();
                    SignSupport(root);
                }

                /// <summary>
                /// 记录展开 - 支持函数
                /// </summary>
                /// <param name="root"></param>
                private void SignSupport(TreeNode root) {
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

                /// <summary>
                /// 界面刷新
                /// </summary>
                private void RefreshForm() {
                    Sign();
                    FillTreeView();
                    Expand();
                    FillListView();
                }

                /// <summary>
                /// 显示路径
                /// </summary>
                private void ShowPath() {
                    textBox1.Text = SELECT_NODE.FullPath;
                }

                /// <summary>
                /// 通过路径选择文件
                /// </summary>
                /// <param name="path"></param>
                private void PathToSelect(string path) {
                    FindNodeByStringList(PathToStringList(path));
                }

                /// <summary>
                /// 文件路径转换成string list
                /// </summary>
                /// <param name="path"></param>
                /// <returns></returns>
                private List<string> PathToStringList(string path) {
                    List<string> result = new List<string>();
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

                /// <summary>
                /// 更新某个文件
                /// </summary>
                /// <param name="old_file"></param>
                /// <param name="new_file"></param>
                public static void UpdateFile(MyFile old_file, MyFile new_file) {
                    ALL_FILE_LIST[IndexInFileList(old_file)] = new_file;
                }

                /// <summary>
                /// 打开文本文件
                /// </summary>
                /// <param name="my_file"></param>
                private void OpenTextFile(MyFile my_file) {
                    if (!my_file.isFolder) {
                        Form text = new 文本文档(my_file);
                        text.Show();
                    }
                    else {
                        MessageBox.Show("打开错误！");
                    }
                }

                /// <summary>
                /// 打开文件夹
                /// </summary>
                /// <param name="my_file"></param>
                private void OpenFolder(MyFile my_file) {
                    if (my_file.isFolder) {
                        treeView1.SelectedNode = treeView1.Nodes.Find(my_file.index, true)[0];
                    }
                    else {
                        MessageBox.Show("打开错误！");
                    }
                }

                /// <summary>
                /// 删除文件
                /// </summary>
                /// <param name="my_file"></param>
                private void DeleteFile(MyFile my_file) {
                    MyFile tempFile = new MyFile();
                    my_file.CopyTo(out tempFile);
                    if (tempFile.isFolder) {
                        foreach(var a_index in tempFile.elementIndex) {
                            DeleteFile(FindFileByIndex(a_index));
                        }
                    }
                    var parent = FindParentFile(my_file);
                    for (int i = 0; i < parent.elementIndex.Count; ++i) {
                        if(parent.elementIndex[i] == my_file.index) {
                            ALL_FILE_LIST[IndexInFileList(parent)].elementIndex.RemoveAt(i);
                            break;
                        }
                    }
                    ALL_FILE_LIST.RemoveAt(IndexInFileList(my_file));
                    RefreshForm();
                }
        */


        private ISystemController sysCtrl;



        public Form1() {
            InitializeComponent();
            sysCtrl = SystemController.GetInstance();

            //ALL_FILE_LIST.RemoveAt(1);
            //WriteBinFile(FILE_NAME, ALL_FILE_LIST);
        }



        /// <summary>
        /// 通过路径查找目标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goPath_Click(object sender, EventArgs e) {
            //PathToSelect(textBox1.Text);
            sysCtrl.PathToSelect(textBox1.Text);
        }

        /// <summary>
        /// 选中节点后再ListView显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            //SELECT_NODE = e.Node;
            sysCtrl.SetSelectedNode(e.Node);
            //FillListView();
            sysCtrl.FillListView();
            sysCtrl.GetHistoryPath().Push(sysCtrl.GetSelectedNode().FullPath);
            sysCtrl.ShowPath();
        }

        /// <summary>
        /// 右键弹出菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                if (this.sysCtrl.GetMainForm().GetListView().GetItemAt(e.X, e.Y) != null) {
                    this.sysCtrl.GetMainForm().GetListView().ContextMenuStrip = this.ListViewItemMenu;
                }
                else {
                    this.sysCtrl.GetMainForm().GetListView().ContextMenuStrip = this.contextMenuStrip1;
                }
            }
        }

        /// <summary>
        /// 双击打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ListViewItem temp = sysCtrl.GetMainForm().GetListView().GetItemAt(e.X, e.Y);
                if (temp != null) {
                    IFile file = sysCtrl.GetFileByListViewItem(temp);
                    sysCtrl.OpenFile(file);
                }
            }
        }

        /// <summary>
        /// 后退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goBack_Click(object sender, EventArgs e) {
            //if (HISTORY_PATH.Count > 1) {
            //    FUTURE_PATH.Push(HISTORY_PATH.Pop());
            //    string historyPath = HISTORY_PATH.Pop();
            //    PathToSelect(historyPath);
            //}
            //else {
            //    MessageBox.Show("没有可返回的路径！");
            //}
            if (sysCtrl.GetHistoryPath().Count > 1) {
                sysCtrl.GetFuturePath().Push(sysCtrl.GetHistoryPath().Pop());
                string hisPath = sysCtrl.GetHistoryPath().Pop();
                sysCtrl.PathToSelect(hisPath);
            }
            else {
                MessageBox.Show("没有可返回的路径！");
            }
        }

        /// <summary>
        /// 前进
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goPrev_Click(object sender, EventArgs e) {
            //if (FUTURE_PATH.Count > 0) {
            //    string futurePath = FUTURE_PATH.Pop();
            //    PathToSelect(futurePath);
            //}
            //else {
            //    MessageBox.Show("没有可前进的路径！");
            //}
            if (sysCtrl.GetFuturePath().Count > 0) {
                string futPath = sysCtrl.GetFuturePath().Pop();
                sysCtrl.PathToSelect(futPath);
            }
            else {
                MessageBox.Show("没有可前进的路径！");
            }
        }

        /// <summary>
        /// 重命名更新函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            var file = sysCtrl.GetFileByListViewItem(sysCtrl.GetMainForm().GetListView().SelectedItems[0]);
            var temp = file;
            var oldName = file.GetName();
            var newName = e.Label;
            if (e.Label != null) {
                if (newName.Trim() == "") {
                    MessageBox.Show("文件名不能为空！");
                }
                else {
                    if (sysCtrl.CheckName(newName, sysCtrl.FindParentFile(file))) {
                        //temp.name = newName;
                        //UpdateFile(file, temp);
                        temp.SetName(newName);
                    }
                    else {
                        MessageBox.Show("错误：重复命名！");
                    }
                }
            }
            sysCtrl.RefreshForm();
        }

        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 文件夹ToolStripMenuItem_Click(object sender, EventArgs e) {
            //if (SELECT_NODE != null) {
            //    MyFile selectedFile = GetFileByTreeNode(SELECT_NODE);
            //    if (selectedFile.isFolder) {
            //        MyFile new_file = new MyFile(GetNewIndex(SELECT_NODE), GetNewName(SELECT_NODE, true), true);
            //        if (IndexInFileList(selectedFile) == -1) {
            //            MessageBox.Show("错误！");
            //        }
            //        ALL_FILE_LIST[IndexInFileList(selectedFile)].elementIndex.Add(new_file.index);
            //        ALL_FILE_LIST.Add(new_file);
            //        SELECT_NODE.Nodes.Add(FileToTreeNode(new_file));
            //        RefreshForm();
            //    }
            //}
            if (sysCtrl.GetSelectedNode() != null) {
                IFile selectedFile = sysCtrl.GetFileByTreeNode(sysCtrl.GetSelectedNode());
                if (selectedFile.GetFileType() == FileType.Folder) {
                    IFile new_folder = new FolderFile(sysCtrl.GetNewName(sysCtrl.GetSelectedNode(), FileType.Folder));
                    selectedFile.Add(new_folder);
                    sysCtrl.GetAllFileList().Add(new_folder);
                    sysCtrl.RefreshForm();
                }
            }
        }

        /// <summary>
        /// 新建文本文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 文本文档ToolStripMenuItem_Click(object sender, EventArgs e) {
            //MyFile selectedFile = GetFileByTreeNode(SELECT_NODE);
            //if (selectedFile.isFolder) {
            //    MyFile new_file = new MyFile(GetNewIndex(SELECT_NODE), GetNewName(SELECT_NODE, false), false);
            //    selectedFile.elementIndex.Add(new_file.index);
            //    SELECT_NODE.Nodes.Add(FileToTreeNode(new_file));
            //    ALL_FILE_LIST.Add(new_file);
            //    RefreshForm();
            //}
            if (sysCtrl.GetSelectedNode() != null) {
                IFile selectedFile = sysCtrl.GetFileByTreeNode(sysCtrl.GetSelectedNode());
                if (selectedFile.GetFileType() == FileType.Folder) {
                    IFile new_text = new FolderFile(sysCtrl.GetNewName(sysCtrl.GetSelectedNode(), FileType.Text));
                    selectedFile.Add(new_text);
                    sysCtrl.GetAllFileList().Add(new_text);
                    sysCtrl.RefreshForm();
                }
            }
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e) {
            //Application.Exit();
            sysCtrl.Exit();
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e) {
            //WriteBinFile(FILE_NAME, ALL_FILE_LIST);
            sysCtrl.WriteBinFile();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e) {
            //OpenTextFile(GetFileByListViewItem(listView1.SelectedItems[0]));
            sysCtrl.OpenFile(sysCtrl.GetFileByListViewItem(sysCtrl.GetMainForm().GetListView().SelectedItems[0]));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e) {
            //DeleteFile(GetFileByListViewItem(listView1.SelectedItems[0]));
            sysCtrl.DeleteFile(sysCtrl.GetFileByListViewItem(sysCtrl.GetMainForm().GetListView().SelectedItems[0]));
        }

        /// <summary>
        /// 重命名编辑函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 重命名ToolStripMenuItem_Click(object sender, EventArgs e) {
            var item = sysCtrl.GetMainForm().GetListView().SelectedItems[0];
            item.BeginEdit();
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e) {
            sysCtrl.GetClipBoard().Clear();
            foreach (ListViewItem a_item in sysCtrl.GetMainForm().GetListView().SelectedItems) {
                IFile newFile = sysCtrl.GetFileByListViewItem(a_item).DeepCopy();
                sysCtrl.GetClipBoard().Add(newFile);
            }
            contextMenuStrip1.Items.Find("粘贴ToolStripMenuItem", false)[0].Enabled = true;
        }

        /// <summary>
        /// 粘贴文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e) {
            var parent = sysCtrl.GetFileByTreeNode(sysCtrl.GetSelectedNode());

            //foreach (var a_file in CLIPBOARD) {
            //    foreach (var a_index in parent.elementIndex) {
            //        var file = sysCtrl.FindFileByIndex(a_index);
            //        if (file.name == a_file.name) {
            //            MessageBox.Show("存在重复名称！ 复制失败！");
            //            return;
            //        }
            //    }
            //    a_file.CopyTo(out newFile);
            //    newFile.index = GetNewIndex(SELECT_NODE);
            //    SELECT_NODE.Nodes.Add(FileToTreeNode(newFile));
            //    ALL_FILE_LIST[IndexInFileList(GetFileByTreeNode(SELECT_NODE))].elementIndex.Add(newFile.index);
            //    ALL_FILE_LIST.Add(newFile);
            //}
            //RefreshForm();

            Iterator clipIterator = sysCtrl.GetClipBoard().ToIterator();
            while (clipIterator.HasNext()) {
                IFile a_file = (IFile)clipIterator.Next();
                if (parent.GetFileType() == FileType.Folder) {
                    Iterator parentItor = parent.GetFileList().ToIterator();
                    while (parentItor.HasNext()) {
                        var p_file = (IFile)parentItor.Next();
                        if (a_file.GetName() == p_file.GetName()) {
                            MessageBox.Show("存在重复名称！ 复制失败！");
                            return;
                        }
                    }
                }
                IFile new_file = a_file.DeepCopy();
                new_file.SetIndex();
                sysCtrl.GetSelectedNode().Nodes.Add(sysCtrl.FileToTreeNode(new_file));
                sysCtrl.GetFileByTreeNode(sysCtrl.GetSelectedNode()).GetFileList().Add(new_file);
                sysCtrl.GetAllFileList().Add(new_file);
            }
            sysCtrl.RefreshForm();
        }

        /// <summary>
        /// 查找按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Find_Click(object sender, EventArgs e) {
            var target = sysCtrl.GetMainForm().GetSearchText().Text;
            sysCtrl.GetMainForm().GetListView().Items.Clear();
            Iterator iterator = sysCtrl.GetAllFileList().ToIterator();
            while (iterator.HasNext()) {
                var a_file = (IFile)iterator.Next();
                if (a_file.GetName() == target) {
                    sysCtrl.GetMainForm().GetListView().Items.Add(
                        sysCtrl.FileToListViewItem(a_file)
                        );
                }
            }


            //foreach (var a_file in ALL_FILE_LIST) {
            //    if (a_file.name == target) {
            //        listView1.Items.Add(FileToListViewItem(a_file));
            //    }
            //}
        }

        /// <summary>
        /// 刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refresh_Click(object sender, EventArgs e) {
            sysCtrl.FillTreeView();
            sysCtrl.GetMainForm().GetTreeView().SelectedNode = sysCtrl.GetMainForm().GetTreeView().Nodes[0];
            listView1.Items.Clear();
        }

        public TreeView GetTreeView() {
            return treeView1;
        }

        public ListView GetListView() {
            return listView1;
        }

        public TextBox GetPathTextBox() {
            return textBox1;
        }

        public TextBox GetSearchText() {
            return findText;
        }

        public Form ToForm() {
            return this;
        }
    }
}