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
            //lRet.Text = String.Format("Всего обработано {0} из {1}", 0, 0);
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
                //lvi.Tag = file;
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

            UpadateStatusBarValues(iCurr, iTotal, "Обработка");
            UpadateBtnEnableStatus(false, false, false);

            foreach (ListViewItem lvi in lvFiles.Items)
            {
                iCurr++;
                BLL.RetStatus status = await Task.Factory.StartNew<BLL.RetStatus>(() => Worker.AddFile(Mng, String.Format("{0}\\{1}", tbPath.Text, lvi.SubItems[0].Text)), TaskCreationOptions.LongRunning);
                UpadateListViewItem(lvi, status);
                UpadateStatusBarValues(iCurr, iTotal, "Обработка");                
                lvFiles.EnsureVisible(lvi.Index); // Автоматический скрол до выбранного элемента
            }

            UpadateStatusBarValues(iCurr, iTotal, "Удаление");
            if (cbAutoDelete.Checked)
                for (int i = lvFiles.Items.Count - 1; i >= 0; i--)
                    if (((BLL.RetStatus)lvFiles.Items[i].Tag).error == BLL.eRetError.NoErr ||
                        ((BLL.RetStatus)lvFiles.Items[i].Tag).error == BLL.eRetError.ErrAlreadyAdd)
                    {
                        UpadateStatusBarValues(iTotal - i, iTotal, "Удаление");
                        if (!BLL.FileUtils.DeleteFile(string.Format("{0}\\{1}", tbPath.Text, lvFiles.Items[i].SubItems[0].Text)))
                        {
                            lvFiles.Items[i].SubItems[1].Text = "Ошибка удаления файла";
                            lvFiles.Items[i].Tag = new BLL.RetStatus(BLL.eRetError.ErrDelFile, ((BLL.RetStatus)lvFiles.Items[i].Tag).id);
                        }
                        else
                            lvFiles.Items[i].Remove();

                    }

            UpadateStatusBarValues(iCurr, iTotal, "Обработка завершена");
            UpadateBtnEnableStatus(true, true, true);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpadateStatusBarValues(int curr, int total, string status)
        {
            slStatus.Text = status;
            slProgress.Text = String.Format("({0} из {1})", curr, total);
            tsProgress.Maximum = total;
            tsProgress.Step = 1;
            tsProgress.Value = curr;
        }

        private void UpadateBtnEnableStatus(bool сlose, bool start, bool path)
        {
            btnClose.Enabled = сlose;
            btnStart.Enabled = start;
            btnSelectPath.Enabled = path;
        }

        private void UpadateListViewItem(ListViewItem lvi, BLL.RetStatus status)
        {
            switch (status.error)
            {
                case BLL.eRetError.NoErr:         { lvi.SubItems[1].Text = String.Format("Добавлен id = {0}", status.id);           break; }
                case BLL.eRetError.ErrAlreadyAdd: { lvi.SubItems[1].Text = String.Format("Ранее был добавлен id = {0}", status.id); break; }
                case BLL.eRetError.ErrAddToArc:   { lvi.SubItems[1].Text = String.Format("Ошибка (добавление в архив)");            break; }
                case BLL.eRetError.ErrAddToDB:    { lvi.SubItems[1].Text = String.Format("Ошибка (добавление в БД)");               break; }
                case BLL.eRetError.ErrMD5:        { lvi.SubItems[1].Text = String.Format("Ошибка (подсчет MD5)");                   break; }
                case BLL.eRetError.ErrReadDesc:   { lvi.SubItems[1].Text = String.Format("Ошибка (fb2 descriptor)");                break; }
                default: { lvi.SubItems[1].Text = "Не известная ошибка"; break; }
            }
            lvi.Tag = status;
        }
    }


    class Worker
    {
        public static BLL.RetStatus AddFile(BLL.FB2SnitchManager Mng, string fn)
        {
            return Mng.AddBook(fn);
        }


    }



}
