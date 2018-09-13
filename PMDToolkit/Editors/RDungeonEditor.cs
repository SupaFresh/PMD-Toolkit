using PMDToolkit.Data;
using System.Windows.Forms;

namespace PMDToolkit.Editors
{
    public partial class RDungeonEditor : Form
    {
        private int dungeonNum;

        public RDungeonEditor()
        {
            InitializeComponent();
        }

        public void LoadRDungeon(int index)
        {
            dungeonNum = index;
            RDungeonEntry entry = GameData.RDungeonDex[index];
        }

        public void SaveRDungeon()
        {
        }
    }
}