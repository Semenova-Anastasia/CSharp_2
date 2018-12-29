using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGame
{
    /// <summary>
    /// Класс кораблей.
    /// </summary>
    class Starship : BaseObject
    {
        protected Bitmap ship = Properties.Resources.Starship;

        public Starship(Point pos, Point dir, int size) : base(pos, dir, size)
        {
        }

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(ship, Pos.X, Pos.Y, Size.Width, Size.Height);
        }

        public override void Update()
        {
            Pos.Y = Pos.Y + Dir.Y;
            if (Pos.Y < 30)
            {
                Dir.Y = 10;
            }
            else if (Pos.Y > Game.Height - (30 + Size.Height))
            {
                Dir.Y = -10;
            }
        }
    }
}
