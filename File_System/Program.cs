using File_System.FileSystem.MyForm;
using File_System.FileSystem.SystemController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_System {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            ISystemController sysCtrl = SystemController.GetInstance();

            IMainForm mainForm = new Form1();
            ITextForm textForm = new Form2();

            sysCtrl.SetMainForm(mainForm);
            sysCtrl.SetTextForm(textForm);

            sysCtrl.Init();

            sysCtrl.Run(mainForm);

        }
    }
}
