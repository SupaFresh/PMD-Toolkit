using System;
using System.Windows.Forms;

namespace PMDToolkit.Editors
{
    public partial class EditList : Form
    {
        public int ChosenEntry;

        public EditList()
        {
            InitializeComponent();

            ChosenEntry = -1;
        }

        public void AddEntries(string[] entries)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                lbxEntries.Items.Add((i + 1) + ": " + entries[i]);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (lbxEntries.SelectedIndex > -1)
            {
                ChosenEntry = lbxEntries.SelectedIndex;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}