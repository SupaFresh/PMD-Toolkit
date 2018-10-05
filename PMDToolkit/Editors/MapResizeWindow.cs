using System;
using System.Windows.Forms;

namespace PMDToolkit.Editors
{
    public partial class MapResizeWindow : Form
    {
        public new int Width { get; set; }
        public new int Height { get; set; }

        public Maps.Direction8 ResizeDir { get; set; }

        public MapResizeWindow(int width, int height)
        {
            InitializeComponent();

            Width = width;
            Height = height;

            nudWidth.Value = width;
            nudHeight.Value = height;

            ResizeDir = Maps.Direction8.None;

            RefreshResizeDir();
        }

        private void RefreshResizeDir()
        {
            btnCenter.Text = "";
            btnBottom.Text = "";
            btnLeft.Text = "";
            btnTop.Text = "";
            btnRight.Text = "";
            btnBottomLeft.Text = "";
            btnTopLeft.Text = "";
            btnTopRight.Text = "";
            btnBottomRight.Text = "";

            switch (ResizeDir)
            {
                case Maps.Direction8.None:
                    btnCenter.Text = "X";
                    break;

                case Maps.Direction8.Down:
                    btnBottom.Text = "X";
                    break;

                case Maps.Direction8.Left:
                    btnLeft.Text = "X";
                    break;

                case Maps.Direction8.Up:
                    btnTop.Text = "X";
                    break;

                case Maps.Direction8.Right:
                    btnRight.Text = "X";
                    break;

                case Maps.Direction8.DownLeft:
                    btnBottomLeft.Text = "X";
                    break;

                case Maps.Direction8.UpLeft:
                    btnTopLeft.Text = "X";
                    break;

                case Maps.Direction8.UpRight:
                    btnTopRight.Text = "X";
                    break;

                case Maps.Direction8.DownRight:
                    btnBottomRight.Text = "X";
                    break;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Width = (int)nudWidth.Value;
            Height = (int)nudHeight.Value;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnTopLeft_Click(object sender, EventArgs e)
        {
            ResizeDir = Maps.Direction8.UpLeft;
            RefreshResizeDir();
        }

        private void BtnTop_Click(object sender, EventArgs e)
        {
            ResizeDir = Maps.Direction8.Up;
            RefreshResizeDir();
        }

        private void BtnTopRight_Click(object sender, EventArgs e)
        {
            ResizeDir = Maps.Direction8.UpRight;
            RefreshResizeDir();
        }

        private void BtnLeft_Click(object sender, EventArgs e)
        {
            ResizeDir = Maps.Direction8.Left;
            RefreshResizeDir();
        }

        private void BtnCenter_Click(object sender, EventArgs e)
        {
            ResizeDir = Maps.Direction8.None;
            RefreshResizeDir();
        }

        private void BtnRight_Click(object sender, EventArgs e)
        {
            ResizeDir = Maps.Direction8.Right;
            RefreshResizeDir();
        }

        private void BtnBottomLeft_Click(object sender, EventArgs e)
        {
            ResizeDir = Maps.Direction8.DownLeft;
            RefreshResizeDir();
        }

        private void BtnBottom_Click(object sender, EventArgs e)
        {
            ResizeDir = Maps.Direction8.Down;
            RefreshResizeDir();
        }

        private void BtnBottomRight_Click(object sender, EventArgs e)
        {
            ResizeDir = Maps.Direction8.DownRight;
            RefreshResizeDir();
        }
    }
}