using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_System.FileSystem.MyForm {
    public interface IMainForm {
        TreeView GetTreeView();

        ListView GetListView();

        TextBox GetPathTextBox();

        TextBox GetSearchText();

        Form ToForm();
    }
}
