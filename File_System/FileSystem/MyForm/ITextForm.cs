using File_System.FileSystem.FileBase.MyFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_System.FileSystem.MyForm {
    public interface ITextForm {
        TextBox GetTextBox();

        void SetTextFile(IFile file);

        ITextForm DeepCopy();

        void ShowTextForm();
    }
}
