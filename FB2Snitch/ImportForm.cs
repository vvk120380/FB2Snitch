using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FB2Snitch
{
    public partial class ImportForm : Form
    {
        BLL.FB2SnitchManager Mng;
        public ImportForm()
        {
            InitializeComponent();
        }

        public ImportForm(BLL.FB2SnitchManager Mng) : this()
        {
            this.Mng = Mng;
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            lRet.Text = String.Format("Всего обработано {0} из {1}", 0, 0);
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.RootFolder = Environment.SpecialFolder.Personal;
            fbd.ShowNewFolderButton = false;

            if (fbd.ShowDialog() != DialogResult.OK) return;

            tbPath.Text = fbd.SelectedPath;
            string[] flist = BLL.FileUtils.GetFileList(fbd.SelectedPath);
            lRet.Text = String.Format("Всего обработано {0} из {1}", 0, flist.Length);

            lvFiles.Items.Clear();
            foreach (string file in flist)
            {
                ListViewItem lvi = new ListViewItem(BLL.FileUtils.GetShotFileName(file));
                lvi.Tag = file;
                lvi.SubItems.Add("Не добавлен");
                lvFiles.Items.Add(lvi);
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (lvFiles.Items.Count <= 0) return;

            int iTotal = lvFiles.Items.Count;
            int iCurr = 0;

            lRet.Text = String.Format("Всего обработано {0} из {1}", iCurr, iTotal);
            btnClose.Enabled = false;
            btnStart.Enabled = false;
            btnSelectPath.Enabled = false;

            foreach (ListViewItem lvi in lvFiles.Items)
            {
                String str = await Task.Factory.StartNew<string>(() => Worker.AddFile(Mng, Convert.ToString(lvi.Tag)), TaskCreationOptions.LongRunning);
                lvi.SubItems[1].Text = str;
                iCurr++;
                lRet.Text = String.Format("Всего обработано {0} из {1}", iCurr, iTotal);
                // Автоматический скрол до выбранного элемента
                lvFiles.EnsureVisible(lvi.Index);
            }

            btnClose.Enabled = true;
            btnStart.Enabled = true;
            btnSelectPath.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    class Worker
    {
        public static string AddFile(BLL.FB2SnitchManager Mng, string fn)
        {
            return String.Format("Добавлен id = {0}", Mng.AddBook(fn));
        }
    }

}
