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
            tsProgress.Maximum = 100;
            tsProgress.Step = 1;
            tsProgress.Value = 0;
            slProgress.Text = "0 из 100";
            slStatus.Text = "Ожидание обработки";
            lRet.Text = String.Format("Всего обработано {0} из {1}", 0, 0);
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;

            if (fbd.ShowDialog() != DialogResult.OK) return;

            tbPath.Text = fbd.SelectedPath;
            string[] flist = BLL.FileUtils.GetFileList(fbd.SelectedPath);
            slProgress.Text = String.Format("({0} из {1})", 0, flist.Length);
            slStatus.Text = "Ожидание обработки";

            lvFiles.Items.Clear();
            lvFiles.BeginUpdate();
            foreach (string file in flist)
            {
                ListViewItem lvi = new ListViewItem(BLL.FileUtils.GetShotFileName(file));
                lvi.Tag = file;
                lvi.SubItems.Add("Не добавлен");
                lvFiles.Items.Add(lvi);
            }
            lvFiles.EndUpdate();

        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (lvFiles.Items.Count <= 0) return;

            int iTotal = lvFiles.Items.Count;
            int iCurr = 0;

            slProgress.Text = String.Format("({0} из {1})", iCurr, iTotal);
            slStatus.Text = "Обработка";
            tsProgress.Maximum = iTotal;
            tsProgress.Step = 1;
            tsProgress.Value = iCurr;
            btnClose.Enabled = false;
            btnStart.Enabled = false;
            btnSelectPath.Enabled = false;

            foreach (ListViewItem lvi in lvFiles.Items)
            {
                String str = await Task.Factory.StartNew<string>(() => Worker.AddFile(Mng, Convert.ToString(lvi.Tag)), TaskCreationOptions.LongRunning);
                lvi.SubItems[1].Text = str;
                iCurr++;
                slProgress.Text = String.Format("({0} из {1})", iCurr, iTotal);
                tsProgress.Maximum = iTotal;
                tsProgress.Step = 1;
                tsProgress.Value = iCurr;
                // Автоматический скрол до выбранного элемента
                lvFiles.EnsureVisible(lvi.Index);
            }

            slStatus.Text = "Обработка завершена";
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
            BLL.RetStatus status  = Mng.AddBook(fn);
            if (status.error == BLL.eRetError.NoErr)
                return String.Format("Добавлен id = {0}", status.id);
            else
            {
                switch (status.error)
                {
                    case BLL.eRetError.ErrAlreadyAdd: return String.Format("Ранее был добавлен id = {0}", status.id);
                    case BLL.eRetError.ErrAddToArc: return String.Format("Ошибка (добавление в архив)");
                    case BLL.eRetError.ErrAddToDB: return String.Format("Ошибка (добавление в БД)");
                    case BLL.eRetError.ErrMD5: return String.Format("Ошибка (подсчет MD5)");
                    case BLL.eRetError.ErrReadDesc: return String.Format("Ошибка (fb2 descriptor)");
                    default:  return "Не известная ошибка";
                }
            }
        }


    }



}
