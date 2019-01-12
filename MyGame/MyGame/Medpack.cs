using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGame
{
    class Medpack : BaseObject
    {
        protected Bitmap mPack = Properties.Resources.Medpack;
        public int Power { get; private set; }

        public Medpack(Point pos, Point dir, int size) : base(pos, dir, size)
        {
            Power = -size;
        }

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(mPack, Pos.X, Pos.Y, Size.Width, Size.Height);
        }

        public override void Update()
        {
            Pos.X = Pos.X + Dir.X;
            if (Pos.X < -Size.Width)
            {
                Pos.X = Game.Width + Size.Width;
                Pos.Y = Game.rnd.Next(0, Game.Height - 40 + Size.Height);
            }
        }

        public override void Regenerate()
        {
            base.Regenerate();
            Pos.X = Game.Width * 2;
        }
    }
}
