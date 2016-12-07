using File_System.FileSystem.FileBase.MyFile;
using File_System.FileSystem.FileBase.MyFileList;
using File_System.FileSystem.FileBase.MyPathList;
using File_System.FileSystem.MyForm;
using File_System.FileSystem.MyIterator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_System.FileSystem.SystemController {
    public interface ISystemController {
        void ReadBinFile();

        void WriteBinFile();

        TreeNode FileToTreeNode(IFile my_file);

        ListViewItem FileToListViewItem(IFile my_file);

        IFile GetFileByTreeNode(TreeNode tree_node);

        IFile FindFileByIndex(string index);

        void FindNodeByStringList(IPathList path_list);

        IFile FindParentFile(IFile child_file);

        bool CheckName(string new_name, IFile parent_file);

        void FillTreeView();

        void FillListView();

        void Expand();

        void ExpandedSupport(TreeNode root);

        void Sign();

        /// <summary>
        /// 记录展开 - 支持函数
        /// </summary>
        /// <param name="root"></param>
        void SignSupport(TreeNode root);

        void RefreshForm();

        void ShowPath();

        void PathToSelect(string path);

        IPathList PathToList(string path);

        void OpenFile(IFile file);

        void OpenTextFile(IFile my_file);

        void OpenFolder(IFile my_file);

        void DeleteFile(IFile my_file);







        void CreateTree();

        void Run(IForm form);

        void SetMainForm(IForm form);

        void SetTextForm(IForm form);

        IFile GetFileByListViewItem(ListViewItem list_view_item);

        string GetNewName(TreeNode tree_node, FileType fileType);

        void SetSelectedNode(TreeNode node);

        Stack<string> GetHistoryPath();

        Stack<string> GetFuturePath();

        TreeNode GetSelectedNode();

        IFileList GetAllFileList();

        void Exit();

        IForm GetMainForm();

        IFileList GetClipBoard();
    }
}
