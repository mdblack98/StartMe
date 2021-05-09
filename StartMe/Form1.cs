using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;


namespace StartMe
{
    public sealed partial class Form1 : Form
    {
        private static readonly Mutex mut = new Mutex();
        internal static class SafeNativeMethods
        {
            public enum ShowWindowEnum
            {
                Hide = 0,
                ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
                Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
                Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
                Restore = 9, ShowDefault = 10, ForceMinimized = 11
            };

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int GetWindowTextLength(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
                IntPtr lParam);

            public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

            [DllImport("User32.dll")]
            public static extern bool SetForegroundWindow(IntPtr point);

            [DllImport("User32.dll")]
            public static extern bool GetForegroundWindow();

            [DllImport("User32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr FindWindow(string lpClass, String lpWindowName);

            [DllImport("User32.dll")]
            public static extern IntPtr GetParent(IntPtr hWnd);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);
            public delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, string lParam);

            public static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
            {
                var handles = new List<IntPtr>();

                foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                    SafeNativeMethods.EnumThreadWindows(thread.Id,
                        (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

                return handles;
            }

        }

        bool autoStartDone = false;
        String settingsKey = "";
        List<String> settingsKeys = new List<string>();
        readonly String[] processArgs = new string[10];
        readonly Process[] process = new Process[10]; // we use 1-9
        private int selectedTask;
        bool settingsSave = false;
        private bool backedUp = false;
        private readonly bool noAuto = false;

        public HelpForm Help { get; } = new HelpForm();

        public ToolTip ToolTip { get; } = new ToolTip
        {
            ShowAlways = true,
            UseAnimation = true,
            UseFading = true,
            IsBalloon = true
        };

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;

            // disable the window handle info
            labelMainWindowHandle1.Visible = false;
            labelMainWindowHandle2.Visible = false;
            labelMainWindowHandle3.Visible = false;
            labelMainWindowHandle4.Visible = false;
            labelMainWindowHandle5.Visible = false;
            labelMainWindowHandle6.Visible = false;
            labelMainWindowHandle7.Visible = false;
            labelMainWindowHandle8.Visible = false;
            labelMainWindowHandle9.Visible = false;

            pid1.Visible = false;
            pid2.Visible = false;
            pid3.Visible = false;
            pid4.Visible = false;
            pid5.Visible = false;
            pid6.Visible = false;
            pid7.Visible = false;
            pid8.Visible = false;
            pid9.Visible = false;

            // set up our context menu for swapping task info with right-click
            textBoxPath1.ContextMenu = MenuGen(1);
            textBoxPath2.ContextMenu = MenuGen(2);
            textBoxPath3.ContextMenu = MenuGen(3);
            textBoxPath4.ContextMenu = MenuGen(4);
            textBoxPath5.ContextMenu = MenuGen(5);
            textBoxPath6.ContextMenu = MenuGen(6);
            textBoxPath7.ContextMenu = MenuGen(7);
            textBoxPath8.ContextMenu = MenuGen(8);
            textBoxPath9.ContextMenu = MenuGen(9);

            String tip = "Path to executable";
            ToolTip.SetToolTip(label1, tip);
            ToolTip.SetToolTip(textBoxPath1, tip);
            ToolTip.SetToolTip(textBoxPath2, tip);
            ToolTip.SetToolTip(textBoxPath3, tip);
            ToolTip.SetToolTip(textBoxPath4, tip);
            ToolTip.SetToolTip(textBoxPath5, tip);
            ToolTip.SetToolTip(textBoxPath6, tip);
            ToolTip.SetToolTip(textBoxPath7, tip);
            ToolTip.SetToolTip(textBoxPath8, tip);
            ToolTip.SetToolTip(textBoxPath9, tip);

            tip = "File Browser";
            //toolTip.SetToolTip(label2, tip);
            ToolTip.SetToolTip(buttonFile1, tip);
            ToolTip.SetToolTip(buttonFile2, tip);
            ToolTip.SetToolTip(buttonFile3, tip);
            ToolTip.SetToolTip(buttonFile4, tip);
            ToolTip.SetToolTip(buttonFile5, tip);
            ToolTip.SetToolTip(buttonFile6, tip);
            ToolTip.SetToolTip(buttonFile7, tip);
            ToolTip.SetToolTip(buttonFile8, tip);
            ToolTip.SetToolTip(buttonFile9, tip);

            tip = "Parameters for program";
            //toolTip.SetToolTip(label3, tip);
            ToolTip.SetToolTip(textBoxArgs1, tip);
            ToolTip.SetToolTip(textBoxArgs2, tip);
            ToolTip.SetToolTip(textBoxArgs3, tip);
            ToolTip.SetToolTip(textBoxArgs4, tip);
            ToolTip.SetToolTip(textBoxArgs5, tip);
            ToolTip.SetToolTip(textBoxArgs6, tip);
            ToolTip.SetToolTip(textBoxArgs7, tip);
            ToolTip.SetToolTip(textBoxArgs8, tip);
            ToolTip.SetToolTip(textBoxArgs9, tip);

            tip = "Command line paramters";
            //toolTip.SetToolTip(label3, tip);
            ToolTip.SetToolTip(textBoxArgs1, tip);
            ToolTip.SetToolTip(textBoxArgs2, tip);
            ToolTip.SetToolTip(textBoxArgs3, tip);
            ToolTip.SetToolTip(textBoxArgs4, tip);
            ToolTip.SetToolTip(textBoxArgs5, tip);
            ToolTip.SetToolTip(textBoxArgs6, tip);
            ToolTip.SetToolTip(textBoxArgs7, tip);
            ToolTip.SetToolTip(textBoxArgs8, tip);
            ToolTip.SetToolTip(textBoxArgs9, tip);

            tip = "Auto start during 1st run of program";
            //toolTip.SetToolTip(label4, tip);
            ToolTip.SetToolTip(checkBoxAutoStart1, tip);
            ToolTip.SetToolTip(checkBoxAutoStart1, tip);
            ToolTip.SetToolTip(checkBoxAutoStart3, tip);
            ToolTip.SetToolTip(checkBoxAutoStart4, tip);
            ToolTip.SetToolTip(checkBoxAutoStart5, tip);
            ToolTip.SetToolTip(checkBoxAutoStart6, tip);
            ToolTip.SetToolTip(checkBoxAutoStart7, tip);
            ToolTip.SetToolTip(checkBoxAutoStart8, tip);
            ToolTip.SetToolTip(checkBoxAutoStart9, tip);

            tip = "Minimize window at startup";
            ToolTip.SetToolTip(checkBoxMinimize1, tip);
            ToolTip.SetToolTip(checkBoxMinimize2, tip);
            ToolTip.SetToolTip(checkBoxMinimize3, tip);
            ToolTip.SetToolTip(checkBoxMinimize4, tip);
            ToolTip.SetToolTip(checkBoxMinimize5, tip);
            ToolTip.SetToolTip(checkBoxMinimize6, tip);
            ToolTip.SetToolTip(checkBoxMinimize7, tip);
            ToolTip.SetToolTip(checkBoxMinimize8, tip);
            ToolTip.SetToolTip(checkBoxMinimize9, tip);

            tip = "Start with Administrative rights";
            //toolTip.SetToolTip(label, tip);
            ToolTip.SetToolTip(checkBoxAdmin1, tip);
            ToolTip.SetToolTip(checkBoxAdmin2, tip);
            ToolTip.SetToolTip(checkBoxAdmin3, tip);
            ToolTip.SetToolTip(checkBoxAdmin4, tip);
            ToolTip.SetToolTip(checkBoxAdmin5, tip);
            ToolTip.SetToolTip(checkBoxAdmin6, tip);
            ToolTip.SetToolTip(checkBoxAdmin7, tip);
            ToolTip.SetToolTip(checkBoxAdmin8, tip);
            ToolTip.SetToolTip(checkBoxAdmin9, tip);

            tip = "Set priority on startup";
            //toolTip.SetToolTip(label7, tip);
            ToolTip.SetToolTip(comboBoxPriority1, tip);
            ToolTip.SetToolTip(comboBoxPriority2, tip);
            ToolTip.SetToolTip(comboBoxPriority3, tip);
            ToolTip.SetToolTip(comboBoxPriority4, tip);
            ToolTip.SetToolTip(comboBoxPriority5, tip);
            ToolTip.SetToolTip(comboBoxPriority6, tip);
            ToolTip.SetToolTip(comboBoxPriority7, tip);
            ToolTip.SetToolTip(comboBoxPriority8, tip);
            ToolTip.SetToolTip(comboBoxPriority9, tip);

            tip = "Start task now. Shift-click to not start Next";
            ToolTip.SetToolTip(buttonStart1, tip);
            ToolTip.SetToolTip(buttonStart2, tip);
            ToolTip.SetToolTip(buttonStart3, tip);
            ToolTip.SetToolTip(buttonStart4, tip);
            ToolTip.SetToolTip(buttonStart5, tip);
            ToolTip.SetToolTip(buttonStart6, tip);
            ToolTip.SetToolTip(buttonStart7, tip);
            ToolTip.SetToolTip(buttonStart8, tip);
            ToolTip.SetToolTip(buttonStart9, tip);

            tip = "Wait for window handle";
            ToolTip.SetToolTip(checkBoxWinWait1, tip);
            ToolTip.SetToolTip(checkBoxWinWait2, tip);
            ToolTip.SetToolTip(checkBoxWinWait3, tip);
            ToolTip.SetToolTip(checkBoxWinWait4, tip);
            ToolTip.SetToolTip(checkBoxWinWait5, tip);
            ToolTip.SetToolTip(checkBoxWinWait6, tip);
            ToolTip.SetToolTip(checkBoxWinWait7, tip);
            ToolTip.SetToolTip(checkBoxWinWait8, tip);
            ToolTip.SetToolTip(checkBoxWinWait9, tip);

            tip = "Delay after startup";
            ToolTip.SetToolTip(numericUpDownDelay1After, tip);
            ToolTip.SetToolTip(numericUpDownDelay2After, tip);
            ToolTip.SetToolTip(numericUpDownDelay3After, tip);
            ToolTip.SetToolTip(numericUpDownDelay4After, tip);
            ToolTip.SetToolTip(numericUpDownDelay5After, tip);
            ToolTip.SetToolTip(numericUpDownDelay6After, tip);
            ToolTip.SetToolTip(numericUpDownDelay7After, tip);
            ToolTip.SetToolTip(numericUpDownDelay8After, tip);
            ToolTip.SetToolTip(numericUpDownDelay9After, tip);

            tip = "Delay after stop signal";
            ToolTip.SetToolTip(numericUpDownDelayStop1, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop2, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop3, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop4, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop5, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop6, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop7, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop8, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop9, tip);

            tip = "% CPU on task before proceeeding";
            ToolTip.SetToolTip(numericUpDownCPU1, tip);
            ToolTip.SetToolTip(numericUpDownCPU2, tip);
            ToolTip.SetToolTip(numericUpDownCPU3, tip);
            ToolTip.SetToolTip(numericUpDownCPU4, tip);
            ToolTip.SetToolTip(numericUpDownCPU5, tip);
            ToolTip.SetToolTip(numericUpDownCPU6, tip);
            ToolTip.SetToolTip(numericUpDownCPU7, tip);
            ToolTip.SetToolTip(numericUpDownCPU8, tip);
            ToolTip.SetToolTip(numericUpDownCPU9, tip);

            tip = "Window title + Keys to send to window after startup delay -- see Help";
            ToolTip.SetToolTip(textBoxStart1, tip);
            ToolTip.SetToolTip(textBoxStart2, tip);
            ToolTip.SetToolTip(textBoxStart3, tip);
            ToolTip.SetToolTip(textBoxStart4, tip);
            ToolTip.SetToolTip(textBoxStart5, tip);
            ToolTip.SetToolTip(textBoxStart6, tip);
            ToolTip.SetToolTip(textBoxStart7, tip);
            ToolTip.SetToolTip(textBoxStart8, tip);
            ToolTip.SetToolTip(textBoxStart9, tip);

            tip = "Sequence # for startup";
            ToolTip.SetToolTip(textBoxStart1Sequence, tip);
            ToolTip.SetToolTip(textBoxStart2Sequence, tip);
            ToolTip.SetToolTip(textBoxStart3Sequence, tip);
            ToolTip.SetToolTip(textBoxStart4Sequence, tip);
            ToolTip.SetToolTip(textBoxStart5Sequence, tip);
            ToolTip.SetToolTip(textBoxStart6Sequence, tip);
            ToolTip.SetToolTip(textBoxStart7Sequence, tip);
            ToolTip.SetToolTip(textBoxStart8Sequence, tip);
            ToolTip.SetToolTip(textBoxStart9Sequence, tip);

            tip = "Sequence # for stopping, 0 skip StopAll";
            ToolTip.SetToolTip(textBoxStart1Stop, tip);
            ToolTip.SetToolTip(textBoxStart2Stop, tip);
            ToolTip.SetToolTip(textBoxStart3Stop, tip);
            ToolTip.SetToolTip(textBoxStart4Stop, tip);
            ToolTip.SetToolTip(textBoxStart5Stop, tip);
            ToolTip.SetToolTip(textBoxStart6Stop, tip);
            ToolTip.SetToolTip(textBoxStart7Stop, tip);
            ToolTip.SetToolTip(textBoxStart8Stop, tip);
            ToolTip.SetToolTip(textBoxStart9Stop, tip);

            tip = "Stop task, Ctrl-Click to kill task";
            ToolTip.SetToolTip(buttonStop1, tip);
            ToolTip.SetToolTip(buttonStop2, tip);
            ToolTip.SetToolTip(buttonStop3, tip);
            ToolTip.SetToolTip(buttonStop4, tip);
            ToolTip.SetToolTip(buttonStop5, tip);
            ToolTip.SetToolTip(buttonStop6, tip);
            ToolTip.SetToolTip(buttonStop7, tip);
            ToolTip.SetToolTip(buttonStop8, tip);
            ToolTip.SetToolTip(buttonStop9, tip);
            

            tip = "Kill task if normal close fails";
            ToolTip.SetToolTip(checkBoxKill1, tip);
            ToolTip.SetToolTip(checkBoxKill2, tip);
            ToolTip.SetToolTip(checkBoxKill3, tip);
            ToolTip.SetToolTip(checkBoxKill4, tip);
            ToolTip.SetToolTip(checkBoxKill5, tip);
            ToolTip.SetToolTip(checkBoxKill6, tip);
            ToolTip.SetToolTip(checkBoxKill7, tip);
            ToolTip.SetToolTip(checkBoxKill8, tip);
            ToolTip.SetToolTip(checkBoxKill9, tip);

            tip = "Wait time for stop to complete (secs)";
            ToolTip.SetToolTip(numericUpDownDelayStop1, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop2, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop3, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop4, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop5, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop6, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop7, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop8, tip);
            ToolTip.SetToolTip(numericUpDownDelayStop9, tip);

            tip = "Window title + Keys to send to window during closing -- see Help";
            ToolTip.SetToolTip(textBoxStop1, tip);
            ToolTip.SetToolTip(textBoxStop2, tip);
            ToolTip.SetToolTip(textBoxStop3, tip);
            ToolTip.SetToolTip(textBoxStop4, tip);
            ToolTip.SetToolTip(textBoxStop5, tip);
            ToolTip.SetToolTip(textBoxStop6, tip);
            ToolTip.SetToolTip(textBoxStop7, tip);
            ToolTip.SetToolTip(textBoxStop8, tip);
            ToolTip.SetToolTip(textBoxStop9, tip);

            //tip = "Process ID";
            //toolTip.SetToolTip(pid1, tip);
            //toolTip.SetToolTip(pid2, tip);
            //toolTip.SetToolTip(pid3, tip);
            //toolTip.SetToolTip(pid4, tip);
            //toolTip.SetToolTip(pid5, tip);
            //toolTip.SetToolTip(pid6, tip);
            //toolTip.SetToolTip(pid7, tip);
            //toolTip.SetToolTip(pid8, tip);
            //toolTip.SetToolTip(pid9, tip);

            tip = "Click to disable/enable task";
            ToolTip.SetToolTip(labelPath1, tip);
            ToolTip.SetToolTip(labelPath2, tip);
            ToolTip.SetToolTip(labelPath3, tip);
            ToolTip.SetToolTip(labelPath4, tip);
            ToolTip.SetToolTip(labelPath5, tip);
            ToolTip.SetToolTip(labelPath6, tip);
            ToolTip.SetToolTip(labelPath7, tip);
            ToolTip.SetToolTip(labelPath8, tip);
            ToolTip.SetToolTip(labelPath9, tip);

            tip = "Keep task running if if crashes";
            ToolTip.SetToolTip(checkBoxKeepRunning1, tip);
            ToolTip.SetToolTip(checkBoxKeepRunning2, tip);
            ToolTip.SetToolTip(checkBoxKeepRunning3, tip);
            ToolTip.SetToolTip(checkBoxKeepRunning4, tip);
            ToolTip.SetToolTip(checkBoxKeepRunning5, tip);
            ToolTip.SetToolTip(checkBoxKeepRunning6, tip);
            ToolTip.SetToolTip(checkBoxKeepRunning7, tip);
            ToolTip.SetToolTip(checkBoxKeepRunning8, tip);
            ToolTip.SetToolTip(checkBoxKeepRunning9, tip);


            ToolTip.SetToolTip(checkBoxMinimize, "Minimize StartMe window after startup");
            ToolTip.SetToolTip(buttonStartAll, "Start all tasks now\nShift-click to start all configs");
            ToolTip.SetToolTip(buttonStopAll, "Stop all tasks now");
            ToolTip.SetToolTip(checkBoxStopAll, "Stop all tasks when program exits");
            ToolTip.SetToolTip(buttonStartAuto, "Start all tasks with Auto checked\nShift-click to start all configs");
            ToolTip.SetToolTip(checkBoxStartup, "Start all auto tasks at startup");
            String[] args = Environment.GetCommandLineArgs();
            for(int i=1;i<args.Length;++i)
            {
                if (args[i].Equals("-noauto"))
                {
                    noAuto = true;
                }
                else
                {
                    SettingsLoad(args[i]);
                }
            }
            settingsSave = true;

            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                //MessageBox.Show("Task Delayed");
            });
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            mut.WaitOne();
            ToolTip.Dispose();
            if (checkBoxStopAll.Checked)
            {
                StopAll();
            }
            Application.Exit();
            settingsSave = false;
            mut.ReleaseMutex();
            base.OnFormClosing(e);
            settingsSave = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //    if (elevateMe())
            //        Application.Exit();
            //Properties.Settings.Default.SettingChanging += new SettingChangingEventHandler(MyCustomSettings_SettingChanging);
            //Properties.Settings.Default.SettingsSaving += new SettingsSavingEventHandler(MyCustomSettings_SettingsSaving);
        
            if (settingsKey.Equals(""))
            {
                settingsKeys = SettingsGetKeys();
                settingsKey = Properties.Settings.Default.SettingsKeyCurrent;
                /*
                if (settingsKeys != null)
                {
                    FormConfigurations myform = new FormConfigurations();
                    String lastConfig = Properties.Settings.Default.SettingsKeyCurrent;
                    if (lastConfig.Equals("")) lastConfig = "Default";
                    myform.setCheckBox(settingsKeys, lastConfig);
                    if (settingsKeys.Count > 1)
                    {
                        myform.ShowDialog();
                        settingsKey = myform.getConfig();
                    }
                    myform.Dispose();
                }
                */
                //MessageBox.Show(settingsKeys.First());
            }
            //settingsKey = Properties.Settings.Default.SettingsKeyCurrent;
            SettingsLoad(settingsKey);
            //settingsGetKeys();
            if (settingsKey == "") settingsKey = "Default";
            comboBoxSettingsKey.Enabled = false;
            SettingsLoad(settingsKey);
            comboBoxSettingsKey.SelectedIndex = comboBoxSettingsKey.Items.IndexOf(settingsKey);
            comboBoxSettingsKey.Enabled = true;
            // Window height/width/position restore
            this.Top = Properties.Settings.Default.RestoreBounds.Top;
            this.Left = Properties.Settings.Default.RestoreBounds.Left;
            this.Height = Properties.Settings.Default.RestoreBounds.Height;
            this.Width = Properties.Settings.Default.RestoreBounds.Width;
            if (this.Height <= 50 || this.Width <= 50)
            {
                this.Height = 100;
                this.Width = 500;
            }
            if (this.Top > Screen.PrimaryScreen.WorkingArea.Height)
            {
                this.Top = 0;
            }
            if (this.Left > Screen.PrimaryScreen.WorkingArea.Width)
            {
                this.Left = 0;
            }

            if (checkBoxMinimize.Checked) this.WindowState = FormWindowState.Minimized;
            ProcessInit();
        }



        private bool ProcessIsRunning(int n)
        {
            String path = "";
            String args = "";
            bool enabled = false;
            switch (n)
            {
                case 1: path = textBoxPath1.Text; args = textBoxArgs1.Text; enabled = textBoxPath1.Enabled; break;
                case 2: path = textBoxPath2.Text; args = textBoxArgs2.Text; enabled = textBoxPath2.Enabled; break;
                case 3: path = textBoxPath3.Text; args = textBoxArgs3.Text; enabled = textBoxPath3.Enabled; break;
                case 4: path = textBoxPath4.Text; args = textBoxArgs4.Text; enabled = textBoxPath4.Enabled; break;
                case 5: path = textBoxPath5.Text; args = textBoxArgs5.Text; enabled = textBoxPath5.Enabled; break;
                case 6: path = textBoxPath6.Text; args = textBoxArgs6.Text; enabled = textBoxPath6.Enabled; break;
                case 7: path = textBoxPath7.Text; args = textBoxArgs7.Text; enabled = textBoxPath7.Enabled; break;
                case 8: path = textBoxPath8.Text; args = textBoxArgs8.Text; enabled = textBoxPath8.Enabled; break;
                case 9: path = textBoxPath9.Text; args = textBoxArgs9.Text; enabled = textBoxPath9.Enabled; break;
            }
            return ProcessIsRunning(path, enabled, args, n, ref process[n]);
        }


