using PMDToolkit.Core;
using PMDToolkit.Data;
using PMDToolkit.Graphics;
using PMDToolkit.Logic.Gameplay;
using PMDToolkit.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace PMDToolkit.Editors
{
    public partial class MapEditor : Form
    {
        private object drawLock;
        private bool runningAnim;
        private volatile int animationTimer;
        private Thread animThread;
        private int animFrame;

        private static List<Image> tiles;

        public static int currentTileset;
        public static bool mapEditing;

        private static string fileName;

        public enum EditLayer
        {
            Data = 0,
            Ground = 1,
            PropBack = 2,
            PropFront = 3,
            Fringe = 4
        }

        public static bool showDataLayer;
        public static List<bool> showGroundLayer;
        public static List<bool> showPropBackLayer;
        public static List<bool> showPropFrontLayer;
        public static List<bool> showFringeLayer;

        public static EditLayer chosenEditLayer;
        public static int chosenLayer;

        public enum TileEditMode
        {
            Draw = 0,
            Fill = 1,
            Eyedrop = 2
        }

        public static TileEditMode chosenEditMode;
        private static bool inAnimMode;
        private static Loc2D chosenTile;
        private static int chosenTileset;
        private static TileAnim chosenAnim;

        public delegate void RefreshCallback();

        public MapEditor()
        {
            drawLock = new object();
            inAnimMode = false;
            chosenTile = new Loc2D();
            chosenTileset = 0;
            chosenAnim = new TileAnim();
            SetupLayerVisibility();

            InitializeComponent();

            if (tiles == null)
            {
                tiles = new List<Image>();
                for (int i = 0; i < 10; i++)
                {
                    tiles.Add(new Bitmap(Paths.TilesPath + "Tiles" + i + ".png"));
                }
            }

            RefreshTitle();

            if (!Directory.Exists(Paths.MapPath))
                Directory.CreateDirectory(Paths.MapPath);
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + Paths.MapPath;
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + Paths.MapPath;

            tbTileset.Value = 0;

            nudTimeLimit.Maximum = int.MaxValue;

            for (int i = 0; i < 12; i++)
            {
                cbWeather.Items.Add(((Enums.Weather)i).ToString());
            }

            nudDarkness.Maximum = int.MaxValue;

            nudFrameLength.Maximum = int.MaxValue;

            RefreshTileset();
            RefreshAnimControls();

            ReloadMusic();

            LoadMapProperties();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Game.UpdateLoadMsg("Loading Map...");
            MainPanel.EnterLoadPhase(Game.GameLoadState.Loading);

            Processor.StartMap("", Processor.Seed);
            Logic.Display.Screen.ForceReady();
            RefreshTitle();
            LoadMapProperties();
            SetupLayerVisibility();
            if (MainPanel.CurrentMapLayerEditor != null)
                MainPanel.CurrentMapLayerEditor.LoadLayers();

            MainPanel.EnterLoadPhase(Game.GameLoadState.Loaded);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Game.UpdateLoadMsg("Loading Map...");
                MainPanel.EnterLoadPhase(Game.GameLoadState.Loading);

                fileName = openFileDialog.FileName;
                Processor.StartMap(fileName, Processor.Seed);
                Logic.Display.Screen.ForceReady();
                RefreshTitle();
                LoadMapProperties();
                SetupLayerVisibility();
                if (MainPanel.CurrentMapLayerEditor != null)
                    MainPanel.CurrentMapLayerEditor.LoadLayers();

                MainPanel.EnterLoadPhase(Game.GameLoadState.Loaded);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;
                BasicMap map = Processor.CurrentMap;

                using (FileStream fileStream = File.Create(fileName))
                {
                    using (BinaryWriter writer = new BinaryWriter(fileStream))
                    {
                        map.Save(writer);
                    }
                }
                RefreshTitle();
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName == "")
            {
                DialogResult result = saveFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    fileName = saveFileDialog.FileName;
                    BasicMap map = Processor.CurrentMap;

                    using (FileStream fileStream = File.Create(fileName))
                    {
                        using (BinaryWriter writer = new BinaryWriter(fileStream))
                        {
                            map.Save(writer);
                        }
                    }
                }
                RefreshTitle();
            }
            else
            {
                BasicMap map = Processor.CurrentMap;

                using (FileStream fileStream = File.Create(fileName))
                {
                    using (BinaryWriter writer = new BinaryWriter(fileStream))
                    {
                        map.Save(writer);
                    }
                }
            }
        }

        private void RefreshTitle()
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "";

            if (fileName == "")
                Text = "New Map";
            else
            {
                string[] fileEnd = fileName.Split('\\');
                Text = fileEnd[fileEnd.Length - 1];
            }
        }

        private void RefreshTileset()
        {
            lblTileset.Text = "Tileset: " + tbTileset.Value;

            RefreshScrollMaximums();

            RefreshPic();
        }

        private void RefreshPic()
        {
            int picX = picTileset.Size.Width / TextureManager.TILE_SIZE;
            int picY = picTileset.Size.Height / TextureManager.TILE_SIZE;

            TextureManager.GetTilesetTileDimensions(currentTileset, out int tileX, out int tileY);

            UpdatePreviewTile(false);

            Image endImage = new Bitmap(picTileset.Width, picTileset.Height);
            using (var graphics = System.Drawing.Graphics.FromImage(endImage))
            {
                int width = picX;
                if (tileX - hScroll.Value < width)
                    width = tileX - hScroll.Value;

                int height = picY;
                if (tileY - vScroll.Value < height)
                    height = tileY - vScroll.Value;

                lock (drawLock)
                {
                    graphics.DrawImage(tiles[currentTileset], 0, 0,
                        new Rectangle(hScroll.Value * TextureManager.TILE_SIZE, vScroll.Value * TextureManager.TILE_SIZE, width * TextureManager.TILE_SIZE, height * TextureManager.TILE_SIZE), GraphicsUnit.Pixel);
                }

                //draw red square
                if (currentTileset == chosenTileset &&
                    chosenTile.X >= hScroll.Value && chosenTile.X < hScroll.Value + width &&
                    chosenTile.Y >= vScroll.Value && chosenTile.Y < vScroll.Value + height)
                {
                    graphics.DrawRectangle(new Pen(Color.Red, 2), new Rectangle((chosenTile.X - hScroll.Value) * TextureManager.TILE_SIZE + 1,
                        (chosenTile.Y - vScroll.Value) * TextureManager.TILE_SIZE + 1,
                        TextureManager.TILE_SIZE - 2, TextureManager.TILE_SIZE - 2));
                }
            }
            picTileset.Image = endImage;
        }

        private void RefreshTileSelect()
        {
            chkAnimationMode.Checked = inAnimMode;
            tbTileset.Value = currentTileset;

            lblTileset.Text = "Tileset: " + tbTileset.Value;

            RefreshScrollMaximums();

            int picX = picTileset.Size.Width / TextureManager.TILE_SIZE;
            int picY = picTileset.Size.Height / TextureManager.TILE_SIZE;

            bool refreshedPic = false;

            //check if selected tile is within current scroll view
            if (chosenTile.X < hScroll.Value || chosenTile.X >= hScroll.Value + picX)
            {
                //if not, move it within scroll view
                int newVal = chosenTile.X - picX / 2;
                refreshedPic |= (hScroll.Value == newVal);
                hScroll.Value = Math.Max(newVal, 0);
            }

            //check if selected tile is within current scroll view
            if (chosenTile.Y < vScroll.Value || chosenTile.Y >= vScroll.Value + picY)
            {
                //if not, move it within scroll view
                int newVal = chosenTile.Y - picY / 2;
                refreshedPic |= (vScroll.Value == newVal);
                vScroll.Value = Math.Max(newVal, 0);
            }

            if (!refreshedPic)
                RefreshPic();
        }

        private void RefreshScrollMaximums()
        {
            int picX = picTileset.Size.Width / TextureManager.TILE_SIZE;
            int picY = picTileset.Size.Height / TextureManager.TILE_SIZE;

            TextureManager.GetTilesetTileDimensions(currentTileset, out int tileX, out int tileY);

            if (tileX - picX <= 0)
            {
                hScroll.Maximum = 0;
                hScroll.Enabled = false;
            }
            else
            {
                hScroll.Maximum = tileX - picX;
            }

            if (tileY - picY <= 0)
            {
                vScroll.Maximum = 0;
                vScroll.Enabled = false;
            }
            else
            {
                vScroll.Maximum = tileY - picY;
            }
        }

        private void ReloadMusic()
        {
            lbxMusic.Items.Clear();

            string[] files = Directory.GetFiles(Paths.MusicPath, "*.ogg", SearchOption.TopDirectoryOnly);

            lbxMusic.Items.Add("None");
            lbxMusic.SelectedIndex = 0;
            for (int i = 0; i < files.Length; i++)
            {
                string song = files[i].Substring((Paths.MusicPath).Length);
                lbxMusic.Items.Add(song);
            }
        }

        private void LoadMapProperties()
        {
            txtMapName.Text = Processor.CurrentMap.Title;

            nudTimeLimit.Value = Processor.CurrentMap.TimeLimit;

            cbWeather.SelectedIndex = (int)Processor.CurrentMap.Weather;

            nudDarkness.Value = Processor.CurrentMap.Darkness;

            chkIndoors.Checked = Processor.CurrentMap.Indoors;

            for (int i = 0; i < lbxMusic.Items.Count; i++)
            {
                string song = (string)lbxMusic.Items[i];
                if (song == Processor.CurrentMap.Music)
                {
                    lbxMusic.SelectedIndex = i;
                    break;
                }
            }
        }

        private void SetupLayerVisibility()
        {
            showDataLayer = false;
            showGroundLayer = new List<bool>();
            for (int i = 0; i < Processor.CurrentMap.GroundLayers.Count; i++)
            {
                showGroundLayer.Add(true);
            }
            showPropBackLayer = new List<bool>();
            for (int i = 0; i < Processor.CurrentMap.PropBackLayers.Count; i++)
            {
                showPropBackLayer.Add(true);
            }
            showPropFrontLayer = new List<bool>();
            for (int i = 0; i < Processor.CurrentMap.PropFrontLayers.Count; i++)
            {
                showPropFrontLayer.Add(true);
            }
            showFringeLayer = new List<bool>();
            for (int i = 0; i < Processor.CurrentMap.FringeLayers.Count; i++)
            {
                showFringeLayer.Add(true);
            }
        }

        private void VScroll_Scroll(object sender, ScrollEventArgs e)
        {
            RefreshPic();
        }

        private void HScroll_Scroll(object sender, ScrollEventArgs e)
        {
            RefreshPic();
        }

        private void MapEditor_Load(object sender, EventArgs e)
        {
            mapEditing = true;
        }

        private void MapEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            //end animation thread
            if (animThread != null)
            {
                lock (drawLock)
                    runningAnim = false;
            }
        }

        private void MapEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainPanel.CurrentMapEditor = null;
            if (MainPanel.CurrentMapLayerEditor != null)
            {
                MainPanel.CurrentMapLayerEditor.Close();
            }
            MainPanel.CurrentMapLayerEditor = null;

            mapEditing = false;
        }

        private void PicTileset_Click(object sender, EventArgs e)
        {
            MouseEventArgs args = e as MouseEventArgs;

            TextureManager.GetTilesetTileDimensions(currentTileset, out int tileX, out int tileY);

            int clickedX = args.X / TextureManager.TILE_SIZE + hScroll.Value;
            int clickedY = args.Y / TextureManager.TILE_SIZE + vScroll.Value;

            if (clickedX < tileX && clickedY < tileY)
            {
                chosenTileset = currentTileset;
                chosenTile = new Loc2D(clickedX, clickedY);
                RefreshPic();

                if (inAnimMode)
                {
                    if (lbxFrames.SelectedIndex > -1)
                        chosenAnim.Frames[lbxFrames.SelectedIndex] = new TileTexture(chosenTile, chosenTileset);
                    else
                        chosenAnim.Frames.Add(new TileTexture(chosenTile, chosenTileset));

                    UpdateAnimFrames();
                }
            }
        }

        private void TbTileset_Scroll(object sender, EventArgs e)
        {
            if (tbTileset.Value == currentTileset)
                return;

            currentTileset = tbTileset.Value;
            RefreshTileset();
        }

        private void BtnReloadSongs_Click(object sender, EventArgs e)
        {
            ReloadMusic();
        }

        private void TxtMapName_TextChanged(object sender, EventArgs e)
        {
            Processor.CurrentMap.Title = txtMapName.Text;
        }

        private void NudTimeLimit_TextChanged(object sender, EventArgs e)
        {
            SetIntFromNumeric(ref Processor.CurrentMap.TimeLimit, nudTimeLimit);
        }

        private void SetIntFromNumeric(ref int setInt, IntNumericUpDown nud)
        {
            int value = nud.Text.ToInt();
            if (value >= nud.Minimum && value <= nud.Maximum)
                setInt = nud.Text.ToInt();
            else
                setInt = (int)nud.Value;
        }

        private void CbWeather_SelectedIndexChanged(object sender, EventArgs e)
        {
            Processor.CurrentMap.Weather = (Enums.Weather)cbWeather.SelectedIndex;
        }

        private void NudDarkness_TextChanged(object sender, EventArgs e)
        {
            SetIntFromNumeric(ref Processor.CurrentMap.Darkness, nudDarkness);
        }

        private void ChkIndoors_CheckedChanged(object sender, EventArgs e)
        {
            Processor.CurrentMap.Indoors = chkIndoors.Checked;
        }

        private void LbxMusic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxMusic.SelectedIndex <= 0)
            {
                Processor.CurrentMap.Music = "";
            }
            else
            {
                string fileName = (string)lbxMusic.Items[lbxMusic.SelectedIndex];
                Processor.CurrentMap.Music = fileName;
            }
            MainPanel.GameNeedWait = true;
            while (!MainPanel.GameWaiting)
                Thread.Sleep(100);

            Logic.Display.Screen.AddResult(new Logic.Results.BGM(Logic.Gameplay.Processor.CurrentMap.Music, true));

            MainPanel.GameNeedWait = false;
            while (MainPanel.GameWaiting)
                Thread.Sleep(100);
        }

        private void ResizeMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapResizeWindow window = new MapResizeWindow(Processor.CurrentMap.Width, Processor.CurrentMap.Height);

            if (window.ShowDialog() == DialogResult.OK)
            {
                Game.UpdateLoadMsg("Resizing Map...");
                MainPanel.EnterLoadPhase(Game.GameLoadState.Loading);

                Loc2D diff = Operations.GetResizeOffset(Processor.CurrentMap.Width, Processor.CurrentMap.Height, window.Width, window.Height, Operations.ReverseDir(window.ResizeDir));
                Processor.FocusedCharacter.CharLoc += diff;

                if (Processor.FocusedCharacter.CharLoc.X < 0)
                    Processor.FocusedCharacter.CharLoc.X = 0;
                else if (Processor.FocusedCharacter.CharLoc.X >= window.Width)
                    Processor.FocusedCharacter.CharLoc.X = window.Width - 1;
                if (Processor.FocusedCharacter.CharLoc.Y < 0)
                    Processor.FocusedCharacter.CharLoc.Y = 0;
                else if (Processor.FocusedCharacter.CharLoc.Y >= window.Height)
                    Processor.FocusedCharacter.CharLoc.Y = window.Height - 1;

                PMDToolkit.Logic.Display.Screen.AddResult(new PMDToolkit.Logic.Results.Loc(Processor.FocusedCharIndex, Processor.FocusedCharacter.CharLoc));

                Processor.CurrentMap.Resize(window.Width, window.Height, Operations.ReverseDir(window.ResizeDir));
                PMDToolkit.Logic.Display.Screen.AddResult(new PMDToolkit.Logic.Results.SetMap(Processor.CurrentMap, 0));

                MainPanel.EnterLoadPhase(Game.GameLoadState.Loaded);
            }
        }

        private void LayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainPanel.CurrentMapLayerEditor == null)
            {
                MainPanel.CurrentMapLayerEditor = new MapLayerEditor();
            }
            MainPanel.CurrentMapLayerEditor.Show();
        }

        private void ChkTexEyeDropper_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTexEyeDropper.Checked)
            {
                chkFill.Checked = false;
                chosenEditMode = TileEditMode.Eyedrop;
            }
            else
            {
                chosenEditMode = TileEditMode.Draw;
            }
        }

        private void ChkFill_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFill.Checked)
            {
                chkTexEyeDropper.Checked = false;
                chosenEditMode = TileEditMode.Fill;
            }
            else
            {
                chosenEditMode = TileEditMode.Draw;
            }
        }

        private static MapLayer GetChosenEditLayer()
        {
            switch (chosenEditLayer)
            {
                case EditLayer.Data:
                    return null;

                case EditLayer.Ground:
                    return Processor.CurrentMap.GroundLayers[chosenLayer];

                case EditLayer.PropBack:
                    return Processor.CurrentMap.PropBackLayers[chosenLayer];

                case EditLayer.PropFront:
                    return Processor.CurrentMap.PropFrontLayers[chosenLayer];

                case EditLayer.Fringe:
                    return Processor.CurrentMap.FringeLayers[chosenLayer];
            }
            return null;
        }

        public static void PaintTile(Loc2D loc, TileAnim anim)
        {
            if (!Operations.IsInBound(Processor.CurrentMap.Width, Processor.CurrentMap.Height, loc.X, loc.Y))
                return;

            switch (chosenEditLayer)
            {
                case EditLayer.Data:
                    {
                        break;
                    }
                case EditLayer.Ground:
                case EditLayer.PropBack:
                case EditLayer.PropFront:
                case EditLayer.Fringe:
                    {
                        GetChosenEditLayer().Tiles[loc.X, loc.Y] = anim;
                        break;
                    }
            }

            Logic.Display.Screen.AddResult(new Logic.Results.SetTile(Processor.CurrentMap, loc));
        }

        public static void EyedropTile(Loc2D loc)
        {
            if (!Operations.IsInBound(Processor.CurrentMap.Width, Processor.CurrentMap.Height, loc.X, loc.Y))
                return;

            switch (chosenEditLayer)
            {
                case EditLayer.Data:
                    {
                        break;
                    }
                case EditLayer.Ground:
                case EditLayer.PropBack:
                case EditLayer.PropFront:
                case EditLayer.Fringe:
                    {
                        GetTileTexFromTileAnim(GetChosenEditLayer().Tiles[loc.X, loc.Y]);
                        break;
                    }
            }
        }

        public static TileAnim GetBrush()
        {
            TileAnim anim;
            if (inAnimMode)
                anim = new TileAnim(chosenAnim);
            else
                anim = new TileAnim(chosenTile, chosenTileset);
            return anim;
        }

        public static void FillTile(Loc2D loc, TileAnim anim)
        {
            if (!Operations.IsInBound(Processor.CurrentMap.Width, Processor.CurrentMap.Height, loc.X, loc.Y))
                return;

            switch (chosenEditLayer)
            {
                case EditLayer.Data:
                    {
                        break;
                    }
                case EditLayer.Ground:
                case EditLayer.PropBack:
                case EditLayer.PropFront:
                case EditLayer.Fringe:
                    {
                        TileAnim oldAnim = GetChosenEditLayer().Tiles[loc.X, loc.Y];

                        if (oldAnim != anim)
                        {
                            Operations.FillArray(GetChosenEditLayer().Tiles.GetLength(0), GetChosenEditLayer().Tiles.GetLength(1),
                                (int x, int y) =>
                                {
                                    return GetChosenEditLayer().Tiles[x, y] == oldAnim;
                                },
                                (int x, int y) =>
                                {
                                    GetChosenEditLayer().Tiles[x, y] = new TileAnim(anim);
                                    Logic.Display.Screen.AddResult(new Logic.Results.SetTile(Processor.CurrentMap, new Loc2D(x, y)));
                                },
                                loc);
                        }
                        break;
                    }
            }
        }

        private static void GetTileTexFromTileAnim(TileAnim anim)
        {
            if (anim.Frames.Count == 1)
            {
                inAnimMode = false;
                chosenTileset = anim.Frames[0].Sheet;
                chosenTile = anim.Frames[0].Texture;

                currentTileset = chosenTileset;

                //refresh
                RefreshCallback refresh = new RefreshCallback(MainPanel.CurrentMapEditor.RefreshTileSelect);
                MainPanel.CurrentMapEditor.Invoke(refresh);
            }
            else if (anim.Frames.Count > 1)
            {
                inAnimMode = true;

                chosenAnim = new TileAnim(anim);

                //refresh
                RefreshCallback refresh = new RefreshCallback(MainPanel.CurrentMapEditor.RefreshTileSelect);
                MainPanel.CurrentMapEditor.Invoke(refresh);
            }
        }

        private void ChkAnimationMode_CheckedChanged(object sender, EventArgs e)
        {
            if (inAnimMode != chkAnimationMode.Checked)
            {
                inAnimMode = chkAnimationMode.Checked;
                if (inAnimMode)
                {
                    chosenAnim = new TileAnim(chosenTile, chosenTileset);
                }
                else
                {
                    if (chosenAnim.Frames.Count > 0)
                    {
                        chosenTile = chosenAnim.Frames[0].Texture;
                        chosenTileset = chosenAnim.Frames[0].Sheet;
                    }
                    else
                    {
                        chosenTile = new Loc2D();
                        chosenTileset = 0;
                    }
                    currentTileset = chosenTileset;
                }
            }
            UpdateAnimFrames();
            RefreshTileSelect();

            RefreshAnimControls();
        }

        private void RefreshAnimControls()
        {
            bool show = inAnimMode;

            lblFrameLength.Visible = show;
            nudFrameLength.Value = chosenAnim.FrameLength.ToMillisecs();
            nudFrameLength.Visible = show;
            lblFrames.Visible = show;
            lbxFrames.Visible = show;
            btnAddFrame.Visible = show;
            btnRemoveFrame.Visible = show;
        }

        private void ChangeAnimationTimer()
        {
            lock (drawLock)
            {
                if (inAnimMode)
                {
                    if (animThread == null)
                    {
                        runningAnim = true;
                        animThread = new Thread(UpdatePreviewTileTimer);
                        animThread.Start();
                    }
                    animationTimer = chosenAnim.FrameLength.ToMillisecs();
                }
            }
        }

        private void UpdatePreviewTileTimer()
        {
            while (true)
            {
                lock (drawLock)
                {
                    if (!runningAnim)
                        return;

                    if (inAnimMode)
                    {
                        animFrame++;
                        if (animFrame >= chosenAnim.Frames.Count)
                            animFrame = 0;

                        UpdatePreviewTile(true);
                    }
                }
                for (int i = 0; i < animationTimer; i++)
                {
                    Thread.Sleep(1);
                }
            }
        }

        private void UpdatePreviewTile(bool timer)
        {
            if (inAnimMode)
            {
                if (timer)
                {
                    Image endTileImage = new Bitmap(picTileset.Width, picTileset.Height);
                    using (var graphics = System.Drawing.Graphics.FromImage(endTileImage))
                    {
                        TileTexture tex = chosenAnim.Frames[animFrame];
                        graphics.DrawImage(tiles[tex.Sheet], 0, 0,
                            new Rectangle(tex.Texture.X * Graphics.TextureManager.TILE_SIZE,
                                tex.Texture.Y * Graphics.TextureManager.TILE_SIZE,
                                Graphics.TextureManager.TILE_SIZE, Graphics.TextureManager.TILE_SIZE), GraphicsUnit.Pixel);
                    }
                    picTile.Image = endTileImage;
                }
                else
                {
                    lblTileInfo.Text = "[Animation]";
                }
            }
            else
            {
                Image endTileImage = new Bitmap(picTileset.Width, picTileset.Height);
                lock (drawLock)
                {
                    using (var graphics = System.Drawing.Graphics.FromImage(endTileImage))
                    {
                        graphics.DrawImage(tiles[chosenTileset], 0, 0,
                            new Rectangle(chosenTile.X * Graphics.TextureManager.TILE_SIZE,
                                chosenTile.Y * Graphics.TextureManager.TILE_SIZE,
                                Graphics.TextureManager.TILE_SIZE, Graphics.TextureManager.TILE_SIZE), GraphicsUnit.Pixel);
                    }
                    picTile.Image = endTileImage;
                }
                lblTileInfo.Text = "Tile" + chosenTileset + " X" + chosenTile.X + " Y" + chosenTile.Y;
            }
        }

        private void UpdateAnimFrames()
        {
            int selection = lbxFrames.SelectedIndex;
            lbxFrames.Items.Clear();
            for (int i = 0; i < chosenAnim.Frames.Count; i++)
            {
                lbxFrames.Items.Add("Tile" + chosenAnim.Frames[i].Sheet + " X" + chosenAnim.Frames[i].Texture.X + " Y" + chosenAnim.Frames[i].Texture.Y);
            }
            if (selection < lbxFrames.Items.Count)
                lbxFrames.SelectedIndex = selection;

            ChangeAnimationTimer();
        }

        private void NudFrameLength_TextChanged(object sender, EventArgs e)
        {
            int millisecs = 0;
            SetIntFromNumeric(ref millisecs, nudFrameLength);
            chosenAnim.FrameLength = RenderTime.FromMillisecs(millisecs);
            ChangeAnimationTimer();
        }

        private void BtnAddFrame_Click(object sender, EventArgs e)
        {
            lock (drawLock)
            {
                if (lbxFrames.SelectedIndex > -1)
                    chosenAnim.Frames.Insert(lbxFrames.SelectedIndex, new TileTexture());
                else
                    chosenAnim.Frames.Add(new TileTexture());
            }
            UpdateAnimFrames();
        }

        private void BtnRemoveFrame_Click(object sender, EventArgs e)
        {
            lock (drawLock)
            {
                if (chosenAnim.Frames.Count > 1)
                {
                    if (lbxFrames.SelectedIndex > -1)
                        chosenAnim.Frames.RemoveAt(lbxFrames.SelectedIndex);
                    else
                        chosenAnim.Frames.RemoveAt(lbxFrames.Items.Count - 1);
                }
                if (animFrame > chosenAnim.Frames.Count)
                    animFrame = 0;
            }
            UpdateAnimFrames();
        }

        private void LbxFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxFrames.SelectedIndex > -1)
            {
                chosenTileset = chosenAnim.Frames[lbxFrames.SelectedIndex].Sheet;
                currentTileset = chosenTileset;
                chosenTile = chosenAnim.Frames[lbxFrames.SelectedIndex].Texture;
                RefreshTileSelect();
            }
        }

        private void BtnReloadTiles_Click(object sender, EventArgs e)
        {
            Game.UpdateLoadMsg("Reloading Tiles...");
            MainPanel.EnterLoadPhase(Game.GameLoadState.Loading);

            lock (drawLock)
            {
                tiles = new List<Image>();
                for (int i = 0; i < 10; i++)
                {
                    tiles.Add(new Bitmap(Paths.TilesPath + "Tiles" + i + ".png"));
                }
            }
            RefreshTileset();

            Conversion.CompileAllTiles(Paths.TilesPath, Paths.CachedGFXPath + "Tile");
            TextureManager.NeedTileReload = true;

            MainPanel.EnterLoadPhase(Game.GameLoadState.Loaded);
        }

        private void ClearLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapLayer layer = GetChosenEditLayer();
            layer = new MapLayer(layer.Name, layer.Tiles.GetLength(0), layer.Tiles.GetLength(1));
            Logic.Display.Screen.AddResult(new Logic.Results.SetMap(Processor.CurrentMap, 0));
        }
    }
}