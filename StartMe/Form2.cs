using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartMe
{
    public partial class FormConfigurations : Form
    {
        String config = "Default";
        public FormConfigurations()
        {
            InitializeComponent();
        }

        public String getConfig()
        {
            return config;
        }

        public void setCheckBox(List<String> configs, String lastConfig)
        {
            if (configs == null) return;
            int i = 0;
            foreach (String s in configs)
            {
                checkedListBox1.Items.Add(s);
                if (s.Equals(lastConfig))
                {
                    checkedListBox1.SelectedIndex = i;
                    checkedListBox1.SetItemChecked(i, true);
                }
                ++i;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            config = checkedListBox1.SelectedItem.ToString();
            this.Close();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String curItem = checkedListBox1.SelectedItem.ToString();
            
            for(int i=0;i< checkedListBox1.Items.Count;++i)
            {
                String thisItem = checkedListBox1.Items[i].ToString();
                if (thisItem.Equals(curItem))
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
                else
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
            //if (checkedListBox1.CheckedItems.Count > 1)
            //{
            //    Int32 checkedItemIndex = checkedListBox1.CheckedIndices[0];
             //   checkedListBox1.ItemCheck -= checkedListBox1_ItemCheck;
            //    checkedListBox1.SetItemChecked(checkedItemIndex, false);
            //    checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            //}
        }
    }
}
