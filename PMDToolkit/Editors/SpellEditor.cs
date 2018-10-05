using PMDToolkit.Data;
using PMDToolkit.Graphics;
using System;
using System.Windows.Forms;

namespace PMDToolkit.Editors
{
    public partial class SpellEditor : Form
    {
        private int spellNum;

        public SpellEditor()
        {
            InitializeComponent();

            Array types = Enum.GetValues(typeof(Enums.Element));
            for (int i = 0; i < types.Length; i++)
            {
                cbType.Items.Add(types.GetValue(i));
            }
            cbType.SelectedIndex = 0;

            Array categories = Enum.GetValues(typeof(Enums.MoveCategory));
            for (int i = 0; i < categories.Length; i++)
            {
                cbCategory.Items.Add(categories.GetValue(i));
            }
            cbCategory.SelectedIndex = 0;

            Array ranges = Enum.GetValues(typeof(Enums.RangeType));
            for (int i = 0; i < ranges.Length; i++)
            {
                cbRange.Items.Add(ranges.GetValue(i));
            }
            cbRange.SelectedIndex = 0;

            Array startAnims = Enum.GetValues(typeof(Logic.Display.MoveAnimationType));
            for (int i = 0; i < startAnims.Length; i++)
            {
                cbStartAnim.Items.Add(startAnims.GetValue(i));
            }
            cbStartAnim.SelectedIndex = 0;

            Array startUserAnims = Enum.GetValues(typeof(Logic.Display.CharSprite.ActionType));
            for (int i = 0; i < startUserAnims.Length; i++)
            {
                cbStartUser.Items.Add(startUserAnims.GetValue(i));
            }
            cbStartUser.SelectedIndex = 0;

            Array midAnims = Enum.GetValues(typeof(Logic.Display.MoveAnimationType));
            for (int i = 0; i < midAnims.Length; i++)
            {
                cbMidAnim.Items.Add(midAnims.GetValue(i));
            }
            cbMidAnim.SelectedIndex = 0;

            Array midUserAnims = Enum.GetValues(typeof(Logic.Display.CharSprite.ActionType));
            for (int i = 0; i < midUserAnims.Length; i++)
            {
                cbMidUser.Items.Add(midUserAnims.GetValue(i));
            }
            cbMidUser.SelectedIndex = 0;

            Array midTargetAnims = Enum.GetValues(typeof(Logic.Display.CharSprite.ActionType));
            for (int i = 0; i < midTargetAnims.Length; i++)
            {
                cbMidTarget.Items.Add(midTargetAnims.GetValue(i));
            }
            cbMidTarget.SelectedIndex = 0;

            Array endAnims = Enum.GetValues(typeof(Logic.Display.MoveAnimationType));
            for (int i = 0; i < endAnims.Length; i++)
            {
                cbEndAnim.Items.Add(endAnims.GetValue(i));
            }
            cbEndAnim.SelectedIndex = 0;

            Array endUserAnims = Enum.GetValues(typeof(Logic.Display.CharSprite.ActionType));
            for (int i = 0; i < endUserAnims.Length; i++)
            {
                cbEndUser.Items.Add(endUserAnims.GetValue(i));
            }
            cbEndUser.SelectedIndex = 0;

            Array endTargetAnims = Enum.GetValues(typeof(Logic.Display.CharSprite.ActionType));
            for (int i = 0; i < endTargetAnims.Length; i++)
            {
                cbEndTarget.Items.Add(endTargetAnims.GetValue(i));
            }
            cbEndTarget.SelectedIndex = 0;

            nudEffect.Maximum = int.MaxValue;

            nudEffect1.Minimum = int.MinValue;
            nudEffect1.Maximum = int.MaxValue;

            nudEffect2.Minimum = int.MinValue;
            nudEffect2.Maximum = int.MaxValue;

            nudEffect3.Minimum = int.MinValue;
            nudEffect3.Maximum = int.MaxValue;

            nudMobility.Maximum = int.MaxValue;

            nudStartIndex.Maximum = int.MaxValue;
            nudStartLength.Maximum = int.MaxValue;

            nudStartData1.Minimum = int.MinValue;
            nudStartData1.Maximum = int.MaxValue;
            nudStartData2.Minimum = int.MinValue;
            nudStartData2.Maximum = int.MaxValue;
            nudStartData3.Minimum = int.MinValue;
            nudStartData3.Maximum = int.MaxValue;

            nudStartUser1.Minimum = int.MinValue;
            nudStartUser1.Maximum = int.MaxValue;
            nudStartUser2.Minimum = int.MinValue;
            nudStartUser2.Maximum = int.MaxValue;
            nudStartUser3.Minimum = int.MinValue;
            nudStartUser3.Maximum = int.MaxValue;

            nudStartSound.Maximum = int.MaxValue;

            nudMidIndex.Maximum = int.MaxValue;
            nudMidLength.Maximum = int.MaxValue;

            nudMidData1.Minimum = int.MinValue;
            nudMidData1.Maximum = int.MaxValue;
            nudMidData2.Minimum = int.MinValue;
            nudMidData2.Maximum = int.MaxValue;
            nudMidData3.Minimum = int.MinValue;
            nudMidData3.Maximum = int.MaxValue;

            nudMidUser1.Minimum = int.MinValue;
            nudMidUser1.Maximum = int.MaxValue;
            nudMidUser2.Minimum = int.MinValue;
            nudMidUser2.Maximum = int.MaxValue;
            nudMidUser3.Minimum = int.MinValue;
            nudMidUser3.Maximum = int.MaxValue;

            nudMidTarget1.Minimum = int.MinValue;
            nudMidTarget1.Maximum = int.MaxValue;
            nudMidTarget2.Minimum = int.MinValue;
            nudMidTarget2.Maximum = int.MaxValue;
            nudMidTarget3.Minimum = int.MinValue;
            nudMidTarget3.Maximum = int.MaxValue;

            nudMidSound.Maximum = int.MaxValue;

            nudEndIndex.Maximum = int.MaxValue;
            nudEndLength.Maximum = int.MaxValue;

            nudEndData1.Minimum = int.MinValue;
            nudEndData1.Maximum = int.MaxValue;
            nudEndData2.Minimum = int.MinValue;
            nudEndData2.Maximum = int.MaxValue;
            nudEndData3.Minimum = int.MinValue;
            nudEndData3.Maximum = int.MaxValue;

            nudEndUser1.Minimum = int.MinValue;
            nudEndUser1.Maximum = int.MaxValue;
            nudEndUser2.Minimum = int.MinValue;
            nudEndUser2.Maximum = int.MaxValue;
            nudEndUser3.Minimum = int.MinValue;
            nudEndUser3.Maximum = int.MaxValue;

            nudEndTarget1.Minimum = int.MinValue;
            nudEndTarget1.Maximum = int.MaxValue;
            nudEndTarget2.Minimum = int.MinValue;
            nudEndTarget2.Maximum = int.MaxValue;
            nudEndTarget3.Minimum = int.MinValue;
            nudEndTarget3.Maximum = int.MaxValue;

            nudEndSound.Maximum = int.MaxValue;
        }

