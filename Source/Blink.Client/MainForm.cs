﻿using Blink.Core;
using CommandLine;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

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

        private readonly PluginManager pluginManager;
        private readonly TaskbarManager taskbarInstance;
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

            pluginManager = new PluginManager();
            taskbarInstance = TaskbarManager.Instance;

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
            if (string.IsNullOrWhiteSpace(WorkingDirectory))
                throw new ArgumentNullException(nameof(WorkingDirectory));

            if (string.IsNullOrWhiteSpace(Action))
                throw new ArgumentNullException(nameof(Action));

            pluginManager.LoadPlugins();

            var detail = pluginManager.SelectPlugin(Action);

            BGWorker.ReportProgress(0, detail);

            pluginManager.InitializePlugin();
            pluginManager.ExecutePlugin(WorkingDirectory);
        }

        private void BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _notification.Visible = true;
            taskbarInstance.SetProgressValue(100, 100);

            if (e.Error is null)
            {
                taskbarInstance.SetProgressState(TaskbarProgressBarState.Normal);
                _notification.ShowBalloonTip(NotificationDelay, "Blink", "Operation completed successfully", ToolTipIcon.Info);
            }
            else
            {
                taskbarInstance.SetProgressState(TaskbarProgressBarState.Error);
                _notification.ShowBalloonTip(NotificationDelay, e.Error.Source, e.Error.Message, ToolTipIcon.Error);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            taskbarInstance.TabbedThumbnail.SetThumbnailClip(Handle, new Rectangle(0, 0, this.Width, this.Height));
            taskbarInstance.SetProgressState(TaskbarProgressBarState.Indeterminate);
            WindowState = FormWindowState.Minimized;

            ParseCommandLineArgs(Environment.GetCommandLineArgs());
        }

        private void BGWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var detail = (Plugin.PluginDetail)e.UserState;

            if (System.IO.File.Exists(detail.PluginIconPath))
                try
                {
                    taskbarInstance.SetOverlayIcon(new Icon(detail.PluginIconPath), detail.Name);
                }
                catch (Exception)
                {
                    // ¯\_(ツ)_/¯
                }

        }
    }
}
