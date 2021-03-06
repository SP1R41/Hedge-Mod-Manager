using SS16;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HedgeModManager
{
    public partial class ChangeLogForm : Form
    {
        public string downloadUrl;

        public ChangeLogForm(string version, string changeLog, string downloadUrl, string product)
        {
            InitializeComponent();
            Text = $"A new version of {product} is available.";
            titleLbl.Text = $"{product} v" + version;
            // Centring the title
            titleLbl.Location = new Point(Size.Width / 2 - titleLbl.Size.Width / 2, titleLbl.Location.Y);

            // Adds bullets
            changeLog = changeLog.Replace("\n- ", "\n\u2022 ");
            if (changeLog.StartsWith("- "))
                changeLog = "\u2022" + changeLog.Substring(1);
            if (changeLog.StartsWith(" + "))
                changeLog = "\u2022" + changeLog.Substring(2);

            changelogLbl.Text = changeLog;
            this.downloadUrl = downloadUrl;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChangeLogForm_Load(object sender, EventArgs e)
        {
            Theme.ApplyDarkThemeToAll(this);
        }
    }
}
