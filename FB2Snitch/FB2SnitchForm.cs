﻿using System;
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

        enum TVLEVELS { GenreRoot = 0, Genre = 1, Author = 2 };
              

        public FB2SnitchForm()
        {
            InitializeComponent();
        }

        private void FB2SnitchForm_Load(object sender, EventArgs e)
        {
            Mng = new FB2SnitchManager();
            changeSettingsAndUpdateControls();
        }

        private void LoadTreeViewData()
        {
            tvMain.Nodes.Clear();
            List<GenreRow> rootGeners = Mng.GetGenresInRoot();
            foreach (GenreRow gr in rootGeners)
            {
                TreeNode tn = new TreeNode(gr.Genre_ru);
                tn.Tag = gr.Id;

                List<GenreRow> geners = Mng.GetGenresByRootId(gr.Id);
                foreach (GenreRow gi in geners)
                {
                    TreeNode tni = new TreeNode(gi.Genre_ru);
                    tni.Tag = gi.Id;
                    tni.Nodes.Add("@@dummnynode@@");
                    tn.Nodes.Add(tni);
                }
                tvMain.Nodes.Add(tn);
            }
        }

        private void UpdateStatusBar()
        {
            slBookCount.Text = Convert.ToString(Mng.GetBookCount());
            slAuthorCount.Text = Convert.ToString(Mng.GetAuthorCount());
        }

        private void tvMain_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Level == (int)TVLEVELS.GenreRoot)
            {

            }
            else
            if (e.Node.Level == (int)TVLEVELS.Genre)
            {
                if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "@@dummnynode@@")
                {
                    List<AuthorRow> authers = Mng.GetAuthorByGenreId((int)e.Node.Tag);
                    if (authers.Count == 0)
                    {
                        e.Cancel = true;
                        return;
                    }

                    e.Node.Nodes.Clear();
                    foreach (AuthorRow author in authers)
                    {
                        TreeNode tni = new TreeNode(author.ToString());
                        tni.Tag = author.Id;
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
                    if (books.Count == 0)
                    {
                        e.Cancel = true;
                        return;
                    }

                    e.Node.Nodes.Clear();
                    foreach (BookRow book in books)
                    {
                        TreeNode tni = new TreeNode(book.BookName);
                        tni.Tag = book.Id;
                        e.Node.Nodes.Add(tni);
                    }
                }
            }

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
    }
}
