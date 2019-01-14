using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGame
{
    /// <summary>
    /// Класс снарядов.
    /// </summary>
    class Bullet : BaseObject
    {
        public bool Away { get; private set; }
        public Bullet(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            Away = false;
        }

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawRectangle(Pens.OrangeRed, Pos.X, Pos.Y, Size.Width, Size.Height);
        }

        public override void Update()
        {
            Pos.X = Pos.X + 10;
            if (Pos.X > Game.Width) Away = true;
        }
    }
}
