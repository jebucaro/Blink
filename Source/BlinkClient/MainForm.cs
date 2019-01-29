﻿/*
 * MIT License
 * 
 * Copyright (c) 2018 Jonathan Búcaro
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BlinkLib;

namespace BlinkClient
{
    public partial class MainForm : Form
    {
        #region Declarations

        private const string CommandStructure = "-structure";
        private const string CommandCleanse = "-cleanse";
        private const string CommandSpreadsheet = "-spreadsheet";

        private const int TaskCompleted = 100;

        private const int NotificationDelay = 4500;

        private BlinkLib.Blink _blinkStrategy;
        
        private readonly NotifyIcon _notification;

        private string[] _args;

        #endregion

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
            //this.Visible = false;

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

        private void MainForm_Load(object sender, EventArgs e)
        {
            _args = Environment.GetCommandLineArgs();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (_args.Count() == 1)
                WindowState = FormWindowState.Normal;
            else
                bgWorker.RunWorkerAsync();
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            bgWorker.ReportProgress(0);

            if (_args.Count() != 3)
                throw new ArgumentException("Invalid arguments used to run Blink");

            bgWorker.ReportProgress(25);

            DirectoryInfo _workingDirectory = new DirectoryInfo(_args[2]);

            bgWorker.ReportProgress(50);

            switch (_args[1].ToLower())
            {
                case CommandSpreadsheet:
                    _blinkStrategy = new GenerateSpreadsheet();
                    break;
                case CommandStructure:
                    _blinkStrategy = new CreateStructure();
                    break;
                case CommandCleanse:
                    _blinkStrategy = new CleanseStructure();
                    break;
                default:
                    throw new InvalidOperationException(_args[1]);
            }

            bgWorker.ReportProgress(75);

            _blinkStrategy.WorkingDirectory = _workingDirectory;
            _blinkStrategy.Execute();

            bgWorker.ReportProgress(TaskCompleted);
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            _notification.Visible = true;

            if (e.Error is null)
            {
                _notification.ShowBalloonTip(NotificationDelay, "Blink", "Operation completed successfully", ToolTipIcon.Info);
                TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);
            }
            else
            {
                _notification.ShowBalloonTip(NotificationDelay, e.Error.Source, e.Error.Message , ToolTipIcon.Error);
                TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Error);
            }
                
            Thread.Sleep(NotificationDelay);
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TaskbarProgress.SetValue(Handle, e.ProgressPercentage, TaskCompleted);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.iconfinder.com/recepkutuk");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.iconfinder.com/igorverizub");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.iconfinder.com/glyphlab");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.iconfinder.com/recepkutuk");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://jonathanbucaro.com/2018/03/20/blink/");
        }
    }
}