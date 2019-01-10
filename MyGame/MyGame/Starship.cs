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
        public int Energy { get; private set; } = 100;
        public int Score { get; private set; } = 0;

        public static event Message MessageDie;

        public Starship(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
        }
        /// <summary>
        /// Изменение энергии корабля.
        /// </summary>
        /// <param name="n">Количество энергии.</param>
        public void EnergyChange(int n)
        {
            Energy -= n;
            if (Energy > 100) Energy = 100;
        }

        public void ScoreChange(int n)
        {
            Score += (n*10);
        }

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(ship, Pos.X, Pos.Y, Size.Width, Size.Height);
        }

        public override void Update()
        {
            //Pos.Y = Pos.Y + Dir.Y;
            //if (Pos.Y < 30)
            //{
            //    Dir.Y = 10;
            //}
            //else if (Pos.Y > Game.Height - (30 + Size.Height))
            //{
            //    Dir.Y = -10;
            //}
        }

        public void Up()
        {
            if (Pos.Y > 0) Pos.Y = Pos.Y - Dir.Y;
        }

        public void Down()
        {
            if (Pos.Y < Game.Height - (40 + Size.Height)) Pos.Y = Pos.Y + Dir.Y;
        }

        public void Die()
        {
            MessageDie?.Invoke();
        }
    }
}
