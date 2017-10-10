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
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            tbDBPath.Text = Properties.Settings.Default.DBPath;
            tbConnectionString.Text = Properties.Settings.Default.MSSQLConnectionString;
            tbArcDir.Text = Properties.Settings.Default.BaseArcDir;
        }

        private void btnDBDir_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "SQLite DB files (*.db)|*.db|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() != DialogResult.OK) return;

            tbDBPath.Text = ofd.FileName;

            tbConnectionString.Text = String.Format("Data Source={0};Version=3;", tbDBPath.Text);

        }

        private void btnArcDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;

            if (fbd.ShowDialog() != DialogResult.OK) return;

            tbArcDir.Text = fbd.SelectedPath;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

            if (Properties.Settings.Default.DBPath.CompareTo(tbDBPath.Text) == 0 &&
                Properties.Settings.Default.MSSQLConnectionString.CompareTo(tbConnectionString.Text) == 0 &&
                Properties.Settings.Default.BaseArcDir.CompareTo(tbArcDir.Text) == 0)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                Properties.Settings.Default.DBPath = tbDBPath.Text;
                Properties.Settings.Default.MSSQLConnectionString = tbConnectionString.Text;
                Properties.Settings.Default.BaseArcDir = tbArcDir.Text;
                DialogResult = DialogResult.OK;
            }

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

    }
}
