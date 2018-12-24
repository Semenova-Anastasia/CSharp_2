using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGame
{
    class Asteroid: BaseObject
    {
        protected Bitmap sRock = Properties.Resources.SpaceRock;

        public Asteroid(Point pos, Point dir, int size) : base(pos, dir, size)
        {
        }

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(sRock, Pos.X, Pos.Y, Size.Width, Size.Height);
        }
        
        public override void Update()
        {
            Pos.X = Pos.X + Dir.X;
            if (Pos.X < -48)
            {
                Pos.X = Game.Width + Size.Width;
                Pos.Y = Game.rnd.Next(0, Game.Height);
            }
        }
    }
}