        // private bool processIsRunning(String path, String args, int n)
        // {
        //     Process ptmp = new Process();
        //     if (File.Exists(path))
        //     {
        //         return processIsRunning(path, args, n, ref ptmp);
        //     }
        //     return false;
        // }

#pragma warning disable IDE0051 // Remove unused private members
        private bool ProcessNameIsUnique(String name)
#pragma warning restore IDE0051 // Remove unused private members
        {
            int n = 0;
            if (name.Equals(textBoxPath1.Text)) ++n;
            if (name.Equals(textBoxPath2.Text)) ++n;
            if (name.Equals(textBoxPath3.Text)) ++n;
            if (name.Equals(textBoxPath4.Text)) ++n;
            if (name.Equals(textBoxPath5.Text)) ++n;
            if (name.Equals(textBoxPath6.Text)) ++n;
            if (name.Equals(textBoxPath7.Text)) ++n;
            if (name.Equals(textBoxPath8.Text)) ++n;
            if (name.Equals(textBoxPath9.Text)) ++n;
            return n == 1;
        }

        private void ProcessSetId(int n, int id)
        {
            switch (n)
            {
                case 1: pid1.Text = id.ToString(); break;
                case 2: pid2.Text = id.ToString(); break;
                case 3: pid3.Text = id.ToString(); break;
                case 4: pid4.Text = id.ToString(); break;
                case 5: pid5.Text = id.ToString(); break;
                case 6: pid6.Text = id.ToString(); break;
                case 7: pid7.Text = id.ToString(); break;
                case 8: pid8.Text = id.ToString(); break;
                case 9: pid9.Text = id.ToString(); break;
            }
        }

#pragma warning disable IDE0051 // Remove unused private members
        private String ProcessWindowTitle(int n)
#pragma warning restore IDE0051 // Remove unused private members
        {
            String fileName = "";
            String args = "";
            switch (n)
            {
                case 1: fileName = textBoxPath1.Text; args = textBoxArgs1.Text; break;
                case 2: fileName = textBoxPath2.Text; args = textBoxArgs2.Text; break;
                case 3: fileName = textBoxPath3.Text; args = textBoxArgs3.Text; break;
                case 4: fileName = textBoxPath4.Text; args = textBoxArgs4.Text; break;
                case 5: fileName = textBoxPath5.Text; args = textBoxArgs5.Text; break;
                case 6: fileName = textBoxPath6.Text; args = textBoxArgs6.Text; break;
                case 7: fileName = textBoxPath7.Text; args = textBoxArgs7.Text; break;
                case 8: fileName = textBoxPath8.Text; args = textBoxArgs8.Text; break;
                case 9: fileName = textBoxPath9.Text; args = textBoxArgs9.Text; break;
            }
            if (fileName.ToLower().Contains("wsjtx"))
            {
                // then we need to add the args to the title for the expected window title
                String match1 = "--rig-name";
                String match2 = "-r ";
                if (args.Contains(match1) || args.Contains(match2))
                {
                    int index = args.IndexOf(match1);
                    fileName = fileName + " " + args.Substring(index + match1.Length);
                }
                else if (args.Contains("-r "))
                {
                    //int index = args.IndexOf(match2);
                    fileName = fileName + " " + args.Split(']').First();
                }
            }
            return fileName;
        }

        private void SetPathColor(int n, Color c)
        {
            switch (n)
            {
                case 1: textBoxPath1.ForeColor = c; break;
                case 2: textBoxPath2.ForeColor = c; break;
                case 3: textBoxPath3.ForeColor = c; break;
                case 4: textBoxPath4.ForeColor = c; break;
                case 5: textBoxPath5.ForeColor = c; break;
                case 6: textBoxPath6.ForeColor = c; break;
                case 7: textBoxPath7.ForeColor = c; break;
                case 8: textBoxPath8.ForeColor = c; break;
                case 9: textBoxPath9.ForeColor = c; break;
            }
        }

        /*
        private String GetWindowTitle(Process p)
        {
            String title = "";
            if (p == null) return null;
            string windowTitle = p.MainWindowTitle;
            if (windowTitle.Contains("JTAlert"))
            {
                char[] split = { ',' };
                string[] tokens = windowTitle.Split(split);
                title = "JTALert" + tokens[4].Substring(0, 2);
            }
            else if (windowTitle.Contains("WSJT-X"))
            {
                if (windowTitle.Substring(0, 9).Equals("WSJT-X  v")) {
                    title = "WSJT-X Default";
                }
            }
            return title;
        }
        */