        public void LoadSpell(int index)
        {
            spellNum = index;
            MoveEntry entry = GameData.MoveDex[index];

            txtName.Text = entry.Name;
            nudPower.Value = entry.Power;
            nudAccuracy.Value = entry.Accuracy;
            txtDescription.Text = entry.Desc;

            cbType.SelectedIndex = (int)entry.Type;
            cbCategory.SelectedIndex = (int)entry.Category;
            nudEffect.Value = entry.Effect;
            nudEffect1.Value = entry.Effect1;
            nudEffect2.Value = entry.Effect2;
            nudEffect3.Value = entry.Effect3;

            chkContact.Checked = entry.Contact;
            chkSound.Checked = entry.SoundBased;
            chkFist.Checked = entry.FistBased;
            chkPulse.Checked = entry.PulseBased;
            chkBullet.Checked = entry.BulletBased;
            chkJaw.Checked = entry.JawBased;

            cbRange.SelectedIndex = (int)entry.Range.RangeType;
            nudDistance.Value = entry.Range.Distance;
            nudMobility.Value = entry.Range.Mobility;
            chkCorners.Checked = entry.Range.CutsCorners;
            chkSelf.Checked = entry.Range.HitsSelf;
            chkFriend.Checked = entry.Range.HitsFriend;
            chkFoe.Checked = entry.Range.HitsFoe;

            cbStartAnim.SelectedIndex = (int)entry.StartAnim.AnimType;
            nudStartIndex.Value = entry.StartAnim.AnimIndex;
            nudStartLength.Value = entry.StartAnim.FrameLength.ToMillisecs();
            nudStartData1.Value = entry.StartAnim.Anim1;
            nudStartData2.Value = entry.StartAnim.Anim2;
            nudStartData3.Value = entry.StartAnim.Anim3;

            cbStartUser.SelectedIndex = (int)entry.StartUserAnim.ActionType;

            nudStartSound.Value = entry.StartSound;

            cbMidAnim.SelectedIndex = (int)entry.MidAnim.AnimType;
            nudMidIndex.Value = entry.MidAnim.AnimIndex;
            nudMidLength.Value = entry.MidAnim.FrameLength.ToMillisecs();
            nudMidData1.Value = entry.MidAnim.Anim1;
            nudMidData2.Value = entry.MidAnim.Anim2;
            nudMidData3.Value = entry.MidAnim.Anim3;

            cbMidUser.SelectedIndex = (int)entry.MidUserAnim.ActionType;

            cbMidTarget.SelectedIndex = (int)entry.MidTargetAnim.ActionType;

            nudMidSound.Value = entry.MidSound;

            cbEndAnim.SelectedIndex = (int)entry.EndAnim.AnimType;
            nudEndIndex.Value = entry.EndAnim.AnimIndex;
            nudEndLength.Value = entry.EndAnim.FrameLength.ToMillisecs();
            nudEndData1.Value = entry.EndAnim.Anim1;
            nudEndData2.Value = entry.EndAnim.Anim2;
            nudEndData3.Value = entry.EndAnim.Anim3;

            cbEndUser.SelectedIndex = (int)entry.EndUserAnim.ActionType;

            cbEndTarget.SelectedIndex = (int)entry.EndTargetAnim.ActionType;

            nudEndSound.Value = entry.EndSound;
        }

