/*
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

using BlinkLib;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Blink
{
    public partial class MainForm : Form
    {
        #region Declarations

        const string COMMAND_STRUCTURE = "-structure";
        const string COMMAND_CLEANSE = "-cleanse";
        const string COMMAND_SPREADSHEET = "-spreadsheet";

        const int TASK_COMPLETED = 100;

        const int NOTIFICATION_DELAY = 4500;

        BlinkCommand blinkCommand;
        WorkingDirectory workingDirectory;
        NotifyIcon notification;

        string[] args;

        #endregion

        #region AlwaysMinimizedForm

        const int WM_SYSCOMMAND = 0x112;

        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_RESTORE = 0xF120;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam == (IntPtr)SC_MAXIMIZE || m.WParam == (IntPtr)SC_RESTORE)
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

            notification = new System.Windows.Forms.NotifyIcon()
            {
                Icon = new Icon(this.Icon, 16, 16),
                BalloonTipIcon = ToolTipIcon.None
            };

            notification.BalloonTipClosed += Notification_BalloonTipClosed;
            notification.BalloonTipClicked += Notification_BalloonTipClicked;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            args = Environment.GetCommandLineArgs();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (args.Count() == 1)
                this.WindowState = FormWindowState.Normal;
            else
                bgWorker.RunWorkerAsync();
        }

        private void Notification_BalloonTipClicked(object sender, EventArgs e)
        {
            if (!(notification is null))
                notification.Dispose();

            this.Close();
        }

        private void Notification_BalloonTipClosed(object sender, EventArgs e)
        {
            if (!(notification is null))
                notification.Dispose();

            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            bgWorker.ReportProgress(0);

            if (args.Count() != 3)
                throw new ArgumentException("Invalid arguments used to run Blink");

            bgWorker.ReportProgress(25);

            workingDirectory = new WorkingDirectory(args[2]);

            bgWorker.ReportProgress(50);

            switch (args[1].ToLower())
            {
                case COMMAND_SPREADSHEET:
                    blinkCommand = new GenerateSpreadsheetCommand(workingDirectory);
                    break;
                case COMMAND_STRUCTURE:
                    blinkCommand = new BuildStructureCommand(workingDirectory);
                    break;
                case COMMAND_CLEANSE:
                    blinkCommand = new CleanseStructureCommand(workingDirectory);
                    break;
                default:
                    throw new InvalidOperationException(args[1]);
            }

            bgWorker.ReportProgress(75);

            blinkCommand.Excecute();

            bgWorker.ReportProgress(TASK_COMPLETED);
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            notification.Visible = true;

            if (e.Error is null)
            {
                notification.ShowBalloonTip(NOTIFICATION_DELAY, "Blink", "Operation completed successfully", ToolTipIcon.Info);
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Normal);
            }
            else
            {
                notification.ShowBalloonTip(NOTIFICATION_DELAY, e.Error.Source, e.Error.Message , ToolTipIcon.Error);
                TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.Error);
            }
                
            System.Threading.Thread.Sleep(NOTIFICATION_DELAY);
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TaskbarProgress.SetValue(this.Handle, e.ProgressPercentage, TASK_COMPLETED);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.iconfinder.com/recepkutuk");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.iconfinder.com/igorverizub");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.iconfinder.com/glyphlab");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.iconfinder.com/recepkutuk");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://jonathanbucaro.com/2018/03/20/blink/");
        }
    }
}