        private bool ProcessFindName(String name, String args, int n, ref Label processId)
        {
            if (name.Length == 0)
            {
                SetStartStop(n, false, false);
                return false;
            }

            if (!File.Exists(name))
            {
               //MessageBox.Show("Tasks #" + n + " file does not exist\n" + name, "Error StartMe");
                SetPathColor(n, Color.Red);
                processId.Text = "0";
                SetStartStop(n, false, false);
                return false;
            }
            SetPathColor(n, Color.Black);
            SetStartStop(n, true, true);
            int pid = 0;
            try
            {
                pid = Convert.ToInt32(processId.Text);
            }
            catch (Exception)
            {

            }
            if (pid != 0)
            {
                try
                {
                    Process myprocess = Process.GetProcessById(pid);
                    myprocess.StartInfo.Arguments = CommandLineUtilities.GetCommandLines(myprocess);
                    String cmdLine = "\"" + name + "\" " + args;
                    if (name.Equals(myprocess.MainModule.FileName))
                    {
                        SetStartStop(n, false, true);
                        process[n] = myprocess;
                        return true;
                    }
                    else
                    {
                        process[n] = null; ;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(name + "\n" + ex.Message + "\n" + ex.StackTrace, "Error StartMe");
                    processId.Text = "0";
                    SetStartStop(n, true, false);
                }
            }
            Process p2 = GetProcessByFileName(n);
            if (p2 == null)
            {
                //process[n] = null;
                SetStartStop(n, true, false);
                return false;
            }
            process[n] = p2;
            processId.Text = process[n].Id.ToString();
            ProcessSetId(n, process[n].Id);
            SetStartStop(n, false, true);
            return true;
        }

        private String GetPath(int n)
        {
            String path = "";
            switch (n)
            {
                case 1: path = textBoxPath1.Text; break;
                case 2: path = textBoxPath2.Text; break;
                case 3: path = textBoxPath3.Text; break;
                case 4: path = textBoxPath4.Text; break;
                case 5: path = textBoxPath5.Text; break;
                case 6: path = textBoxPath6.Text; break;
                case 7: path = textBoxPath7.Text; break;
                case 8: path = textBoxPath8.Text; break;
                case 9: path = textBoxPath9.Text; break;
            }
            return path;
        }

        private String GetArgs(int n)
        {
            String args = "";
            switch (n)
            {
                case 1: args = textBoxArgs1.Text; break;
                case 2: args = textBoxArgs2.Text; break;
                case 3: args = textBoxArgs3.Text; break;
                case 4: args = textBoxArgs4.Text; break;
                case 5: args = textBoxArgs5.Text; break;
                case 6: args = textBoxArgs6.Text; break;
                case 7: args = textBoxArgs7.Text; break;
                case 8: args = textBoxArgs8.Text; break;
                case 9: args = textBoxArgs9.Text; break;
            }
            return args;
        }

        private Process GetProcessByFileName(int n)
        {
            int i = -1;
            String fileName = GetPath(n);
            String fileArgs = GetArgs(n);
            String processArgs = "";
            int index = fileName.LastIndexOf('\\');
            bool adminNeeded = false;
            String exeName = fileName.Substring(index + 1).Replace(".exe", "");
            Process[] pName = Process.GetProcessesByName(exeName);
            foreach (Process p in pName)
            {
                if (p != null)
                {
                    processArgs = CommandLineUtilities.GetCommandLines(p);
                }

                ++i;
                try
                {
                    //if (p.HasExited) return null;
                    if (fileName != null && p != null && p.MainModule != null)
                    {
                        String compare1 = "\"" + p.MainModule.FileName + "\" ";
                        String s2 = compare1 + fileArgs;
                        if (fileName.Equals(p.MainModule.FileName) && processArgs.Equals(s2))
                            break; // find the first one
                    }
                    else return null;
                }
                catch (Win32Exception)
                {
                    //labelStatusMessage.Text = "Need admin rights??";
                    Application.DoEvents();
                    //MessageBox.Show("Restart with admin rights" + "\n" + fileName + "\n" + ex.Message+"\n"+ex.StackTrace, "Error StartMe");
                    //adminNeeded = true;
                }
            }
            if (!adminNeeded)
            {
                try
                {
                    String compare1 = "\"" + fileName + "\" " + fileArgs;
                    if (i >= 0 && fileName.Equals(pName[i].MainModule.FileName) && compare1.Equals(processArgs))
                    {
                        return pName[i];
                    }
                }
                catch (Exception)
                {

                }
            }

            return null;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private Process GetProcessById(int pid)
#pragma warning restore IDE0051 // Remove unused private members
        {
            Process pName = Process.GetProcessById(pid);
            if (pName != null) return pName;
            return null;

        }
#pragma warning disable IDE0051 // Remove unused private members
        private Process GetProcessByFileName(String fileName)
#pragma warning restore IDE0051 // Remove unused private members
        {
            int index = fileName.LastIndexOf('\\');
            String exeName = fileName.Substring(index + 1).Replace(".exe", "");
            Process[] pName = Process.GetProcessesByName(exeName);
            //if (pName.Count() != 1) return null; // more than one so can't tell which
            return pName[0];
        }
#pragma warning disable IDE0051 // Remove unused private members
        private Process[] GetProcessesByFileName(String fileName)
#pragma warning restore IDE0051 // Remove unused private members
        {
            int index = fileName.LastIndexOf('\\');
            String exeName = fileName.Substring(index + 1).Replace(".exe", "");
            Process[] pName = Process.GetProcessesByName(exeName);
            return pName;
        }
        void SetEZName(int n, string newName = null)
        {
            TextBox lbox = textBoxEZName1;
            string s = "!!Bug!!";
            switch (n)
            {
                case 1: s = textBoxPath1.Text; lbox = textBoxEZName1; break;
                case 2: s = textBoxPath2.Text; lbox = textBoxEZName2; break;
                case 3: s = textBoxPath3.Text; lbox = textBoxEZName3; break;
                case 4: s = textBoxPath4.Text; lbox = textBoxEZName4; break;
                case 5: s = textBoxPath5.Text; lbox = textBoxEZName5; break;
                case 6: s = textBoxPath6.Text; lbox = textBoxEZName6; break;
                case 7: s = textBoxPath7.Text; lbox = textBoxEZName7; break;
                case 8: s = textBoxPath8.Text; lbox = textBoxEZName8; break;
                case 9: s = textBoxPath9.Text; lbox = textBoxEZName9; break;
            }
            if (newName != null) // then we're forcing the name
            {
                s = newName;
            }
            else if (newName == "")
            {
                s = "";
            }
            else
            {
                if (s != "" && File.Exists(s))
                {
                    s = System.IO.Path.GetFileNameWithoutExtension(s);
                }
                else if (s != "")
                {
                    MessageBox.Show("Check path for task #" + n + "\nFile '" + s + "not found", "StartMe Error");
                }
            }
            lbox.Text = s;
        }

        private void ProcessInit()
        {
            process[0] = null;
            process[1] = null;
            process[2] = null;
            process[3] = null;
            process[4] = null;
            process[5] = null;
            process[6] = null;
            process[7] = null;
            process[8] = null;
            settingsSave = false;
            ProcessFindName(textBoxPath1.Text, textBoxArgs1.Text, 1, ref pid1);
            ProcessFindName(textBoxPath2.Text, textBoxArgs2.Text, 2, ref pid2);
            ProcessFindName(textBoxPath3.Text, textBoxArgs3.Text, 3, ref pid3);
            ProcessFindName(textBoxPath4.Text, textBoxArgs4.Text, 4, ref pid4);
            ProcessFindName(textBoxPath5.Text, textBoxArgs5.Text, 5, ref pid5);
            ProcessFindName(textBoxPath6.Text, textBoxArgs6.Text, 6, ref pid6);
            ProcessFindName(textBoxPath7.Text, textBoxArgs7.Text, 7, ref pid7);
            ProcessFindName(textBoxPath8.Text, textBoxArgs8.Text, 8, ref pid8);
            ProcessFindName(textBoxPath9.Text, textBoxArgs9.Text, 9, ref pid9);
            settingsSave = true;
            return;

        }

        private bool ProcessIsRunning(string path, bool enabled, string args, int n, ref Process myProcess)
        {
            // we will use our current pid
            if (path.Length == 0)
            {
                SetStartStop(n, false, false);
                if (enabled) SetEZName(n, "");
                ProcessSetMainWindowHandle(n, (IntPtr)0);
                return false;
            }
            if (!File.Exists(path))
            {
                SetPathColor(n, Color.Red);
                SetStartStop(n, false, false);
                SetEZName(n, "");
                ProcessSetMainWindowHandle(n, (IntPtr)0);
                return false;
            }
            SetPathColor(n, Color.Black);
            try
            {
                Process p1 = GetProcessByFileName(n);

                if (p1 != null && process[n] != null && process[n].Id > 0)
                {
                    int pid = 0;
                    switch (n)
                    {
                        case 1: pid = Convert.ToInt32(pid1.Text); break;
                        case 2: pid = Convert.ToInt32(pid2.Text); break;
                        case 3: pid = Convert.ToInt32(pid3.Text); break;
                        case 4: pid = Convert.ToInt32(pid4.Text); break;
                        case 5: pid = Convert.ToInt32(pid5.Text); break;
                        case 6: pid = Convert.ToInt32(pid6.Text); break;
                        case 7: pid = Convert.ToInt32(pid7.Text); break;
                        case 8: pid = Convert.ToInt32(pid8.Text); break;
                        case 9: pid = Convert.ToInt32(pid9.Text); break;
                    }
                    try
                    {
                        bool didExit = process[n].WaitForExit(20);
                        if (didExit)
                        {
                            SetStartStop(n, true, false);
                        }
                        else
                        {
                            SetStartStop(n, false, true);
                            if (ProcessGetMainWindowHandle(n) != (IntPtr)0)
                            {
                                ProcessSetMainWindowHandle(n, ProcessGetWindowHandle(process[n].Id));
                            }
                            return true;
                        }
                    }
                    catch (Exception)
                    {
                        // if we get exception than it's not running anymore
                        SetStartStop(n, enabled, false);
                    }
                }
                else {
                    //process[n] = new Process();
                    SetStartStop(n, enabled, false);
                    return false;
                }
            }
            catch (Exception)
            {
                Process p1 = GetProcessByFileName(n);
                if (p1 != null)
                {
                    process[n] = p1;
                    SetPid(n, p1.Id.ToString());
                    SetStartStop(n, false, enabled);
                    return true;
                }
                else
                {
                    SetStartStop(n, enabled, false);
                }
            }
            return false;
        }

        private IntPtr ProcessGetWindowHandle(int pid)
        {
            // Just return the first window -- does this work all the time?
            foreach (var handle in SafeNativeMethods.EnumerateProcessWindowHandles(pid))
            {
                return handle;
            }
            return (IntPtr)0;
        }

        private void ProcessSendKeys(int n, bool start)
        {
            string windowName;
            string keys;
            string[] tokens = { "" };
            if (start)
            {
                switch (n)
                {
                    case 1: tokens = textBoxStart1.Text.Split('"'); break;
                    case 2: tokens = textBoxStart2.Text.Split('"'); break;
                    case 3: tokens = textBoxStart3.Text.Split('"'); break;
                    case 4: tokens = textBoxStart4.Text.Split('"'); break;
                    case 5: tokens = textBoxStart5.Text.Split('"'); break;
                    case 6: tokens = textBoxStart6.Text.Split('"'); break;
                    case 7: tokens = textBoxStart7.Text.Split('"'); break;
                    case 8: tokens = textBoxStart8.Text.Split('"'); break;
                    case 9: tokens = textBoxStart9.Text.Split('"'); break;
                    default: break;

                }
            }
            else
            {
                switch (n)
                {
                    case 1: tokens = textBoxStop1.Text.Split('"'); break;
                    case 2: tokens = textBoxStop2.Text.Split('"'); break;
                    case 3: tokens = textBoxStop3.Text.Split('"'); break;
                    case 4: tokens = textBoxStop4.Text.Split('"'); break;
                    case 5: tokens = textBoxStop5.Text.Split('"'); break;
                    case 6: tokens = textBoxStop6.Text.Split('"'); break;
                    case 7: tokens = textBoxStop7.Text.Split('"'); break;
                    case 8: tokens = textBoxStop8.Text.Split('"'); break;
                    case 9: tokens = textBoxStop9.Text.Split('"'); break;
                    default: break;
                }

            }
            if (tokens.Length < 2) return; // no keys to send
            if (!ProcessIsRunning(n))
            {
                MessageBox.Show("Send to window for task#" + n + " not executed as process has already stopped", "Info StartMe");
                return;
            }
            windowName = tokens[1];
            keys = tokens[2];
            tokens = keys.Split(' ');
            if (keys.Length == 0) return;
            IntPtr mainWindowHandle = ProcessGetMainWindowHandle(n);
            if (mainWindowHandle != (IntPtr)0) SafeNativeMethods.SetForegroundWindow(mainWindowHandle);
            else SafeNativeMethods.SetForegroundWindow(process[n].MainWindowHandle);
            Application.DoEvents();
            Thread.Sleep(1000);
            var h = FindWindowsWithText(windowName);
            if (h.Count() > 1)
            {
                MessageBox.Show("Window name '" + windowName + "' matches :" + h.Count() + " windows\n", "Debug StartMe");
                return;
            }
            if (h == null && h.Count() == 0 && ProcessIsRunning(n))
            {
                MessageBox.Show("Window name '" + windowName + "' not found", "Error StartMe", MessageBoxButtons.OK);
                return;
            }
            if (h == null || h.Count() == 0) return; // must have stopped
            SafeNativeMethods.SetForegroundWindow(h.First());
            //Thread.Sleep(2000);
            foreach (String s in tokens)
            {
                // Shift = "+"
                // Ctrl = "^"
                // Alt = "%";
                // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys?view=netframework-4.8
                String key;
                switch (s.ToUpper())
                {
                    case "DOWN":
                        key = "{DOWN}";
                        break;
                    case "UP":
                        key = "{UP}";
                        break;
                    case "RIGHT":
                        key = "{RIGHT}";
                        break;
                    case "LEFT":
                        key = "{LEFT}";
                        break;
                    case "ENTER":
                        key = "{ENTER}";
                        break;
                    case "DELAY":
                        Thread.Sleep(500);
                        key = "";
                        break;
                    case "TAB":
                        key = "{TAB}";
                        break;
                    case "ALT":
                        key = "%";
                        break;
                    case "ALT+A":
                        key = "%A";
                        break;
                    case "ALT+F":
                        key = "%F";
                        break;
                    case "ALT+X":
                        key = "%X";
                        break;
                    case "ALT+F4":
                        key = "(%{F4})";
                        break;
                    case "LWIN":
                        key = "{LWIN}";
                        break;
                    case "RWIN":
                        key = "{RWIN}";
                        break;
                    default:
                        key = s;
                        break;
                }
                if (key.Length > 0)
                {
                    SendKeys.SendWait(key);
                    //{  Another way if needed?
                    //    _ = SendMessage(h.First(), 0x000c, (IntPtr)0, key);
                    //}
                    Thread.Sleep(100);
                }
            }
        }

        private IntPtr ProcessGetMainWindowHandle(int n)
        {
            string hexNumber;
            switch (n)
            {
                case 1: hexNumber = labelMainWindowHandle1.Text; break;
                case 2: hexNumber = labelMainWindowHandle2.Text; break;
                case 3: hexNumber = labelMainWindowHandle3.Text; break;
                case 4: hexNumber = labelMainWindowHandle4.Text; break;
                case 5: hexNumber = labelMainWindowHandle5.Text; break;
                case 6: hexNumber = labelMainWindowHandle6.Text; break;
                case 7: hexNumber = labelMainWindowHandle7.Text; break;
                case 8: hexNumber = labelMainWindowHandle8.Text; break;
                case 9: hexNumber = labelMainWindowHandle9.Text; break;
                default:
                    hexNumber = "0";
                    MessageBox.Show("Invalid task# in " + System.Reflection.MethodBase.GetCurrentMethod().Name + "Error StartMe");
                    break;
            }
            IntPtr myHandle;
            try
            {
                myHandle = (IntPtr)int.Parse(hexNumber, System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception)
            {
                myHandle = (IntPtr)0;
            }
            return myHandle;
        }


        // Delegate to filter which windows to include 
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter"> A delegate that returns true for windows
        ///    that should be returned and false for windows that should
        ///    not be returned </param>
        /// <summary> Get the text for the window pointed to by hWnd </summary>
        public static string GetWindowText(IntPtr hWnd)
        {
            int size = SafeNativeMethods.GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                SafeNativeMethods.GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }
        public static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            SafeNativeMethods.EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }
        /// <summary> Find all windows that contain the given title text </summary>
        /// <param name="titleText"> The text that the window title must contain. </param>
        public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
        {
            return FindWindows(delegate (IntPtr wnd, IntPtr param)
            {
                string s = GetWindowText(wnd);
                //bool b = s.StartsWith(titleText);
                //if (b) MessageBox.Show(s,"Debug StartMe");
                return s.StartsWith(titleText);
                //return GetWindowText(wnd).Contains(titleText);
                //return wnd;
            });
        }
        private void SetStartStop(int n, bool start, bool stop)
        {
            switch (n)
            {
                case 1: buttonStart1.Enabled = start; buttonStop1.Enabled = stop; break;
                case 2: buttonStart2.Enabled = start; buttonStop2.Enabled = stop; break;
                case 3: buttonStart3.Enabled = start; buttonStop3.Enabled = stop; break;
                case 4: buttonStart4.Enabled = start; buttonStop4.Enabled = stop; break;
                case 5: buttonStart5.Enabled = start; buttonStop5.Enabled = stop; break;
                case 6: buttonStart6.Enabled = start; buttonStop6.Enabled = stop; break;
                case 7: buttonStart7.Enabled = start; buttonStop7.Enabled = stop; break;
                case 8: buttonStart8.Enabled = start; buttonStop8.Enabled = stop; break;
                case 9: buttonStart9.Enabled = start; buttonStop9.Enabled = stop; break;
                default:
                    MessageBox.Show("Start/Stop button#" + n + " not found", "Error StartMe");
                    break;
            }
            //Application.DoEvents();
        }

        /*
        private void processNext(String next)
        {
            if (next.Length == 0) return;
            String[] tokens = next.Split(' ');
            foreach (String s in tokens)
            {
                try
                {
                    int n = Int32.Parse(s);
                    if (!processIsRunning(n))
                    {
                        // may not want to pass exisiting modifierkeys here -- do we carry them forward?
                        processStart(n, ModifierKeys);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error converting \"" + s + "\" to integer\n" + ex.StackTrace, "Error StartMe");
                    return;
                }
            }
        }
        */

        private void ButtonStart1_Click(object sender, EventArgs e)
        {
            buttonStart1.Enabled = false;
            ProcessStart(1, ModifierKeys);
            buttonStop1.Enabled = true;
        }

        private void ButtonStart2_Click(object sender, EventArgs e)
        {
            buttonStart2.Enabled = false;
            ProcessStart(2, ModifierKeys);
            buttonStop2.Enabled = true;
        }

        private void ButtonStart3_Click(object sender, EventArgs e)
        {
            buttonStart3.Enabled = false;
            ProcessStart(3, ModifierKeys);
            buttonStop3.Enabled = true;
        }

        private void ButtonStart4_Click(object sender, EventArgs e)
        {
            buttonStart4.Enabled = false;
            ProcessStart(4, ModifierKeys);
            buttonStop4.Enabled = true;
        }

        private void ButtonStart5_Click(object sender, EventArgs e)
        {
            buttonStart5.Enabled = false;
            ProcessStart(5, ModifierKeys);
            buttonStop5.Enabled = true;
        }

        private void ButtonStart6_Click(object sender, EventArgs e)
        {
            buttonStart6.Enabled = false;
            ProcessStart(6, ModifierKeys);
            buttonStop6.Enabled = true;
        }

        private void ButtonStart7_Click(object sender, EventArgs e)
        {
            buttonStart7.Enabled = false;
            ProcessStart(7, ModifierKeys);
            buttonStop7.Enabled = true;
        }

        private void ButtonStart8_Click(object sender, EventArgs e)
        {
            buttonStart8.Enabled = false;
            ProcessStart(8, ModifierKeys);
            buttonStop8.Enabled = true;
        }

        private void ButtonStart9_Click(object sender, EventArgs e)
        {
            buttonStart9.Enabled = false;
            ProcessStart(9, ModifierKeys);
            buttonStop9.Enabled = true;
        }

        private bool SendBeforeExists()
        {
            bool exists =
                textBoxStart1.Text.Length > 0
                || textBoxStart2.Text.Length > 0
                || textBoxStart3.Text.Length > 0
                || textBoxStart4.Text.Length > 0
                || textBoxStart5.Text.Length > 0
                || textBoxStart6.Text.Length > 0
                || textBoxStart7.Text.Length > 0
                || textBoxStart8.Text.Length > 0
                || textBoxStart9.Text.Length > 0;
            return exists;
        }

        private void ProcessStart(int n, Keys modifierKeys)
        {
            if (!SendBeforeExists())
            {
                this.Activate();
            }
            timer1.Stop();
            labelStatusMessage.Text = "Starting task#" + n;
            Application.DoEvents();
            bool ourCursor = false;
            if (Application.UseWaitCursor == false)
            {
                Application.UseWaitCursor = true;
                ourCursor = true;
            }
            //Application.DoEvents();
            String processName = "";
            String args = "";
            bool winWait = false;
            decimal sleepAfter = 0;
            decimal cpu = 0;
            bool minimize = false;
            int priority = 0;
            bool enabled = false;
            switch (n)
            {
                case 1: processName = textBoxPath1.Text; winWait = checkBoxWinWait1.Checked; sleepAfter = numericUpDownDelay1After.Value; cpu = numericUpDownCPU1.Value; args = processArgs[n] = textBoxArgs1.Text; minimize = checkBoxMinimize1.Checked; priority = comboBoxPriority1.SelectedIndex; enabled = textBoxPath1.Enabled; break;
                case 2: processName = textBoxPath2.Text; winWait = checkBoxWinWait2.Checked; sleepAfter = numericUpDownDelay2After.Value; cpu = numericUpDownCPU2.Value; args = processArgs[n] = textBoxArgs2.Text; minimize = checkBoxMinimize2.Checked; priority = comboBoxPriority2.SelectedIndex; enabled = textBoxPath2.Enabled; break;
                case 3: processName = textBoxPath3.Text; winWait = checkBoxWinWait3.Checked; sleepAfter = numericUpDownDelay3After.Value; cpu = numericUpDownCPU3.Value; args = processArgs[n] = textBoxArgs3.Text; minimize = checkBoxMinimize3.Checked; priority = comboBoxPriority3.SelectedIndex; enabled = textBoxPath3.Enabled; break;
                case 4: processName = textBoxPath4.Text; winWait = checkBoxWinWait4.Checked; sleepAfter = numericUpDownDelay4After.Value; cpu = numericUpDownCPU4.Value; args = processArgs[n] = textBoxArgs4.Text; minimize = checkBoxMinimize4.Checked; priority = comboBoxPriority4.SelectedIndex; enabled = textBoxPath4.Enabled; break;
                case 5: processName = textBoxPath5.Text; winWait = checkBoxWinWait5.Checked; sleepAfter = numericUpDownDelay5After.Value; cpu = numericUpDownCPU5.Value; args = processArgs[n] = textBoxArgs5.Text; minimize = checkBoxMinimize5.Checked; priority = comboBoxPriority5.SelectedIndex; enabled = textBoxPath5.Enabled; break;
                case 6: processName = textBoxPath6.Text; winWait = checkBoxWinWait6.Checked; sleepAfter = numericUpDownDelay6After.Value; cpu = numericUpDownCPU6.Value; args = processArgs[n] = textBoxArgs6.Text; minimize = checkBoxMinimize6.Checked; priority = comboBoxPriority6.SelectedIndex; enabled = textBoxPath6.Enabled; break;
                case 7: processName = textBoxPath7.Text; winWait = checkBoxWinWait6.Checked; sleepAfter = numericUpDownDelay7After.Value; cpu = numericUpDownCPU7.Value; args = processArgs[n] = textBoxArgs7.Text; minimize = checkBoxMinimize7.Checked; priority = comboBoxPriority7.SelectedIndex; enabled = textBoxPath7.Enabled; break;
                case 8: processName = textBoxPath8.Text; winWait = checkBoxWinWait7.Checked; sleepAfter = numericUpDownDelay8After.Value; cpu = numericUpDownCPU8.Value; args = processArgs[n] = textBoxArgs8.Text; minimize = checkBoxMinimize8.Checked; priority = comboBoxPriority8.SelectedIndex; enabled = textBoxPath8.Enabled; break;
                case 9: processName = textBoxPath9.Text; winWait = checkBoxWinWait8.Checked; sleepAfter = numericUpDownDelay9After.Value; cpu = numericUpDownCPU9.Value; args = processArgs[n] = textBoxArgs9.Text; minimize = checkBoxMinimize9.Checked; priority = comboBoxPriority9.SelectedIndex; enabled = textBoxPath9.Enabled; break;
            }
            if (process[n] == null)
            {
                process[n] = new Process();
            }
            if (!enabled || processName.Length < 1)
            {
                timer1.Interval = timer1.Interval;
                timer1.Start();
                return; // skipping it or empty process name
            }
            if (!File.Exists(processName))
            {
                labelStatusMessage.Text = "Task " + n + " path does not exist";
                MessageBox.Show("File does not exist\n" + processName, "Error StartMe");
                SetStartStop(n, false, false);
                if (ourCursor) Application.UseWaitCursor = false;
                timer1.Interval = timer1.Interval;
                timer1.Start();
                return;
            }
            ProcessPriorityClass priorityClass = ProcessPriorityClass.Normal;
            switch (priority)
            {
                case 0: priorityClass = ProcessPriorityClass.Idle; break;
                case 1: priorityClass = ProcessPriorityClass.BelowNormal; break;
                case 2: priorityClass = ProcessPriorityClass.Normal; break;
                case 3: priorityClass = ProcessPriorityClass.AboveNormal; break;
                case 4: priorityClass = ProcessPriorityClass.High; break;
                case 5: priorityClass = ProcessPriorityClass.RealTime; break;
                case 6: priorityClass = ProcessPriorityClass.Idle; break;
            }
            process[n].StartInfo.Arguments = args;
            process[n].StartInfo.FileName = processName;
            process[n].StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(process[n].StartInfo.FileName);
            //process[n].StartInfo.Verb = "runas";
            if (ProcessIsRunning(n))
            {
                timer1.Start();
                return; // already running
            }
            if (minimize)
            {
                process[n].StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }
            try
            {
                process[n].Start();
                Thread.Sleep(500);
                IntPtr mainWindowHandle;
                Stopwatch timerHandle = new Stopwatch();
                timerHandle.Start();
                int timeout = 30; // seconds
                labelStatusMessage.Text = "Waiting for task#" + n + " window handle";
                do
                {
                    mainWindowHandle = process[n].MainWindowHandle;
                    long myTime = timeout - (timerHandle.ElapsedMilliseconds / 1000);
                    Color labelColor = labelStatusMessage.ForeColor;
                    if (labelColor == Color.Black) labelColor = Color.Red; else labelColor = Color.Black;
                    labelStatusMessage.ForeColor = labelColor;
                    labelStatusMessage.Text = "Waiting for task#" + n + " window handle " + myTime;
                    labelStatusMessage.Refresh();
                    Application.DoEvents();
                    Thread.Sleep(200);
                } while (winWait && timerHandle.ElapsedMilliseconds < timeout * 1000 && mainWindowHandle == (IntPtr)0);
                labelStatusMessage.ForeColor = Color.Black;

                //if (mainWindowHandle == null)
                //{
                //    MessageBox.Show("No main window handle??", "Debug StartMe");
                //}
                timerHandle.Stop();
                ProcessSetMainWindowHandle(n, mainWindowHandle);
            }
            catch (Exception ex)
            {
                labelStatusMessage.Text = "Task " + n + " error did not start";
                if (ourCursor) Application.UseWaitCursor = false;
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error StartMe");
                timer1.Start();
                return;
            }
            //processID[n] = process[n].Id;
            do
            {
                Thread.Sleep(1000);
                labelStatusMessage.Text = "After task#" + n + " sleeping " + sleepAfter;
                Application.DoEvents();
            } while (--sleepAfter > 0);
            try
            {
                labelStatusMessage.Text = "Task " + n + " waiting for input idle";
                Application.DoEvents();
                process[n].WaitForInputIdle();
            }
            catch (Exception)
            {
                //MessageBox.Show("Here5");
                // catch error on non-GUI processes
            }
            if (priorityClass == ProcessPriorityClass.RealTime && !IsUserAdministrator())
            {
                MessageBox.Show("RealTime priority requires runnig as Admin.  Will run as High instead", "Info StartMe");
            }
            bool gotit;
            do
            {
                try
                {
                    if (priorityClass != ProcessPriorityClass.Normal)
                    {
                        process[n].PriorityClass = priorityClass;
                    }
                    gotit = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error setting priority?  Administrator needed?\n" + ex.StackTrace, "Error StartMe");
                    gotit = true; // just to get out of the loop
                }
            } while (gotit == false);

            //process[n] = Process.GetProcessById(processID[n]);
            try
            {
                switch (n)
                {
                    case 1: pid1.Text = process[n].Id.ToString(); break;
                    case 2: pid2.Text = process[n].Id.ToString(); break;
                    case 3: pid3.Text = process[n].Id.ToString(); break;
                    case 4: pid4.Text = process[n].Id.ToString(); break;
                    case 5: pid5.Text = process[n].Id.ToString(); break;
                    case 6: pid6.Text = process[n].Id.ToString(); break;
                    case 7: pid7.Text = process[n].Id.ToString(); break;
                    case 8: pid8.Text = process[n].Id.ToString(); break;
                    case 9: pid9.Text = process[n].Id.ToString(); break;
                }
            }
            catch (Exception ex)
            {
                labelStatusMessage.Text = "Task " + n + " error invalid #";
                if (ourCursor) Application.UseWaitCursor = false;
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error StartMe");
                timer1.Start();
                return;
            }

            labelStatusMessage.Text = "Task " + n + " waiting for CPU% <" + cpu;
            //Application.DoEvents();
            //Application.UseWaitCursor = true;
            int cpupct;
            PerformanceCounter mycpupct1 = new PerformanceCounter("Process", "% Processor Time", process[n].ProcessName);
            try
            {
                do
                { // sleep for 1000ms and wait for < desired CPU usage
                    Thread.Sleep(500);
                    cpupct = (int)mycpupct1.NextValue();
                    labelStatusMessage.Text = "Task " + n + " waiting for CPU% <" + cpu + ", cpu=" + cpupct;
                    Application.DoEvents();
                } while (cpupct > cpu);
            }
            catch (Exception)
            {
                labelStatusMessage.Text = "debug";// continue is any of the above causes a problem -- rigctld starting on it's own for example does this
            }
            mycpupct1.Dispose();
            SetStartStop(n, false, true);
            labelStatusMessage.Text = "Task " + n + " started";
            this.Activate();
            if (ourCursor) Application.UseWaitCursor = false;
            Application.DoEvents();
            timer1.Interval = timer1.Interval;
            timer1.Start();
        }

        private void ProcessSetMainWindowHandle(int n, IntPtr mainWindowHandle)
        {
            string myText = "";
            if (mainWindowHandle != (IntPtr)0)
            {
                myText = mainWindowHandle.ToString("X");
            }
            switch (n) // we only want to set the handle once -- so be sure to clear it when done
            {
                case 1: if (labelMainWindowHandle1.Text.Length == 0 || myText.Length == 0) labelMainWindowHandle1.Text = myText; break;
                case 2: if (labelMainWindowHandle2.Text.Length == 0 || myText.Length == 0) labelMainWindowHandle2.Text = myText; break;
                case 3: if (labelMainWindowHandle3.Text.Length == 0 || myText.Length == 0) labelMainWindowHandle3.Text = myText; break;
                case 4: if (labelMainWindowHandle4.Text.Length == 0 || myText.Length == 0) labelMainWindowHandle4.Text = myText; break;
                case 5: if (labelMainWindowHandle5.Text.Length == 0 || myText.Length == 0) labelMainWindowHandle5.Text = myText; break;
                case 6: if (labelMainWindowHandle6.Text.Length == 0 || myText.Length == 0) labelMainWindowHandle6.Text = myText; break;
                case 7: if (labelMainWindowHandle7.Text.Length == 0 || myText.Length == 0) labelMainWindowHandle7.Text = myText; break;
                case 8: if (labelMainWindowHandle8.Text.Length == 0 || myText.Length == 0) labelMainWindowHandle8.Text = myText; break;
                case 9: if (labelMainWindowHandle9.Text.Length == 0 || myText.Length == 0) labelMainWindowHandle9.Text = myText; break;
                default: labelMainWindowHandle1.Text = "?Handle"; break;
            }
        }

        /*
        private bool EnumWindow(IntPtr hWnd, IntPtr lParam)
        {
            var WM_CLOSE = 0x10;
            if (GetParent(hWnd) != null)
            {
                SendMessage(hWnd, WM_CLOSE, (IntPtr)0, null);
            }
            return true;
        }
        */
        private bool ProcessStop(int n, Keys modifierKeys)

        {
            timer1.Stop();
            labelStatusMessage.Text = "Stopping Task " + n;
            if (!ProcessIsRunning(n)) // don't need to stop it then
            {
                timer1.Interval = timer1.Interval;
                timer1.Start();
                return true;
            }
            // we only mess with the cursor if nobody else is
            bool ourCursor = false;
            if (Application.UseWaitCursor == false)
            {
                Application.UseWaitCursor = true;
                ourCursor = true;
            }
            Application.DoEvents();
            if (modifierKeys.HasFlag(Keys.Control) && ProcessIsRunning(n) && !modifierKeys.HasFlag(Keys.Alt))
            {
                if (MessageBox.Show("This will impolitely stop this task.  Are you sure?", "Info StartMe", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    process[n].Kill();
                }
                if (ourCursor) Application.UseWaitCursor = false;
                timer1.Interval = timer1.Interval;
                timer1.Start();
                return true;
            }

            bool kill = false;
            decimal stopWait = 0;
            bool hasStopSend = false;
            switch (n)
            {
                case 1: stopWait = numericUpDownDelayStop1.Value; kill = checkBoxKill1.Checked; hasStopSend = textBoxStop1.Text.Length > 0; break;
                case 2: stopWait = numericUpDownDelayStop2.Value; kill = checkBoxKill2.Checked; hasStopSend = textBoxStop2.Text.Length > 0; break;
                case 3: stopWait = numericUpDownDelayStop3.Value; kill = checkBoxKill3.Checked; hasStopSend = textBoxStop3.Text.Length > 0; break;
                case 4: stopWait = numericUpDownDelayStop4.Value; kill = checkBoxKill4.Checked; hasStopSend = textBoxStop4.Text.Length > 0; break;
                case 5: stopWait = numericUpDownDelayStop5.Value; kill = checkBoxKill5.Checked; hasStopSend = textBoxStop5.Text.Length > 0; break;
                case 6: stopWait = numericUpDownDelayStop6.Value; kill = checkBoxKill6.Checked; hasStopSend = textBoxStop6.Text.Length > 0; break;
                case 7: stopWait = numericUpDownDelayStop7.Value; kill = checkBoxKill7.Checked; hasStopSend = textBoxStop7.Text.Length > 0; break;
                case 8: stopWait = numericUpDownDelayStop8.Value; kill = checkBoxKill8.Checked; hasStopSend = textBoxStop8.Text.Length > 0; break;
                case 9: stopWait = numericUpDownDelayStop9.Value; kill = checkBoxKill9.Checked; hasStopSend = textBoxStop9.Text.Length > 0; break;
            }
            bool timeout = false;
            int loops = 0;
            while (ProcessIsRunning(n) && !timeout)
            {
                try
                {
                    //var hMain = GetParent(process[n].MainWindowHandle);
                    //process[n] = Process.GetProcessById(process[n].Id);
                    //Application.DoEvents();
                    // JTAlert and LogOMUI do not behave well closing child windows
                    //if (false &&   !process[n].ProcessName.Contains("JTAlert")
                    //&& !process[n].ProcessName.Contains("LogOMUI")
                    //    )
                    //{
                    //    // Close all the child windows
                    //    EnumChildWindows(process[n].MainWindowHandle, EnumWindow, (IntPtr)0);
                    //    Thread.Sleep(200);
                    //}
                    // Update our process to get the parent window
                    if (!ProcessIsRunning(n)) return true;
                    //process[n] = Process.GetProcessById(process[n].Id);
                    //IntPtr mainWindowHandle = ProcessGetMainWindowHandle(n);
                    //if (mainWindowHandle != (IntPtr)0) SetForegroundWindow(mainWindowHandle);
                    //else SetForegroundWindow(process[n].MainWindowHandle);
                    SafeNativeMethods.SetForegroundWindow(process[n].MainWindowHandle);
                    Application.DoEvents();
                    if (!hasStopSend)  // only ask for 2nd close window if not sending keys to window
                    {
                        // do it again -- seems like this message gets lost sometimes
                        try
                        {
                            if (ProcessIsRunning(n))
                            {
                                process[n].CloseMainWindow();
                                Thread.Sleep(500);
                                process[n] = Process.GetProcessById(process[n].Id);
                                process[n].CloseMainWindow();
                            }
                        }
                        catch (Exception)
                        {
                            // Nothing to do here if we get an error
                        }
                    
                        Application.DoEvents();
                        //Thread.Sleep(200);
                    }
                    else
                    {
                        process[n].CloseMainWindow();
                        // Need the same sleep time as the clause above
                        // This keeps the sendKeys timing consistent after window to foreground
                        Thread.Sleep(400);
                        ProcessSendKeys(n, false);
                    }
                    Thread.Sleep(300);
                    if (!ProcessIsRunning(n)) // we're done
                    {
                        labelStatusMessage.Text = "Task " + n + " stopped";
                        Application.DoEvents();
                        timer1.Interval = timer1.Interval;
                        timer1.Start();
                        if (ourCursor) Application.UseWaitCursor = false;
                        SetPid(n, "");
                        return true;
                    }
                    else
                    {
                        labelStatusMessage.Text = "Task " + n + " did not stop yet..waiting...";
                        Application.DoEvents();
                    }
                    if (kill)
                    {
                        process[n].Kill();
                        Thread.Sleep(1000);
                        timer1.Interval = timer1.Interval;
                        timer1.Start();
                        if (ourCursor) Application.UseWaitCursor = false;
                        if (ProcessIsRunning(n))
                        {
                            return false;
                        }
                        SetPid(n, "");
                        return true;
                    }
                    Color labelColor = Color.Black;
                    Stopwatch timerHandle = new Stopwatch();
                    timerHandle.Start();
                    while (ProcessIsRunning(n) && (timerHandle.ElapsedMilliseconds < stopWait * 1000))
                    {
                        Thread.Sleep(1000);
                        if (labelColor == Color.Black) labelColor = Color.Red;
                        else labelColor = Color.Black;
                        labelStatusMessage.ForeColor = labelColor;
                        labelStatusMessage.Text = "Task " + n + " did not stop yet..waiting..." + (stopWait - (timerHandle.ElapsedMilliseconds / 1000));
                        Application.DoEvents();
                        ++loops;
                    }
                    if (timerHandle.ElapsedMilliseconds >= stopWait * 1000) timeout = true;
                    labelStatusMessage.ForeColor = Color.Black;
                    labelStatusMessage.Text = "Task " + n + " did not stop yet..waiting..."+(stopWait - (timerHandle.ElapsedMilliseconds / 1000));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problem stopping task#" + n + "\n" + ex.Message + "\n" + ex.StackTrace, "Error StartMe");
                    timer1.Interval = timer1.Interval;
                    timer1.Start();
                    if (ourCursor) Application.UseWaitCursor = false;
                    return false;
                }
            }

            if (timeout && ProcessIsRunning(n))
            {
                MessageBox.Show("Task " + process[n].ProcessName + " did not terminate", "Warning StartMe", MessageBoxButtons.OK);
                Application.UseWaitCursor = false;
                labelStatusMessage.Text = "Task " + n + " did not stop?";
                timer1.Interval = timer1.Interval;
                timer1.Start();
                if (ourCursor) Application.UseWaitCursor = false;
                return false;
            }
            if (ourCursor) Application.UseWaitCursor = false;
            timer1.Interval = timer1.Interval;
            timer1.Start();
            labelStatusMessage.Text = "Task " + n + " stopped";
            SetPid(n, "");
            return true;
        }

        private void ButtonStop1_Click(object sender, EventArgs e)
        {
            if (ProcessStop(1, ModifierKeys))
            {
                buttonStart1.Enabled = true;
                buttonStop1.Enabled = false;
                pid1.Text = "";
                labelMainWindowHandle1.Text = "";
                Application.DoEvents();
            }
        }

        private void ButtonStop2_Click(object sender, EventArgs e)
        {
            if (ProcessStop(2, ModifierKeys))
            {
                buttonStart2.Enabled = true;
                buttonStop2.Enabled = false;
                pid2.Text = "";
                labelMainWindowHandle2.Text = "";
                Application.DoEvents();
            }

        }

        private void ButtonStop3_Click(object sender, EventArgs e)
        {
            if (ProcessStop(3, ModifierKeys))
            {
                buttonStart3.Enabled = true;
                buttonStop3.Enabled = false;
                pid3.Text = "";
                labelMainWindowHandle3.Text = "";
                Application.DoEvents();
            }

        }

        private void ButtonStop4_Click(object sender, EventArgs e)
        {
            if (ProcessStop(4, ModifierKeys))
            {
                buttonStart4.Enabled = true;
                buttonStop4.Enabled = false;
                pid4.Text = "";
                labelMainWindowHandle4.Text = "";
                Application.DoEvents();
            }

        }

        private void ButtonStop5_Click(object sender, EventArgs e)
        {
            if (ProcessStop(5, ModifierKeys))
            {
                buttonStart5.Enabled = true;
                buttonStop5.Enabled = false;
                pid5.Text = "";
                labelMainWindowHandle5.Text = "";
                Application.DoEvents();
            }

        }

        private void ButtonStop6_Click(object sender, EventArgs e)
        {
            if (ProcessStop(6, ModifierKeys))
            {
                buttonStart6.Enabled = true;
                buttonStop6.Enabled = false;
                pid6.Text = "";
                labelMainWindowHandle6.Text = "";
                Application.DoEvents();
            }

        }

        private void ButtonStop7_Click(object sender, EventArgs e)
        {
            if (ProcessStop(7, ModifierKeys))
            {
                buttonStart7.Enabled = true;
                buttonStop7.Enabled = false;
                pid7.Text = "";
                labelMainWindowHandle7.Text = "";
                Application.DoEvents();
            }

        }

        private void ButtonStop8_Click(object sender, EventArgs e)
        {
            if (ProcessStop(8, ModifierKeys))
            {
                buttonStart8.Enabled = true;
                buttonStop8.Enabled = false;
                pid8.Text = "";
                labelMainWindowHandle8.Text = "";
                Application.DoEvents();
            }

        }

        private void ButtonStop9_Click(object sender, EventArgs e)
        {
            if (ProcessStop(9, ModifierKeys))
            {
                buttonStart9.Enabled = true;
                buttonStop9.Enabled = false;
                pid9.Text = "";
                labelMainWindowHandle9.Text = "";
                Application.DoEvents();
            }
        }

        private void StartAllConfigs(bool autoOnly = false)
        {
            int next;
            bool started;

            List<string> settingsKeys;
            //if (settingsKey == "") // then will will autostart all settings
            //{
                settingsKeys = SettingsGetKeys();
            //}
            //else // we will only start the settings passed in
            //{
            //    settingsKeys.Add(settingsKey);
            //}

            foreach (string settings in settingsKeys)
            {
                if (SettingsLoad(settings) == false)
                {
                    MessageBox.Show("Unable to load settings for " + settings);
                    continue;
                }
                ProcessInit();
                Application.DoEvents(); // let the controls update
                next = 1;
                started = false;
                while (next <= 9)
                {
                    this.BringToFront();
                    String snext = next.ToString();
                    bool checkBox;
                    string textBox;
                    switch (next)
                    {
                        case 1: checkBox = checkBoxAutoStart1.Checked; textBox = textBoxStart1Sequence.Text; break;
                        case 2: checkBox = checkBoxAutoStart2.Checked; textBox = textBoxStart2Sequence.Text; break;
                        case 3: checkBox = checkBoxAutoStart3.Checked; textBox = textBoxStart3Sequence.Text; break;
                        case 4: checkBox = checkBoxAutoStart4.Checked; textBox = textBoxStart4Sequence.Text; break;
                        case 5: checkBox = checkBoxAutoStart5.Checked; textBox = textBoxStart5Sequence.Text; break;
                        case 6: checkBox = checkBoxAutoStart6.Checked; textBox = textBoxStart6Sequence.Text; break;
                        case 7: checkBox = checkBoxAutoStart7.Checked; textBox = textBoxStart7Sequence.Text; break;
                        case 8: checkBox = checkBoxAutoStart8.Checked; textBox = textBoxStart8Sequence.Text; break;
                        case 9: checkBox = checkBoxAutoStart9.Checked; textBox = textBoxStart9Sequence.Text; break;
                        default: checkBox = false; textBox = null; break;

                    }
                    if (autoOnly == true && checkBox == true && textBox.Equals(snext))
                    {
                        ProcessStart(next, ModifierKeys);
                        started = true;
                    }
                    else if (autoOnly == false && textBox.Equals(snext))
                    {
                        ProcessStart(next, ModifierKeys);
                        started = true;
                    }
                    ++next;
                }
                if (started)
                {
                    labelStatusMessage.Text = "All Tasks Started";
                    Application.UseWaitCursor = false;
                    this.Activate();
                    return; // return if we stopped using sequence numbers
                }
                // Otherwise we just start them in sequence
                next = 1;
                while (next <= 9)
                {
                    var checkBox = next switch
                    {
                        1 => checkBoxAutoStart1.Checked,
                        2 => checkBoxAutoStart2.Checked,
                        3 => checkBoxAutoStart3.Checked,
                        4 => checkBoxAutoStart4.Checked,
                        5 => checkBoxAutoStart5.Checked,
                        6 => checkBoxAutoStart6.Checked,
                        7 => checkBoxAutoStart7.Checked,
                        8 => checkBoxAutoStart8.Checked,
                        9 => checkBoxAutoStart9.Checked,
                        _ => false,
                    };
                    // If autoOnly requested then just them, or if not autoOnly then anybody can start here
                    if ((autoOnly == true && checkBox == true) || autoOnly == false)
                    {
                        if (!ProcessIsRunning(next))
                            ProcessStart(next, ModifierKeys);
                        started = true;
                    }
                    ++next;
                }
            }
            Application.UseWaitCursor = false;
            SettingsLoad("Default");
        }

        string GetStartSequence(int n)
        {
            var textBox = n switch
            {
                1 => textBoxStart1Sequence.Text,
                2 => textBoxStart2Sequence.Text,
                3 => textBoxStart3Sequence.Text,
                4 => textBoxStart4Sequence.Text,
                5 => textBoxStart5Sequence.Text,
                6 => textBoxStart6Sequence.Text,
                7 => textBoxStart7Sequence.Text,
                8 => textBoxStart8Sequence.Text,
                9 => textBoxStart9Sequence.Text,
                _ => null,
            };
            return textBox;
        }
        string GetStartStopSequence(int n)
        {
            var textBox = n switch
            {
                1 => textBoxStart1Stop.Text,
                2 => textBoxStart2Stop.Text,
                3 => textBoxStart3Stop.Text,
                4 => textBoxStart4Stop.Text,
                5 => textBoxStart5Stop.Text,
                6 => textBoxStart6Stop.Text,
                7 => textBoxStart7Stop.Text,
                8 => textBoxStart8Stop.Text,
                9 => textBoxStart9Stop.Text,
                _ => null,
            };
            return textBox;
        }
        bool GetAutoStart(int n)
        {
            var autoStart = n switch
            {
                1 => checkBoxAutoStart1.Checked,
                2 => checkBoxAutoStart2.Checked,
                3 => checkBoxAutoStart3.Checked,
                4 => checkBoxAutoStart4.Checked,
                5 => checkBoxAutoStart5.Checked,
                6 => checkBoxAutoStart6.Checked,
                7 => checkBoxAutoStart7.Checked,
                8 => checkBoxAutoStart8.Checked,
                9 => checkBoxAutoStart9.Checked,
                _ => false,
            };
            return autoStart;
        }
        private void StartAll(bool autoOnly = false)
        {
            bool started = false;
            int next = 1;
            Application.UseWaitCursor = true;
            Application.DoEvents();
            while (next <= 9)
            {
                this.BringToFront();
                String snext = next.ToString();
                string textBox = GetStartSequence(next);
                bool autoStart = GetAutoStart(next);
                bool doWeStart = (autoOnly == true && autoStart) || autoOnly == false;
                if (snext != null && textBox.Equals(snext) && doWeStart)
                {
                    ProcessStart(next, ModifierKeys);
                    started = true;
                }
                ++next;
            }
            if (started)
            {
                labelStatusMessage.Text = "All Tasks Started by Start#";
                Application.UseWaitCursor = false;
                return; // return if we stopped anything by using sequence numbers
            }
            // Otherwise we just start them in sequence
            next = 1;
            while (next <= 9)
            {
                bool autoStart = GetAutoStart(next);
                bool doWeStart = (autoOnly == true && autoStart) || autoOnly == false;
                if (doWeStart)
                {
                    ProcessStart(next, ModifierKeys);
                }
                ++next;
            }
            labelStatusMessage.Text = "All Tasks Started by Task#";
            Application.UseWaitCursor = false;
        }
        private void StopAll()
        {
            bool stopped = false;
            int next = 1;
            Application.UseWaitCursor = true;
            Application.DoEvents();
            // First off we get the sequence of startups
            while (next <= 9)
            {
                this.BringToFront();
                String snext = next.ToString();
                labelStatusMessage.Text = "Stopping task#" + next;
                string stopSeq = GetStartStopSequence(next);
                // Allow skipping of stop if requested with "0"
                if (stopSeq.Equals("0"))
                {
                    labelStatusMessage.Text = "Skipping stop of task#" + snext;
                }
                else if (stopSeq.Equals(snext))
                {
                    stopped = true;
                    ProcessStop(next, ModifierKeys);
                    Application.DoEvents();
                }
                ++next;
            }
            // If we stopped anything at using sequence numbers at all we return
            if (stopped)
            {
                labelStatusMessage.Text = "Tasks have been stopped";
                Application.UseWaitCursor = false;
                return;
            }
            // Otherwise we'll use the sequence numbers in the Start sequence
            next = 9;
            while (next > 0)
            {
                String snext = next.ToString();
                labelStatusMessage.Text = "Stopping task#" + next;
                string skipIt = GetStartStopSequence(next);  // Allow skip request
                // Allow skipping of stop if requested with "0"
                if (skipIt.Equals("0"))
                {
                    labelStatusMessage.Text = "Skipping stop of task#" + snext;
                }
                else
                {
                    stopped = true;
                    _ = ProcessStop(next, ModifierKeys);
                    SetStartStop(next, true, false);
                    Application.DoEvents();
                }
                --next;
            }
            if (stopped)
            {
                labelStatusMessage.Text = "Tasks have been stopped";
                Application.UseWaitCursor = false;
                return;
            }
            // Otherwise we stop all in reverse sequence
            next = 9;
            while (next > 0)
            {
                String snext = next.ToString();
                labelStatusMessage.Text = "Stopping task#" + next;
                string skipSeq = GetStartStopSequence(next);
                // Allow skipping of stop if requested with "0"
                if (skipSeq.Equals("0"))
                {
                    labelStatusMessage.Text = "Skipping stop of task#" + snext;
                }
                else
                {
                    ProcessStop(next, ModifierKeys);
                    SetStartStop(next, true, false);
                    Application.DoEvents();
                }
                --next;
            }
            Application.UseWaitCursor = false;
            labelStatusMessage.Text = "All tasks stopped";
        }

        private void ButtonStopAll_Click_1(object sender, EventArgs e)
        {
            StopAll();
        }

        private string FileGet(string path)
        {
            FileDialog fileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Executables (*.exe *.bat, *.cmd)|*.exe;*.bat;*.cmd|All Files (*.*) |*.*",
            };
            if (path.Length > 0)
            {
                fileDialog.InitialDirectory = Path.GetDirectoryName(path);
            }
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                fileDialog.Dispose();
                return fileDialog.FileName;
            }
            fileDialog.Dispose();
            return null;
        }

        private void ButtonFile1_Click(object sender, EventArgs e)
        {
            String file = FileGet(textBoxPath1.Text);
            if (file != null)
            {
                textBoxPath1.Text = file;
                SetPathColor(1, Color.Black);
                //SetEZName(1);
                ProcessIsRunning(1);
            }
        }

        private void ButtonFile2_Click(object sender, EventArgs e)
        {
            String file = FileGet(textBoxPath2.Text);
            if (file != null)
            {
                textBoxPath2.Text = file;
                SetPathColor(2, Color.Black);
                //SetEZName(2);
                ProcessIsRunning(2);
            }
        }

        private void ButtonFile3_Click(object sender, EventArgs e)
        {
            String file = FileGet(textBoxPath3.Text);
            if (file != null)
            {
                textBoxPath3.Text = file;
                SetPathColor(3, Color.Black);
                //SetEZName(3);
                ProcessIsRunning(3);
            }
        }

        private void ButtonFile4_Click(object sender, EventArgs e)
        {
            String file = FileGet(textBoxPath4.Text);
            if (file != null)
            {
                textBoxPath4.Text = file;
                SetPathColor(4, Color.Black);
                //SetEZName(4);
                ProcessIsRunning(4);

            }
        }

        private void ButtonFile5_Click(object sender, EventArgs e)
        {
            String file = FileGet(textBoxPath5.Text);
            if (file != null)
            {
                textBoxPath5.Text = file;
                SetPathColor(5, Color.Black);
                //SetEZName(5);
                ProcessIsRunning(5);
            }
        }

        private void ButtonFile6_Click(object sender, EventArgs e)
        {
            String file = FileGet(textBoxPath6.Text);
            if (file != null)
            {
                textBoxPath6.Text = file;
                SetPathColor(6, Color.Black);
                //SetEZName(6);
                ProcessIsRunning(6);
            }
        }

        private void ButtonFile7_Click(object sender, EventArgs e)
        {
            String file = FileGet(textBoxPath7.Text);
            if (file != null)
            {
                textBoxPath7.Text = file;
                SetPathColor(7, Color.Black);
                //SetEZName(7);
                ProcessIsRunning(7);
            }
        }

        private void ButtonFile8_Click(object sender, EventArgs e)
        {
            String file = FileGet(textBoxPath8.Text);
            if (file != null)
            {
                textBoxPath8.Text = file;
                SetPathColor(8, Color.Black);
                //SetEZName(8);
                ProcessIsRunning(8);
            }
        }

        private void ButtonFile9_Click(object sender, EventArgs e)
        {
            String file = FileGet(textBoxPath9.Text);
            if (file != null)
            {
                textBoxPath9.Text = file;
                SetPathColor(9, Color.Black);
                //SetEZName(9);
                ProcessIsRunning(9);
            }
        }

        private void Form1Closing(object sender, FormClosingEventArgs e)
        {
            mut.WaitOne();
            settingsSave = true;
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
            settingsSave = false;
            mut.ReleaseMutex();
        }

        private bool GetKeep(int n)
        {
            var keepRunning = n switch
            {
                1 => checkBoxKeepRunning1.Checked,
                2 => checkBoxKeepRunning2.Checked,
                3 => checkBoxKeepRunning3.Checked,
                4 => checkBoxKeepRunning4.Checked,
                5 => checkBoxKeepRunning5.Checked,
                6 => checkBoxKeepRunning6.Checked,
                7 => checkBoxKeepRunning7.Checked,
                8 => checkBoxKeepRunning8.Checked,
                9 => checkBoxKeepRunning9.Checked,
                _ => false,
            };
            return keepRunning;
        }
        private Int64 GetPid(int n)
        {
            var pid = n switch
            {
                1 => pid1.Text,
                2 => pid2.Text,
                3 => pid3.Text,
                4 => pid4.Text,
                5 => pid5.Text,
                6 => pid6.Text,
                7 => pid7.Text,
                8 => pid8.Text,
                9 => pid9.Text,
                _ => "",
            };
            if (pid.Length == 0) return 0;
            return Int64.Parse(pid);
        }
        private void SetPid(int n, string pidText)
        {
            switch (n)
            {
                case 1: pid1.Text = pidText; break;
                case 2: pid2.Text = pidText; break;
                case 3: pid3.Text = pidText; break;
                case 4: pid4.Text = pidText; break;
                case 5: pid5.Text = pidText; break;
                case 6: pid6.Text = pidText; break;
                case 7: pid7.Text = pidText; break;
                case 8: pid8.Text = pidText; break;
                case 9: pid9.Text = pidText; break;
            }
            if (pidText.Length == 0)
            {
                ProcessSetMainWindowHandle(n, (IntPtr)0);
            }
        }

        private void ProcessUpdate()
        {
            for (int i = 1; i < 10; ++i)
            {
                bool running = false;
                if (process[i] == null)
                {
                    process[i] = new Process();
                }
                if (ProcessIsRunning(i))
                {
                    try
                    {
                        Process[] processes = Process.GetProcessesByName(process[i].ProcessName);

                        int len = processes.Length;
                        process[i] = processes[0];
                        running = true;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                //SetStartStop(i, !running, running);
                if (!running && GetKeep(i) && GetPid(i)!=0)
                {
                    ProcessStart(i, ModifierKeys);
                }
                if (!running) SetPid(i, "");
            }
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            // don't eat CPU time if not visible
            if (this.WindowState != FormWindowState.Minimized)
            {
                ProcessUpdate();
            }
            timer1.Start();
        }

        private void TaskNumberCheck(object sender, EventArgs e)
        {
            // check we have space delimited integers 1 through 9
            MaskedTextBox text = (MaskedTextBox)sender;
            String[] tokens = text.Text.Split(' ');
            if (tokens[0].Length == 0) return;
            foreach (String s in tokens)
            {
                try
                {
                    int n = Int32.Parse(s);
                    if (n < 0 || n > 9)
                    {
                        MessageBox.Show("Only space delmiited values 1 through 9 can be used here");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error converting \"" + s + "\" to integer\n" + ex.StackTrace, "Error StartMe");
                }
            }
        }

        // Return a byte array as a sequence of hex values.
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }
        // If hash code changes back it up for 5 levels
        void FileBackup(string fileNow)
        {
            if (backedUp) return; // we only do this once per run
            backedUp = true;
            using var md5Now = MD5.Create();
            using var md5Previous = MD5.Create();
            using (var stream = File.OpenRead(fileNow))
            {
                md5Now.ComputeHash(stream);
            }
            string filePrevious = fileNow + ".1";

            // If we've never backed it up just do it and be done
            if (!File.Exists(filePrevious))
            {
                File.Copy(fileNow, filePrevious);
                string msg = "Your user.config was backed up -- Click Backups to manage";
                labelStatusMessage.Text = msg;
                return;
            }
            // Otherwise we'll get the MD5 of the backup and see if things changed
            using (var stream = File.OpenRead(filePrevious))
            {
                md5Previous.ComputeHash(stream);
            }
            string hashNow = BytesToString(md5Now.Hash);
            string hashPrevious = BytesToString(md5Previous.Hash);
            if (!hashNow.Equals(hashPrevious))
            {
                string msg = "Your user.config was backed up -- Click Backups to manage";
                // Move 1-4 to 2-5 rolling backup
                for (int i = 8; i >= 1; --i)
                {
                    string fileFrom = fileNow + "." + i;
                    string fileTo = fileNow + "." + (i + 1);
                    if (File.Exists(fileFrom))
                    {
                        File.Delete(fileTo);
                        File.Move(fileFrom, fileTo);
                    }
                }
                // Copy our existing user.config
                File.Copy(fileNow, fileNow + ".1", true);
                labelStatusMessage.Text = msg;
            }
        }
        private List<String> SettingsGetKeys()
        {
            List<String> keys = new List<string>();
            var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            if (!File.Exists(userConfig))
            {
                return null;
            }
            FileBackup(userConfig);
            XDocument doc = XDocument.Load(userConfig);
            if (doc == null)
                return keys;
            foreach (XElement element in doc.Descendants("userSettings"))
            {
                foreach (XElement settings in element.Descendants())
                {
                    foreach (XElement settings2 in settings.Descendants())
                    {
                        String key = settings2.Parent.Name.LocalName;
                        if (key.Contains("StartMe"))
                        {
                            String rootKey = "StartMe.Properties.Settings";
                            String key2 = "Default";
                            if (key.Length > rootKey.Length)
                            {
                                key2 = key.Substring(rootKey.Length + 1);
                            }
                            if (!keys.Contains(key2))
                            {
                                keys.Add(key2);
                            }
                        }
                    }
                }
            }
            keys.Sort();
            comboBoxSettingsKey.Items.Clear();
            foreach (String s in keys)
            {
                comboBoxSettingsKey.Items.Add(s);
            }

            return keys;
        }

        private void SettingsSave(String key)
        {
            if (!settingsSave) return;
            //var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

            Properties.Settings.Default.SettingsKeyCurrent = key;
            Properties.Settings.Default.SettingsKey = key;

            //0 -- Non task specific settings
            Properties.Settings.Default.Minimize = checkBoxMinimize.Checked;
            Properties.Settings.Default.CloseAll = checkBoxStopAll.Checked;
            Properties.Settings.Default.Startup = checkBoxStartup.Checked;

            //1
            Properties.Settings.Default.Path1 = textBoxPath1.Text;
            Properties.Settings.Default.Path2 = textBoxPath2.Text;
            Properties.Settings.Default.Path3 = textBoxPath3.Text;
            Properties.Settings.Default.Path4 = textBoxPath4.Text;
            Properties.Settings.Default.Path5 = textBoxPath5.Text;
            Properties.Settings.Default.Path6 = textBoxPath6.Text;
            Properties.Settings.Default.Path7 = textBoxPath7.Text;
            Properties.Settings.Default.Path8 = textBoxPath8.Text;
            Properties.Settings.Default.Path9 = textBoxPath9.Text;

            //2
            Properties.Settings.Default.Args1 = textBoxArgs1.Text;
            Properties.Settings.Default.Args2 = textBoxArgs2.Text;
            Properties.Settings.Default.Args3 = textBoxArgs3.Text;
            Properties.Settings.Default.Args4 = textBoxArgs4.Text;
            Properties.Settings.Default.Args5 = textBoxArgs5.Text;
            Properties.Settings.Default.Args6 = textBoxArgs6.Text;
            Properties.Settings.Default.Args7 = textBoxArgs7.Text;
            Properties.Settings.Default.Args8 = textBoxArgs8.Text;
            Properties.Settings.Default.Args9 = textBoxArgs9.Text;

            //3
            Properties.Settings.Default.AutoStart1 = checkBoxAutoStart1.Checked;
            Properties.Settings.Default.AutoStart2 = checkBoxAutoStart2.Checked;
            Properties.Settings.Default.AutoStart3 = checkBoxAutoStart3.Checked;
            Properties.Settings.Default.AutoStart4 = checkBoxAutoStart4.Checked;
            Properties.Settings.Default.AutoStart5 = checkBoxAutoStart5.Checked;
            Properties.Settings.Default.AutoStart6 = checkBoxAutoStart6.Checked;
            Properties.Settings.Default.AutoStart7 = checkBoxAutoStart7.Checked;
            Properties.Settings.Default.AutoStart8 = checkBoxAutoStart8.Checked;
            Properties.Settings.Default.AutoStart9 = checkBoxAutoStart9.Checked;

            //4
            Properties.Settings.Default.Minimize1 = checkBoxMinimize1.Checked;
            Properties.Settings.Default.Minimize2 = checkBoxMinimize2.Checked;
            Properties.Settings.Default.Minimize3 = checkBoxMinimize3.Checked;
            Properties.Settings.Default.Minimize4 = checkBoxMinimize4.Checked;
            Properties.Settings.Default.Minimize5 = checkBoxMinimize5.Checked;
            Properties.Settings.Default.Minimize6 = checkBoxMinimize6.Checked;
            Properties.Settings.Default.Minimize7 = checkBoxMinimize7.Checked;
            Properties.Settings.Default.Minimize8 = checkBoxMinimize8.Checked;
            Properties.Settings.Default.Minimize9 = checkBoxMinimize9.Checked;

            //5
            Properties.Settings.Default.Admin1 = checkBoxAdmin1.Checked;
            Properties.Settings.Default.Admin2 = checkBoxAdmin2.Checked;
            Properties.Settings.Default.Admin3 = checkBoxAdmin3.Checked;
            Properties.Settings.Default.Admin4 = checkBoxAdmin4.Checked;
            Properties.Settings.Default.Admin5 = checkBoxAdmin5.Checked;
            Properties.Settings.Default.Admin6 = checkBoxAdmin6.Checked;
            Properties.Settings.Default.Admin7 = checkBoxAdmin7.Checked;
            Properties.Settings.Default.Admin8 = checkBoxAdmin8.Checked;
            Properties.Settings.Default.Admin9 = checkBoxAdmin9.Checked;

            //6
            Properties.Settings.Default.Priority1 = comboBoxPriority1.SelectedIndex;
            Properties.Settings.Default.Priority2 = comboBoxPriority2.SelectedIndex;
            Properties.Settings.Default.Priority3 = comboBoxPriority3.SelectedIndex;
            Properties.Settings.Default.Priority4 = comboBoxPriority4.SelectedIndex;
            Properties.Settings.Default.Priority5 = comboBoxPriority5.SelectedIndex;
            Properties.Settings.Default.Priority6 = comboBoxPriority6.SelectedIndex;
            Properties.Settings.Default.Priority7 = comboBoxPriority7.SelectedIndex;
            Properties.Settings.Default.Priority8 = comboBoxPriority8.SelectedIndex;
            Properties.Settings.Default.Priority9 = comboBoxPriority9.SelectedIndex;

            //7
            Properties.Settings.Default.EZName1 = textBoxEZName1.Text;
            Properties.Settings.Default.EZName2 = textBoxEZName2.Text;
            Properties.Settings.Default.EZName3 = textBoxEZName3.Text;
            Properties.Settings.Default.EZName4 = textBoxEZName4.Text;
            Properties.Settings.Default.EZName5 = textBoxEZName5.Text;
            Properties.Settings.Default.EZName6 = textBoxEZName6.Text;
            Properties.Settings.Default.EZName7 = textBoxEZName7.Text;
            Properties.Settings.Default.EZName8 = textBoxEZName8.Text;
            Properties.Settings.Default.EZName9 = textBoxEZName9.Text;

            //8
            Properties.Settings.Default.WinWait1 = checkBoxWinWait1.Checked;
            Properties.Settings.Default.WinWait2 = checkBoxWinWait2.Checked;
            Properties.Settings.Default.WinWait3 = checkBoxWinWait3.Checked;
            Properties.Settings.Default.WinWait4 = checkBoxWinWait4.Checked;
            Properties.Settings.Default.WinWait5 = checkBoxWinWait5.Checked;
            Properties.Settings.Default.WinWait6 = checkBoxWinWait6.Checked;
            Properties.Settings.Default.WinWait7 = checkBoxWinWait7.Checked;
            Properties.Settings.Default.WinWait8 = checkBoxWinWait8.Checked;
            Properties.Settings.Default.WinWait9 = checkBoxWinWait9.Checked;

            //9
            Properties.Settings.Default.CPU1 = numericUpDownCPU1.Value;
            Properties.Settings.Default.CPU2 = numericUpDownCPU2.Value;
            Properties.Settings.Default.CPU3 = numericUpDownCPU3.Value;
            Properties.Settings.Default.CPU4 = numericUpDownCPU4.Value;
            Properties.Settings.Default.CPU5 = numericUpDownCPU5.Value;
            Properties.Settings.Default.CPU6 = numericUpDownCPU6.Value;
            Properties.Settings.Default.CPU7 = numericUpDownCPU7.Value;
            Properties.Settings.Default.CPU8 = numericUpDownCPU8.Value;
            Properties.Settings.Default.CPU9 = numericUpDownCPU9.Value;

            //10
            Properties.Settings.Default.KeysStart1 = textBoxStart1.Text;
            Properties.Settings.Default.KeysStart2 = textBoxStart2.Text;
            Properties.Settings.Default.KeysStart3 = textBoxStart3.Text;
            Properties.Settings.Default.KeysStart4 = textBoxStart4.Text;
            Properties.Settings.Default.KeysStart5 = textBoxStart5.Text;
            Properties.Settings.Default.KeysStart6 = textBoxStart6.Text;
            Properties.Settings.Default.KeysStart7 = textBoxStart7.Text;
            Properties.Settings.Default.KeysStart8 = textBoxStart8.Text;
            Properties.Settings.Default.KeysStart9 = textBoxStart9.Text;

            //11
            Properties.Settings.Default.Start1Next = textBoxStart1Sequence.Text;
            Properties.Settings.Default.Start2Next = textBoxStart2Sequence.Text;
            Properties.Settings.Default.Start3Next = textBoxStart3Sequence.Text;
            Properties.Settings.Default.Start4Next = textBoxStart4Sequence.Text;
            Properties.Settings.Default.Start5Next = textBoxStart5Sequence.Text;
            Properties.Settings.Default.Start6Next = textBoxStart6Sequence.Text;
            Properties.Settings.Default.Start7Next = textBoxStart7Sequence.Text;
            Properties.Settings.Default.Start8Next = textBoxStart8Sequence.Text;
            Properties.Settings.Default.Start9Next = textBoxStart9Sequence.Text;

            //12
            Properties.Settings.Default.Start1Stop = textBoxStart1Stop.Text;
            Properties.Settings.Default.Start2Stop = textBoxStart2Stop.Text;
            Properties.Settings.Default.Start3Stop = textBoxStart3Stop.Text;
            Properties.Settings.Default.Start4Stop = textBoxStart4Stop.Text;
            Properties.Settings.Default.Start5Stop = textBoxStart5Stop.Text;
            Properties.Settings.Default.Start6Stop = textBoxStart6Stop.Text;
            Properties.Settings.Default.Start7Stop = textBoxStart7Stop.Text;
            Properties.Settings.Default.Start8Stop = textBoxStart8Stop.Text;
            Properties.Settings.Default.Start9Stop = textBoxStart9Stop.Text;

            //13
            Properties.Settings.Default.StopDelay1 = numericUpDownDelayStop1.Value;
            Properties.Settings.Default.StopDelay2 = numericUpDownDelayStop2.Value;
            Properties.Settings.Default.StopDelay3 = numericUpDownDelayStop3.Value;
            Properties.Settings.Default.StopDelay4 = numericUpDownDelayStop4.Value;
            Properties.Settings.Default.StopDelay5 = numericUpDownDelayStop5.Value;
            Properties.Settings.Default.StopDelay6 = numericUpDownDelayStop6.Value;
            Properties.Settings.Default.StopDelay7 = numericUpDownDelayStop7.Value;
            Properties.Settings.Default.StopDelay8 = numericUpDownDelayStop8.Value;
            Properties.Settings.Default.StopDelay9 = numericUpDownDelayStop9.Value;

            //14
            Properties.Settings.Default.KeysStop1 = textBoxStop1.Text;
            Properties.Settings.Default.KeysStop2 = textBoxStop2.Text;
            Properties.Settings.Default.KeysStop3 = textBoxStop3.Text;
            Properties.Settings.Default.KeysStop4 = textBoxStop4.Text;
            Properties.Settings.Default.KeysStop5 = textBoxStop5.Text;
            Properties.Settings.Default.KeysStop6 = textBoxStop6.Text;
            Properties.Settings.Default.KeysStop7 = textBoxStop7.Text;
            Properties.Settings.Default.KeysStop8 = textBoxStop8.Text;
            Properties.Settings.Default.KeysStop9 = textBoxStop9.Text;

            //15
            Properties.Settings.Default.Kill1 = checkBoxKill1.Checked;
            Properties.Settings.Default.Kill2 = checkBoxKill2.Checked;
            Properties.Settings.Default.Kill3 = checkBoxKill3.Checked;
            Properties.Settings.Default.Kill4 = checkBoxKill4.Checked;
            Properties.Settings.Default.Kill5 = checkBoxKill5.Checked;
            Properties.Settings.Default.Kill6 = checkBoxKill6.Checked;
            Properties.Settings.Default.Kill7 = checkBoxKill7.Checked;
            Properties.Settings.Default.Kill8 = checkBoxKill8.Checked;
            Properties.Settings.Default.Kill9 = checkBoxKill9.Checked;

            //16
            Properties.Settings.Default.StartDelay1After = numericUpDownDelay1After.Value;
            Properties.Settings.Default.StartDelay2After = numericUpDownDelay2After.Value;
            Properties.Settings.Default.StartDelay3After = numericUpDownDelay3After.Value;
            Properties.Settings.Default.StartDelay4After = numericUpDownDelay4After.Value;
            Properties.Settings.Default.StartDelay5After = numericUpDownDelay5After.Value;
            Properties.Settings.Default.StartDelay6After = numericUpDownDelay6After.Value;
            Properties.Settings.Default.StartDelay7After = numericUpDownDelay7After.Value;
            Properties.Settings.Default.StartDelay8After = numericUpDownDelay8After.Value;
            Properties.Settings.Default.StartDelay9After = numericUpDownDelay9After.Value;

            //17
            Properties.Settings.Default.Path1Enabled = textBoxPath1.Enabled;
            Properties.Settings.Default.Path2Enabled = textBoxPath2.Enabled;
            Properties.Settings.Default.Path3Enabled = textBoxPath3.Enabled;
            Properties.Settings.Default.Path4Enabled = textBoxPath4.Enabled;
            Properties.Settings.Default.Path5Enabled = textBoxPath5.Enabled;
            Properties.Settings.Default.Path6Enabled = textBoxPath6.Enabled;
            Properties.Settings.Default.Path7Enabled = textBoxPath7.Enabled;
            Properties.Settings.Default.Path8Enabled = textBoxPath8.Enabled;
            Properties.Settings.Default.Path9Enabled = textBoxPath9.Enabled;

            //18
            Properties.Settings.Default.KeepRunning1 = checkBoxKeepRunning1.Checked;
            Properties.Settings.Default.KeepRunning2 = checkBoxKeepRunning2.Checked;
            Properties.Settings.Default.KeepRunning3 = checkBoxKeepRunning3.Checked;
            Properties.Settings.Default.KeepRunning4 = checkBoxKeepRunning4.Checked;
            Properties.Settings.Default.KeepRunning5 = checkBoxKeepRunning5.Checked;
            Properties.Settings.Default.KeepRunning6 = checkBoxKeepRunning6.Checked;
            Properties.Settings.Default.KeepRunning7 = checkBoxKeepRunning7.Checked;
            Properties.Settings.Default.KeepRunning8 = checkBoxKeepRunning8.Checked;
            Properties.Settings.Default.KeepRunning9 = checkBoxKeepRunning9.Checked;

            /*
            try
            {
                //16
                if (pid1.Text.Length > 0) Properties.Settings.Default.Pid1 = Convert.ToInt32(pid1.Text);
                if (pid2.Text.Length > 0) Properties.Settings.Default.Pid2 = Convert.ToInt32(pid2.Text);
                if (pid3.Text.Length > 0) Properties.Settings.Default.Pid3 = Convert.ToInt32(pid3.Text);
                if (pid4.Text.Length > 0) Properties.Settings.Default.Pid4 = Convert.ToInt32(pid4.Text);
                if (pid5.Text.Length > 0) Properties.Settings.Default.Pid5 = Convert.ToInt32(pid5.Text);
                if (pid6.Text.Length > 0) Properties.Settings.Default.Pid6 = Convert.ToInt32(pid6.Text);
                if (pid7.Text.Length > 0) Properties.Settings.Default.Pid7 = Convert.ToInt32(pid7.Text);
                if (pid8.Text.Length > 0) Properties.Settings.Default.Pid8 = Convert.ToInt32(pid8.Text);
                if (pid9.Text.Length > 0) Properties.Settings.Default.Pid9 = Convert.ToInt32(pid9.Text);
            }
            catch (Exception)
            {

            }
            */
            Properties.Settings.Default.Save();
        }

        private bool SettingsLoad(String key)
        {
            var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

            if (!(File.Exists(userConfig)))
            {
                //MessageBox.Show("Config file key '" + key + "' not found:\nCreating new config user.config" + userConfig, "Error StartMe");
                return false;
            }
            if (key.Length == 0) key = "Default";
            settingsKeys = SettingsGetKeys();
            if (!settingsKeys.Contains(key, StringComparer.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("Key '" + key + "' not found in user.config", "Error StartMe");
                return false;
            }
            //if (key == "Default")
            //{
            //    Properties.Settings.Default.SettingsKey = "";
            //}
            //{
            //    Properties.Settings.Default.SettingsKey = key;
            //}
            Properties.Settings.Default.SettingsKey = key;
            Properties.Settings.Default.Reload();
            Properties.Settings.Default.SettingsKeyCurrent = key;

            //String s1 = Properties.Settings.Default.SettingsKey;
            //String s2 = Properties.Settings.Default.SettingsKeyCurrent;

            //0
            checkBoxMinimize.Checked = Properties.Settings.Default.Minimize;
            checkBoxStopAll.Checked = Properties.Settings.Default.CloseAll;
            checkBoxStartup.Checked = Properties.Settings.Default.Startup;

            //1
            textBoxPath1.Text = Properties.Settings.Default.Path1;
            if (textBoxPath1.Text.Contains("\r\n")) textBoxPath1.Text = "";
            textBoxPath2.Text = Properties.Settings.Default.Path2;
            if (textBoxPath2.Text.Contains("\r\n")) textBoxPath2.Text = "";
            textBoxPath3.Text = Properties.Settings.Default.Path3;
            if (textBoxPath3.Text.Contains("\r\n")) textBoxPath3.Text = "";
            textBoxPath4.Text = Properties.Settings.Default.Path4;
            if (textBoxPath4.Text.Contains("\r\n")) textBoxPath4.Text = "";
            textBoxPath5.Text = Properties.Settings.Default.Path5;
            if (textBoxPath5.Text.Contains("\r\n")) textBoxPath5.Text = "";
            textBoxPath6.Text = Properties.Settings.Default.Path6;
            if (textBoxPath6.Text.Contains("\r\n")) textBoxPath6.Text = "";
            textBoxPath7.Text = Properties.Settings.Default.Path7;
            if (textBoxPath7.Text.Contains("\r\n")) textBoxPath7.Text = "";
            textBoxPath8.Text = Properties.Settings.Default.Path8;
            if (textBoxPath8.Text.Contains("\r\n")) textBoxPath8.Text = "";
            textBoxPath9.Text = Properties.Settings.Default.Path9;
            if (textBoxPath9.Text.Contains("\r\n")) textBoxPath9.Text = "";

            //2
            textBoxArgs1.Text = Properties.Settings.Default.Args1;
            textBoxArgs2.Text = Properties.Settings.Default.Args2;
            textBoxArgs3.Text = Properties.Settings.Default.Args3;
            textBoxArgs4.Text = Properties.Settings.Default.Args4;
            textBoxArgs5.Text = Properties.Settings.Default.Args5;
            textBoxArgs6.Text = Properties.Settings.Default.Args6;
            textBoxArgs7.Text = Properties.Settings.Default.Args7;
            textBoxArgs8.Text = Properties.Settings.Default.Args8;
            textBoxArgs9.Text = Properties.Settings.Default.Args9;

            //3
            checkBoxAutoStart1.Checked = Properties.Settings.Default.AutoStart1;
            checkBoxAutoStart2.Checked = Properties.Settings.Default.AutoStart2;
            checkBoxAutoStart3.Checked = Properties.Settings.Default.AutoStart3;
            checkBoxAutoStart4.Checked = Properties.Settings.Default.AutoStart4;
            checkBoxAutoStart5.Checked = Properties.Settings.Default.AutoStart5;
            checkBoxAutoStart6.Checked = Properties.Settings.Default.AutoStart6;
            checkBoxAutoStart7.Checked = Properties.Settings.Default.AutoStart7;
            checkBoxAutoStart8.Checked = Properties.Settings.Default.AutoStart8;
            checkBoxAutoStart9.Checked = Properties.Settings.Default.AutoStart9;

            //4
            checkBoxMinimize1.Checked = Properties.Settings.Default.Minimize1;
            checkBoxMinimize2.Checked = Properties.Settings.Default.Minimize2;
            checkBoxMinimize3.Checked = Properties.Settings.Default.Minimize3;
            checkBoxMinimize4.Checked = Properties.Settings.Default.Minimize4;
            checkBoxMinimize5.Checked = Properties.Settings.Default.Minimize5;
            checkBoxMinimize6.Checked = Properties.Settings.Default.Minimize6;
            checkBoxMinimize7.Checked = Properties.Settings.Default.Minimize7;
            checkBoxMinimize8.Checked = Properties.Settings.Default.Minimize8;
            checkBoxMinimize9.Checked = Properties.Settings.Default.Minimize9;

            //5
            checkBoxAdmin1.Checked = Properties.Settings.Default.Admin1;
            checkBoxAdmin2.Checked = Properties.Settings.Default.Admin2;
            checkBoxAdmin3.Checked = Properties.Settings.Default.Admin3;
            checkBoxAdmin4.Checked = Properties.Settings.Default.Admin4;
            checkBoxAdmin5.Checked = Properties.Settings.Default.Admin5;
            checkBoxAdmin6.Checked = Properties.Settings.Default.Admin6;
            checkBoxAdmin7.Checked = Properties.Settings.Default.Admin7;
            checkBoxAdmin8.Checked = Properties.Settings.Default.Admin8;
            checkBoxAdmin9.Checked = Properties.Settings.Default.Admin9;

            //6
            comboBoxPriority1.Enabled = false;
            comboBoxPriority2.Enabled = false;
            comboBoxPriority3.Enabled = false;
            comboBoxPriority4.Enabled = false;
            comboBoxPriority5.Enabled = false;
            comboBoxPriority6.Enabled = false;
            comboBoxPriority7.Enabled = false;
            comboBoxPriority8.Enabled = false;
            comboBoxPriority9.Enabled = false;
            comboBoxPriority1.SelectedIndex = Properties.Settings.Default.Priority1;
            comboBoxPriority2.SelectedIndex = Properties.Settings.Default.Priority2;
            comboBoxPriority3.SelectedIndex = Properties.Settings.Default.Priority3;
            comboBoxPriority4.SelectedIndex = Properties.Settings.Default.Priority4;
            comboBoxPriority5.SelectedIndex = Properties.Settings.Default.Priority5;
            comboBoxPriority6.SelectedIndex = Properties.Settings.Default.Priority6;
            comboBoxPriority7.SelectedIndex = Properties.Settings.Default.Priority7;
            comboBoxPriority8.SelectedIndex = Properties.Settings.Default.Priority8;
            comboBoxPriority9.SelectedIndex = Properties.Settings.Default.Priority9;
            comboBoxPriority1.Enabled = true;
            comboBoxPriority2.Enabled = true;
            comboBoxPriority3.Enabled = true;
            comboBoxPriority4.Enabled = true;
            comboBoxPriority5.Enabled = true;
            comboBoxPriority6.Enabled = true;
            comboBoxPriority7.Enabled = true;
            comboBoxPriority8.Enabled = true;
            comboBoxPriority9.Enabled = true;

            //7
            textBoxEZName1.Text = Properties.Settings.Default.EZName1;
            textBoxEZName2.Text = Properties.Settings.Default.EZName2;
            textBoxEZName3.Text = Properties.Settings.Default.EZName3;
            textBoxEZName4.Text = Properties.Settings.Default.EZName4;
            textBoxEZName5.Text = Properties.Settings.Default.EZName5;
            textBoxEZName6.Text = Properties.Settings.Default.EZName6;
            textBoxEZName7.Text = Properties.Settings.Default.EZName7;
            textBoxEZName8.Text = Properties.Settings.Default.EZName8;
            textBoxEZName9.Text = Properties.Settings.Default.EZName9;

            //8
            checkBoxWinWait1.Checked = Properties.Settings.Default.WinWait1;
            checkBoxWinWait2.Checked = Properties.Settings.Default.WinWait2;
            checkBoxWinWait3.Checked = Properties.Settings.Default.WinWait3;
            checkBoxWinWait4.Checked = Properties.Settings.Default.WinWait4;
            checkBoxWinWait5.Checked = Properties.Settings.Default.WinWait5;
            checkBoxWinWait6.Checked = Properties.Settings.Default.WinWait6;
            checkBoxWinWait7.Checked = Properties.Settings.Default.WinWait7;
            checkBoxWinWait8.Checked = Properties.Settings.Default.WinWait8;
            checkBoxWinWait9.Checked = Properties.Settings.Default.WinWait9;
            //9
            numericUpDownCPU1.Value = Properties.Settings.Default.CPU1;
            numericUpDownCPU2.Value = Properties.Settings.Default.CPU2;
            numericUpDownCPU3.Value = Properties.Settings.Default.CPU3;
            numericUpDownCPU4.Value = Properties.Settings.Default.CPU4;
            numericUpDownCPU5.Value = Properties.Settings.Default.CPU5;
            numericUpDownCPU6.Value = Properties.Settings.Default.CPU6;
            numericUpDownCPU7.Value = Properties.Settings.Default.CPU7;
            numericUpDownCPU8.Value = Properties.Settings.Default.CPU8;
            numericUpDownCPU9.Value = Properties.Settings.Default.CPU9;

            //10
            textBoxStart1.Text = Properties.Settings.Default.KeysStart1;
            textBoxStart2.Text = Properties.Settings.Default.KeysStart2;
            textBoxStart3.Text = Properties.Settings.Default.KeysStart3;
            textBoxStart4.Text = Properties.Settings.Default.KeysStart4;
            textBoxStart5.Text = Properties.Settings.Default.KeysStart5;
            textBoxStart6.Text = Properties.Settings.Default.KeysStart6;
            textBoxStart7.Text = Properties.Settings.Default.KeysStart7;
            textBoxStart8.Text = Properties.Settings.Default.KeysStart8;
            textBoxStart9.Text = Properties.Settings.Default.KeysStart9;

            //11
            textBoxStart1Sequence.Text = Properties.Settings.Default.Start1Next;
            textBoxStart2Sequence.Text = Properties.Settings.Default.Start2Next;
            textBoxStart3Sequence.Text = Properties.Settings.Default.Start3Next;
            textBoxStart4Sequence.Text = Properties.Settings.Default.Start4Next;
            textBoxStart5Sequence.Text = Properties.Settings.Default.Start5Next;
            textBoxStart6Sequence.Text = Properties.Settings.Default.Start6Next;
            textBoxStart7Sequence.Text = Properties.Settings.Default.Start7Next;
            textBoxStart8Sequence.Text = Properties.Settings.Default.Start8Next;
            textBoxStart9Sequence.Text = Properties.Settings.Default.Start9Next;

            //12
            textBoxStart1Stop.Text = Properties.Settings.Default.Start1Stop;
            textBoxStart2Stop.Text = Properties.Settings.Default.Start2Stop;
            textBoxStart3Stop.Text = Properties.Settings.Default.Start3Stop;
            textBoxStart4Stop.Text = Properties.Settings.Default.Start4Stop;
            textBoxStart5Stop.Text = Properties.Settings.Default.Start5Stop;
            textBoxStart6Stop.Text = Properties.Settings.Default.Start6Stop;
            textBoxStart7Stop.Text = Properties.Settings.Default.Start7Stop;
            textBoxStart8Stop.Text = Properties.Settings.Default.Start8Stop;
            textBoxStart9Stop.Text = Properties.Settings.Default.Start9Stop;

            //13
            numericUpDownDelayStop1.Value = Properties.Settings.Default.StopDelay1;
            numericUpDownDelayStop2.Value = Properties.Settings.Default.StopDelay2;
            numericUpDownDelayStop3.Value = Properties.Settings.Default.StopDelay3;
            numericUpDownDelayStop4.Value = Properties.Settings.Default.StopDelay4;
            numericUpDownDelayStop5.Value = Properties.Settings.Default.StopDelay5;
            numericUpDownDelayStop6.Value = Properties.Settings.Default.StopDelay6;
            numericUpDownDelayStop7.Value = Properties.Settings.Default.StopDelay7;
            numericUpDownDelayStop8.Value = Properties.Settings.Default.StopDelay8;
            numericUpDownDelayStop9.Value = Properties.Settings.Default.StopDelay9;

            //14
            textBoxStop1.Text = Properties.Settings.Default.KeysStop1;
            textBoxStop2.Text = Properties.Settings.Default.KeysStop2;
            textBoxStop3.Text = Properties.Settings.Default.KeysStop3;
            textBoxStop4.Text = Properties.Settings.Default.KeysStop4;
            textBoxStop5.Text = Properties.Settings.Default.KeysStop5;
            textBoxStop6.Text = Properties.Settings.Default.KeysStop6;
            textBoxStop7.Text = Properties.Settings.Default.KeysStop7;
            textBoxStop8.Text = Properties.Settings.Default.KeysStop8;
            textBoxStop9.Text = Properties.Settings.Default.KeysStop9;

            //15
            checkBoxKill1.Checked = Properties.Settings.Default.Kill1;
            checkBoxKill2.Checked = Properties.Settings.Default.Kill2;
            checkBoxKill3.Checked = Properties.Settings.Default.Kill3;
            checkBoxKill4.Checked = Properties.Settings.Default.Kill4;
            checkBoxKill5.Checked = Properties.Settings.Default.Kill5;
            checkBoxKill6.Checked = Properties.Settings.Default.Kill6;
            checkBoxKill7.Checked = Properties.Settings.Default.Kill7;
            checkBoxKill8.Checked = Properties.Settings.Default.Kill8;
            checkBoxKill9.Checked = Properties.Settings.Default.Kill9;

            //16
            numericUpDownDelay1After.Value = Properties.Settings.Default.StartDelay1After;
            numericUpDownDelay2After.Value = Properties.Settings.Default.StartDelay2After;
            numericUpDownDelay3After.Value = Properties.Settings.Default.StartDelay3After;
            numericUpDownDelay4After.Value = Properties.Settings.Default.StartDelay4After;
            numericUpDownDelay5After.Value = Properties.Settings.Default.StartDelay5After;
            numericUpDownDelay6After.Value = Properties.Settings.Default.StartDelay6After;
            numericUpDownDelay7After.Value = Properties.Settings.Default.StartDelay7After;
            numericUpDownDelay8After.Value = Properties.Settings.Default.StartDelay8After;
            numericUpDownDelay9After.Value = Properties.Settings.Default.StartDelay9After;

            //17
            textBoxPath1.Enabled = buttonStart1.Enabled = Properties.Settings.Default.Path1Enabled;
            textBoxPath2.Enabled = buttonStart2.Enabled = Properties.Settings.Default.Path2Enabled;
            textBoxPath3.Enabled = buttonStart3.Enabled = Properties.Settings.Default.Path3Enabled;
            textBoxPath4.Enabled = buttonStart4.Enabled = Properties.Settings.Default.Path4Enabled;
            textBoxPath5.Enabled = buttonStart5.Enabled = Properties.Settings.Default.Path5Enabled;
            textBoxPath6.Enabled = buttonStart6.Enabled = Properties.Settings.Default.Path6Enabled;
            textBoxPath7.Enabled = buttonStart7.Enabled = Properties.Settings.Default.Path7Enabled;
            textBoxPath8.Enabled = buttonStart8.Enabled = Properties.Settings.Default.Path8Enabled;
            textBoxPath9.Enabled = buttonStart9.Enabled = Properties.Settings.Default.Path9Enabled;

            //18
            checkBoxKeepRunning1.Checked = Properties.Settings.Default.KeepRunning1;
            checkBoxKeepRunning2.Checked = Properties.Settings.Default.KeepRunning2;
            checkBoxKeepRunning3.Checked = Properties.Settings.Default.KeepRunning3;
            checkBoxKeepRunning4.Checked = Properties.Settings.Default.KeepRunning4;
            checkBoxKeepRunning5.Checked = Properties.Settings.Default.KeepRunning5;
            checkBoxKeepRunning6.Checked = Properties.Settings.Default.KeepRunning6;
            checkBoxKeepRunning7.Checked = Properties.Settings.Default.KeepRunning7;
            checkBoxKeepRunning8.Checked = Properties.Settings.Default.KeepRunning8;
            checkBoxKeepRunning9.Checked = Properties.Settings.Default.KeepRunning9;

            this.Activated += AfterLoading;
            return true;
        }

        void AfterLoading(object sender, EventArgs e)
        {
            this.Activated -= AfterLoading;
            if (autoStartDone == false && noAuto == false)
            {
                autoStartDone = true;
                if (checkBoxStartup.Checked)
                {
                    StartAllConfigs(true);
                }
            }
            timer1.Interval = 2000;
            timer1.Enabled = true;
        }

        private void SettingsAdd(string item)
        {
            if (settingsKeys.Contains(item))
            {
                String s1 = Properties.Settings.Default.SettingsKeyCurrent;
                //String s2 = Properties.Settings.Default.SettingsKey;
                if (!comboBoxSettingsKey.SelectedItem.Equals(s1))
                {
                    SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
                    SettingsLoad(item);
                    comboBoxSettingsKey.SelectedItem = item;
                }
                return;
            }
            if (MessageBox.Show("Do you want to add a new configuration called " + item + "?", "Config StartMe", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (item == "Default") item = "";
                Properties.Settings.Default.SettingsKeyCurrent = item;
                if (MessageBox.Show("Click OK to keep copy old task info to new copy, click Cancel to start with empty tasks", "Config StartMe", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    Properties.Settings.Default.Reset();
                }
                Properties.Settings.Default.Save();
                SettingsSave(item);
            }

        }
        private void ComboBoxSettingsName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Shift))
            {
                if (MessageBox.Show("Do you want to delete " + comboBoxSettingsKey.Text + "?", "Config Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MessageBox.Show("Delete not implemented yet");
                }
            }
            SettingsAdd(comboBoxSettingsKey.Text);
            Application.DoEvents();
            ProcessInit();
        }

        private void ComboBoxSettingsName_Leave(object sender, EventArgs e)
        {
            if (comboBoxSettingsKey.Text == "")
            {
                comboBoxSettingsKey.SelectedIndex = 0;
                comboBoxSettingsKey.Refresh();
                Application.DoEvents();
                SettingsLoad(comboBoxSettingsKey.Text);
                return;
            }

            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent); // save the old one
            String item = comboBoxSettingsKey.Text;
            if (!settingsKeys.Contains(item))
            {
                if (MessageBox.Show("Do you want to add a new configuration called '" + item + "'?", "Config StartMe", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Properties.Settings.Default.SettingsKeyCurrent = item;
                    if (MessageBox.Show("Do you want to reset the copy to default values?", "Config StartMe", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Properties.Settings.Default.Reset();
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        comboBoxSettingsKey.SelectedIndex = 0;
                        item = "Default";
                    }
                    SettingsLoad(item);
                    comboBoxSettingsKey.Items.Add(item);
                }
                else
                {
                    comboBoxSettingsKey.SelectedIndex = 0;
                    comboBoxSettingsKey.Refresh();
                    Application.DoEvents();
                    SettingsLoad(comboBoxSettingsKey.Text);
                }
            }
        }

        private void ComboBoxPriority1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBoxPriority1.SelectedIndex;
            Properties.Settings.Default.Priority1 = n;
        }

        private void ComboBoxPriority2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBoxPriority2.SelectedIndex;
            Properties.Settings.Default.Priority2 = n;

        }

        private void ComboBoxPriority3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBoxPriority3.SelectedIndex;
            Properties.Settings.Default.Priority3 = n;

        }

        private void ComboBoxPriority4_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBoxPriority4.SelectedIndex;
            Properties.Settings.Default.Priority4 = n;

        }

