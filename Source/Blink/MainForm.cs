using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Blink.Core;
using CommandLine;

namespace Blink
{
    public partial class MainForm : Form
    {
        private class Options
        {
            [Option('d', "directory", Required = false, HelpText = "Specify working directory.")]
            public string WorkingDirectory { get; set; }

            [Option('a', "action", Required = false, HelpText = "Specify the Action Keyword for the plugin.")]
            public string Action { get; set; }
        }

        private void ParseCommandLineArgs(IList<string> args)
        {
            if (args == null)
                return;

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (o.WorkingDirectory != null)
                        txtWorkingDirectory.Text = o.WorkingDirectory;

                    if (o.Action != null)
                    {
                        try
                        {
                            nlvPlugins.Items[o.Action].Selected = true;
                            nlvPlugins.Items[o.Action].EnsureVisible();
                            btnStart.Focus();
                        }
                        catch (Exception)
                        {
                            // ¯\_(ツ)_/¯
                        }
                    }
                });
        }

        PluginManager PluginManager = new PluginManager();

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
        }

        private void ListAllPlugins()
        {
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(48, 48);

            nlvPlugins.BeginUpdate();

            nlvPlugins.LargeImageList = imageList;
            nlvPlugins.Columns.Add("Name");
            nlvPlugins.Columns.Add("Action Keyword");

            foreach (var item in PluginManager.AvailablePlugins)
            {
                imageList.Images.Add(item.Id, Image.FromFile(item.PluginIconPath));

                var lvItem = nlvPlugins.Items.Add(item.ActionKeyword, item.Name, item.Id);

                lvItem.Tag = item.Id;
                lvItem.SubItems.Add(item.ActionKeyword);

                lvItem.ToolTipText = item.Description;
            }

            for (int i = 0; i < nlvPlugins.Columns.Count; i++)
            {
                nlvPlugins.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            
            nlvPlugins.EndUpdate();
            
        }

        private void nlvPlugins_ClientSizeChanged(object sender, EventArgs e)
        {
            var height = 64 ;
            nlvPlugins.TileSize = new Size(nlvPlugins.ClientSize.Width, height);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (nlvPlugins.SelectedItems.Count > 0)
            {
                var pluginId = nlvPlugins.SelectedItems[0].Tag.ToString();

                this.WindowState = FormWindowState.Minimized;

                PluginManager.ExecutePlugin(
                    PluginManager.PluginSearchType.SearchById,
                    pluginId,
                    txtWorkingDirectory.Text);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            txtWorkingDirectory.Text = folderBrowserDialog1.SelectedPath;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ListAllPlugins();

            ParseCommandLineArgs(Environment.GetCommandLineArgs());
        }
    }
}
