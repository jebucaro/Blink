using CommandLine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Blink.Core;

namespace Blink.Client
{
    public partial class MainForm : Form
    {
        private class Options
        {
            [Option('d', "directory", Required = false, HelpText = "Specify working directory.")]
            public string WorkingDirectory { get; set; }

            [Option('a', "action", Required = false, HelpText = "Specify the Action Keyword to select and use a plugin.")]
            public string Action { get; set; }
        }

        private void ParseCommandLineArgs(IList<string> args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    WorkingDirectory = o.WorkingDirectory;
                    Action = o.Action;
                });

            BGWorker.RunWorkerAsync();
        }

        private readonly NotifyIcon _notification;
        private const int NotificationDelay = 4500;
        private string WorkingDirectory { get; set; }
        private string Action { get; set; }

        #region AlwaysMinimizedForm
        const int WmSyscommand = 0x112;
        const int ScMinimize = 0xF020;
        const int ScMaximize = 0xF030;
        const int ScRestore = 0xF120;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmSyscommand)
            {
                if (m.WParam == (IntPtr)ScMaximize || m.WParam == (IntPtr)ScRestore)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
            }

            base.WndProc(ref m);
        }
        #endregion


        public MainForm()
        {
            // use Segoe UI in Vista & 7
            Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            _notification = new NotifyIcon
            {
                Icon = new Icon(Icon, 16, 16),
                BalloonTipIcon = ToolTipIcon.None
            };

            _notification.BalloonTipClosed += Notification_BalloonTipClosed;
            _notification.BalloonTipClicked += Notification_BalloonTipClicked;
        }


        private void Notification_BalloonTipClicked(object sender, EventArgs e)
        {
            _notification?.Dispose();

            Close();
        }

        private void Notification_BalloonTipClosed(object sender, EventArgs e)
        {
            _notification?.Dispose();

            Close();
        }

        private void BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            PluginManager pluginManager = new PluginManager();

            if (string.IsNullOrWhiteSpace(WorkingDirectory))
                throw new ArgumentNullException(nameof(WorkingDirectory));

            if (string.IsNullOrWhiteSpace(Action))
                throw new ArgumentNullException(nameof(Action));

            pluginManager.LoadPlugins();
            pluginManager.InitializePlugin(PluginManager.PluginSearchType.SearchByActionKeyword, Action);
            pluginManager.ExecutePlugin(PluginManager.PluginSearchType.SearchByActionKeyword, Action, WorkingDirectory);
        }

        private void BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _notification.Visible = true;

            if (e.Error is null)
            {
                _notification.ShowBalloonTip(NotificationDelay, "Blink", "Operation completed successfully", ToolTipIcon.Info);
            }
            else
            {
                _notification.ShowBalloonTip(NotificationDelay, e.Error.Source, e.Error.Message, ToolTipIcon.Error);
            }

            Thread.Sleep(NotificationDelay);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ParseCommandLineArgs(Environment.GetCommandLineArgs());
        }
    }
}
