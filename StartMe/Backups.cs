using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace StartMe
{
    public partial class Backups : Form
    {
        string userConfigFile = null;
        public Backups() => InitializeComponent();

        private void Backups_Load(object sender, EventArgs e)
        {
            buttonRestore.Enabled = false;
        }

        public void Backups_List(string filepath)
        {
            userConfigFile = filepath;
            string path = Path.GetDirectoryName(userConfigFile);
            for (int i = 1; i <= 9; ++i) {
                string fromFile = "user.config." + i;
                string fromFilePath = path + "\\" + fromFile;
                if (File.Exists(fromFilePath))
                {
                    string myTime = File.GetLastWriteTime(path + "\\" + fromFile).ToString("yyyyMMdd HH:mm");
                    checkedListBox1.Items[i - 1] = fromFile + "  - " + myTime;
                }
                else
                {
                    checkedListBox1.Items[i - 1] = fromFile + " - Not found";
                }
            }
            this.ShowDialog(); 
        }

        private void CheckedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = checkedListBox1.SelectedIndex;

            checkedListBox1.SetItemChecked(0, false);
            checkedListBox1.SetItemChecked(1, false);
            checkedListBox1.SetItemChecked(2, false);
            checkedListBox1.SetItemChecked(3, false);
            checkedListBox1.SetItemChecked(4, false);
            checkedListBox1.SetItemChecked(5, false);
            checkedListBox1.SetItemChecked(6, false);
            checkedListBox1.SetItemChecked(7, false);
            checkedListBox1.SetItemChecked(8, false);
            //checkedListBox1.SetItemChecked(9, false);
            try
            {
                checkedListBox1.SetItemChecked(i, true);
            }
            catch { 
                //continue
            }
            buttonRestore.Enabled = true;
        }

        private void ButtonQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonRestore_Click(object sender, EventArgs e)
        {
            int i = checkedListBox1.SelectedIndex;
            if (i < 0 || i > 4)
            {
                MessageBox.Show("Oops...select index for backup file out of range\nExpected 0-4 but got " + i);
                return;
            }
            if (checkedListBox1.Items[i].ToString().Contains("Not found"))
            {
                checkedListBox1.SetItemChecked(i,false);
                buttonRestore.Enabled = false;
                MessageBox.Show("Can't restore something that doesn't exist!!");
                return;
            }

            string path = Path.GetDirectoryName(userConfigFile);
            string fromFile = path + "\\user.config." + (i + 1);
            //MessageBox.Show("Restoring\n" + userConfigFile + "\nFrom" + fromFile);
            var result = MessageBox.Show("This will overwrite your user.config file...are you sure?", "StartMe Backups", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                File.Copy(fromFile, userConfigFile, true);
                MessageBox.Show("user.config restored from backup#" + (i + 1));
                this.Close();
            }
        }
    }
}
