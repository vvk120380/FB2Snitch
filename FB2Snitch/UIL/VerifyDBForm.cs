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

namespace FB2Snitch.UIL
{

    public struct ProgessRet
    {
        public int total;
        public int cur;
        public int err;
        public string status;
        public string time;

        public override string ToString()
        {
            return String.Format("({0} из {1})", cur, total);
        }

        public ProgessRet(int total, int cur, int err, string status, string time)
        {
            this.total = total;
            this.cur = cur;
            this.err = err;
            this.status = status;
            this.time = time;
        }

    }

    public partial class VerifyDBForm : Form
    {

        BLL.FB2SnitchManager Mng;



        Progress<ProgessRet> progress;
        public VerifyDBForm()
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

        public VerifyDBForm(BLL.FB2SnitchManager mng) : this()
        {
            Mng = mng;
        }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            UpdateBtnEnable(false, false, false);
            UpdateStatusBar(0, 0, 0, "Формирование списка...", "00:00:00");

            //Получаем список книг из БД 
            List<DAL.BookRow> rows = await Task.Factory.StartNew<List<DAL.BookRow>>(() => Worker.GetBooksListFromDB(Mng), TaskCreationOptions.LongRunning);

            //Получаем список файлов содержащихся в архивах и сортируем его 
            List<string> arcFiles = await Task.Factory.StartNew<List<string>>(() => Worker.GetBooksListFromArcs(), TaskCreationOptions.LongRunning);
            arcFiles.Sort();

            UpdateStatusBar(rows.Count, 0, 0, "Обработка...", "00:00:00");

            //Проверяем соответствие файлов в БД и в архивах 
            List<DAL.BookRow> errRows= await Task.Factory.StartNew<List<DAL.BookRow>> (() => Worker.CheckBooks(progress, rows, arcFiles), TaskCreationOptions.None);

            //Выводим список файлов, для которых не удалось найти соответствия
            lvFiles.BeginUpdate();
            foreach (DAL.BookRow row in errRows)
            {
                ListViewItem lvi = new ListViewItem(row.BookName);
                lvi.SubItems.Add(row.ArcFileName);
                lvi.SubItems.Add(row.MD5);
                lvi.Tag = row.Id;
                lvFiles.Items.Add(lvi);
            }
            lvFiles.EndUpdate();

            UpdateStatusBar(rows.Count, rows.Count, errRows.Count, "Завершено...", tsTime.Text);
            UpdateBtnEnable(true, true, true);

        }


        private void UpdateStatusBar(int total, int curr, int err, string status, string time)
        {
            ((IProgress<ProgessRet>)progress).Report(new ProgessRet(total, curr, err, status, time));
            slStatus.Text = status;
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            UpdateBtnEnable(false, false, false);
            UpdateStatusBar(lvFiles.Items.Count, 0, 0, "Удаление...", "00:00:00");
            tsError.Text = lvFiles.Items.Count.ToString();

            int iTotal = lvFiles.Items.Count; 
            for (int i = iTotal - 1; i >= 0; i--)
            {
                int id = Convert.ToInt16(lvFiles.Items[i].Tag);
                bool status = await Task.Factory.StartNew<bool>(() => Worker.DeleteFile(Mng, id), TaskCreationOptions.LongRunning);
                lvFiles.Items[i].Remove();
                UpdateStatusBar(iTotal, iTotal - i, 0, "Удаление...", "00:00:00");
                tsError.Text = lvFiles.Items.Count.ToString();
            }
            UpdateBtnEnable(true, true, true);
        }

        private void UpdateBtnEnable(bool сlose, bool find, bool delete)
        {
            btnClose.Enabled  = сlose;
            btnFind.Enabled   = find;
            btnDelete.Enabled = delete;
        }

        private void VerifyDBForm_Load(object sender, EventArgs e)
        {
            UpdateStatusBar(0, 0, 0, "Ожидание обработки...", "00:00:00");
        }
    }

    class Worker
    {
        public static List<DAL.BookRow> GetBooksListFromDB(BLL.FB2SnitchManager Mng)
        {
            return Mng.GetAllBooks();
        }

        public static List<String> GetBooksListFromArcs()
        {
            return BLL.ZipBLL.GetAllFilesInArc(Properties.Settings.Default.BaseArcDir);
        }

        public static bool DeleteFile(BLL.FB2SnitchManager Mng, int id)
        {
            return Mng.DeleteBookById(id);
        }


        public static List<DAL.BookRow> CheckBooks(IProgress<ProgessRet> progress, List<DAL.BookRow> srcList, List<string> destList)
        {

            TimeSpan ts;
            string elapsedTime;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<DAL.BookRow> errRows = new List<DAL.BookRow>();
            int err = 0;
            for (int i = 0; i < srcList.Count; i++)
            {
                string str = destList.Find(f => f == (srcList[i].MD5 + ".fb2"));
                if (String.IsNullOrEmpty(str))
                {
                    err++;
                    errRows.Add(srcList[i]);
                }

                ts = stopWatch.Elapsed;
                elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);

                progress.Report(new ProgessRet(srcList.Count, i + 1, err, "Обработка...", elapsedTime));
            }

            stopWatch.Stop();
            return errRows;
        }


    }


}
