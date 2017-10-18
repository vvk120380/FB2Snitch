using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FB2Snitch.DAL;
using FB2Snitch.BLL;

namespace FB2Snitch
{
    public partial class FB2SnitchForm : Form
    {
        BLL.FB2SnitchManager Mng = null;

        enum TVLEVELS { GenreRoot = 0, Genre = 1, Author = 2, Book = 3 };
              

        public FB2SnitchForm()
        {
            InitializeComponent();
        }

        private void FB2SnitchForm_Load(object sender, EventArgs e)
        {
            Mng = new FB2SnitchManager();
            changeSettingsAndUpdateControls();
            splitContainer1.Panel1MinSize = 300;
            splitContainer1.Panel2MinSize = 400;


            ilMain.Images.Add(Properties.Resources.genre_root);
            ilMain.Images.Add(Properties.Resources.genre);
            ilMain.Images.Add(Properties.Resources.personal);
            ilMain.Images.Add(Properties.Resources.book);
            tvMain.ImageList = ilMain;

        }

        private void LoadTreeViewData()
        {
            tvMain.Nodes.Clear();
            List<GenreRow> rootGeners = Mng.GetGenresInRoot();
            foreach (GenreRow gr in rootGeners)
            {
                TreeNode tn = new TreeNode(gr.Genre_ru);
                tn.Tag = gr.Id;
                tn.ImageIndex = 0;
                tn.SelectedImageIndex = 0;
                tn.Nodes.Add("@@dummnynode@@");
                tvMain.Nodes.Add(tn);
            }
        }

        private void UpdateStatusBar()
        {
            slBookCount.Text = Convert.ToString(Mng.GetBookCount());
            slAuthorCount.Text = Convert.ToString(Mng.GetAuthorCount());
        }

        private void LoadLanguages()
        {
            var langs = Mng.GetLanguages().OrderByDescending(x => x.Item1).ToList<Tuple<int, String>>();
            tsLangCB.Items.Clear();

            foreach (Tuple<int, String> lang in langs)
                tsLangCB.Items.Add(String.Format("{1}", lang.Item1, lang.Item2));
            tsLangCB.SelectedIndex = 0;
        }

        private void tvMain_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Level == (int)TVLEVELS.GenreRoot)
            {
                if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "@@dummnynode@@")
                {
                    List<GenreRow> geners = Mng.GetGenresByRootId((int)e.Node.Tag);
                    if (geners.Count == 0) { e.Cancel = true; return; }
                    e.Node.Nodes.Clear();

                    foreach (GenreRow gi in geners)
                    {
                        // Добавлять жанр только, если есть авторы с соответствующим языком
                        int count = Mng.GetAuthorCountByGenreId(gi.Id, tsLangCB.Text);
                        if (count == 0) continue;

                        TreeNode tni = new TreeNode(String.Format("{0} ({1})", String.IsNullOrEmpty(gi.Genre_ru) ? gi.Genre : gi.Genre_ru, count));
                        tni.Tag = gi.Id;
                        tni.ImageIndex = 1;
                        tni.SelectedImageIndex = 1;
                        tni.Nodes.Add("@@dummnynode@@");
                        e.Node.Nodes.Add(tni);
                    }
                }
            }
            else
            if (e.Node.Level == (int)TVLEVELS.Genre)
            {
                if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "@@dummnynode@@")
                {
                    List<AuthorRow> authers = Mng.GetAuthorByGenreId((int)e.Node.Tag, tsLangCB.Text);
                    if (authers.Count == 0) { e.Cancel = true; return; }
                    e.Node.Nodes.Clear();

                    foreach (AuthorRow author in authers)
                    {
                        TreeNode tni = new TreeNode(author.ToString());
                        tni.Tag = author.Id;
                        tni.ImageIndex = 2;
                        tni.SelectedImageIndex = 2;
                        tni.Nodes.Add("@@dummnynode@@");
                        e.Node.Nodes.Add(tni);
                    }
                }
            }
            else
            if (e.Node.Level == (int)TVLEVELS.Author)
            {
                if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "@@dummnynode@@")
                {
                    List<BookRow> books = Mng.GetBookByAuthorId((int)e.Node.Tag);
                    if (books.Count == 0) { e.Cancel = true; return; }
                    e.Node.Nodes.Clear();

                    foreach (BookRow book in books)
                    {
                        TreeNode tni = new TreeNode(book.BookName);
                        tni.Tag = book.Id;
                        tni.ImageIndex = 3;
                        tni.SelectedImageIndex = 3;
                        e.Node.Nodes.Add(tni);
                    }
                }
            }
            //else
            //if (e.Node.Level == (int)TVLEVELS.Book)
            //{

            //}

        }

        private void menuitemImport_Click(object sender, EventArgs e)
        {
            ImportForm importForm = new ImportForm(Mng);
            importForm.ShowDialog();
        }


        private void changeSettingsAndUpdateControls()
        {
            bool checkSuccess = false;
            while (!checkSuccess)
                if (!FileUtils.isFileExist(Properties.Settings.Default.DBPath) ||
                    !Mng.CheckConnection() ||
                    !FileUtils.isFolderExists(Properties.Settings.Default.BaseArcDir))
                {
                    if (MessageBox.Show("При подключении в БД возникла ошибка.\nНеобходимо указать верные настройки.", "Ошибка", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                    {
                        Close();
                        return;
                    }
                    else
                    {
                        SettingsForm settingsForm = new SettingsForm();
                        settingsForm.ShowDialog();
                    }

                }
                else checkSuccess = true;
            Properties.Settings.Default.Save();
            LoadLanguages();
            LoadTreeViewData();
            UpdateStatusBar();
        }

        private void menuitemProperties_Click(object sender, EventArgs e)
        {

            SettingsForm settingsForm = new SettingsForm();
            DialogResult res = settingsForm.ShowDialog();
            if (res == DialogResult.Cancel) return;

            changeSettingsAndUpdateControls();
        }

        private void tvMain_AfterCollapse(object sender, TreeViewEventArgs e)
        {
        }

        private void tvMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == (int)TVLEVELS.Book)
            {
                BookRow row = Mng.GetBookById((int)e.Node.Tag);
                if (row == null) return;

                string arc_name = String.Format("{0}\\{1}", Properties.Settings.Default.BaseArcDir, row.ArcFileName);
                string file_name = String.Format("{0}.fb2", row.MD5);
                string tmppath = Properties.Settings.Default.TemDir;
                try
                {
                    ZipBLL.ExtractFile(arc_name, file_name, tmppath);
                    BLL.FB2Description fb2Desc = BLL.FB2Manager.ReadDecription(String.Format("{0}\\{1}", tmppath, file_name));
                    BLL.FB2Binary fb2Bin = BLL.FB2Manager.ReadBinary(String.Format("{0}\\{1}", tmppath, file_name));
                }
                catch (FB2ZipException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
        }
    }
}