        private void ComboBoxPriority5_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBoxPriority5.SelectedIndex;
            Properties.Settings.Default.Priority5 = n;

        }

        private void ComboBoxPriority6_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBoxPriority6.SelectedIndex;
            Properties.Settings.Default.Priority6 = n;

        }

        private void ComboBoxPriority7_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBoxPriority7.SelectedIndex;
            Properties.Settings.Default.Priority7 = n;

        }

        private void ComboBoxPriority8_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBoxPriority8.SelectedIndex;
            Properties.Settings.Default.Priority8 = n;

        }

        private void ComboBoxPriority9_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBoxPriority9.SelectedIndex;
            Properties.Settings.Default.Priority9 = n;
        }

        private void CheckBoxAdmin1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Admin1 = checkBoxAdmin1.Checked;
        }

        private void CheckBoxAdmin2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Admin2 = checkBoxAdmin2.Checked;
        }

        private void CheckBoxAdmin3_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Admin3 = checkBoxAdmin3.Checked;
        }

        private void CheckBoxAdmin4_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Admin4 = checkBoxAdmin4.Checked;
        }

        private void CheckBoxAdmin5_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Admin5 = checkBoxAdmin5.Checked;
        }

        private void CheckBoxAdmin6_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Admin6 = checkBoxAdmin6.Checked;
        }

        private void CheckBoxAdmin7_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Admin7 = checkBoxAdmin7.Checked;
        }

        private void CheckBoxAdmin8_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Admin8 = checkBoxAdmin8.Checked;
        }

        private void CheckBoxAdmin9_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Admin9 = checkBoxAdmin9.Checked;
        }
        private bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
                MessageBox.Show("Here1", "Debug StartMe");
            }
            catch (Exception)
            {
                isAdmin = false;
                MessageBox.Show("Here2", "Debug StartMe");
            }
            return isAdmin;
        }

        private void Pid1_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }

        private void Pid2_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }

        private void Pid3_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }

        private void Pid4_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }

        private void Pid5_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }

        private void Pid6_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }

        private void Pid7_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }

        private void Pid8_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }

        private void Pid9_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }

        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            Help.Show();
        }

        private void TextBoxStart1Next_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }

        private void TextBoxStart1Stop_TextChanged(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
        }
        #region Helper Functions for Admin Privileges and Elevation Status 

        /// <summary> 
        /// The function checks whether the primary access token of the process belongs  
        /// to user account that is a member of the local Administrators group, even if  
        /// it currently is not elevated. 
        /// </summary> 
        /// <returns> 
        /// Returns true if the primary access token of the process belongs to user  
        /// account that is a member of the local Administrators group. Returns false  
        /// if the token does not. 
        /// </returns> 
        /// <exception cref="System.ComponentModel.Win32Exception"> 
        /// When any native Windows API call fails, the function throws a Win32Exception  
        /// with the last error code. 
        /// </exception> 
        internal bool IsUserInAdminGroup()
        {
            bool fInAdminGroup = false;
            SafeTokenHandle hToken = null;
            SafeTokenHandle hTokenToCheck = null;
            IntPtr pElevationType = IntPtr.Zero;
            IntPtr pLinkedToken = IntPtr.Zero;
#pragma warning disable IDE0059 // Value assigned to symbol is never used
            int cbSize = 0;
#pragma warning restore IDE0059 // Value assigned to symbol is never used

            try
            {
                // Open the access token of the current process for query and duplicate. 
                if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethods.TOKEN_QUERY | NativeMethods.TOKEN_DUPLICATE, out hToken))
                {
                    throw new Win32Exception();
                }

                // Determine whether system is running Windows Vista or later operating  
                // systems (major version >= 6) because they support linked tokens, but  
                // previous versions (major version < 6) do not. 
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    // Running Windows Vista or later (major version >= 6).  
                    // Determine token type: limited, elevated, or default.  

                    // Allocate a buffer for the elevation type information. 
                    cbSize = sizeof(TOKEN_ELEVATION_TYPE);
                    pElevationType = Marshal.AllocHGlobal(cbSize);
                    if (pElevationType == IntPtr.Zero)
                    {
                        throw new Win32Exception();
                    }

                    // Retrieve token elevation type information. 
                    if (!NativeMethods.GetTokenInformation(hToken,
                        TOKEN_INFORMATION_CLASS.TokenElevationType, pElevationType,
                        cbSize, out cbSize))
                    {
                        throw new Win32Exception();
                    }

                    // Marshal the TOKEN_ELEVATION_TYPE enum from native to .NET. 
                    TOKEN_ELEVATION_TYPE elevType = (TOKEN_ELEVATION_TYPE)
                        Marshal.ReadInt32(pElevationType);

                    // If limited, get the linked elevated token for further check. 
                    if (elevType == TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
                    {
                        // Allocate a buffer for the linked token. 
                        cbSize = IntPtr.Size;
                        pLinkedToken = Marshal.AllocHGlobal(cbSize);
                        if (pLinkedToken == IntPtr.Zero)
                        {
                            throw new Win32Exception();
                        }

                        // Get the linked token. 
                        if (!NativeMethods.GetTokenInformation(hToken,
                            TOKEN_INFORMATION_CLASS.TokenLinkedToken, pLinkedToken,
                            cbSize, out cbSize))
                        {
                            throw new Win32Exception();
                        }

                        // Marshal the linked token value from native to .NET. 
                        IntPtr hLinkedToken = Marshal.ReadIntPtr(pLinkedToken);
                        hTokenToCheck = new SafeTokenHandle(hLinkedToken);
                    }
                }

                // CheckTokenMembership requires an impersonation token. If we just got  
                // a linked token, it already is an impersonation token.  If we did not  
                // get a linked token, duplicate the original into an impersonation  
                // token for CheckTokenMembership. 
                if (hTokenToCheck == null)
                {
                    if (!NativeMethods.DuplicateToken(hToken,
                        SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                        out hTokenToCheck))
                    {
                        throw new Win32Exception();
                    }
                }

                // Check if the token to be checked contains admin SID. 
                WindowsIdentity id = new WindowsIdentity(hTokenToCheck.DangerousGetHandle());
                WindowsPrincipal principal = new WindowsPrincipal(id);
                fInAdminGroup = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            finally
            {
                // Centralized cleanup for all allocated resources.  
                if (hToken != null)
                {
                    hToken.Close();
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    hToken = null;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
                if (hTokenToCheck != null)
                {
                    hTokenToCheck.Close();
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    hTokenToCheck = null;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
                if (pElevationType != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pElevationType);
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    pElevationType = IntPtr.Zero;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
                if (pLinkedToken != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pLinkedToken);
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    pLinkedToken = IntPtr.Zero;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
            }

            return fInAdminGroup;
        }


        /// <summary> 
        /// The function checks whether the current process is run as administrator. 
        /// In other words, it dictates whether the primary access token of the  
        /// process belongs to user account that is a member of the local  
        /// Administrators group and it is elevated. 
        /// </summary> 
        /// <returns> 
        /// Returns true if the primary access token of the process belongs to user  
        /// account that is a member of the local Administrators group and it is  
        /// elevated. Returns false if the token does not. 
        /// </returns> 
        internal bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }


        /// <summary> 
        /// The function gets the elevation information of the current process. It  
        /// dictates whether the process is elevated or not. Token elevation is only  
        /// available on Windows Vista and newer operating systems, thus  
        /// IsProcessElevated throws a C++ exception if it is called on systems prior  
        /// to Windows Vista. It is not appropriate to use this function to determine  
        /// whether a process is run as administartor. 
        /// </summary> 
        /// <returns> 
        /// Returns true if the process is elevated. Returns false if it is not. 
        /// </returns> 
        /// <exception cref="System.ComponentModel.Win32Exception"> 
        /// When any native Windows API call fails, the function throws a Win32Exception  
        /// with the last error code. 
        /// </exception> 
        /// <remarks> 
        /// TOKEN_INFORMATION_CLASS provides TokenElevationType to check the elevation  
        /// type (TokenElevationTypeDefault / TokenElevationTypeLimited /  
        /// TokenElevationTypeFull) of the process. It is different from TokenElevation  
        /// in that, when UAC is turned off, elevation type always returns  
        /// TokenElevationTypeDefault even though the process is elevated (Integrity  
        /// Level == High). In other words, it is not safe to say if the process is  
        /// elevated based on elevation type. Instead, we should use TokenElevation.  
        /// </remarks> 
        internal bool IsProcessElevated()
        {
            bool fIsElevated = false;
            SafeTokenHandle hToken = null;
#pragma warning disable IDE0059 // Value assigned to symbol is never used
            int cbTokenElevation = 0;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
            IntPtr pTokenElevation = IntPtr.Zero;

            try
            {
                // Open the access token of the current process with TOKEN_QUERY. 
                if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethods.TOKEN_QUERY, out hToken))
                {
                    throw new Win32Exception();
                }

                // Allocate a buffer for the elevation information. 
                cbTokenElevation = Marshal.SizeOf(typeof(TOKEN_ELEVATION));
                pTokenElevation = Marshal.AllocHGlobal(cbTokenElevation);
                if (pTokenElevation == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                // Retrieve token elevation information. 
                if (!NativeMethods.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenElevation, pTokenElevation,
                    cbTokenElevation, out cbTokenElevation))
                {
                    // When the process is run on operating systems prior to Windows  
                    // Vista, GetTokenInformation returns false with the error code  
                    // ERROR_INVALID_PARAMETER because TokenElevation is not supported  
                    // on those operating systems. 
                    throw new Win32Exception();
                }

                // Marshal the TOKEN_ELEVATION struct from native to .NET object. 
                TOKEN_ELEVATION elevation = (TOKEN_ELEVATION)Marshal.PtrToStructure(
                    pTokenElevation, typeof(TOKEN_ELEVATION));

                // TOKEN_ELEVATION.TokenIsElevated is a non-zero value if the token  
                // has elevated privileges; otherwise, a zero value. 
                fIsElevated = (elevation.TokenIsElevated != 0);
            }
            finally
            {
                // Centralized cleanup for all allocated resources.  
                if (hToken != null)
                {
                    hToken.Close();
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    hToken = null;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
                if (pTokenElevation != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTokenElevation);
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    pTokenElevation = IntPtr.Zero;
                    cbTokenElevation = 0;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
            }

            return fIsElevated;
        }


        /// <summary> 
        /// The function gets the integrity level of the current process. Integrity  
        /// level is only available on Windows Vista and newer operating systems, thus  
        /// GetProcessIntegrityLevel throws a C++ exception if it is called on systems  
        /// prior to Windows Vista. 
        /// </summary> 
        /// <returns> 
        /// Returns the integrity level of the current process. It is usually one of  
        /// these values: 
        ///  
        ///    SECURITY_MANDATORY_UNTRUSTED_RID - means untrusted level. It is used  
        ///    by processes started by the Anonymous group. Blocks most write access. 
        ///    (SID: S-1-16-0x0) 
        ///     
        ///    SECURITY_MANDATORY_LOW_RID - means low integrity level. It is used by 
        ///    Protected Mode Internet Explorer. Blocks write acess to most objects  
        ///    (such as files and registry keys) on the system. (SID: S-1-16-0x1000) 
        ///  
        ///    SECURITY_MANDATORY_MEDIUM_RID - means medium integrity level. It is  
        ///    used by normal applications being launched while UAC is enabled.  
        ///    (SID: S-1-16-0x2000) 
        ///     
        ///    SECURITY_MANDATORY_HIGH_RID - means high integrity level. It is used  
        ///    by administrative applications launched through elevation when UAC is  
        ///    enabled, or normal applications if UAC is disabled and the user is an  
        ///    administrator. (SID: S-1-16-0x3000) 
        ///     
        ///    SECURITY_MANDATORY_SYSTEM_RID - means system integrity level. It is  
        ///    used by services and other system-level applications (such as Wininit,  
        ///    Winlogon, Smss, etc.)  (SID: S-1-16-0x4000) 
        ///  
        /// </returns> 
        /// <exception cref="System.ComponentModel.Win32Exception"> 
        /// When any native Windows API call fails, the function throws a Win32Exception  
        /// with the last error code. 
        /// </exception> 
        internal int GetProcessIntegrityLevel()
        {
            int IL = -1;
            SafeTokenHandle hToken = null;
#pragma warning disable IDE0059 // Value assigned to symbol is never used
            int cbTokenIL = 0;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
            IntPtr pTokenIL = IntPtr.Zero;

            try
            {
                // Open the access token of the current process with TOKEN_QUERY. 
                if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethods.TOKEN_QUERY, out hToken))
                {
                    throw new Win32Exception();
                }

                // Then we must query the size of the integrity level information  
                // associated with the token. Note that we expect GetTokenInformation  
                // to return false with the ERROR_INSUFFICIENT_BUFFER error code  
                // because we've given it a null buffer. On exit cbTokenIL will tell  
                // the size of the group information. 
                if (!NativeMethods.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, 0,
                    out cbTokenIL))
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != NativeMethods.ERROR_INSUFFICIENT_BUFFER)
                    {
                        // When the process is run on operating systems prior to  
                        // Windows Vista, GetTokenInformation returns false with the  
                        // ERROR_INVALID_PARAMETER error code because  
                        // TokenIntegrityLevel is not supported on those OS's. 
                        throw new Win32Exception(error);
                    }
                }

                // Now we allocate a buffer for the integrity level information. 
                pTokenIL = Marshal.AllocHGlobal(cbTokenIL);
                if (pTokenIL == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                // Now we ask for the integrity level information again. This may fail  
                // if an administrator has added this account to an additional group  
                // between our first call to GetTokenInformation and this one. 
                if (!NativeMethods.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTokenIL, cbTokenIL,
                    out cbTokenIL))
                {
                    throw new Win32Exception();
                }

                // Marshal the TOKEN_MANDATORY_LABEL struct from native to .NET object. 
                TOKEN_MANDATORY_LABEL tokenIL = (TOKEN_MANDATORY_LABEL)
                    Marshal.PtrToStructure(pTokenIL, typeof(TOKEN_MANDATORY_LABEL));

                // Integrity Level SIDs are in the form of S-1-16-0xXXXX. (e.g.  
                // S-1-16-0x1000 stands for low integrity level SID). There is one  
                // and only one subauthority. 
                IntPtr pIL = NativeMethods.GetSidSubAuthority(tokenIL.Label.Sid, 0);
                IL = Marshal.ReadInt32(pIL);
            }
            finally
            {
                // Centralized cleanup for all allocated resources.  
                if (hToken != null)
                {
                    hToken.Close();
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    hToken = null;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
                if (pTokenIL != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTokenIL);
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    pTokenIL = IntPtr.Zero;
                    cbTokenIL = 0;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
            }

            return IL;
        }

        #endregion

        /*
        public MainForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            // Get and display whether the primary access token of the process belongs  
            // to user account that is a member of the local Administrators group even  
            // if it currently is not elevated (IsUserInAdminGroup). 
            try
            {
                bool fInAdminGroup = IsUserInAdminGroup();
                this.lbInAdminGroup.Text = fInAdminGroup.ToString();
            }
            catch (Exception ex)
            {
                this.lbInAdminGroup.Text = "N/A";
                MessageBox.Show(ex.Message, "An error occurred in IsUserInAdminGroup", "Error StartMe",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Get and display whether the process is run as administrator or not  
            // (IsRunAsAdmin). 
            try
            {
                bool fIsRunAsAdmin = IsRunAsAdmin();
                this.lbIsRunAsAdmin.Text = fIsRunAsAdmin.ToString();
            }
            catch (Exception ex)
            {
                this.lbIsRunAsAdmin.Text = "N/A";
                MessageBox.Show(ex.Message, "An error occurred in IsRunAsAdmin", "Error StartMe",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            // Get and display the process elevation information (IsProcessElevated)  
            // and integrity level (GetProcessIntegrityLevel). The information is not  
            // available on operating systems prior to Windows Vista. 
            if (Environment.OSVersion.Version.Major >= 6)
            {
                // Running Windows Vista or later (major version >= 6).  

                try
                {
                    // Get and display the process elevation information. 
                    bool fIsElevated = IsProcessElevated();
                    this.lbIsElevated.Text = fIsElevated.ToString();

                    // Update the Self-elevate button to show the UAC shield icon on  
                    // the UI if the process is not elevated. 
                    this.btnElevate.FlatStyle = FlatStyle.System;
                    NativeMethods.SendMessage(btnElevate.Handle,
                        NativeMethods.BCM_SETSHIELD, 0,
                        fIsElevated ? IntPtr.Zero : (IntPtr)1);
                }
                catch (Exception ex)
                {
                    this.lbIsElevated.Text = "N/A";
                    MessageBox.Show(ex.Message, "An error occurred in IsProcessElevated", "Error StartMe",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    // Get and display the process integrity level. 
                    int IL = GetProcessIntegrityLevel();
                    switch (IL)
                    {
                        case NativeMethods.SECURITY_MANDATORY_UNTRUSTED_RID:
                            this.lbIntegrityLevel.Text = "Untrusted"; break;
                        case NativeMethods.SECURITY_MANDATORY_LOW_RID:
                            this.lbIntegrityLevel.Text = "Low"; break;
                        case NativeMethods.SECURITY_MANDATORY_MEDIUM_RID:
                            this.lbIntegrityLevel.Text = "Medium"; break;
                        case NativeMethods.SECURITY_MANDATORY_HIGH_RID:
                            this.lbIntegrityLevel.Text = "High"; break;
                        case NativeMethods.SECURITY_MANDATORY_SYSTEM_RID:
                            this.lbIntegrityLevel.Text = "System"; break;
                        default:
                            this.lbIntegrityLevel.Text = "Unknown"; break;
                    }
                }
                catch (Exception ex)
                {
                    this.lbIntegrityLevel.Text = "N/A";
                    MessageBox.Show(ex.Message, "An error occurred in GetProcessIntegrityLevel", "Error StartMe",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                this.lbIsElevated.Text = "N/A";
                this.lbIntegrityLevel.Text = "N/A";
            }
        }
        */
