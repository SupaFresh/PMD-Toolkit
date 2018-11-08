using OpenTK;
using PMDToolkit.Graphics;
using PMDToolkit.Maps;

namespace PMDToolkit.Logic.Display
{
    internal class DirectionalMoveAnimation : ISpellSprite
    {
        #region Constructors

        public DirectionalMoveAnimation(Loc2D tileLoc, int animIndex, RenderTime animTime, Direction8 dir, int loops)
        {
            AnimationIndex = animIndex;
            FrameLength = animTime;
            TotalLoops = loops;
            Direction = dir;
            StartLoc = new Loc2D(tileLoc.X, tileLoc.Y);
        }

        #endregion Constructors

        #region Properties

        public int AnimationIndex
        {
            get;
            set;
        }

        public RenderTime FrameTime
        {
            get;
            set;
        }

        public RenderTime FrameLength
        {
            get;
            set;
        }

        public int Frame
        {
            get;
            set;
        }

        //total frames

        public int Loops
        {
            get;
            set;
        }

        public int TotalLoops
        {
            get;
            set;
        }

        public Direction8 Direction
        {
            get;
            set;
        }

        public MoveAnimationType AnimType
        {
            get { return MoveAnimationType.Directional; }
        }

        public Loc2D StartLoc { get; set; }
        public Loc2D MapLoc { get { return new Loc2D(StartLoc.X * TextureManager.TILE_SIZE, StartLoc.Y * TextureManager.TILE_SIZE); } }
        public int MapHeight { get; set; }

        public RenderTime ActionTime { get; set; }
        public bool ActionDone { get; set; }

        #endregion Properties

        public virtual void Begin()
        {
        }

        public virtual void Process(RenderTime elapsedTime)
        {
            ActionTime += elapsedTime;
            FrameTime += elapsedTime;
            if (FrameTime >= FrameLength)
            {
                FrameTime = FrameTime - FrameLength;
                Frame++;
            }

            if (Frame >= TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Spell, AnimationIndex).TotalFrames)
            {
                Loops++;
                Frame = 0;
            }

            if (Loops >= TotalLoops)
            {
                ActionDone = true;
            }
        }

        public virtual void Draw()
        {
            if (!ActionDone)
            {
                TextureManager.TextureProgram.PushModelView();
                Loc2D drawLoc = GetStart();
                TextureManager.TextureProgram.LeftMultModelView(Matrix4.CreateTranslation(drawLoc.X, drawLoc.Y, 0));
                TextureManager.TextureProgram.UpdateModelView();

                AnimSheet sheet = TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Spell, AnimationIndex);
                sheet.RenderAnim(Frame, 0, 0);

                TextureManager.TextureProgram.PopModelView();
            }
        }

        public Loc2D GetStart()
        {
            return new Loc2D(MapLoc.X + TextureManager.TILE_SIZE / 2 - TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Spell, AnimationIndex).TileWidth / 2,
                MapLoc.Y + TextureManager.TILE_SIZE / 2 - TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Spell, AnimationIndex).TileHeight / 2 - MapHeight);
        }

        public Loc2D GetEnd()
        {
            return new Loc2D(MapLoc.X + TextureManager.TILE_SIZE / 2 + TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Spell, AnimationIndex).TileWidth / 2,
                MapLoc.Y + TextureManager.TILE_SIZE / 2 + TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Spell, AnimationIndex).TileHeight / 2 - MapHeight);
        }
    }
}

