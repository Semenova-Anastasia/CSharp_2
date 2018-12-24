using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyGame
{
    static class Game
    {
        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;
        public static Bitmap bg;

        public static int Width { get; set; }
        public static int Height { get; set; }

        public static Random rnd;

        public static BaseObject[] _asts;
        public static BaseObject[] _stars;
        public static Starship ship;

        static Game()
        {
        }

        public static void Init(Form form)
        {
            Graphics g;

            _context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();

            rnd = new Random();
        
            Width = form.ClientSize.Width;
            Height = form.ClientSize.Height;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));
            Load();

            Timer timer = new Timer { Interval = 100 };
            timer.Start();
            timer.Tick += Timer_Tick;
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Draw();
            Update();
        }

        public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            bg = Properties.Resources.Universe;
            Buffer.Graphics.DrawImage(bg, new Rectangle(0, 0, Width, Height));
            foreach (BaseObject obj in _asts)
                obj.Draw();
            foreach (BaseObject obj in _stars)
                obj.Draw();
            ship.Draw();
            Buffer.Render();
        }

        public static void Load()
        {
            _asts = new BaseObject[10];
            _stars = new BaseObject[50];
            for (int i = 0; i < _asts.Length; i++)
                _asts[i] = new Asteroid(new Point(rnd.Next(0, Width), rnd.Next(0, Height)), new Point(rnd.Next(-15, -10), 0), rnd.Next(8, 48));
            for (int i = 0; i < _stars.Length; i++)
                _stars[i] = new Star(new Point(rnd.Next(0, Width), rnd.Next(0, Height)), new Point(rnd.Next(-4, -1), 0), rnd.Next(4, 16));
            ship = new Starship(new Point(100, Height / 2 - 50), new Point(0, 10), 100);
        }

        public static void Update()
        {
            foreach (BaseObject obj in _asts)
                obj.Update();
            foreach (BaseObject obj in _stars)
                obj.Update();
            ship.Update();
        }
    }
}
