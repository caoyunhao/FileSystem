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

    public partial class Form1 : Form, IMainForm {

        private ISystemController sysCtrl;

        public Form1() {
            InitializeComponent();

            //单例模式---------------------------
            sysCtrl = SystemController.GetInstance();
            //----------------------------------

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
                    //sysCtrl.GetHistoryPath().Push(sysCtrl.GetSelectedNode().FullPath);
                    sysCtrl.GetFuturePath().Clear();
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
            if (sysCtrl.GetHistoryPath().Count > 0) {
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
            if (sysCtrl.GetMainForm().GetListView().SelectedItems.Count == 0) {
                return;
            }
            var file = sysCtrl.GetFileByListViewItem(sysCtrl.GetMainForm().GetListView().SelectedItems[0]);
            var temp = file;
            var oldName = file.GetName();
            var newName = e.Label;
            if (e.Label != null) {
                if (newName.Trim() == "") {
                    MessageBox.Show("文件名不能为空！");
                }
                else {
                    IFile pFile = sysCtrl.FindParentFile(file);
                    if (sysCtrl.CheckName(newName, pFile)) {
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
                    sysCtrl.RefreshForm();
                    //sysCtrl.SetSelectedNode(treeView1.SelectedNode);
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
                    IFile new_text = new TextFile(sysCtrl.GetNewName(sysCtrl.GetSelectedNode(), FileType.Text));
                    selectedFile.Add(new_text);
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
                //------原型模式-------
                IFile new_file = a_file.DeepCopy();     //深度复制该文件
                new_file.SetNewIndex();                 //设置新的GUID以区别旧文件
                //--------------------
                sysCtrl.GetSelectedNode().Nodes.Add(sysCtrl.FileToTreeNode(new_file));
                sysCtrl.GetFileByTreeNode(sysCtrl.GetSelectedNode()).GetFileList().Add(new_file);
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {

        }
    }
}