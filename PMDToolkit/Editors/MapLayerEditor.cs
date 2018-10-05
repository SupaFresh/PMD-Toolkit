using PMDToolkit.Logic.Gameplay;
using System;
using System.Windows.Forms;

namespace PMDToolkit.Editors
{
    public partial class MapLayerEditor : Form
    {
        public MapLayerEditor()
        {
            InitializeComponent();

            LoadLayers();
        }

        public void LoadLayers()
        {
            chklbLayers.Items.Clear();
            for (int i = Processor.CurrentMap.FringeLayers.Count - 1; i >= 0; i--)
            {
                chklbLayers.Items.Add("[Fringe] " + Processor.CurrentMap.FringeLayers[i].Name, MapEditor.showFringeLayer[i]);
            }

            for (int i = Processor.CurrentMap.PropFrontLayers.Count - 1; i >= 0; i--)
            {
                chklbLayers.Items.Add("[Front] " + Processor.CurrentMap.PropFrontLayers[i].Name, MapEditor.showPropFrontLayer[i]);
            }

            for (int i = Processor.CurrentMap.PropBackLayers.Count - 1; i >= 0; i--)
            {
                chklbLayers.Items.Add("[Back] " + Processor.CurrentMap.PropBackLayers[i].Name, MapEditor.showPropBackLayer[i]);
            }

            for (int i = Processor.CurrentMap.GroundLayers.Count - 1; i >= 0; i--)
            {
                chklbLayers.Items.Add("[Ground] " + Processor.CurrentMap.GroundLayers[i].Name, MapEditor.showGroundLayer[i]);
            }

            //chklbLayers.Items.Add("[Data]", MapEditor.showDataLayer);

            chklbLayers.SelectedIndex = 0;
        }

        private void MapLayerEditor_Load(object sender, EventArgs e)
        {
        }

        private void MapLayerEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainPanel.CurrentMapLayerEditor = null;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
        }

        private void ChklbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetLayerInfoFromIndex(chklbLayers.SelectedIndex, ref MapEditor.chosenEditLayer, ref MapEditor.chosenLayer);
        }

        private void GetLayerInfoFromIndex(int index, ref MapEditor.EditLayer layerType, ref int layer)
        {
            if (Processor.CurrentMap.FringeLayers.Count > index)
            {
                layerType = MapEditor.EditLayer.Fringe;
                layer = Processor.CurrentMap.FringeLayers.Count - index - 1;
                return;
            }
            index -= Processor.CurrentMap.FringeLayers.Count;

            if (Processor.CurrentMap.PropFrontLayers.Count > index)
            {
                layerType = MapEditor.EditLayer.PropFront;
                layer = Processor.CurrentMap.PropFrontLayers.Count - index - 1;
                return;
            }
            index -= Processor.CurrentMap.PropFrontLayers.Count;

            if (Processor.CurrentMap.PropBackLayers.Count > index)
            {
                layerType = MapEditor.EditLayer.PropBack;
                layer = Processor.CurrentMap.PropBackLayers.Count - index - 1;
                return;
            }
            index -= Processor.CurrentMap.PropBackLayers.Count;

            if (Processor.CurrentMap.GroundLayers.Count > index)
            {
                layerType = MapEditor.EditLayer.Ground;
                layer = Processor.CurrentMap.GroundLayers.Count - index - 1;
                return;
            }

            layerType = MapEditor.EditLayer.Data;
            layer = 0;
        }

        private int GetIndexFromLayerInfo(MapEditor.EditLayer layerType, int layer)
        {
            int layerIndex = 0;
            if (layerType == MapEditor.EditLayer.Fringe)
                return layerIndex + Processor.CurrentMap.FringeLayers.Count - 1 - layer;

            layerIndex += Processor.CurrentMap.FringeLayers.Count;
            if (layerType == MapEditor.EditLayer.PropFront)
                return layerIndex + Processor.CurrentMap.PropFrontLayers.Count - 1 - layer;

            layerIndex += Processor.CurrentMap.PropFrontLayers.Count;
            if (layerType == MapEditor.EditLayer.PropBack)
                return layerIndex + Processor.CurrentMap.PropBackLayers.Count - 1 - layer;

            layerIndex += Processor.CurrentMap.PropBackLayers.Count;
            if (layerType == MapEditor.EditLayer.Ground)
                return layerIndex + Processor.CurrentMap.GroundLayers.Count - 1 - layer;

            layerIndex += Processor.CurrentMap.GroundLayers.Count;
            if (layerType == MapEditor.EditLayer.Data)
                return layerIndex;

            return -1;
        }

        private void ChklbLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            MapEditor.EditLayer layerType = MapEditor.EditLayer.Data;
            int layerNum = -1;
            GetLayerInfoFromIndex(e.Index, ref layerType, ref layerNum);

            switch (layerType)
            {
                case MapEditor.EditLayer.Data:
                    {
                        MapEditor.showDataLayer = (e.NewValue == CheckState.Checked);
                        break;
                    }
                case MapEditor.EditLayer.Ground:
                    {
                        MapEditor.showGroundLayer[layerNum] = (e.NewValue == CheckState.Checked);
                        break;
                    }
                case MapEditor.EditLayer.PropBack:
                    {
                        MapEditor.showPropBackLayer[layerNum] = (e.NewValue == CheckState.Checked);
                        break;
                    }
                case MapEditor.EditLayer.PropFront:
                    {
                        MapEditor.showPropFrontLayer[layerNum] = (e.NewValue == CheckState.Checked);
                        break;
                    }
                case MapEditor.EditLayer.Fringe:
                    {
                        MapEditor.showFringeLayer[layerNum] = (e.NewValue == CheckState.Checked);
                        break;
                    }
            }
        }
    }
}