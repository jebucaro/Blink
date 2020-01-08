using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Blink.Core;

namespace Blink
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            // use Segoe UI in Vista & 7
            Font = SystemFonts.MessageBoxFont;

            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            PluginManager.LoadPlugins();
            PluginManager.InitializePlugins();

            ListAllPlugins();
        }

        private void ListAllPlugins()
        {
            nlvPlugins.BeginUpdate();

            nlvPlugins.Columns.Add("Name");
            nlvPlugins.Columns.Add("Action Keyword");
            nlvPlugins.Columns.Add("Description");
            nlvPlugins.Columns.Add("Author");
            nlvPlugins.Columns.Add("Website");

            foreach (var item in PluginManager.AllPlugins)
            {
                var lvItem = nlvPlugins.Items.Add(item.Detail.Name);

                lvItem.SubItems.Add(item.Detail.ActionKeyword);
                lvItem.SubItems.Add(item.Detail.Description);
                lvItem.SubItems.Add(item.Detail.Author);
                lvItem.SubItems.Add(item.Detail.Website);
                lvItem.ToolTipText = item.Detail.Description;
            }

            for (int i = 0; i < nlvPlugins.Columns.Count; i++)
            {
                nlvPlugins.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            
            nlvPlugins.EndUpdate();
            
        }

        private void nlvPlugins_ClientSizeChanged(object sender, EventArgs e)
        {
            nlvPlugins.TileSize = new Size(nlvPlugins.ClientSize.Width, nlvPlugins.TileSize.Height + 16);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            DirectoryInfo workingDirectory;

            try
            {
                workingDirectory = new DirectoryInfo(txtWorkingDirectory.Text);
            }
            catch (Exception)
            {
                throw;
            }

            if (nlvPlugins.SelectedItems.Count > 0)
            {
                var item = nlvPlugins.SelectedItems[0];

                var pluginDuo = PluginManager.GetPluginForActionKeyword(item.SubItems[1].Text);

                pluginDuo.Plugin.WorkingDirectory = workingDirectory;

                PluginManager.ExecutePlugin(pluginDuo);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            txtWorkingDirectory.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}
