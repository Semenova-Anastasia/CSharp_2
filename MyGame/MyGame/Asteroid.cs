using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGame
{
    /// <summary>
    /// Класс астероидов.
    /// </summary>
    class Asteroid : BaseObject
    {
        protected Bitmap sRock = Properties.Resources.SpaceRock;
        public int Power { get; set; }

        public Asteroid(Point pos, Point dir, int size) : base(pos, dir, size)
        {
            Power = 1;
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
                Pos.Y = Game.rnd.Next(0, Game.Height - 10);
            }
        }

        public override void Regenerate()
        {
            base.Regenerate();
            Pos.X = Game.Width - Size.Width;
        }
    }
}