#pragma warning disable IDE0051 // Remove unused private members
        private bool ElevateMe()
#pragma warning restore IDE0051 // Remove unused private members
        {
            // Elevate the process if it is not run as administrator. 
            if (!IsRunAsAdmin())
            {
                // Launch itself as administrator 
                ProcessStartInfo proc = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Application.ExecutablePath,
                    Verb = "runas"
                };

                try
                {
                    Process.Start(proc);
                }
                catch
                {
                    // The user refused the elevation. 
                    // Do nothing and return directly ... 
                    return false;
                }
                return true;
                //this.Application.Exit();  // Quit itself 
            }
            return false;
        }

        private void CheckBoxAutoStart1_CheckedChanged(object sender, EventArgs e)
        {
            //AutoStartUpdate();
        }

        private void CheckBoxAutoStart2_CheckedChanged(object sender, EventArgs e)
        {
            //AutoStartUpdate();
        }

        private void CheckBoxAutoStart3_CheckedChanged(object sender, EventArgs e)
        {
            //AutoStartUpdate();
        }

        private void CheckBoxAutoStart4_CheckedChanged(object sender, EventArgs e)
        {
            //AutoStartUpdate();
        }

        private void CheckBoxAutoStart5_CheckedChanged(object sender, EventArgs e)
        {
            //AutoStartUpdate();
        }

        private void CheckBoxAutoStart6_CheckedChanged(object sender, EventArgs e)
        {
            //AutoStartUpdate();
        }

        private void CheckBoxAutoStart7_CheckedChanged(object sender, EventArgs e)
        {
            //AutoStartUpdate();
        }

        private void CheckBoxAutoStart8_CheckedChanged(object sender, EventArgs e)
        {
            //AutoStartUpdate();
        }

        private void CheckBoxAutoStart9_CheckedChanged(object sender, EventArgs e)
        {
            //AutoStartUpdate();
        }

        private bool CheckStartSeqOK()
        {
            for (int n = 1; n <= 9; ++n)
            {
                if (!GetStartSequence(n).Equals(""))
                {
                    MessageBox.Show("If one Start box has a number they all must have numbers!!", "Error StartMe");
                    return false;
                }
            }
            return true;
        }
        private void ButtonStartAll_Click(object sender, EventArgs e)
        {
            if (!CheckStartSeqOK()) return;
            timer1.Enabled = false;
            if (ModifierKeys.HasFlag(Keys.Shift))
            {
                StartAllConfigs(true);
            }
            else
            {
                StartAll();
            }
            timer1.Enabled = true;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.F12)
            {
                var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                MessageBox.Show("user.config = " + userConfig, "Debug StartMe");
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            Properties.Settings.Default.RestoreBounds = this.Bounds;
            Properties.Settings.Default.Save();
        }

        private void ComboBoxSettingsKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                MessageBox.Show("Delete key not implemented yet", "Error StartMe");
                //if (MessageBox.Show("Do you want to delete this setting?", "StartMe Settings", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{
                //SettingsDelete(comboBoxSettingsKey.Text);
                //}

            }
            else if (e.KeyCode == Keys.Enter)
            {
                SettingsAdd(comboBoxSettingsKey.Text);
            }
        }

        private void TextBoxPath1_MouseEnter(object sender, EventArgs e)
        {
            selectedTask = 1;
        }

        private void TextBoxPath2_MouseEnter(object sender, EventArgs e)
        {
            selectedTask = 2;
        }

        private void TextBoxPath3_MouseEnter(object sender, EventArgs e)
        {
            selectedTask = 3;
        }

        private void TextBoxPath4_MouseEnter(object sender, EventArgs e)
        {
            selectedTask = 4;
        }

        private void TextBoxPath5_MouseEnter(object sender, EventArgs e)
        {
            selectedTask = 5;
        }

        private void TextBoxPath6_MouseEnter(object sender, EventArgs e)
        {
            selectedTask = 6;
        }

        private void TextBoxPath7_MouseEnter(object sender, EventArgs e)
        {
            selectedTask = 7;
        }

        private void TextBoxPath8_MouseEnter(object sender, EventArgs e)
        {
            selectedTask = 8;
        }

        private void TextBoxPath9_MouseEnter(object sender, EventArgs e)
        {
            selectedTask = 9;
        }

        void SwapTaskPath(int swap1, int swap2)
        {
            TextBox text1 = null, text2 = null;
            switch (swap1)
            {
                case 1:
                    text1 = textBoxPath1; break;
                case 2:
                    text1 = textBoxPath2; break;
                case 3:
                    text1 = textBoxPath3; break;
                case 4:
                    text1 = textBoxPath4; break;
                case 5:
                    text1 = textBoxPath5; break;
                case 6:
                    text1 = textBoxPath6; break;
                case 7:
                    text1 = textBoxPath7; break;
                case 8:
                    text1 = textBoxPath8; break;
                case 9:
                    text1 = textBoxPath9; break;
            }
            switch (swap2)
            {
                case 1:
                    text2 = textBoxPath1; break;
                case 2:
                    text2 = textBoxPath2; break;
                case 3:
                    text2 = textBoxPath3; break;
                case 4:
                    text2 = textBoxPath4; break;
                case 5:
                    text2 = textBoxPath5; break;
                case 6:
                    text2 = textBoxPath6; break;
                case 7:
                    text2 = textBoxPath7; break;
                case 8:
                    text2 = textBoxPath8; break;
                case 9:
                    text2 = textBoxPath9; break;

            }
            string s = text1.Text;
            text1.Text = text2.Text;
            text2.Text = s;
        }
        void SwapTaskParameters(int swap1, int swap2)
        {
            TextBox text1 = null, text2 = null;
            switch (swap1)
            {
                case 1:
                    text1 = textBoxArgs1; break;
                case 2:
                    text1 = textBoxArgs2; break;
                case 3:
                    text1 = textBoxArgs3; break;
                case 4:
                    text1 = textBoxArgs4; break;
                case 5:
                    text1 = textBoxArgs5; break;
                case 6:
                    text1 = textBoxArgs6; break;
                case 7:
                    text1 = textBoxArgs7; break;
                case 8:
                    text1 = textBoxArgs8; break;
                case 9:
                    text1 = textBoxArgs9; break;
            }
            switch (swap2)
            {
                case 1:
                    text2 = textBoxArgs1; break;
                case 2:
                    text2 = textBoxArgs2; break;
                case 3:
                    text2 = textBoxArgs3; break;
                case 4:
                    text2 = textBoxArgs4; break;
                case 5:
                    text2 = textBoxArgs5; break;
                case 6:
                    text2 = textBoxArgs6; break;
                case 7:
                    text2 = textBoxArgs7; break;
                case 8:
                    text2 = textBoxArgs8; break;
                case 9:
                    text2 = textBoxArgs9; break;

            }
            string s = text1.Text;
            text1.Text = text2.Text;
            text2.Text = s;
        }

        void SwapTaskAuto(int swap1, int swap2)
        {
            CheckBox box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = checkBoxAutoStart1; break;
                case 2:
                    box1 = checkBoxAutoStart2; break;
                case 3:
                    box1 = checkBoxAutoStart3; break;
                case 4:
                    box1 = checkBoxAutoStart4; break;
                case 5:
                    box1 = checkBoxAutoStart5; break;
                case 6:
                    box1 = checkBoxAutoStart6; break;
                case 7:
                    box1 = checkBoxAutoStart7; break;
                case 8:
                    box1 = checkBoxAutoStart8; break;
                case 9:
                    box1 = checkBoxAutoStart9; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = checkBoxAutoStart1; break;
                case 2:
                    box2 = checkBoxAutoStart2; break;
                case 3:
                    box2 = checkBoxAutoStart3; break;
                case 4:
                    box2 = checkBoxAutoStart4; break;
                case 5:
                    box2 = checkBoxAutoStart5; break;
                case 6:
                    box2 = checkBoxAutoStart6; break;
                case 7:
                    box2 = checkBoxAutoStart7; break;
                case 8:
                    box2 = checkBoxAutoStart8; break;
                case 9:
                    box2 = checkBoxAutoStart9; break;

            }
            bool b = box1.Checked;
            box1.Checked = box2.Checked;
            box2.Checked = b;

        }
        void SwapTaskMinimize(int swap1, int swap2)
        {
            CheckBox box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = checkBoxMinimize1; break;
                case 2:
                    box1 = checkBoxMinimize2; break;
                case 3:
                    box1 = checkBoxMinimize3; break;
                case 4:
                    box1 = checkBoxMinimize4; break;
                case 5:
                    box1 = checkBoxMinimize5; break;
                case 6:
                    box1 = checkBoxMinimize6; break;
                case 7:
                    box1 = checkBoxMinimize7; break;
                case 8:
                    box1 = checkBoxMinimize8; break;
                case 9:
                    box1 = checkBoxMinimize9; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = checkBoxMinimize1; break;
                case 2:
                    box2 = checkBoxMinimize2; break;
                case 3:
                    box2 = checkBoxMinimize3; break;
                case 4:
                    box2 = checkBoxMinimize4; break;
                case 5:
                    box2 = checkBoxMinimize5; break;
                case 6:
                    box2 = checkBoxMinimize6; break;
                case 7:
                    box2 = checkBoxMinimize7; break;
                case 8:
                    box2 = checkBoxMinimize8; break;
                case 9:
                    box2 = checkBoxMinimize9; break;

            }
            bool b = box1.Checked;
            box1.Checked = box2.Checked;
            box2.Checked = b;

        }
        void SwapTaskAdmin(int swap1, int swap2)
        {
            CheckBox box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = checkBoxAdmin1; break;
                case 2:
                    box1 = checkBoxAdmin2; break;
                case 3:
                    box1 = checkBoxAdmin3; break;
                case 4:
                    box1 = checkBoxAdmin4; break;
                case 5:
                    box1 = checkBoxAdmin5; break;
                case 6:
                    box1 = checkBoxAdmin6; break;
                case 7:
                    box1 = checkBoxAdmin7; break;
                case 8:
                    box1 = checkBoxAdmin8; break;
                case 9:
                    box1 = checkBoxAdmin9; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = checkBoxAdmin1; break;
                case 2:
                    box2 = checkBoxAdmin2; break;
                case 3:
                    box2 = checkBoxAdmin3; break;
                case 4:
                    box2 = checkBoxAdmin4; break;
                case 5:
                    box2 = checkBoxAdmin5; break;
                case 6:
                    box2 = checkBoxAdmin6; break;
                case 7:
                    box2 = checkBoxAdmin7; break;
                case 8:
                    box2 = checkBoxAdmin8; break;
                case 9:
                    box2 = checkBoxAdmin9; break;

            }
            bool b = box1.Checked;
            box1.Checked = box2.Checked;
            box2.Checked = b;

        }
        void SwapTaskPriority(int swap1, int swap2)
        {
            ComboBox box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = comboBoxPriority1; break;
                case 2:
                    box1 = comboBoxPriority2; break;
                case 3:
                    box1 = comboBoxPriority3; break;
                case 4:
                    box1 = comboBoxPriority4; break;
                case 5:
                    box1 = comboBoxPriority5; break;
                case 6:
                    box1 = comboBoxPriority6; break;
                case 7:
                    box1 = comboBoxPriority7; break;
                case 8:
                    box1 = comboBoxPriority8; break;
                case 9:
                    box1 = comboBoxPriority9; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = comboBoxPriority1; break;
                case 2:
                    box2 = comboBoxPriority2; break;
                case 3:
                    box2 = comboBoxPriority3; break;
                case 4:
                    box2 = comboBoxPriority4; break;
                case 5:
                    box2 = comboBoxPriority5; break;
                case 6:
                    box2 = comboBoxPriority6; break;
                case 7:
                    box2 = comboBoxPriority7; break;
                case 8:
                    box2 = comboBoxPriority8; break;
                case 9:
                    box2 = comboBoxPriority9; break;

            }
            int i = box1.SelectedIndex;
            box1.SelectedIndex = box2.SelectedIndex;
            box2.SelectedIndex = i;

        }

        void SwapTaskEzName(int swap1, int swap2)
        {
            TextBox text1 = null, text2 = null;
            switch (swap1)
            {
                case 1:
                    text1 = textBoxEZName1; break;
                case 2:
                    text1 = textBoxEZName2; break;
                case 3:
                    text1 = textBoxEZName3; break;
                case 4:
                    text1 = textBoxEZName4; break;
                case 5:
                    text1 = textBoxEZName5; break;
                case 6:
                    text1 = textBoxEZName6; break;
                case 7:
                    text1 = textBoxEZName7; break;
                case 8:
                    text1 = textBoxEZName8; break;
                case 9:
                    text1 = textBoxEZName9; break;
            }
            switch (swap2)
            {
                case 1:
                    text2 = textBoxEZName1; break;
                case 2:
                    text2 = textBoxEZName2; break;
                case 3:
                    text2 = textBoxEZName3; break;
                case 4:
                    text2 = textBoxEZName4; break;
                case 5:
                    text2 = textBoxEZName5; break;
                case 6:
                    text2 = textBoxEZName6; break;
                case 7:
                    text2 = textBoxEZName7; break;
                case 8:
                    text2 = textBoxEZName8; break;
                case 9:
                    text2 = textBoxEZName9; break;

            }
            string s = text1.Text;
            text1.Text = text2.Text;
            text2.Text = s;
        }
        void SwapTaskWinWait(int swap1, int swap2)
        {
            CheckBox box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = checkBoxWinWait1; break;
                case 2:
                    box1 = checkBoxWinWait2; break;
                case 3:
                    box1 = checkBoxWinWait3; break;
                case 4:
                    box1 = checkBoxWinWait4; break;
                case 5:
                    box1 = checkBoxWinWait5; break;
                case 6:
                    box1 = checkBoxWinWait6; break;
                case 7:
                    box1 = checkBoxWinWait7; break;
                case 8:
                    box1 = checkBoxWinWait8; break;
                case 9:
                    box1 = checkBoxWinWait9; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = checkBoxWinWait1; break;
                case 2:
                    box2 = checkBoxWinWait2; break;
                case 3:
                    box2 = checkBoxWinWait3; break;
                case 4:
                    box2 = checkBoxWinWait4; break;
                case 5:
                    box2 = checkBoxWinWait5; break;
                case 6:
                    box2 = checkBoxWinWait6; break;
                case 7:
                    box2 = checkBoxWinWait7; break;
                case 8:
                    box2 = checkBoxWinWait8; break;
                case 9:
                    box2 = checkBoxWinWait9; break;

            }
            bool b = box1.Checked;
            box1.Checked = box2.Checked;
            box2.Checked = b;
        }
        void SwapTaskDelayAfter(int swap1, int swap2)
        {
            NumericUpDown box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = numericUpDownDelay1After; break;
                case 2:
                    box1 = numericUpDownDelay2After; break;
                case 3:
                    box1 = numericUpDownDelay3After; break;
                case 4:
                    box1 = numericUpDownDelay4After; break;
                case 5:
                    box1 = numericUpDownDelay5After; break;
                case 6:
                    box1 = numericUpDownDelay6After; break;
                case 7:
                    box1 = numericUpDownDelay7After; break;
                case 8:
                    box1 = numericUpDownDelay8After; break;
                case 9:
                    box1 = numericUpDownDelay9After; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = numericUpDownDelay1After; break;
                case 2:
                    box2 = numericUpDownDelay2After; break;
                case 3:
                    box2 = numericUpDownDelay3After; break;
                case 4:
                    box2 = numericUpDownDelay4After; break;
                case 5:
                    box2 = numericUpDownDelay5After; break;
                case 6:
                    box2 = numericUpDownDelay6After; break;
                case 7:
                    box2 = numericUpDownDelay7After; break;
                case 8:
                    box2 = numericUpDownDelay8After; break;
                case 9:
                    box2 = numericUpDownDelay9After; break;

            }
            decimal d = box1.Value;
            box1.Value = box2.Value;
            box2.Value = d;
        }
        void SwapTaskCPU(int swap1, int swap2)
        {
            NumericUpDown box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = numericUpDownCPU1; break;
                case 2:
                    box1 = numericUpDownCPU2; break;
                case 3:
                    box1 = numericUpDownCPU3; break;
                case 4:
                    box1 = numericUpDownCPU4; break;
                case 5:
                    box1 = numericUpDownCPU5; break;
                case 6:
                    box1 = numericUpDownCPU6; break;
                case 7:
                    box1 = numericUpDownCPU7; break;
                case 8:
                    box1 = numericUpDownCPU8; break;
                case 9:
                    box1 = numericUpDownCPU9; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = numericUpDownCPU1; break;
                case 2:
                    box2 = numericUpDownCPU2; break;
                case 3:
                    box2 = numericUpDownCPU3; break;
                case 4:
                    box2 = numericUpDownCPU4; break;
                case 5:
                    box2 = numericUpDownCPU5; break;
                case 6:
                    box2 = numericUpDownCPU6; break;
                case 7:
                    box2 = numericUpDownCPU7; break;
                case 8:
                    box2 = numericUpDownCPU8; break;
                case 9:
                    box2 = numericUpDownCPU9; break;

            }
            decimal d = box1.Value;
            box1.Value = box2.Value;
            box2.Value = d;
        }
        void SwapTaskSendBefore(int swap1, int swap2)
        {
            TextBox text1 = null, text2 = null;
            switch (swap1)
            {
                case 1:
                    text1 = textBoxStart1; break;
                case 2:
                    text1 = textBoxStart2; break;
                case 3:
                    text1 = textBoxStart3; break;
                case 4:
                    text1 = textBoxStart4; break;
                case 5:
                    text1 = textBoxStart5; break;
                case 6:
                    text1 = textBoxStart6; break;
                case 7:
                    text1 = textBoxStart7; break;
                case 8:
                    text1 = textBoxStart8; break;
                case 9:
                    text1 = textBoxStart9; break;
            }
            switch (swap2)
            {
                case 1:
                    text2 = textBoxStart1; break;
                case 2:
                    text2 = textBoxStart2; break;
                case 3:
                    text2 = textBoxStart3; break;
                case 4:
                    text2 = textBoxStart4; break;
                case 5:
                    text2 = textBoxStart5; break;
                case 6:
                    text2 = textBoxStart6; break;
                case 7:
                    text2 = textBoxStart7; break;
                case 8:
                    text2 = textBoxStart8; break;
                case 9:
                    text2 = textBoxStart9; break;

            }
            string s = text1.Text;
            text1.Text = text2.Text;
            text2.Text = s;
        }
        void SwapTaskSendAfter(int swap1, int swap2)
        {
            TextBox text1 = null, text2 = null;
            switch (swap1)
            {
                case 1:
                    text1 = textBoxStop1; break;
                case 2:
                    text1 = textBoxStop2; break;
                case 3:
                    text1 = textBoxStop3; break;
                case 4:
                    text1 = textBoxStop4; break;
                case 5:
                    text1 = textBoxStop5; break;
                case 6:
                    text1 = textBoxStop6; break;
                case 7:
                    text1 = textBoxStop7; break;
                case 8:
                    text1 = textBoxStop8; break;
                case 9:
                    text1 = textBoxStop9; break;
            }
            switch (swap2)
            {
                case 1:
                    text2 = textBoxStop1; break;
                case 2:
                    text2 = textBoxStop2; break;
                case 3:
                    text2 = textBoxStop3; break;
                case 4:
                    text2 = textBoxStop4; break;
                case 5:
                    text2 = textBoxStop5; break;
                case 6:
                    text2 = textBoxStop6; break;
                case 7:
                    text2 = textBoxStop7; break;
                case 8:
                    text2 = textBoxStop8; break;
                case 9:
                    text2 = textBoxStop9; break;

            }
            string s = text1.Text;
            text1.Text = text2.Text;
            text2.Text = s;
        }
        void SwapTaskStartSequence(int swap1, int swap2)
        {
            MaskedTextBox text1 = null, text2 = null;
            switch (swap1)
            {
                case 1:
                    text1 = textBoxStart1Sequence; break;
                case 2:
                    text1 = textBoxStart2Sequence; break;
                case 3:
                    text1 = textBoxStart3Sequence; break;
                case 4:
                    text1 = textBoxStart4Sequence; break;
                case 5:
                    text1 = textBoxStart5Sequence; break;
                case 6:
                    text1 = textBoxStart6Sequence; break;
                case 7:
                    text1 = textBoxStart7Sequence; break;
                case 8:
                    text1 = textBoxStart8Sequence; break;
                case 9:
                    text1 = textBoxStart9Sequence; break;
            }
            switch (swap2)
            {
                case 1:
                    text2 = textBoxStart1Sequence; break;
                case 2:
                    text2 = textBoxStart2Sequence; break;
                case 3:
                    text2 = textBoxStart3Sequence; break;
                case 4:
                    text2 = textBoxStart4Sequence; break;
                case 5:
                    text2 = textBoxStart5Sequence; break;
                case 6:
                    text2 = textBoxStart6Sequence; break;
                case 7:
                    text2 = textBoxStart7Sequence; break;
                case 8:
                    text2 = textBoxStart8Sequence; break;
                case 9:
                    text2 = textBoxStart9Sequence; break;

            }
            string s = text1.Text;
            text1.Text = text2.Text;
            text2.Text = s;
        }

        void SwapTaskKeep(int swap1, int swap2)
        {
            CheckBox box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = checkBoxKeepRunning1; break;
                case 2:
                    box1 = checkBoxKeepRunning2; break;
                case 3:
                    box1 = checkBoxKeepRunning3; break;
                case 4:
                    box1 = checkBoxKeepRunning4; break;
                case 5:
                    box1 = checkBoxKeepRunning5; break;
                case 6:
                    box1 = checkBoxKeepRunning6; break;
                case 7:
                    box1 = checkBoxKeepRunning7; break;
                case 8:
                    box1 = checkBoxKeepRunning8; break;
                case 9:
                    box1 = checkBoxKeepRunning9; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = checkBoxKeepRunning1; break;
                case 2:
                    box2 = checkBoxKeepRunning2; break;
                case 3:
                    box2 = checkBoxKeepRunning3; break;
                case 4:
                    box2 = checkBoxKeepRunning4; break;
                case 5:
                    box2 = checkBoxKeepRunning5; break;
                case 6:
                    box2 = checkBoxKeepRunning6; break;
                case 7:
                    box2 = checkBoxKeepRunning7; break;
                case 8:
                    box2 = checkBoxKeepRunning8; break;
                case 9:
                    box2 = checkBoxKeepRunning9; break;

            }
            bool b = box1.Checked;
            box1.Checked = box2.Checked;
            box2.Checked = b;

        }
        void SwapTaskStopSequence(int swap1, int swap2)
        {
            MaskedTextBox text1 = null, text2 = null;
            switch (swap1)
            {
                case 1:
                    text1 = textBoxStart1Stop; break;
                case 2:
                    text1 = textBoxStart2Stop; break;
                case 3:
                    text1 = textBoxStart3Stop; break;
                case 4:
                    text1 = textBoxStart4Stop; break;
                case 5:
                    text1 = textBoxStart5Stop; break;
                case 6:
                    text1 = textBoxStart6Stop; break;
                case 7:
                    text1 = textBoxStart7Stop; break;
                case 8:
                    text1 = textBoxStart8Stop; break;
                case 9:
                    text1 = textBoxStart9Stop; break;
            }
            switch (swap2)
            {
                case 1:
                    text2 = textBoxStart1Stop; break;
                case 2:
                    text2 = textBoxStart2Stop; break;
                case 3:
                    text2 = textBoxStart3Stop; break;
                case 4:
                    text2 = textBoxStart4Stop; break;
                case 5:
                    text2 = textBoxStart5Stop; break;
                case 6:
                    text2 = textBoxStart6Stop; break;
                case 7:
                    text2 = textBoxStart7Stop; break;
                case 8:
                    text2 = textBoxStart8Stop; break;
                case 9:
                    text2 = textBoxStart9Stop; break;

            }
            string s = text1.Text;
            text1.Text = text2.Text;
            text2.Text = s;
        }
        void SwapTaskKill(int swap1, int swap2)
        {
            CheckBox box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = checkBoxKill1; break;
                case 2:
                    box1 = checkBoxKill2; break;
                case 3:
                    box1 = checkBoxKill3; break;
                case 4:
                    box1 = checkBoxKill4; break;
                case 5:
                    box1 = checkBoxKill5; break;
                case 6:
                    box1 = checkBoxKill6; break;
                case 7:
                    box1 = checkBoxKill7; break;
                case 8:
                    box1 = checkBoxKill8; break;
                case 9:
                    box1 = checkBoxKill9; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = checkBoxKill1; break;
                case 2:
                    box2 = checkBoxKill2; break;
                case 3:
                    box2 = checkBoxKill3; break;
                case 4:
                    box2 = checkBoxKill4; break;
                case 5:
                    box2 = checkBoxKill5; break;
                case 6:
                    box2 = checkBoxKill6; break;
                case 7:
                    box2 = checkBoxKill7; break;
                case 8:
                    box2 = checkBoxKill8; break;
                case 9:
                    box2 = checkBoxKill9; break;

            }
            bool b = box1.Checked;
            box1.Checked = box2.Checked;
            box2.Checked = b;

        }
        void SwapTaskStopWait(int swap1, int swap2)
        {
            NumericUpDown box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = numericUpDownDelayStop1; break;
                case 2:
                    box1 = numericUpDownDelayStop2; break;
                case 3:
                    box1 = numericUpDownDelayStop3; break;
                case 4:
                    box1 = numericUpDownDelayStop4; break;
                case 5:
                    box1 = numericUpDownDelayStop5; break;
                case 6:
                    box1 = numericUpDownDelayStop6; break;
                case 7:
                    box1 = numericUpDownDelayStop7; break;
                case 8:
                    box1 = numericUpDownDelayStop8; break;
                case 9:
                    box1 = numericUpDownDelayStop9; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = numericUpDownDelayStop1; break;
                case 2:
                    box2 = numericUpDownDelayStop2; break;
                case 3:
                    box2 = numericUpDownDelayStop3; break;
                case 4:
                    box2 = numericUpDownDelayStop4; break;
                case 5:
                    box2 = numericUpDownDelayStop5; break;
                case 6:
                    box2 = numericUpDownDelayStop6; break;
                case 7:
                    box2 = numericUpDownDelayStop7; break;
                case 8:
                    box2 = numericUpDownDelayStop8; break;
                case 9:
                    box2 = numericUpDownDelayStop9; break;

            }
            decimal d = box1.Value;
            box1.Value = box2.Value;
            box2.Value = d;
        }
        void SwapTaskPID(int swap1, int swap2)
        {
            Label box1 = null, box2 = null;
            switch (swap1)
            {
                case 1:
                    box1 = pid1; break;
                case 2:
                    box1 = pid2; break;
                case 3:
                    box1 = pid3; break;
                case 4:
                    box1 = pid4; break;
                case 5:
                    box1 = pid5; break;
                case 6:
                    box1 = pid6; break;
                case 7:
                    box1 = pid7; break;
                case 8:
                    box1 = pid8; break;
                case 9:
                    box1 = pid9; break;
            }
            switch (swap2)
            {
                case 1:
                    box2 = pid1; break;
                case 2:
                    box2 = pid2; break;
                case 3:
                    box2 = pid3; break;
                case 4:
                    box2 = pid4; break;
                case 5:
                    box2 = pid5; break;
                case 6:
                    box2 = pid6; break;
                case 7:
                    box2 = pid7; break;
                case 8:
                    box2 = pid8; break;
                case 9:
                    box2 = pid9; break;

            }
            string s = box1.Text;
            box1.Text = box2.Text;
            box2.Text = s;
        }

        void SwapTasks(object o, EventArgs e)
        {
            timer1.Stop();
            using (MenuItem m = (MenuItem)o)
            {
                int task2 = 0;
                switch (m.Text)
                {
                    case "Swap with Task#1":
                        task2 = 1;
                        break;
                    case "Swap with Task#2":
                        task2 = 2;
                        break;
                    case "Swap with Task#3":
                        task2 = 3;
                        break;
                    case "Swap with Task#4":
                        task2 = 4;
                        break;
                    case "Swap with Task#5":
                        task2 = 5;
                        break;
                    case "Swap with Task#6":
                        task2 = 6;
                        break;
                    case "Swap with Task#7":
                        task2 = 7;
                        break;
                    case "Swap with Task#8":
                        task2 = 8;
                        break;
                    case "Swap with Task#9":
                        task2 = 9;
                        break;
                    default:
                        MessageBox.Show("Unknown case in in SwapTasks=" + m.Text, "Error StartMe");
                        break;
                }
                SwapTaskPath(selectedTask, task2);
                SwapTaskParameters(selectedTask, task2);
                SwapTaskAuto(selectedTask, task2);
                SwapTaskMinimize(selectedTask, task2);
                SwapTaskAdmin(selectedTask, task2);
                SwapTaskPriority(selectedTask, task2); //OK
                SwapTaskEzName(selectedTask, task2);
                SwapTaskWinWait(selectedTask, task2);
                SwapTaskDelayAfter(selectedTask, task2);//OK
                SwapTaskCPU(selectedTask, task2);
                SwapTaskSendBefore(selectedTask, task2);
                SwapTaskStartSequence(selectedTask, task2);
                SwapTaskKeep(selectedTask, task2);
                SwapTaskStopSequence(selectedTask, task2);
                SwapTaskKill(selectedTask, task2);
                SwapTaskStopWait(selectedTask, task2);
                SwapTaskSendAfter(selectedTask, task2);
                SwapTaskPID(selectedTask, task2);
                Process tmpProcess = process[selectedTask];
                process[selectedTask] = process[task2];
                process[task2] = tmpProcess;
                textBoxPath1.ContextMenu = MenuGen(1);
                textBoxPath2.ContextMenu = MenuGen(2);
                textBoxPath3.ContextMenu = MenuGen(3);
                textBoxPath4.ContextMenu = MenuGen(4);
                textBoxPath5.ContextMenu = MenuGen(5);
                textBoxPath6.ContextMenu = MenuGen(6);
                textBoxPath7.ContextMenu = MenuGen(7);
                textBoxPath8.ContextMenu = MenuGen(8);
                textBoxPath9.ContextMenu = MenuGen(9);
            }
            timer1.Interval = timer1.Interval;
            timer1.Start();
        }

        ContextMenu MenuGen(int except)
        {
            ContextMenu cm = new ContextMenu();
            for (int i = 1; i < 10; ++i)
            {
                if (i != except)
                {
                    //cm.MenuItems.Add("Swap with Task#" + i, MyRightClick(except, 1));
                    cm.MenuItems.Add("Swap with Task#" + i, SwapTasks);
                }
            }
            return cm;
        }

        private void ButtonBackups_Click(object sender, EventArgs e)
        {
            using (var myBackups = new Backups())
            {
                var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                myBackups.Backups_List(userConfig);
            }
            _ = SettingsLoad(settingsKey);
        }

        private void LabelPath1_Click(object sender, EventArgs e)
        {
            textBoxPath1.Enabled = !textBoxPath1.Enabled;
            buttonStart1.Enabled = textBoxPath1.Enabled;
        }

        private void LabelPath2_Click(object sender, EventArgs e)
        {
            textBoxPath2.Enabled = !textBoxPath2.Enabled;
            buttonStart2.Enabled = textBoxPath2.Enabled;
        }

        private void LabelPath3_Click(object sender, EventArgs e)
        {
            textBoxPath3.Enabled = !textBoxPath3.Enabled;
            buttonStart3.Enabled = textBoxPath3.Enabled;
        }

        private void LabelPath4_Click(object sender, EventArgs e)
        {
            textBoxPath4.Enabled = !textBoxPath4.Enabled;
            buttonStart4.Enabled = textBoxPath4.Enabled;
        }

        private void LabelPath5_Click(object sender, EventArgs e)
        {
            textBoxPath5.Enabled = !textBoxPath5.Enabled;
            buttonStart5.Enabled = textBoxPath5.Enabled;
        }

        private void LabelPath6_Click(object sender, EventArgs e)
        {
            textBoxPath6.Enabled = !textBoxPath6.Enabled;
            buttonStart6.Enabled = textBoxPath6.Enabled;
        }

        private void LabelPath7_Click(object sender, EventArgs e)
        {
            textBoxPath7.Enabled = !textBoxPath7.Enabled;
            buttonStart7.Enabled = textBoxPath7.Enabled;
        }

        private void LabelPath8_Click(object sender, EventArgs e)
        {
            textBoxPath8.Enabled = !textBoxPath8.Enabled;
            buttonStart8.Enabled = textBoxPath8.Enabled;
        }

        private void LabelPath9_Click(object sender, EventArgs e)
        {
            textBoxPath9.Enabled = !textBoxPath9.Enabled;
            buttonStart9.Enabled = textBoxPath9.Enabled;
        }

        private void ButtonStartAuto_Click(object sender, EventArgs e)
        {
            mut.WaitOne();
            if (ModifierKeys.HasFlag(Keys.Shift))
            {
                StartAllConfigs(true);
            }
            else
            {
                StartAll(true);
            }
            mut.ReleaseMutex();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
        }

        private void CheckBoxStartup_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
