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
using FB2Snitch;
using FB2Snitch.UIL;

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
                    List<BookRow> books = Mng.GetBookByAuthorId((int)e.Node.Tag, tsLangCB.Text);
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

            LoadLanguages();
            LoadTreeViewData();
            UpdateStatusBar();
        }


        private void changeSettingsAndUpdateControls()
        {
            bool checkSuccess = false;
            while (!checkSuccess)
                if (!FileUtils.isFileExist(Properties.Settings.Default.DBPath) ||
                    !Mng.CheckConnection() ||
                    !FileUtils.isFolderExists(Properties.Settings.Default.BaseArcDir) ||
                    !FileUtils.isFolderExists(Properties.Settings.Default.TemDir))
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
                    bool extrZip = ZipBLL.ExtractFile(arc_name, file_name, tmppath);
                    if (!extrZip)
                    {
                        MessageBox.Show("Книга отсутствует в архиве", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    BLL.FB2Description fb2Desc = BLL.FB2Manager.ReadDecription(String.Format("{0}\\{1}", tmppath, file_name));
                    BLL.FB2Binary fb2Bin = BLL.FB2Manager.ReadBinary(String.Format("{0}\\{1}", tmppath, file_name));

                    //А вот здесь отображать
                    FB2Image fb2image = fb2Bin[fb2Desc.titleinfo.coverpage];
                    if (fb2image != null)
                    {
                        Bitmap bitmap = new Bitmap(ImageLib.Base64ToImage(fb2image.data));
                        pbti_coverpage.Image = bitmap;
                    }
                    else
                        pbti_coverpage.Image = Properties.Resources.emptybook;


                    string authors = "";
                    if (fb2Desc.titleinfo.author.Count > 0)
                    {
                        authors = fb2Desc.titleinfo.author[0].ToString();
                        for (int i = 1; i < fb2Desc.titleinfo.author.Count; i++)
                            authors += (Environment.NewLine + fb2Desc.titleinfo.author[i].ToString());
                    }
                    else authors = "N/A";

                    string description = String.IsNullOrEmpty(fb2Desc.titleinfo.annotation) ? "N/A" : fb2Desc.titleinfo.annotation;

                    panelBookInfo.Tag = new Dictionary<string, string>() {
                                                                            { "Name", fb2Desc.titleinfo.book_title },
                                                                            { "Authors", authors },
                                                                            { "Lang", fb2Desc.titleinfo.lang},
                                                                            { "Desc", description}
                                                                         };
                    panelBookInfo.Refresh();

                }
                catch (FB2ZipException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (FB2BaseException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
        }

        private Rectangle DrawRectData(Graphics gr, string strTitle, string strData, Rectangle rectPrev, Rectangle rectImage)
        {
            int uiOffset = 5;

            Font fontTitels = new Font(panelBookInfo.Font.FontFamily, 10, FontStyle.Bold | FontStyle.Italic);
            Font fontText = new Font(panelBookInfo.Font.FontFamily, 10, FontStyle.Regular);

            // Отрисовываем заголовок ---------------------------------------------------------------------------------------------------
            SizeF sizeDataTitle = gr.MeasureString(strTitle, fontTitels, panelBookInfo.Width - rectImage.Width);
            Rectangle rectDataTitleArea = new Rectangle(rectImage.X + rectImage.Width,
                                                        rectPrev.Y + rectPrev.Height,
                                                        Convert.ToInt32(sizeDataTitle.Width) + uiOffset * 2,
                                                        Convert.ToInt32(sizeDataTitle.Height) + uiOffset * 2);
            gr.DrawString(strTitle, fontTitels, new SolidBrush(Color.MidnightBlue), rectDataTitleArea.X + uiOffset, rectDataTitleArea.Y + uiOffset);
            //---------------------------------------------------------------------------------------------------------------------------

            // Отрисовываем основной текст ---------------------------------------------------------------------------------------------------
            SizeF sizeData = gr.MeasureString(strData, fontText, (panelBookInfo.Width - rectImage.Width - rectDataTitleArea.Width - uiOffset));
            Rectangle rectDataArea = new Rectangle(rectDataTitleArea.X + rectDataTitleArea.Width,
                                                        rectDataTitleArea.Y,
                                                        (panelBookInfo.Width - rectImage.Width - rectDataTitleArea.Width - uiOffset),
                                                        Convert.ToInt32(sizeData.Height) + uiOffset * 2);

            gr.DrawString(strData, fontText, new SolidBrush(Color.Black), new Rectangle(rectDataArea.X, rectDataArea.Y + uiOffset, rectDataArea.Width, rectDataArea.Height));
            //---------------------------------------------------------------------------------------------------------------------------

            //Выравниваем границы областей по высоте -------------------------------------------------------------------------------------
            if (rectDataTitleArea.Height > rectDataArea.Height)
                rectDataArea.Height = rectDataTitleArea.Height;
            else
                rectDataTitleArea.Height = rectDataArea.Height;
            //---------------------------------------------------------------------------------------------------------------------------

            return (rectDataTitleArea);
        }

        private void panelBookInfo_Paint(object sender, PaintEventArgs e)
        {
            if (panelBookInfo.Tag == null) return;
            Dictionary<string, string> bookData = (Dictionary<string, string>)panelBookInfo.Tag;

            UInt16 uiOffset = 5;

            #region Выводим Имя Книги

            // Переменные используемые при выводе текста и закрашенного прямоугольника
            Brush brushBookNameText = new SolidBrush(Color.FromArgb(240, 240, 100));
            Brush brushBookNameBG = new SolidBrush(Color.FromArgb(2, 17, 34));
            Font fontBookName = new Font(panelBookInfo.Font.FontFamily, 10, FontStyle.Bold);

            // Высчитывает область занимаемую текстом
            SizeF sizeBookName = e.Graphics.MeasureString(bookData["Name"], fontBookName, panelBookInfo.Width);
            Rectangle rectBookNameArea = new Rectangle(0, 0, panelBookInfo.Width, Convert.ToInt32(sizeBookName.Height + uiOffset * 2));

            // Выводим закрашенный прямоугольник
            e.Graphics.FillRectangle(brushBookNameBG, 0, 0, rectBookNameArea.Width, rectBookNameArea.Height);
            // Выводим текст (Имя книги) 
            e.Graphics.DrawString(bookData["Name"], fontBookName, brushBookNameText, new RectangleF(0, uiOffset, rectBookNameArea.Width, rectBookNameArea.Height));

            #endregion

            #region Отрисовываем картинку книги

            Rectangle rectImageArea = new Rectangle(0, (int)rectBookNameArea.Height, pbti_coverpage.Width + uiOffset * 2, pbti_coverpage.Height + uiOffset * 2);
            pbti_coverpage.Location = new Point(rectImageArea.X + uiOffset, rectImageArea.Y + uiOffset);

            #endregion

            Rectangle rectAuthors  = DrawRectData(e.Graphics, "Автор(ы):", bookData["Authors"], rectBookNameArea, rectImageArea);
            Rectangle rectLanguage = DrawRectData(e.Graphics, "Язык:",     bookData["Lang"],    rectAuthors,      rectImageArea);
            Rectangle rectDesc     = DrawRectData(e.Graphics, "Описание:", bookData["Desc"],    rectLanguage,     rectImageArea);

        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Вызов диалоги About
            AboutForm AboutDlg = new AboutForm();
            AboutDlg.ShowDialog();
        }

        private void verifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VerifyDBForm VerifyDlg = new VerifyDBForm(Mng);
            VerifyDlg.ShowDialog();

        }
    }
}
