﻿using System;
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
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace StartMe
{
    public partial class Form1 : Form
    {
        [DllImport("User32.dll")]
        static extern bool SetForegroundWindow(IntPtr point);
        [DllImport("User32.dll")]
        static extern IntPtr FindWindow(string lpClass, String lpWindowName);
        bool autoStartDone = false;
        String settingsKey = "";
        List<String> settingsKeys = new List<string>();
        readonly String[] processArgs = new string[10];
        readonly Process[] process = new Process[10]; // we use 1-9
        //int[] processID = new int[10];
        //String[] windowTitle = new string[10];
        static int jtalerts = 0;
        private readonly bool noauto = false;
        readonly bool settingsSave = false;
        public Form1()
        {
            InitializeComponent();

            ToolTip toolTip = new ToolTip
            {
                UseAnimation = true,
                UseFading = true,
                IsBalloon = true
            };
            String tip = "Path to executable";
            toolTip.SetToolTip(label1, tip);
            toolTip.SetToolTip(textBoxPath1, tip);
            toolTip.SetToolTip(textBoxPath2, tip);
            toolTip.SetToolTip(textBoxPath3, tip);
            toolTip.SetToolTip(textBoxPath4, tip);
            toolTip.SetToolTip(textBoxPath5, tip);
            toolTip.SetToolTip(textBoxPath6, tip);
            toolTip.SetToolTip(textBoxPath7, tip);
            toolTip.SetToolTip(textBoxPath8, tip);
            toolTip.SetToolTip(textBoxPath9, tip);

            tip = "File Browser";
            //toolTip.SetToolTip(label2, tip);
            toolTip.SetToolTip(buttonFile1, tip);
            toolTip.SetToolTip(buttonFile2, tip);
            toolTip.SetToolTip(buttonFile3, tip);
            toolTip.SetToolTip(buttonFile4, tip);
            toolTip.SetToolTip(buttonFile5, tip);
            toolTip.SetToolTip(buttonFile6, tip);
            toolTip.SetToolTip(buttonFile7, tip);
            toolTip.SetToolTip(buttonFile8, tip);
            toolTip.SetToolTip(buttonFile9, tip);

            tip = "Parameters for program";
            //toolTip.SetToolTip(label3, tip);
            toolTip.SetToolTip(textBoxArgs1, tip);
            toolTip.SetToolTip(textBoxArgs2, tip);
            toolTip.SetToolTip(textBoxArgs3, tip);
            toolTip.SetToolTip(textBoxArgs4, tip);
            toolTip.SetToolTip(textBoxArgs5, tip);
            toolTip.SetToolTip(textBoxArgs6, tip);
            toolTip.SetToolTip(textBoxArgs7, tip);
            toolTip.SetToolTip(textBoxArgs8, tip);
            toolTip.SetToolTip(textBoxArgs9, tip);

            tip = "Command line paramters";
            //toolTip.SetToolTip(label3, tip);
            toolTip.SetToolTip(textBoxArgs1, tip);
            toolTip.SetToolTip(textBoxArgs2, tip);
            toolTip.SetToolTip(textBoxArgs3, tip);
            toolTip.SetToolTip(textBoxArgs4, tip);
            toolTip.SetToolTip(textBoxArgs5, tip);
            toolTip.SetToolTip(textBoxArgs6, tip);
            toolTip.SetToolTip(textBoxArgs7, tip);
            toolTip.SetToolTip(textBoxArgs8, tip);
            toolTip.SetToolTip(textBoxArgs9, tip);

            tip = "Auto start during 1st run of program";
            //toolTip.SetToolTip(label4, tip);
            toolTip.SetToolTip(checkBoxAutoStart1, tip);
            toolTip.SetToolTip(checkBoxAutoStart1, tip);
            toolTip.SetToolTip(checkBoxAutoStart3, tip);
            toolTip.SetToolTip(checkBoxAutoStart4, tip);
            toolTip.SetToolTip(checkBoxAutoStart5, tip);
            toolTip.SetToolTip(checkBoxAutoStart6, tip);
            toolTip.SetToolTip(checkBoxAutoStart7, tip);
            toolTip.SetToolTip(checkBoxAutoStart8, tip);
            toolTip.SetToolTip(checkBoxAutoStart9, tip);

            tip = "Minimize window at startup";
            toolTip.SetToolTip(checkBoxMinimize1, tip);
            toolTip.SetToolTip(checkBoxMinimize2, tip);
            toolTip.SetToolTip(checkBoxMinimize3, tip);
            toolTip.SetToolTip(checkBoxMinimize4, tip);
            toolTip.SetToolTip(checkBoxMinimize5, tip);
            toolTip.SetToolTip(checkBoxMinimize6, tip);
            toolTip.SetToolTip(checkBoxMinimize7, tip);
            toolTip.SetToolTip(checkBoxMinimize8, tip);
            toolTip.SetToolTip(checkBoxMinimize9, tip);

            tip = "Start with Administrative rights";
            //toolTip.SetToolTip(label, tip);
            toolTip.SetToolTip(checkBoxAdmin1, tip);
            toolTip.SetToolTip(checkBoxAdmin2, tip);
            toolTip.SetToolTip(checkBoxAdmin3, tip);
            toolTip.SetToolTip(checkBoxAdmin4, tip);
            toolTip.SetToolTip(checkBoxAdmin5, tip);
            toolTip.SetToolTip(checkBoxAdmin6, tip);
            toolTip.SetToolTip(checkBoxAdmin7, tip);
            toolTip.SetToolTip(checkBoxAdmin8, tip);
            toolTip.SetToolTip(checkBoxAdmin9, tip);

            tip = "Set priority on startup";
            //toolTip.SetToolTip(label7, tip);
            toolTip.SetToolTip(comboBoxPriority1, tip);
            toolTip.SetToolTip(comboBoxPriority2, tip);
            toolTip.SetToolTip(comboBoxPriority3, tip);
            toolTip.SetToolTip(comboBoxPriority4, tip);
            toolTip.SetToolTip(comboBoxPriority5, tip);
            toolTip.SetToolTip(comboBoxPriority6, tip);
            toolTip.SetToolTip(comboBoxPriority7, tip);
            toolTip.SetToolTip(comboBoxPriority8, tip);
            toolTip.SetToolTip(comboBoxPriority9, tip);

            tip = "Start task now. Shift-click to not start Next";
            toolTip.SetToolTip(buttonStart1, tip);
            toolTip.SetToolTip(buttonStart2, tip);
            toolTip.SetToolTip(buttonStart3, tip);
            toolTip.SetToolTip(buttonStart4, tip);
            toolTip.SetToolTip(buttonStart5, tip);
            toolTip.SetToolTip(buttonStart6, tip);
            toolTip.SetToolTip(buttonStart7, tip);
            toolTip.SetToolTip(buttonStart8, tip);
            toolTip.SetToolTip(buttonStart9, tip);

            tip = "Delay before startup";
            toolTip.SetToolTip(numericUpDownDelay1Before, tip);
            toolTip.SetToolTip(numericUpDownDelay2Before, tip);
            toolTip.SetToolTip(numericUpDownDelay3Before, tip);
            toolTip.SetToolTip(numericUpDownDelay4Before, tip);
            toolTip.SetToolTip(numericUpDownDelay5Before, tip);
            toolTip.SetToolTip(numericUpDownDelay6Before, tip);
            toolTip.SetToolTip(numericUpDownDelay7Before, tip);
            toolTip.SetToolTip(numericUpDownDelay8Before, tip);
            toolTip.SetToolTip(numericUpDownDelay9Before, tip);

            tip = "Delay after startup";
            toolTip.SetToolTip(numericUpDownDelay1After, tip);
            toolTip.SetToolTip(numericUpDownDelay2After, tip);
            toolTip.SetToolTip(numericUpDownDelay3After, tip);
            toolTip.SetToolTip(numericUpDownDelay4After, tip);
            toolTip.SetToolTip(numericUpDownDelay5After, tip);
            toolTip.SetToolTip(numericUpDownDelay6After, tip);
            toolTip.SetToolTip(numericUpDownDelay7After, tip);
            toolTip.SetToolTip(numericUpDownDelay8After, tip);
            toolTip.SetToolTip(numericUpDownDelay9After, tip);

            tip = "Delay after stop signal";
            toolTip.SetToolTip(numericUpDownDelayStop1, tip);
            toolTip.SetToolTip(numericUpDownDelayStop2, tip);
            toolTip.SetToolTip(numericUpDownDelayStop3, tip);
            toolTip.SetToolTip(numericUpDownDelayStop4, tip);
            toolTip.SetToolTip(numericUpDownDelayStop5, tip);
            toolTip.SetToolTip(numericUpDownDelayStop6, tip);
            toolTip.SetToolTip(numericUpDownDelayStop7, tip);
            toolTip.SetToolTip(numericUpDownDelayStop8, tip);
            toolTip.SetToolTip(numericUpDownDelayStop9, tip);

            tip = "% CPU on task before proceeeding";
            toolTip.SetToolTip(numericUpDownCPU1, tip);
            toolTip.SetToolTip(numericUpDownCPU2, tip);
            toolTip.SetToolTip(numericUpDownCPU3, tip);
            toolTip.SetToolTip(numericUpDownCPU4, tip);
            toolTip.SetToolTip(numericUpDownCPU5, tip);
            toolTip.SetToolTip(numericUpDownCPU6, tip);
            toolTip.SetToolTip(numericUpDownCPU7, tip);
            toolTip.SetToolTip(numericUpDownCPU8, tip);
            toolTip.SetToolTip(numericUpDownCPU9, tip);

            tip = "Window title + Keys to send to window after startup delay";
            toolTip.SetToolTip(textBoxStart1, tip);
            toolTip.SetToolTip(textBoxStart2, tip);
            toolTip.SetToolTip(textBoxStart3, tip);
            toolTip.SetToolTip(textBoxStart4, tip);
            toolTip.SetToolTip(textBoxStart5, tip);
            toolTip.SetToolTip(textBoxStart6, tip);
            toolTip.SetToolTip(textBoxStart7, tip);
            toolTip.SetToolTip(textBoxStart8, tip);
            toolTip.SetToolTip(textBoxStart9, tip);

            tip = "Sequence # for startup";
            toolTip.SetToolTip(textBoxStart1Sequence, tip);
            toolTip.SetToolTip(textBoxStart2Sequence, tip);
            toolTip.SetToolTip(textBoxStart3Sequence, tip);
            toolTip.SetToolTip(textBoxStart4Sequence, tip);
            toolTip.SetToolTip(textBoxStart5Sequence, tip);
            toolTip.SetToolTip(textBoxStart6Sequence, tip);
            toolTip.SetToolTip(textBoxStart7Sequence, tip);
            toolTip.SetToolTip(textBoxStart8Sequence, tip);
            toolTip.SetToolTip(textBoxStart9Sequence, tip);

            tip = "Sequence # for stopping";
            toolTip.SetToolTip(textBoxStart1Stop, tip);
            toolTip.SetToolTip(textBoxStart2Stop, tip);
            toolTip.SetToolTip(textBoxStart3Stop, tip);
            toolTip.SetToolTip(textBoxStart4Stop, tip);
            toolTip.SetToolTip(textBoxStart5Stop, tip);
            toolTip.SetToolTip(textBoxStart6Stop, tip);
            toolTip.SetToolTip(textBoxStart7Stop, tip);
            toolTip.SetToolTip(textBoxStart8Stop, tip);
            toolTip.SetToolTip(textBoxStart9Stop, tip);

            tip = "Stop task, Ctrl-Click to kill task, Alt-click to stop all tasks";
            toolTip.SetToolTip(buttonStop1, tip);
            toolTip.SetToolTip(buttonStop2, tip);
            toolTip.SetToolTip(buttonStop3, tip);
            toolTip.SetToolTip(buttonStop4, tip);
            toolTip.SetToolTip(buttonStop5, tip);
            toolTip.SetToolTip(buttonStop6, tip);
            toolTip.SetToolTip(buttonStop7, tip);
            toolTip.SetToolTip(buttonStop8, tip);
            toolTip.SetToolTip(buttonStop9, tip);

            tip = "Kill task if normal close fails";
            toolTip.SetToolTip(checkBoxKill1, tip);
            toolTip.SetToolTip(checkBoxKill2, tip);
            toolTip.SetToolTip(checkBoxKill3, tip);
            toolTip.SetToolTip(checkBoxKill4, tip);
            toolTip.SetToolTip(checkBoxKill5, tip);
            toolTip.SetToolTip(checkBoxKill6, tip);
            toolTip.SetToolTip(checkBoxKill7, tip);
            toolTip.SetToolTip(checkBoxKill8, tip);
            toolTip.SetToolTip(checkBoxKill9, tip);

            tip = "Wait time for stop to complete (secs)";
            toolTip.SetToolTip(numericUpDownDelayStop1, tip);
            toolTip.SetToolTip(numericUpDownDelayStop2, tip);
            toolTip.SetToolTip(numericUpDownDelayStop3, tip);
            toolTip.SetToolTip(numericUpDownDelayStop4, tip);
            toolTip.SetToolTip(numericUpDownDelayStop5, tip);
            toolTip.SetToolTip(numericUpDownDelayStop6, tip);
            toolTip.SetToolTip(numericUpDownDelayStop7, tip);
            toolTip.SetToolTip(numericUpDownDelayStop8, tip);
            toolTip.SetToolTip(numericUpDownDelayStop9, tip);

            tip = "Window title + Keys to send to window during closing";
            toolTip.SetToolTip(textBoxStop1, tip);
            toolTip.SetToolTip(textBoxStop2, tip);
            toolTip.SetToolTip(textBoxStop3, tip);
            toolTip.SetToolTip(textBoxStop4, tip);
            toolTip.SetToolTip(textBoxStop5, tip);
            toolTip.SetToolTip(textBoxStop6, tip);
            toolTip.SetToolTip(textBoxStop7, tip);
            toolTip.SetToolTip(textBoxStop8, tip);
            toolTip.SetToolTip(textBoxStop9, tip);

            toolTip.SetToolTip(checkBoxMinimize, "Minimize StartMe window after startup");
            toolTip.SetToolTip(buttonStopAll, "Stop all tasks now");

            String[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && args[1].Equals("-noauto"))
            {
                noauto = false;
            }
            if (args.Length > 2)
            {
                MessageBox.Show("Only one argument (i.e. configname) expected");
                return;
            }
            if (args.Length == 2)
            {
                SettingsLoad(args[1]);
            }
            else
            {
                SettingsLoad("Default");
            }
            //processArgs[1] = textBoxArgs1.Text;
            //processArgs[2] = textBoxArgs2.Text;
            //processArgs[3] = textBoxArgs3.Text;
            //processArgs[4] = textBoxArgs4.Text;
            //processArgs[5] = textBoxArgs5.Text;
            //processArgs[6] = textBoxArgs6.Text;
            //processArgs[7] = textBoxArgs7.Text;
            //processArgs[8] = textBoxArgs8.Text;
            //processArgs[9] = textBoxArgs9.Text;
            settingsSave = true;
            
            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                //MessageBox.Show("Task Delayed");
            });
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (checkBoxCloseAll.Checked)
            {
                StopAll();
            }
            Application.Exit();
            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        //    if (elevateMe())
        //        Application.Exit();
            if (settingsKey.Equals(""))
            {
                settingsKeys = SettingsGetKeys();
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
                }
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
            if (checkBoxMinimize.Checked) this.WindowState = FormWindowState.Minimized;
            ProcessCheck();
            ProcessInit();

        }



        private bool ProcessIsRunning(int n)
        {
            String exe = "";
            String args = "";
            switch (n)
            {
                case 1: exe = textBoxPath1.Text; args = textBoxArgs1.Text; break;
                case 2: exe = textBoxPath2.Text; args = textBoxArgs2.Text; break;
                case 3: exe = textBoxPath3.Text; args = textBoxArgs3.Text; break;
                case 4: exe = textBoxPath4.Text; args = textBoxArgs4.Text; break;
                case 5: exe = textBoxPath5.Text; args = textBoxArgs5.Text; break;
                case 6: exe = textBoxPath6.Text; args = textBoxArgs6.Text; break;
                case 7: exe = textBoxPath7.Text; args = textBoxArgs7.Text; break;
                case 8: exe = textBoxPath8.Text; args = textBoxArgs8.Text; break;
                case 9: exe = textBoxPath9.Text; args = textBoxArgs9.Text; break;
            }
            return ProcessIsRunning(exe, args, n, ref process[n]);
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
            else if (fileName.ToLower().Contains("jtalert"))
            {
                fileName = process[n].MainWindowTitle.Split(']').First();
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

#pragma warning disable IDE0060 // Remove unused parameter
        private bool ProcessFindName(String name, String args, int n, ref Label processId, ref Label labelWindowTitle)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (name.Length == 0) return false;

            if (!File.Exists(name))
            {
                MessageBox.Show("Process #" + n + " file does not exist\n" + name);
                SetPathColor(n, Color.Red);
                processId.Text = "0";
                SetStartStop(n, false, false);
                return false;
            }
            labelWindowTitle.Text = ""; // Unique process with Id's do not need the window title to distinguish them
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
                    if (name.Equals(myprocess.MainModule.FileName))
                    {
                        SetStartStop(n, false, true);
                        process[n] = myprocess;
                        labelWindowTitle.Text = GetWindowTitle(myprocess);
                        
//                        processID[n] = myprocess.Id;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(name + "\n" + ex.Message + "\n");
                    processId.Text = "0";
                    SetStartStop(n, true, false);
                }
            }
            Process p2 = GetProcessByFileName(n);
            if (p2 == null)
            {
                process[n] = null;
                SetStartStop(n, true, false);
                return false;
            }
            //if (processNameIsUnique(name))
            {
                //String windowTitle = "";
                process[n] = p2;
                //                processID[n] = process[n].Id;
                processId.Text = process[n].Id.ToString();
                ProcessSetId(n, process[n].Id);
                // MDB temp for debugging
                if (name.ToLower().Contains("jtalert"))
                {
                    //char[] split = { ',' };
                    //string[] tokens = name.Split(split);
                    //string title = "JTALert"+tokens[4].Substring(0, 2);
                    labelWindowTitle.Text = p2.MainWindowTitle;
                }
                else if (name.ToLower().Contains("wsjtx")) { 
                labelWindowTitle.Text = p2.MainWindowTitle;
                }
                SetStartStop(n, false, true);
                return true;
            }
            /*
            // Not a unique process so what do we do now?
            Process[] pAll = getProcessesByFileName(name);
            List<int> indexes = new List<int>();
            String sx = textBoxPath5.Text.Split('\\').Last().Split('.').First();
            if (textBoxPath1.Text.Split('\\').Last().Split('.').First().Equals(pAll[0].ProcessName)) indexes.Add(1);
            if (textBoxPath2.Text.Split('\\').Last().Split('.').First().Equals(pAll[0].ProcessName)) indexes.Add(2);
            if (textBoxPath3.Text.Split('\\').Last().Split('.').First().Equals(pAll[0].ProcessName)) indexes.Add(3);
            if (textBoxPath4.Text.Split('\\').Last().Split('.').First().Equals(pAll[0].ProcessName)) indexes.Add(4);
            if (textBoxPath5.Text.Split('\\').Last().Split('.').First().Equals(pAll[0].ProcessName)) indexes.Add(5);
            if (textBoxPath6.Text.Split('\\').Last().Split('.').First().Equals(pAll[0].ProcessName)) indexes.Add(6);
            if (textBoxPath7.Text.Split('\\').Last().Split('.').First().Equals(pAll[0].ProcessName)) indexes.Add(7);
            if (textBoxPath8.Text.Split('\\').Last().Split('.').First().Equals(pAll[0].ProcessName)) indexes.Add(8);
            if (textBoxPath9.Text.Split('\\').Last().Split('.').First().Equals(pAll[0].ProcessName)) indexes.Add(9);
            List<int>.Enumerator eIndexes = indexes.GetEnumerator();

            for (int i = 0; i < pAll.Count(); ++i)
            {
                eIndexes.MoveNext();
                int index = eIndexes.Current;
                switch (index)
                {
                    case 1: labelWindowTitle.Text = pAll[i].MainWindowTitle; pid1.Text = pAll[i].Id.ToString(); break;
                    case 2: labelWindowTitle.Text = pAll[i].MainWindowTitle; pid2.Text = pAll[i].Id.ToString(); break;
                    case 3: labelWindowTitle.Text = pAll[i].MainWindowTitle; pid3.Text = pAll[i].Id.ToString(); break;
                    case 4: labelWindowTitle.Text = pAll[i].MainWindowTitle; pid4.Text = pAll[i].Id.ToString(); break;
                    case 5: labelWindowTitle.Text = pAll[i].MainWindowTitle; pid5.Text = pAll[i].Id.ToString(); break;
                    case 6: labelWindowTitle.Text = pAll[i].MainWindowTitle; pid6.Text = pAll[i].Id.ToString(); break;
                    case 7: labelWindowTitle.Text = pAll[i].MainWindowTitle; pid7.Text = pAll[i].Id.ToString(); break;
                    case 8: labelWindowTitle.Text = pAll[i].MainWindowTitle; pid8.Text = pAll[i].Id.ToString(); break;
                    case 9: labelWindowTitle.Text = pAll[i].MainWindowTitle; pid9.Text = pAll[i].Id.ToString(); break;
                }
                //labelWindowTitle.Text = pAll[i].MainWindowTitle;

            }
            
            return false;
            */
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
            if (exeName.ToLower().Contains("jtalert")) ++jtalerts;
            foreach (Process p in pName)
            {
                if (p.MainWindowTitle.ToLower().Contains("jtalert"))
                {
                    // get the instance#
                    String sInstance = p.MainWindowTitle.Split('#').Last().Split(']').First();
                    int instance = Convert.ToInt16(sInstance);
                    if (jtalerts == instance) return p;
                }
                processArgs = ProcCmdLine.GetCommandLineOfProcessIgnoreFirst(p);
                
                ++i;
                try
                {
                    if (fileName.Equals(p.MainModule.FileName) && processArgs.Equals(fileArgs) )
                        break; // find the first one
                }
                catch (Win32Exception ex)
                {
                    MessageBox.Show("Restart with admin rights" + "\n" + fileName + "\n" + ex.Message);
                    adminNeeded = true;
                }
            }
            if (!adminNeeded)
            {
                if (i>=0 && fileName.Equals(pName[i].MainModule.FileName) && fileArgs.Equals(processArgs))
                {
                    return pName[i];
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
#pragma warning disable IDE0060 // Remove unused parameter
        private Process GetProcessByFileName(String fileName, ref String windowTitle)
#pragma warning restore IDE0060 // Remove unused parameter
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

        private void ProcessInit()
        {
            ProcessFindName(textBoxPath1.Text, textBoxArgs1.Text, 1, ref pid1, ref labelWindowTitle1);
            //if (pid1.Text.Length == 0 && checkBoxAutoStart1.Checked)
            //    processStart(1, Keys.None);
            ProcessFindName(textBoxPath2.Text, textBoxArgs2.Text, 2, ref pid2, ref labelWindowTitle2);
            ProcessFindName(textBoxPath3.Text, textBoxArgs3.Text, 3, ref pid3, ref labelWindowTitle3);
            ProcessFindName(textBoxPath4.Text, textBoxArgs4.Text, 4, ref pid4, ref labelWindowTitle4);
            ProcessFindName(textBoxPath5.Text, textBoxArgs5.Text, 5, ref pid5, ref labelWindowTitle5);
            ProcessFindName(textBoxPath6.Text, textBoxArgs6.Text, 6, ref pid6, ref labelWindowTitle6);
            ProcessFindName(textBoxPath7.Text, textBoxArgs7.Text, 7, ref pid7, ref labelWindowTitle7);
            ProcessFindName(textBoxPath8.Text, textBoxArgs8.Text, 8, ref pid8, ref labelWindowTitle8);
            ProcessFindName(textBoxPath9.Text, textBoxArgs9.Text, 9, ref pid9, ref labelWindowTitle9);
            return;

        }

        private void ProcessCheck()
        {
            Process[] pAll = Process.GetProcesses();
            foreach (Process p in pAll)
            {
                if (p.MainWindowTitle.Equals("")) continue;
                try
                {
                    ProcessModule mainModule = p.MainModule;
                    String mainWindowTitle = p.MainWindowTitle;
                    if (mainWindowTitle.Contains("JTAlert"))
                    {
                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show("Here6");
                    continue;
                }
            }
        }
#pragma warning disable IDE0060 // Remove unused parameter
        private bool ProcessIsRunning(String path, String args, int n, ref Process myProcess)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // we will use our current pid
            try
            {
                if (process[n] != null && process[n].Id > 0)
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
                    //pid++;
                    //Boolean start = false, stop = true;
                    try
                    {
                        Process p1 = Process.GetProcessById(pid);
                        return true;
                    }
                    catch (Exception)
                    {
                        // if we get exception than it's not running anymore
                        SetStartStop(n, true, false);
                        process[n] = null;
                    }
                }
            }
            catch (Exception)
            {

            }
            return false;
            /*
            int index = path.LastIndexOf('\\');
            String exe = path.Substring(index + 1).Replace(".exe", "");
            Process[] p = Process.GetProcessesByName(exe);
            if (p.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < p.Length; ++i)
            {
                bool isTheProcess = false;
                String title = p[i].MainWindowTitle.Split(']').First();
                if (n == 1 && labelWindowTitle1.Equals("")) labelWindowTitle1.Text = p[i].MainWindowTitle;
                else if (n == 2 && labelWindowTitle2.Text.Equals(""))
                    labelWindowTitle2.Text = title;
                else if (n == 3 && labelWindowTitle3.Text.Equals(""))
                    labelWindowTitle3.Text = title;
                else if (n == 4 && labelWindowTitle4.Text.Equals(""))
                    labelWindowTitle4.Text = title;
                else if (n == 5 && labelWindowTitle5.Text.Equals(""))
                    labelWindowTitle5.Text = title;
                else if (n == 6 && labelWindowTitle6.Text.Equals(""))
                    labelWindowTitle6.Text = title;
                else if (n == 7 && labelWindowTitle7.Text.Equals(""))
                    labelWindowTitle7.Text = title;
                else if (n == 8 && labelWindowTitle8.Text.Equals(""))
                    labelWindowTitle8.Text = title;
                else if (n == 9 && labelWindowTitle9.Text.Equals(""))
                    labelWindowTitle9.Text = title;
                if (n == 1 && p[i].MainWindowTitle.Equals(labelWindowTitle1.Text))
                {
                    if (pid1.Text.Equals("")) pid1.Text = p[i].Id.ToString();
                    isTheProcess = true;
                }
                else if (n == 2 && p[i].MainWindowTitle.Equals(labelWindowTitle2.Text))
                {
                    if (pid2.Text.Equals("")) pid2.Text = p[i].Id.ToString();
                    isTheProcess = true;
                }
                else if (n == 3 && p[i].MainWindowTitle.Equals(labelWindowTitle3.Text))
                {
                    if (pid3.Text.Equals("")) pid3.Text = p[i].Id.ToString();
                    isTheProcess = true;
                }
                else if (n == 4 && p[i].MainWindowTitle.Equals(labelWindowTitle4.Text))
                {
                    if (pid4.Text.Equals("")) pid4.Text = p[i].Id.ToString();
                    isTheProcess = true;
                }
                else if (n == 5 && p[i].MainWindowTitle.Equals(labelWindowTitle5.Text))
                {
                    if (pid5.Text.Equals("")) pid5.Text = p[i].Id.ToString();
                    isTheProcess = true;
                }
                else if (n == 6 && p[i].MainWindowTitle.Equals(labelWindowTitle6.Text))
                {
                    if (pid6.Text.Equals("")) pid6.Text = p[i].Id.ToString();
                    isTheProcess = true;
                }
                else if (n == 7 && p[i].MainWindowTitle.Equals(labelWindowTitle7.Text))
                {
                    if (pid7.Text.Equals("")) pid7.Text = p[i].Id.ToString();
                    isTheProcess = true;
                }
                else if (n == 8 && p[i].MainWindowTitle.Equals(labelWindowTitle8.Text))
                {
                    if (pid8.Text.Equals("")) pid8.Text = p[i].Id.ToString();
                    isTheProcess = true;
                }
                else if (n == 9 && p[i].MainWindowTitle.Equals(labelWindowTitle9.Text))
                {
                    if (pid9.Text.Equals("")) pid9.Text = p[i].Id.ToString();
                    isTheProcess = true;
                }
                //if (processArgs[n].Equals(args) && p[i].Id==processID[n])
                if (isTheProcess)
                {
                    if (myProcess != null)
                    {
                        myProcess = p[i];
                    }
                    setStartStop(n, false, true);
                    return true;
                }
            }
            setStartStop(n, true, false);
            return false;
            */
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
            //if (tokens[0].Length == 0) return;
            //if (tokens.Length != 3)
            //{
            //    MessageBox.Show("Improper use of Key sending...see Help","StartMe Error",MessageBoxButtons.OK);
            //    return;
            //}
            windowName = tokens[1];
            keys = tokens[2];
            tokens = keys.Split(' ');
            if (keys.Length == 0) return;
            IntPtr h = FindWindow(null, windowName);
            if (h == null)
            {
                MessageBox.Show("Windowname '" + windowName + " not found", "StartMe Error", MessageBoxButtons.OK);
                return;
            }
            SetForegroundWindow(h);
            foreach (String s in tokens)
            {
                String key = "";
                bool alt = false;
                switch (s.ToUpper())
                {
                    case "ENTER":
                        key = "{ENTER}";
                        break;
                    case "TAB":
                        key = "{TAB}";
                        break;
                    case "ALT":
                        alt = true;
                        break;
                    case "F4":
                        key = "{F4}";
                        break;
                    default:
                        key = s;
                        break;
                }
                if (alt) key = "%" + key;
                SendKeys.SendWait(key);
            }
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
                    MessageBox.Show("Start/Stop buttuon#" + n + " not found");
                    break;
            }
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
                    MessageBox.Show("Error converting \"" + s + "\" to integer\n" + ex.StackTrace);
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
#pragma warning disable IDE0060 // Remove unused parameter
        private void ProcessStart(int n, Keys modifierKeys)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            labelStatusMessage.Text = "Starting process#" + n;
            Application.DoEvents();

            Cursor.Current = Cursors.WaitCursor;
            String processName = "";
            String args = "";
            decimal sleepBefore = 0;
            decimal sleepAfter = 0;
            decimal cpu = 0;
            bool minimize = false;
            int priority = 0;
            switch (n)
            {
                case 1: processName = textBoxPath1.Text; sleepBefore = numericUpDownDelay1Before.Value; sleepAfter = numericUpDownDelay1After.Value; cpu = numericUpDownCPU1.Value; args = processArgs[n] = textBoxArgs1.Text; minimize = checkBoxMinimize1.Checked; priority = comboBoxPriority1.SelectedIndex; break;
                case 2: processName = textBoxPath2.Text; sleepBefore = numericUpDownDelay2Before.Value; sleepAfter = numericUpDownDelay2After.Value; cpu = numericUpDownCPU2.Value; args = processArgs[n] = textBoxArgs2.Text; minimize = checkBoxMinimize2.Checked; priority = comboBoxPriority2.SelectedIndex; break;
                case 3: processName = textBoxPath3.Text; sleepBefore = numericUpDownDelay3Before.Value; sleepAfter = numericUpDownDelay3After.Value; cpu = numericUpDownCPU3.Value; args = processArgs[n] = textBoxArgs3.Text; minimize = checkBoxMinimize3.Checked; priority = comboBoxPriority3.SelectedIndex; break;
                case 4: processName = textBoxPath4.Text; sleepBefore = numericUpDownDelay4Before.Value; sleepAfter = numericUpDownDelay4After.Value; cpu = numericUpDownCPU4.Value; args = processArgs[n] = textBoxArgs4.Text; minimize = checkBoxMinimize4.Checked; priority = comboBoxPriority4.SelectedIndex; break;
                case 5: processName = textBoxPath5.Text; sleepBefore = numericUpDownDelay5Before.Value; sleepAfter = numericUpDownDelay5After.Value; cpu = numericUpDownCPU5.Value; args = processArgs[n] = textBoxArgs5.Text; minimize = checkBoxMinimize5.Checked; priority = comboBoxPriority5.SelectedIndex; break;
                case 6: processName = textBoxPath6.Text; sleepBefore = numericUpDownDelay6Before.Value; sleepAfter = numericUpDownDelay6After.Value; cpu = numericUpDownCPU6.Value; args = processArgs[n] = textBoxArgs6.Text; minimize = checkBoxMinimize6.Checked; priority = comboBoxPriority6.SelectedIndex; break;
                case 7: processName = textBoxPath7.Text; sleepBefore = numericUpDownDelay7Before.Value; sleepAfter = numericUpDownDelay7After.Value; cpu = numericUpDownCPU7.Value; args = processArgs[n] = textBoxArgs7.Text; minimize = checkBoxMinimize7.Checked; priority = comboBoxPriority7.SelectedIndex; break;
                case 8: processName = textBoxPath8.Text; sleepBefore = numericUpDownDelay8Before.Value; sleepAfter = numericUpDownDelay8After.Value; cpu = numericUpDownCPU8.Value; args = processArgs[n] = textBoxArgs8.Text; minimize = checkBoxMinimize8.Checked; priority = comboBoxPriority8.SelectedIndex; break;
                case 9: processName = textBoxPath9.Text; sleepBefore = numericUpDownDelay9Before.Value; sleepAfter = numericUpDownDelay9After.Value; cpu = numericUpDownCPU9.Value; args = processArgs[n] = textBoxArgs9.Text; minimize = checkBoxMinimize9.Checked; priority = comboBoxPriority9.SelectedIndex; break;
            }
            Thread.Sleep((int)sleepBefore * 1000);
            if (process[n] == null)
            {
                process[n] = new Process();
            }
            if (processName.Length < 1) return; // empty process name
            if (!File.Exists(processName))
            {
                MessageBox.Show("File does not exist\n" + processName);
                SetStartStop(n, false, false);
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
            if (ProcessIsRunning(n)) return; // already running
            if (minimize)
            {
                process[n].StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }
            try
            {
                process[n].Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            //processID[n] = process[n].Id;
            Thread.Sleep((int)sleepAfter * 1000);
            try
            {
                process[n].WaitForInputIdle();
            }
            catch (Exception)
            {
                //MessageBox.Show("Here5");
                // catch error on non-GUI processes
            }
            if (priorityClass == ProcessPriorityClass.RealTime && !IsUserAdministrator())
            {
                MessageBox.Show("RealTime priority requires runnig as Admin.  Will run as High instead");
            }
            bool gotit = false;
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
                catch (Exception)
                {
                    MessageBox.Show("Error setting priority?  Administrator needed?");
                }
            } while (gotit == false);

            //process[n] = Process.GetProcessById(processID[n]);
            try
            {
                switch (n)
                {
                    case 1: labelWindowTitle1.Text = process[n].MainWindowTitle; pid1.Text = process[n].Id.ToString(); break;
                    case 2: labelWindowTitle2.Text = process[n].MainWindowTitle; pid2.Text = process[n].Id.ToString(); break;
                    case 3: labelWindowTitle3.Text = process[n].MainWindowTitle; pid3.Text = process[n].Id.ToString(); break;
                    case 4: labelWindowTitle4.Text = process[n].MainWindowTitle; pid4.Text = process[n].Id.ToString(); break;
                    case 5: labelWindowTitle5.Text = process[n].MainWindowTitle; pid5.Text = process[n].Id.ToString(); break;
                    case 6: labelWindowTitle6.Text = process[n].MainWindowTitle; pid6.Text = process[n].Id.ToString(); break;
                    case 7: labelWindowTitle7.Text = process[n].MainWindowTitle; pid7.Text = process[n].Id.ToString(); break;
                    case 8: labelWindowTitle8.Text = process[n].MainWindowTitle; pid8.Text = process[n].Id.ToString(); break;
                    case 9: labelWindowTitle9.Text = process[n].MainWindowTitle; pid9.Text = process[n].Id.ToString(); break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            TimeSpan ttime;
            TimeSpan ttimeDiff;
            cpu *= 10;  // Conver CPU % to millseconds
            do
            { // sleep for 200ms and wait for < desired CPU time
                ttime = process[n].TotalProcessorTime;
                Thread.Sleep(200);
                TimeSpan ttime2 = process[n].TotalProcessorTime;
                ttimeDiff = ttime2.Subtract(ttime);
            } while (ttimeDiff.TotalMilliseconds * 5 > (int)cpu);
            buttonStop1.Enabled = true;
            /*  Changing logic to use sequence # instead of next to keep it the same as the stop value
            try
            {
                int inext = int.Parse(next);
                if (!modifierKeys.HasFlag(Keys.Shift))
                {
                    processNext((inext + 1).ToString());
                }
            }
            catch
            {
                // nothing to do really -- string might be empty/invalid
            }
            */
            //richTextBox1.AppendText("Process#" + n + ", HandleCount=" + process[n].HandleCount);
        }

        private bool ProcessStop(int n, Keys modifierKeys)
        {
            //richTextBox1.AppendText("Stopping process#" + n + ", SafeHandle=" + process[n].Handle +"\n");
            //richTextBox1.AppendText("Process#" + n + ", HandleCount=" + process[n].HandleCount);
            if (!ProcessIsRunning(n)) // don't need to stop it then
            {
                return true;
            }
            timer1.Stop();
            Cursor.Current = Cursors.WaitCursor;
            if (modifierKeys.HasFlag(Keys.Alt)) // alt-click stops all
            {
                if (MessageBox.Show("This will stop all processes.  Are you sure?", "StartMe Stop All", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    Keys newKeys = ModifierKeys & Keys.Control; // Just keep the ctrl key
                    for (int i = 1; i < 10; ++i)
                    {
                        ProcessStop(i, newKeys);
                    }
                }
                //timer1.Start();
                return true;
            }
            if (modifierKeys.HasFlag(Keys.Control) && ProcessIsRunning(n) && !modifierKeys.HasFlag(Keys.Alt))
            {
                if (MessageBox.Show("This will impolitely stop this process.  Are you sure?", "StartMe", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    process[n].Kill();
                    //process[n].Close();
                }
                Cursor.Current = Cursors.Default;
                //timer1.Start();
                return true;
            }

            bool kill = false;
            decimal sleep = 0;
            switch (n)
            {
                case 1: sleep = numericUpDownDelayStop1.Value; kill = checkBoxKill1.Checked; break;
                case 2: sleep = numericUpDownDelayStop2.Value; kill = checkBoxKill2.Checked; break;
                case 3: sleep = numericUpDownDelayStop3.Value; kill = checkBoxKill3.Checked; break;
                case 4: sleep = numericUpDownDelayStop4.Value; kill = checkBoxKill4.Checked; break;
                case 5: sleep = numericUpDownDelayStop5.Value; kill = checkBoxKill5.Checked; break;
                case 6: sleep = numericUpDownDelayStop6.Value; kill = checkBoxKill6.Checked; break;
                case 7: sleep = numericUpDownDelayStop7.Value; kill = checkBoxKill7.Checked; break;
                case 8: sleep = numericUpDownDelayStop8.Value; kill = checkBoxKill8.Checked; break;
                case 9: sleep = numericUpDownDelayStop9.Value; kill = checkBoxKill9.Checked; break;
            }
            bool timeout = false;
            int loops = 0;
            while (ProcessIsRunning(n) && !timeout)
            {
                try
                {
                    ProcessUpdate();
                    bool ok = process[n].CloseMainWindow();
                    if (!ok || kill)
                    {
                        process[n].Kill();
                        process[n].Dispose();
                    }
                    Thread.Sleep(1000);
                    ProcessSendKeys(n, false);
                    while (loops < sleep && ProcessIsRunning(n))
                    {
                        Thread.Sleep(1000);
                        ++loops;
                    }
                    if (loops >= sleep) timeout = true;
                    ProcessUpdate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problem stopping task#" + n + "\n" + ex.Message);
                    //timeout = true;
                    return false;
                }
                //return true;
            }

            if (timeout && ProcessIsRunning(n))
            {
                MessageBox.Show("Process " + process[n].ProcessName + " did not terminate", "StartMe warning", MessageBoxButtons.OK);
                Cursor.Current = Cursors.Default;
                //timer1.Start();
                return false;
            }
            Cursor.Current = Cursors.Default;
            //timer1.Start();
            return true;
        }

        private void ButtonStop1_Click(object sender, EventArgs e)
        {
            if (ProcessStop(1, ModifierKeys))
            {
                buttonStart1.Enabled = true;
                buttonStop1.Enabled = false;
                pid1.Text = "";
            }
        }

        private void ButtonStop2_Click(object sender, EventArgs e)
        {
            if (ProcessStop(2, ModifierKeys))
            {
                buttonStart2.Enabled = true;
                buttonStop2.Enabled = false;
                pid2.Text = "";
            }

        }

        private void ButtonStop3_Click(object sender, EventArgs e)
        {
            if (ProcessStop(3, ModifierKeys))
            {
                buttonStart3.Enabled = true;
                buttonStop3.Enabled = false;
                pid3.Text = "";
            }

        }

        private void ButtonStop4_Click(object sender, EventArgs e)
        {
            if (ProcessStop(4, ModifierKeys))
            {
                buttonStart4.Enabled = true;
                buttonStop4.Enabled = false;
                pid4.Text = "";
            }

        }

        private void ButtonStop5_Click(object sender, EventArgs e)
        {
            if (ProcessStop(5, ModifierKeys))
            {
                buttonStart5.Enabled = true;
                buttonStop5.Enabled = false;
                pid5.Text = "";
            }

        }

        private void ButtonStop6_Click(object sender, EventArgs e)
        {
            if (ProcessStop(6, ModifierKeys))
            {
                buttonStart6.Enabled = true;
                buttonStop6.Enabled = false;
                pid6.Text = "";
            }

        }

        private void ButtonStop7_Click(object sender, EventArgs e)
        {
            if (ProcessStop(7, ModifierKeys))
            {
                buttonStart7.Enabled = true;
                buttonStop7.Enabled = false;
                pid7.Text = "";
            }

        }

        private void ButtonStop8_Click(object sender, EventArgs e)
        {
            if (ProcessStop(8, ModifierKeys))
            {
                buttonStart8.Enabled = true;
                buttonStop8.Enabled = false;
                pid8.Text = "";
            }

        }

        private void ButtonStop9_Click(object sender, EventArgs e)
        {
            if (ProcessStop(9, ModifierKeys))
            {
                buttonStart9.Enabled = true;
                buttonStop9.Enabled = false;
                pid9.Text = "";
            }
        }

        private void StartAll()
        {
            int next = 1;
            while (next <= 9)
            {
                String snext = next.ToString();
                if (snext.Equals(textBoxStart1Sequence.Text))
                {
                    ProcessStart(1, ModifierKeys);
                }
                if (snext.Equals(textBoxStart2Sequence.Text))
                {
                    ProcessStart(2, ModifierKeys);
                }
                if (snext.Equals(textBoxStart3Sequence.Text))
                {
                    ProcessStart(3, ModifierKeys);
                }
                if (snext.Equals(textBoxStart4Sequence.Text))
                {
                    ProcessStart(4, ModifierKeys);
                }
                if (snext.Equals(textBoxStart5Sequence.Text))
                {
                    ProcessStart(5, ModifierKeys);
                }
                if (snext.Equals(textBoxStart6Sequence.Text))
                {
                    ProcessStart(6, ModifierKeys);
                }
                if (snext.Equals(textBoxStart7Sequence.Text))
                {
                    ProcessStart(7, ModifierKeys);
                }
                if (snext.Equals(textBoxStart8Sequence.Text))
                {
                    ProcessStart(8, ModifierKeys);
                }
                if (snext.Equals(textBoxStart9Sequence.Text))
                {
                    ProcessStart(9, ModifierKeys);
                }
                ++next;
            }
            labelStatusMessage.Text = "All Processes Started";
        }
        private void StopAll()
        {
            bool stopped = false;
            int next = 1;
            // First off wet get the sequence of startups
            while (next <= 9)
            {
                String snext = next.ToString();
                labelStatusMessage.Text = "Stopping Process#" + snext;
                if (snext.Equals(textBoxStart1Stop.Text))
                {
                    stopped = true;
                    ProcessStop(1, ModifierKeys);
                }
                if (snext.Equals(textBoxStart2Stop.Text))
                {
                    stopped = true;
                    ProcessStop(2, ModifierKeys);
                }
                if (snext.Equals(textBoxStart3Stop.Text))
                {
                    stopped = true;
                    ProcessStop(3, ModifierKeys);
                }
                if (snext.Equals(textBoxStart4Stop.Text))
                {
                    stopped = true;
                    ProcessStop(4, ModifierKeys);
                }
                if (snext.Equals(textBoxStart5Stop.Text))
                {
                    stopped = true;
                    ProcessStop(5, ModifierKeys);
                }
                if (snext.Equals(textBoxStart6Stop.Text))
                {
                    stopped = true;
                    ProcessStop(6, ModifierKeys);
                }
                if (snext.Equals(textBoxStart7Stop.Text))
                {
                    stopped = true;
                    ProcessStop(7, ModifierKeys);
                }
                if (snext.Equals(textBoxStart8Stop.Text))
                {
                    stopped = true;
                    ProcessStop(8, ModifierKeys);
                }
                if (snext.Equals(textBoxStart9Stop.Text))
                {
                    stopped = true;
                    ProcessStop(9, ModifierKeys);
                }
                ++next;
            }
            // If we stopped anything at all we return
            if (stopped)
            {
                labelStatusMessage.Text = "Processes have been stopped";
                return;
            }
            // Otherwise we stop all in reverse sequence
            next = 9;
            while (next > 0)
            {
                String snext = next.ToString();
                labelStatusMessage.Text = "Stopping Process#" + snext;
                if (snext.Equals(textBoxStart1Sequence.Text))
                {
                    ProcessStop(1, ModifierKeys);
                }
                if (snext.Equals(textBoxStart2Sequence.Text))
                {
                    ProcessStop(2, ModifierKeys);
                }
                if (snext.Equals(textBoxStart3Sequence.Text))
                {
                    ProcessStop(3, ModifierKeys);
                }
                if (snext.Equals(textBoxStart4Sequence.Text))
                {
                    ProcessStop(4, ModifierKeys);
                }
                if (snext.Equals(textBoxStart5Sequence.Text))
                {
                    ProcessStop(5, ModifierKeys);
                }
                if (snext.Equals(textBoxStart6Sequence.Text))
                {
                    ProcessStop(6, ModifierKeys);
                }
                if (snext.Equals(textBoxStart7Sequence.Text))
                {
                    ProcessStop(7, ModifierKeys);
                }
                if (snext.Equals(textBoxStart8Sequence.Text))
                {
                    ProcessStop(8, ModifierKeys);
                }
                if (snext.Equals(textBoxStart9Sequence.Text))
                {
                    ProcessStop(9, ModifierKeys);
                }
                --next;
            }
            labelStatusMessage.Text = "All Processes stopped";
        }

        private void ButtonStopAll_Click_1(object sender, EventArgs e)
        {
            StopAll();
        }

        private void TextBoxPath1_TextChanged(object sender, EventArgs e)
        {
        }

        private String FileGet()
        {
            FileDialog fileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Executables (*.exe *.bat, *.cmd)|*.exe;*.bat;*.cmd|All Files (*.*) |*.*"
            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                return fileDialog.FileName;
            }
            return null;
        }

        private void ButtonFile1_Click(object sender, EventArgs e)
        {
            String file = FileGet();
            if (file != null)
            {
                textBoxPath1.Text = file;
                SetPathColor(1, Color.Black);
                ProcessIsRunning(1);
            }
        }

        private void ButtonFile2_Click(object sender, EventArgs e)
        {
            String file = FileGet();
            if (file != null)
            {
                textBoxPath2.Text = file;
                SetPathColor(2, Color.Black);
                ProcessIsRunning(2);
            }
        }

        private void ButtonFile3_Click(object sender, EventArgs e)
        {
            String file = FileGet();
            if (file != null)
            {
                textBoxPath3.Text = file;
                SetPathColor(3, Color.Black);
                ProcessIsRunning(3);
            }
        }

        private void ButtonFile4_Click(object sender, EventArgs e)
        {
            String file = FileGet();
            if (file != null)
            {
                textBoxPath4.Text = file;
                SetPathColor(4, Color.Black);
                ProcessIsRunning(4);

            }
        }

        private void ButtonFile5_Click(object sender, EventArgs e)
        {
            String file = FileGet();
            if (file != null)
            {
                textBoxPath5.Text = file;
                SetPathColor(5, Color.Black);
                ProcessIsRunning(5);
            }
        }

        private void ButtonFile6_Click(object sender, EventArgs e)
        {
            String file = FileGet();
            if (file != null)
            {
                textBoxPath6.Text = file;
                SetPathColor(6, Color.Black);
                ProcessIsRunning(6);
            }
        }

        private void ButtonFile7_Click(object sender, EventArgs e)
        {
            String file = FileGet();
            if (file != null)
            {
                textBoxPath7.Text = file;
                SetPathColor(7, Color.Black);
                ProcessIsRunning(7);
            }
        }

        private void ButtonFile8_Click(object sender, EventArgs e)
        {
            String file = FileGet();
            if (file != null)
            {
                textBoxPath8.Text = file;
                SetPathColor(8, Color.Black);
                ProcessIsRunning(8);
            }
        }

        private void ButtonFile9_Click(object sender, EventArgs e)
        {
            String file = FileGet();
            if (file != null)
            {
                textBoxPath9.Text = file;
                SetPathColor(9, Color.Black);
                ProcessIsRunning(9);
            }
        }

        private void Form1Closing(object sender, FormClosingEventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent);
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
                        //MessageBox.Show("Here3");
                        continue;
                    }
                }
                switch (i)
                {
                    case 1: buttonStart1.Enabled = !running; buttonStop1.Enabled = running; break;
                    case 2: buttonStart2.Enabled = !running; buttonStop2.Enabled = running; break;
                    case 3: buttonStart3.Enabled = !running; buttonStop3.Enabled = running; break;
                    case 4: buttonStart4.Enabled = !running; buttonStop4.Enabled = running; break;
                    case 5: buttonStart5.Enabled = !running; buttonStop5.Enabled = running; break;
                    case 6: buttonStart6.Enabled = !running; buttonStop6.Enabled = running; break;
                    case 7: buttonStart7.Enabled = !running; buttonStop7.Enabled = running; break;
                    case 8: buttonStart8.Enabled = !running; buttonStop8.Enabled = running; break;
                    case 9: buttonStart9.Enabled = !running; buttonStop9.Enabled = running; break;
                }
            }
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void AutoStart()
#pragma warning restore IDE0051 // Remove unused private members
        {
            timer1.Enabled = false;
            Keys keys = Keys.None;
            if (checkBoxAutoStart1.Checked)
            {
                ProcessStart(1, keys);
                ProcessUpdate();
                Application.DoEvents();
            }
            if (checkBoxAutoStart2.Checked)
            {
                ProcessStart(2, keys);
                ProcessUpdate();
                Application.DoEvents();
            }
            if (checkBoxAutoStart3.Checked)
            {
                ProcessStart(3, keys);
                ProcessUpdate();
                Application.DoEvents();
            }
            if (checkBoxAutoStart4.Checked)
            {
                ProcessStart(4, keys);
                ProcessUpdate();
                Application.DoEvents();
            }
            if (checkBoxAutoStart5.Checked)
            {
                ProcessStart(5, keys);
                ProcessUpdate();
                Application.DoEvents();
            }
            if (checkBoxAutoStart6.Checked)
            {
                ProcessStart(6, keys);
                ProcessUpdate();
                Application.DoEvents();
            }
            if (checkBoxAutoStart7.Checked)
            {
                ProcessStart(7, keys);
                ProcessUpdate();
                Application.DoEvents();
            }
            if (checkBoxAutoStart8.Checked)
            {
                ProcessStart(8, keys);
                ProcessUpdate();
                Application.DoEvents();
            }
            if (checkBoxAutoStart9.Checked)
            {
                ProcessStart(9, keys);
                ProcessUpdate();
                Application.DoEvents();
            }
            labelStatusMessage.Text = "All Processes Started";
            timer1.Enabled = true;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            if (autoStartDone == false && noauto == false)
            {
                autoStartDone = true;
                //AutoStart();
            }
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
            TextBox text = (TextBox)sender;
            String[] tokens = text.Text.Split(' ');
            if (tokens[0].Length == 0) return;
            foreach (String s in tokens)
            {
                try
                {
                    int n = Int32.Parse(s);
                    if (n < 1 || n > 9)
                    {
                        MessageBox.Show("Only space delmiited values 1 through 9 can be used here");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error converting \"" + s + "\" to integer\n" + ex.StackTrace);
                }
            }
        }
        private List<String> SettingsGetKeys()
        {
            List<String> keys = new List<string>();
            var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            if (!File.Exists(userConfig))
            {
                MessageBox.Show("Cnfig file doesn't exist?\n" + userConfig);
                return null;
            }
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
            AutoStartUpdate();

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
            Properties.Settings.Default.StartDelay1Before = numericUpDownDelay1Before.Value;
            Properties.Settings.Default.StartDelay2Before = numericUpDownDelay2Before.Value;
            Properties.Settings.Default.StartDelay3Before = numericUpDownDelay3Before.Value;
            Properties.Settings.Default.StartDelay4Before = numericUpDownDelay4Before.Value;
            Properties.Settings.Default.StartDelay5Before = numericUpDownDelay5Before.Value;
            Properties.Settings.Default.StartDelay6Before = numericUpDownDelay6Before.Value;
            Properties.Settings.Default.StartDelay7Before = numericUpDownDelay7Before.Value;
            Properties.Settings.Default.StartDelay8Before = numericUpDownDelay8Before.Value;
            Properties.Settings.Default.StartDelay9Before = numericUpDownDelay9Before.Value;

            //8
            Properties.Settings.Default.CPU1 = numericUpDownCPU1.Value;
            Properties.Settings.Default.CPU2 = numericUpDownCPU2.Value;
            Properties.Settings.Default.CPU3 = numericUpDownCPU3.Value;
            Properties.Settings.Default.CPU4 = numericUpDownCPU4.Value;
            Properties.Settings.Default.CPU5 = numericUpDownCPU5.Value;
            Properties.Settings.Default.CPU6 = numericUpDownCPU6.Value;
            Properties.Settings.Default.CPU7 = numericUpDownCPU7.Value;
            Properties.Settings.Default.CPU8 = numericUpDownCPU8.Value;
            Properties.Settings.Default.CPU9 = numericUpDownCPU9.Value;

            //9
            Properties.Settings.Default.KeysStart1 = textBoxStart1.Text;
            Properties.Settings.Default.KeysStart2 = textBoxStart2.Text;
            Properties.Settings.Default.KeysStart3 = textBoxStart3.Text;
            Properties.Settings.Default.KeysStart4 = textBoxStart4.Text;
            Properties.Settings.Default.KeysStart5 = textBoxStart5.Text;
            Properties.Settings.Default.KeysStart6 = textBoxStart6.Text;
            Properties.Settings.Default.KeysStart7 = textBoxStart7.Text;
            Properties.Settings.Default.KeysStart8 = textBoxStart8.Text;
            Properties.Settings.Default.KeysStart9 = textBoxStart9.Text;

            //10
            Properties.Settings.Default.Start1Next = textBoxStart1Sequence.Text;
            Properties.Settings.Default.Start2Next = textBoxStart2Sequence.Text;
            Properties.Settings.Default.Start3Next = textBoxStart3Sequence.Text;
            Properties.Settings.Default.Start4Next = textBoxStart4Sequence.Text;
            Properties.Settings.Default.Start5Next = textBoxStart5Sequence.Text;
            Properties.Settings.Default.Start6Next = textBoxStart6Sequence.Text;
            Properties.Settings.Default.Start7Next = textBoxStart7Sequence.Text;
            Properties.Settings.Default.Start8Next = textBoxStart8Sequence.Text;
            Properties.Settings.Default.Start9Next = textBoxStart9Sequence.Text;

            //11
            Properties.Settings.Default.Start1Stop = textBoxStart1Stop.Text;
            Properties.Settings.Default.Start2Stop = textBoxStart2Stop.Text;
            Properties.Settings.Default.Start3Stop = textBoxStart3Stop.Text;
            Properties.Settings.Default.Start4Stop = textBoxStart4Stop.Text;
            Properties.Settings.Default.Start5Stop = textBoxStart5Stop.Text;
            Properties.Settings.Default.Start6Stop = textBoxStart6Stop.Text;
            Properties.Settings.Default.Start7Stop = textBoxStart7Stop.Text;
            Properties.Settings.Default.Start8Stop = textBoxStart8Stop.Text;
            Properties.Settings.Default.Start9Stop = textBoxStart9Stop.Text;

            //12
            Properties.Settings.Default.StopDelay1 = numericUpDownDelayStop1.Value;
            Properties.Settings.Default.StopDelay2 = numericUpDownDelayStop2.Value;
            Properties.Settings.Default.StopDelay3 = numericUpDownDelayStop3.Value;
            Properties.Settings.Default.StopDelay4 = numericUpDownDelayStop4.Value;
            Properties.Settings.Default.StopDelay5 = numericUpDownDelayStop5.Value;
            Properties.Settings.Default.StopDelay6 = numericUpDownDelayStop6.Value;
            Properties.Settings.Default.StopDelay7 = numericUpDownDelayStop7.Value;
            Properties.Settings.Default.StopDelay8 = numericUpDownDelayStop8.Value;
            Properties.Settings.Default.StopDelay9 = numericUpDownDelayStop9.Value;

            //13
            Properties.Settings.Default.KeysStop1 = textBoxStop1.Text;
            Properties.Settings.Default.KeysStop2 = textBoxStop2.Text;
            Properties.Settings.Default.KeysStop3 = textBoxStop3.Text;
            Properties.Settings.Default.KeysStop4 = textBoxStop4.Text;
            Properties.Settings.Default.KeysStop5 = textBoxStop5.Text;
            Properties.Settings.Default.KeysStop6 = textBoxStop6.Text;
            Properties.Settings.Default.KeysStop7 = textBoxStop7.Text;
            Properties.Settings.Default.KeysStop8 = textBoxStop8.Text;
            Properties.Settings.Default.KeysStop9 = textBoxStop9.Text;

            //14
            Properties.Settings.Default.Minimize = checkBoxMinimize.Checked;

            //15
            Properties.Settings.Default.CloseAll = checkBoxCloseAll.Checked;

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

        private void SettingsLoad(String key)
        {
            var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

            if (!(File.Exists(userConfig)))
            {
                MessageBox.Show("Config file :" + key + "not found:\n" + userConfig);
                return;
            }
            if (key.Length == 0) key = "Default";
            settingsKeys = SettingsGetKeys();
            if (!settingsKeys.Contains(key, StringComparer.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("Key '" + key + "' not found in user.config");
                return;
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
            checkBoxMinimize.Checked = Properties.Settings.Default.Minimize;
            checkBoxCloseAll.Checked = Properties.Settings.Default.CloseAll;

            textBoxPath1.Text = Properties.Settings.Default.Path1;
            textBoxPath2.Text = Properties.Settings.Default.Path2;
            textBoxPath3.Text = Properties.Settings.Default.Path3;
            textBoxPath4.Text = Properties.Settings.Default.Path4;
            textBoxPath5.Text = Properties.Settings.Default.Path5;
            textBoxPath6.Text = Properties.Settings.Default.Path6;
            textBoxPath7.Text = Properties.Settings.Default.Path7;
            textBoxPath8.Text = Properties.Settings.Default.Path8;
            textBoxPath9.Text = Properties.Settings.Default.Path9;

            checkBoxAutoStart1.Checked = Properties.Settings.Default.AutoStart1;
            checkBoxAutoStart2.Checked = Properties.Settings.Default.AutoStart2;
            checkBoxAutoStart3.Checked = Properties.Settings.Default.AutoStart3;
            checkBoxAutoStart4.Checked = Properties.Settings.Default.AutoStart4;
            checkBoxAutoStart5.Checked = Properties.Settings.Default.AutoStart5;
            checkBoxAutoStart6.Checked = Properties.Settings.Default.AutoStart6;
            checkBoxAutoStart7.Checked = Properties.Settings.Default.AutoStart7;
            checkBoxAutoStart8.Checked = Properties.Settings.Default.AutoStart8;
            checkBoxAutoStart9.Checked = Properties.Settings.Default.AutoStart9;

            numericUpDownDelay1Before.Value = Properties.Settings.Default.StartDelay1Before;
            numericUpDownDelay2Before.Value = Properties.Settings.Default.StartDelay2Before;
            numericUpDownDelay3Before.Value = Properties.Settings.Default.StartDelay3Before;
            numericUpDownDelay4Before.Value = Properties.Settings.Default.StartDelay4Before;
            numericUpDownDelay5Before.Value = Properties.Settings.Default.StartDelay5Before;
            numericUpDownDelay6Before.Value = Properties.Settings.Default.StartDelay6Before;
            numericUpDownDelay7Before.Value = Properties.Settings.Default.StartDelay7Before;
            numericUpDownDelay8Before.Value = Properties.Settings.Default.StartDelay8Before;
            numericUpDownDelay9Before.Value = Properties.Settings.Default.StartDelay9Before;

            textBoxStart1.Text = Properties.Settings.Default.KeysStart1;
            textBoxStart2.Text = Properties.Settings.Default.KeysStart2;
            textBoxStart3.Text = Properties.Settings.Default.KeysStart3;
            textBoxStart4.Text = Properties.Settings.Default.KeysStart4;
            textBoxStart5.Text = Properties.Settings.Default.KeysStart5;
            textBoxStart6.Text = Properties.Settings.Default.KeysStart6;
            textBoxStart7.Text = Properties.Settings.Default.KeysStart7;
            textBoxStart9.Text = Properties.Settings.Default.KeysStart8;
            textBoxStart1.Text = Properties.Settings.Default.KeysStart9;

            textBoxStart1Sequence.Text = Properties.Settings.Default.Start1Next;
            textBoxStart2Sequence.Text = Properties.Settings.Default.Start2Next;
            textBoxStart3Sequence.Text = Properties.Settings.Default.Start3Next;
            textBoxStart4Sequence.Text = Properties.Settings.Default.Start4Next;
            textBoxStart5Sequence.Text = Properties.Settings.Default.Start5Next;
            textBoxStart6Sequence.Text = Properties.Settings.Default.Start6Next;
            textBoxStart7Sequence.Text = Properties.Settings.Default.Start7Next;
            textBoxStart8Sequence.Text = Properties.Settings.Default.Start8Next;
            textBoxStart9Sequence.Text = Properties.Settings.Default.Start9Next;

            textBoxStart1Stop.Text = Properties.Settings.Default.Start1Stop;
            textBoxStart2Stop.Text = Properties.Settings.Default.Start2Stop;
            textBoxStart3Stop.Text = Properties.Settings.Default.Start3Stop;
            textBoxStart4Stop.Text = Properties.Settings.Default.Start4Stop;
            textBoxStart5Stop.Text = Properties.Settings.Default.Start5Stop;
            textBoxStart6Stop.Text = Properties.Settings.Default.Start6Stop;
            textBoxStart7Stop.Text = Properties.Settings.Default.Start7Stop;
            textBoxStart8Stop.Text = Properties.Settings.Default.Start8Stop;
            textBoxStart9Stop.Text = Properties.Settings.Default.Start9Stop;

            numericUpDownDelayStop1.Value = Properties.Settings.Default.StopDelay1;
            numericUpDownDelayStop2.Value = Properties.Settings.Default.StopDelay2;
            numericUpDownDelayStop3.Value = Properties.Settings.Default.StopDelay3;
            numericUpDownDelayStop4.Value = Properties.Settings.Default.StopDelay4;
            numericUpDownDelayStop5.Value = Properties.Settings.Default.StopDelay5;
            numericUpDownDelayStop6.Value = Properties.Settings.Default.StopDelay6;
            numericUpDownDelayStop7.Value = Properties.Settings.Default.StopDelay7;
            numericUpDownDelayStop8.Value = Properties.Settings.Default.StopDelay8;
            numericUpDownDelayStop9.Value = Properties.Settings.Default.StopDelay9;

            textBoxStop1.Text = Properties.Settings.Default.KeysStop1;
            textBoxStop2.Text = Properties.Settings.Default.KeysStop2;
            textBoxStop3.Text = Properties.Settings.Default.KeysStop3;
            textBoxStop4.Text = Properties.Settings.Default.KeysStop4;
            textBoxStop5.Text = Properties.Settings.Default.KeysStop5;
            textBoxStop6.Text = Properties.Settings.Default.KeysStop6;
            textBoxStop7.Text = Properties.Settings.Default.KeysStop7;
            textBoxStop8.Text = Properties.Settings.Default.KeysStop8;
            textBoxStop9.Text = Properties.Settings.Default.KeysStop9;

            numericUpDownCPU1.Value = Properties.Settings.Default.CPU1;
            numericUpDownCPU2.Value = Properties.Settings.Default.CPU2;
            numericUpDownCPU3.Value = Properties.Settings.Default.CPU3;
            numericUpDownCPU4.Value = Properties.Settings.Default.CPU4;
            numericUpDownCPU5.Value = Properties.Settings.Default.CPU5;
            numericUpDownCPU6.Value = Properties.Settings.Default.CPU6;
            numericUpDownCPU7.Value = Properties.Settings.Default.CPU7;
            numericUpDownCPU8.Value = Properties.Settings.Default.CPU8;
            numericUpDownCPU9.Value = Properties.Settings.Default.CPU9;

            textBoxArgs1.Text = Properties.Settings.Default.Args1;
            textBoxArgs2.Text = Properties.Settings.Default.Args2;
            textBoxArgs3.Text = Properties.Settings.Default.Args3;
            textBoxArgs4.Text = Properties.Settings.Default.Args4;
            textBoxArgs5.Text = Properties.Settings.Default.Args5;
            textBoxArgs6.Text = Properties.Settings.Default.Args6;
            textBoxArgs7.Text = Properties.Settings.Default.Args7;
            textBoxArgs8.Text = Properties.Settings.Default.Args8;
            textBoxArgs9.Text = Properties.Settings.Default.Args9;

            checkBoxMinimize1.Checked = Properties.Settings.Default.Minimize1;
            checkBoxMinimize2.Checked = Properties.Settings.Default.Minimize2;
            checkBoxMinimize3.Checked = Properties.Settings.Default.Minimize3;
            checkBoxMinimize4.Checked = Properties.Settings.Default.Minimize4;
            checkBoxMinimize5.Checked = Properties.Settings.Default.Minimize5;
            checkBoxMinimize6.Checked = Properties.Settings.Default.Minimize6;
            checkBoxMinimize7.Checked = Properties.Settings.Default.Minimize7;
            checkBoxMinimize8.Checked = Properties.Settings.Default.Minimize8;
            checkBoxMinimize1.Checked = Properties.Settings.Default.Minimize9;

            checkBoxAdmin1.Checked = Properties.Settings.Default.Admin1;
            checkBoxAdmin2.Checked = Properties.Settings.Default.Admin2;
            checkBoxAdmin3.Checked = Properties.Settings.Default.Admin3;
            checkBoxAdmin4.Checked = Properties.Settings.Default.Admin4;
            checkBoxAdmin5.Checked = Properties.Settings.Default.Admin5;
            checkBoxAdmin6.Checked = Properties.Settings.Default.Admin6;
            checkBoxAdmin7.Checked = Properties.Settings.Default.Admin7;
            checkBoxAdmin8.Checked = Properties.Settings.Default.Admin8;
            checkBoxAdmin9.Checked = Properties.Settings.Default.Admin9;

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

            checkBoxKill1.Checked = Properties.Settings.Default.Kill1;
            checkBoxKill2.Checked = Properties.Settings.Default.Kill2;
            checkBoxKill3.Checked = Properties.Settings.Default.Kill3;
            checkBoxKill4.Checked = Properties.Settings.Default.Kill4;
            checkBoxKill5.Checked = Properties.Settings.Default.Kill5;
            checkBoxKill6.Checked = Properties.Settings.Default.Kill6;
            checkBoxKill7.Checked = Properties.Settings.Default.Kill7;
            checkBoxKill8.Checked = Properties.Settings.Default.Kill8;
            checkBoxKill9.Checked = Properties.Settings.Default.Kill9;

            numericUpDownDelay1After.Value = Properties.Settings.Default.StartDelay1After;
            numericUpDownDelay2After.Value = Properties.Settings.Default.StartDelay2After;
            numericUpDownDelay3After.Value = Properties.Settings.Default.StartDelay3After;
            numericUpDownDelay4After.Value = Properties.Settings.Default.StartDelay4After;
            numericUpDownDelay5After.Value = Properties.Settings.Default.StartDelay5After;
            numericUpDownDelay6After.Value = Properties.Settings.Default.StartDelay6After;
            numericUpDownDelay7After.Value = Properties.Settings.Default.StartDelay7After;
            numericUpDownDelay8After.Value = Properties.Settings.Default.StartDelay8After;
            numericUpDownDelay9After.Value = Properties.Settings.Default.StartDelay9After;

            //pid1.Text = Properties.Settings.Default.Pid1.ToString();
            //pid2.Text = Properties.Settings.Default.Pid2.ToString();
            //pid3.Text = Properties.Settings.Default.Pid3.ToString();
            //pid4.Text = Properties.Settings.Default.Pid4.ToString();
            //pid5.Text = Properties.Settings.Default.Pid5.ToString();
            //pid6.Text = Properties.Settings.Default.Pid6.ToString();
            //pid7.Text = Properties.Settings.Default.Pid7.ToString();
            //pid8.Text = Properties.Settings.Default.Pid8.ToString();
            //pid9.Text = Properties.Settings.Default.Pid9.ToString();

            if (textBoxPath1.Text.Length == 0)
            {
                // FLrig example
                textBoxPath1.Text = "C:\\Program Files (x86)\\flrig-1.3.40.34\\flrig.exe";
                numericUpDownDelay1Before.Value = 5;
                textBoxStart1Sequence.Text = "2";

                // rigctld
                textBoxPath2.Text = "C:\\WSJT\\wsjtx\\bin\\rigctld-wsjtx.exe";
                textBoxArgs2.Text = "-m 4 -vvv";
                textBoxStart2Sequence.Text = "3";

                // 
                textBoxPath3.Text = "C:\\Program Files (x86)\\IW3HMH\\Log4OM\\LogOMUI.exe";
                textBoxStop3.Text = "\"Awaiting Confirmation\" ENTER ";
                textBoxStart3Sequence.Text = "4";

                textBoxPath4.Text = "C:\\Program Files (x86)\\HamApps\\JTAlert\\JTAlert.exe";
                textBoxArgs4.Text = "/wsjtx";

                textBoxPath4.Text = "C:\\Program Files (x86)\\Fldigi-4.0.8\\fldigi.exe";
            }

            timer1.Interval = 1000;
            timer1.Enabled = true;

        }

        private void ComboBoxSettingsName_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item = comboBoxSettingsKey.SelectedItem.ToString();
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
            if (MessageBox.Show("Do you want to add a new configuration?", "StartMe Config", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (item == "Default") item = "";
                Properties.Settings.Default.SettingsKeyCurrent = item;
                SettingsLoad(Properties.Settings.Default.SettingsKeyCurrent);
            }
        }

        private void ComboBoxSettingsName_Leave(object sender, EventArgs e)
        {
            SettingsSave(Properties.Settings.Default.SettingsKeyCurrent); // save the old one
            String item = comboBoxSettingsKey.Text;
            if (!settingsKeys.Contains(item))
            {
                if (MessageBox.Show("Do you want to add a new configuration called '" + item + "'?", "StartMe Config", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Properties.Settings.Default.SettingsKeyCurrent = item;
                    if (MessageBox.Show("Do you want to reset the copy to default values?", "StartMe New Config", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Properties.Settings.Default.Reset();
                    }
                    SettingsLoad(item);
                    comboBoxSettingsKey.Items.Add(item);
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
                MessageBox.Show("Here1");
            }
            catch (Exception)
            {
                isAdmin = false;
                MessageBox.Show("Here2");
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
            HelpForm help = new HelpForm();
            help.Show();
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
                MessageBox.Show(ex.Message, "An error occurred in IsUserInAdminGroup",
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
                MessageBox.Show(ex.Message, "An error occurred in IsRunAsAdmin",
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
                    MessageBox.Show(ex.Message, "An error occurred in IsProcessElevated",
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
                    MessageBox.Show(ex.Message, "An error occurred in GetProcessIntegrityLevel",
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

        private void AutoStartUpdate()
        {
            textBoxStart1Sequence.Enabled = checkBoxAutoStart1.Checked;
            textBoxStart2Sequence.Enabled = checkBoxAutoStart2.Checked;
            textBoxStart3Sequence.Enabled = checkBoxAutoStart3.Checked;
            textBoxStart4Sequence.Enabled = checkBoxAutoStart4.Checked;
            textBoxStart5Sequence.Enabled = checkBoxAutoStart5.Checked;
            textBoxStart6Sequence.Enabled = checkBoxAutoStart6.Checked;
            textBoxStart7Sequence.Enabled = checkBoxAutoStart7.Checked;
            textBoxStart8Sequence.Enabled = checkBoxAutoStart8.Checked;
            textBoxStart9Sequence.Enabled = checkBoxAutoStart9.Checked;

        }

        private void CheckBoxAutoStart1_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartUpdate();
        }

        private void CheckBoxAutoStart2_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartUpdate();
        }

        private void CheckBoxAutoStart3_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartUpdate();
        }

        private void CheckBoxAutoStart4_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartUpdate();
        }

        private void CheckBoxAutoStart5_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartUpdate();
        }

        private void CheckBoxAutoStart6_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartUpdate();
        }

        private void CheckBoxAutoStart7_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartUpdate();
        }

        private void CheckBoxAutoStart8_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartUpdate();
        }

        private void CheckBoxAutoStart9_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartUpdate();
        }

        private void ButtonStartAll_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            StartAll();
            timer1.Enabled = true;
        }
    }
}