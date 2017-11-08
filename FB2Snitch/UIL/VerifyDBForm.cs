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

            UpdateBtnEnableStatus(false, false, false);
            UpdateStatusBar(0, 0, 0, "Формирование списка...", "00:00:00");

            UpdateStatusBarValues(0, 0, "Формирование списока...");
            List<DAL.BookRow> rows = await Task.Factory.StartNew<List<DAL.BookRow>>(() => Worker.GetBooksList(Mng), TaskCreationOptions.LongRunning);
            arcFiles.Sort();

            UpdateStatusBarValues(0, rows.Count, "Обработка...");

            int errFb2 = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                bool status = await Task.Factory.StartNew<bool>(() => Worker.IsFileInArcive(rows[i]), TaskCreationOptions.LongRunning);
                UpdateStatusBarValues(i+1, rows.Count, "Обработка...");

                if (!status)
                {
                    ListViewItem lvi = new ListViewItem(rows[i].BookName);
                    lvi.SubItems.Add(rows[i].ArcFileName);
                    lvi.SubItems.Add(rows[i].MD5 + ".fb2");
                    lvi.Tag = rows[i].Id;
                    lvFiles.Items.Add(lvi);
                    lvFiles.EnsureVisible(errFb2++);
                    tsError.Text = errFb2.ToString();
                }
            }

            UpdateBtnEnableStatus(true, true, true);
        }

        private void UpdateStatusBarValues(int curr, int total, string status)
        {
            slStatus.Text = status;
            slProgress.Text = String.Format("({0} из {1})", curr, total);
            tsProgress.Maximum = total;
            tsProgress.Step = 1;
            tsProgress.Value = curr;
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            UpdateBtnEnableStatus(false, false, false);
            UpdateStatusBarValues(0, lvFiles.Items.Count, "Удаление...");
            tsError.Text = lvFiles.Items.Count.ToString();

            int iTotal = lvFiles.Items.Count; 
            for (int i = iTotal - 1; i >= 0; i--)
            {
                int id = Convert.ToInt16(lvFiles.Items[i].Tag);
                bool status = await Task.Factory.StartNew<bool>(() => Worker.DeleteFile(Mng, id), TaskCreationOptions.LongRunning);
                lvFiles.Items[i].Remove();
                UpdateStatusBarValues(iTotal - i, iTotal, "Удаление...");
                tsError.Text = lvFiles.Items.Count.ToString();
            }
            UpdateBtnEnableStatus(true, true, true);
        }

        private void UpdateBtnEnableStatus(bool сlose, bool find, bool delete)
        {
            btnClose.Enabled = сlose;
            btnFind.Enabled = find;
            btnDelete.Enabled = delete;
        }

        private void VerifyDBForm_Load(object sender, EventArgs e)
        {
            UpdateStatusBarValues(0, 0, "Ожидание обработки...");
        }
    }

    class Worker
    {
        public static List<DAL.BookRow> GetBooksList(BLL.FB2SnitchManager Mng)
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
