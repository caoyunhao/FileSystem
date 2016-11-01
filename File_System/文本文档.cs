using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_System {
    public partial class 文本文档 : Form {

        private MyFile MY_FILE;
        private bool MY_CLOSE = false;
        private bool HAS_CHANGED = false;
        private string OLD_CONTANT = "";

        public 文本文档() {
            InitializeComponent();
        }

        public 文本文档(MyFile my_file) {
            InitializeComponent();
            MY_FILE = my_file;
            textBox1.Text = MY_FILE.contant;
            OLD_CONTANT = textBox1.Text;
            this.Text = MY_FILE.name + " - 文本文档";
            textBox1.Select(0, 0);
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e) {
            MyFile newFile = MY_FILE;
            newFile.contant = textBox1.Text;
            Form1.UpdateFile(MY_FILE, newFile);
        }

        private void 文本文档_FormClosing(object sender, FormClosingEventArgs e) {
            if (OLD_CONTANT == textBox1.Text) {
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
                        MyFile newFile = MY_FILE;
                        newFile.contant = textBox1.Text;
                        Form1.UpdateFile(MY_FILE, newFile);
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
        }
    }
}
