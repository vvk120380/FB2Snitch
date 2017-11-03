using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FB2Snitch.UIL
{
    public partial class VerifyDBForm : Form
    {

        BLL.FB2SnitchManager Mng;

        public VerifyDBForm()
        {
            InitializeComponent();
        }

        public VerifyDBForm(BLL.FB2SnitchManager mng) : this()
        {
            Mng = mng;
        }

        private async void btnFind_Click(object sender, EventArgs e)
        {

            UpdateBtnEnableStatus(false, false, false);

            UpdateStatusBarValues(0, 0, "Формирование списока...");
            List<DAL.BookRow> rows = await Task.Factory.StartNew<List<DAL.BookRow>>(() => Worker.GetBooksList(Mng), TaskCreationOptions.LongRunning);

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

        public static bool IsFileInArcive(DAL.BookRow row)
        {
            string arc_name = String.Format("{0}\\{1}", Properties.Settings.Default.BaseArcDir, row.ArcFileName);
            string file_name = String.Format("{0}.fb2", row.MD5);
            string tmppath = Properties.Settings.Default.TemDir;

            return BLL.ZipBLL.IsFilePresent(arc_name, file_name, tmppath);
        }

        public static bool DeleteFile(BLL.FB2SnitchManager Mng, int id)
        {
            return Mng.DeleteBookById(id);
        }
    }


}
