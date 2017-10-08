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

        public FB2SnitchForm()
        {
            InitializeComponent();
        }

        private void FB2SnitchForm_Load(object sender, EventArgs e)
        {
            Mng = new FB2SnitchManager();
            LoadTreeViewData();
        }

        private void LoadTreeViewData()
        {
            tvMain.Nodes.Clear();
            List<GenreRow> rootGeners = Mng.GetGenresInRoot();
            foreach (GenreRow gr in rootGeners)
            {
                TreeNode tn = new TreeNode(gr.ToString());
                tn.Tag = gr.Id;

                List<GenreRow> geners = Mng.GetGenresByRootId(gr.Id);
                foreach (GenreRow gi in geners)
                {
                    TreeNode tni = new TreeNode(gi.ToString());
                    tni.Tag = gi.Id;
                    tni.Nodes.Add("@@dummnynode@@");
                    tn.Nodes.Add(tni);
                }

                tvMain.Nodes.Add(tn);

            }
        }
    }
}