        private MoveEntry GetEntry()
        {
            MoveEntry entry = new MoveEntry
            {
                Name = txtName.Text,
                Power = (int)nudPower.Value,
                Accuracy = (int)nudAccuracy.Value,
                Desc = txtDescription.Text,

                Type = (Enums.Element)cbType.SelectedIndex,
                Category = (Enums.MoveCategory)cbCategory.SelectedIndex,
                Effect = (int)nudEffect.Value,
                Effect1 = (int)nudEffect1.Value,
                Effect2 = (int)nudEffect2.Value,
                Effect3 = (int)nudEffect3.Value,

                Contact = chkContact.Checked,
                SoundBased = chkSound.Checked,
                FistBased = chkFist.Checked,
                PulseBased = chkPulse.Checked,
                BulletBased = chkBullet.Checked,
                JawBased = chkJaw.Checked
            };

            entry.Range.RangeType = (Enums.RangeType)cbRange.SelectedIndex;
            entry.Range.Distance = (int)nudDistance.Value;
            entry.Range.Mobility = (int)nudMobility.Value;
            entry.Range.CutsCorners = chkCorners.Checked;
            entry.Range.HitsSelf = chkSelf.Checked;
            entry.Range.HitsFriend = chkFriend.Checked;
            entry.Range.HitsFoe = chkFoe.Checked;

            entry.StartAnim.AnimType = (Logic.Display.MoveAnimationType)cbStartAnim.SelectedIndex;
            entry.StartAnim.AnimIndex = (int)nudStartIndex.Value;
            entry.StartAnim.FrameLength = RenderTime.FromMillisecs((int)nudStartLength.Value);
            entry.StartAnim.Anim1 = (int)nudStartData1.Value;
            entry.StartAnim.Anim2 = (int)nudStartData2.Value;
            entry.StartAnim.Anim3 = (int)nudStartData3.Value;

            entry.StartUserAnim.ActionType = (Logic.Display.CharSprite.ActionType)cbStartUser.SelectedIndex;

            entry.StartSound = (int)nudStartSound.Value;

            entry.MidAnim.AnimType = (Logic.Display.MoveAnimationType)cbMidAnim.SelectedIndex;
            entry.MidAnim.AnimIndex = (int)nudMidIndex.Value;
            entry.MidAnim.FrameLength = RenderTime.FromMillisecs((int)nudMidLength.Value);
            entry.MidAnim.Anim1 = (int)nudMidData1.Value;
            entry.MidAnim.Anim2 = (int)nudMidData2.Value;
            entry.MidAnim.Anim3 = (int)nudMidData3.Value;

            entry.MidUserAnim.ActionType = (Logic.Display.CharSprite.ActionType)cbMidUser.SelectedIndex;

            entry.MidTargetAnim.ActionType = (Logic.Display.CharSprite.ActionType)cbMidTarget.SelectedIndex;

            entry.MidSound = (int)nudMidSound.Value;

            entry.EndAnim.AnimType = (Logic.Display.MoveAnimationType)cbEndAnim.SelectedIndex;
            entry.EndAnim.AnimIndex = (int)nudEndIndex.Value;
            entry.EndAnim.FrameLength = RenderTime.FromMillisecs((int)nudEndLength.Value);
            entry.EndAnim.Anim1 = (int)nudEndData1.Value;
            entry.EndAnim.Anim2 = (int)nudEndData2.Value;
            entry.EndAnim.Anim3 = (int)nudEndData3.Value;

            entry.EndUserAnim.ActionType = (Logic.Display.CharSprite.ActionType)cbEndUser.SelectedIndex;

            entry.EndTargetAnim.ActionType = (Logic.Display.CharSprite.ActionType)cbEndTarget.SelectedIndex;

            entry.EndSound = (int)nudEndSound.Value;

            return entry;
        }

        public void SaveSpell()
        {
            MoveEntry entry = GetEntry();

            GameData.MoveDex[spellNum] = entry;
            GameData.MoveDex[spellNum].Save(spellNum);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            SaveSpell();
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            MoveEntry entry = GetEntry();

            Logic.Gameplay.Processor.MockAttack(entry);
        }

        private void Anim_Changed(object sender, EventArgs e)
        {
            if (chkAutoTest.Checked)
            {
                MoveEntry entry = GetEntry();
                Logic.Gameplay.Processor.MockAttack(entry);
            }
        }
    }
}