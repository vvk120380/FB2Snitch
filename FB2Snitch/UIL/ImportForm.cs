using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        Progress<ProgessRet> progress;

        public ImportForm()
        {
            InitializeComponent();
            progress = new Progress<ProgessRet>(s => {
                slProgress.Text = s.ToString();
                tsProgress.Maximum = s.total;
                tsProgress.Step = 1;
                tsProgress.Value = s.cur;
                tsError.Text = s.err.ToString();
                tsTime.Text = s.time;
            });
        }

        public ImportForm(BLL.FB2SnitchManager Mng) : this()
        {
            this.Mng = Mng;
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            UpdateStatusBarValues(0, 0, "Ожидание обработки...");
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;

            if (fbd.ShowDialog() != DialogResult.OK) return;

            tbPath.Text = fbd.SelectedPath;
            string[] flist = BLL.FileUtils.GetFileList(fbd.SelectedPath);

            UpdateStatusBarValues(0, 0, "Ожидание обработки...");

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

            UpdateStatusBarValues(iCurr, iTotal, "Обработка");
            UpdateBtnEnableStatus(false, false, false);

            //Формируем список файлов
            List<string> fns = new List<string>();
            for (int i = 0; i < lvFiles.Items.Count; i++)
                fns.Add(String.Format("{0}\\{1}", tbPath.Text, lvFiles.Items[i].SubItems[0].Text));

            //Добавляем файлы в архив и в БД
            List<BLL.RetStatus> stateList = await Task.Factory.StartNew<List<BLL.RetStatus>>(() => Worker.AddFiles(progress, Mng, fns), TaskCreationOptions.LongRunning);

            //Отображаем состояние - был ли файл добавлен и если нет, то почему
            lvFiles.BeginUpdate();
            for (int i = 0; i < lvFiles.Items.Count; i++)
                UpadateListViewItem(lvFiles.Items[i], stateList[i]);
            lvFiles.EndUpdate();

            //foreach (ListViewItem lvi in lvFiles.Items)
            //{
            //    iCurr++;
            //    BLL.RetStatus status = await Task.Factory.StartNew<BLL.RetStatus>(() => Worker.AddFile(Mng, String.Format("{0}\\{1}", tbPath.Text, lvi.SubItems[0].Text)), TaskCreationOptions.LongRunning);
            //    UpadateListViewItem(lvi, status);
            //    UpdateStatusBarValues(iCurr, iTotal, "Обработка");                
            //    lvFiles.EnsureVisible(lvi.Index); // Автоматический скрол до выбранного элемента                
            //}

            UpdateStatusBarValues(iCurr, iTotal, "Удаление");
            if (cbAutoDelete.Checked)
                for (int i = lvFiles.Items.Count - 1; i >= 0; i--)
                    if (((BLL.RetStatus)lvFiles.Items[i].Tag).error == BLL.eRetError.NoErr ||
                        ((BLL.RetStatus)lvFiles.Items[i].Tag).error == BLL.eRetError.ErrAlreadyAdd)
                    {
                        UpdateStatusBarValues(iTotal - i, iTotal, "Удаление");
                        string path = String.Format("{0}\\{1}", tbPath.Text, lvFiles.Items[i].SubItems[0].Text);
                        bool isDelete = await Task.Factory.StartNew<bool>(() => Worker.DeleteFile(path), TaskCreationOptions.LongRunning);
                        if (!isDelete)
                        {
                            lvFiles.Items[i].SubItems[1].Text = "Ошибка удаления файла";
                            lvFiles.Items[i].Tag = new BLL.RetStatus(BLL.eRetError.ErrDelFile, ((BLL.RetStatus)lvFiles.Items[i].Tag).id);
                        }
                        else
                            lvFiles.Items[i].Remove();                        
                    }

            UpdateStatusBarValues(iCurr, iTotal, "Обработка завершена");
            UpdateBtnEnableStatus(true, true, true);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateStatusBarValues(int curr, int total, string status)
        {
            slStatus.Text = status;
            slProgress.Text = String.Format("({0} из {1})", curr, total);
            tsProgress.Maximum = total;
            tsProgress.Step = 1;
            tsProgress.Value = curr;
        }

        private void UpdateBtnEnableStatus(bool сlose, bool start, bool path)
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


    public partial class Worker
    {
        public static BLL.RetStatus AddFile(BLL.FB2SnitchManager Mng, string fn)
        {
            return Mng.AddBook(fn);
        }

        public static List<BLL.RetStatus> AddFiles(IProgress<ProgessRet> progress, BLL.FB2SnitchManager Mng, List<string> fns)
        {
            TimeSpan ts;
            string elapsedTime;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<BLL.RetStatus> stateList = new List<BLL.RetStatus>();
            int err = 0;
            for (int i = 0; i < fns.Count; i++)
            {

                BLL.RetStatus retStatus = Mng.AddBook(fns[i]);
                if (retStatus.error != BLL.eRetError.NoErr) err++;
                stateList.Add(retStatus);
                ts = stopWatch.Elapsed;
                elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                progress.Report(new ProgessRet(fns.Count, i + 1, err, "Обработка...", elapsedTime));
            }
            stopWatch.Stop();
            return stateList;
        }

        public static bool DeleteFile(string fn)
        {
            return BLL.FileUtils.DeleteFile(fn);
        }
    }



}
