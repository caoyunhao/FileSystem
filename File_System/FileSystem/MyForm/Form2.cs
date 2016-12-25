using File_System.FileSystem.FileBase.MyFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using File_System.FileSystem.MyForm;
using File_System.FileSystem.SystemController;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace File_System {
    [Serializable]
    public partial class Form2 : Form, ITextForm {
        private ISystemController sysCtrl;
        private IFile MY_FILE;
        private bool MY_CLOSE = false;
        private bool HAS_CHANGED = false;

        public Form2() {
            InitializeComponent();
        }

        public Form2(IFile my_file) {
            InitializeComponent();
            SetTextFile(my_file);
        }

        public void Init() {
            textBox1.Text = MY_FILE.GetContent();
            this.Text = MY_FILE.GetName() + " - 文本文档";
            textBox1.Select(0, 0);
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e) {
            IFile newFile = MY_FILE;
            newFile.SetContent(textBox1.Text);
            //Form1.UpdateFile(MY_FILE, newFile);
        }

        private void 文本文档_FormClosing(object sender, FormClosingEventArgs e) {
            if (MY_FILE.GetContent() == textBox1.Text) {
                HAS_CHANGED = false;
            }
            else {
                HAS_CHANGED = true;
            }
            if (HAS_CHANGED) {
                if (e.CloseReason == CloseReason.UserClosing && !MY_CLOSE) {
                    var res = MessageBox.Show("是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (res == DialogResult.Yes) {
                        MY_CLOSE = true;
                        IFile newFile = MY_FILE;
                        newFile.SetContent(textBox1.Text);
                        this.Close();
                    }
                    if (res == DialogResult.No) {
                        MY_CLOSE = true;
                        this.Close();
                    }
                    else {
                        e.Cancel = true;
                    }
                }
            }
            //------------单例模式-------------
            sysCtrl = SystemController.GetInstance();
            sysCtrl.FillListView();
        }

        public TextBox GetTextBox() {
            return textBox1;
        }

        public void ShowTextForm() {
            this.Show();
        }

        public void SetTextFile(IFile file) {
            MY_FILE = file;
            Init();
        }

        public ITextForm DeepCopy() {
            ITextForm retval;
            using (MemoryStream ms = new MemoryStream()) {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Seek(0, SeekOrigin.Begin);
                retval = bf.Deserialize(ms) as ITextForm;
                ms.Close();
            }
            return retval as ITextForm;
        }
    }
}
